using CommandLine;
using CommandLine.Text;
using System;

namespace biblepay_rpc
{
    class Options
    {
        [Option('u', "user", Required = true, HelpText = "rpcuser defined in biblepay.conf")]
        public string User { get; set; }

        [Option('p', "password", Required = true, HelpText = "rpcpassword defined in biblepay.conf")]
        public string Password { get; set; }

        [Option('d', "daemonurl", DefaultValue = "localhost", HelpText = "biblepayd url")]
        public string DaemonURL { get; set; }

        [Option('r', "port", HelpText = "(Default: 39000 [main] | 39001 [testnet]) rpcport defined in biblepay.conf")]
        public string Port { get; set; }

        [Option('t', "testnet", DefaultValue = false, HelpText = "use testnet (ignored if port is manually specified)")]
        public bool Testnet { get; set; }

        public string Url
        {
            get
            {
                return String.Format("http://{0}:{1}", 
                    DaemonURL, 
                    (!String.IsNullOrEmpty(Port) ? Port : (Testnet ? "39001" : "39000")));
            }
        }
    }
}
