using QuestionsService.models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace QuestionsService.Service
{
    interface IQstnService
    {
        Task<bool> InitAsync(string userId);
        Task<bool> DisconnectAsync();
        Task<User> SignInAsync(string userNmae,string language);
        Task<User> SearchUser(string username, string language);
        Task<List<User>> SearchUser(string language);
       // Task SyncUsersAsync(SyncUsers message);
      //  Task StarGame(InitGame initGame);
       // Task NextQAsync(NextQuestion nextQuestion);
    }
}
