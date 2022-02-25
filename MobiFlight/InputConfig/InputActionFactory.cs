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

            switch (reader["type"])
            {
                case FsuipcOffsetInputAction.TYPE:
                    action = new FsuipcOffsetInputAction();
                    action.ReadXml(reader);
                    reader.Read(); // this should be the closing tag "onChange"
                    break;

                case KeyInputAction.TYPE:
                    action = new KeyInputAction();
                    action.ReadXml(reader);
                    break;

                case EventIdInputAction.TYPE:
                    action = new EventIdInputAction();
                    action.ReadXml(reader);
                    break;

                case PmdgEventIdInputAction.TYPE:
                    action = new PmdgEventIdInputAction();
                    action.ReadXml(reader);
                    break;

                case JeehellInputAction.TYPE:
                    action = new JeehellInputAction();
                    action.ReadXml(reader);
                    break;

                case LuaMacroInputAction.TYPE:
                    action = new LuaMacroInputAction();
                    action.ReadXml(reader);
                    break;

                case RetriggerInputAction.TYPE:
                    action = new RetriggerInputAction();
                    action.ReadXml(reader);
                    break;

                case VJoyInputAction.TYPE:
                    action = new VJoyInputAction();
                    action.ReadXml(reader);
                    break;

                case MSFS2020EventIdInputAction.TYPE:
                    action = new MSFS2020EventIdInputAction();
                    action.ReadXml(reader);
                    break;

                case VariableInputAction.TYPE:
                    action = new VariableInputAction();
                    action.ReadXml(reader);
                    break;

                case MSFS2020CustomInputAction.TYPE:
                    action = new MSFS2020CustomInputAction();
                    action.ReadXml(reader);
                    break;
            }

            return action;
        }
    }
}
