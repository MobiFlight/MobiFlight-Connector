using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class MobiFlightVariableTest
    {
        [TestMethod]
        public void MobiFlightVariable_HasCorrectDefaultValues()
        {
            // Arrange + Act
            var variable = new MobiFlightVariable();
            
            // Assert
            Assert.AreEqual(MobiFlightVariable.TYPE_NUMBER, variable.TYPE);
            Assert.AreEqual("MyVar", variable.Name);
            Assert.AreEqual(0.0, variable.Number);
            Assert.AreEqual(string.Empty, variable.Text);
            Assert.AreEqual("$", variable.Expression);
        }

        [TestMethod]
        public void MobiFlightVariable_Clone_Test()
        {
            // Arrange
            MobiFlightVariable original = new MobiFlightVariable()
            {
                TYPE = MobiFlightVariable.TYPE_NUMBER,
                Name = "TestVar",
                Number = 42.0,
                Text = "TestText",
                Expression = "$ + 1"
            };

            // Act
            MobiFlightVariable clone = (MobiFlightVariable)original.Clone();

            // Assert
            Assert.AreEqual(original.TYPE, clone.TYPE);
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Number, clone.Number);
            Assert.AreEqual(original.Text, clone.Text);
            Assert.AreEqual(original.Expression, clone.Expression);
        }

        [TestMethod]
        public void MobiFlightVariable_Equals_IsTrueForEqualValues_Test()
        {
            // Arrange
            MobiFlightVariable var1 = new MobiFlightVariable()
            {
                TYPE = MobiFlightVariable.TYPE_NUMBER,
                Name = "TestVar",
                Number = 42.0,
                Text = "TestText",
                Expression = "$ + 1"
            };
            MobiFlightVariable var2 = new MobiFlightVariable()
            {
                TYPE = MobiFlightVariable.TYPE_NUMBER,
                Name = "TestVar",
                Number = 42.0,
                Text = "TestText",
                Expression = "$ + 1"
            };

            // Act + Assert
            Assert.IsTrue(var1.Equals(var2));
        }
        [TestMethod]
        public void MobiFlightVariable_Equals_WorksCorrectlyForIndividualProps_Test()
        {
            // Arrange
            MobiFlightVariable var1 = new MobiFlightVariable()
            {
                TYPE = MobiFlightVariable.TYPE_NUMBER,
                Name = "TestVar",
                Number = 42.0,
                Text = "TestText",
                Expression = "$ + 1"
            };
            MobiFlightVariable var2 = new MobiFlightVariable()
            {
                TYPE = MobiFlightVariable.TYPE_STRING,
                Name = "TestVar",
                Number = 42.0,
                Text = "TestText",
                Expression = "$ + 1"
            };

            // Act + Assert
            Assert.IsFalse(var1.Equals(var2));
            // Arrange 
            var1.TYPE = MobiFlightVariable.TYPE_STRING;
            // Act + Assert
            Assert.IsTrue(var1.Equals(var2));

            // Arrange
            var2.Name = "DifferentName";
            // Act + Assert
            Assert.IsFalse(var1.Equals(var2));
            // Arrange
            var1.Name = "DifferentName";
            // Act + Assert
            Assert.IsTrue(var1.Equals(var2));

            // Arrange
            var2.Number = 999;
            // Act + Assert
            Assert.IsFalse(var1.Equals(var2));
            // Arrange
            var1.Number = 999;
            // Act + Assert
            Assert.IsTrue(var1.Equals(var2));

            // Arrange
            var2.Text = "DifferentText";
            // Act + Assert
            Assert.IsFalse(var1.Equals(var2));
            // Arrange
            var1.Text = "DifferentText";
            // Act + Assert
            Assert.IsTrue(var1.Equals(var2));

            // Arrange
            var2.Expression = "DifferentExpression";
            // Act + Assert
            Assert.IsFalse(var1.Equals(var2));
            // Arrange
            var1.Expression = "DifferentExpression";
            // Act + Assert
            Assert.IsTrue(var1.Equals(var2));
        }
    }
}