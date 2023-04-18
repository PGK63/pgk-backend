﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using PGK.Application.Common;
using PGK.Application.Common.Exceptions;
using PGK.Application.Common.Model;
using PGK.Application.Interfaces;
using PGK.Application.Security;
using PGK.Application.Services.EmailService;

namespace PGK.Application.App.User.Commands.UpdateEmail
{
    public class UserUpdateEmailCommandHandler
        : IRequestHandler<UserUpdateEmailCommand, MessageModel>
    {
        private readonly IPGKDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IAuth _auth;

        public UserUpdateEmailCommandHandler(IPGKDbContext dbContext,
            IEmailService emailService, IAuth auth) =>
            (_dbContext, _emailService, _auth) = (dbContext, emailService, auth);

        public async Task<MessageModel> Handle(UserUpdateEmailCommand request,
            CancellationToken cancellationToken)
        {
            var userByEmail = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if(userByEmail != null)
            {
                return new MessageModel
                {
                    Message = "Пользователь с таким email уже сущестует"
                };
            }

            var user = await _dbContext.Users.FindAsync(request.UserId);

            if(user == null)
            {
                throw new NotFoundException(nameof(Domain.User.User), request.UserId);
            }

            var sendEmailToken = _auth.CreateToken();
            var oldEmail = user.Email;

            user.Email = request.Email;
            user.EmailVerification = false;
            user.SendEmailToken = sendEmailToken;
            await _dbContext.SaveChangesAsync(cancellationToken);

            var date = DateTime.Now.Date;

            var sentEmailVerificationMessageHtml = File.ReadAllText("Html/send_email_verification_message.html");

            sentEmailVerificationMessageHtml = sentEmailVerificationMessageHtml.Replace("{{username}}", user.FirstName);
            sentEmailVerificationMessageHtml = sentEmailVerificationMessageHtml.Replace("{{email}}", user.Email);
            sentEmailVerificationMessageHtml = sentEmailVerificationMessageHtml.Replace("{{url}}", $"{Constants.BASE_URL}/User/{user.Id}/Email/Verification.html?token={sendEmailToken}");

            sentEmailVerificationMessageHtml = sentEmailVerificationMessageHtml.Replace("{{image_src}}", GetPgkIconUrl());

            await _emailService.SendEmailAsync(
                email: user.Email,
                subject: "Подтверждение адреса электронной почты для входа в приложение ПГК",
                message: sentEmailVerificationMessageHtml);

            if (oldEmail != null)
            {
                var sentEmailMessageHtml = File.ReadAllText("Html/send_email_update_email.html");

                sentEmailMessageHtml = sentEmailMessageHtml.Replace("{{username}}", user.FirstName);

                sentEmailMessageHtml = sentEmailMessageHtml.Replace("{{image_src}}", GetPgkIconUrl());

                await _emailService.SendEmailAsync(
                    email: oldEmail,
                    subject: "Email успешно изменен",
                    message: sentEmailMessageHtml);
            }

            return new MessageModel
            {
                Message = "Почта сохранена, мы отправили письмо для подтверждения почты"
            };
        }

        private string GetPgkIconUrl()
        {
            var date = DateTime.Now.Date;

            if ((date.Month == 12 && date.Day >= 20) || (date.Month == 1 && date.Day <= 15))
            {
                return $"{Constants.BASE_URL}/Image/new_year_pgk_icon.png";
            }
            else
            {
                return $"{Constants.BASE_URL}/Image/pgk_icon.png";
            }
        }
    }
}
