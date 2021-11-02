// ------------------------------------------------------------------------------
//     <copyright file="StudentProfile.cs" company="BlackLine">
//         Copyright (C) BlackLine. All rights reserved.
//     </copyright>
// ------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace StudentService.Data.Models
{
    public class StudentProfile
    {
        [Key]
        public Guid StudentId { get; set; }

        public long Score { get; set; }

        public Student Student { get; set; }
    }
}