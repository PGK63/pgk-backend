using PGK.Domain.User.History;
using System.ComponentModel.DataAnnotations;

namespace PGK.WebApi.Models.User
{
    public class AddHistoryItemModel
    {
        [Required] public int ContentId { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required] public HistoryType Type { get; set; }
    }
}
