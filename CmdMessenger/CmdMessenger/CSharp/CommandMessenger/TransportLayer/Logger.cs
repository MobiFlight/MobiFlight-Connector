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
            isEnabled = true;
            isOpen = false;
        }

        public Logger()
        {
            LogFileName = null;
        }

        ~Logger()
        {
            Close();
        }

        public bool isEnabled { get; set; }
        public bool isOpen { get; private set; }

        /// <summary> Gets or sets the log file name. </summary>
        /// <value> The logfile name . </value>
        public String LogFileName
        {
            get { return _logFileName; }
            set
            {
                if (value != _logFileName && value != null)
                {
                    _logFileName = value;
                    if (isOpen) { Open(); }
                }
            }
        }

        public bool Open()
        {
            return Open(LogFileName);
        }

        public bool Open(string logFileName)
        {
            LogFileName = logFileName;
            if (isOpen) { 
                try
                {
                    _fileStream.Close();
                }
                catch (Exception) {}
                isOpen = false;
            }

            try
            {
                _fileStream = new FileStream(logFileName, FileMode.Create, FileAccess.ReadWrite);
                
            }
            catch (Exception)
            {
                return false;
            }
            isOpen = true;
            return true;
        }

        public void Close()
        {
            if (isOpen)
            {
                try
                {
                    _fileStream.Close();
                }
                catch (Exception) { }
                isOpen = false;
            }
        }

        public void Log(string logString)
        {
            if (isEnabled && isOpen)
            {
                byte[] writeBytes = StringEncoder.GetBytes(logString);
                _fileStream.Write(writeBytes, 0, writeBytes.Length);
                _fileStream.Flush();
            }
        }


        public void LogLine(string logString)
        {
            Log(logString + '\n');
        }
    }
}
