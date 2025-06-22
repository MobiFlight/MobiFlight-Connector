using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingFontConverter
    {
        private List<byte[]> SplitAndPadArray(byte[] dataArray, int size = 63, byte padValue = 0, bool isFill = true)
        {
            var result = new List<byte[]>();

            for (int i = 0; i < dataArray.Length; i += size)
            {
                int chunkSize = Math.Min(size, dataArray.Length - i);
                byte[] chunk = new byte[chunkSize];

                for (int j = 0; j < chunkSize; j++)
                {
                    chunk[j] = dataArray[i + j];
                }

                if (isFill && chunkSize < size)
                {
                    byte[] finalChunk = new byte[size];

                    for (int j = 0; j < chunkSize; j++)
                    {
                        finalChunk[j] = chunk[j];
                    }

                    for (int j = chunkSize; j < size; j++)
                    {
                        finalChunk[j] = padValue;
                    }

                    chunk = finalChunk;
                }
                result.Add(chunk);                
            }

            return result;
        }

        private List<byte[]> CreateFontHeadCommand(WinwingFontHeadConfig fontHeadConfig, byte[] destinationAddress)
        {
            List<byte[]> commands = new List<byte[]>();
            List<byte> command = new List<byte>(destinationAddress);
            command.AddRange(new byte[2]);
            command.AddRange(WinwingConstants.DisplayCmdHeaders["0601"]);
            command.AddRange(BitConverter.GetBytes(fontHeadConfig.id));
            command.AddRange(BitConverter.GetBytes(fontHeadConfig.matrixW));
            command.AddRange(BitConverter.GetBytes(fontHeadConfig.matrixH));
            command.AddRange(BitConverter.GetBytes(fontHeadConfig.glyphSize));
            command.AddRange(BitConverter.GetBytes(fontHeadConfig.charSize));
            command.AddRange(BitConverter.GetBytes(fontHeadConfig.version));
            command.AddRange(BitConverter.GetBytes(fontHeadConfig.size));
            command.AddRange(new byte[] { fontHeadConfig.isWriteFlash });
            commands.Add(command.ToArray());
            return commands;
        }

        private List<byte[]> CreateFontCommands(WinwingFontConfig fontConfig, byte[] destinationAddress)
        {
            int offset = 0;
            List<byte[]> commands = new List<byte[]>();            
            var pixelDataList = SplitAndPadArray(fontConfig.pixelUint8Array, size: 512, isFill: false);
            foreach (var pixelData in pixelDataList)
            {
                int pixelDataLength = pixelData.Length;
                List<byte> fontData = new List<byte>();
                fontData.AddRange(BitConverter.GetBytes(fontConfig.headConfig.id));
                fontData.AddRange(BitConverter.GetBytes(offset));
                fontData.AddRange(BitConverter.GetBytes(pixelDataLength));
                fontData.AddRange(pixelData);

                List<byte> command = new List<byte>(destinationAddress);
                command.AddRange(new byte[2]);
                command.AddRange(WinwingConstants.DisplayCmdHeaders["0701"]);         
                command.AddRange(BitConverter.GetBytes(fontData.Count));
                command.AddRange(fontData);
                commands.Add(command.ToArray());

                offset += pixelDataLength;
            }
            
            return commands;
        }

        public WinwingFontCommands FontJsonToDisplayCommands(string fontJson, byte[] destinationAddress)
        {           
            // Done like that because otherwise confusor renaming protection does not work, 
            // when deserializing into class
            JObject obj = JObject.Parse(fontJson);                                   
            WinwingFontHeadConfig headConfigLarge = new WinwingFontHeadConfig();
            headConfigLarge.id = (int)obj["largeFontConfig"]["headConfig"]["id"];
            headConfigLarge.matrixW = (short)obj["largeFontConfig"]["headConfig"]["matrixW"];
            headConfigLarge.matrixH = (short)obj["largeFontConfig"]["headConfig"]["matrixH"];                       
            headConfigLarge.glyphSize = (int)obj["largeFontConfig"]["headConfig"]["glyphSize"];
            headConfigLarge.charSize = (int)obj["largeFontConfig"]["headConfig"]["charSize"];
            headConfigLarge.size = (int)obj["largeFontConfig"]["headConfig"]["size"];
            headConfigLarge.version = (int)obj["largeFontConfig"]["headConfig"]["version"];
            headConfigLarge.isWriteFlash = (byte)obj["largeFontConfig"]["headConfig"]["isWriteFlash"];

            WinwingFontHeadConfig headConfigSmall = new WinwingFontHeadConfig();
            headConfigSmall.id = (int)obj["smallFontConfig"]["headConfig"]["id"];
            headConfigSmall.matrixW = (short)obj["smallFontConfig"]["headConfig"]["matrixW"];
            headConfigSmall.matrixH = (short)obj["smallFontConfig"]["headConfig"]["matrixH"];
            headConfigSmall.glyphSize = (int)obj["smallFontConfig"]["headConfig"]["glyphSize"];
            headConfigSmall.charSize = (int)obj["smallFontConfig"]["headConfig"]["charSize"];
            headConfigSmall.size = (int)obj["smallFontConfig"]["headConfig"]["size"];
            headConfigSmall.version = (int)obj["smallFontConfig"]["headConfig"]["version"];
            headConfigSmall.isWriteFlash = (byte)obj["smallFontConfig"]["headConfig"]["isWriteFlash"];

            WinwingFontConfig largeFontConfig = new WinwingFontConfig();
            largeFontConfig.headConfig = headConfigLarge;
            JArray pixelArray = (JArray)obj["largeFontConfig"]["pixelUint8Array"];
            largeFontConfig.pixelUint8Array = pixelArray.Select(p => (byte)p).ToArray();

            WinwingFontConfig smallFontConfig = new WinwingFontConfig();
            smallFontConfig.headConfig = headConfigSmall;
            JArray pixelArray2 = (JArray)obj["smallFontConfig"]["pixelUint8Array"];
            smallFontConfig.pixelUint8Array = pixelArray2.Select(p => (byte)p).ToArray();

            WinwingFontData fontData = new WinwingFontData();
            fontData.smallFontConfig = smallFontConfig;
            fontData.largeFontConfig = largeFontConfig;

            WinwingFontCommands fontCommands = new WinwingFontCommands();
            fontCommands.LargeFontHead = CreateFontHeadCommand(largeFontConfig.headConfig, destinationAddress);
            fontCommands.SmallFontHead = CreateFontHeadCommand(smallFontConfig.headConfig, destinationAddress);
            fontCommands.LargeFont = CreateFontCommands(largeFontConfig, destinationAddress);
            fontCommands.SmallFont = CreateFontCommands(smallFontConfig, destinationAddress);

            return fontCommands;
        }
    }
}
