using MediatR;
using PGK.Application.App.Group.Queries.GetGroupDetails;
using PGK.Domain.User;

namespace PGK.Application.App.Group.Commands.UpdateCourse
{
    public class GroupUpdateCourseCommand: IRequest<GroupDetails>
    {
        public int GroupId { get; set; }
        public int Course { get; set; }
        public UserRole UserRole { get; set; }
        public int UserId { get; set; }
    }
}
