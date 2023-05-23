using System;

namespace LightSourceProgram
{
    public class JLRS485Light : Light
    {
        public override string CovertTo(int channel, string value,int id485)
        {
            string value_str = Convert.ToInt32(value).ToString("X");
            if (value_str.Length == 1)
            {
                value_str = "00" + value_str;
            }

            switch (value_str.Length)
            {
                case 1:
                    value_str = "00" + value_str;
                    break;
                case 2:
                    value_str = "0" + value_str;
                    break;
                default:
                    break;
            }
            string ID_485 = id485.ToString("X");
            if (ID_485.Length == 1)
            {
                ID_485 = "0" + ID_485;
            }
            string msg = "$" + ID_485 + "3" + channel + value_str;
            msg = msg + GetXorResualt(msg);
          
            return msg.ToString();

        }
    }
}
