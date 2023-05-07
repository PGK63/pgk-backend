using MediatR;
using PGK.Domain.User;

namespace Market.Application.App.Raportichka.Row.Commands.UpdateAllConfirmation;

public class UpdateAllConfirmationCommand : IRequest
{
    public UserRole Role { get; set; }
    public int UserId { get; set; }
    public int RaportichkaId { get; set; }
}