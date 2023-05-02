﻿using PGK.Domain.Raportichka;

namespace PGK.WebApi.Models.Teacher
{
    public class TeacherUpdateRaportichkaRowModel
    {
        public int NumberLesson { get; set; }
        public int Hours { get; set; }
        public int SubjectId { get; set; }
        public int StudentId { get; set; }
        public int RaportichkaId { get; set; }
        public RaportichkaCause Cause { get; set; }
    }
}
