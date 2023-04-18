using PGK.Application.Common.Paged;

namespace PGK.Application.App.User.Queries.GetHistoryList
{
    public class HistoryListVm : PagedResult<HistoryDto>
    {
        public override PagedList<HistoryDto> Results { get; set; }
    }
}
