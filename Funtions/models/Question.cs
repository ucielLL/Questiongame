using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Funtions.models
{
    class Question:TableEntity
    {
        public string TheQuestion { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string Answer { get; set; }
        public int NumberQuestion { get; set; }

    }
}
