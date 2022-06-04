using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.InputConfig
{
    public class InputActionFactory
    {
        static public InputAction CreateFromXmlReader(XmlReader reader)
        {
            InputAction action = null;
            action = CreateByType(reader["type"]);

            if (action == null) return action;

            action.ReadXml(reader);

            return action;
        }

        static public InputAction CreateByType(string type)
        {
            InputAction action = null;
            switch (type)
            {
                case FsuipcOffsetInputAction.TYPE:
                    action = new FsuipcOffsetInputAction();
                    break;

                case KeyInputAction.TYPE:
                    action = new KeyInputAction();
                    break;

                case EventIdInputAction.TYPE:
                    action = new EventIdInputAction();
                    break;

                case PmdgEventIdInputAction.TYPE:
                    action = new PmdgEventIdInputAction();
                    break;

                case JeehellInputAction.TYPE:
                    action = new JeehellInputAction();
                    break;

                case LuaMacroInputAction.TYPE:
                    action = new LuaMacroInputAction();
                    break;

                case RetriggerInputAction.TYPE:
                    action = new RetriggerInputAction();
                    break;

                case VJoyInputAction.TYPE:
                    action = new VJoyInputAction();
                    break;

                case MSFS2020EventIdInputAction.TYPE:
                    action = new MSFS2020EventIdInputAction();
                    break;

                case VariableInputAction.TYPE:
                    action = new VariableInputAction();
                    break;

                case MSFS2020CustomInputAction.TYPE:
                    action = new MSFS2020CustomInputAction();
                    break;
                case XplaneInputAction.TYPE:
                    action = new XplaneInputAction();
                    break;
            }

            return action;
        }
    }
}
