using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Web.Hubs;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsDay.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TournamentsController : Controller
    {
        private readonly ITournamentService _tournamentService;
        private readonly SportsDayDbContext _context;
        private readonly IHubContext<SportsHub> _hubContext;

        public TournamentsController(
            ITournamentService tournamentService,
            SportsDayDbContext context,
            IHubContext<SportsHub> hubContext)
        {
            _tournamentService = tournamentService;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var tournaments = await _tournamentService.GetTournamentsAsync();
            return View(tournaments);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(id.Value);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

//[Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
//[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name,TournamentDate,IsActive")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                await _tournamentService.CreateTournamentAsync(tournament);
                return RedirectToAction(nameof(Index));
            }
            return View(tournament);
        }

//[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(id.Value);
            if (tournament == null)
            {
                return NotFound();
            }
            return View(tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
//[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,TournamentDate,IsActive")] Tournament tournament)
        {
            if (id != tournament.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _tournamentService.UpdateTournamentAsync(tournament);
                }
                catch (Exception)
                {
                    // Log the exception
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tournament);
        }

//[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(id.Value);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

//[Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _tournamentService.DeleteTournamentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(Guid id)
        {
            await _tournamentService.SetActiveTournamentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishPointsStanding(Guid id)
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id);
            if (tournament == null)
            {
                TempData["Error"] = "Tournament not found.";
                return RedirectToAction(nameof(Index));
            }

            // Get all houses
            var houses = await _context.Houses.OrderBy(h => h.Name).ToListAsync();

            // Get all results for this tournament with related data
            var results = await _context.Results
                .Include(r => r.House)
                .Include(r => r.Event)
                .Where(r => r.TournamentId == id && r.Placement.HasValue)
                .ToListAsync();

            if (!results.Any())
            {
                TempData["Error"] = "No results found for this tournament.";
                return RedirectToAction(nameof(Index));
            }

            // Build the points standing markdown content
            var sb = new StringBuilder();
            sb.AppendLine($"# Points Standing - {tournament.Name}");
            sb.AppendLine();
            sb.AppendLine($"*Updated: {DateTime.Now:MMMM dd, yyyy 'at' h:mm tt}*");
            sb.AppendLine();

            // Calculate points by house and division
            var divisions = new[] { DivisionType.Boys, DivisionType.Girls, DivisionType.Open };

            foreach (var division in divisions)
            {
                sb.AppendLine($"## {division} Division");
                sb.AppendLine();

                var divisionResults = results
                    .Where(r => r.Event != null && r.Event.GenderGroup == division)
                    .GroupBy(r => r.HouseId)
                    .Select(g => new
                    {
                        HouseId = g.Key,
                        House = g.First().House,
                        TotalPoints = g.Sum(r => r.Points)
                    })
                    .OrderByDescending(x => x.TotalPoints)
                    .ToList();

                if (divisionResults.Any())
                {
                    sb.AppendLine("| Position | House | Points |");
                    sb.AppendLine("|----------|-------|--------|");

                    int position = 1;
                    foreach (var houseResult in divisionResults)
                    {
                        var medal = position switch
                        {
                            1 => "🥇",
                            2 => "🥈",
                            3 => "🥉",
                            _ => ""
                        };
                        sb.AppendLine($"| {medal} {position} | **{houseResult.House?.Name}** | {houseResult.TotalPoints} |");
                        position++;
                    }
                }
                else
                {
                    sb.AppendLine("*No results recorded for this division yet.*");
                }

                sb.AppendLine();
            }

            // Overall standings
            sb.AppendLine("## Overall Standings");
            sb.AppendLine();

            var overallResults = results
                .GroupBy(r => r.HouseId)
                .Select(g => new
                {
                    HouseId = g.Key,
                    House = g.First().House,
                    TotalPoints = g.Sum(r => r.Points)
                })
                .OrderByDescending(x => x.TotalPoints)
                .ToList();

            if (overallResults.Any())
            {
                sb.AppendLine("| Position | House | Total Points |");
                sb.AppendLine("|----------|-------|--------------|");

                int position = 1;
                foreach (var houseResult in overallResults)
                {
                    var medal = position switch
                    {
                        1 => "🥇",
                        2 => "🥈",
                        3 => "🥉",
                        _ => ""
                    };
                    sb.AppendLine($"| {medal} {position} | **{houseResult.House?.Name}** | {houseResult.TotalPoints} |");
                    position++;
                }
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine("*This is an automated announcement generated from current results.*");

            // Create the announcement
            var announcement = new Announcement
            {
                Id = Guid.NewGuid(),
                Body = sb.ToString(),
                Tag = "Points Standing",
                Priority = AnnouncementPriority.Info,
                TournamentId = id,
                IsEnabled = true,
                CreatedBy = User.Identity?.Name ?? "System",
                CreatedAt = DateTime.Now
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            // Send real-time update
            await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement");

            TempData["Success"] = "Points standing announcement published successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
