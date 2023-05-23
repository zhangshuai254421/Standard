using System;
using System.Drawing;

namespace JM.CameraModule.IOC
{
    public class ImageAll : IDisposable
    {
        private string name;


        public ImageAll()
        {

        }
        public string Name { get => name; set => name = value; }
        public Bitmap Bit { get; set; }

        public void Dispose()
        {
            // bitmap?.Dispose();
            Bit?.Dispose();
            // Log.WriteLogInfo($"图片名称：{name},被接口Dispose释放！");
        }
        public int ImageKey { get; set; }
        // private Bitmap bitmap;
    }
}
