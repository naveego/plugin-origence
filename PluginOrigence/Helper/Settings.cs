using System;

namespace PluginOrigence.Helper
{
    public class Settings
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public bool UseSftp { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SshKey { get; set; }
        public string RootPath { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(RootPath))
            {
                throw new Exception("The RootPath property must be set");
            }

            if (string.IsNullOrWhiteSpace(Hostname))
            {
                throw new Exception("The Hostname property must be set");
            }

            if (Port <= 0)
            {
                throw new Exception("Invalid port");
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new Exception("The Username property must be set");
            }

            if (string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(SshKey))
            {
                throw new Exception("The Password or SshKey property must be set");
            }

            return true;
        }

    }
}