#region CmdMessenger - MIT - (c) 2014 Thijs Elenbaas.
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

  Copyright 2014 - Thijs Elenbaas
*/
#endregion

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommandMessenger.Transport.Bluetooth
{
    public class BluetoothConnectionStorer : IBluetoothConnectionStorer
    {
        private readonly string _settingsFileName;
        /// <summary>
        /// Contructor of Store/Retreive object for SerialConnectionManagerSettings
        /// The file is serialized as a simple binary file
        /// </summary>
        public BluetoothConnectionStorer()
        {
            _settingsFileName = @"BluetoothConnectionManagerSettings.cfg";
        }

        /// <summary>
        /// Contructor of Store/Retreive object for SerialConnectionManagerSettings
        /// The file is serialized as a simple binary file
        /// </summary>
        /// <param name="settingsFileName">Filename of the settings file</param>
        public BluetoothConnectionStorer(string settingsFileName)
        {
            _settingsFileName = settingsFileName;
        }

        /// <summary>
        /// Store SerialConnectionManagerSettings
        /// </summary>
        /// <param name="bluetoothConnectionManagerSettings">BluetoothConnectionManagerSettings</param>
        public void StoreSettings(BluetoothConnectionManagerSettings bluetoothConnectionManagerSettings)
        {
            var fileStream = File.Create(_settingsFileName);
            var serializer = new BinaryFormatter();
            serializer.Serialize(fileStream, bluetoothConnectionManagerSettings);
            fileStream.Close();
        }

        /// <summary>
        /// Retreive SerialConnectionManagerSettings
        /// </summary>
        /// <returns>SerialConnectionManagerSettings</returns>
        public BluetoothConnectionManagerSettings RetrieveSettings()
        {
            var bluetoothConnectionManagerSettings = new BluetoothConnectionManagerSettings();
            if (File.Exists(_settingsFileName))
            {
                var fileStream = File.OpenRead(_settingsFileName);
                var deserializer = new BinaryFormatter();
                bluetoothConnectionManagerSettings = (BluetoothConnectionManagerSettings)deserializer.Deserialize(fileStream);
                fileStream.Close();
            }
            return bluetoothConnectionManagerSettings;
        }
    }
}
