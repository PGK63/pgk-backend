using MediatR;

namespace Market.Application.App.VedomostAttendance;

public class GetVedomostAttendanceQuery : IRequest<Stream>
{
    public int GroupId { get; set; }
    public DateTime Date { get; set; }
}