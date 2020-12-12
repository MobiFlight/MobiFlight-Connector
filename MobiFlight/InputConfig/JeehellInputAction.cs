using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class JeehellInputAction : InputAction, ICloneable
    {
        const Int16 BaseOffset = 0x73CC;
        const Int16 ParamOffset = 0x73CD;

        const Int16 Offset_SPD = 0x73C1;
        const Int16 Offset_HDG = 0x73C3;
        const Int16 Offset_ALT = 0x73C5;
        const Int16 Offset_VS  = 0x73C7;
        const Int16 Offset_CPT_QNH = 0x73C8;
        const Int16 Offset_FO_QNH = 0x73CA;

        public const String TYPE = "JeehellInputAction";

        public Byte EventId;
        public String Param;
        
        override public object Clone()
        {
            JeehellInputAction clone = new JeehellInputAction();
            clone.EventId = EventId;
            clone.Param = Param;

            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            String eventId = reader["pipeId"];
            String param = reader["value"];

            EventId = Byte.Parse(eventId);
            Param = param;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("pipeId", EventId.ToString());
            writer.WriteAttributeString("value", Param.ToString());
        }

        private IFsuipcConfigItem CreateJeehellBaseOffsetConfigItem()
        {
            FSUIPC.FSUIPCConfigItem result = new FSUIPC.FSUIPCConfigItem();
            result.FSUIPCOffset = BaseOffset;
            result.FSUIPCOffsetType = FSUIPCOffsetType.Integer;
            result.FSUIPCSize = 1;
            result.FSUIPCMask = 0xFF;
            result.FSUIPCBcdMode = false;
            
            return result;
        }

        private IFsuipcConfigItem CreateFsuipcConfigItem ()
        {
            FSUIPC.FSUIPCConfigItem result = new FSUIPC.FSUIPCConfigItem();
            result.FSUIPCOffsetType = FSUIPCOffsetType.Integer;
            result.FSUIPCSize = 2;
            result.FSUIPCMask = 0xFFFF;
            result.FSUIPCBcdMode = false;

            switch (EventId)
            {
                case 1:
                    result.FSUIPCOffset = Offset_SPD;
                    break;

                case 2:
                    result.FSUIPCOffset = Offset_HDG;
                    break;

                case 3:
                    result.FSUIPCOffset = Offset_ALT;
                    //result.FSUIPCMultiplier = 100;
                    break;
                case 4:
                    result.FSUIPCOffset = Offset_VS;
                    result.FSUIPCSize = 1;
                    result.FSUIPCMask = 0xFF;
                    //result.FSUIPCMultiplier = 100;
                    break;
                case 5:
                    result.FSUIPCOffset = Offset_CPT_QNH;
                    break;
                case 6:
                    result.FSUIPCOffset = Offset_FO_QNH;
                    break;

                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    break;
            }
            /*
            0 None
            1 SPD / Mach => 2
            2 HDG/TRK => 2
            3 ALT => 2
            4 VS(format is VS in ft/min) / FPA (format is FPA*10)  => 1
            5 CPT QNH  => 2
            6 FO QNH  => 2
            7 LDG ELEV => 2?
            8 Cockpit TEMP => 2?
            9 FWD Cabin TEMP => 2?
            10 AFT cabin TEMP => 2?
            11 FWD Cargo TEMP => 2?
            12 AFT Cargo TEMP => 2?
            13 Trim wheel Threshold degrees*100. Default is 100 (equals to 1°
            out of trim command) => 2?
            */

            return result;
        }

        public override void execute(FSUIPC.FSUIPCCacheInterface cache, SimConnectMSFS.SimConnectCacheInterface simConnectCache, MobiFlightCacheInterface moduleCache)
        {
            String value = Param;
            IFsuipcConfigItem cfg = CreateFsuipcConfigItem();

            if (value.Contains("$"))
            {
                ConnectorValue tmpValue = FSUIPC.FsuipcHelper.executeRead(cfg, cache as FSUIPC.FSUIPCCacheInterface);
                tmpValue = FSUIPC.FsuipcHelper.executeTransform(tmpValue, cfg);

                Log.Instance.log("JeehellInputAction:Execute : Current value " + tmpValue.Int64.ToString(), LogSeverity.Debug);

                String expression = value.Replace("$", tmpValue.Int64.ToString());
                var ce = new NCalc.Expression(expression);
                try
                {
                    value = (ce.Evaluate()).ToString();
                }
                catch
                {
                    Log.Instance.log("checkPrecondition : Exception on NCalc evaluate", LogSeverity.Warn);
                    throw new Exception(i18n._tr("uiMessageErrorOnParsingExpression"));
                }
            }

            cfg.FSUIPCOffset = ParamOffset;
            cfg.FSUIPCSize = 2;
            cfg.FSUIPCMask = 0xFFFF;

            Log.Instance.log("JeehellInputAction:Execute : Setting value " + value + " for EventID: " + EventId, LogSeverity.Debug);
            FSUIPC.FsuipcHelper.executeWrite(value, cfg, cache as FSUIPC.FSUIPCCacheInterface);
            FSUIPC.FsuipcHelper.executeWrite(EventId.ToString(), CreateJeehellBaseOffsetConfigItem(), cache as FSUIPC.FSUIPCCacheInterface);
            cache.Write();
        }
    }
}
