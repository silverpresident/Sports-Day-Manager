using Microsoft.Extensions.DependencyInjection;
using SportsDay.Lib.Services;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSportsDayServices(this IServiceCollection services)
    {
        // Register all application services
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<IHouseService, HouseService>();
        services.AddScoped<IHouseLeaderService, HouseLeaderService>();
        services.AddScoped<IParticipantService, ParticipantService>();
        services.AddScoped<IEventTemplateService, EventTemplateService>();
        services.AddScoped<IDashboardService, DashboardService>();

#if DEBUG
        // Developer service only available in debug mode
        services.AddScoped<IDeveloperService, DeveloperService>();
#endif

        return services;
    }
}