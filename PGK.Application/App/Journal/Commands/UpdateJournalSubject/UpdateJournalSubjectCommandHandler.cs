using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.App.Journal.Queries.GetJournalSubjectList;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Domain.User;
using PGK.Domain.User.Teacher;

namespace Market.Application.App.Journal.Commands.UpdateJournalSubject;

public class UpdateJournalSubjectCommandHandler
    : IRequestHandler<UpdateJournalSubjectCommand, JournalSubjectDto>
{
    private readonly IPGKDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateJournalSubjectCommandHandler(IPGKDbContext dbContext,
        IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper);
    
    public async Task<JournalSubjectDto> Handle(UpdateJournalSubjectCommand request,
        CancellationToken cancellationToken)
    {
        var journalSubject = await _dbContext.JournalSubjects.FindAsync(request.JournalSubjectId);

        if (journalSubject == null)
        {
            throw new NotFoundException(nameof(journalSubject), request.JournalSubjectId);
        }
        
        var subject = await _dbContext.Subjects.FindAsync(request.SubjectId);

        if(subject == null)
        {
            throw new NotFoundException(nameof(PGK.Domain.Subject.Subject), request.SubjectId);
        }
        
        TeacherUser teacherUser;

        if(request.Role == UserRole.TEACHER)
        {
            teacherUser = await _dbContext.TeacherUsers
                              .Include(u => u.Subjects)
                              .FirstOrDefaultAsync(u => u.Id == request.UserId)
                          ?? throw new NotFoundException(nameof(TeacherUser), request.UserId);

            if(teacherUser.Subjects.All(u => u.Id != subject.Id))
            {
                throw new ArgumentException("Преподаватель может взаимодействовать только со своим предметом");
            }
        }
        else
        {
            if(request.TeacherId == null)
            {
                throw new ArgumentException("TeacherId not found");
            }

            teacherUser = await _dbContext.TeacherUsers.FindAsync(request.TeacherId) ??
                          throw new NotFoundException(nameof(TeacherUser), request.TeacherId);
        }

        journalSubject.Hours = request.Hours;
        journalSubject.Subject = subject;
        journalSubject.Teacher = teacherUser;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<JournalSubjectDto>(journalSubject);
    }
}