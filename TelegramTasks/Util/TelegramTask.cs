using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpDeskBot.Util
{
    // Модель Заявка
    public class TelegramTask
    {
        // ID 
        public Guid Id { get; set; }
        // Номер чата
        public int ChatId { get; set; }
        // Район
        [Required(ErrorMessage = "Поле 'Название заявки' должно быть заполнено")]
        [Display(Name = "Название района")]
        [MaxLength(100, ErrorMessage = "Превышена максимальная длина записи")]
        public string District { get; set; }
        // Филиал
        [Required(ErrorMessage = "Поле 'Название заявки' должно быть заполнено")]
        [Display(Name = "Название района")]
        [MaxLength(100, ErrorMessage = "Превышена максимальная длина записи")]
        public string Office { get; set; }
        // Описание
        [Required(ErrorMessage = "Поле 'Описание' должно быть заполнено'")]
        [Display(Name = "Описание")]
        [MaxLength(300, ErrorMessage = "Превышена максимальная длина записи")]
        public string Description { get; set; }
        // Статус заявки
        [Display(Name = "Статус")]
        public int Status { get; set; }
        // Файл ошибки
        [Display(Name = "Файл с ошибкой")]
        public string File { get; set; }
        //
        [Display(Name = "botId")]
        public string BotId { get; set; }
        //
        [Display(Name = "channelId")]
        public string ChannelId { get; set; }
        //
        [Display(Name = "userId")]
        public string UserId { get; set; }
        //
        [Display(Name = "ConversationId")]
        public string ConversationId { get; set; }
        //
        [Display(Name = "serviceUrl")]
        public string ServiceUrl { get; set; }
        //
        [Display(Name = "userName")]
        public string UserName { get; set; }
        //
        [Display(Name = "isGroup")]
        public bool IsGroup { get; set; }
    }
}