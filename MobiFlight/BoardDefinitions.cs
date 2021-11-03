using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    static class BoardDefinitions
    {
        private static List<Board> boards = new List<Board>();

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
                        Log.Instance.log($"Loaded board definition for {board.ManufacturerName}", LogSeverity.Info);
                    } catch (Exception ex)
                    {
                        Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                    }
                }
            }
        }
    }
}
