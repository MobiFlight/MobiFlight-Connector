using MobiFlight.Base;

namespace MobiFlight.Controllers
{
    public class ControllerBinding
    {
        public Controller BoundController { get; set; }
        public ControllerBindingStatus Status { get; set; }
        public Controller OriginalController { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (BoundController?.GetHashCode() ?? 0);
                hash = hash * 23 + Status.GetHashCode();
                hash = hash * 23 + (OriginalController?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = obj as ControllerBinding;

            return BoundController.AreEqual(other.BoundController) &&
                   Status == other.Status &&
                   OriginalController.AreEqual(other.OriginalController);
        }
    }
}
