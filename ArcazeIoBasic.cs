using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcazeUSB
{
    class ArcazeIoBasic
    {
        public const int MAX_PIN_NUM = 40;
        private int pin;

        public int Port { get; set; }
        public int Pin
        {
            get { return pin; }
            set {
            if (value < 0  || value > MAX_PIN_NUM) throw new ArgumentOutOfRangeException ("Trying to use a pin higher than " + MAX_PIN_NUM);
            this.pin = value;
            } 
        }


        /// <summary>
        /// default constructor
        /// </summary>
        public ArcazeIoBasic()
        {
            Port = -1;
            Pin = -1;
        } // ArcazeIoBasic()

        /// <summary>
        /// constructor, pass in a port and pin combo like "A01" or "D12"
        /// </summary>
        /// <param name="portPin"></param>
        public ArcazeIoBasic(string portPin)
        {
            Parse(portPin);
        } // ArcazeIoBasic()

        /// <summary>
        /// parse a given port and pin combo like "A01"
        /// </summary>
        /// <param name="portPin"></param>
        public void Parse(string portPin)
        {
            // determine the correct arcaze usb pin defined by string, e.g. "A01"            
            switch (portPin.ElementAt(0))
            {
                // Base board
                case 'A':
                    Port = 0;
                    break;
                case 'B':
                    Port = 1;
                    break;
                // Extension: LED Driver 1
                case 'C':
                    Port = 2;
                    break;
                case 'D':
                    Port = 3;
                    break;
                case 'E':
                    Port = 4;
                    break;
                // Extension: LED Driver 2
                case 'F':
                    Port = 5;
                    break;
                case 'G':
                    Port = 6;
                    break;
                case 'H':
                    Port = 7;
                    break;
                // Extension: LED Driver 3
                case 'I':
                    Port = 8;
                    break;
                case 'J':
                    Port = 9;
                    break;
                case 'K':
                    Port = 10;
                    break;
                // Extension: LED Driver 4
                case 'L':
                    Port = 11;
                    break;
                case 'M':
                    Port = 12;
                    break;
                case 'N':
                    Port = 13;
                    break;
            }

            Int32 pin = (Int32.Parse(portPin.Substring(1)));
            Pin = (Int32)(pin - 1);
        }

        public bool isValid()
        {
            return (Port != -1 && Pin != -1);
        }

    }
}
