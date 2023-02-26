using System;
using System.CodeDom;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskBot.Util
{
    // Модель Заявка
    public class AppTask
    {
        // ID 
        public Guid Id { get; set; }
        // ID 
        public int Number { get; set; }
        // Район
        [Required(ErrorMessage = "Район должен быть выбран")]
        [Display(Name = "Название района")]
        public string District { get; set; }
        // Филиал
        [Required(ErrorMessage = "Офис должен быть выбран")]
        [Display(Name = "Название офиса")]
        public string Office { get; set; }
        // Описание
        [Required(ErrorMessage = "Поле 'Описание заявки' должно быть заполнено'")]
        [Display(Name = "Описание заявки")]
        [MaxLength(300, ErrorMessage = "Превышена максимальная длина записи")]
        public string Description { get; set; }
        // Комментарий к заявке
        public Guid? CommentId { get; set; }
        public Comment Comment { get; set; }
        // Статус заявки
        [Display(Name = "Статус")]
        public int Status { get; set; }
        // Приоритет заявки
        [Required(ErrorMessage = "Приоритет выполнения должнен быть выбран'")]
        [Display(Name = "Приоритет")]
        public int Priority { get; set; }
        // Файл ошибки
        [Display(Name = "Файл с ошибкой")]
        public string File { get; set; }
        // Внешний ключ Категория
        [Display(Name = "Категория")]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        // Внешний ключ
        // Отдел пользователя - Навигационное свойство
        [Display(Name = "Исполнитель")]
        public Guid? ExecutorId { get; set; }
        public User Executor { get; set; }
        // Внешний ключ
        // ID жизненного цикла заявки - обычное свойство
        public Guid LifecycleId { get; set; }
        // Ссылка на жизненный цикл заявки - Навигационное свойство
        public Lifecycle Lifecycle { get; set; }
        // ID создателя
        public Guid? CreateUserId { get; set; }
        public User CreateUser { get; set; }
        public Guid? CreateDistrictId  {get; set; }
        public District CreateDistrict { get; set; }
        public Guid? DepartmentId { get; set; }
        //Ссылка на пользователя - навигационное свойство
        public Department Department { get; set; }
       //Ссылка на пользователя - навигационное свойство
        public Guid? DistrictId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? LastUpdate { get; set; }

        [Required(ErrorMessage = "Поле 'Тема' должно быть заполнено'")]
        [Display(Name = "Тема заявки")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Subject { get; set; }
    }

    // Модель жизенного цикла заявки
    public class Lifecycle
    {
        // ID 
        public Guid Id { get; set; }
        // Дата открытия
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy H:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Opened { get; set; }
       // Дата обработки
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy H:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Proccesing { get; set; }
        // Дата проверки
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy H:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Checking { get; set; }
        // Дата закрытия
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy H:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Closed { get; set; }
    }

    // Перечисление для статуса заявки
    public enum RequestStatus
    {
        Opened = 1,
        Proccesing = 2,
        Checking = 3,
        Closed = 4
    }

    // Перечисление для приоритета заявки
    public enum RequestPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
}