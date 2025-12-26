using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;

namespace SportsDay.Lib.ViewModels;

public class HouseParticipantsViewModel
{
    public House House { get; set; } = null!;
    public Tournament? ActiveTournament { get; set; }
    public IEnumerable<Participant> Participants { get; set; } = new List<Participant>();
}
