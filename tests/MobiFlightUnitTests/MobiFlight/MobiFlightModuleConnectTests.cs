namespace MobiFlight.Tests
{
    [TestClass]
    public class MobiFlightModuleConnectTests
    {
        [TestMethod]
        public void Connect_ShouldReturnNotResponding_WhenPortDoesNotExist()
        {
            // Arrange
            BoardDefinitions.LoadDefinitions();
            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");
            var module = new MobiFlightModule("COM987", board);

            // Act
            var result = module.Connect();

            // Assert
            Assert.AreEqual(ConnectResult.NotResponding, result);
            Assert.IsFalse(module.Connected);
        }
    }
}