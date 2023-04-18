using MediatR;
using PGK.Application.App.User.Queries.GetHistoryList;
using PGK.Domain.User.History;
using System.ComponentModel.DataAnnotations;

namespace PGK.Application.App.User.Commands.AddHistoryItem
{
    public class AddHistoryItemCommand : IRequest<HistoryDto>
    {
        [Required] public int ContentId { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required] public HistoryType Type { get; set; }

        [Required] public int UserId { get; set; }
    }
}
