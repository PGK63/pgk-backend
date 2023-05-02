using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.App.User.Student.Queries.GetStudentUserList;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Domain.User;
using PGK.Domain.User.Student;
using PGK.Domain.User.Teacher;

namespace Market.Application.App.User.Student.Commands.UpdateState;

public class StudentUpdateStateCommandHandler 
    : IRequestHandler<StudentUpdateStateCommand, StudentDto>
{
    private readonly IPGKDbContext _dbContext;
    private readonly IMapper _mapper;

    public StudentUpdateStateCommandHandler(IPGKDbContext dbContext, IMapper mapper) =>
        (_dbContext, _mapper) = (dbContext, mapper);
    
    public async Task<StudentDto> Handle(StudentUpdateStateCommand request,
        CancellationToken cancellationToken)
    {
        var student = await _dbContext.StudentsUsers
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Id == request.StudentId);
        
        
        if (student == null)
        {
            throw new NotFoundException(nameof(StudentUser), request.StudentId);
        }

        if (request.Role == UserRole.TEACHER)
        {
            var teaher = await _dbContext.TeacherUsers
                .Include(u => u.Сurator)
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (teaher == null)
            {
                throw new NotFoundException(nameof(TeacherUser), request.UserId);
            }

            if (teaher.Сurator.Any(u => u.Id != student.Group.Id))
            {
                throw new Exception("Вы не можете изменять этого студента");
            }
        }

        student.State = request.State;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<StudentDto>(student);
    }
}