using System;
using System.Collections.Generic;
using System.Text;

namespace TheMessages
{
    public class InitGame : MessageBase
    {
        public string Category { get; set; }
        public List<Question> Questions {get; set;}

    }
}
