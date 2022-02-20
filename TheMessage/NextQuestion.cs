using System;
using System.Collections.Generic;
using System.Text;

namespace TheMessage
{
    public class NextQuestion : MessageBase
    {
        public int CorrectQuestions { get; set; } 
        public string Answere { get; set; }
        public int QuestionNomber { get; set; }
    }
}
