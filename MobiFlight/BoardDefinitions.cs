using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    static class BoardDefinitions
    {
        private static List<Board> boards = new List<Board>();

        /// <summary>
        /// Finds a board definition by matching against the USB VID/PID.
        /// </summary>
        /// <param name="hardwareIdPattern">A RegEx of the VID/PID to match against.</param>
        /// <returns>The first board definition matching the hardwareIdPattern, or null if none found.</returns>
        public static Board GetBoardByHardwareId(String hardwareIdPattern)
        {
            return boards.Find(board => board.HardwareIds.Any(hardwareId =>
            {
                var regEx = new Regex(hardwareId);
                return regEx.Match(hardwareIdPattern).Success;
            }));
        }
        
        /// <summary>
        /// Finds a board definition by matching against the manufacturer's name for the board.
        /// </summary>
        /// <param name="friendlyName">The name to search for</param>
        /// <returns>The first board definition matching the name, or null if none found</returns>
        public static Board GetBoardByFriendlyName(String friendlyName)
        {
            return boards.Find(board => board.FriendlyName.ToLower() == friendlyName.ToLower());
        }

        /// <summary>
        /// Finds a board definition by matching against the MobiFlight firmware name for the board.
        /// </summary>
        /// <param name="mobiflightType">The name to search for</param>
        /// <returns>The first board definition matching the name, or null if none found</returns>
        public static Board GetBoardByMobiFlightType(String mobiflightType)
        {
            return boards.Find(board => board.MobiFlightType.ToLower() == mobiflightType?.ToLower());
        }

        /// <summary>
        /// Loads all board definintions from disk.
        /// </summary>
        public static void Load()
        {
            var serializer = new XmlSerializer(typeof(Board));
            foreach (var definitionFile in Directory.GetFiles("Boards", "*.board.xml"))
            {
                using (var reader = XmlReader.Create(definitionFile))
                {
                    try
                    {
                        var board = (Board)serializer.Deserialize(reader);
                        boards.Add(board);
                        Log.Instance.log($"Loaded board definition for {board.FriendlyName}", LogSeverity.Info);
                    } catch (Exception ex)
                    {
                        Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                    }
                }
            }
        }
    }
}
