using System;

namespace ArcazeUSB
{
    public class Adapter
    {
        public enum ArcazeMaskCommand
        {
            AND,
            OR,
            COPYBOOL,
            COPYBOOL_INVERTED
        }

        public Int32 m_gridIndex;
        public Int32 m_offset;
        public Int32 m_dataSize;
        public Int32 m_mask;
        public Int32 m_lastValue = 0;
        public Int32 m_currValue = 0;

        public String m_arcazeSerial = "";
        public Int32 m_arcazePort = 0;
        public ArcazeMaskCommand m_arcazeMaskCommand = ArcazeMaskCommand.COPYBOOL;
        public Int32 m_arcazeMask = 0;

        public Adapter(
          Int32 gridIndex,
          Int32 offset,
          Int32 dataSize,
          Int32 mask,

          String arcazeSerial,
          Int32 arcazePort,
          ArcazeMaskCommand arcazeMaskCommand,
          Int32 arcazeMask)
        {
            m_gridIndex = gridIndex;
            m_offset = offset;
            m_dataSize = dataSize;
            m_mask = mask;

            m_arcazeSerial = arcazeSerial;
            m_arcazePort = arcazePort;
            m_arcazeMaskCommand = arcazeMaskCommand;
            m_arcazeMask = arcazeMask;
        }

        public static ArcazeMaskCommand String2ArcazeMaskCommand(String cmd)
        {
            switch (cmd)
            {
                case "AND": return ArcazeMaskCommand.AND;
                case "OR": return ArcazeMaskCommand.OR;
                case "COPYBOOL": return ArcazeMaskCommand.COPYBOOL;
                case "COPYBOOL_INV": return ArcazeMaskCommand.COPYBOOL_INVERTED;
                default: return ArcazeMaskCommand.COPYBOOL;
            }
        }

    }
}
