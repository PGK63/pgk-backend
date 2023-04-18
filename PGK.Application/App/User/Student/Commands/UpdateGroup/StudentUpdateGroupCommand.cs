using MediatR;
using PGK.Application.App.User.Student.Queries.GetStudentUserList;
using PGK.Domain.User;

namespace PGK.Application.App.User.Student.Commands.UpdateGroup
{
    public class StudentUpdateGroupCommand: IRequest<StudentDto>
    {
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public UserRole UserRole { get; set; }
    }
}
