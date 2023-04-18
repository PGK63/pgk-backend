﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.App.Journal.Queries.GetJournalSubjectColumnList;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Application.Services.FCMService;
using PGK.Domain.Journal;
using PGK.Domain.Notification;
using PGK.Domain.User;
using PGK.Domain.User.Student;
using PGK.Domain.User.Teacher;

namespace PGK.Application.App.Journal.Commands.CreateJournalSubjectColumn
{
    internal class CreateJournalSubjectColumnCommandHandler
        : IRequestHandler<CreateJournalSubjectColumnCommand, CreateJournalColumnVm>
    {
        private readonly IPGKDbContext _dbContext;
        private readonly IFCMService _fCMService;
        private readonly IMapper _mapper;

        public CreateJournalSubjectColumnCommandHandler(IPGKDbContext dbContext,
            IFCMService fCMService,IMapper mapper) => 
            (_dbContext, _fCMService, _mapper) = (dbContext, fCMService, mapper);

        public async Task<CreateJournalColumnVm> Handle(CreateJournalSubjectColumnCommand request,
            CancellationToken cancellationToken)
        {
            JournalSubjectRow journalSubjectRow;
            bool isOldRow;

            if (request.JournalSubjectRowId != null)
            {
                var row = await _dbContext.JournalSubjectRows
                    .Include(u => u.Student)
                    .Include(u => u.JournalSubject.Teacher)
                    .Include(u => u.JournalSubject.Subject)
                    .FirstOrDefaultAsync(u => u.Id == request.JournalSubjectRowId);

                if (row == null)
                {
                    throw new NotFoundException(nameof(JournalSubjectRow), request.JournalSubjectRowId);
                }else
                {
                    isOldRow = true;
                    journalSubjectRow = row;
                }
            }
            else
            {
                if(request.StudentId == null || request.JournalSubjectId == null)
                {
                    throw new ArgumentException();
                }

                var student = await _dbContext.StudentsUsers.FindAsync(request.StudentId);

                if(student == null)
                {
                    throw new NotFoundException(nameof(StudentUser), request.StudentId);
                }

                var journalSubject = await _dbContext.JournalSubjects
                    .Include(u => u.Teacher)
                    .Include(u => u.Subject)
                    .FirstOrDefaultAsync(u => u.Id == request.JournalSubjectId);

                if (journalSubject == null)
                {
                    throw new NotFoundException(nameof(JournalSubject), request.JournalSubjectId);
                }

                isOldRow = false;
                
                journalSubjectRow = new JournalSubjectRow
                {
                    Student = student,
                    JournalSubject = journalSubject,
                };
            }
            
            var column = new JournalSubjectColumn
            {
                Evaluation = request.Evaluation,
                Date = request.Date,
                Row = journalSubjectRow
            };

            if(request.Role == UserRole.TEACHER)
            {
                var teacher = await _dbContext.TeacherUsers.FindAsync(request.UserId);

                if(teacher == null)
                {
                    throw new NotFoundException(nameof(TeacherUser), request.UserId);
                }

                if(journalSubjectRow.JournalSubject.Teacher.Id != teacher.Id)
                {
                    throw new UnauthorizedAccessException("Преподаватель может взаимодействовать только со своим предметом");
                }
            }

            var notification = new Notification
            {
                Title = "Вам поставили оценку в журнале",
                Message = $"Оценка {column.Evaluation}," +
                $" предмет {journalSubjectRow.JournalSubject.Subject.SubjectTitle}, " +
                $"преподаватель {journalSubjectRow.JournalSubject.Teacher.LastName}",
                Users = new List<Domain.User.User> { journalSubjectRow.Student }
            };

            await _dbContext.JournalSubjectColumns.AddAsync(column);
            await _dbContext.Notifications.AddAsync(notification);

            await _dbContext.SaveChangesAsync(cancellationToken);

            if (journalSubjectRow.Student.IncludedJournalNotifications)
            {
                await _fCMService.SendMessage(
                    notification.Title,
                    notification.Message,
                    notification.Users.Last().Id.ToString()
               );
            }

            return new CreateJournalColumnVm
            {
                IsOldRow = isOldRow,
                ColumnDto = _mapper.Map<CreateJournalColumnDto>(column)
            };
        }
    }
}
