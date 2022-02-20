using FreshMvvm;
using QuestionService.Models;
using QuestionService.Service;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestionMovil.ViewModels
{
    class WinPopUpViewModel : FreshBasePageModel
    {
        public WinPopUpViewModel(IQstnService qstnService)
        {
            Service = qstnService;
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            var data = initData as Tuple<User, int, User, int>;
            MyQuestion = data.Item2;
            MyUserName = data.Item1.UserName;
            MyUserAvatar = data.Item1.Avatar;
            OpponentQuestion = data.Item4;
            OpponentUserName = data.Item3.UserName;
            OpponentAvatar = data.Item3.Avatar;
            Message = MyQuestion == OpponentQuestion ?"Empate": (MyQuestion > OpponentQuestion ? "Ganaste" : "Perdiste");
            myUser = data.Item1;
        }
        public ICommand FinishCommand => new Command(async () =>
       {
            //  await CoreMethods.PushPageModel<LoginViewModel>();
            myUser.Estado = "wait";
           await Service.UserState(myUser);
           //await CoreMethods.PopPageModel();
           await CoreMethods.PushPageModel<PreGameViewModel>(myUser);
            await PopupNavigation.Instance.PopAllAsync();
        });
      
        public int MyQuestion { get; set; }
        public int OpponentQuestion { get; set; }
        public string MyUserName { get; set; }
        public string OpponentUserName { get; set; }
        public string MyUserAvatar { get; set; }
        public string OpponentAvatar { get; set; }
        public string Message { get; set; }
        IQstnService Service;
        User myUser;
    }
}
