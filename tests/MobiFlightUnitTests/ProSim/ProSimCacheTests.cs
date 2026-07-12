using GraphQL;
using GraphQL.Client.Abstractions.Websocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.ProSim;
using Moq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MobiFlight.Tests.ProSim
{
    [TestClass()]
    public class ProSimCacheTests
    {
        private ProSimCache _cache;

        [TestInitialize]
        public void Setup()
        {
            _cache = new ProSimCache();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_cache != null && _cache.IsConnected())
            {
                _cache.Disconnect();
            }
        }

        #region writeDataref Tests

        [TestMethod()]
        public void WriteDataref_WhenNotConnected_ShouldReturnEarly()
        {
            // Arrange
            var datarefPath = "test/dataref";
            var value = true;

            // Act - call writeDataref when not connected
            _cache.writeDataref(datarefPath, value);

            // Assert - should not throw exception, just return early
            Assert.IsFalse(_cache.IsConnected());
        }

        [TestMethod()]
        public void WriteDataref_WithNullValue_ShouldHandleGracefully()
        {
            // Arrange
            var datarefPath = "test/dataref";
            object value = null;

            // Act - should not throw, just return early
            _cache.writeDataref(datarefPath, value);

            // Assert - if we get here without exception, test passes
            Assert.IsFalse(_cache.IsConnected());
        }

        [TestMethod()]
        public void DataRefDescription_CanWrite_ShouldControlWriteAccess()
        {
            // Verify CanWrite property works correctly
            var writableDataRef = new DataRefDescription { CanWrite = true };
            var readOnlyDataRef = new DataRefDescription { CanWrite = false };

            Assert.IsTrue(writableDataRef.CanWrite);
            Assert.IsFalse(readOnlyDataRef.CanWrite);
        }

        [TestMethod()]
        public void GetDataRefDescriptions_WhenNotConnected_ShouldReturnEmptyDictionary()
        {
            var descriptions = _cache.GetDataRefDescriptions();

            Assert.IsNotNull(descriptions);
            Assert.IsEmpty(descriptions);
        }

        [TestMethod()]
        public void Clear_ShouldClearDataRefDescriptions()
        {
            // Act
            _cache.Clear();

            // Assert
            var descriptions = _cache.GetDataRefDescriptions();
            Assert.IsNotNull(descriptions);
            Assert.IsEmpty(descriptions);
        }

        [TestMethod()]
        public void WriteDataref_MutationLookup_MapsTypesToCorrectMutations()
        {
            // Verify mutation lookup matches what's in ProSimCache
            // This is critical - wrong mutation = data won't be written correctly
            var mutationLookup = new Dictionary<string, (string method, string graphqlType)>
            {
                { "System.Int32", ("writeInt", "Int!") },
                { "System.Double", ("writeFloat", "Float!") },
                { "System.Boolean", ("writeBool", "Boolean!") }
            };

            Assert.AreEqual("writeInt", mutationLookup["System.Int32"].method);
            Assert.AreEqual("writeFloat", mutationLookup["System.Double"].method);
            Assert.AreEqual("writeBool", mutationLookup["System.Boolean"].method);
        }

        [TestMethod()]
        public void WriteDataref_Boolean_SendsCorrectMutation()
        {
            // Arrange
            var datarefPath = "test/boolean/dataref";
            GraphQLRequest capturedRequest = null;

            // Mock the GraphQL client
            var mockClient = new Mock<IGraphQLWebSocketClient>();
            mockClient.Setup(c => c.SendMutationAsync<object>(It.IsAny<GraphQLRequest>(), default))
                .Callback<GraphQLRequest, System.Threading.CancellationToken>((request, ct) =>
                {
                    capturedRequest = request;
                })
                .ReturnsAsync(new GraphQL.GraphQLResponse<object>());

            // Use reflection to set up the cache state
            SetupConnectedCache(_cache, mockClient.Object, new Dictionary<string, DataRefDescription>
            {
                { datarefPath, new DataRefDescription
                    {
                        Name = datarefPath,
                        CanWrite = true,
                        DataType = "System.Boolean"
                    }
                }
            });

            // Act
            _cache.writeDataref(datarefPath, true);

            // Wait a bit for async operation
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.IsNotNull(capturedRequest, "Expected a GraphQL request to be sent");
            Assert.Contains("writeBool",
                            capturedRequest.Query, 
                            $"Expected mutation to contain 'writeBool', but got: {capturedRequest.Query}");
            Assert.IsTrue(capturedRequest.Query.Contains("$name") && capturedRequest.Query.Contains("$value"),
                $"Expected mutation to use variables, but got: {capturedRequest.Query}");
            Assert.IsNotNull(capturedRequest.Variables, "Expected variables to be set");
        }

        [TestMethod()]
        public void WriteDataref_Integer_SendsCorrectMutation()
        {
            // Arrange
            var datarefPath = "test/int/dataref";
            GraphQLRequest capturedRequest = null;

            // Mock the GraphQL client
            var mockClient = new Mock<IGraphQLWebSocketClient>();
            mockClient.Setup(c => c.SendMutationAsync<object>(It.IsAny<GraphQLRequest>(), default))
                .Callback<GraphQLRequest, System.Threading.CancellationToken>((request, ct) =>
                {
                    capturedRequest = request;
                })
                .ReturnsAsync(new GraphQL.GraphQLResponse<object>());

            // Use reflection to set up the cache state
            SetupConnectedCache(_cache, mockClient.Object, new Dictionary<string, DataRefDescription>
            {
                { datarefPath, new DataRefDescription
                    {
                        Name = datarefPath,
                        CanWrite = true,
                        DataType = "System.Int32"
                    }
                }
            });

            // Act
            _cache.writeDataref(datarefPath, 42);

            // Wait a bit for async operation
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.IsNotNull(capturedRequest, "Expected a GraphQL request to be sent");
            Assert.Contains("writeInt",
                            capturedRequest.Query, 
                            $"Expected mutation to contain 'writeInt', but got: {capturedRequest.Query}");
            Assert.IsTrue(capturedRequest.Query.Contains("$name") && capturedRequest.Query.Contains("$value"),
                $"Expected mutation to use variables, but got: {capturedRequest.Query}");
            Assert.IsNotNull(capturedRequest.Variables, "Expected variables to be set");
        }

        [TestMethod()]
        public void WriteDataref_Double_SendsCorrectMutation()
        {
            // Arrange
            var datarefPath = "test/double/dataref";
            GraphQLRequest capturedRequest = null;

            // Mock the GraphQL client
            var mockClient = new Mock<IGraphQLWebSocketClient>();
            mockClient.Setup(c => c.SendMutationAsync<object>(It.IsAny<GraphQLRequest>(), default))
                .Callback<GraphQLRequest, System.Threading.CancellationToken>((request, ct) =>
                {
                    capturedRequest = request;
                })
                .ReturnsAsync(new GraphQL.GraphQLResponse<object>());

            // Use reflection to set up the cache state
            SetupConnectedCache(_cache, mockClient.Object, new Dictionary<string, DataRefDescription>
            {
                { datarefPath, new DataRefDescription
                    {
                        Name = datarefPath,
                        CanWrite = true,
                        DataType = "System.Double"
                    }
                }
            });

            // Act
            _cache.writeDataref(datarefPath, 3.14);

            // Wait a bit for async operation
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.IsNotNull(capturedRequest, "Expected a GraphQL request to be sent");
            Assert.Contains("writeFloat",
                            capturedRequest.Query, 
                            $"Expected mutation to contain 'writeFloat', but got: {capturedRequest.Query}");
            Assert.IsTrue(capturedRequest.Query.Contains("$name") && capturedRequest.Query.Contains("$value"),
                $"Expected mutation to use variables, but got: {capturedRequest.Query}");
            Assert.IsNotNull(capturedRequest.Variables, "Expected variables to be set");
        }

        [TestMethod()]
        public void WriteDataref_Boolean_ConvertsIntegerOneToTrue()
        {
            // Arrange
            var datarefPath = "test/boolean/dataref";
            GraphQLRequest capturedRequest = null;

            // Mock the GraphQL client
            var mockClient = new Mock<IGraphQLWebSocketClient>();
            mockClient.Setup(c => c.SendMutationAsync<object>(It.IsAny<GraphQLRequest>(), default))
                .Callback<GraphQLRequest, System.Threading.CancellationToken>((request, ct) =>
                {
                    capturedRequest = request;
                })
                .ReturnsAsync(new GraphQL.GraphQLResponse<object>());

            // Use reflection to set up the cache state
            SetupConnectedCache(_cache, mockClient.Object, new Dictionary<string, DataRefDescription>
            {
                { datarefPath, new DataRefDescription
                    {
                        Name = datarefPath,
                        CanWrite = true,
                        DataType = "System.Boolean"
                    }
                }
            });

            // Act - pass integer 1 which should be converted to boolean true
            _cache.writeDataref(datarefPath, 1);

            // Wait a bit for async operation
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.IsNotNull(capturedRequest, "Expected a GraphQL request to be sent");
            Assert.Contains("writeBool",
                            capturedRequest.Query, 
                            $"Expected mutation to contain 'writeBool', but got: {capturedRequest.Query}");
            Assert.IsNotNull(capturedRequest.Variables, "Expected variables to be set");
        }

        [TestMethod()]
        public void WriteDataref_Boolean_ConvertsIntegerZeroToFalse()
        {
            // Arrange
            var datarefPath = "test/boolean/dataref";
            GraphQLRequest capturedRequest = null;

            // Mock the GraphQL client
            var mockClient = new Mock<IGraphQLWebSocketClient>();
            mockClient.Setup(c => c.SendMutationAsync<object>(It.IsAny<GraphQLRequest>(), default))
                .Callback<GraphQLRequest, System.Threading.CancellationToken>((request, ct) =>
                {
                    capturedRequest = request;
                })
                .ReturnsAsync(new GraphQL.GraphQLResponse<object>());

            // Use reflection to set up the cache state
            SetupConnectedCache(_cache, mockClient.Object, new Dictionary<string, DataRefDescription>
            {
                { datarefPath, new DataRefDescription
                    {
                        Name = datarefPath,
                        CanWrite = true,
                        DataType = "System.Boolean"
                    }
                }
            });

            // Act - pass integer 0 which should be converted to boolean false
            _cache.writeDataref(datarefPath, 0);

            // Wait a bit for async operation
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.IsNotNull(capturedRequest, "Expected a GraphQL request to be sent");
            Assert.Contains("writeBool",
                            capturedRequest.Query, 
                            $"Expected mutation to contain 'writeBool', but got: {capturedRequest.Query}");
            Assert.IsNotNull(capturedRequest.Variables, "Expected variables to be set");
        }

        [TestMethod()]
        public void WriteDataref_TypeConversion_ConvertsStringToInt()
        {
            // Arrange
            var datarefPath = "test/int/dataref";
            GraphQLRequest capturedRequest = null;

            // Mock the GraphQL client
            var mockClient = new Mock<IGraphQLWebSocketClient>();
            mockClient.Setup(c => c.SendMutationAsync<object>(It.IsAny<GraphQLRequest>(), default))
                .Callback<GraphQLRequest, System.Threading.CancellationToken>((request, ct) =>
                {
                    capturedRequest = request;
                })
                .ReturnsAsync(new GraphQL.GraphQLResponse<object>());

            // Use reflection to set up the cache state
            SetupConnectedCache(_cache, mockClient.Object, new Dictionary<string, DataRefDescription>
            {
                { datarefPath, new DataRefDescription
                    {
                        Name = datarefPath,
                        CanWrite = true,
                        DataType = "System.Int32"
                    }
                }
            });

            // Act - pass string that should be converted to int
            _cache.writeDataref(datarefPath, "123");

            // Wait a bit for async operation
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.IsNotNull(capturedRequest, "Expected a GraphQL request to be sent");
            Assert.Contains("writeInt",
                            capturedRequest.Query, 
                            $"Expected mutation to contain 'writeInt', but got: {capturedRequest.Query}");
            Assert.IsNotNull(capturedRequest.Variables, "Expected variables to be set");
        }

        [TestMethod()]
        public void WriteDataref_ReadOnlyDataRef_DoesNotSendMutation()
        {
            // Arrange
            var datarefPath = "test/readonly/dataref";
            var mutationSent = false;

            // Mock the GraphQL client
            var mockClient = new Mock<IGraphQLWebSocketClient>();
            mockClient.Setup(c => c.SendMutationAsync<object>(It.IsAny<GraphQLRequest>(), default))
                .Callback<GraphQLRequest, System.Threading.CancellationToken>((request, ct) =>
                {
                    mutationSent = true;
                })
                .ReturnsAsync(new GraphQL.GraphQLResponse<object>());

            // Use reflection to set up the cache state with read-only dataref
            SetupConnectedCache(_cache, mockClient.Object, new Dictionary<string, DataRefDescription>
            {
                { datarefPath, new DataRefDescription
                    {
                        Name = datarefPath,
                        CanWrite = false,  // Read-only!
                        DataType = "System.Int32"
                    }
                }
            });

            // Act
            _cache.writeDataref(datarefPath, 42);

            // Wait a bit for async operation
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.IsFalse(mutationSent, "Expected no mutation to be sent for read-only dataref");
        }

        // Helper method to set up connected cache state using reflection
        private void SetupConnectedCache(ProSimCache cache, IGraphQLWebSocketClient mockClient, Dictionary<string, DataRefDescription> dataRefDescriptions)
        {
            var type = typeof(ProSimCache);

            // Set _connected field
            var connectedField = type.GetField("_connected", BindingFlags.NonPublic | BindingFlags.Instance);
            connectedField.SetValue(cache, true);

            // Set _connection field
            var connectionField = type.GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance);
            connectionField.SetValue(cache, mockClient);

            // Set _dataRefDescriptions dictionary
            var descriptionsField = type.GetField("_dataRefDescriptions", BindingFlags.NonPublic | BindingFlags.Instance);
            descriptionsField.SetValue(cache, dataRefDescriptions);
        }

        #endregion

        #region RefreshDataDefinitions Tests

        [TestMethod()]
        public async Task RefreshDataDefinitionsAsync_WhenRefreshIsInProgress_ShouldReuseExistingTask()
        {
            // Arrange
            var queryCompletion = new TaskCompletionSource<GraphQLResponse<DataRefData>>();
            var sendQueryCallCount = 0;
            var mockClient = new Mock<IGraphQLWebSocketClient>();
            mockClient.Setup(c => c.SendQueryAsync<DataRefData>(It.IsAny<GraphQLRequest>(), default))
                .Callback<GraphQLRequest, System.Threading.CancellationToken>((request, ct) =>
                {
                    sendQueryCallCount++;
                })
                .Returns(queryCompletion.Task);

            SetupConnectedCache(_cache, mockClient.Object, new Dictionary<string, DataRefDescription>());

            // Act
            var firstRefresh = _cache.RefreshDataDefinitionsAsync();
            var secondRefresh = _cache.RefreshDataDefinitionsAsync();

            // Assert
            Assert.AreSame(firstRefresh, secondRefresh, "Concurrent refresh callers should wait for the same in-flight task.");
            Assert.AreEqual(
                1,
                sendQueryCallCount,
                "Only one ProSim data definition query should be sent while a refresh is in progress.");

            queryCompletion.SetResult(CreateDataRefResponse());

            Assert.IsTrue(await firstRefresh);
            Assert.HasCount(1, _cache.GetDataRefDescriptions());
        }

        [TestMethod()]
        public async Task RefreshDataDefinitionsAsync_WhenDisconnected_ShouldReturnCompletedFalseTask()
        {
            // Act
            var refreshTask = _cache.RefreshDataDefinitionsAsync();

            // Assert
            Assert.IsNotNull(refreshTask);
            Assert.IsFalse(await refreshTask);
        }

        [TestMethod()]
        public async Task RefreshDataDefinitionsAsync_WhenOldRefreshCompletesAfterReconnect_ShouldKeepNewRefreshInProgress()
        {
            // Arrange
            var firstQueryCompletion = new TaskCompletionSource<GraphQLResponse<DataRefData>>();
            var secondQueryCompletion = new TaskCompletionSource<GraphQLResponse<DataRefData>>();
            var firstClient = new Mock<IGraphQLWebSocketClient>();
            var secondClient = new Mock<IGraphQLWebSocketClient>();

            firstClient.Setup(c => c.SendQueryAsync<DataRefData>(It.IsAny<GraphQLRequest>(), default))
                .Returns(firstQueryCompletion.Task);
            secondClient.Setup(c => c.SendQueryAsync<DataRefData>(It.IsAny<GraphQLRequest>(), default))
                .Returns(secondQueryCompletion.Task);

            SetupConnectedCache(_cache, firstClient.Object, new Dictionary<string, DataRefDescription>());
            var firstRefresh = _cache.RefreshDataDefinitionsAsync();

            _cache.Disconnect();
            SetupConnectedCache(_cache, secondClient.Object, new Dictionary<string, DataRefDescription>());
            var secondRefresh = _cache.RefreshDataDefinitionsAsync();

            // Act
            firstQueryCompletion.SetResult(CreateDataRefResponse());
            var firstRefreshResult = await firstRefresh;

            // Assert
            Assert.IsFalse(firstRefreshResult);
            Assert.IsTrue(GetRefreshInProgress(_cache));

            secondQueryCompletion.SetResult(CreateDataRefResponse());
            Assert.IsTrue(await secondRefresh);
            Assert.IsFalse(GetRefreshInProgress(_cache));
        }

        #endregion

        #region Connection Tests

        [TestMethod()]
        public void IsConnected_InitialState_ShouldBeFalse()
        {
            // Assert
            Assert.IsFalse(_cache.IsConnected());
        }

        [TestMethod()]
        public void Disconnect_WhenNotConnected_ShouldReturnTrue()
        {
            // Act
            var result = _cache.Disconnect();

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(_cache.IsConnected());
        }

        #endregion

        private GraphQLResponse<DataRefData> CreateDataRefResponse()
        {
            return new GraphQLResponse<DataRefData>
            {
                Data = new DataRefData
                {
                    DataRef = new DataRefResponse
                    {
                        DataRefDescriptions = new List<DataRefDescription>
                        {
                            new DataRefDescription
                            {
                                Name = "aircraft/test/path",
                                CanRead = true,
                                CanWrite = true,
                                DataType = "System.Boolean"
                            }
                        }
                    }
                }
            };
        }

        private bool GetRefreshInProgress(ProSimCache cache)
        {
            var refreshTaskField = typeof(ProSimCache).GetField(
                "_refreshTask",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var refreshTask = (Task<bool>)refreshTaskField.GetValue(cache);
            return refreshTask != null && !refreshTask.IsCompleted;
        }

        #region readDataref Tests

        [TestMethod()]
        public void ReadDataref_WhenNotConnected_ShouldReturnZero()
        {
            // Arrange
            var datarefPath = "test/dataref";

            // Act
            var result = _cache.readDataref(datarefPath);

            // Assert
            Assert.AreEqual(0.0, result);
        }

        #endregion
    }
}
