using Microsoft.AspNetCore.Mvc;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SportsDay.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TournamentsController : Controller
    {
        private readonly ITournamentService _tournamentService;

        public TournamentsController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
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
    }
}
