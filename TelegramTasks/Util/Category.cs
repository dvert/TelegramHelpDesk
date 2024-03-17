﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpDeskBot.Util
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Название категории")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Name { get; set; }
        public Guid? DepartmentId { get; set; }
        //Ссылка на пользователя - навигационное свойство
        public Department Department { get; set; }
    }
}