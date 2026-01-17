using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Controllers
{
    /// <summary>
    /// Performs auto-binding analysis and application for controller serials
    /// </summary>
    public class ControllerAutoBinder
    {
        private readonly List<string> _connectedControllers;
        private readonly Dictionary<string, int> _controllerTypeCount = new Dictionary<string, int>();

        public ControllerAutoBinder(List<string> connectedControllers)
        {
            _connectedControllers = connectedControllers ?? new List<string>();

            // Count controllers by type:name
            foreach (var controllerSerial in _connectedControllers)
            {
                var deviceIdentifier = GetDeviceIdentifier(controllerSerial);

                if (!_controllerTypeCount.ContainsKey(deviceIdentifier))
                    _controllerTypeCount[deviceIdentifier] = 0;

                _controllerTypeCount[deviceIdentifier]++;
            }
        }

        /// <summary>
        /// Gets a unique key combining device name and serial prefix for matching
        /// </summary>
        private static string GetDeviceIdentifier(string controllerSerial)
        {
            var deviceName = SerialNumber.ExtractDeviceName(controllerSerial);
            var deviceSerialPrefix = SerialNumber.ExtractPrefix(controllerSerial);
            return $"{deviceSerialPrefix}:{deviceName}";
        }

        /// <summary>
        /// Analyzes binding status for all config items without modifying them
        /// Returns a dictionary mapping original serial -> binding status
        /// </summary>
        public List<ControllerBinding> AnalyzeBindings(List<IConfigItem> configItems, List<ControllerBinding> existingBindings)
        {
            var results = new List<ControllerBinding>();
            var availableControllers = new List<string>(_connectedControllers);

            var uniqueSerials = configItems
                .Where(c => !string.IsNullOrEmpty(c.ModuleSerial) && c.ModuleSerial != "-")
                .Select(c => c.ModuleSerial)
                .Distinct()
                .OrderByDescending(serial => availableControllers.Contains(serial) || (existingBindings?.FirstOrDefault(b => b.OriginalController == serial) != null))
                .ToList();

            foreach (var serial in uniqueSerials)
            {
                // Check if this serial was already bound in a previous config file
                var previouslyBoundController = existingBindings?.FirstOrDefault(b => b.OriginalController == serial);
                var alreadyBoundInPreviousConfigFile = previouslyBoundController != null;
                if (alreadyBoundInPreviousConfigFile)
                {
                    // Check if the previously bound controller is still available
                    if (!availableControllers.Contains(previouslyBoundController.BoundController)) continue;

                    // Reuse the same binding
                    var previousStatus = previouslyBoundController.BoundController == serial
                        ? ControllerBindingStatus.Match
                        : ControllerBindingStatus.AutoBind;

                    results.Add(new ControllerBinding()
                    {
                        Status = previousStatus,
                        BoundController = previouslyBoundController.BoundController,
                        OriginalController = serial
                    });

                    availableControllers.Remove(previouslyBoundController.BoundController);
                    continue;
                }

                var controllerBinding = AnalyzeSingleBinding(serial, uniqueSerials, availableControllers);

                results.Add(controllerBinding);
                if (controllerBinding.Status == ControllerBindingStatus.Match)
                {
                    // Remove from available controllers to prevent multiple bindings
                    availableControllers.Remove(controllerBinding.BoundController);
                }

                if (controllerBinding.Status == ControllerBindingStatus.AutoBind)
                {
                    availableControllers.Remove(controllerBinding.BoundController);
                }
            }

            return results;
        }

        /// <summary>
        /// Applies auto-binding updates to config items based on analysis results
        /// </summary>
        /// <returns>Dictionary mapping original serial -> new serial (only for AutoBound items)</returns>
        public List<ControllerBinding> ApplyAutoBinding(
            List<IConfigItem> configItems,
            List<ControllerBinding> bindingStatus)
        {
            var serialMappings = bindingStatus.Where((status) => status.Status == ControllerBindingStatus.AutoBind);

            if (serialMappings.Count() == 0) return serialMappings.ToList();

            // Apply the mappings to config items
            foreach (var item in configItems)
            {
                if (string.IsNullOrEmpty(item.ModuleSerial) || item.ModuleSerial == "-")
                    continue;

                var mapping = serialMappings.FirstOrDefault(m => m.OriginalController == item.ModuleSerial);
                if (mapping == null) continue;

                item.ModuleSerial = mapping.BoundController;
            }

            return serialMappings.ToList();
        }

        private ControllerBinding AnalyzeSingleBinding(string configSerial, List<string> uniqueSerials, List<string> availableControllers)
        {
            // Scenario 1: Exact match
            if (availableControllers.Contains(configSerial))
            {
                return new ControllerBinding() { Status = ControllerBindingStatus.Match, BoundController = configSerial, OriginalController = configSerial };
            }

            var deviceTypeName = GetDeviceIdentifier(configSerial);
            var potentialTypeNameMatches = availableControllers
                .Where(c => GetDeviceIdentifier(c) == deviceTypeName)
                .ToList();

            var deviceSerial = SerialNumber.ExtractSerial(configSerial);
            var potentialSerialMatches = availableControllers
                .Where(c => SerialNumber.ExtractSerial(c) == deviceSerial)
                .ToList();

            // Scenario 4: Missing
            if (potentialTypeNameMatches.Count == 0 && potentialSerialMatches.Count == 0)
            {
                return new ControllerBinding() { Status = ControllerBindingStatus.Missing, BoundController = null, OriginalController = configSerial };
            }


            // Scenario 5: Multiple connected controller match, need user selection
            if (potentialTypeNameMatches.Count > 1)
            {
                return new ControllerBinding() { Status = ControllerBindingStatus.RequiresManualBind, BoundController = null, OriginalController = configSerial };
            }

            // Senario 6: Multiple configs exist in same profile for same type:name, need user selection
            var configsWithTypeNameMatch = uniqueSerials
                .Where(s => GetDeviceIdentifier(s) == deviceTypeName);
            if (configsWithTypeNameMatch.Count() > 1)
            {
                return new ControllerBinding() { Status = ControllerBindingStatus.RequiresManualBind, BoundController = null, OriginalController = configSerial };
            }

            // Scenarios 2, 3: Auto-bind
            // - Scenario 2: Serial differs but device name/type match (single match)
            // - Scenario 3: Name differs but serial matches (single match)
            if (potentialTypeNameMatches.Count == 1 || potentialSerialMatches.Count == 1)
            {
                var autoBindSerial = potentialTypeNameMatches.Count == 1 ? potentialTypeNameMatches.First() : potentialSerialMatches.First();
                return new ControllerBinding() { Status = ControllerBindingStatus.AutoBind, BoundController = autoBindSerial, OriginalController = configSerial };
            }

            // Fallback
            return new ControllerBinding() { Status = ControllerBindingStatus.Missing, BoundController = null, OriginalController = configSerial };
        }

        public static string GetTypeAndName(string fullSerial)
        {
            var parts = fullSerial.Split(new[] { SerialNumber.SerialSeparator }, StringSplitOptions.None);
            return parts.Length > 0 ? parts[0].Trim() : fullSerial;
        }

        internal void ApplyBindingUpdate(List<IConfigItem> configItems, List<ControllerBinding> controllerBindings)
        {
            // Apply the mappings to config items
            foreach (var item in configItems)
            {
                var skipItemBecauseEmpty = string.IsNullOrEmpty(item.ModuleSerial) || item.ModuleSerial == "-";
                if (skipItemBecauseEmpty) continue;

                var mapping = controllerBindings.FirstOrDefault(m => m.OriginalController == item.ModuleSerial);

                if (mapping == null) continue;
                if (mapping.BoundController == null) continue;

                item.ModuleSerial = mapping.BoundController;
            }
        }
    }
}