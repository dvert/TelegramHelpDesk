using System.Configuration;
using System.Data;
using System.Linq;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables;
using Newtonsoft.Json.Linq;
using TableDependency.Enums;


namespace HelpDeskBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Util;
    using System.Net.Http;
    using System.Net;
    using TableDependency.SqlClient;
    using TableDependency;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string description;
        private string office;
        private string districts;
        private string change;
        private string contentUrl;
        private string contentName;
        private string path;
        private string fullPath;
        private static Guid rest;
        private string subject;

        private static readonly string conn = ConfigurationManager.ConnectionStrings["HelpdeskContext"].ToString();

        public Task StartAsync(IDialogContext context) {
           
            context.Wait(this.MessageReceivedAsync);
           
            return Task.CompletedTask;
        }
        
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            HelpdeskContext db = new HelpdeskContext();

            var message = await argument;

            if (message.Value != null) {

                var obj = JObject.Parse(message.Value.ToString());

                AppTask tasks = db.AppTasks.Find(Guid.Parse(obj["id"].ToString()));
                Lifecycle lifecycle = db.Lifecycles.Find(tasks.LifecycleId);

                if (obj["text"].ToString() == "Да") {

                    tasks.Status = 5;
                    lifecycle.Closed = DateTime.Now;
                    db.SaveChanges();

                    await context.PostAsync("Спасибо за обращение, Ваша заявка закрыта");
                }
                else if (obj["text"].ToString() == "Нет") {

                    tasks.Status = 3;
                    db.SaveChanges();

                    await context.PostAsync("Ваша заявка была возвращена для дальнейшего выполнения");
                }
            }
            else
            {
                await context.PostAsync("Привет! Я собираю заявки для отдела информационных технологий. Я помогу вам всё правильно заполнить!");
                var newtask = new string[] {"Создать заявку", "Отменить"};
                PromptDialog.Choice(context, this.DistrictReceivedAsync, newtask, "Если хотите оставить заявку, нажмите");
            }
        }

        public async Task DistrictReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            
            this.description = await argument;
            if (description == "Отменить")
            {
              
                context.Wait(this.MessageReceivedAsync);

                await context.PostAsync("Создание заявки было отменено. Для создания новой заявки, просто напишите мне об этом");
            }
            if (description == "Создать заявку")
            {
                HelpdeskContext db = new HelpdeskContext();

                var districtsList = db.Districts.GroupBy(d => d.DistrictName)
                        .Select(di => di.FirstOrDefault()
                            .DistrictName)
                        .ToList();
                var districts = new string[districtsList.Count];
                for (int i = 0; i < districtsList.Count; i++)
                {
                    districts[i] = districtsList[i];
                }

                PromptDialog.Choice(context, this.OfficetReceivedAsync, districts, "Для начала выберите свой район");
            }
        }

        public async Task OfficetReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            HelpdeskContext db = new HelpdeskContext();
            this.districts = await argument;

            var officeList = db.Districts.Where(r=>r.DistrictName== districts)
                .Select(r=>r.OfficeName)
                .ToList();
            var offices = new string[officeList.Count];
            for (int i = 0; i < officeList.Count; i++)
            {
                offices[i] = officeList[i];
            }
            PromptDialog.Choice(context, this.SubjectReceivedAsync, offices, "Выберите центр, в котором наблюдается проблема");
        }

        public async Task SubjectReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.office = await argument;
            PromptDialog.Text(context, this.DescriptionReceivedAsync, "В несколько слов опишите тему вашего обращение (например: Не работает принтер)");
        }

        public async Task DescriptionReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.subject = await argument;
            PromptDialog.Text(context, this.ChangeAddFileReceivedAsync, "Пожалуйста, кратко опишите вашу проблему");
        }

        public async Task ChangeAddFileReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.description = await argument;
            var change = new string[] { "Прикрепить", "Не прикреплять" };
            PromptDialog.Choice(context,this.FileReceivedAsync,change,"К заявке можно прикрепить фотографию/документ с описанием вашей проблемы");
           }

        public async Task FileReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.change = await argument;
            if (change == "Не прикреплять") {

                var text = $"Отлично! Создаю завку для \"{this.districts}\" в центре  \"{this.office}\". " +
                           $"Ваша заявку заключается в следующем: \"{this.description}\". Заявка сформирована верно?";

                PromptDialog.Confirm(context, this.IssueConfirmedMessageReceivedAsync, text);
            }
            else {

                PromptDialog.Attachment(context, this.ResumeAfterAttachment, "Выберите файл");
            }
        }
        
        private async Task ResumeAfterAttachment(IDialogContext context, IAwaitable<IEnumerable<Attachment>> argument) {
           
            // Тут короче принимается файл, а не фотография

            var orders = await argument;
            
            foreach (var order in orders) {
                contentName = order.Name;
                contentUrl = order.ContentUrl;
               
                using (WebClient client = new WebClient())
                {
                    DateTime current = DateTime.Now;
                    //Получаем расширение
                    string ext = contentName.Substring(contentName.LastIndexOf('.'));
                    //сохраняем файл по определенному пути на сервере
                    path = current.ToString("dd.mm.yyyy hh:mm").Replace(":", "_").Replace("/", ".") + ext;

                    fullPath = @"c:\inetpub\wwwroot\TelegramHelpdesk\Files\" + path;

                    client.DownloadFile(contentUrl, fullPath);
                }

                PromptDialog.Text(context, this.CategoryMessageReceivedAsync, "Вы отправили мне файл, он успешно добавлен к заявке!");
            }
            var a = context.PostAsync("Вы отправили мне файл, он успешно добавлен к заявке!");
            context.Done(a);
        }

        public async Task CategoryMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            
            var text = $"Отлично! Создаю завку для \"{this.districts}\" в центре  \"{this.office}\". " +
                       $"Ваша заявка заключается в следующем: \"{this.description}\"." + "\n" + "К вашей заявке, прикреплен файл. Заявка сформирована верно?";
            PromptDialog.Confirm(context, this.IssueConfirmedMessageReceivedAsync, text);
           
        }

        [System.Web.Http.HttpPost]
        public async Task IssueConfirmedMessageReceivedAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var message = context.MakeMessage();
            var resume = new ResumptionCookie(message);
            HelpdeskContext db = new HelpdeskContext();
            var taskId = Guid.NewGuid();
            var lifecycleId = Guid.NewGuid();
            var coun = db.AppTasks.Count()+1;

            TelegramTask telegramTask = new TelegramTask()
            {
                ChatId = 1,
                District = this.districts,
                Office = this.office,
                Description = this.description,
                Id = taskId,
                Status = 1,
                File = path,
                BotId = resume.Address.BotId,
                ChannelId = resume.Address.ChannelId,
                UserId = resume.Address.UserId,
                ConversationId = resume.Address.ConversationId,
                ServiceUrl = resume.Address.ServiceUrl,
                UserName = resume.UserName,
                IsGroup = resume.IsGroup
            };

            Lifecycle newLifecycle = new Lifecycle() { Opened = DateTime.Now, Id = lifecycleId };

            db.TelegramTasks.Add(telegramTask);
            db.Lifecycles.Add(newLifecycle);
            db.SaveChanges();

            Department dep = db.Departments.Where(m => m.Name == "ИТ отдел").First();

            AppTask appTask = new AppTask()
            {
                Id = taskId,
                Number = coun,
                District = this.districts,
                Office = this.office,
                Description = this.description,
                Status = 1,
                Priority = 1,
                LifecycleId = lifecycleId,
                Subject = this.subject,
                File = path,
                DepartmentId = dep.Id
        };

            
            db.AppTasks.Add(appTask);
            db.SaveChanges();

            path = null;
            var confirmed = await argument;

            if (confirmed)
            {
                if (coun != -1)
                {

                    var attachment = new Attachment()
                    {
                            ContentType = "application/vnd.microsoft.card.adaptive",
                            Content = this.CreateCard(coun, this.districts, this.office,this.description),
                    };

                    message.Attachments.Add(attachment);
                    fullPath = null;

                    await context.PostAsync((Activity)message);
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    fullPath = null;
                    await context.PostAsync("Упс! Что-то пошло не так, при попытке сохранить заявку. Пропробуйте еще раз.");
                }
            }
            else
            {
                fullPath = null;
                await context.PostAsync("Хорошо. Заявка не была создана. Вы можете начать заново, если хотите.");
            }

            context.Done<object>(null);
        }

        

        private AdaptiveCard CreateCard(int coun, string districts, string office, string description)
        {
            AdaptiveCard card = new AdaptiveCard();

            var headerBlock = new TextBlock()
            {
                Text = $"Заявка #{coun}." +"\n\r"+"Ваша зявка скоро будет обработана",
                Weight = TextWeight.Bolder,
                Size = TextSize.Medium
            };

          
            var columnsBlock = new ColumnSet()
            {
                Columns = new List<Column>
                {
                    new Column
                    {
                        Size = "1",
                        Items = new List<CardElement>
                        {
                            new FactSet
                            {
                                Facts = new List<AdaptiveCards.Fact>
                                {
                                    new AdaptiveCards.Fact("Район:", districts),
                                    new AdaptiveCards.Fact("Центр:", office),
                                    new AdaptiveCards.Fact("Тема:", subject),
                                    new AdaptiveCards.Fact("Описание:", description),
                                }
                            },
                        }
                    },
                    new Column
                    {
                        Size = "auto",
                        Items = new List<CardElement>
                        {
                            new Image
                            { 
                                Size = ImageSize.Small,
                                HorizontalAlignment = HorizontalAlignment.Right
                            }
                        }
                    }
                }
            };

            card.Body.Add(headerBlock);
            card.Body.Add(columnsBlock);

            return card;

       
        }

        public static async Task RequestText(Guid requestId, Guid executorId)
        {
            rest = requestId;
            HelpdeskContext db = new HelpdeskContext();

            var arg = db.TelegramTasks.Find(rest);
            var executor = db.Users.Find(executorId)?.Name;
            var description = db.AppTasks.Find(requestId);
            var userAccount = new ChannelAccount() { Id = arg.UserId, Name = arg.UserName };
            var botAccount = new ChannelAccount() { Id = arg.ChannelId, Name = "telegramBot" };
            string url = arg.ServiceUrl;

            var connector = new ConnectorClient(new Uri(url));
            IMessageActivity message = Activity.CreateMessageActivity();
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount() { Id = arg.ConversationId };
            message.Locale = "ru";

            AppTask request = db.AppTasks.First(r => r.Id == requestId);
            var coun = request.Number;
            message.Attachments = new List<Attachment>
            {
                new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content =  AnswerCard(coun, executor, description?.Description),
                }
            };

            await connector.Conversations.SendToConversationAsync((Activity)message);
        }



        private static AdaptiveCard AnswerCard(int coun,string executor,string description)
        {
            AdaptiveCard card = new AdaptiveCard();

            var headerBlock = new TextBlock()
            {
                Text = $"Ваша заявка #{coun} взята в обработку",
                Weight = TextWeight.Bolder,
                Size = TextSize.Medium
            };

            var columnsBlock = new ColumnSet()
            {
                Separation = SeparationStyle.Strong,
                Columns = new List<Column>
                {
                    new Column
                    {
                        Size = "1",
                        Items = new List<CardElement>
                        {
                            new FactSet
                            {
                                Facts = new List<AdaptiveCards.Fact>
                                {
                                    new AdaptiveCards.Fact("Исполнитель:", executor),
                                    new AdaptiveCards.Fact("Описание:",description)
                                }
                            }
                         }
                    },
                    new Column
                    {
                        Size = "auto",
                        Items = new List<CardElement>
                        {
                            new Image
                            {
                                Size = ImageSize.Small,
                                HorizontalAlignment = HorizontalAlignment.Right
                            }
                        }
                    }
                }
            };

            card.Body.Add(headerBlock);
            card.Body.Add(columnsBlock);

            return card;
        }


        public static async Task EndText(Guid requestId) {
            rest = requestId;
            HelpdeskContext db = new HelpdeskContext();

            var arg = db.TelegramTasks.Find(rest);
            var description = db.AppTasks.Find(requestId);
            var userAccount = new ChannelAccount() {Id = arg.UserId, Name = arg.UserName};
            var botAccount = new ChannelAccount() {Id = arg.ChannelId, Name = "telegramBot"};
            string url = arg.ServiceUrl;

            var connector = new ConnectorClient(new Uri(url));
            IMessageActivity message = Activity.CreateMessageActivity();
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount() {Id = arg.ConversationId};
            message.Locale = "ru";

            AppTask request = db.AppTasks.First(r => r.Id == requestId);
            var coun = request.Number;
            message.Attachments = new List<Attachment> {
                new Attachment {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = EndCard(coun, description?.Description, requestId),
                }
            };

            await connector.Conversations.SendToConversationAsync((Activity) message);
        }


        private static AdaptiveCard EndCard(int coun,string description,Guid requestId)
        {
            AdaptiveCard card = new AdaptiveCard();

            var headerBlock = new TextBlock()
            {
                Text = $"Исполнение вашей заявки #{coun}" + "\r" + "завершено",
                Weight = TextWeight.Bolder,
                Size = TextSize.Medium
            };

            var columnsBlock = new ColumnSet()
            {
                Separation = SeparationStyle.Strong,
                Columns = new List<Column>
                {
                    new Column
                    {
                        Size = "1",
                        Items = new List<CardElement>
                        {
                            new FactSet
                            {
                                Facts = new List<AdaptiveCards.Fact>
                                {
                                    new AdaptiveCards.Fact("Описание:",description)
                                }
                            },

                         }
                    },

                    new Column
                    {
                        Size = "auto",
                        Items = new List<CardElement>
                        {
                            new Image
                            {
                                 Size = ImageSize.Small,
                                HorizontalAlignment = HorizontalAlignment.Right
                            }

                        }

                    }

                }

            };

            card.Body.Add(headerBlock);
            card.Body.Add(columnsBlock);
            card.Body.Add(new TextBlock()
            {
                Text = "Проблема устранена?" + "\n\r" + "В случае, если проблема не устранена," + "\n\r" + "работа с заявкой будет возобновлена."
            });
            card.Actions.Add(new SubmitAction()
            {
                Title = "Да, всё в порядке",
                Data = Newtonsoft.Json.Linq.JObject.FromObject(new {text = "Да", id = requestId})
            });
            
            card.Actions.Add(new SubmitAction()
            {
                Title = "Нет, не исправлена",
                Data = Newtonsoft.Json.Linq.JObject.FromObject(new { text = "Нет", id = requestId })
            });

            return card;
        }


       


        public static void StartWatching() {
            WatchTable();
            StartTableWatcher();
        }


        private static SqlTableDependency<AppTask> _dependency;
        public static void WatchTable()
        {
            var mapper = new ModelToTableMapper<AppTask>();
            mapper.AddMapping(model => model.Status, "Status");
            _dependency = new SqlTableDependency<AppTask>(conn, "AppTasks", mapper);
            _dependency.OnChanged += _dependency_OnChanged;
            _dependency.OnError += _dependency_OnError;

        }

        public static void StartTableWatcher()
        {
            _dependency.Start();
        }
        public void StopTableWatcher()
        {
            _dependency.Stop();
        }


        static void _dependency_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {

            throw e.Error;
        }


        static void _dependency_OnChanged(object sender, TableDependency.EventArgs.RecordChangedEventArgs<AppTask> e)
        {

            if (e.ChangeType != ChangeType.None)
            {
                    if(e.ChangeType == ChangeType.Update && e.Entity.Status==3) {
                            
                            RequestText(e.Entity.Id, Guid.Parse(e.Entity.ExecutorId.ToString()));
                    }
                    else if (e.ChangeType == ChangeType.Update && e.Entity.Status == 4)
                    {
                        EndText(e.Entity.Id);
                    }
            }
        }
    }   

    }

