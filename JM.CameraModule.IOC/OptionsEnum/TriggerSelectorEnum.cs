using System;
using System.ComponentModel;
using System.Globalization;

namespace JM.CameraModule.IOC
{
    [TypeConverter(typeof(TriggerSourceEnumTypeConverter))]
    /// <summary>
    ///  AcquisitionStart=0, AcquisitionEnd=1, AcquisitionActive=2,FrameStart=3, FrameEnd=4,FrameActive=5, FrameBurstStart=6, FrameBurstEnd=7, FrameBurstActive=8,LineStart=9, ExposureStart=10, ExposureEnd=11, ExposureActive=12
    /// </summary>
    public enum TriggerSelectorEnum
    {
        AcquisitionStart=0, AcquisitionEnd=1, AcquisitionActive=2,FrameStart=3, FrameEnd=4,FrameActive=5, FrameBurstStart=6, FrameBurstEnd=7, FrameBurstActive=8,LineStart=9, ExposureStart=10, ExposureEnd=11, ExposureActive=12
    }
    public class TriggerSelectorEnumTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(TriggerSelectorEnum);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return System.Enum.Parse(typeof(TriggerSelectorEnum), value.ToString());
        }
    }
}
