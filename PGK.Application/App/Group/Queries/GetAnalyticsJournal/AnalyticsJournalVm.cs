
namespace Market.Application.App.Group.Queries.GetAnalyticsJournal;

public class AnalyticsJournalVm
{
    public int CountEvaluationHas2 { get; set; }
    public int CountEvaluationHas3 { get; set; }
    public int CountEvaluationHas4 { get; set; }
    public int CountEvaluationHas5 { get; set; }
    public int CountEvaluationHasH { get; set; }
    
    public List<AnalyticsJournalDto> AnalyticsEvaluation { get; set; } = new();
}