using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace MobiFlight
{
    public static class BoardDefinitions
    {
        private static readonly List<Board> boards = new List<Board>();

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
        /// Loads all board definintions from disk, verifing them against the board JSON schema.
        /// </summary>
        public static void Load()
        {
            var schema = JSchema.Parse(File.ReadAllText("Boards/mfboard.schema.json"));

            foreach (var definitionFile in Directory.GetFiles("Boards", "*.board.json"))
            {
                try
                {
                    // Load the inital, un-migrated, board definition file.
                    var rawBoard = JObject.Parse(File.ReadAllText(definitionFile));
                    
                    // Convert the JObject to a Board object and migrate it. Migration has to
                    // happen before schema validation to ensure old files that have an upgrade
                    // flow won't fail validation.
                    var board = rawBoard.ToObject<Board>();
                    board.Migrate();

                    // Now that the board is migrated it can be converted back to a JObject for schema validation.
                    // The NullValueHandling must be set to ignore otherwise the converted JObject will have a bunch of
                    // null values that cause schema validation to fail as invalid values.
                    rawBoard = JObject.FromObject(board, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

                    // Actually validate against the schema. If the schema validation fails then continue
                    // to the next file, otherwise add the board to the list of known board definitions.
                    var jsonIsValid = rawBoard.IsValid(schema, out IList<string> validationMessages);

                    if (!jsonIsValid)
                    {
                        Log.Instance.log($"Board definition file {definitionFile} isn't valid:", LogSeverity.Error);
                        foreach (var message in validationMessages)
                        {
                            Log.Instance.log(message, LogSeverity.Error);
                        }

                        continue;
                    }

                    boards.Add(board);
                    Log.Instance.log($"Loaded board definition for {board.Info.MobiFlightType} ({board.Info.FriendlyName})", LogSeverity.Info);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                }
            }
        }
    }
}