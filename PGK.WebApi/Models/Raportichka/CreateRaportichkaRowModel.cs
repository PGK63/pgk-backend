﻿using System.ComponentModel.DataAnnotations;
using PGK.Domain.Raportichka;

namespace PGK.WebApi.Models.Raportichka
{
    public class CreateRaportichkaRowModel
    {
        [Required] public int NumberLesson { get; set; }
        [Required] public int Hours { get; set; }
        [Required] public RaportichkaCause Cause { get; set; }
        [Required] public int SubjectId { get; set; }
        [Required] public List<int> StudentId { get; set; }
        [Required] public int TeacherId { get; set; }
    }
}
