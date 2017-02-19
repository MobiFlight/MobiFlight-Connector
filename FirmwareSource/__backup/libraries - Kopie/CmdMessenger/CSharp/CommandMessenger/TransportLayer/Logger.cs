using System;
using System.IO;
using System.Text;

namespace CommandMessenger.TransportLayer
{
    class Logger
    {
        public readonly Encoding StringEncoder = Encoding.GetEncoding("ISO-8859-1");	// The string encoder
        private FileStream _fileStream;
        private string _logFileName;

        public Logger(string logFileName)
        {
            LogFileName = logFileName;
        }

        public Logger()
        {
            LogFileName = null;
        }

        ~Logger()
        {
            try
            {
                _fileStream.Close();
            }
            catch (Exception) { }
        }


        /// <summary> Gets or sets the log file name. </summary>
        /// <value> The logfile name . </value>
        public String LogFileName
        {
            get { return _logFileName; }
            set
            {
                if (value != _logFileName && value != null)
                {
                    if (OpenLogFile(value))
                        _logFileName = value;
                }
            }
        }

        private bool OpenLogFile(string logFileName)
        {
            try
            {
                _fileStream.Close();
            }
            catch (Exception) {}

            try
            {
                _fileStream = new FileStream(logFileName, FileMode.Create, FileAccess.ReadWrite);
                
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Log(string logString)
        {
            byte[] writeBytes = StringEncoder.GetBytes(logString);
            _fileStream.Write(writeBytes, 0, writeBytes.Length);
            _fileStream.Flush();
        }


        public void LogLine(string logString)
        {
            byte[] writeBytes = StringEncoder.GetBytes(logString + '\n');
            _fileStream.Write(writeBytes, 0, writeBytes.Length);
            _fileStream.Flush();
        }
    }
}
