using System;
using System.IO;
using System.Text;

namespace CommandMessenger
{
    static class Logger
    {
        private static readonly Encoding StringEncoder = Encoding.GetEncoding("ISO-8859-1");	// The string encoder
        private static FileStream _fileStream;

        static Logger()
        {
            LogFileName = null;
            IsEnabled = true;
        }

        static public bool IsEnabled { get; set; }
        static public bool IsOpen { get; private set; }
        static public bool DirectFlush { get; set; }

        /// <summary> Gets or sets the log file name. </summary>
        /// <value> The logfile name . </value>
        static public String LogFileName { get; private set; }


        static public bool Open()
        {
            return Open(LogFileName);
        }

        static public bool Open(string logFileName)
        {
            if (IsOpen && LogFileName == logFileName) return true;

            LogFileName = logFileName;
            if (IsOpen)
            {
                try
                {
                    _fileStream.Close();
                }
                catch (Exception) { }
                IsOpen = false;
            }

            try
            {
                _fileStream = new FileStream(logFileName, FileMode.Create, FileAccess.ReadWrite);
            }
            catch (Exception)
            {
                return false;
            }
            IsOpen = true;
            return true;
        }

        static public void Close()
        {
            if (!IsOpen) return;
            try
            {
                _fileStream.Close();
            }
            catch (Exception) { }
            IsOpen = false;
        }

        static public void Log(string logString)
        {
            if (!IsEnabled || !IsOpen) return;
            var writeBytes = StringEncoder.GetBytes(logString);
            _fileStream.Write(writeBytes, 0, writeBytes.Length);
            if (DirectFlush) _fileStream.Flush();
        }

        static public void LogLine(string logString)
        {
            Log(logString + Environment.NewLine);
        }
    }
}
