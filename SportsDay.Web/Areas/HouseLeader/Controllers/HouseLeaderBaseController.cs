using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SportsDay.Web.Areas.HouseLeader.Controllers;

[Area("HouseLeader")]
//[Authorize(Roles = "HouseLeader,Administrator")]
public abstract class HouseLeaderBaseController : Controller
{
}