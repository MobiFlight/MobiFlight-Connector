using MobiFlight.MQTT;
using System;
using System.Security;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MqttServerSettingsPanel : UserControl
    {
        private MQTTServerSettings settings;
        private bool passwordChanged = false;

        public MqttServerSettingsPanel()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            settings = MQTTServerSettings.Load();

            addressTextBox.Text = settings.Address;
            encryptConectionCheckbox.Checked = settings.EncryptConnection;
            passwordTextBox.Text = "****";
            portTextBox.Text = settings.Port.ToString();
            usernameTextBox.Text = settings.Username;
            validateCertificateCheckbox.Checked = settings.ValidateCertificate;
            validateCertificateCheckbox.Enabled = encryptConectionCheckbox.Checked;

            // After setting the password text box to a placeholder value register for TextChanged events
            // so we can track whether the user changed the password and it needs to be saved after.
            this.passwordTextBox.TextChanged += new System.EventHandler(this.passwordTextBox_TextChanged);
        }

        public void SaveSettings()
        {
            settings.Address = addressTextBox.Text;
            settings.EncryptConnection = encryptConectionCheckbox.Checked;
            settings.Port = Convert.ToInt32(settings.Port);
            settings.Username = usernameTextBox.Text;
            settings.ValidateCertificate = validateCertificateCheckbox.Checked;

            if (passwordChanged)
            {
                var securePassword = new SecureString();
                Array.ForEach(passwordTextBox.Text.ToCharArray(), securePassword.AppendChar);
                passwordTextBox.Text = ""; // Clear the password from the UI as soon as possible
                settings.SetPassword(securePassword);
            }

            settings.Save();
        }

        //Only allow numbers in the port text box.
        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            passwordChanged = true;
        }

        private void encryptConectionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            validateCertificateCheckbox.Enabled = encryptConectionCheckbox.Checked;
        }
    }
}