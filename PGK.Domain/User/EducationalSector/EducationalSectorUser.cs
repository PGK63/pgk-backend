using System.ComponentModel.DataAnnotations.Schema;
using PGK.Domain.User.Quide;

namespace PGK.Domain.User.EducationalSector
{
    [Table("EducationalSectorUsers")]
    public class EducationalSectorUser : User
    {
        public override string Role => UserRole.EDUCATIONAL_SECTOR.ToString();

        public GuideState State { get; set; }
    }
}
