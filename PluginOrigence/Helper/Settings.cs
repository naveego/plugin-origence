using System;

namespace PluginOrigence.Helper
{
    public class Settings
    {
        public string RootPath { get; set; }
        public string HostType { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SshKey { get; set; }

        private bool UseRemote => HostType == "ftp" || HostType == "sftp";
        private bool UseSftp => HostType == "sftp";

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
                if (!string.IsNullOrWhiteSpace(Hostname))
                {
                    throw new Exception("Hostname property is only valid when a remote host is selected. To enable remote connections, please set the Host Type property to 'ftp' or 'sftp'.");
                }

                if (!string.IsNullOrWhiteSpace(SshKey))
                {
                    throw new Exception("SshKey property is only valid when a remote host is selected. To enable remote connections, please set the Host Type property to 'ftp' or 'sftp'.");
                }
            }

            return true;
        }

    }
}