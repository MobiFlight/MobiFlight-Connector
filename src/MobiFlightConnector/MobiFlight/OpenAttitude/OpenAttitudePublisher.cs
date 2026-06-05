using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.OpenAttitude
{
    /// <summary>
    /// Subscribes to <see cref="ConfigValueRawAndFinalUpdate"/> from <see cref="MessageExchange"/>
    /// and forwards each changed config value to the <see cref="PropertyListenerServer"/>
    /// using the config's Name as the property path.
    /// </summary>
    public class OpenAttitudePublisher
    {
        private readonly PropertyListenerServer _server;
        private readonly Func<IReadOnlyList<IConfigItem>> _configItemsProvider;

        public OpenAttitudePublisher(PropertyListenerServer server, Func<IReadOnlyList<IConfigItem>> configItemsProvider)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _configItemsProvider = configItemsProvider ?? throw new ArgumentNullException(nameof(configItemsProvider));
        }

        public void SubscribeToUpdates()
        {
            MessageExchange.Instance.Subscribe<ConfigValueRawAndFinalUpdate>(OnConfigValuesUpdated);
        }

        internal void OnConfigValuesUpdated(ConfigValueRawAndFinalUpdate update)
        {
            if (!_server.IsRunning) return;
            if (update?.ConfigItems == null || update.ConfigItems.Count == 0) return;

            // Build a GUID→full config item lookup for names
            var lookup = _configItemsProvider()
                .OfType<IConfigItem>()
                .ToDictionary(c => c.GUID, c => c);

            foreach (var item in update.ConfigItems)
            {
                if (!lookup.TryGetValue(item.GUID, out var fullItem)) continue;

                var path = fullItem.Name;
                if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(item.Value)) continue;

                // Try to push as a number; fall back to string so the panel coerces correctly
                object value = double.TryParse(item.Value,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var d)
                    ? (object)d
                    : item.Value;

                _server.BroadcastUpdate(path, value);
            }
        }
    }
}