using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Data
{
    public class Student
    {
        [Key]
        public Guid StudentId { get; set; }

        public string Name { get; set; }
    }
}
