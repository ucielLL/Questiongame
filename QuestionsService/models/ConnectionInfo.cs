using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace QuestionsService.models
{
    public class ConnectionInfo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("accessToken")]
       public string AccessToken { get; set; }
    }
}
