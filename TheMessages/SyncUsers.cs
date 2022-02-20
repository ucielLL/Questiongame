using System;
using System.Collections.Generic;
using System.Text;

namespace TheMessages
{
   public class SyncUsers: MessageBase
    {

        public string UserInvited { get; set; }
        public bool Accept { get; set; }

    }
}
