namespace Market.Application.App.Group.Queries.GetAnalyticsJournal;

public class AnalyticsJournalDto
{
    public int GroupByDate { get; set; }
    public DateTime FirstDate { get; set; }
    public double AverageEvaluation { get; set; }
}