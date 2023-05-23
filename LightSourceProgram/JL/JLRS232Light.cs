using System;

namespace LightSourceProgram
{
    public class JLRS232Light : Light
    {
        public override string CovertTo(int channel, string str, int id485 = 0)
        {
            string value_str = Convert.ToInt32(str).ToString("X");
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
            string msg = "$3" + channel + value_str;
            msg = msg + GetXorResualt(msg);
            return msg;
        }
    }
}
