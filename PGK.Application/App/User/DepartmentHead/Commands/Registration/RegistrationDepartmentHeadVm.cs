﻿using System.ComponentModel.DataAnnotations;

namespace PGK.Application.App.User.DepartmentHead.Commands.Registration
{
    public class RegistrationDepartmentHeadVm
    {
        [Required] public string AccessToken { get; set; } = string.Empty;
        [Required] public string RefreshToken { get; set; } = string.Empty;

        [Required] public int UserId { get; set; }
        [Required] public string Passowrd { get; set; } = string.Empty;
        [Required] public string UserRole { get; set; } = string.Empty;
    }
}
