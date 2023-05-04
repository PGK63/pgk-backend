using MediatR;

namespace Market.Application.App.VedomostAttendance;

public class GetVedomostAttendanceQuery : IRequest<byte[]>
{
    public int GroupId { get; set; }
    public DateTime Date { get; set; }
}