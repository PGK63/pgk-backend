using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Interfaces;
using PGK.Domain.Journal;

namespace Market.Application.App.Group.Queries.GetAnalyticsJournal;

internal class GetAnalyticsJournalQueryHandler : IRequestHandler<GetAnalyticsJournalQuery, AnalyticsJournalVm>
{
    private readonly IPGKDbContext _dbContext;

    public GetAnalyticsJournalQueryHandler(IPGKDbContext dbContext) => (_dbContext) = (dbContext);
    
    public async Task<AnalyticsJournalVm> Handle(GetAnalyticsJournalQuery request, CancellationToken cancellationToken)
    {
        var columns = await _dbContext.JournalSubjectColumns.Where(
            u => u.Row.JournalSubject.Journal.Group.Id == request.GroupId
        ).ToListAsync(cancellationToken: cancellationToken);

        if (request.StartDate != null)
        {
            columns = columns.Where(u => u.Date >= request.StartDate).ToList();
        }
        
        if (request.EndDate != null)
        {
            columns = columns.Where(u => u.Date <= request.EndDate).ToList();
        }

        var columnsNotHasH = columns.Where(u => u.Evaluation != JournalEvaluation.HAS_H);
        IEnumerable<IGrouping<int, JournalSubjectColumn>>? columnsGroupDate = null;

        switch (request.GroupByType)
        {
            case AnalyticsJournalGroupByType.DAY:
                columnsGroupDate = columnsNotHasH.GroupBy(u => u.Date.Day);
                break;
            case AnalyticsJournalGroupByType.YEAR:
                columnsGroupDate = columnsNotHasH.GroupBy(u => u.Date.Year);
                break;
            case AnalyticsJournalGroupByType.MONTH:
                columnsGroupDate = columnsNotHasH.GroupBy(u => u.Date.Month);
                break;
        }
        
        var analyticsEvaluation = (from column in
            columnsGroupDate let averageEvaluation = 
            column.Average(u => Evaluation.ToInt(u.Evaluation))
            select new AnalyticsJournalDto
            {
                AverageEvaluation = averageEvaluation,
                GroupByDate = column.Key,
                FirstDate = column.Select(u => u.Date).First()
            }).ToList();

        return new AnalyticsJournalVm
        {
            CountEvaluationHas2 = columns.Count(u => u.Evaluation == JournalEvaluation.HAS_2),
            CountEvaluationHas3 = columns.Count(u => u.Evaluation == JournalEvaluation.HAS_3),
            CountEvaluationHas4 = columns.Count(u => u.Evaluation == JournalEvaluation.HAS_4),
            CountEvaluationHas5 = columns.Count(u => u.Evaluation == JournalEvaluation.HAS_5),
            CountEvaluationHasH = columns.Count(u => u.Evaluation == JournalEvaluation.HAS_H),
            AnalyticsEvaluation = analyticsEvaluation
        };
    }
}