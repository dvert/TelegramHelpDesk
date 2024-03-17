using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpDeskBot.Util
{
    public class District
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Название района")]
        [MaxLength(100, ErrorMessage = "Превышена максимальная длина записи")]
        public string DistrictName { get; set; }

        [Required]
        [Display(Name = "Название офиса")]
        [MaxLength(100, ErrorMessage = "Превышена максимальная длина записи")]
        public string OfficeName { get; set; }
    }
}