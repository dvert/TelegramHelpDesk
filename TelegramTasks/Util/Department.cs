using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskBot.Util
{
    // модель отделы
    public class Department
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Название отдела")]
        public string Name { get; set; }
    }
}