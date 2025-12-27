using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Areas.Admin.Controllers
{
    public class EventClassGroupsController : AdminBaseController
    {
        private readonly IEventClassGroupService _classGroupService;
        private readonly ILogger<EventClassGroupsController> _logger;

        public EventClassGroupsController(
            IEventClassGroupService classGroupService,
            ILogger<EventClassGroupsController> logger)
        {
            _classGroupService = classGroupService;
            _logger = logger;
        }

        // GET: Admin/EventClassGroups
        public async Task<IActionResult> Index()
        {
            try
            {
                var classGroups = await _classGroupService.GetAllClassGroupsAsync();
                return View(classGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class groups");
                TempData["Error"] = "An error occurred while retrieving class groups.";
                return View();
            }
        }

        // GET: Admin/EventClassGroups/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var classGroup = await _classGroupService.GetClassGroupByNumberAsync(id);
                if (classGroup == null)
                {
                    _logger.LogWarning("Class group {ClassGroupNumber} not found", id);
                    TempData["Error"] = "Class group not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(classGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class group {ClassGroupNumber}", id);
                TempData["Error"] = "An error occurred while retrieving the class group.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/EventClassGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int maxParticipantAge, string? description)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                var success = await _classGroupService.UpdateClassGroupAsync(id, maxParticipantAge, description, userName);

                if (success)
                {
                    _logger.LogInformation("Class group {ClassGroupNumber} updated by {User}", id, userName);
                    TempData["Success"] = "Class group updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("Failed to update class group {ClassGroupNumber}", id);
                    TempData["Error"] = "Failed to update class group.";
                    
                    // Reload the class group for the view
                    var classGroup = await _classGroupService.GetClassGroupByNumberAsync(id);
                    return View(classGroup);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class group {ClassGroupNumber}", id);
                TempData["Error"] = "An error occurred while updating the class group.";
                
                // Reload the class group for the view
                var classGroup = await _classGroupService.GetClassGroupByNumberAsync(id);
                return View(classGroup);
            }
        }
    }
}