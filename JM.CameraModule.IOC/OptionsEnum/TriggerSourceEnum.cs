
using System;
using System.ComponentModel;
using System.Globalization;

namespace JM.CameraModule.IOC
{

    /// <summary>
    /// 触发源      Line0=0,
    //Line1=1,
    //Line2=2,
    //Line3=3,
    //Counter0=4,
    //Software=7,
    //FrequencyConverter=8
    /// </summary>
    [System.ComponentModel.TypeConverter(typeof(TriggerSourceEnumTypeConverter))]
    public enum TriggerSourceEnum
    {
        Line0=0,
        Line1=1,
        Line2=2,
        Line3=3,
        Counter0=4,
        Software=7,
        FrequencyConverter=8
    }
    public class TriggerSourceEnumTypeConverter : TypeConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return System.Enum.Parse(typeof(TriggerSourceEnum),value.ToString());
        }
    }

}
