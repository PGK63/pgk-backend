using MediatR;

namespace Market.Application.App.Group.Queries.GetAnalyticsJournal;

public class GetAnalyticsJournalQuery : IRequest<AnalyticsJournalVm>
{
    public int GroupId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public AnalyticsJournalGroupByType GroupByType { get; set; } = AnalyticsJournalGroupByType.DAY;
}