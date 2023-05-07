using MediatR;
using PGK.Application.Common.Date;

namespace Market.Application.App.VedomostAttendance;

public class GetVedomostAttendanceQuery : IRequest<byte[]>
{
    public int GroupId { get; set; }
    public int Year { get; set; }
    public Month Month { get; set; }
}