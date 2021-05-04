using System;
using System.ComponentModel.DataAnnotations;

namespace StudentService.Data.Models
{
    public class Student
    {
        [Key]
        public Guid StudentId { get; set; }

        public string Name { get; set; }
    }
}
