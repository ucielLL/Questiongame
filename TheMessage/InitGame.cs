using System;
using System.Collections.Generic;
using System.Text;

namespace TheMessage
{
  public  class InitGame: MessageBase
    {
        public string Category { get; set; }
        public List<Question> Questions { get; set; }
    }
}
