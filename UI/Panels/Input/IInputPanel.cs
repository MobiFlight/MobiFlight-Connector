using System;
using System.Collections.Generic;

namespace MobiFlight.UI.Panels.Input
{
    public interface IInputPanel
    {
        void Init(IExecutionManager executionManager);
        void SetVariableReferences(Dictionary<String, MobiFlightVariable> variables);
    }
} 