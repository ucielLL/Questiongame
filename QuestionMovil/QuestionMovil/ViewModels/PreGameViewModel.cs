using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using FreshMvvm.Popups;
using QuestionService.Models;
using QuestionService.Service;
using Rg.Plugins.Popup.Contracts;
using TheMessage;
using Xamarin.Forms;

namespace QuestionMovil.ViewModels
{
    public class PreGameViewModel : FreshBasePageModel
    {
        public PreGameViewModel(IQstnService Service)
        {
            _Service = Service;
            _Service.ReceivedSyncUserEvent += _Service_ReceivedSyncUserEvent;
        }
        public override void Init(object initData)
        {
            base.Init(initData);
            MyUser = initData as User;
            UserInvite = null;
            MyUserIsIvite(false);
        }
        public void ReverseInit()
        {
            
            UserInviteName = "sin user";
            UserInvteAvatar = "sinuser";
            UserInvite = null;
            MyUserIsIvite(false);
        }
        public ICommand RandomInviteCommand => new Command(async () =>
        {
           
            if (ListInvites.Count == 0 || LastUserIntive >= ListInvites.Count)
            {
                ListInvites = await _Service.SearchUser(MyUser.Language);
                LastUserIntive = 0;
            }

            if (ListInvites.Count > 0)
            {
                var msg = new SyncUsers()
                {
                    UserInvited = ListInvites[LastUserIntive].UserName,
                    Sender = MyUser.UserName,
                    avatar = MyUser.Avatar,
                    Receiver = ListInvites[LastUserIntive].UserName
                };
                await _Service.SyncUsersAsync(msg);
                LastUserIntive++;
            }
            
        });
        public ICommand InviteUserCommand => new Command(async () =>
        {
            var respons = await this.CurrentPage.DisplayPromptAsync("Jugar con...", $"Idioma: {MyUser.Language}\n Nombre de usuario", placeholder: "usuario1");
            if (string.IsNullOrEmpty(respons)) return;
            User user = await _Service.SearchUser(respons, MyUser.Language);
            if (user != null && user.Estado == "wait")
            {
                var msg = new SyncUsers()
                {
                    UserInvited = user.UserName,
                    Sender = MyUser.UserName,
                    avatar = MyUser.Avatar,
                    Receiver = user.UserName
                };
                await _Service.SyncUsersAsync(msg);
            }
            else
            {
                await CoreMethods.DisplayAlert("Error", $"el usuario {respons} no se encontro o no esta disponible", "Ok");
            }
        });
        public ICommand CategoryCommand => new Command<string>(
            execute:(date) =>
            {
                Category = date;
                StartEnablecheck();
            });
        public Command StartCommand => new Command
            ( async () =>
            {
                    InitGame init = new InitGame
                    {
                        Sender = MyUser.UserName,
                        Receiver = UserInvite.UserName,
                        Category = this.Category,
                    };
                    await _Service.StarGame(init);
            } );
          void MyUserIsIvite(bool isInvite) 
        {
            CancelVisible = isInvite;
            StartVisible = !isInvite;
        }
        void StartEnablecheck() 
        {
            StartIsEnable = !string.IsNullOrEmpty(Category) && UserInvite != null;
        }
        public ICommand CancelInviteCommand => new Command(async()=>
        {
            var msg = new SyncUsers
            {
                Sender = MyUser.UserName,
                Receiver = UserInvite.UserName,
                UserInvited = MyUser.UserName,
                Accept = false
            };
            await _Service.SyncUsersAsync(msg);
            MyUserIsIvite(false);
            UserInviteName = "sin user";
            UserInvteAvatar = "sinuser";
            UserInvite = null;
        });
        private async Task _Service_ReceivedSyncUserEvent(MessageBase e)
        {
            if (e.Typeobject == nameof(SyncUsers) && e != null)
            {
                var user = e as SyncUsers;
                if (user.UserInvited == MyUser.UserName)
                {
                    var acetp = await CoreMethods.DisplayAlert("invitacion", $"¿Quieres jugar con {user.Sender}","Sí","No");
                    var msg = new SyncUsers
                    {
                        Sender = MyUser.UserName,
                        Receiver = user.Sender,
                        UserInvited = user.UserInvited,
                        Accept = acetp,
                        avatar = MyUser.Avatar
                    };
                   await _Service.SyncUsersAsync(msg);
                    if (acetp)
                    {
                        UserInvite = new User
                        {
                            Language = MyUser.Language
                        };
                        UserInviteName = user.Sender;
                        UserInvteAvatar = user.avatar;
                        MyUserIsIvite(acetp);
                    }
                  
                }
                else if (user.Receiver == MyUser.UserName && user.UserInvited != MyUser.UserName)
                {
                    var acept = user.Accept ? "acepto" : "rechazo";
                    await CoreMethods.DisplayAlert("Invitación", $"{user.UserInvited} {acept} la invitación", "Ok");
                    if (user.Accept)
                    {
                        UserInvite = new User { Language = MyUser.Language };
                        UserInviteName = user.Sender;
                        UserInvteAvatar = user.avatar;
                    }
                    else 
                    {
                        UserInviteName = "sin user";
                        UserInvteAvatar = "sinuser";
                       UserInvite = null;
                     }
                }
            }
            else if (e.Typeobject == nameof(InitGame) && e != null) 
            {
                var initGame= e as InitGame;
               // List<Question> list = initGame.Questions;
                MyUser.Estado = "play";
               await _Service.UserState(MyUser);
                Tuple<InitGame, User,User> data = new Tuple<InitGame, User,User>(initGame, MyUser,UserInvite);
                await CoreMethods.PushPageModel<GameViewModel>(data);
            }
        }
        public string UserInviteName 
        {
            get { return UserInvite is null ? "No user " : UserInvite.UserName; }
            set { UserInvite.UserName = value; RaisePropertyChanged("UserInviteName");
                StartEnablecheck(); }
             }
        public string UserInvteAvatar {// estos valores cambiarlos para no tener problemas
            get { return UserInvite is null ? "sinuser" : UserInvite.Avatar; }
            set { UserInvite.Avatar = value; RaisePropertyChanged("UserInvteAvatar");
                StartEnablecheck();  } }
        public string MyUserName
        {
            get { return MyUser.UserName; }
            set { MyUser.UserName = value; RaisePropertyChanged("MyUserName"); }
        }
        public string MyUserAvatar
        {
            get { return MyUser.Avatar; }
            set { MyUser.Avatar = value; RaisePropertyChanged("MyUserName"); }
        }
        public bool CancelVisible
        {
            get { return cancelVisible; }
            set { cancelVisible = value; RaisePropertyChanged("CancelVisible"); }
        }
        public bool StartVisible
        {
            get { return startVisible; }
            set { startVisible = value; RaisePropertyChanged("StartVisible"); }
        }
        public bool StartIsEnable
        {
            get { return startIsEnable; }
            set { startIsEnable = value; RaisePropertyChanged("StartIsEnable"); }
        }
        bool startIsEnable;
        bool startVisible;
        bool cancelVisible;
        string Category ;
        User MyUser;
        User UserInvite; 
        List<User> ListInvites = new List<User>();
        int LastUserIntive = 0;
        IQstnService _Service;
    }
}
