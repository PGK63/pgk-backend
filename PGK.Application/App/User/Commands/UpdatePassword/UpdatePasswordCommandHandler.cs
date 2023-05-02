using MediatR;
using PGK.Application.Common;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Application.Security;

namespace PGK.Application.App.User.Commands.UpdatePassword
{
    internal class UpdatePasswordCommandHandler
        : IRequestHandler<UpdatePasswordCommand, string>
    {
        private readonly IPGKDbContext _dbContext;

        public UpdatePasswordCommandHandler(IPGKDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<string> Handle(UpdatePasswordCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(request.UserId);

            if(user == null)
            {
                throw new NotFoundException(nameof(Domain.User.User), request.UserId);
            }

            var password = GeneratorPassword.GetPassword();

            var passwordHash = PasswordHash.CreateHash(password);

            user.PasswordHash = passwordHash;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return password;
        }
    }
}
