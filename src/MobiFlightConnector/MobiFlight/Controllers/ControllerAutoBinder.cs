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
        private readonly List<Controller> _connectedControllers;
        private readonly Dictionary<string, int> _controllerTypeCount = new Dictionary<string, int>();

        public ControllerAutoBinder(List<Controller> connectedControllers)
        {
            _connectedControllers = connectedControllers ?? new List<Controller>();

            // Count controllers by type:name
            foreach (var controller in _connectedControllers)
            {
                var deviceIdentifier = GetDeviceIdentifier(controller);

                if (!_controllerTypeCount.ContainsKey(deviceIdentifier))
                    _controllerTypeCount[deviceIdentifier] = 0;

                _controllerTypeCount[deviceIdentifier]++;
            }
        }

        /// <summary>
        /// Gets a unique key combining device name and serial prefix for matching
        /// </summary>
        private static string GetDeviceIdentifier(Controller controller)
        {
            var deviceName = controller?.Name;
            var deviceSerialPrefix = SerialNumber.ExtractPrefix(controller?.Serial);
            return $"{deviceSerialPrefix}:{deviceName}";
        }

        /// <summary>
        /// Analyzes binding status for all config items without modifying them
        /// Returns a dictionary mapping original serial -> binding status
        /// </summary>
        public List<ControllerBinding> AnalyzeBindings(List<IConfigItem> configItems, List<ControllerBinding> existingBindings)
        {
            var results = new List<ControllerBinding>();
            var availableControllers = new List<Controller>(_connectedControllers);

            var uniqueControllers = configItems
                .Where(c => !SkipInvalidController(c))
                .Select(c => c.Controller)
                .Distinct()
                .OrderByDescending(uniqueController => availableControllers.Any(ac => ac.AreEqual(uniqueController)) || (existingBindings?.FirstOrDefault(b => b.OriginalController.AreEqual(uniqueController)) != null))
                .ToList();

            foreach (var controller in uniqueControllers)
            {
                // Check if this serial was already bound in a previous config file
                var previouslyBoundController = existingBindings?.FirstOrDefault(b => b.OriginalController.AreEqual(controller));
                var alreadyBoundInPreviousConfigFile = previouslyBoundController != null;
                if (alreadyBoundInPreviousConfigFile)
                {
                    // Check if the previously bound controller is still available
                    if (!availableControllers.Contains(previouslyBoundController.BoundController)) continue;

                    // Reuse the same binding
                    var previousStatus = previouslyBoundController.BoundController.AreEqual(controller)
                        ? ControllerBindingStatus.Match
                        : ControllerBindingStatus.AutoBind;

                    results.Add(new ControllerBinding()
                    {
                        Status = previousStatus,
                        BoundController = previouslyBoundController.BoundController,
                        OriginalController = controller
                    });

                    availableControllers.Remove(previouslyBoundController.BoundController);
                    continue;
                }

                var controllerBinding = AnalyzeSingleBinding(controller, uniqueControllers, availableControllers);

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

        private static bool SkipInvalidController(IConfigItem configItem)
        {
            return configItem.Controller == null || configItem.Controller.Serial == SerialNumber.NOT_SET;
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
                if (item.Controller == null)
                    continue;

                var mapping = serialMappings.FirstOrDefault(m => m.OriginalController.AreEqual(item.Controller));
                if (mapping == null) continue;

                item.Controller = mapping.BoundController;
            }

            return serialMappings.ToList();
        }

        private ControllerBinding AnalyzeSingleBinding(Controller controller, List<Controller> uniqueControllers, List<Controller> availableControllers)
        {
            // Scenario 1: Exact match
            if (availableControllers.Contains(controller))
            {
                return new ControllerBinding() { Status = ControllerBindingStatus.Match, BoundController = controller, OriginalController = controller };
            }

            var controllerTypeName = GetDeviceIdentifier(controller);
            var potentialTypeNameMatches = availableControllers
                .Where(c => GetDeviceIdentifier(c) == controllerTypeName)
                .ToList();

            var controllerSerial = controller?.Serial;
            var potentialControllerMatches = availableControllers
                .Where(c => c.Serial == controllerSerial)
                .ToList();

            // Scenario 4: Missing
            if (potentialTypeNameMatches.Count == 0 && potentialControllerMatches.Count == 0)
            {
                return new ControllerBinding() { Status = ControllerBindingStatus.Missing, BoundController = null, OriginalController = controller };
            }


            // Scenario 5: Multiple connected controller match, need user selection
            if (potentialTypeNameMatches.Count > 1)
            {
                return new ControllerBinding() { Status = ControllerBindingStatus.RequiresManualBind, BoundController = null, OriginalController = controller };
            }

            // Senario 6: Multiple configs exist in same profile for same type:name, need user selection
            var configsWithTypeNameMatch = uniqueControllers
                .Where(s => GetDeviceIdentifier(s) == controllerTypeName);
            if (configsWithTypeNameMatch.Count() > 1)
            {
                return new ControllerBinding() { Status = ControllerBindingStatus.RequiresManualBind, BoundController = null, OriginalController = controller };
            }

            // Scenarios 2, 3: Auto-bind
            // - Scenario 2: Serial differs but device name/type match (single match)
            // - Scenario 3: Name differs but serial matches (single match)
            if (potentialTypeNameMatches.Count == 1 || potentialControllerMatches.Count == 1)
            {
                var autoBindSerial = potentialTypeNameMatches.Count == 1 ? potentialTypeNameMatches.First() : potentialControllerMatches.First();
                return new ControllerBinding() { Status = ControllerBindingStatus.AutoBind, BoundController = autoBindSerial, OriginalController = controller };
            }

            // Fallback
            return new ControllerBinding() { Status = ControllerBindingStatus.Missing, BoundController = null, OriginalController = controller };
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
                var skipItemBecauseEmpty = SkipInvalidController(item);
                if (skipItemBecauseEmpty) continue;

                var mapping = controllerBindings.FirstOrDefault(m => m.OriginalController.AreEqual(item.Controller));

                if (mapping == null) continue;
                if (mapping.BoundController == null) continue;

                item.Controller = mapping.BoundController;
            }
        }
    }
}