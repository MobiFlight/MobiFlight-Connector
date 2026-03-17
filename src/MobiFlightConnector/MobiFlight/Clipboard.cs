using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public sealed class Clipboard : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler     PropertyChanged;

        private InputAction inputAction;
        private OutputConfigItem outputConfigItem;
        private String outputConfigName;
        private InputConfigItem inputConfigItem;
        private String inputConfigName;
        private bool outputConfigActive;
        private bool inputConfigActive;

        private static readonly Clipboard clipboard = new Clipboard();

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Clipboard() { }

        public static Clipboard Instance
        {
            get { return clipboard; }
        }

        public InputAction InputAction { 
            get { return inputAction; }
            set {
                if (value != this.inputAction)
                {
                    this.inputAction = value;
                    NotifyPropertyChanged();
                }
            } 
        }
        public OutputConfigItem OutputConfigItem
        {
            get { return outputConfigItem; }
            set
            {
                if (value != this.outputConfigItem)
                {
                    this.outputConfigItem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public String OutputConfigName
        {
            get { return outputConfigName; }
            set
            {
                if (value != this.outputConfigName)
                {
                    this.outputConfigName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public InputConfigItem InputConfigItem
        {
            get { return inputConfigItem; }
            set
            {
                if (value != this.inputConfigItem)
                {
                    this.inputConfigItem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public String InputConfigName
        {
            get { return inputConfigName; }
            set
            {
                if (value != this.inputConfigName)
                {
                    this.inputConfigName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool InputConfigActive
        {
            get { return inputConfigActive; }
            set
            {
                if (value != this.inputConfigActive)
                {
                    this.inputConfigActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool OutputConfigActive
        {
            get { return outputConfigActive; }
            set
            {
                if (value != this.outputConfigActive)
                {
                    this.outputConfigActive = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
