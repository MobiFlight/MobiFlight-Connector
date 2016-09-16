using MobiFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlightUnitTests.mock.Base
{
    class KeyboardInputMock : KeyboardInputInterface
    {
        public List<String> Writes = new List<String>();

        public void SendKeyAsInput(Keys Key, bool Control, bool Alt, bool Shift)
        {
            String write = "";
            if (Control) write += "Ctrl+";
            if (Shift) write += "Shift+";
            if (Alt) write += "Alt+";
            write += Key.ToString();

            Writes.Add(write);
        }
    }
}
