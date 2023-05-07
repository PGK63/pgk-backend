using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Common;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Application.Security;
using PGK.Domain.User;
using PGK.Domain.User.Teacher;

namespace PGK.Application.App.User.Student.Commands.UpdatePassword
{
    internal class StudentUpdatePassswordCoomandHandler
        : IRequestHandler<StudentUpdatePassswordCoomand, string>
    {
        private readonly IPGKDbContext _dbContext;

        public StudentUpdatePassswordCoomandHandler(IPGKDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<string> Handle(StudentUpdatePassswordCoomand request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.StudentsUsers
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Id == request.StudentId);

            if (user == null)
            {
                throw new NotFoundException(nameof(Domain.User.Student.StudentUser), request.StudentId);
            }

            if(request.Role == UserRole.TEACHER)
            {
                var teacher = await _dbContext.TeacherUsers.FindAsync(request.UserId);

                if (teacher == null)
                {
                    throw new NotFoundException(nameof(TeacherUser), request.UserId);
                }

                if(teacher.Id != user.Group.ClassroomTeacher.Id)
                {
                    throw new Exception("You have to be a curator");
                }
            }

            var password = GeneratorPassword.GetPassword();

            var passwordHash = PasswordHash.CreateHash(password);

            user.Password = passwordHash;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return password;
        }
    }
}
