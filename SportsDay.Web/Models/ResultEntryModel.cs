namespace SportsDay.Web.Models;

public class ResultEntryModel
{
    public Guid ResultId { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int? Placement { get; set; }
    public decimal? SpeedOrDistance { get; set; }
    public bool IsDisqualified { get; set; }
    public bool IsDNF { get; set; }
    public bool IsDNS { get; set; }
}