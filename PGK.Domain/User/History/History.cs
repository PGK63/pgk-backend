using System.ComponentModel.DataAnnotations;

namespace PGK.Domain.User.History
{
    public class History
    {
        [Key] public int Id { get; set; }
        
        [Required] public int ContentId { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required] public HistoryType Type { get; set; }
        [Required] public DateTime Date { get; set; } = new DateTime();

        [Required] public User User { get; set; }
    }
}
