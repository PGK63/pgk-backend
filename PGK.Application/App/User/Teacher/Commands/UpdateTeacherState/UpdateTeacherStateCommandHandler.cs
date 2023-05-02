using AutoMapper;
using MediatR;
using PGK.Application.App.User.Teacher.Queries.GetTeacherUserDetails;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Domain.User.Teacher;

namespace Market.Application.App.User.Teacher.Commands.UpdateTeacherState;

public class UpdateTeacherStateCommandHandler : IRequestHandler<UpdateTeacherStateCommand, TeacherUserDetails>
{
    private readonly IPGKDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public async Task<TeacherUserDetails> Handle(UpdateTeacherStateCommand request,
        CancellationToken cancellationToken)
    {
        var teacher = await _dbContext.TeacherUsers.FindAsync(request.TeacherId);

        if (teacher == null)
        {
            throw new NotFoundException(nameof(TeacherUser), request.TeacherId);
        }

        teacher.State = request.State;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TeacherUserDetails>(teacher);
    }
}