using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobiFlight;
using MobiFlight.Base;
using MobiFlight.OutputConfig;

namespace MobiFlight.FSUIPC
{
    public static class FsuipcHelper
    {
        public static ConnectorValue executeRead(FsuipcOffset offset, FSUIPCCacheInterface fsuipcCache)
        {
            ConnectorValue result = new ConnectorValue();

            if (offset.OffsetType == FSUIPCOffsetType.String)
            {
                result.type = FSUIPCOffsetType.String;
                result.String = fsuipcCache.getStringValue(offset.Offset, offset.Size);
            }
            else if (offset.OffsetType == FSUIPCOffsetType.Integer)
            {
                result = _executeReadInt(offset, fsuipcCache);
            }
            else if (offset.OffsetType == FSUIPCOffsetType.Float)
            {
                result = _executeReadFloat(offset, fsuipcCache);
            }
            return result;
        }

        private static ConnectorValue _executeReadInt(FsuipcOffset offset, FSUIPCCacheInterface fsuipcCache)
        {
            ConnectorValue result = new ConnectorValue();
            switch (offset.Size)
            {
                case 1:
                    Byte value8 = (Byte)(offset.Mask & fsuipcCache.getValue(
                                                offset.Offset,
                                                    offset.Size
                                              ));
                    if (offset.BcdMode)
                    {
                        FsuipcBCD val = new FsuipcBCD() { Value = value8 };
                        value8 = (Byte)val.asBCD;
                    }

                    result.type = FSUIPCOffsetType.Integer;
                    result.Float64 = value8;
                    break;
                case 2:
                    Int16 value16 = (Int16)(offset.Mask & fsuipcCache.getValue(
                                                offset.Offset,
                                                offset.Size
                                              ));
                    if (offset.BcdMode)
                    {
                        FsuipcBCD val = new FsuipcBCD() { Value = value16 };
                        value16 = (Int16)val.asBCD;
                    }

                    result.type = FSUIPCOffsetType.Integer;
                    result.Float64 = value16;
                    break;
                case 4:
                    Int64 value32 = ((int)offset.Mask & fsuipcCache.getValue(
                                                offset.Offset,
                                                offset.Size
                                              ));

                    // no bcd support anymore for 4 byte

                    result.type = FSUIPCOffsetType.Integer;
                    result.Float64 = value32;
                    break;
                case 8:
                    Double value64 = (Double)fsuipcCache.getDoubleValue(
                                                offset.Offset,
                                                offset.Size
                                                );

                    result.type = FSUIPCOffsetType.Float;
                    result.Float64 = (int)(Math.Round(value64, 0));

                    break;
            }
            return result;
        }

        private static ConnectorValue _executeReadFloat(FsuipcOffset offset, FSUIPCCacheInterface fsuipcCache)
        {
            ConnectorValue result = new ConnectorValue();
            result.type = FSUIPCOffsetType.Float;
            switch (offset.Size)
            {
                case 4:
                    Double value32 = fsuipcCache.getFloatValue(
                                                offset.Offset,
                                                offset.Size
                                              );

                    result.Float64 = value32;
                    break;

                case 8:
                    Double value64 = fsuipcCache.getDoubleValue(
                                                offset.Offset,
                                                offset.Size
                                                );

                    result.Float64 = value64;
                    break;
            }
            return result;
        }


        public static void executeWrite(String value, IFsuipcConfigItem cfg, FSUIPCCacheInterface fsuipcCache)
        {
            if (cfg.FSUIPC.OffsetType == FSUIPCOffsetType.String)
            {
                fsuipcCache.setOffset(cfg.FSUIPC.Offset, value);
                return;
            }

            if (cfg.FSUIPC.Size == 1)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (cfg.FSUIPC.BcdMode) format = System.Globalization.NumberStyles.HexNumber;
                byte bValue = Byte.Parse(value, format);
                if (cfg.FSUIPC.Mask != 0xFF)
                {
                    byte cByte = (byte)fsuipcCache.getValue(cfg.FSUIPC.Offset, cfg.FSUIPC.Size);
                    if (bValue == 1)
                    {
                        bValue = (byte)(cByte | cfg.FSUIPC.Mask);
                    }
                    else
                    {
                        bValue = (byte)(cByte & ~cfg.FSUIPC.Mask);
                    }
                }
                fsuipcCache.setOffset(cfg.FSUIPC.Offset, bValue);
            }
            else if (cfg.FSUIPC.Size == 2)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (cfg.FSUIPC.BcdMode) format = System.Globalization.NumberStyles.HexNumber;
                Int16 sValue = Int16.Parse(value, format);
                if (cfg.FSUIPC.Mask != 0xFFFF)
                {
                    Int16 cByte = (Int16)fsuipcCache.getValue(cfg.FSUIPC.Offset, cfg.FSUIPC.Size);
                    if (sValue == 1)
                    {
                        sValue = (Int16)(cByte | cfg.FSUIPC.Mask);
                    }
                    else
                    {
                        sValue = (Int16)(cByte & ~cfg.FSUIPC.Mask);
                    }
                }

                fsuipcCache.setOffset(cfg.FSUIPC.Offset, sValue);
            }
            else if (cfg.FSUIPC.Size == 4)
            {
                Int32 iValue = Int32.Parse(value);
                if (cfg.FSUIPC.Mask != 0xFFFFFFFF)
                {
                    Int32 cByte = (Int32)fsuipcCache.getValue(cfg.FSUIPC.Offset, cfg.FSUIPC.Size);
                    if (iValue == 1)
                    {
                        iValue = (Int32)(cByte | cfg.FSUIPC.Mask);
                    }
                    else
                    {
                        iValue = (Int32)(cByte & ~cfg.FSUIPC.Mask);
                    }
                }

                fsuipcCache.setOffset(cfg.FSUIPC.Offset, iValue);
            }
        }

    }
}
