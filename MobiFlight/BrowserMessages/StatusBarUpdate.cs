namespace MobiFlight.BrowserMessages
{
    public class StatusBarUpdate
    {
        public int Value { get; set; }
        public string Text { get; set; }

        public StatusBarUpdate(string text, int value)
        {
            this.Text = text;
            this.Value = value;
        }
    }
}