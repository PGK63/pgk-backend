﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Interfaces;
using PGK.Application.Common.Exceptions;
using PGK.Domain.TechnicalSupport;

namespace PGK.Application.App.TechnicalSupport.Queries.GetMessageList
{
    public class GetMessageListQueryHandler
        : IRequestHandler<GetMessageListQuery, MessageListVm>
    {
        private readonly IPGKDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetMessageListQueryHandler(IPGKDbContext dbContext, IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<MessageListVm> Handle(GetMessageListQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Message> query = _dbContext.Messages
                .OrderByDescending(u => u.Date);

            if (!string.IsNullOrEmpty(request.Search))
            {
                var seacrh = request.Search.ToLower().Trim();
                query = query
                    .Where(u => u.Text != null)
                    .Where(u => u.Text.ToLower().Contains(seacrh));
            }

            if(request.Pin != null)
            {
                query = query.Where(u => u.Pin == request.Pin);
            }

            if(request.UserVisible != null)
            {
                query = query.Where(u => u.UserVisible == request.UserVisible);
            }

            if(request.OnlyDate != null)
            {
                query = query.Where(u => u.Date == request.OnlyDate);
            }

            if(request.StartDate != null && request.OnlyDate == null)
            {
                query = query.Where(u => u.Date >= request.StartDate);
            }

            if (request.EndDate != null && request.OnlyDate == null)
            {
                query = query.Where(u => u.Date <= request.EndDate);
            }

            if(request.UserId != null)
            {
                var user = await _dbContext.Users
                    .Include(u => u.TechnicalSupportChat)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId);

                if(user == null)
                {
                    throw new NotFoundException(nameof(Domain.User.User), request.UserId);
                }

                if(user.TechnicalSupportChat == null)
                {
                    return new MessageListVm
                    {
                        Messages = new List<MessageDto>()
                    };
                }

                query = query.Where(u => u.Chat.Id == user.TechnicalSupportChat.Id);
            }

            if (request.ChatId != null)
            {
                query = query.Where(u => u.Chat.Id == request.ChatId);
            }

            var messages =  await query
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new MessageListVm
            {
                Messages = messages
            };
        }
    }
}
