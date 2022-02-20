using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace QuestionsService.models
{
  public  class User
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("estado")]
        public string Estado { get; set; }
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
    }
}
