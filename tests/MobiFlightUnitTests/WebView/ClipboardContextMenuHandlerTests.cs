using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.WebView;
using System.Linq;

namespace MobiFlightUnitTests.WebView
{
    [TestClass()]
    public class ClipboardContextMenuHandlerTests
    {
        #region GetAllowedItemNames

        [TestMethod()]
        [DataRow(false)]
        [DataRow(true)]
        public void GetAllowedItemNames_ShouldReturnClipboardItems_WhenTargetIsEditable(bool hasSelection)
        {
            // Arrange
            string[] expected = ["cut", "copy", "paste", "selectAll"];

            // Act
            var names = ClipboardContextMenuHandler.GetAllowedItemNames(isEditable: true, hasSelection);

            // Assert
            CollectionAssert.AreEquivalent(expected, names.ToList());
        }

        [TestMethod()]
        public void GetAllowedItemNames_ShouldReturnOnlyCopy_WhenReadOnlyTextIsSelected()
        {
            // Arrange
            string[] expected = ["copy"];

            // Act
            var names = ClipboardContextMenuHandler.GetAllowedItemNames(isEditable: false, hasSelection: true);

            // Assert
            CollectionAssert.AreEquivalent(expected, names.ToList());
        }

        [TestMethod()]
        public void GetAllowedItemNames_ShouldReturnNothing_WhenTargetIsNotEditableAndNothingIsSelected()
        {
            // Act
            var names = ClipboardContextMenuHandler.GetAllowedItemNames(isEditable: false, hasSelection: false);

            // Assert
            Assert.IsEmpty(names);
        }

        #endregion
    }
}
