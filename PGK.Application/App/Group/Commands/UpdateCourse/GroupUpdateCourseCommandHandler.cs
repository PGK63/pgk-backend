using AutoMapper;
using MediatR;
using PGK.Application.App.Group.Queries.GetGroupDetails;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Domain.User;

namespace PGK.Application.App.Group.Commands.UpdateCourse
{
    internal class GroupUpdateCourseCommandHandler
        : IRequestHandler<GroupUpdateCourseCommand, GroupDetails>
    {
        private readonly IPGKDbContext _dbContext;
        private readonly IMapper _mapper;

        public GroupUpdateCourseCommandHandler(IPGKDbContext dbContext,
            IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<GroupDetails> Handle(GroupUpdateCourseCommand request,
            CancellationToken cancellationToken)
        {
            var group = await _dbContext.Groups.FindAsync(request.GroupId);

            if(group == null)
            {
                throw new NotFoundException(nameof(Domain.Group.Group), request.GroupId);
            }

            if(request.UserRole == UserRole.TEACHER)
            {
                if(group.ClassroomTeacher.Id != request.UserId)
                {
                    throw new Exception("У вас нет доступа для изменений");
                }
            }

            group.Course = request.Course;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<GroupDetails>(group);
        }
    }
}
