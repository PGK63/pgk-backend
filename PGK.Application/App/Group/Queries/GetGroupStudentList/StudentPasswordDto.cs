using System.ComponentModel.DataAnnotations;
using AutoMapper;
using PGK.Application.App.Group.Queries.GetGroupDetails;
using PGK.Application.App.User.Student.Queries.GetStudentUserList;
using PGK.Application.Common.Mappings;
using PGK.Domain.User.Student;

namespace PGK.Application.App.Group.Queries.GetGroupStudentList;

public class StudentPasswordDto : IMapWith<StudentUser>
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(128)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(128)] public string LastName { get; set; } = string.Empty;
    [MaxLength(128)] public string? MiddleName { get; set; }
    [MaxLength(128)] public string? Password { get; set; }
    public string? PhotoUrl { get; set; } = null;
    [Required] public GroupDetails Group { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<StudentUser, StudentDto>();
    }
}