using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Base;
using System.Collections.Generic;
using System.Windows.Forms;

namespace System.Tests
{
    [TestClass()]
    public class ComboBoxHelperTests
    {
        [TestMethod()]
        public void SetSelectedItemTest()
        {
            ComboBox cb = generateTestObject();

            Assert.AreNotEqual(1,cb.SelectedIndex, "Selected index should not be 1");
            ComboBoxHelper.SetSelectedItem(cb, "App");
            Assert.AreNotEqual(1, cb.SelectedIndex, "Selected index should not be 1");
            ComboBoxHelper.SetSelectedItem(cb, "Apples");
            Assert.AreEqual(0, cb.SelectedIndex, "Selected index should be 0");
            ComboBoxHelper.SetSelectedItem(cb, "Cherries");
            Assert.AreEqual(1, cb.SelectedIndex, "Selected index should be 1");
            ComboBoxHelper.SetSelectedItem(cb, "Cheese");
            Assert.AreEqual(2, cb.SelectedIndex,"Selected index should be 2");
        }

        [TestMethod()]
        public void SetSelectedItemByPartTest()
        {
            ComboBox cb = new ComboBox();
            cb.Items.Add("Apples");
            cb.Items.Add("Cherries");
            cb.Items.Add("Cheese");

            Assert.AreNotEqual(1, cb.SelectedIndex, "Selected index should not be 1");
            ComboBoxHelper.SetSelectedItemByPart(cb, "App");
            Assert.AreEqual(0, cb.SelectedIndex, "Selected index should be 0");
            ComboBoxHelper.SetSelectedItemByPart(cb, "Che");
            Assert.AreEqual(1, cb.SelectedIndex, "Selected index should be 1");
            ComboBoxHelper.SetSelectedItemByPart(cb, "Chee");
            Assert.AreEqual(2, cb.SelectedIndex, "Selected index should be 2");
        }

        [TestMethod()]
        public void SetSelectedListItemByValue_WithController_ShouldSelectMatchingItem()
        {
            // Arrange
            ComboBox cb = new ComboBox();
            var controller1 = new Controller { Name = "Module1", Serial = "SN001" };
            var controller2 = new Controller { Name = "Module2", Serial = "SN002" };
            var controller3 = new Controller { Name = "Module3", Serial = "SN003" };

            cb.Items.Add(new ListItem<Controller> { Label = "Module1 (COM3)", Value = controller1 });
            cb.Items.Add(new ListItem<Controller> { Label = "Module2 (COM4)", Value = controller2 });
            cb.Items.Add(new ListItem<Controller> { Label = "Module3 (COM5)", Value = controller3 });

            var searchController = new Controller { Name = "Module2", Serial = "SN002" };

            // Act
            bool result = ComboBoxHelper.SetSelectedListItemByValue(cb, searchController);

            // Assert
            Assert.IsTrue(result, "Should return true when item is found");
            Assert.AreEqual(1, cb.SelectedIndex, "Should select the matching controller");
            Assert.AreEqual(controller2, ((ListItem<Controller>)cb.SelectedItem).Value, "Selected item should be controller2");
        }

        [TestMethod()]
        public void SetSelectedListItemByValue_WithNullValue_ShouldNotThrowAndReturnFalse()
        {
            // Arrange
            ComboBox cb = new ComboBox();
            var controller1 = new Controller { Name = "Module1", Serial = "SN001" };

            cb.Items.Add(new ListItem<Controller> { Label = "-", Value = null });
            cb.Items.Add(new ListItem<Controller> { Label = "Module1 (COM3)", Value = controller1 });

            Controller searchController = null;

            // Act
            bool result = ComboBoxHelper.SetSelectedListItemByValue(cb, searchController);

            // Assert
            Assert.IsTrue(result, "Should return true when both are null");
            Assert.AreEqual(0, cb.SelectedIndex, "Should select the item with null value");
        }

        [TestMethod()]
        public void SetSelectedListItemByValue_WithNoMatch_ShouldReturnFalseAndNotChangeSelection()
        {
            // Arrange
            ComboBox cb = new ComboBox();
            var controller1 = new Controller { Name = "Module1", Serial = "SN001" };
            var controller2 = new Controller { Name = "Module2", Serial = "SN002" };

            cb.Items.Add(new ListItem<Controller> { Label = "Module1 (COM3)", Value = controller1 });
            cb.Items.Add(new ListItem<Controller> { Label = "Module2 (COM4)", Value = controller2 });
            cb.SelectedIndex = 0;

            var searchController = new Controller { Name = "Module3", Serial = "SN003" };

            // Act
            bool result = ComboBoxHelper.SetSelectedListItemByValue(cb, searchController);

            // Assert
            Assert.IsFalse(result, "Should return false when item is not found");
            Assert.AreEqual(0, cb.SelectedIndex, "Should not change selection when item not found");
        }

        [TestMethod()]
        public void SetSelectedListItemByValue_WithEmptyComboBox_ShouldReturnFalse()
        {
            // Arrange
            ComboBox cb = new ComboBox();
            var searchController = new Controller { Name = "Module1", Serial = "SN001" };

            // Act
            bool result = ComboBoxHelper.SetSelectedListItemByValue(cb, searchController);

            // Assert
            Assert.IsFalse(result, "Should return false when combobox is empty");
            Assert.AreEqual(-1, cb.SelectedIndex, "Selected index should remain -1");
        }

        [TestMethod()]
        public void SetSelectedListItemByValue_WithMultipleNullValues_ShouldSelectFirstNull()
        {
            // Arrange
            ComboBox cb = new ComboBox();
            cb.Items.Add(new ListItem<Controller> { Label = "-", Value = null });
            cb.Items.Add(new ListItem<Controller> { Label = "Empty", Value = null });
            cb.Items.Add(new ListItem<Controller> { Label = "Module1 (COM3)", Value = new Controller { Name = "Module1", Serial = "SN001" } });

            Controller searchController = null;

            // Act
            bool result = ComboBoxHelper.SetSelectedListItemByValue(cb, searchController);

            // Assert
            Assert.IsTrue(result, "Should return true when match found");
            Assert.AreEqual(0, cb.SelectedIndex, "Should select the first item with null value");
        }

        [TestMethod()]
        public void SetSelectedListItemByValue_WithPartialMatch_ShouldNotSelect()
        {
            // Arrange
            ComboBox cb = new ComboBox();
            var controller1 = new Controller { Name = "Module1", Serial = "SN001" };
            var controller2 = new Controller { Name = "Module1", Serial = "SN002" }; // Same name, different serial

            cb.Items.Add(new ListItem<Controller> { Label = "Module1 (COM3)", Value = controller1 });
            cb.Items.Add(new ListItem<Controller> { Label = "Module1 (COM4)", Value = controller2 });

            var searchController = new Controller { Name = "Module1", Serial = "SN003" }; // Name matches, serial doesn't

            // Act
            bool result = ComboBoxHelper.SetSelectedListItemByValue(cb, searchController);

            // Assert
            Assert.IsFalse(result, "Should return false when only partial match exists");
            Assert.AreEqual(-1, cb.SelectedIndex, "Should not select any item");
        }

        #region Pin assignment

        [TestMethod()]
        [DataRow("")]
        [DataRow(null)]
        public void FreePin_WithUnassignedPin_ShouldNotThrowAndKeepPinsUsed(string unassignedPin)
        {
            // Arrange
            var pinList = generatePinList(usedPins: 2);

            // Act
            ComboBoxHelper.freePin(pinList, unassignedPin);

            // Assert
            Assert.IsTrue(pinList[0].Used, "Pin 2 should still be used");
            Assert.IsTrue(pinList[1].Used, "Pin 3 should still be used");
        }

        [TestMethod()]
        public void FreePin_WithAssignedPin_ShouldMarkOnlyThatPinAsFree()
        {
            // Arrange
            var pinList = generatePinList(usedPins: 2);

            // Act
            ComboBoxHelper.freePin(pinList, "2");

            // Assert
            Assert.IsFalse(pinList[0].Used, "Pin 2 should be free");
            Assert.IsTrue(pinList[1].Used, "Pin 3 should still be used");
        }

        [TestMethod()]
        [DataRow("")]
        [DataRow(null)]
        public void ReassignPin_FromUnassignedPin_ShouldAssignNewPin(string unassignedPin)
        {
            // Arrange
            var pinList = generatePinList(usedPins: 0);
            var currentPin = unassignedPin;

            // Act
            ComboBoxHelper.reassignPin("3", pinList, ref currentPin);

            // Assert
            Assert.AreEqual("3", currentPin, "The new pin should be stored in the config");
            Assert.IsFalse(pinList[0].Used, "Pin 2 should be untouched");
            Assert.IsTrue(pinList[1].Used, "Pin 3 should be marked as used");
        }

        [TestMethod()]
        public void ReassignPin_FromAssignedPin_ShouldFreeOldAndUseNewPin()
        {
            // Arrange
            var pinList = generatePinList(usedPins: 1);
            var currentPin = "2";

            // Act
            ComboBoxHelper.reassignPin("3", pinList, ref currentPin);

            // Assert
            Assert.AreEqual("3", currentPin, "The new pin should be stored in the config");
            Assert.IsFalse(pinList[0].Used, "The old pin should be free");
            Assert.IsTrue(pinList[1].Used, "The new pin should be marked as used");
        }

        [TestMethod()]
        public void BindMobiFlightFreePins_WithUnassignedCurrentPin_ShouldListOnlyFreePins()
        {
            // Arrange
            var pinList = generatePinList(usedPins: 1);
            var comboBox = new ComboBox();

            // Act
            ComboBoxHelper.BindMobiFlightFreePins(comboBox, pinList, "");

            // Assert
            var boundPins = (List<MobiFlightPin>)comboBox.DataSource;
            Assert.AreEqual(1, boundPins.Count, "Only the free pin should be listed");
            Assert.AreEqual(3u, boundPins[0].Pin);
        }

        [TestMethod()]
        public void BindMobiFlightFreePins_WithAssignedCurrentPin_ShouldAlsoListCurrentPin()
        {
            // Arrange
            var pinList = generatePinList(usedPins: 1);
            var comboBox = new ComboBox();

            // Act
            ComboBoxHelper.BindMobiFlightFreePins(comboBox, pinList, "2");

            // Assert
            var boundPins = (List<MobiFlightPin>)comboBox.DataSource;
            Assert.AreEqual(2, boundPins.Count, "The current pin should be selectable again");
            Assert.IsTrue(pinList[0].Used, "The base pin list must not be modified");
        }

        private static List<MobiFlightPin> generatePinList(int usedPins)
        {
            var pins = new List<MobiFlightPin>
            {
                new MobiFlightPin { Pin = 2 },
                new MobiFlightPin { Pin = 3 }
            };

            for (var i = 0; i < usedPins; i++) pins[i].Used = true;

            return pins;
        }

        #endregion

        public ComboBox generateTestObject()
        {
            ComboBox cb = new ComboBox();
            cb.Items.Add(new ComboboxItem() { Value = "Apples", Text = "Apples" });
            cb.Items.Add(new ComboboxItem() { Value = "Cherries", Text = "Cherries" });
            cb.Items.Add(new ComboboxItem() { Value = "Cheese", Text = "Cheese" });
            return cb;
        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}