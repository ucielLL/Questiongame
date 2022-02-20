
using Microsoft.AspNetCore.SignalR.Client;
using QuestionService.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TheMessage;
namespace QuestionService.Service
{
    public class QstnService : IQstnService
    {
        public async Task<bool> InitAsync(string UserId)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var query = $"{Config.InitEndPointd}/{UserId}";
                var respon = await Client.GetStringAsync(query);
                var info = JsonSerializer.Deserialize<ConnectionInfo>(respon, new JsonSerializerOptions {PropertyNameCaseInsensitive = true });
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
                        var data = JsonSerializer.Deserialize<MessageBase>(message.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (data.Typeobject == nameof(SyncUsers))
                        {
                            msg = JsonSerializer.Deserialize<SyncUsers>(message.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        else
                        {
                            msg = JsonSerializer.Deserialize<InitGame>(message.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }

                        ReceivedSyncUserEvent?.Invoke(msg);
                    }
                });
                Token = hub.ConnectionId;
                IsConnected = true;
                hub.On<object>("OnNextQuestion", (message) =>
                {
                    if (message != null)
                    {
                        var msg = JsonSerializer.Deserialize<NextQuestion>(message.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        ReceivedNextCuestion?.Invoke(msg);
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
        public async Task<bool> DisconnectAsync()
        {

            if (!IsConnected)
            {
                return true;
            }

            try
            {
                await hub.DisposeAsync();
                return true;
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
        public async Task<User> SearchUser(string userName, string languge)
        {
            try
            {
                var query = $"{Config.SearchUserEndpoind}/{userName}/{languge.ToLower()}/false";
                var respons = await Client.GetStringAsync(query);
                var user = JsonSerializer.Deserialize<User>(respons, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
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
                var query = $"{Config.SearchUserEndpoind}/empty/{language.ToLower()}/true";
                var respons = await Client.GetStringAsync(query);
                var user = JsonSerializer.Deserialize<List<User>>(respons, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                    Language = language.ToLower(),
                    Avatar = $"image_{r.Next(1, 5).ToString() }"
                };
                var json = JsonSerializer.Serialize<User>(newUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await Client.PostAsync(Config.SignInEndpoind, content);
                return newUser;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task NextQAsync(NextQuestion nextQuestion)
        {
            var json = JsonSerializer.Serialize(nextQuestion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(Config.NextQuestionEndpoind, content);
        }

        public async Task UserState(User newStateUser)
        {
            var json = JsonSerializer.Serialize<User>(newStateUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
           var a = await Client.PostAsync(Config.UserStateEnpoind, content);
          //  if (!a.IsSuccessStatusCode) { }
        }

        #region atributes/ properties
        private static HttpClient Client = new HttpClient();
        HubConnection hub;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public event Func<MessageBase, Task> ReceivedSyncUserEvent;
        public event Func<NextQuestion, Task> ReceivedNextCuestion;
      
        bool IsConnected { get; set; }
        public string Token { get; set; }
        #endregion

    }
}
