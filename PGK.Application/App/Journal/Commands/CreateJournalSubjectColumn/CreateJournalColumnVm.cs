namespace PGK.Application.App.Journal.Commands.CreateJournalSubjectColumn;

public class CreateJournalColumnVm
{
    public bool IsOldRow { get; set; } = false;
    public CreateJournalColumnDto ColumnDto { get; set; } = new();
}