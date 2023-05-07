using MediatR;
using PGK.Domain.User;

namespace PGK.Application.App.User.Student.Commands.UpdatePassword
{
    public class StudentUpdatePassswordCoomand : IRequest<string>
    {
        public int UserId { get; set; }
        public int StudentId { get; set; }
        public UserRole Role { get; set; }
    }
}
