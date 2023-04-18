using MediatR;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Domain.User.Headman;
using PGK.Domain.User.Student;

namespace Market.Application.App.User.Headman.Commands.DeleteHeadman;

public class DeleteHeadmanCommandHandler : IRequestHandler<DeleteHeadmanCommand>
{
    private readonly IPGKDbContext _dbContext;

    public DeleteHeadmanCommandHandler(IPGKDbContext dbContext) => _dbContext = dbContext;
    
    public async Task<Unit> Handle(DeleteHeadmanCommand request, CancellationToken cancellationToken)
    {
        if (request.Deputy)
        {

        }else
        {
            var headman = await _dbContext.HeadmanUsers.FindAsync(request.Id);

            if(headman == null)
            {
                throw new NotFoundException(nameof(HeadmanUser), request.Id);
            }
        }

        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}