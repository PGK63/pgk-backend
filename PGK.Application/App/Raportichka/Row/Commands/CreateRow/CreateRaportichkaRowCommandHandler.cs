﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Common.Exceptions;
using PGK.Application.Interfaces;
using PGK.Application.Services.FCMService;
using PGK.Domain.Notification;
using PGK.Domain.Raportichka;
using PGK.Domain.User;
using PGK.Domain.User.Student;
using PGK.Domain.User.Teacher;

namespace PGK.Application.App.Raportichka.Row.Commands.CreateRow
{
    internal class CreateRaportichkaRowCommandHandler
        : IRequestHandler<CreateRaportichkaRowCommand, CreateRaportichkaRowVm>
    {
        private readonly IPGKDbContext _dbContext;
        private readonly IFCMService _fCMService;

        public CreateRaportichkaRowCommandHandler(IPGKDbContext dbContext, IFCMService fCMService) =>
            (_dbContext, _fCMService) = (dbContext, fCMService);

        public async Task<CreateRaportichkaRowVm> Handle(CreateRaportichkaRowCommand request,
            CancellationToken cancellationToken)
        {
            var teacherId = request.Role == UserRole.TEACHER ? request.UserId : request.TeacherId;

            var teacher = await _dbContext.TeacherUsers
                .Include(u => u.RaportichkaRows)
                    .ThenInclude(u => u.Raportichka)
                .FirstOrDefaultAsync(u => u.Id == teacherId);

            if (teacher == null)
            {
                throw new NotFoundException(nameof(TeacherUser), teacherId ?? 0);
            }

            //if (
            //    request.Role == UserRole.TEACHER && !teacher.RaportichkaRows
            //    .Any(u => u.Raportichka.Id == request.RaportichkaId)
            //    )
            //{
            //    throw new UnauthorizedAccessException("");
            //}

            var raportichka = await _dbContext.Raportichkas
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Id == request.RaportichkaId);

            if (raportichka == null)
            {
                throw new NotFoundException(nameof(Domain.Raportichka.Raportichka),
                    request.RaportichkaId);
            }

            if (request.Role == UserRole.HEADMAN || request.Role == UserRole.DEPUTY_HEADMAN)
            {
                var studentHeadman = await _dbContext.StudentsUsers
                    .Include(u => u.Group)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId);

                if (studentHeadman == null)
                {
                    throw new NotFoundException(nameof(StudentUser), request.RaportichkaId);
                }

                if(raportichka.Group.Id != studentHeadman.Group.Id)
                {
                    throw new UnauthorizedAccessException("Вы пытаетесь изменить не свою группу");
                }
            }

            var subject = await _dbContext.Subjects.FindAsync(request.SubjectId);

            if (subject == null)
            {
                throw new NotFoundException(nameof(Domain.Subject.Subject),
                    request.SubjectId);
            }

            var studentUsers = request.StudentId.Select(u => getStudent(u, raportichka.Group.Id));
            var rowIds = new List<int>();

            foreach (var studentUser in studentUsers)
            {
                var student = await studentUser;
                
                var row = new RaportichkaRow
                {
                    NumberLesson = request.NumberLesson,
                    Confirmation = false,
                    Hours = request.Hours,
                    Student = student,
                    Subject = subject,
                    Teacher = teacher,
                    Raportichka = raportichka,
                    Cause = request.Cause
                };

                var notification = new Notification
                {
                    Title = "Вас отметели в рапортичке",
                    Message = $"Преподаватель {teacher.LastName}, предмет {subject.SubjectTitle}",
                    Users = new List<Domain.User.User> { student }
                };

                await _dbContext.RaportichkaRows.AddAsync(row, cancellationToken);
                await _dbContext.Notifications.AddAsync(notification, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);
                rowIds.Add(row.Id);

                if (student.IncludedRaportichkaNotifications)
                {
                    await _fCMService.SendMessage(
                        notification.Title,
                        notification.Message,
                        notification.Users.Last().Id.ToString()
                    );
                }   
            }

            return new CreateRaportichkaRowVm
            {
                Id = rowIds
            };
        }

        async Task<StudentUser> getStudent(int studentId, int groupId)
        {
            var student = await _dbContext.StudentsUsers
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Id == studentId);
            
            if (student == null)
            {
                throw new NotFoundException(nameof(StudentUser), studentId);
            }

            if(groupId != student.Group.Id)
            {
                throw new Exception("У студент и рапортички разные группы");
            }

            return student;
        }
    }
}
