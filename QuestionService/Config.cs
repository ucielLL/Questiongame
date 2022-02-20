using System;

namespace QuestionService
{
    public class Config
    {
        static readonly string Base = "url de tu azure funtion";

        public static string InitEndPointd = $"{Base}/Negotiate";
        public static string SyncUsersEndpoind = $"{Base}/SyncUsers";
        public static string SignInEndpoind = $"{Base}/SignIn";
        public static string SearchUserEndpoind = $"{Base}/User";
        public static string InitGameEndpoind = $"{Base}/initGame";
        public static string NextQuestionEndpoind = $"{Base}/next";
        public static string UserStateEnpoind = $"{Base}/UserState";
    }
}
