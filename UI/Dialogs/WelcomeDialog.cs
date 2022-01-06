using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class WelcomeDialog : Form
    {
        public event EventHandler<EventArgs> ReleaseNotesClicked;
        public WelcomeDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void WelcomeDialog_Load(object sender, EventArgs e)
        {

        }

        private void transparentOverlay1_Click(object sender, EventArgs e)
        {
            ReleaseNotesClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
