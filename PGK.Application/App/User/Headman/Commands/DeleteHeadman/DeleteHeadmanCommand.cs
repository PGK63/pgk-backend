using MediatR;
using PGK.Domain.User;

namespace Market.Application.App.User.Headman.Commands.DeleteHeadman;

public class DeleteHeadmanCommand : IRequest
{
    public int Id { get; set; }
    public bool Deputy { get; set; } = false;
}