using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.App.User.Student.Queries.GetStudentUserList;
using PGK.Application.Common.Exceptions;
using PGK.Application.Common.Paged;
using PGK.Application.Interfaces;
using PGK.Domain.User;
using PGK.Domain.User.Student;
using PGK.Domain.User.Teacher;

namespace PGK.Application.App.Group.Queries.GetGroupStudentList
{
    internal class GetGroupStudentListQueryHandler
        : IRequestHandler<GetGroupStudentListQuery, GroupStudentListVm>
    {
        private readonly IPGKDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetGroupStudentListQueryHandler(IPGKDbContext dbContext,
            IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<GroupStudentListVm> Handle(GetGroupStudentListQuery request,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.StudentsUsers
                .Include(u => u.Group)
                .Where(u => u.Group.Id == request.GroupId);
            
            var students = query
                .OrderByDescending(u => u.Id)
                .ProjectTo<StudentPasswordDto>(_mapper.ConfigurationProvider);

            if (request.PasswordVisibility)
            {
                if (request.Role != UserRole.ADMIN && request.Role == UserRole.TEACHER)
                {
                    var teacher = await _dbContext.TeacherUsers.FindAsync(request.UserId);

                    if (teacher == null)
                    {
                        throw new NotFoundException(nameof(TeacherUser), request.UserId);
                    }

                    if (teacher.Сurator.Any(u => u.Id != request.GroupId))
                    {
                        students = students.Select(u => StudentToPasswordNull(u));    
                    }
                }
                else
                {
                    students = students.Select(u => StudentToPasswordNull(u));
                }
            }
            else
            {
                students = students.Select(u => StudentToPasswordNull(u));
            }
            
            var studentsPaged = await PagedList<StudentPasswordDto>.ToPagedList(students, request.PageNumber,
                request.PageSize);

            return new GroupStudentListVm
            {
                Results = studentsPaged
            };
        }

        private StudentPasswordDto StudentToPasswordNull(StudentPasswordDto studentUser)
        {
            studentUser.Password = null;
            return studentUser;
        }
    }
}
