using Proficy.Historian.Gateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Client
{
    public class HistorianClientConfiguration
    {
        private string _serverName;
        private string _username;
        private string _password;

        public SubscribeMessage SubscribeMessage { get; set; }

        public string ServerName
        {
            get
            {
                return _serverName;
            }
            set
            {
                var env = Environment.GetEnvironmentVariable(value);
                if (env != null)
                {
                    _serverName = env;
                }
                else
                {
                    _serverName = value;
                }
            }
        }

        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                var env = Environment.GetEnvironmentVariable(value);
                if(env != null)
                {
                    _username = env;
                }
                else
                {
                    _username = value;
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                var env = Environment.GetEnvironmentVariable(value);
                if (env != null)
                {
                    _password = env;
                }
                else
                {
                    _password = value;
                }
            }
        }
    }
}
