namespace PGK.WebApi.Models.Journal;

public class UpdateJournalSubjectModal
{
    public int Hours { get; set; }
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
}