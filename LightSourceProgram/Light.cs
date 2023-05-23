using System.Collections.Generic;
using System.Text;

namespace LightSourceProgram
{
    public abstract class Light
    {
        
        public Light() { }
        public abstract string CovertTo(int channel, string str, int id485=0);
        public string GetXorResualt(string msg)
        {
            //获取字节数组
            byte[] b = Encoding.ASCII.GetBytes(msg);
            // xorResult 存放校验结注意：初值首元素值
            byte xorResult = b[0];
            // 求xor校验注意：XOR运算第二元素始
            for (int i = 1; i < b.Length; i++)
            {
                xorResult ^= b[i];
            }
            // 运算xorResultXOR校验结，^=为异或符号
            // MessageBox.Show();
            return xorResult.ToString("X");
        }
    }
}
