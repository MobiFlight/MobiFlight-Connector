namespace MobiFlight
{
    public interface KeyboardInputInterface
    {
        void SendKeyAsInput(System.Windows.Forms.Keys Key, bool Control, bool Alt, bool Shift);
    }
}