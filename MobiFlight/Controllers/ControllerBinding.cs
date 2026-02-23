using MobiFlight.Base;

namespace MobiFlight.Controllers
{
    public class ControllerBinding
    {
        public Controller BoundController { get; set; }
        public ControllerBindingStatus Status { get; set; }
        public Controller OriginalController { get; set; }

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
