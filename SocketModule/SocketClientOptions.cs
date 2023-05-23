using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace SocketModule
{
    public class SocketClientOptions
    {
        public const string SocketClient01 = "SocketClient01";
        public const string SocketClient02 = "SocketClient02";
        public const string SocketClient03 = "SocketClient03";
        public const string SocketClient04 = "SocketClient04";

        public string IpAddress { get; set; }
        public int IpPort { get; set; }

        public  List<SocketClientDataVariable> SocketClientDataVariable { get; set; }

    }
    public class SocketClientDataVariable
    {
        public string Name { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }

      
        public ValueType ValueType { get; set; }
        [System.ComponentModel.TypeConverter(typeof(VariableAccessTypeEnumTypeConverter))]
        public VariableAccessTypeEnum VariableAccessTypeEnum { get; set; }

        public SocketClientDataVariable Clone()
        {
            return new SocketClientDataVariable
            {
                Name = Name,
                Start = Start,
                Length = Length,
                Value = Value,
                Description = Description,
                ValueType = ValueType.Clone(),
                VariableAccessTypeEnum = VariableAccessTypeEnum
            };
        }
    }
   
    public enum VariableAccessTypeEnum  
    {
        Readable, Writable
    }
    [System.ComponentModel.TypeConverter(typeof(ValueTypeTypeConverter))]
    public class ValueType
    {
        public Type Type { get; set; }
        public ValueType Clone()
        {
            return new ValueType { Type = Type };
        }
    }
   
    public class VariableAccessTypeEnumTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return System.Enum.Parse(typeof(VariableAccessTypeEnum), value.ToString());
        }
    }
    public class ValueTypeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
          
       
            return new ValueType() { Type = System.Type.GetType("System.String") };
        }
    }
}
