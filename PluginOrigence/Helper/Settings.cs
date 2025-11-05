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
                    if (string.IsNullOrWhiteSpace(SshKey) && string.IsNullOrWhiteSpace(Password))
                    {
                        throw new Exception("For SFTP connections, either the Password or SshKey property must be set");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        throw new Exception("The Password property must be set for FTP connections");
                    }
                }
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(Hostname) || !string.IsNullOrWhiteSpace(SshKey) || UseSftp)
                {
                    throw new Exception("Hostname, SshKey, and Use SFTP properties are only valid when Use Remote Server is selected. To enable remote connections, please check the Use Remote Server option.");
                }
            }

            return true;
        }

    }
}