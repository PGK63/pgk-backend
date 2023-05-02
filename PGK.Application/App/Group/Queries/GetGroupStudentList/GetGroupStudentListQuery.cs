using MediatR;
using PGK.Domain.User;

namespace PGK.Application.App.Group.Queries.GetGroupStudentList
{
    public class GetGroupStudentListQuery : IRequest<GroupStudentListVm>
    {
        public int GroupId { get; set; }
        public bool PasswordVisibility { get; set; } = false;

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        
        public UserRole Role { get; set; }
        public int UserId { get; set; }
    }
}
