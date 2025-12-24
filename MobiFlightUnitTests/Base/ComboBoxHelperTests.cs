using Microsoft.VisualStudio.TestTools.UnitTesting;
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