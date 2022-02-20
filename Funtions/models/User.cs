
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Funtions.models
{
   public class User : TableEntity
    {
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }
        [JsonPropertyName("Estado")]
        public string Estado { get; set; }
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
        [JsonPropertyName("lastGameDate")]
        public DateTime LastGameDate { get; set; }
       
    }
}
