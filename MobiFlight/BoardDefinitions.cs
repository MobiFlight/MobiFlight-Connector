using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json.Schema;

namespace MobiFlight
{
    public static class BoardDefinitions
    {
        // Set to true if any errors occurred when loading the definition files.
        // Used as part of the unit test automation to determine if the checked-in
        // JSON files are valid.
        public static bool LoadingError = false;

        private static List<Board> boards = new List<Board>();

        /// <summary>
        /// Finds a board definition by matching against USB drive volume label. This does not check for the
        /// presence of a secondary file on the drive to confirm it is a supported USB drive. It is assumed
        /// for the initial identification purposes that the drive name is sufficient.
        /// </summary>
        /// <param name="volumeLabel">The volume label to match</param>
        /// <returns>The first board definition matching the volumeLabel, or null if none found.</returns>
        public static Board GetBoardByUsbVolumeLabel(String volumeLabel)
        {
            return boards.Find(board => board.UsbDriveSettings?.VolumeLabel == volumeLabel);
        }

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
        /// Finds all matching board definitions by matching against the USB VID/PID.
        /// </summary>
        /// <param name="hardwareIdPattern">A RegEx of the VID/PID to match against.</param>
        /// <returns>A list of board definitions matching the hardwareIdPattern or empty list if none found.</returns>
        public static List<Board> GetBoardsByHardwareId(String hardwareIdPattern)
        {
            return boards.FindAll(board => board.HardwareIds.Any(hardwareId =>
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
            return boards.Find(board => board.Info.FriendlyName.ToLower() == friendlyName.ToLower());
        }

        /// <summary>
        /// Finds a board definition by matching against the MobiFlight firmware name for the board.
        /// </summary>
        /// <param name="mobiflightType">The name to search for</param>
        /// <returns>The first board definition matching the name, or null if none found</returns>
        public static Board GetBoardByMobiFlightType(String mobiflightType)
        {
            return boards.Find(board => board.Info.MobiFlightType.ToLower() == mobiflightType?.ToLower());
        }

        /// <summary>
        /// Loads all board definintions from disk.
        /// </summary>
        public static void LoadDefinitions()
        {
            var files = new List<String>(Directory.GetFiles("Boards", "*.board.json"));
            files.AddRange(Directory.GetFiles("Community", "*.board.json", SearchOption.AllDirectories));
            boards = JsonBackedObject.LoadDefinitions<Board>(files.ToArray(), "Boards/mfboard.schema.json",
                onSuccess: (board, definitionFile) => {
                    Log.Instance.log($"Loaded board definition for {board.Info.MobiFlightType} ({board.Info.FriendlyName})", LogSeverity.Info);

                    var boardPath = Path.GetDirectoryName(definitionFile);
                    board.BasePath = Path.GetDirectoryName(boardPath);

                    // Take the logo as the fallback for the board picture
                    var logoPath = $@"{board.BasePath}\logo.png";
                    if (File.Exists(logoPath))
                    {
                        board.Info.BoardPicture = Image.FromFile(logoPath);
                    }

                    // check if we have a more board specifc picture
                    logoPath = $@"{boardPath}\{board.Info.FirmwareBaseName}.png";
                    if (File.Exists(logoPath))
                    {
                        board.Info.BoardPicture = Image.FromFile(logoPath);
                    }

                    var boardIconPath = $@"{boardPath}\board-logo.png";
                    if (File.Exists(boardIconPath))
                    {
                        board.Info.BoardIcon = Image.FromFile(boardIconPath);
                    }
                },
                onError: () => LoadingError = true
                ); ;
        }
    }
}