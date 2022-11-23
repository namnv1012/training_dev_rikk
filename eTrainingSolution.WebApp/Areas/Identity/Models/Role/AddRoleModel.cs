﻿using eTrainingSolution.EntityFrameworkCore.Validation.Role;
using System.ComponentModel.DataAnnotations;

namespace eTrainingSolution.WebApp.Areas.Identity.Models.Role
{
    public class AddRoleModel
    {
        [Required(ErrorMessage = "Bạn phải nhập vai trò")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự")]
        [Display(Name = "Vai trò")]
        [ValidateRoleName]
        public string? RoleName { get; set; }
    }
}
