using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Domain.Journal;
using PGK.Domain.User;
using PGK.Domain.User.Teacher;

namespace PGK.Application.App.Journal.Commands.DeleteJournalSubjectColumn
{
    internal class DeleteJournalSubjectColumnCommandHandler
        : IRequestHandler<DeleteJournalSubjectColumnCommand>
    {
        private readonly IPGKDbContext _dbContext;

        public DeleteJournalSubjectColumnCommandHandler(IPGKDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<Unit> Handle(DeleteJournalSubjectColumnCommand request,
            CancellationToken cancellationToken)
        {
            var column = await _dbContext.JournalSubjectColumns
                .Include(u => u.Row)
                    .ThenInclude(u => u.JournalSubject)
                        .ThenInclude(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == request.Id);

            if (column == null)
            {
                throw new NotFoundException(nameof(JournalSubjectColumn), request.Id);
            }

            if (request.Role == UserRole.TEACHER)
            {
                if (column.Row.JournalSubject.Teacher.Id != request.UserId)
                {
                    throw new UnauthorizedAccessException("Преподаватель может взаимодействовать только со своим предметом");
                }
            }

            _dbContext.JournalSubjectColumns.Remove(column);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
