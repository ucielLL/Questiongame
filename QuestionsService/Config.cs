using System;

namespace QuestionsService
{
    public class Config
    {
         static string Base = "http://localhost:7071/api";

        public static string InitEndPointd = $"{Base}/Negotiate";
        public static string SyncUsersEndpoind = $"{Base}/SyncUsers";
        public static string SignInEndpoind = $"{Base}/SignIn";
        public static string SearchUserEndpoind = $"{Base}/User";
        public static string InitGameEndpoind = $"{Base}/initGame";
        public static string NextQuestionEndpoind = $"{Base}/next";



    }
}
