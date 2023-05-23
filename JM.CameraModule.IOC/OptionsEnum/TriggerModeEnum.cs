using System;
using System.ComponentModel;
using System.Globalization;

namespace JM.CameraModule.IOC
{
    [System.ComponentModel.TypeConverter(typeof(TriggerModeEnumTypeConverter))]
    /// <summary>
    /// 触发模式 On,Off
    /// </summary>
    public enum TriggerModeEnum
    {
        Off,On
    }

    public class TriggerModeEnumTypeConverter : TypeConverter
    {

       
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
        
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {         
            return System.Enum.Parse(typeof(TriggerModeEnum), value.ToString());

        }
    }
}
