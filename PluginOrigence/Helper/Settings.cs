using System;

namespace PluginOrigence.Helper
{
    public class Settings
    {
        public string RootPath { get; set; }
        public bool UseRemote { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }
        public bool UseSftp { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SshKey { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(RootPath))
            {
                throw new Exception("The RootPath property must be set");
            }

            if (UseRemote)
            {
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

                if (UseSftp)
                {
                    if(string.IsNullOrEmpty(SshKey) && string.IsNullOrEmpty(Password))
                    {
                        throw new Exception("For SFTP connections, either the Password or SshKey property must be set");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Password))
                    {
                        throw new Exception("The Password property must be set for FTP connections");
                    }
                }
            }

            return true;
        }

    }
}