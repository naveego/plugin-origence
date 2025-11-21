using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginOrigence.Helper;
using Xunit;

namespace PluginOrigenceTest.Plugin
{
    public class PluginIntegrationTest
    {

        /* Test Instructions:
         * 1. Fill in the connection settings in the methods below:
         *    - GetLocalSettings(): Set RootPath to the local directory containing XML files (e.g., "/path/to/local/xml/files")
         *    - GetFtpSettings(): Set Hostname, Port (default 21), Username, Password, and RootPath for FTP server
         *    - GetSftpSettings(): Set Hostname, Port (default 22), Username, Password, and RootPath for SFTP server
         * 2. Ensure that the specified directories contain the required XML files with element (e.g., CUDLVehicleLoanApplication)
         * 3. Verify that the FTP/SFTP servers are accessible and credentials are correct
         * 4. Run the tests to verify connectivity, schema discovery, and data reading functionalities
         */

        private static Settings GetLocalSettings()
        {
            return new Settings
            {
                HostType = "local",
                RootPath = "",
            };
        }

        private static Settings GetFtpSettings()
        {
            return new Settings
            {
                HostType = "ftp",
                Hostname = "",
                Port = 21,
                Username = "",
                Password = "",
                RootPath = "",
            };
        }

        private static Settings GetSftpSettings()
        {
            return new Settings
            {
                HostType = "sftp",
                Hostname = "",
                Port = 22,
                Username = "",
                Password = "",
                RootPath = "",
            };
        }

        private static ConnectRequest GetConnectRequest(Settings settings)
        {
            return new ConnectRequest
            {
                SettingsJson = JsonConvert.SerializeObject(settings),
                OauthConfiguration = new OAuthConfiguration(),
                OauthStateJson = "",
            };
        }

        private Schema GetTestSchema()
        {
            return new Schema
            {
                Id = "CUDLVehicleLoanApplication",
                Name = "CUDLVehicleLoanApplication",
                Properties =
                {

                    new Property
                    {
                        Id = "ID",
                        Name = "ID",
                        IsKey = true,
                        IsNullable = false,
                        Type = PropertyType.String,
                    },
                    new Property
                    {
                        Id = "JSONObject",
                        Name = "JSONObject",
                        IsKey = false,
                        IsNullable = false,
                        Type = PropertyType.String,
                    }
                }
            };
        }

        [Fact]
        public async Task ConnectTest_Local_Success()
        {

            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var request = GetConnectRequest(GetLocalSettings());
            // Act
            var response = await client.ConnectAsync(request);

            // Assert
            Assert.IsType<ConnectResponse>(response);
            Assert.Equal("", response.SettingsError);
            Assert.Equal("", response.ConnectionError);
            Assert.Equal("", response.OauthError);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task DiscoverSchemasTest_Local_Success()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectRequest(GetLocalSettings());
            await client.ConnectAsync(connectRequest);

            var request = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.All,
                SampleSize = 10
            };

            // Act
            var response = await client.DiscoverSchemasAsync(request);

            // Assert
            Assert.IsType<DiscoverSchemasResponse>(response);
            Assert.NotNull(response.Schemas);
            Assert.NotEmpty(response.Schemas);

            var testSchema = response.Schemas.FirstOrDefault(s => s.Id == "CUDLVehicleLoanApplication");
            Assert.NotNull(testSchema);
            Assert.Equal("CUDLVehicleLoanApplication", testSchema.Id);
            Assert.NotEmpty(testSchema.Properties);
            Assert.Contains(testSchema.Properties, p => p.Id == "ID" && p.IsKey);
            Assert.Contains(testSchema.Properties, p => p.Id == "JSONObject");

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamTest_Local_Success()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectRequest(GetLocalSettings());
            await client.ConnectAsync(connectRequest);

            var schema = GetTestSchema();
            var request = new ReadRequest
            {
                Schema = schema,
                Limit = 10
            };

            // Act
            var records = new List<Naveego.Sdk.Plugins.Record>();
            using (var call = client.ReadStream(request))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    records.Add(responseStream.Current);
                }
            }

            // Assert
            Assert.NotNull(records);
            Assert.NotEmpty(records);
            Assert.All(records, record =>
            {
                Assert.NotNull(record.DataJson);
                Assert.Contains("ID", record.DataJson);
                Assert.Contains("JSONObject", record.DataJson);
            });

            Assert.True(records.Count > 0, "Expected at least one record to be read");

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ConnectTest_Ftp_Success()
        {
            // setup
            Server server = new Server
            {
            Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
            Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var request = GetConnectRequest(GetFtpSettings());
            // Act
            var response = await client.ConnectAsync(request);

            // Assert
            Assert.IsType<ConnectResponse>(response);
            Assert.Equal("", response.SettingsError);
            Assert.Equal("", response.ConnectionError);
            Assert.Equal("", response.OauthError);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task DiscoverSchemasTest_Ftp_Success()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectRequest(GetFtpSettings());
            await client.ConnectAsync(connectRequest);

            var request = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.All,
                SampleSize = 10
            };

            // Act
            var response = await client.DiscoverSchemasAsync(request);

            // Assert
            Assert.IsType<DiscoverSchemasResponse>(response);
            Assert.NotNull(response.Schemas);
            Assert.NotEmpty(response.Schemas);

            var testSchema = response.Schemas.FirstOrDefault(s => s.Id == "CUDLVehicleLoanApplication");
            Assert.NotNull(testSchema);
            Assert.Equal("CUDLVehicleLoanApplication", testSchema.Id);
            Assert.NotEmpty(testSchema.Properties);
            Assert.Contains(testSchema.Properties, p => p.Id == "ID" && p.IsKey);
            Assert.Contains(testSchema.Properties, p => p.Id == "JSONObject");

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamTest_Ftp_Success()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectRequest(GetFtpSettings());
            await client.ConnectAsync(connectRequest);

            var schema = GetTestSchema();
            var request = new ReadRequest
            {
                Schema = schema,
                Limit = 10
            };

            // Act
            var records = new List<Naveego.Sdk.Plugins.Record>();
            using (var call = client.ReadStream(request))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    records.Add(responseStream.Current);
                }
            }

            // Assert
            Assert.NotNull(records);
            Assert.NotEmpty(records);
            Assert.All(records, record =>
            {
                Assert.NotNull(record.DataJson);
                Assert.Contains("ID", record.DataJson);
                Assert.Contains("JSONObject", record.DataJson);
            });

            Assert.True(records.Count > 0, "Expected at least one record to be read");

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ConnectTest_Sftp_Success()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var request = GetConnectRequest(GetSftpSettings());
            // Act
            var response = await client.ConnectAsync(request);

            // Assert
            Assert.IsType<ConnectResponse>(response);
            Assert.Equal("", response.SettingsError);
            Assert.Equal("", response.ConnectionError);
            Assert.Equal("", response.OauthError);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }
        
        [Fact]
        public async Task DiscoverSchemasTest_Sftp_Success()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectRequest(GetSftpSettings());
            await client.ConnectAsync(connectRequest);

            var request = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.All,
                SampleSize = 10
            };

            // Act
            var response = await client.DiscoverSchemasAsync(request);

            // Assert
            Assert.IsType<DiscoverSchemasResponse>(response);
            Assert.NotNull(response.Schemas);
            Assert.NotEmpty(response.Schemas);

            var testSchema = response.Schemas.FirstOrDefault(s => s.Id == "CUDLVehicleLoanApplication");
            Assert.NotNull(testSchema);
            Assert.Equal("CUDLVehicleLoanApplication", testSchema.Id);
            Assert.NotEmpty(testSchema.Properties);
            Assert.Contains(testSchema.Properties, p => p.Id == "ID" && p.IsKey);
            Assert.Contains(testSchema.Properties, p => p.Id == "JSONObject");

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamTest_Sftp_Success()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginOrigence.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectRequest(GetSftpSettings());
            await client.ConnectAsync(connectRequest);

            var schema = GetTestSchema();
            var request = new ReadRequest
            {
                Schema = schema,
                Limit = 10
            };

            // Act
            var records = new List<Naveego.Sdk.Plugins.Record>();
            using (var call = client.ReadStream(request))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    records.Add(responseStream.Current);
                }
            }

            // Assert
            Assert.NotNull(records);
            Assert.NotEmpty(records);
            Assert.All(records, record =>
            {
                Assert.NotNull(record.DataJson);
                Assert.Contains("ID", record.DataJson);
                Assert.Contains("JSONObject", record.DataJson);
            });

            Assert.True(records.Count > 0, "Expected at least one record to be read");

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }
    }
}

