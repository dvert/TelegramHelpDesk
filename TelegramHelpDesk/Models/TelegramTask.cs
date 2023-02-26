using System;
using System.ComponentModel.DataAnnotations;

namespace TelegramHelpDesk.Models
{
    // Модель Заявка
    public class TelegramTask
    {
        // ID 
        public Guid Id { get; set; }
        // Номер чата
        public int ChatId { get; set; }
        // Район
        [Display(Name = "Название района")]
        public string District { get; set; }
        // Филиал
        [Display(Name = "Название офиса")]
        public string Office { get; set; }
        // Описание
        [Display(Name = "Описание")]
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