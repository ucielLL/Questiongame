using Microsoft.AspNetCore.SignalR.Client;
using QuestionsService.models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TheMessages;

namespace QuestionsService.Service
{
     public  class QstnService : IQstnService
     {
        public async Task<bool> DisconnectAsync()
        {

            if (IsConnected!) 
            {
                return true;
            }

            try
            {
                await hub.DisposeAsync();
                return true;
            }
            catch (Exception )
            {
                return false;
            }
          
        }

        public async Task<bool> InitAsync(string UserId)
        {
            await semaphoreSlim.WaitAsync();
        
            try
            {
                var query = $"{Config.InitEndPointd}/{UserId}";
                var respon = await Client.GetStringAsync(query);
                var info = JsonSerializer.Deserialize<ConnectionInfo>(respon);
                var connectionBuilder = new HubConnectionBuilder();
                connectionBuilder.WithUrl(info.Url, (options) =>
                 {
                     options.AccessTokenProvider = () => Task.Run<string>(() => info.AccessToken);
                 });
                hub = connectionBuilder.Build();
                await hub.StartAsync();
                hub.On<object>("OnSyncUsers", (message) => 
                {
                    if (message != null) 
                    {
                        MessageBase msg = null;
                        var data = JsonSerializer.Deserialize<MessageBase>(message.ToString());
                        if (data.Typeobject == nameof(SyncUsers))
                        {
                            msg = JsonSerializer.Deserialize<SyncUsers>(message.ToString());
                        }
                        else 
                        {
                            msg = JsonSerializer.Deserialize<InitGame>(message.ToString());
                        }
                       
                        ReceivedSyncUserEvent?.Invoke(this, msg);
                    }
                });
                Token = hub.ConnectionId;
                IsConnected = true;
                hub.On<object>("OnNextQuestion", (message) => 
                {
                    if (message != null)
                    {
                        var msg = JsonSerializer.Deserialize<NextQuestion>(message.ToString());
                        ReceivedNextCuestion?.Invoke(this, msg);

                    }
                });

                semaphoreSlim.Release();
                return IsConnected;
            }
            catch (Exception)
            {
              
                return false;
            }
           

        }

        public async Task SyncUsersAsync(SyncUsers message) // pendite, terminafr 
        {
            if (!IsConnected || message is null) return;
            var json = JsonSerializer.Serialize(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(Config.SyncUsersEndpoind, content);
        }
        public async Task StarGame(InitGame initGame)
        {
            if (!IsConnected || initGame is null) return;
            var json = JsonSerializer.Serialize(initGame);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync($"{Config.InitGameEndpoind}", content);
        }
        public async Task<User> SearchUser(string userName , string languge)
        { 
            try
            {
                var query = $"{Config.SearchUserEndpoind}/{userName}/{languge}/false";
                var respons = await Client.GetStringAsync(query);
                var user = JsonSerializer.Deserialize<User>(respons);
                return user;
            }
            catch (Exception) 
            {
                return null;
            }

        }
        public async Task<List<User>> SearchUser(string language)
        {
            try
            {
                var query = $"{Config.SearchUserEndpoind}/empty/{language}/true";
                var respons = await Client.GetStringAsync(query);
                var user = JsonSerializer.Deserialize<List<User>>(respons);
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> SignInAsync(string userName, string language)
        {
            try
            {
                Random r = new Random();
                var newUser = new User
                {
                    UserName = userName,
                    Language = language,
                    Avatar = $"imag_{r.Next(1, 5).ToString() }"
                };
                var json = JsonSerializer.Serialize<User>(newUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await Client.PostAsync(Config.SignInEndpoind, content);
                return newUser;
            }
            catch (Exception) 
            {
                return  null;
            }
        }
       public async Task NextQAsync(NextQuestion nextQuestion)
        {
            var json = JsonSerializer.Serialize(nextQuestion);
            var content = new  StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(Config.NextQuestionEndpoind, content);
        }

        #region atributes/ properties
        private static HttpClient Client = new HttpClient();
        HubConnection hub;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public event EventHandler<MessageBase> ReceivedSyncUserEvent;
        public event EventHandler<NextQuestion> ReceivedNextCuestion;
        bool IsConnected { get; set; }
        public string Token{ get; set; }
        #endregion
    }
}
