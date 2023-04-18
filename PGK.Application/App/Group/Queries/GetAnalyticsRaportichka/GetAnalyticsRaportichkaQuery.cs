using MediatR;

namespace Market.Application.App.Group.Queries.GetAnalyticsRaportichka;

public class GetAnalyticsRaportichkaQuery : IRequest<List<AnalyticsRaportichkaDto>>
{
    public int GroupId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public AnalyticsRaportichkaGroupByType GroupByType { get; set; } = AnalyticsRaportichkaGroupByType.DAY;
}