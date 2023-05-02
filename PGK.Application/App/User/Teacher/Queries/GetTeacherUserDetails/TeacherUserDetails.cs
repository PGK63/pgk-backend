﻿using AutoMapper;
using PGK.Application.Common.Mappings;
using PGK.Domain.User.Teacher;
using System.ComponentModel.DataAnnotations;
using PGK.Domain.User.Quide;

namespace PGK.Application.App.User.Teacher.Queries.GetTeacherUserDetails
{
    public class TeacherUserDetails : IMapWith<TeacherUser>
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(128)] public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(128)] public string LastName { get; set; } = string.Empty;
        [MaxLength(128)] public string? MiddleName { get; set; }
        [Required] public GuideState State { get; set; }
        public string? Cabinet { get; set; }
        public string? Information { get; set; }

        public string? PhotoUrl { get; set; } = null;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TeacherUser, TeacherUserDetails>();
        }
    }
}
