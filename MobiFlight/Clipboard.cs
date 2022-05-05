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
    }
}
