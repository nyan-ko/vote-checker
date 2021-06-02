using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerrariaApi.Server;
using Terraria;
using System.Net;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Http;

namespace VoteTracker
{
    [ApiVersion(2, 1)]
    public class VoteTracker : TerrariaPlugin
    {
        private HttpClient _hc = new HttpClient();
        private BlockingCollection<Request> _requests = new BlockingCollection<Request>();

        private Thread _reqThread;

        private string _key;
        private bool _enabled;

        private const string CHECK_URL = "https://terraria-servers.com/api/?object=votes&element=claim&key={{0}}&username={{1}}";
        private const string CLAIM_URL = "https://terraria-servers.com/api/?action=post&object=votes&element=claim&key={{0}}&username={{1}}";

        // -- FIELDS --

        public VoteTracker(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            // in case more stuff needs to be added to the config later on
            // only hold onto the string instead of the object for now though
            _key = Config.Read().ServerKey; 

            if (string.IsNullOrEmpty(_key))
            {
                TShock.Log.ConsoleError($"Vote Tracker was not provided with a valid server key and will not run for this instance. A config file can be found at {Config.SavePath}.");
                return;
            }

            _enabled = true;

            _reqThread = new Thread(RequestThread);
            _reqThread.IsBackground = true;
            _reqThread.Start();
        }

        private void RequestThread(object unused)
        {
            while (true) 
            {
                if (_requests.TryTake(out Request req, -1))
                {
                    HandleRequest(req);
                }
            }
        }

        private async void HandleRequest(Request request)
        {
            string name = request.Username;

            Response response;
            try
            {
                if (request.Type == RequestType.CheckVoted)
                {
                    string res = _hc.GetStringAsync(string.Format(CHECK_URL, _key, name)).GetAwaiter().GetResult();

                    if (res.Contains("incorrect server key"))  // relic from old TSReward plugin, maybe not even needed?
                    {
                        response = Response.InvalidServerKey;
                    }
                    else
                    {
                        response = (Response)int.Parse(res);
                    }
                }
                else
                {
                    string res = _hc.GetStringAsync(string.Format(CLAIM_URL, _key, name)).GetAwaiter().GetResult();

                    if (res.Contains("incorrect server key")) 
                    {
                        response = Response.InvalidServerKey;
                    }
                    else
                    {
                        response = (Response)(int.Parse(res) + 1);
                    }
                }
            }
            catch
            {
                response = Response.Error;
            }

            await request.Unblock(response);
        }

        // -- MANAGE REQUESTS --

        public Request AddRequest(string username, RequestType type) => AddRequest(new Request(username, type));

        public Request AddRequest(Request request)
        {
            if (!_enabled)
            {
                return null;
            }

            _requests.Add(request);
            return request;
        }


    }
}
