using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using PGK.Application.Common.Paged;
using PGK.Application.Interfaces;

namespace PGK.Application.App.User.Queries.GetHistoryList
{
    internal class GetHistoryListQueryHandler
        : IRequestHandler<GetHistoryListQuery, HistoryListVm>
    {

        private readonly IPGKDbContext _dbContext;
        public readonly IMapper _mapper;

        public GetHistoryListQueryHandler(IPGKDbContext dbContext, IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<HistoryListVm> Handle(
            GetHistoryListQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Domain.User.History.History> query = _dbContext.Histories
                .Where(u => u.User.Id == request.UserId)
                .OrderByDescending(u => u.Date);

            var histories = query.ProjectTo<HistoryDto>(_mapper.ConfigurationProvider);

            var historiesPaged = await PagedList<HistoryDto>.ToPagedList(histories,
                    request.PageNumber, request.PageSize);

            return new HistoryListVm { Results = historiesPaged };
        }
    }
}
