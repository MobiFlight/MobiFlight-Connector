using MobiFlight.Base;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Controllers
{
    /// <summary>
    /// High-level service for controller binding operations
    /// </summary>
    public class ControllerBindingService
    {
        private readonly IExecutionManager _executionManager;

        public ControllerBindingService(IExecutionManager executionManager)
        {
            _executionManager = executionManager;
        }

        /// <summary>
        /// Analyzes binding status WITHOUT modifying the project
        /// Returns: list of ControllerBindings for the entire project
        /// </summary>
        public List<ControllerBinding> AnalyzeProjectBindings(Project project)
        {
            var connectedControllers = GetAllConnectedControllers();
            var binder = new ControllerAutoBinder(connectedControllers);

            var allResults = new List<ControllerBinding>();
            var appliedBindingMappings = new List<ControllerBinding>();

            foreach (var configFile in project.ConfigFiles)
            {
                var results = binder.AnalyzeBindings(configFile.ConfigItems, appliedBindingMappings);
                results.ForEach(b =>
                {
                    // Only add if not already present (first occurrence wins)
                    if (!allResults.Any(existing => existing.OriginalController == b.OriginalController))
                    {
                        allResults.Add(b);
                    }
                });
            }

            return allResults;
        }

        /// <summary>
        /// Performs auto-binding and modifies config items
        /// Returns: Dictionary mapping ModuleSerial -> ControllerBindingStatus
        /// </summary>
        public List<ControllerBinding> PerformAutoBinding(Project project)
        {
            var connectedControllers = GetAllConnectedControllers();
            var binder = new ControllerAutoBinder(connectedControllers);

            var allResults = new List<ControllerBinding>();
            var appliedBindingMappings = new List<ControllerBinding>();

            foreach (var configFile in project.ConfigFiles)
            {
                var results = binder.AnalyzeBindings(configFile.ConfigItems, appliedBindingMappings);
                var serialMappings = binder.ApplyAutoBinding(configFile.ConfigItems, results);

                foreach (var binding in results)
                {
                    var bindingExists = allResults.FirstOrDefault(b => b.OriginalController == binding.OriginalController);
                    // Only add if not already present (first occurrence wins)
                    if (bindingExists != null) continue;
                    
                    allResults.Add(binding);
                }

                // Update binding mappings for next config file
                foreach (var mapping in serialMappings)
                {
                    if (appliedBindingMappings.FirstOrDefault(b => b.BoundController == mapping.BoundController) != null) continue;

                    appliedBindingMappings.Add(mapping);
                }
            }

            project.ControllerBindings = allResults;

            return allResults;
        }

        internal void UpdateControllerBindings(Project project, List<ControllerBinding> bindings)
        {
            var connectedControllers = GetAllConnectedControllers();
            var binder = new ControllerAutoBinder(connectedControllers);

            foreach (var configFile in project.ConfigFiles)
            {
                foreach (var binding in bindings)
                {
                    binder.ApplyBindingUpdate(configFile.ConfigItems, new List<ControllerBinding> { binding });
                }
            }

            project.ControllerBindings = bindings;
        }

        private List<string> GetAllConnectedControllers()
        {
            var serials = new List<string>();

            foreach (var module in _executionManager.getMobiFlightModuleCache().GetModules())
            {
                serials.Add($"{module.Name}{SerialNumber.SerialSeparator}{module.Serial}");
            }

            foreach (var joystick in _executionManager.GetJoystickManager().GetJoysticks())
            {
                serials.Add($"{joystick.Name} {SerialNumber.SerialSeparator}{joystick.Serial}");
            }

            foreach (var midiBoard in _executionManager.GetMidiBoardManager().GetMidiBoards())
            {
                serials.Add($"{midiBoard.Name} {SerialNumber.SerialSeparator}{midiBoard.Serial}");
            }

            return serials;
        }
    }
}