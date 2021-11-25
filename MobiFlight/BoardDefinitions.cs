﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    public static class BoardDefinitions
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
            return boards.Find(board => board.Info.FriendlyName.ToLower() == friendlyName.ToLower());
        }

        /// <summary>
        /// Finds a board definition by matching against the MobiFlight firmware name for the board.
        /// If multiple board definitions match this returns the board definition with the highest
        /// MinimumFirmwareVersion that supports the provided current firmware version.
        /// </summary>
        /// <param name="mobiflightType">The name to search for.</param>
        /// <param name="version">The version of the firmware on the board.</param>
        /// <returns>The first board definition matching the name and firmware version, or null if none found.</returns>
        public static Board GetBoardByMobiFlightType(String mobiflightType, string version)
        {
            var firmwareVersion = new Version(version);

            // Candidate boards match the mobiflight type and have a minimum firmware version equal to or lower than the
            // firmware version provided.
            var candidateBoards = boards.FindAll(board => {
                return board.Info.MobiFlightType.ToLower() == mobiflightType?.ToLower() &&
                       board.Info.MinimumFirmwareVersion.CompareTo(firmwareVersion) <= 0;
                });

            if (candidateBoards == null)
            {
                return null;
            }

            // Sort the boards in descending order by MinimumFirmwareVersion
            candidateBoards.Sort((y, x) => x.Info.MinimumFirmwareVersion.CompareTo(y.Info.MinimumFirmwareVersion));

            // Return the first board in the sorted list, which will be the one with the highest MinimumFirmwareVersion.
            return candidateBoards[0];
        }

        /// <summary>
        /// Loads all board definintions from disk.
        /// </summary>
        public static void Load()
        {
            foreach (var definitionFile in Directory.GetFiles("Boards", "*.board.json"))
            {
                try
                {
                    var board = JsonConvert.DeserializeObject<Board>(File.ReadAllText(definitionFile));
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
