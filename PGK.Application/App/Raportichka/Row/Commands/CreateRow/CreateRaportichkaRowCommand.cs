﻿using MediatR;
using PGK.Domain.User;
using System.ComponentModel.DataAnnotations;
using PGK.Domain.Raportichka;

namespace PGK.Application.App.Raportichka.Row.Commands.CreateRow
{
    public class CreateRaportichkaRowCommand : IRequest<CreateRaportichkaRowVm>
    {
        [Required] public UserRole Role { get; set; }
        [Required] public int UserId { get; set; }

        [Required] public int RaportichkaId { get; set; }
        [Required] public int NumberLesson { get; set; }
        [Required] public int Hours { get; set; }
        [Required] public int SubjectId { get; set; }
        [Required] public List<int> StudentId { get; set; }
        [Required] public RaportichkaCause Cause { get; set; }
        
        public int? TeacherId { get; set; }
    }
}
