using MobiFlightMoza.Cdu;

namespace MobiFlightMoza.Cdu.Tests
{
    [TestClass]
    public class CduPageTests
    {
        #region TryParse

        [TestMethod]
        public void TryParse_FullPage_Parses336Cells()
        {
            // Arrange
            string json = BuildJson(("A", "w", 0, false));

            // Act
            var parsed = CduPage.TryParse(json, out var page, out _);

            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual('A', page[0, 0].Character);
        }

        [TestMethod]
        public void TryParse_BlankCell_DefaultsToSpaceAndWhite()
        {
            // Arrange
            string json = "{\"Target\":\"Display\",\"Data\":[[]]}";

            // Act
            CduPage.TryParse(json, out var page, out _);

            // Assert
            Assert.AreEqual(' ', page[0, 0].Character);
            Assert.AreEqual('w', page[0, 0].ColorLetter);
        }

        [TestMethod]
        public void TryParse_ThreeElementCell_DefaultsInvertedToFalse()
        {
            // Arrange
            string json = "{\"Target\":\"Display\",\"Data\":[[\"A\",\"r\",1]]}";

            // Act
            CduPage.TryParse(json, out var page, out _);

            // Assert
            Assert.IsFalse(page[0, 0].IsInverted);
            Assert.IsTrue(page[0, 0].IsSmall);
        }

        [TestMethod]
        public void TryParse_FourElementCell_ReadsInverted()
        {
            // Arrange
            string json = "{\"Target\":\"Display\",\"Data\":[[\"A\",\"r\",0,true]]}";

            // Act
            CduPage.TryParse(json, out var page, out _);

            // Assert
            Assert.IsTrue(page[0, 0].IsInverted);
        }

        [TestMethod]
        public void TryParse_ShortDataArray_LeavesRestBlank()
        {
            // Arrange
            string json = "{\"Target\":\"Display\",\"Data\":[[\"A\",\"w\",0]]}"; // only 1 of 336 cells

            // Act
            var parsed = CduPage.TryParse(json, out var page, out _);

            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual('A', page[0, 0].Character);
            Assert.AreEqual(' ', page[0, 1].Character);
        }

        [TestMethod]
        public void TryParse_LongDataArray_IgnoresExtraCells()
        {
            // Arrange
            var cells = new string[340];
            for (int i = 0; i < cells.Length; i++) cells[i] = "[\"A\",\"w\",0]";
            string json = "{\"Target\":\"Display\",\"Data\":[" + string.Join(",", cells) + "]}";

            // Act
            var parsed = CduPage.TryParse(json, out _, out _);

            // Assert
            Assert.IsTrue(parsed);
        }

        [TestMethod]
        public void TryParse_MissingDataProperty_ReturnsFalse()
        {
            // Arrange
            string json = "{\"Target\":\"Display\"}";

            // Act
            var parsed = CduPage.TryParse(json, out _, out string error);

            // Assert
            Assert.IsFalse(parsed);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void TryParse_MalformedJson_ReturnsFalse()
        {
            // Act
            var parsed = CduPage.TryParse("not json", out _, out string error);

            // Assert
            Assert.IsFalse(parsed);
            Assert.IsNotNull(error);
        }

        #endregion

        #region RowEquals

        [TestMethod]
        public void RowEquals_IdenticalRows_ReturnsTrue()
        {
            // Arrange
            var a = CduPage.CreateBlank();
            var b = CduPage.CreateBlank();

            // Act & Assert
            Assert.IsTrue(a.RowEquals(b, 0));
        }

        [TestMethod]
        public void RowEquals_DifferentCharacter_ReturnsFalse()
        {
            // Arrange
            var a = CduPage.CreateBlank();
            var b = CduPage.CreateBlank();
            b[0, 0] = new CduCell('X', 'w', false, false);

            // Act & Assert
            Assert.IsFalse(a.RowEquals(b, 0));
        }

        [TestMethod]
        public void RowEquals_OnlyStyleDiffers_ReturnsFalse()
        {
            // Arrange
            var a = CduPage.CreateBlank();
            var b = CduPage.CreateBlank();
            a[0, 0] = new CduCell(' ', 'w', false, false);
            b[0, 0] = new CduCell(' ', 'r', false, false);

            // Act & Assert
            Assert.IsFalse(a.RowEquals(b, 0));
        }

        #endregion

        #region GetRowText

        [TestMethod]
        public void GetRowText_ReturnsExactlyColumnsCharacters()
        {
            // Arrange
            var page = CduPage.CreateBlank();

            // Act
            string text = page.GetRowText(0);

            // Assert
            Assert.AreEqual(CduPage.Columns, text.Length);
        }

        #endregion

        private static string BuildJson(params (string Character, string Color, int Size, bool Inverted)[] firstCells)
        {
            var builder = new System.Text.StringBuilder("{\"Target\":\"Display\",\"Data\":[");
            foreach (var cell in firstCells)
            {
                builder.Append($"[\"{cell.Character}\",\"{cell.Color}\",{cell.Size},{cell.Inverted.ToString().ToLowerInvariant()}],");
            }
            for (int i = firstCells.Length; i < CduPage.Rows * CduPage.Columns; i++)
            {
                builder.Append("[],");
            }
            builder.Length--; // trailing comma
            builder.Append("]}");
            return builder.ToString();
        }
    }
}
