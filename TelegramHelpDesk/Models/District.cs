using System;
using System.ComponentModel.DataAnnotations;

namespace TelegramHelpDesk.Models
{
    // модель районы
    public class District
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле 'Название района' должно быть заполнено'")]
        [Display(Name = "Название района")]
        [MaxLength(100, ErrorMessage = "Превышена максимальная длина записи")]
        public string DistrictName { get; set; }

        [Required(ErrorMessage = "Поле 'Название офиса' должно быть заполнено'")]
        [Display(Name = "Название офиса")]
        [MaxLength(100, ErrorMessage = "Превышена максимальная длина записи")]
        public string OfficeName { get; set; }

        public Guid DistrictId { get; set; }
    }
}