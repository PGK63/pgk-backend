using MediatR;
using PGK.Application.App.User.Student.Queries.GetStudentUserList;
using PGK.Domain.User;
using PGK.Domain.User.Student;

namespace Market.Application.App.User.Student.Commands.UpdateState;

public class StudentUpdateStateCommand : IRequest<StudentDto>
{
    public int StudentId { get; set; }
    public StudentState State { get; set; }
    
    public UserRole Role { get; set; }
    public int UserId { get; set; }   
}