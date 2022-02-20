
using QuestionService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheMessage;

namespace Questions
{
    class Program
    {
        static QstnService service = new QstnService();
        static QuestionService.Models.User Myuser = null;
        static string responseInvite = " ";
        static SyncUsers invite;
        static string StadoGame;
        static List<Question> Questions = new List<Question>();
        static int NomberQuestion = 0;
        static async Task Main(string[] args)
        {

            string username;
            string language;
            bool isconnected = false;
           // service.ReceivedSyncUserEvent += Service_ReceivedSyncUserEvent;
          //  service.ReceivedNextCuestion += Service_ReceivedNextCuestion;
            Console.WriteLine("write your user Name");
            username = Console.ReadLine();
            Console.WriteLine("choose the laguage spanish(s) or english(e)");
            language = Console.ReadLine();

            Console.WriteLine(" sign in (S) or log in (l)");
            var SorL = Console.ReadLine();
            if (SorL.ToLower() == "s")
            {
                var usersearch = await service.SearchUser(username, language);
                if (usersearch is null)
                {
                    Myuser = await service.SignInAsync(username, language);
                    isconnected = await service.InitAsync(username);
                    if (isconnected && Myuser != null)
                    {
                        Console.WriteLine("you are new connection");
                    }
                    else
                    {
                        Console.WriteLine("el servicio no esta disponible intenta mas tarde ");
                    }

                }
                else
                {
                    Console.WriteLine(" El nombre ya esta en uso");
                }

            }
            else if (SorL.ToLower() == "l")
            {
                Myuser = await service.SearchUser(username, language);
                if (Myuser != null)
                {
                    isconnected = await service.InitAsync(username);
                    if (isconnected)
                    {
                        Console.WriteLine("you are new connection");
                    }
                    else
                    {
                        Console.WriteLine("el servicio no esta disponible intenta mas tarde ");
                    }
                }
                else
                {
                    Console.WriteLine(" El usuario no existe. brifica que los datos sean correctos ");
                }
            }

            while (isconnected)
            {
                Console.WriteLine("esperar s/n");
                if (Console.ReadLine() == "s")
                {
                    string respons = "";
                    do
                    {
                        respons = Console.ReadLine().ToLower();
                        if (respons == "s")
                        {
                            await service.SyncUsersAsync(invite);
                            break;
                        }

                    } while (respons != "s" || respons != "n");

                } else
                {
                    QuestionService.Models.User UserInvite;
                    Console.WriteLine("aleatorio(a) o buscar un amigo (b)");
                    var resposnsearch = Console.ReadLine();
                    if (resposnsearch.ToLower() == "a")
                    {
                        var users = await service.SearchUser(language);
                        if (users.Count > 0)
                        {
                            int i = 0;

                            do
                            {
                                UserInvite = users[i];
                                SyncUsers syncUser = new SyncUsers
                                {
                                    Sender = Myuser.UserName,
                                    avatar = Myuser.Avatar,
                                    Receiver = UserInvite.UserName,
                                    UserInvited = UserInvite.UserName
                                };
                                await service.SyncUsersAsync(syncUser);
                                Console.WriteLine("esperado respuesta ");
                                responseInvite = Console.ReadLine();
                                if (responseInvite == "false")
                                {
                                    i++;
                                }

                            } while (responseInvite != "true");
                            Console.WriteLine("quers inicial el jego");
                            var responseinit = Console.ReadLine();
                            var initGame = new InitGame()
                            {
                                Sender = Myuser.UserName,
                                Receiver = UserInvite.UserName,
                                Category = "mat"
                            };
                            await service.StarGame(initGame);
                        }


                    }

                }
                do
                {
                    var text = Console.ReadLine().ToLower();
                    if (text == "out")
                    {
                        isconnected = !await service.DisconnectAsync();
                        var msg = isconnected ? "no se apodido desconectar" : "esta desconectado";
                        Console.WriteLine(msg);
                    }
                    else if (text == "a" && Questions[NomberQuestion].OptionA == Questions[NomberQuestion].Answer)
                    {
                        Console.WriteLine("correcto");
                        NomberQuestion++;
                        WriteQuestion(Questions[NomberQuestion]);

                    }
                    else if (text == "b" && Questions[NomberQuestion].OptionA == Questions[NomberQuestion].Answer)
                    {
                        Console.WriteLine("correcto");
                        NomberQuestion++;
                        WriteQuestion(Questions[NomberQuestion]);

                    }
                    else if (text == "c" && Questions[NomberQuestion].OptionA == Questions[NomberQuestion].Answer)
                    {
                        Console.WriteLine("correcto");
                        NomberQuestion++;
                        WriteQuestion(Questions[NomberQuestion]);
                    }
                    else 
                    {
                        Console.WriteLine("incorrecto");
                    }
                    if (NomberQuestion == 9) { NomberQuestion = 0; Console.WriteLine("fin"); StadoGame = "finish";  }


                } while (StadoGame == "play");

            }

            Console.ReadKey();

           
        }

        private static void Service_ReceivedNextCuestion(object sender, NextQuestion e)
        {
            Console.WriteLine("siguinte pregunta");
            NomberQuestion++;
            WriteQuestion(Questions[NomberQuestion]);
        }
        private static void Service_ReceivedSyncUserEvent(object sender, MessageBase e)
        {
            if (e.Typeobject == nameof(SyncUsers) && e != null)
            {
                var user = e as SyncUsers;
                if (user.UserInvited == Myuser.UserName)
                {
                    Console.WriteLine("queres jugar con " + user.Sender);
                    invite = new SyncUsers
                    {
                        Sender = Myuser.UserName,
                        Receiver = user.Sender,
                        UserInvited = user.UserInvited,
                        Accept = true
                    };
                }
                else if (user.Receiver == Myuser.UserName && user.UserInvited != Myuser.UserName)
                {
                    responseInvite = user.Accept.ToString();
                    Console.WriteLine(user.UserInvited + ": " + user.Accept.ToString());

                }
            }
            else if (e.Typeobject == nameof(InitGame) && e != null)
            {
                StadoGame = "play";
                var user = e as InitGame;
                Questions = user.Questions;
                //  Questions = user.Questions.Select(a => { return a; }).ToList();
                Console.WriteLine("empieza el juego");
                WriteQuestion(Questions[NomberQuestion]);
                
            }

        }
        static void WriteQuestion(Question question)
        {
            Console.WriteLine(question.TheQuestion);
            Console.WriteLine("A- " +question.OptionA);
            Console.WriteLine("B- " + question.OptionB);
            Console.WriteLine("C- " + question.OptionC);
        }
    }
}
