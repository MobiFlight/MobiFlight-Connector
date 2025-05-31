using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
            WinwingFontData fontData = JsonConvert.DeserializeObject<WinwingFontData>(fontJson);
            WinwingFontCommands fontCommands = new WinwingFontCommands();
            WinwingFontConfig largeFontConfig = fontData.largeFontConfig;
            WinwingFontConfig smallFontConfig = fontData.smallFontConfig;
            fontCommands.LargeFontHead = CreateFontHeadCommand(largeFontConfig.headConfig, destinationAddress);
            fontCommands.SmallFontHead = CreateFontHeadCommand(smallFontConfig.headConfig, destinationAddress);
            fontCommands.LargeFont = CreateFontCommands(largeFontConfig, destinationAddress);
            fontCommands.SmallFont = CreateFontCommands(smallFontConfig, destinationAddress);

            return fontCommands;
        }
    }
}
