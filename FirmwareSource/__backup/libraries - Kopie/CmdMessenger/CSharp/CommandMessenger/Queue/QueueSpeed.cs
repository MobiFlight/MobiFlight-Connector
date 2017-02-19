#region CmdMessenger - MIT - (c) 2013 Thijs Elenbaas.
/*
  CmdMessenger - library that provides command based messaging

  Permission is hereby granted, free of charge, to any person obtaining
  a copy of this software and associated documentation files (the
  "Software"), to deal in the Software without restriction, including
  without limitation the rights to use, copy, modify, merge, publish,
  distribute, sublicense, and/or sell copies of the Software, and to
  permit persons to whom the Software is furnished to do so, subject to
  the following conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  Copyright 2013 - Thijs Elenbaas
*/
#endregion

using System;
using System.Threading;

namespace CommandMessenger
{
    /// <summary> Class that regulates sleeping  within a queue thread.  
    /// 		  Based on load the sleep time will increase or decrease </summary>
    public class QueueSpeed
    {
        private long _queueCount;
        private long _prevTime;
        private long _sleepTime;
        private const double Alpha = 0.8;
        private readonly double _targetQueue = 0.5;
        private const long MaxSleep = 50;
        private const long MinSleep = 0;

        /// <summary> Gets or sets the QueueSpeed name. Used for debugging </summary>
        /// <value> The object name. </value>
        public string Name { get; set;  }

        /// <summary> Initialize the queue speed with a target filling of the queue. </summary>
        /// <param name="targetQueue"> target filling of the queue. </param>
        public  QueueSpeed(double targetQueue)
        {
            _targetQueue = targetQueue;       
            _prevTime = TimeUtils.Millis;
            _sleepTime = 0;
        }

        /// <summary> Calculates the sleep time taking into account work being done in queue. </summary>
        public void CalcSleepTime() {
            var currentTime = TimeUtils.Millis;
            var deltaT = (currentTime-_prevTime);
            var processT = deltaT- _sleepTime;
            double rate = (double)_queueCount / (double)deltaT;
            double targetT = Math.Min(_targetQueue / rate, MaxSleep); 
            double compensatedT = Math.Min(Math.Max(targetT - processT, 0), 1e6);
            _sleepTime = Math.Max(Math.Min((long)(Alpha * _sleepTime + (1 - Alpha) * compensatedT), MaxSleep), MinSleep);

            //if (Name != "" && Name != null)
            //{
            //     Console.WriteLine("Rate {1} {0}", Name, rate);
            //    Console.WriteLine("Sleeptime {1} {0}", Name, _sleepTime);
            //}

            // Reset
            _prevTime = currentTime;
            _queueCount = 0;
        }

        /// <summary> Calculates the sleep without time taking into account work being done in queue. </summary>
        public void CalcSleepTimeWithoutLoad()
        {
            var currentTime = TimeUtils.Millis;
            var deltaT = (currentTime - _prevTime);
            double rate = _queueCount / (double)deltaT;
            double targetT = Math.Min(_targetQueue / rate,MaxSleep);
            _sleepTime = Math.Max((long)(Alpha * _sleepTime + (1 - Alpha) * targetT), MinSleep);
            //if (Name != "" && Name != null)
            //{
                //Console.WriteLine("Rate {1} {0}", Name, rate);
                //Console.WriteLine("targetT {1} {0}", Name, targetT);
                //Console.WriteLine("sleepTime {1} {0}", Name, _sleepTime);
            //}

            // Reset
            _prevTime = currentTime;
            _queueCount = 0;
        }

        /// <summary> Adds a unit to the load count. </summary>
        public void AddCount() {
            _queueCount++;
        }

        /// <summary> Adds a count units to the load count. </summary>
        /// <param name="count"> Number of load units to increase. </param>
        public void AddCount(int count)
        {
            _queueCount+= count;
        }

        /// <summary> Sets the count units to the load count. </summary>
        /// <param name="count"> Number of load units to increase. </param>
        public void SetCount(int count)
        {
            _queueCount = count;
        }

        /// <summary> Resets the count units to zero. </summary>
        public void ResetCount()
        {
            _queueCount = 0;
        }

        /// <summary> Perform the sleep based on load. </summary>
        public void Sleep() {
            Sleep(_sleepTime);
        }

        public void Sleep(long millis)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(millis));
        }
    }
}
