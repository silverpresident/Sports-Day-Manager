using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Lib.Services;

/// <summary>
/// Service for retrieving dashboard statistics and data with caching support
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DashboardService> _logger;

    private const string AdminDashboardCacheKey = "AdminDashboard";
    private const string PublicDashboardCacheKey = "PublicDashboard";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);

    public DashboardService(
        SportsDayDbContext context,
        ITournamentService tournamentService,
        IMemoryCache cache,
        ILogger<DashboardService> logger)
    {
        _context = context;
        _tournamentService = tournamentService;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(AdminDashboardCacheKey, out AdminDashboardViewModel? cachedViewModel) && cachedViewModel != null)
        {
            _logger.LogDebug("Returning cached admin dashboard data");
            return cachedViewModel;
        }

        _logger.LogDebug("Building admin dashboard data from database");

        var viewModel = new AdminDashboardViewModel();

        try
        {
            // Get active tournament
            viewModel.ActiveTournament = await _tournamentService.GetActiveTournamentAsync();

            // Get total counts
            viewModel.TotalTournaments = await _context.Tournaments.CountAsync();
            viewModel.TotalHouses = await _context.Houses.CountAsync();
            viewModel.TotalEventTemplates = await _context.EventTemplates.CountAsync(et => et.IsActive);

            // Get counts based on active tournament or all
            if (viewModel.ActiveTournament != null)
            {
                var tournamentId = viewModel.ActiveTournament.Id;

                viewModel.TotalEvents = await _context.Events
                    .CountAsync(e => e.TournamentId == tournamentId);

                viewModel.TotalParticipants = await _context.Participants
                    .CountAsync(p => p.TournamentId == tournamentId);

                viewModel.TotalResults = await _context.Results
                    .CountAsync(r => r.TournamentId == tournamentId);

                viewModel.ActiveAnnouncements = await _context.Announcements
                    .CountAsync(a => a.TournamentId == tournamentId 
                        && a.IsEnabled 
                        && (!a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.Now));

                // Get active tournament stats
                viewModel.ActiveTournamentStats = new ActiveTournamentStats
                {
                    EventCount = viewModel.TotalEvents,
                    CompletedEvents = await _context.Events
                        .CountAsync(e => e.TournamentId == tournamentId && e.Status == EventStatus.Completed),
                    InProgressEvents = await _context.Events
                        .CountAsync(e => e.TournamentId == tournamentId && e.Status == EventStatus.InProgress),
                    ScheduledEvents = await _context.Events
                        .CountAsync(e => e.TournamentId == tournamentId && e.Status == EventStatus.Scheduled),
                    ParticipantCount = viewModel.TotalParticipants,
                    ResultCount = viewModel.TotalResults,
                    NewRecords = await _context.Results
                        .CountAsync(r => r.TournamentId == tournamentId && r.IsNewRecord)
                };

                // Get recent updates for active tournament
                viewModel.RecentUpdates = await _context.EventUpdates
                    .Include(u => u.Event)
                    .Where(u => u.TournamentId == tournamentId)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync();

                // Get recent results for active tournament
                viewModel.RecentResults = await _context.Results
                    .Include(r => r.Event)
                    .Include(r => r.Participant)
                    .Include(r => r.House)
                    .Where(r => r.TournamentId == tournamentId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                // No active tournament - show all counts
                viewModel.TotalEvents = await _context.Events.CountAsync();
                viewModel.TotalParticipants = await _context.Participants.CountAsync();
                viewModel.TotalResults = await _context.Results.CountAsync();
                viewModel.ActiveAnnouncements = await _context.Announcements
                    .CountAsync(a => a.IsEnabled && (!a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.Now));

                // Get recent updates across all tournaments
                viewModel.RecentUpdates = await _context.EventUpdates
                    .Include(u => u.Event)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync();

                // Get recent results across all tournaments
                viewModel.RecentResults = await _context.Results
                    .Include(r => r.Event)
                    .Include(r => r.Participant)
                    .Include(r => r.House)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync();
            }

            // Cache the result
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration)
                .SetPriority(CacheItemPriority.Normal);

            _cache.Set(AdminDashboardCacheKey, viewModel, cacheOptions);
            _logger.LogDebug("Admin dashboard data cached for {Duration} minutes", CacheDuration.TotalMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building admin dashboard data");
            throw;
        }

        return viewModel;
    }

    /// <inheritdoc />
    public async Task<PublicDashboardViewModel> GetPublicDashboardAsync()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(PublicDashboardCacheKey, out PublicDashboardViewModel? cachedViewModel) && cachedViewModel != null)
        {
            _logger.LogDebug("Returning cached public dashboard data");
            return cachedViewModel;
        }

        _logger.LogDebug("Building public dashboard data from database");

        var viewModel = new PublicDashboardViewModel();

        try
        {
            // Get active tournament
            viewModel.ActiveTournament = await _tournamentService.GetActiveTournamentAsync();

            if (viewModel.ActiveTournament != null)
            {
                var tournamentId = viewModel.ActiveTournament.Id;

                // Get announcements
                viewModel.Announcements = await _context.Announcements
                    .Where(a => a.TournamentId == tournamentId && a.IsEnabled)
                    .Where(a => !a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.Now)
                    .OrderByDescending(a => a.Priority)
                    .ThenByDescending(a => a.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();

                // Get recent updates
                viewModel.Updates = await _context.EventUpdates
                    .Include(u => u.Event)
                    .Where(u => u.TournamentId == tournamentId)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(20)
                    .AsNoTracking()
                    .ToListAsync();

                // Get house summaries for leaderboard
                viewModel.Summaries = await _context.TournamentHouseSummaries
                    .Include(s => s.House)
                    .Where(s => s.TournamentId == tournamentId)
                    .OrderByDescending(s => s.Points)
                    .AsNoTracking()
                    .ToListAsync();

                // Get division count
                viewModel.DivisionCount = viewModel.Summaries
                    .Select(s => s.Division)
                    .Distinct()
                    .Count();

                // Get event statistics
                viewModel.TotalEvents = await _context.Events
                    .CountAsync(e => e.TournamentId == tournamentId);

                viewModel.CompletedEvents = await _context.Events
                    .CountAsync(e => e.TournamentId == tournamentId && e.Status == EventStatus.Completed);

                // Get participant count
                viewModel.TotalParticipants = await _context.Participants
                    .CountAsync(p => p.TournamentId == tournamentId);

                // Get results count
                viewModel.TotalResults = await _context.Results
                    .CountAsync(r => r.TournamentId == tournamentId);

                // Get new records count
                viewModel.NewRecords = await _context.Results
                    .CountAsync(r => r.TournamentId == tournamentId && r.IsNewRecord);
            }

            // Cache the result
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration)
                .SetPriority(CacheItemPriority.Normal);

            _cache.Set(PublicDashboardCacheKey, viewModel, cacheOptions);
            _logger.LogDebug("Public dashboard data cached for {Duration} minutes", CacheDuration.TotalMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building public dashboard data");
            throw;
        }

        return viewModel;
    }

    /// <inheritdoc />
    public void InvalidateAdminCache()
    {
        _cache.Remove(AdminDashboardCacheKey);
        _logger.LogDebug("Admin dashboard cache invalidated");
    }

    /// <inheritdoc />
    public void InvalidatePublicCache()
    {
        _cache.Remove(PublicDashboardCacheKey);
        _logger.LogDebug("Public dashboard cache invalidated");
    }

    /// <inheritdoc />
    public void InvalidateCache()
    {
        InvalidateAdminCache();
        InvalidatePublicCache();
        _logger.LogDebug("All dashboard caches invalidated");
    }
}