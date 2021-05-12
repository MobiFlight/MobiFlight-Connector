using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobiFlight;

namespace MobiFlight.FSUIPC
{
    public static class FsuipcHelper
    {
        public static ConnectorValue executeRead(IFsuipcConfigItem cfg, FSUIPCCacheInterface fsuipcCache)
        {
            ConnectorValue result = new ConnectorValue();

            if (cfg.OffsetType == FSUIPCOffsetType.String)
            {
                result.type = FSUIPCOffsetType.String;
                result.String = fsuipcCache.getStringValue(cfg.Offset, cfg.Size);
            }
            else if (cfg.OffsetType == FSUIPCOffsetType.Integer)
            {
                result = _executeReadInt(cfg, fsuipcCache);
            }
            else if (cfg.OffsetType == FSUIPCOffsetType.Float)
            {
                result = _executeReadFloat(cfg, fsuipcCache);
            }
            return result;
        }

        private static ConnectorValue _executeReadInt(IFsuipcConfigItem cfg, FSUIPCCacheInterface fsuipcCache)
        {
            ConnectorValue result = new ConnectorValue();
            switch (cfg.Size)
            {
                case 1:
                    Byte value8 = (Byte)(cfg.Mask & fsuipcCache.getValue(
                                                cfg.Offset,
                                                cfg.Size
                                              ));
                    if (cfg.BcdMode)
                    {
                        FsuipcBCD val = new FsuipcBCD() { Value = value8 };
                        value8 = (Byte)val.asBCD;
                    }

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value8;
                    break;
                case 2:
                    Int16 value16 = (Int16)(cfg.Mask & fsuipcCache.getValue(
                                                cfg.Offset,
                                                cfg.Size
                                              ));
                    if (cfg.BcdMode)
                    {
                        FsuipcBCD val = new FsuipcBCD() { Value = value16 };
                        value16 = (Int16)val.asBCD;
                    }

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value16;
                    break;
                case 4:
                    Int64 value32 = ((int)cfg.Mask & fsuipcCache.getValue(
                                                cfg.Offset,
                                                cfg.Size
                                              ));

                    // no bcd support anymore for 4 byte

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value32;
                    break;
                case 8:
                    Double value64 = (Double)fsuipcCache.getDoubleValue(
                                                cfg.Offset,
                                                cfg.Size
                                                );

                    result.type = FSUIPCOffsetType.Float;
                    result.Float64 = (int)(Math.Round(value64, 0));

                    break;
            }
            return result;
        }

        private static ConnectorValue _executeReadFloat(IFsuipcConfigItem cfg, FSUIPCCacheInterface fsuipcCache)
        {
            ConnectorValue result = new ConnectorValue();
            result.type = FSUIPCOffsetType.Float;
            switch (cfg.Size)
            {
                case 4:
                    Double value32 = fsuipcCache.getFloatValue(
                                                cfg.Offset,
                                                cfg.Size
                                              );

                    result.Float64 = value32;
                    break;
                case 8:
                    Double value64 = (Double)fsuipcCache.getDoubleValue(
                                                cfg.Offset,
                                                cfg.Size
                                                );

                    result.Float64 = (int)(Math.Round(value64, 0));

                    break;
            }
            return result;
        }

        public static ConnectorValue executeTransform(ConnectorValue value, IFsuipcConfigItem cfg)
        {
            double tmpValue;
            List<ConfigRefValue> configRefs = new List<ConfigRefValue>();

            switch (value.type)
            {
                case FSUIPCOffsetType.Integer:
                    tmpValue = value.Int64;
                    tmpValue = cfg.Transform.Apply(tmpValue, configRefs);
                    value.Int64 = (Int64)Math.Floor(tmpValue);
                    break;

                /*case FSUIPCOffsetType.UnsignedInt:
                    tmpValue = value.Uint64;
                    tmpValue = tmpValue * cfg.FSUIPCMultiplier;
                    value.Uint64 = (UInt64)Math.Floor(tmpValue);
                    break;*/

                case FSUIPCOffsetType.Float:
                    value.Float64 = Math.Floor(cfg.Transform.Apply(value.Float64, configRefs));
                    break;

                // nothing to do in case of string
            }
            return value;
        }

        public static string executeComparison(ConnectorValue connectorValue, OutputConfigItem cfg)
        {
            string result = null;
            if (connectorValue.type == FSUIPCOffsetType.String)
            {
                return _executeStringComparison(connectorValue, cfg);
            }

            Double value = connectorValue.Int64;
            /*if (connectorValue.type == FSUIPCOffsetType.UnsignedInt) value = connectorValue.Uint64;*/
            if (connectorValue.type == FSUIPCOffsetType.Float) value = connectorValue.Float64;

            if (!cfg.ComparisonActive)
            {
                return value.ToString();
            }

            if (cfg.ComparisonValue == "")
            {
                return value.ToString();
            }

            Double comparisonValue = Double.Parse(cfg.ComparisonValue);
            string comparisonIfValue = cfg.ComparisonIfValue != "" ? cfg.ComparisonIfValue : value.ToString();
            string comparisonElseValue = cfg.ComparisonElseValue != "" ? cfg.ComparisonElseValue : value.ToString();

            switch (cfg.ComparisonOperand)
            {
                case "!=":
                    result = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case ">":
                    result = (value > comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case ">=":
                    result = (value >= comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "<=":
                    result = (value <= comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "<":
                    result = (value < comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "=":
                    result = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                default:
                    result = (value > 0) ? "1" : "0";
                    break;
            }

            // apply ncalc logic
            if (result.Contains("$"))
            {
                result = result.Replace("$", value.ToString());
                var ce = new NCalc.Expression(result);
                try
                {
                    result = (ce.Evaluate()).ToString();
                }
                catch
                {
                    Log.Instance.log("checkPrecondition : Exception on NCalc evaluate", LogSeverity.Warn);
                    throw new Exception(i18n._tr("uiMessageErrorOnParsingExpression"));
                }
            }

            return result;
        }

        private static string _executeStringComparison(ConnectorValue connectorValue, OutputConfigItem cfg)
        {
            string result = connectorValue.String;
            string value = connectorValue.String;

            if (!cfg.ComparisonActive)
            {
                return connectorValue.String;
            }

            string comparisonValue = cfg.ComparisonValue;
            string comparisonIfValue = cfg.ComparisonIfValue;
            string comparisonElseValue = cfg.ComparisonElseValue;

            switch (cfg.ComparisonOperand)
            {
                case "!=":
                    result = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "=":
                    result = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
            }

            return result;
        }

        public static void executeWrite(String value, IFsuipcConfigItem cfg, FSUIPCCacheInterface fsuipcCache)
        {
            if (cfg.OffsetType == FSUIPCOffsetType.String)
            {
                fsuipcCache.setOffset(cfg.Offset, value);
                return;
            }

            if (cfg.Size == 1)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (cfg.BcdMode) format = System.Globalization.NumberStyles.HexNumber;
                byte bValue = Byte.Parse(value, format);
                if (cfg.Mask != 0xFF)
                {
                    byte cByte = (byte)fsuipcCache.getValue(cfg.Offset, cfg.Size);
                    if (bValue == 1)
                    {
                        bValue = (byte)(cByte | cfg.Mask);
                    }
                    else
                    {
                        bValue = (byte)(cByte & ~cfg.Mask);
                    }
                }
                fsuipcCache.setOffset(cfg.Offset, bValue);
            }
            else if (cfg.Size == 2)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (cfg.BcdMode) format = System.Globalization.NumberStyles.HexNumber;
                Int16 sValue = Int16.Parse(value, format);
                if (cfg.Mask != 0xFFFF)
                {
                    Int16 cByte = (Int16)fsuipcCache.getValue(cfg.Offset, cfg.Size);
                    if (sValue == 1)
                    {
                        sValue = (Int16)(cByte | cfg.Mask);
                    }
                    else
                    {
                        sValue = (Int16)(cByte & ~cfg.Mask);
                    }
                }

                fsuipcCache.setOffset(cfg.Offset, sValue);
            }
            else if (cfg.Size == 4)
            {
                Int32 iValue = Int32.Parse(value);
                if (cfg.Mask != 0xFFFFFFFF)
                {
                    Int32 cByte = (Int32)fsuipcCache.getValue(cfg.Offset, cfg.Size);
                    if (iValue == 1)
                    {
                        iValue = (Int32)(cByte | cfg.Mask);
                    }
                    else
                    {
                        iValue = (Int32)(cByte & ~cfg.Mask);
                    }
                }

                fsuipcCache.setOffset(cfg.Offset, iValue);
            }
        }

    }
}
