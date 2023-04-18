using PGK.Domain.TechnicalSupport;
using PGK.Domain.User.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PGK.Domain.User
{
    [Table("Users")]
    public class User
    {
        [Key] public int Id { get; set; }

        public int? TelegramId { get; set; }
        public string? TelegramToken { get; set; }

        [Required, MaxLength(256)] public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(256)] public string LastName { get; set; } = string.Empty;
        [MaxLength(256)] public string? MiddleName { get; set; }

        [MaxLength(256)] public string? Email { get; set; }
        [Required] public bool EmailVerification { get; set; }

        public string? SendEmailToken { get; set; }

        [Required, MaxLength(256)] public string Password { get; set; } = string.Empty;

        public bool? DrarkMode { get; set; }
        [Required] public ThemeStyle ThemeStyle { get; set; } = ThemeStyle.Blue;
        [Required] public ThemeFontStyle ThemeFontStyle { get; set; } = ThemeFontStyle.Default;
        [Required] public ThemeFontSize ThemeFontSize { get; set; } = ThemeFontSize.Medium;
        [Required] public ThemeCorners ThemeCorners { get; set; } = ThemeCorners.Rounded;
        
        public Language.Language? Language { get; set; }

        [Required] public bool IncludedNotifications { get; set; } = true;
        [Required] public bool SoundNotifications { get; set; } = true;
        [Required] public bool VibrationNotifications { get; set; } = true;
        [Required] public bool IncludedSchedulesNotifications { get; set; } = true;
        [Required] public bool IncludedJournalNotifications { get; set; } = true;
        [Required] public bool IncludedRaportichkaNotifications { get; set; } = true;
        [Required] public bool IncludedTechnicalSupportNotifications { get; set; } = true;

        public string? PhotoUrl { get; set; }
        public string? RefreshToken { get; set; }

        public Chat? TechnicalSupportChat { get; set; }
        public virtual List<Message> TechnicalSupportMessages { get; set; } = new();

        public virtual List<Notification.Notification> Notifications { get; set; } = new();
        //public virtual List<History.History> Histories { get; set; } = new();
        
        public virtual string Role => "USER";
    }
}
