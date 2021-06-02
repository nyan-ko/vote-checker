using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using TShockAPI;

namespace VoteTracker
{
    public class Config
    {
        public static readonly string SavePath = Path.Combine(TShock.SavePath, "votetracker_config.json");

        public string ServerKey { get; set; } = string.Empty;

        private Config()
        {

        }

        public static Config Read()
        {
            Config config = new Config();

            if (File.Exists(SavePath))
            {
                config.ServerKey = File.ReadAllText(SavePath);
            }
            else
            {
                File.Create(SavePath);
            }

            return config;
        }
    }
}
