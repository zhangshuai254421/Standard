using MvCamCtrl.NET.CameraParams;
using System;

namespace JM.CameraModule.IOC
{
    public class QForHIK : IDisposable
    {
        public ImageAll ImageTemp = null;
        public IntPtr pData;
        public MV_FRAME_OUT_INFO_EX pFrameInfo;
        public IntPtr pUser;

        public void Dispose()
        {


            ImageTemp?.Dispose();

        }
    }
}
