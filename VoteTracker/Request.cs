using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteTracker
{
    public class Request
    {
        // Represents an API request to check whether a user has voted, or if they've claimed.
        // Blocks until the request has been made, where it then returns a response.

        // ------------------------------------------ todo 
        // check if Request objects end up uncleared from memory once both programs are done using
        // ------------------------------------------

        public string Username { get; private set; }
        public RequestType Type { get; private set; }

        private bool _unblocked;
        private Response _response = Response.Error;

        public Request(string username, RequestType type)
        {
            Username = username;
            Type = type;
        }

        public async Task Unblock(Response response)
        {
            _response = response;
            _unblocked = true;
        }

        public async Task<Response> AwaitResponseAsync()
        {
            while (!_unblocked)
            {
                await Task.Delay(50);
            }

            return _response;
        }
    }

    public enum RequestType
    {
        CheckVoted,
        CheckClaimed
    }
}
