﻿using PGK.Domain.User.Teacher;
using System.ComponentModel.DataAnnotations;

namespace PGK.Domain.Journal
{
    public class JournalSubject
    {
        [Key] public int Id { get; set; }
        [Required] public int Hours { get; set; }
        [Required] public Subject.Subject Subject { get; set; }
        [Required] public TeacherUser Teacher { get; set; }
        [Required] public Journal Journal { get; set; }

        public virtual List<JournalTopic> Topics { get; set; } = new List<JournalTopic>();
        public virtual List<JournalSubjectRow> Rows { get; set; } = new List<JournalSubjectRow>();
    }
}
