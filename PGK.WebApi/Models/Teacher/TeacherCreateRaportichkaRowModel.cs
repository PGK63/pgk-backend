using System.ComponentModel.DataAnnotations;
using PGK.Domain.Raportichka;

namespace PGK.WebApi.Models.Teacher
{
    public class TeacherCreateRaportichkaRowModel
    {
        [Required] public int NumberLesson { get; set; }
        [Required] public int Hours { get; set; }
        [Required] public RaportichkaCause Cause { get; set; }
        [Required] public int SubjectId { get; set; }
        [Required] public int StudentId { get; set; }
    }
}
