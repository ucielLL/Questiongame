using FreshMvvm;
using QuestionService.Models;
using QuestionService.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TheMessage;
using Xamarin.Forms;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using FreshMvvm.Popups;

namespace QuestionMovil.ViewModels
{
    class GameViewModel: FreshBasePageModel
    {
        public GameViewModel(IQstnService service)
        {
            _Service = service;
            _Service.ReceivedNextCuestion += Service_ReceivedNextCuestion;
        }
        public override  void Init(object initData)
        {
            base.Init(initData);
           var Datas = initData as Tuple<InitGame, User, User>;
            OpponentUser = Datas.Item3;
            MyUser = Datas.Item2;
            // InitGame initgame = Datas.Item1;
            ListQuestion = Datas.Item1.Questions; //initgame.Questions;
            Title = Datas.Item1.Category;
            NewQuestion = 0;
            MyCorrectQuestions = 0;
           ChangeQuestionSimple();
        }
        public ICommand AnswerCommand => new Command<string>(async (data) =>
        {
            string respons = " ";
        switch (data)
        {
            case "Btn1":
                respons = TextAnswer1;
              
             break;
            case "Btn2":
                respons = TextAnswer2;

                break;

            case "Btn3":
                respons = TextAnswer3;

                break;
            }
            if (respons == ListQuestion[NewQuestion].Answer)
            {
                MyCorrectQuestions++;
                var qtn = new NextQuestion
                {
                    avatar = MyUser.Avatar,
                    Sender = MyUser.UserName,
                    Receiver = OpponentUser.UserName,
                    QuestionNomber = ListQuestion[NewQuestion].NumberQuestion,
                    Answere = ListQuestion[NewQuestion].Answer,
                    CorrectQuestions =MyCorrectQuestions
                };
                await _Service.NextQAsync(qtn);
                NewQuestion++;
                await ChangeQuestion();
            }
        });
        void ChangeQuestionSimple() 
        {
            TheQuestion = ListQuestion[NewQuestion].TheQuestion;
            TextAnswer1 = ListQuestion[NewQuestion].OptionA;
            TextAnswer2 = ListQuestion[NewQuestion].OptionB;
            TextAnswer3 = ListQuestion[NewQuestion].OptionC;
            Progress = progress + 0.10;
            
        }
        async Task ChangeQuestion()
        {
            
            if (ListQuestion.Count <= NewQuestion)
            {
                var datas = new Tuple<User, int, User, int>(MyUser,MyCorrectQuestions,OpponentUser,OpponentCorrectQuestions);
                await CoreMethods.PushPopupPageModel<WinPopUpViewModel>(datas);
            }
            else 
            {
                ChangeQuestionSimple();
            }
        }
       
        private async Task Service_ReceivedNextCuestion(NextQuestion arg)
        {
            if (ListQuestion[NewQuestion].NumberQuestion == arg.QuestionNomber && ListQuestion[NewQuestion].Answer == arg.Answere) 
            {
                NewQuestion++;
                OpponentCorrectQuestions = arg.CorrectQuestions;
                await ChangeQuestion();
            }
            return;
        }
        public string MyUserName
        {
            get { return MyUser.UserName; }
            set { MyUser.UserName = value; RaisePropertyChanged("MyUserName"); }
        }
        public string MyUserAvatar
        {
            get { return MyUser.Avatar; }
            set { MyUser.Avatar = value; RaisePropertyChanged("MyUserAvatar"); }
        }
        public string Title { get; set; }
        public double Progress 
        {
            get { return progress; }
            set { progress = value; RaisePropertyChanged("Progress"); }
        }
        public string TextAnswer1
        {
            get { return textAnswer1; }
            set { textAnswer1 = value; RaisePropertyChanged("TextAnswer1");  }
        }
        public string TextAnswer2
        {
            get { return textAnswer2; }
            set { textAnswer2 = value; RaisePropertyChanged("TextAnswer2"); }
        }
        public string TextAnswer3
        {
            get { return textAnswer3; }
            set { textAnswer3 = value; RaisePropertyChanged("TextAnswer3"); }
        }
        public string TheQuestion 
        {
            get { return theQuestion; }
            set { theQuestion = value; RaisePropertyChanged("TheQuestion"); }
        }
        double progress;
        string textAnswer1;
        string textAnswer2;
        string textAnswer3;
        string theQuestion;
        List<Question> ListQuestion;
        User MyUser;
        User OpponentUser;
        int NewQuestion;
        int MyCorrectQuestions;
        int OpponentCorrectQuestions;
        IQstnService _Service;
        
    }
   
}
