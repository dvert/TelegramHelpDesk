using System;
using System.ComponentModel.DataAnnotations;

namespace TelegramHelpDesk.Models
{
    public class Comment
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        
        public Guid? TaskId { get; set; }
        
        [Required(ErrorMessage = "Поле 'комментарий' должно быть заполнено'")]
        [Display(Name = "Комментарий")]
        [MaxLength(300, ErrorMessage = "Превышена максимальная длина записи")]
        public string Text { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}