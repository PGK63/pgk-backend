using MediatR;
using PGK.Application.App.User.Teacher.Queries.GetTeacherUserDetails;
using PGK.Domain.User.Quide;

namespace Market.Application.App.User.Teacher.Commands.UpdateTeacherState;

public class UpdateTeacherStateCommand : IRequest<TeacherUserDetails>
{
    public int TeacherId { get; set; }
    public GuideState State { get; set; }
}