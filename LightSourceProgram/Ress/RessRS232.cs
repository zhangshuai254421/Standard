using System;

namespace LightSourceProgram
{
    public class RessRS232:RSLight {
        public override string CovertTo(int channel, string str, int id485 = 0)
        {
          
            string value_str = Convert.ToInt32(str).ToString("0000");
            value_str =$"S{((channelType)channel).ToString()}{value_str}#";            
            return value_str; 
        }
        public enum channelType
        {
            Empty,A,B,C,D,E,F,G
        }

    }
}
