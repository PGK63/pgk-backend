using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Interfaces;
using PGK.Domain.User;

namespace Market.Application.App.Raportichka.Row.Commands.UpdateAllConfirmation;

public class UpdateAllConfirmationCommandHandler : IRequestHandler<UpdateAllConfirmationCommand>
{
    private readonly IPGKDbContext _dbContext;

    public UpdateAllConfirmationCommandHandler(IPGKDbContext dbContext) =>
        _dbContext = dbContext;
    
    public async Task<Unit> Handle(UpdateAllConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        var rows = await _dbContext.RaportichkaRows
            .Include(u => u.Raportichka)
            .Include(u => u.Teacher)
            .Where(u => u.Raportichka.Id == request.RaportichkaId)
            .ToListAsync(cancellationToken: cancellationToken);

        if (request.Role == UserRole.TEACHER)
        {
            foreach (var row in rows.Where(u => u.Teacher.Id == request.UserId))
            {
                row.Confirmation = !row.Confirmation;
                
            }
        }
        else
        {
            foreach (var row in rows)
            {
                row.Confirmation = !row.Confirmation;;
            }
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}