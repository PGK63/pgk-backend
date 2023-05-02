using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Interfaces;
using PGK.Application.Security;
using PGK.Domain.User;
using PGK.Domain.User.Quide;
using PGK.Domain.User.Student;

namespace PGK.Application.App.User.Auth.Commands.RefreshToken
{ 
    internal class RefreshTokenCommandHandler
        : IRequestHandler<RefreshTokenCommand, RefreshTokenVm>
    {
        private readonly IPGKDbContext _dbContext;
        private readonly IAuth _auth;

        public RefreshTokenCommandHandler(IPGKDbContext dbContext,
            IAuth auth) => (_dbContext, _auth) = (dbContext, auth);

        public async Task<RefreshTokenVm> Handle(RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(
                u => u.RefreshToken == request.RefreshToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"Invalid token ({request.RefreshToken})");
            }

            if(!_auth.TokenValidation(token: request.RefreshToken, type: TokenType.REFRESH_TOKEN))
            {
                throw new UnauthorizedAccessException("The token has expired");
            }
            
            if (user.Role == UserRole.TEACHER.ToString())
            {
                var teaher = await _dbContext.TeacherUsers.FindAsync(user.Id);

                if (teaher is { State: GuideState.DISMISSED })
                {
                    throw new UnauthorizedAccessException("Вы больше не работайте в ПГК");
                }
            }else if (user.Role == UserRole.STUDENT.ToString()
                      || user.Role == UserRole.DEPUTY_HEADMAN.ToString()
                      || user.Role == UserRole.HEADMAN.ToString())
            {
                var student = await _dbContext.StudentsUsers.FindAsync(user.Id);

                if (student is { State: StudentState.EXPELLED })
                {
                    throw new UnauthorizedAccessException("Вы больше не учитесь в ПГК");
                }
            }else if (user.Role == UserRole.DIRECTOR.ToString())
            {
                var director = await _dbContext.DirectorUsers.FindAsync(user.Id);

                if (!director.Current)
                {
                    throw new UnauthorizedAccessException("Вы больше не работайте в ПГК");
                }
            }else if (user.Role == UserRole.DEPARTMENT_HEAD.ToString())
            {
                var departmentHeadUser = await _dbContext.DepartmentHeadUsers.FindAsync(user.Id);

                if (departmentHeadUser is { State: GuideState.DISMISSED })
                {
                    throw new UnauthorizedAccessException("Вы больше не работайте в ПГК");
                }
            }

            var jwtToken = _auth.CreateAccessToken(userId: user.Id, userRole: user.Role);

            return new RefreshTokenVm { AccessToken = jwtToken };
        }
    }
}
