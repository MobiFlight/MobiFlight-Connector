using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.About
{
    public partial class LicenseReferenceControl : UserControl
    {
        [Description("The name of the Libary"), Category("Data")]
        public string Library
        {
            get => LibraryLabel.Text;
            set => LibraryLabel.Text = value;
        }

        private String licenseLink;

        [Description("The url to the license"), Category("Data")]
        public String LicenseLink
        {
            get { return licenseLink; }
            set { licenseLink = value; }
        }

        private String libraryLink;

        [Description("The url to the library"), Category("Data")]
        public String LibraryLink
        {
            get { return libraryLink; }
            set { libraryLink = value; }
        }


        public LicenseReferenceControl()
        {
            InitializeComponent();
            LicenseLinkLabel.Click += (s, e) => { LicenseLinkLabel.LinkVisited = true; System.Diagnostics.Process.Start(LicenseLink); };
            LibraryLinkLabel.Click += (s, e) => { LibraryLinkLabel.LinkVisited = true; System.Diagnostics.Process.Start(LibraryLink); };
        }
    }
}
