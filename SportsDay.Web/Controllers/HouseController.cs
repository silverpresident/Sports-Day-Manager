using Microsoft.AspNetCore.Mvc;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Web.Controllers
{
    public class HouseController : Controller
    {
        private readonly ITournamentService _tournamentService;
        private readonly IHouseService _houseService;
        private readonly IParticipantService _participantService;
        private readonly ILogger<HouseController> _logger;

        public HouseController(
            IHouseService houseService,
            IParticipantService participantService,
            ILogger<HouseController> logger,
            ITournamentService tournamentService)
        {
            _houseService = houseService;
            _participantService = participantService;
            _logger = logger;
            _tournamentService = tournamentService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var houses = await _houseService.GetAllAsync();
                return View(houses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving houses for public index");
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var houseDetails = await _houseService.GetHouseDetailsForActiveTournamentAsync(id);
                if (houseDetails == null)
                {
                    _logger.LogWarning("House not found with ID: {HouseId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Loaded house details for {HouseName} (ID: {HouseId})",
                    houseDetails.House.Name, id);

                return View(houseDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving house details for house {HouseId}", id);
                return View("Error");
            }
        }

        public async Task<IActionResult> Results(int id)
        {
            try
            {
                var houseMembers = await _houseService.GetHouseMembersForActiveTournamentAsync(id);
                if (houseMembers == null)
                {
                    _logger.LogWarning("House not found with ID: {HouseId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Loaded house members for {HouseName} (ID: {HouseId})",
                    houseMembers.House.Name, id);

                return View(houseMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving house members for house {HouseId}", id);
                return View("Error");
            }
        }

        public async Task<IActionResult> Participants(int id)
        {
            try
            {
                var model = new HouseParticipantsViewModel();
                var house = await _houseService.GetByIdAsync(id);
                if (house == null)
                {
                    return NotFound();
                }
                model.House = house;
                model.ActiveTournament = await _tournamentService.GetActiveTournamentAsync();
                model.Participants = await _participantService.GetByHouseIdAsync(id);
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving participants for house {HouseId}", id);
                return View("Error");
            }
        }
    }
}
