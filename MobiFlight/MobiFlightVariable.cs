namespace MobiFlight
{
    public class MobiFlightVariable
    {
        public string TYPE = "number";
        public string Name = "MyVar";
        public double Number;
        public string Text = "";
        public string Expression = "$";

        public object Clone()
        {
            MobiFlightVariable clone = new MobiFlightVariable();
            clone.TYPE = TYPE;
            clone.Name = Name;
            clone.Number = Number;
            clone.Text = Text;
            clone.Expression = Expression;

            return clone;
        }
    }
}