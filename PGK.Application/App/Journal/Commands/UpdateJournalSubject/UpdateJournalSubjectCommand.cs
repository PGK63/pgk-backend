using MediatR;
using PGK.Application.App.Journal.Queries.GetJournalSubjectList;
using PGK.Domain.User;

namespace Market.Application.App.Journal.Commands.UpdateJournalSubject;

public class UpdateJournalSubjectCommand : IRequest<JournalSubjectDto>
{
    public int JournalSubjectId { get; set; }
    
    public int Hours { get; set; }
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}