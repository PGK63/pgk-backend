using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.App.User.Queries.GetHistoryList;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;

namespace PGK.Application.App.User.Commands.AddHistoryItem
{
    internal class AddHistoryItemCommandHandler
        : IRequestHandler<AddHistoryItemCommand, HistoryDto>
    {
        private readonly IPGKDbContext _dbContext;
        public readonly IMapper _mapper;

        public AddHistoryItemCommandHandler(IPGKDbContext dbContext, IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<HistoryDto> Handle(AddHistoryItemCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(request.UserId);

            if(user == null)
            {
                throw new NotFoundException(nameof(Domain.User.User), request.UserId);
            }

            var item = await _dbContext.Histories
                .FirstOrDefaultAsync(u => u.Type == request.Type && u.ContentId == request.ContentId);

            if (item != null)
            {
                _dbContext.Histories.Remove(item);
            }

            var historyItem = new Domain.User.History.History
            {
                ContentId = request.ContentId,
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                User = user
            };

            await _dbContext.Histories.AddAsync(historyItem, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<HistoryDto>(historyItem);
        }
    }
}
