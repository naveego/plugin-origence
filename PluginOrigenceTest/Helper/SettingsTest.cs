using System;
using PluginOrigence.Helper;
using Xunit;

namespace PluginOrigenceTest.Helper
{
    public class SettingsTest
    {
        [Fact]
        public void Validate_WhenRootPathIsNull_ThrowsException()
        {
            var settings = new Settings { RootPath = null };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("The RootPath property must be set", exception.Message);
        }

        [Fact]
        public void Validate_WhenRootPathIsEmpty_ThrowsException()
        {
            var settings = new Settings { RootPath = "" };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("The RootPath property must be set", exception.Message);
        }

        [Fact]
        public void Validate_WhenRootPathIsWhitespace_ThrowsException()
        {
            var settings = new Settings { RootPath = "   " };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("The RootPath property must be set", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsLocalAndRootPathIsSet_ReturnsTrue()
        {
            var settings = new Settings { RootPath = "/path/to/root", HostType = "local" };
            Assert.True(settings.Validate());
        }

        [Fact]
        public void Validate_WhenHostTypeIsFtpAndHostnameIsNull_ThrowsException()
        {
            var settings = new Settings { RootPath = "/path", HostType = "ftp", Hostname = null };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("The Hostname property must be set", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsFtpAndPortIsZero_ThrowsException()
        {
            var settings = new Settings { RootPath = "/path", HostType = "ftp", Hostname = "host", Port = 0 };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("Invalid port", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsFtpAndPortIsNegative_ThrowsException()
        {
            var settings = new Settings { RootPath = "/path", HostType = "ftp", Hostname = "host", Port = -1 };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("Invalid port", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsFtpAndUsernameIsNull_ThrowsException()
        {
            var settings = new Settings { RootPath = "/path", HostType = "ftp", Hostname = "host", Port = 21, Username = null };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("The Username property must be set", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsFtpAndPasswordIsNull_ThrowsException()
        {
            var settings = new Settings { RootPath = "/path", HostType = "ftp", Hostname = "host", Port = 21, Username = "user", Password = null };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("The Password property must be set for FTP connections", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsFtpAndAllRequiredFieldsAreSet_ReturnsTrue()
        {
            var settings = new Settings { RootPath = "/path", HostType = "ftp", Hostname = "host", Port = 21, Username = "user", Password = "pass" };
            Assert.True(settings.Validate());
        }

        [Fact]
        public void Validate_WhenHostTypeIsSftpAndBothPasswordAndSshKeyAreNull_ThrowsException()
        {
            var settings = new Settings { RootPath = "/path", HostType = "sftp", Hostname = "host", Port = 22, Username = "user", Password = null, SshKey = null };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("For SFTP connections, either the Password or SshKey property must be set", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsSftpAndPasswordIsSet_ReturnsTrue()
        {
            var settings = new Settings { RootPath = "/path", HostType = "sftp", Hostname = "host", Port = 22, Username = "user", Password = "pass" };
            Assert.True(settings.Validate());
        }

        [Fact]
        public void Validate_WhenHostTypeIsSftpAndSshKeyIsSet_ReturnsTrue()
        {
            var settings = new Settings { RootPath = "/path", HostType = "sftp", Hostname = "host", Port = 22, Username = "user", SshKey = "key" };
            Assert.True(settings.Validate());
        }

        [Fact]
        public void Validate_WhenHostTypeIsSftpAndBothPasswordAndSshKeyAreSet_ReturnsTrue()
        {
            var settings = new Settings { RootPath = "/path", HostType = "sftp", Hostname = "host", Port = 22, Username = "user", Password = "pass", SshKey = "key" };
            Assert.True(settings.Validate());
        }

        [Fact]
        public void Validate_WhenHostTypeIsLocalAndHostnameIsSet_ThrowsException()
        {
            var settings = new Settings { RootPath = "/path", HostType = "local", Hostname = "host" };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("Hostname property is only valid when a remote host is selected. To enable remote connections, please set the Host Type property to 'ftp' or 'sftp'.", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsLocalAndSshKeyIsSet_ThrowsException()
        {
            var settings = new Settings { RootPath = "/path", HostType = "local", SshKey = "key" };
            var exception = Assert.Throws<Exception>(() => settings.Validate());
            Assert.Equal("SshKey property is only valid when a remote host is selected. To enable remote connections, please set the Host Type property to 'ftp' or 'sftp'.", exception.Message);
        }

        [Fact]
        public void Validate_WhenHostTypeIsNullAndRootPathIsSet_ReturnsTrue()
        {
            var settings = new Settings { RootPath = "/path", HostType = null };
            Assert.True(settings.Validate());
        }
    }
}