using System;
using System.ComponentModel;
using System.Globalization;

namespace JM.CameraModule.IOC
{
    [System.ComponentModel.TypeConverter(typeof(AcquisitionModeEnumTypeConverter))]
    /// <summary>
    /// 采集模式SingleFrame, Continuous
    /// </summary>
    public enum AcquisitionModeEnum
    {
        Single,
        Mutli,
        Continuous
    }
    public class AcquisitionModeEnumTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return System.Enum.Parse(typeof(AcquisitionModeEnum), value.ToString());
        }
    }
}
