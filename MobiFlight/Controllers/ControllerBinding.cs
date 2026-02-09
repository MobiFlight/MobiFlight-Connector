using MobiFlight.Base;

namespace MobiFlight.Controllers
{
    public class ControllerBinding
    {
        public Controller BoundController { get; set; }
        public ControllerBindingStatus Status { get; set; }
        public Controller OriginalController { get; set; }
    }
}
