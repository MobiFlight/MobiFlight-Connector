using MobiFlight.Base;
using System;
using System.ComponentModel;

namespace MobiFlight.HubHop
{
    public class HubHopState : ObservableObject
    {
        DateTime lastUpdate;
        public DateTime LastUpdate
        {
            get => lastUpdate;
            set => SetProperty(ref lastUpdate, value);
        }

        bool shouldUpdate = false;
        public bool ShouldUpdate
        {
            get => shouldUpdate;
            set => SetProperty(ref shouldUpdate, value);
        }

        double updateProgress = 0;
        public double UpdateProgress
        {
            get => updateProgress;
            set => SetProperty(ref updateProgress, value);
        }

        string result = string.Empty;
        public string Result
        {
            get => result;
            set => SetProperty(ref result, value);
        }

        /// <summary>
        /// Lightweight equality used by MainForm to avoid unnecessary replaces.
        /// </summary>
        public bool AreEqual(HubHopState other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;
            return LastUpdate == other.LastUpdate
                && ShouldUpdate == other.ShouldUpdate
                && Math.Abs(UpdateProgress - other.UpdateProgress) < double.Epsilon
                && string.Equals(Result, other.Result, StringComparison.Ordinal);
        }
    }
}
