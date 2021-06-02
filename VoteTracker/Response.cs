using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteTracker
{
    public enum Response
    {
        NotFound = 0,
        NotClaimed = 1,
        Claimed = 2,
        InvalidServerKey = 3,
        Error = 4
    }
}
