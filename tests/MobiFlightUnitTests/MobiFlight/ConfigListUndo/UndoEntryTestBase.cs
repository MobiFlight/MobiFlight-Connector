using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.ConfigListUndo;
using MobiFlight.FSUIPC;
using MobiFlight.ProSim;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using Moq;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    public abstract class UndoEntryTestBase
    {
        private ExecutionManager _executionManager;
        private IMessagePublisher _previousPublisher;

        private protected Mock<IMessagePublisher> Publisher
        {
            get;
            private set;
        }

        private protected ConfigListUndoContext Context
        {
            get;
            private set;
        }

        protected Project TestProject { get; private set; }

        protected ConfigFile TestFile { get; private set; }

        [TestInitialize]
        public void Setup()
        {
            _previousPublisher = MessageExchange.Instance.GetPublisher();

            Publisher = new Mock<IMessagePublisher>();

            MessageExchange.Instance.SetPublisher(Publisher.Object);

            _executionManager = new ExecutionManager(
                IntPtr.Zero,
                new Mock<XplaneCacheInterface>().Object,
                new Mock<SimConnectCacheInterface>().Object,
                new Mock<FSUIPCCacheInterface>().Object,
                new Mock<ProSimCacheInterface>().Object);

            TestFile = new ConfigFile
            {
                Label = "Test Profile"
            };

            TestProject = new Project();
            TestProject.ConfigFiles.Add(TestFile);

            _executionManager.Project = TestProject;
            _executionManager.ActiveConfigIndex = 0;

            Context = new ConfigListUndoContext(_executionManager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                _executionManager?.Stop();
                _executionManager?.Shutdown();
            }
            finally
            {
                MessageExchange.Instance.ClearSubscriptions();

                MessageExchange.Instance.SetPublisher(_previousPublisher);
            }
        }

        protected static InputConfigItem CreateItem(string name, bool active = true)
        {
            return new InputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = name,
                Active = active
            };
        }

        private protected void AssertConfigItemsPublished(
            IConfigListUndoEntry entry,
            ConfigFile file,
            int configIndex)
        {
            Publisher.Invocations.Clear();

            entry.PublishChanges(Context);

            Publisher.Verify(
                publisher => publisher.Publish(
                    It.Is<ConfigValueFullUpdate>(message =>
                        message.ConfigIndex == configIndex &&
                        ReferenceEquals(message.ConfigItems,file.ConfigItems))),
                Times.Once());
        }

        private protected void AssertProjectPublished(IConfigListUndoEntry entry)
        {
            Publisher.Invocations.Clear();

            entry.PublishChanges(Context);

            Publisher.Verify(
                publisher => publisher.Publish(
                    It.Is<Project>(project => ReferenceEquals(project, TestProject))),
                Times.Once());
        }
    }
}
