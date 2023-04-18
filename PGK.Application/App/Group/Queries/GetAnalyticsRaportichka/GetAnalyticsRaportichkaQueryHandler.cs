using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Interfaces;
using PGK.Domain.Raportichka;

namespace Market.Application.App.Group.Queries.GetAnalyticsRaportichka;

internal class GetAnalyticsRaportichkaQueryHandler : IRequestHandler<GetAnalyticsRaportichkaQuery, List<AnalyticsRaportichkaDto>>
{
    private readonly IPGKDbContext _dbContext;

    public GetAnalyticsRaportichkaQueryHandler(IPGKDbContext dbContext) => _dbContext = dbContext;

    public async Task<List<AnalyticsRaportichkaDto>> Handle(GetAnalyticsRaportichkaQuery request, CancellationToken cancellationToken)
    {
        var raportichka = await _dbContext.RaportichkaRows
            .Include(u => u.Raportichka)
                .ThenInclude(u => u.Group)
            .Where(
                u => u.Raportichka.Group.Id == request.GroupId
            ).ToListAsync(cancellationToken);
        
        if (request.StartDate != null)
        {
            raportichka = raportichka.Where(u => u.Raportichka.Date >= request.StartDate).ToList();
        }
        
        if (request.EndDate != null)
        {
            raportichka = raportichka.Where(u => u.Raportichka.Date <= request.EndDate).ToList();
        }
        
        IEnumerable<IGrouping<int, RaportichkaRow>>? raportichkaGroupBy = null;

        switch (request.GroupByType)
        {
            case AnalyticsRaportichkaGroupByType.DAY:
                raportichkaGroupBy = raportichka.GroupBy(u => u.Raportichka.Date.Day);
                break;
            case AnalyticsRaportichkaGroupByType.YEAR:
                raportichkaGroupBy = raportichka.GroupBy(u => u.Raportichka.Date.Year);
                break;
            case AnalyticsRaportichkaGroupByType.MONTH:
                raportichkaGroupBy = raportichka.GroupBy(u => u.Raportichka.Date.Month);
                break;
        }

        var analyticsRaporichakaCount = new List<AnalyticsRaportichkaDto>();

        if (raportichkaGroupBy != null)
        {
            foreach (var row in raportichkaGroupBy)
            {
                analyticsRaporichakaCount.Add(new AnalyticsRaportichkaDto
                {
                    GroupByDate = row.Key,
                    FirstDate = row.Select(u => u.Raportichka.Date).First(),
                    CountRow = row.Count()
                });
            }   
        }

        return analyticsRaporichakaCount;
    }
}