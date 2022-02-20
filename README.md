# questiongame
<h2>juego de preguntas</h2> 

En este proyecto se usó:

* xamarin
* azure funtion 
* azure storage
* azure signal R
* FreshMVVM

## Screens ##
<img src="Imgs/img3.png" width="220" /> <img src="Imgs/img5.png" width="220" />
 <img src="Imgs/img6.png" width="220" /> <img src="Imgs/img2.png" width="220" />
<img src="Imgs/img1.png" width="220" />
<img src="Imgs/img4.png" width="220" />

## como correr el ejemplo ##
1. clone el repositorio 
2. en el proyecto de "Funtions":
 En el archivo "local.settings.json" colocar tus licencias de azure <br />
 
 { <br />
  "IsEncrypted": false, <br />
  "Values": { <br /> 
    "AzureWebJobsStorage": "UseDevelopmentStorage=true", <br /> 
    "FUNCTIONS_WORKER_RUNTIME": "dotnet", <br />
    "AzureSignalRConnectionString": "coloca tu endpoint", <br /> 
    "StorageConection": "coloca tu endpoint" <br /> 
  } <br /> 
} <br /> 

3. El en proyecto "QuestionService": <br /> 
coloque la url o endpoint base de azure funtion, si es de azure tiene que publicar primero las funtions y lo je ejecuta local solo copie el endpoint base  <br /> 
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








