terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# Random suffix to ensure unique names
resource "random_string" "suffix" {
  length  = 8
  special = false
  upper   = false
}

# Resource group
resource "azurerm_resource_group" "rg" {
  name     = "rg-sportsday-${random_string.suffix.result}"
  location = var.location
}

# SQL Server
resource "azurerm_mssql_server" "sql" {
  name                         = "sql-sportsday-${random_string.suffix.result}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_login
  administrator_login_password = var.sql_admin_password

  azuread_administrator {
    login_username = var.sql_aad_admin_login
    object_id      = var.sql_aad_admin_object_id
  }
}

# SQL Database
resource "azurerm_mssql_database" "db" {
  name           = "sqldb-sportsday"
  server_id      = azurerm_mssql_server.sql.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 2
  sku_name       = "Basic"
  zone_redundant = false
}

# SignalR Service
resource "azurerm_signalr_service" "signalr" {
  name                = "signalr-sportsday-${random_string.suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  sku {
    name     = "Free_F1"
    capacity = 1
  }

  cors {
    allowed_origins = ["*"]
  }

  features {
    flag  = "ServiceMode"
    value = "Default"
  }
}

# App Service Plan
resource "azurerm_service_plan" "plan" {
  name                = "plan-sportsday-${random_string.suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type            = "Windows"
  sku_name           = "B1"
}

# App Service
resource "azurerm_windows_web_app" "app" {
  name                = "app-sportsday-${random_string.suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.plan.id

  site_config {
    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                    = "Production"
    "Azure__SignalR__ConnectionString"          = azurerm_signalr_service.signalr.primary_connection_string
    "ConnectionStrings__DefaultConnection"      = "Server=${azurerm_mssql_server.sql.fully_qualified_domain_name};Database=${azurerm_mssql_database.db.name};User Id=${var.sql_admin_login};Password=${var.sql_admin_password};TrustServerCertificate=True;"
    "Authentication__Google__ClientId"          = var.google_client_id
    "Authentication__Google__ClientSecret"      = var.google_client_secret
  }
}

# Application Insights
resource "azurerm_application_insights" "appinsights" {
  name                = "appi-sportsday-${random_string.suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

# Key Vault
resource "azurerm_key_vault" "kv" {
  name                = "kv-sportsday-${random_string.suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  tenant_id          = data.azurerm_client_config.current.tenant_id
  sku_name           = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get",
      "List",
      "Set",
      "Delete"
    ]
  }
}
