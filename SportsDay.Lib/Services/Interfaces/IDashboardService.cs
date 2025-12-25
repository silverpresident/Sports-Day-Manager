using SportsDay.Lib.ViewModels;

namespace SportsDay.Lib.Services.Interfaces;

/// <summary>
/// Service for retrieving dashboard statistics and data
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Gets the admin dashboard view model with all statistics
    /// </summary>
    /// <returns>AdminDashboardViewModel with populated statistics</returns>
    Task<AdminDashboardViewModel> GetAdminDashboardAsync();

    /// <summary>
    /// Gets the public dashboard view model with tournament data
    /// </summary>
    /// <returns>PublicDashboardViewModel with populated data</returns>
    Task<PublicDashboardViewModel> GetPublicDashboardAsync();

    /// <summary>
    /// Invalidates the admin dashboard cache, forcing a refresh on next request
    /// </summary>
    void InvalidateAdminCache();

    /// <summary>
    /// Invalidates the public dashboard cache, forcing a refresh on next request
    /// </summary>
    void InvalidatePublicCache();

    /// <summary>
    /// Invalidates all dashboard caches
    /// </summary>
    void InvalidateCache();
}