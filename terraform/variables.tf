variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "eastus"
}

variable "sql_admin_login" {
  description = "The administrator username for the SQL server"
  type        = string
}

variable "sql_admin_password" {
  description = "The administrator password for the SQL server"
  type        = string
  sensitive   = true
}

variable "sql_aad_admin_login" {
  description = "The Azure AD administrator username for the SQL server"
  type        = string
}

variable "sql_aad_admin_object_id" {
  description = "The Azure AD administrator object ID for the SQL server"
  type        = string
}

variable "google_client_id" {
  description = "The Google OAuth client ID"
  type        = string
}

variable "google_client_secret" {
  description = "The Google OAuth client secret"
  type        = string
  sensitive   = true
}

# Create a terraform.tfvars.example file with example values
# Do not include sensitive values in the example file
