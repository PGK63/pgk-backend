using PGK.Application.Common.Paged;

namespace PGK.Application.App.Group.Queries.GetGroupStudentList
{
    public class GroupStudentListVm : PagedResult<StudentPasswordDto>
    {
        public override PagedList<StudentPasswordDto> Results { get; set; }
    }
}
