using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace PGK.Application.App.User.Queries.GetHistoryList
{
    public class GetHistoryListQuery: IRequest<HistoryListVm>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public int UserId { get; set; }
    }
}
