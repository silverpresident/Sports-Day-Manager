using Microsoft.AspNetCore.Mvc;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers
{
    public class HouseController : Controller
    {
        private readonly IHouseService _houseService;
        private readonly IParticipantService _participantService;
        private readonly ILogger<HouseController> _logger;

        public HouseController(
            IHouseService houseService,
            IParticipantService participantService,
            ILogger<HouseController> logger)
        {
            _houseService = houseService;
            _participantService = participantService;
            _logger = logger;
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

        public async Task<IActionResult> Participants(int id)
        {
            try
            {
                var house = await _houseService.GetByIdAsync(id);
                if (house == null)
                {
                    return NotFound();
                }

                var participants = await _participantService.GetByHouseIdAsync(id);
                
                ViewBag.House = house;
                return View(participants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving participants for house {HouseId}", id);
                return View("Error");
            }
        }
    }
}
