using Acr.UserDialogs;
using FreshMvvm;
using QuestionService.Models;
using QuestionService.Service;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestionMovil.ViewModels
{
    class LoginViewModel : FreshBasePageModel
    {

        public LoginViewModel(IQstnService qstnService)
        {
            Service = qstnService;

        }
        public ICommand ConnectedCommsnd => new Command(async () =>
        {
           if (await Check() == false)
            {
                return;
            }

            ActivitiIsRunning = true;
            MyUser = await Service.SearchUser(UserName, Language);
            if (MyUser != null)
            {
                MyUser.Estado = "wait";
                await Service.UserState(MyUser);
                if (await Service.InitAsync(UserName))
                {
                    await CoreMethods.PushPageModel<PreGameViewModel>(MyUser);
                }
                ActivitiIsRunning = false;
            }
            else 
            {
                ActivitiIsRunning = false;
                await CoreMethods.DisplayAlert("Alert", "no se encontro el usuario", "OK");
            }
        });
       
        public ICommand RegistrerCommand => new Command(async ()=> 
        {
            
            if (await Check() == false)
            {
                return;
            }
            ActivitiIsRunning = true;
           var users = await Service.SearchUser(UserName, Language);
            if (users is null)
            {

               MyUser = await Service.SignInAsync(UserName, Language);
                if (await Service.InitAsync(UserName))
                {
                    await CoreMethods.PushPageModel<PreGameViewModel>(MyUser);
                }
                ActivitiIsRunning = false;
            }
            else
            {
                ActivitiIsRunning = false;
                await CoreMethods.DisplayAlert("Alert", "el usuario ya existe", "OK");
            }
            
        } );

       async Task<bool> Check() 
        {
            if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Language))
            {
                await CoreMethods.DisplayAlert("Alert", "hay campos vacios", "OK");
                return false;
            }
            return true;
        }

        IQstnService Service;

       // ActivityIndicator ActivityIndicator = new ActivityIndicator();
        public string UserName { get; set; }
        public string Language { get; set; }
        public bool ActivitiIsRunning { get; set; }
        User MyUser;
}
}
