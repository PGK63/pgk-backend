using MediatR;
using PGK.Domain.User.Quide;

namespace PGK.Application.App.User.Teacher.Queries.GetTeacherUserList
{
    public class GetTeacherUserListQuery : IRequest<TeacherUserListVm>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public string? Search { get; set; } = string.Empty;
        public GuideState? State { get; set; }
        public List<int>? SubjectIds { get; set; } = new List<int>();
    }
}
