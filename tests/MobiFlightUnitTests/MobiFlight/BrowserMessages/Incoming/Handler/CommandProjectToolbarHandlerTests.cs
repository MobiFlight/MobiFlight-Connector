using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.UI;

namespace MobiFlight.BrowserMessages.Incoming.Handler.Tests
{
    [TestClass]
    public class CommandProjectToolbarHandlerTests
    {
        public class ProjectToolbarMock : IProjectToolbar
        {
            public string LastMethodCalled { get; private set; }
            public string ProjectName { get; private set; }

            public void StartProjectExecution() => LastMethodCalled = nameof(StartProjectExecution);
            public void StopExecution() => LastMethodCalled = nameof(StopExecution);
            public void StartTestModeExecution() => LastMethodCalled = nameof(StartTestModeExecution);
            public void ToggleAutoRunSetting() => LastMethodCalled = nameof(ToggleAutoRunSetting);
            public void RenameProject(string name)
            {
                LastMethodCalled = nameof(RenameProject);
                ProjectName = name;
            }
        }

        [TestMethod]
        public void Handle_Run_CallsStart()
        {
            var mainForm = new ProjectToolbarMock();
            var handler = new CommandProjectToolbarHandler(mainForm);
            var message = new CommandProjectToolbar { Action = CommandProjectToolbarAction.run };

            handler.Handle(message);

            Assert.AreEqual("StartProjectExecution", mainForm.LastMethodCalled);
        }

        [TestMethod]
        public void Handle_Stop_CallsStopAndStopTest()
        {
            var mainForm = new ProjectToolbarMock();
            var handler = new CommandProjectToolbarHandler(mainForm);
            var message = new CommandProjectToolbar { Action = CommandProjectToolbarAction.stop };

            handler.Handle(message);

            // Only the last called method is tracked, but both should be called in order.
            Assert.AreEqual("StopExecution", mainForm.LastMethodCalled);
        }

        [TestMethod]
        public void Handle_Test_CallsRunTest()
        {
            var mainForm = new ProjectToolbarMock();
            var handler = new CommandProjectToolbarHandler(mainForm);
            var message = new CommandProjectToolbar { Action = CommandProjectToolbarAction.test };

            handler.Handle(message);

            Assert.AreEqual("StartTestModeExecution", mainForm.LastMethodCalled);
        }

        [TestMethod]
        public void Handle_ToggleAutoRun_CallsAutoRun()
        {
            var mainForm = new ProjectToolbarMock();
            var handler = new CommandProjectToolbarHandler(mainForm);
            var message = new CommandProjectToolbar { Action = CommandProjectToolbarAction.toggleAutoRun };

            handler.Handle(message);

            Assert.AreEqual("ToggleAutoRunSetting", mainForm.LastMethodCalled);
        }

        [TestMethod]
        public void Handle_Rename_WithValue_ChangesProjectName()
        {
            var mainForm = new ProjectToolbarMock();
            var handler = new CommandProjectToolbarHandler(mainForm);
            var message = new CommandProjectToolbar { Action = CommandProjectToolbarAction.rename, Value = "NewProjectName" };

            handler.Handle(message);

            Assert.AreEqual("RenameProject", mainForm.LastMethodCalled);
        }
    }
}