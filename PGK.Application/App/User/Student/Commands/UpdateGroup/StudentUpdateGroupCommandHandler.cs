using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.App.User.Student.Queries.GetStudentUserList;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Domain.User;
using PGK.Domain.User.Student;

namespace PGK.Application.App.User.Student.Commands.UpdateGroup
{
    internal class StudentUpdateGroupCommandHandler
        : IRequestHandler<StudentUpdateGroupCommand, StudentDto>
    {
        private readonly IPGKDbContext _dbContext;
        public readonly IMapper _mapper;

        public StudentUpdateGroupCommandHandler(IPGKDbContext dbContext, IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<StudentDto> Handle(StudentUpdateGroupCommand request,
            CancellationToken cancellationToken)
        {
            var student = await _dbContext.StudentsUsers.FindAsync(request.StudentId);

            if(student == null)
            {
                throw new NotFoundException(nameof(StudentUser), request.StudentId);
            }

            var group = await _dbContext.Groups
                .Include(u => u.ClassroomTeacher)
                .FirstOrDefaultAsync(u => u.Id == request.GroupId);

            if (group == null)
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

            student.Group = group;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<StudentDto>(student);
        }
    }
}
