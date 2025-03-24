using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
using Moq;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config.Tests
{
    [TestClass()]
    public class FsuipcConfigPanelTests
    {
        [TestMethod()]
        public void syncFromConfig_InitializeWithIFsuipcConfigItem_Test()
        {
            var panel = new FsuipcConfigPanel();
            var fsuipcOffset = new FsuipcOffset
            {
                Offset = 0x1234,
                Size = 4,
                OffsetType = FSUIPCOffsetType.Integer,
                Mask = 0xFFFFFFFF,
                BcdMode = true
            };
            var modifiers = new ModifierList();
            var configItem = new Mock<IFsuipcConfigItem>();
            configItem.Setup(c => c.FSUIPC).Returns(fsuipcOffset);
            configItem.Setup(c => c.Modifiers).Returns(modifiers);

            // Act
            panel.syncFromConfig(configItem.Object);

            // Assert
            var fsuipcOffsetTextBox = panel.Controls.Find("fsuipcOffsetTextBox", true)[0];
            Assert.AreEqual("0x1234", fsuipcOffsetTextBox.Text);
            var fsuipcOffsetTypeComboBox = panel.Controls.Find("fsuipcOffsetTypeComboBox", true)[0] as ComboBox;
            Assert.AreEqual(FSUIPCOffsetType.Integer.ToString(), fsuipcOffsetTypeComboBox.SelectedValue);
            var fsuipcSizeComboBox = panel.Controls.Find("fsuipcSizeComboBox", true)[0] as ComboBox;
            Assert.AreEqual("4", fsuipcSizeComboBox.Text);
            var fsuipcMaskTextBox = panel.Controls.Find("fsuipcMaskTextBox", true)[0];
            Assert.AreEqual("0xFFFFFFFF", fsuipcMaskTextBox.Text);
            var fsuipcBcdModeCheckBox = panel.Controls.Find("fsuipcBcdModeCheckBox", true)[0] as CheckBox;
            Assert.IsTrue(fsuipcBcdModeCheckBox.Checked);
        }

        [TestMethod()]
        public void syncFromConfig_InitializeWithOutputConfigItem_Test()
        {
            var panel = new FsuipcConfigPanel();
            var fsuipcOffset = new FsuipcOffset
            {
                Offset = 0x1234,
                Size = 4,
                OffsetType = FSUIPCOffsetType.Integer,
                Mask = 0xFFFFFFFF,
                BcdMode = true
            };
            var modifiers = new ModifierList();

            var outputConfigItem = new OutputConfigItem()
            {
                Source = new FsuipcSource()
                {
                    FSUIPC = fsuipcOffset
                },
                Modifiers = modifiers
            };


            // Act
            panel.syncFromConfig(outputConfigItem);

            // Assert
            var fsuipcOffsetTextBox = panel.Controls.Find("fsuipcOffsetTextBox", true)[0];
            Assert.AreEqual("0x1234", fsuipcOffsetTextBox.Text);
            var fsuipcOffsetTypeComboBox = panel.Controls.Find("fsuipcOffsetTypeComboBox", true)[0] as ComboBox;
            Assert.AreEqual(FSUIPCOffsetType.Integer.ToString(), fsuipcOffsetTypeComboBox.SelectedValue);
            var fsuipcSizeComboBox = panel.Controls.Find("fsuipcSizeComboBox", true)[0] as ComboBox;
            Assert.AreEqual("4", fsuipcSizeComboBox.Text);
            var fsuipcMaskTextBox = panel.Controls.Find("fsuipcMaskTextBox", true)[0];
            Assert.AreEqual("0xFFFFFFFF", fsuipcMaskTextBox.Text);
            var fsuipcBcdModeCheckBox = panel.Controls.Find("fsuipcBcdModeCheckBox", true)[0] as CheckBox;
            Assert.IsTrue(fsuipcBcdModeCheckBox.Checked);
        }
    }
}