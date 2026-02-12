using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
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