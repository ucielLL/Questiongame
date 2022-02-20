using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FreshMvvm;
using QuestionMovil.ViewModels;
using QuestionService.Service;
using Acr.UserDialogs;

namespace QuestionMovil
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ConfigureContainer();
            var pagemain = FreshPageModelResolver.ResolvePageModel<LoginViewModel>();
            var navigation = new FreshNavigationContainer(pagemain);
            MainPage = navigation;
        }
        private void ConfigureContainer()
        {
            FreshIOC.Container.Register<IQstnService,QstnService>();
            //FreshIOC.Container.Register<IUserDialogs>(UserDialogs.Instance);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
