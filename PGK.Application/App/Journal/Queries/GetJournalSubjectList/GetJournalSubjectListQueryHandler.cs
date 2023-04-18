using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using PGK.Application.Common.Paged;
using PGK.Application.Interfaces;
using PGK.Domain.Journal;

namespace PGK.Application.App.Journal.Queries.GetJournalSubjectList
{
    internal class GetJournalSubjectListQueryHandler
        : IRequestHandler<GetJournalSubjectListQuery, JournalSubjectListVm>
    {
        private readonly IPGKDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetJournalSubjectListQueryHandler(IPGKDbContext dbContext, IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<JournalSubjectListVm> Handle(GetJournalSubjectListQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<JournalSubject> query = _dbContext.JournalSubjects;

            if(request.JournalId != null)
            {
                query = query.Where(u => u.Journal.Id == request.JournalId);
            }

            var journalSubject = query
                .OrderByDescending(u => u.Id)
                .ProjectTo<JournalSubjectDto>(_mapper.ConfigurationProvider);

            var journalPaged = await PagedList<JournalSubjectDto>.ToPagedList(journalSubject,
                request.PageNumber, request.PageSize);

            return new JournalSubjectListVm
            {
                Results = journalPaged
            };
        }
    }
}
