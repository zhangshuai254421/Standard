using JM.LogModule.IOC;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JM.CameraModule.IOC
{
    public partial class HKAreaCamera
    {

#region 自身定义的字段
        public Action<ActionTriggerSource_enum, ImageAll> ImageAllCamera_event;
        int nRet = CErrorDefine.MV_OK;

        //设备打开状态
        bool m_bIsDeviceOpen = false;
        //网络中枚举的设备信息
        List<CCameraInfo> ltDeviceList = new List<CCameraInfo>();
        #endregion
        /// <summary>
        /// 相机控制类
        /// </summary>
        public CCamera CCamera = new CCamera();
        PixelFormat m_enBitmapPixelFormat = PixelFormat.DontCare;
        /// <summary>
        /// 相机信息
        /// </summary>
        public CCameraInfo CCameraInfo = new CCameraInfo();
        /// <summary>
        /// 枚举参数信息
        /// </summary>
        CEnumValue cEnumValue = new CEnumValue();
        CEnumEntry cEnumEntry = new CEnumEntry();
        public bool IsOpen { get; set; }
        /// <summary>
        /// 图像采集的回调委托
        /// </summary>
        cbOutputExdelegate ImageCallback;
        ConcurrentQueue<QForHIK> concurrentQueue = new ConcurrentQueue<QForHIK>();

        public HKAreaCameraOptions HKAreaCameraOptions { get; set; }
        private Log Log { get; set; }
        public HKAreaCamera(HKAreaCameraOptions hKAreaCameraOptions , Log log)
        {
            Log = log;
            HKAreaCameraOptions = hKAreaCameraOptions;
        }

        


        public bool ConnectCamera(string SerialNumber)
        {
            try
            {


                //枚举网络上的设备
                FindCameraDevice();
                //显示-选择-打开-相机设备
                ShowAndChooseAndOpenCameraDevice(SerialNumber);
                //探测网络包最佳大小
                GetOptimalPacketSize();
                //获取像素格式
                GetPixelFormat();
                //设置相机的采集模式
                SetAcquisitionMode();
                //设置相机的触发模式
                SetTriggerMode();
                //触发模式打开设置触发源才又意义
                SetTriggerSource();
                //注册取图回调函数
                RegisterImageCallback();
                //设置的配置保存到通道1
                SavaParCameraChannel();
                //加载配置
                LoadParCameraChannel();
                //开始采集前的准备工作
                NecessaryOperBeforeGrab();
                //开始采集图片
                StartAcquisition();
                if (nRet != CErrorDefine.MV_OK)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                return false;
            }
        }

        private void StartAcquisition()
        {
            nRet = CCamera.StartGrabbing();
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"StartGrabbing failed:{nRet:x8}");
            }
        }
        static int ImageKey = new Random().Next();
        IntPtr intPtr = Marshal.AllocHGlobal(ImageKey);
        private void RegisterImageCallback()
        {
            if (HKAreaCameraOptions.triggerModeEnum == TriggerModeEnum.On)
            {
                ImageCallback = new cbOutputExdelegate(Crab_CallBack);
                CCamera.RegisterImageCallBackEx(ImageCallback, intPtr);

            }
        }
        public void TriggerSoftware()
        {

            nRet = CCamera.SetCommandValue("TriggerSoftware");
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"TriggerSoftware failed:{nRet:x8}");
            }
        }
        public  void TriggerSoftware(int key)
        {
            Marshal.WriteInt32(intPtr, key);
            TriggerSoftware();
        }

        private void GetPixelFormat()
        {
            nRet = CCamera.GetEnumValue("PixelFormat", ref cEnumValue);
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Get PixelFormat failed:{nRet:x8}");
            }
            else
            {
                cEnumEntry.Value = cEnumValue.CurValue;
                nRet = CCamera.GetEnumEntrySymbolic("PixelFormat", ref cEnumEntry);
                if (CErrorDefine.MV_OK != nRet)
                {
                    Log.Info($"GetEnumEntrySymbolic PixelFormt failed:{nRet:x8}");

                }
                else
                {
                    Log.Info($"相机图像格式:{cEnumEntry.Symbolic}");
                }
            }
        }

        private void GetOptimalPacketSize()
        {
            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
            if (CCameraInfo.nTLayerType == CSystem.MV_GIGE_DEVICE)
            {
                int nPacketSize = CCamera.GIGE_GetOptimalPacketSize();
                if (nPacketSize > 0)
                {
                    nRet = CCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        Log.Warn($"Warning: Set Packet Size failed:{nRet:x8}");
                    }
                }
                else
                {
                    Log.Warn($"Warning: Get Packet Size failed:{nRet:x8}");
                }
            }
        }

        private void ShowAndChooseAndOpenCameraDevice(string SerialNumber)
        {
            for (int i = 0; i < ltDeviceList.Count; i++)
            {
                if (CSystem.MV_GIGE_DEVICE == ltDeviceList[i].nTLayerType)
                {
                    CGigECameraInfo cGigEDeviceInfo = (CGigECameraInfo)ltDeviceList[i];

                    uint nIp1 = (cGigEDeviceInfo.nCurrentIp & 0xff000000) >> 24;
                    uint nIp2 = (cGigEDeviceInfo.nCurrentIp & 0x00ff0000) >> 16;
                    uint nIp3 = (cGigEDeviceInfo.nCurrentIp & 0x0000ff00) >> 8;
                    uint nIp4 = cGigEDeviceInfo.nCurrentIp & 0x000000ff;
                    Log.Info($"Device{i.ToString()}: DevIP:{nIp1}.{nIp2}.{nIp3}.{nIp4}");
                    if ("" != cGigEDeviceInfo.UserDefinedName)
                    {
                        Log.Info($"UserDefineName:{cGigEDeviceInfo.UserDefinedName}");
                    }
                    else
                    {
                        Log.Info($"chUserDefinedName:{cGigEDeviceInfo.chUserDefinedName}");
                    }
                    if (SerialNumber == cGigEDeviceInfo.chSerialNumber)
                    {
                        CCameraInfo = ltDeviceList[i];
                        //创建设备 
                        nRet = CCamera.CreateHandle(ref CCameraInfo);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Log.Warn($"Create Device failed:{nRet:x8}");
                            break;
                        }
                        nRet = CCamera.OpenDevice();
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Log.Warn($"Open device failed:{nRet:x8}");
                            break;
                        }
                        m_bIsDeviceOpen = true;
                    }

                }
                else if (CSystem.MV_USB_DEVICE == ltDeviceList[i].nTLayerType)
                {
                    CUSBCameraInfo cUsb3DeviceInfo = (CUSBCameraInfo)ltDeviceList[i];
                    Log.Info($"device :{cUsb3DeviceInfo.chSerialNumber}");
                    if ("" != cUsb3DeviceInfo.UserDefinedName)
                    {
                        Log.Info($"UserDefineName:{cUsb3DeviceInfo.UserDefinedName}");
                    }
                    else
                    {
                        Log.Info($"chUserDefinedName:{cUsb3DeviceInfo.chUserDefinedName}");
                    }
                    if (SerialNumber == cUsb3DeviceInfo.chSerialNumber)
                    {
                        CCameraInfo = ltDeviceList[i];
                        //创建设备 
                        nRet = CCamera.CreateHandle(ref CCameraInfo);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Log.Warn($"Create Device failed:{nRet:x8}");
                            break;
                        }
                        //打开设备

                        nRet = CCamera.OpenDevice();
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Log.Warn ($"Open device failed:{nRet:x8}");
                            break;
                        }
                        m_bIsDeviceOpen = true;
                    }
                }
            }
        }

        private bool FindCameraDevice()
        {
            nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE | CSystem.MV_USB_DEVICE, ref ltDeviceList);
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Enum device failed:{nRet:X8}");
            }
            Log.Info($"Enum device count:{ltDeviceList.Count}");
            if (0 == ltDeviceList.Count)
            {
                return false;
            }
            return true;
        }

        private int NecessaryOperBeforeGrab()
        {
            // ch:取图像宽 | en:Get Iamge Width
            CIntValue pcWidth = new CIntValue();
            int nRet = CCamera.GetIntValue("Width", ref pcWidth);
            if (CErrorDefine.MV_OK != nRet)
            {

                Log.Warn($"Get Width Info Fail!{nRet:x8}");
                return nRet;
            }
            // ch:取图像高 | en:Get Iamge Height
            CIntValue pcHeight = new CIntValue();
            nRet = CCamera.GetIntValue("Height", ref pcHeight);
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Get Height Info Fail!{nRet:x8}");
                return nRet;
            }
            // ch:取像素格式 | en:Get Pixel Format
            CEnumValue pcPixelFormat = new CEnumValue();
            nRet = CCamera.GetEnumValue("PixelFormat", ref pcPixelFormat);
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Get Pixel Format Info Fail!{nRet:x8}");
                return nRet;
            }

            // ch:设置bitmap像素格式
            if ((int)MvGvspPixelType.PixelType_Gvsp_Undefined == (int)pcPixelFormat.CurValue)
            {

                Log.Warn($"Get Pixel Format Info Fail!{CErrorDefine.MV_E_UNKNOW:x8}");
                return CErrorDefine.MV_E_UNKNOW;
            }
            else if (IsMonoData((MvGvspPixelType)pcPixelFormat.CurValue))
            {
                m_enBitmapPixelFormat = PixelFormat.Format8bppIndexed;
            }
            else
            {
                m_enBitmapPixelFormat = PixelFormat.Format24bppRgb;
            }
            bitmap = new Bitmap((Int32)pcWidth.CurValue, (Int32)pcHeight.CurValue, m_enBitmapPixelFormat);
            // ch:Mono8格式，设置为标准调色板 | en:Set Standard Palette in Mono8 Format
            if (PixelFormat.Format8bppIndexed == m_enBitmapPixelFormat)
            {
                ColorPalette palette = bitmap.Palette;
                for (int i = 0; i < palette.Entries.Length; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                bitmap.Palette = palette;
            }

            return CErrorDefine.MV_OK;
        }
        Bitmap bitmap = null;

        private int SavaParCameraChannel(int nRet = 0)
        {


            nRet = CCamera.SetEnumValue("UserSetSelector", 1);
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Set UserSetSelector failed:{nRet:x8}");
            }
            nRet = CCamera.SetCommandValue("UserSetSave");
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Set UserSetSave failed:{nRet:x8}");
            }
            nRet = CCamera.SetEnumValue("UserSetDefault", 1);
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Set UserSetDefault failed:{nRet:x8}");
            }

            return nRet;
        }
        private int LoadParCameraChannel(int nRet = 0)
        {


            nRet = CCamera.SetEnumValue("UserSetSelector", 1);
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Set UserSetSelector failed:{nRet:x8}");
            }
            nRet = CCamera.SetCommandValue("UserSetLoad");
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Set UserSetLoad failed:{nRet:x8}");
            }

            return nRet;
        }
        //ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser);
        private void Crab_CallBack(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            if (TriggerModeEnum.On == HKAreaCameraOptions.triggerModeEnum)
            {

                MV_FRAME_OUT_INFO_EX pFrameInfoTemp = pFrameInfo;

                //这里把回调转成bitmap格式---


                QForHIK qForHIK = new QForHIK
                {

                    pData = pData,
                    pFrameInfo = pFrameInfo,
                    pUser = pUser
                };
                if (concurrentQueue.Count > 30)
                {
                    Log.Info($"采图队列已满，队列里未处理图片数量:{concurrentQueue.Count}");
                    concurrentQueue.TryDequeue(out QForHIK result);
                    QForHIK qForHIKTemp = result as QForHIK;
                    qForHIKTemp.Dispose();
                }
                concurrentQueue.Enqueue(qForHIK);
#if DEBUG
                Log.Info($"采图队列收到软触发key:{Marshal.ReadInt32(pUser)}");
                //  concurrentQueue.TryDequeue(out QForHIK result1);
#endif

            }
        }

        private void SetTriggerSource()
        {
            if (TriggerModeEnum.On == HKAreaCameraOptions.triggerModeEnum)
            {
                nRet = CCamera.SetEnumValue("TriggerSource", (uint)HKAreaCameraOptions.triggerSourceEnum);
                if (CErrorDefine.MV_OK != nRet)
                {
                    Log.Info($"Set TriggerSource failed:{nRet:x8}");
                }
            }

        }

        private void SetTriggerMode()
        {
            nRet = CCamera.SetEnumValue("TriggerMode", (uint)HKAreaCameraOptions.triggerModeEnum);
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Set TriggerMode failed:{nRet:x8}");

            }
        }

        private void SetAcquisitionMode()
        {
            nRet = CCamera.SetEnumValue("AcquisitionMode", (uint)HKAreaCameraOptions.acquisitionModeEnum);
            //判断返回结果是否正确
            if (CErrorDefine.MV_OK != nRet)
            {
                Log.Warn($"Set AcquisitionMode failed:{nRet:x8}");
            }
        }


        public  bool OpenCamera()
        {
            if (!ConnectCamera(HKAreaCameraOptions.SN))
            {
                IsOpen = false;
                return false;
            }
            IsOpen = true;
            new Task(MonitorHIKCallback).Start();
            return true;
        }
        public void MonitorHIKCallback()
        {

            while (IsOpen)
            {

                Thread.Sleep(5);
                QForHIK qForHIKTemp = new QForHIK();
                try
                {
                    bool result = concurrentQueue.TryDequeue(out qForHIKTemp);
                    if (result == false)
                    {
                        continue;
                    }
                    IntPtr pData = qForHIKTemp.pData;
                    MV_FRAME_OUT_INFO_EX pFrameInfo = qForHIKTemp.pFrameInfo;


                    ConvertBitmap(pData, pFrameInfo);


                    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, (int)pcConvertParam.InImage.Width, (int)pcConvertParam.InImage.Height)
            , ImageLockMode.ReadWrite, bitmap.PixelFormat);
                    Marshal.Copy(pcConvertParam.OutImage.ImageData, 0, bitmapData.Scan0, (Int32)pcConvertParam.OutImage.ImageData.Length);
                    bitmap.UnlockBits(bitmapData);






                    ImageAll imageAllTemp = new ImageAll { Bit = (Bitmap)bitmap.Clone(), ImageKey = Marshal.ReadInt32(qForHIKTemp.pUser) };

                    if (ImageAllCamera_event != null)
                    {
                        ImageAllCamera_event(ActionTriggerSource_enum.Camera, imageAllTemp);
                    }

                }

                catch (Exception ex)
                {

                    throw;
                }
                finally
                {
                    qForHIKTemp?.Dispose();
                }



            }
        }
        CPixelConvertParam pcConvertParam = new CPixelConvertParam();
        private void ConvertBitmap(IntPtr pData, MV_FRAME_OUT_INFO_EX pFrameInfo)
        {

#if DEBUG
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
#endif
            MvGvspPixelType pixelFormat = MvGvspPixelType.PixelType_Gvsp_Undefined;
            int nChannelNum = 0;
            if (IsColorData(pFrameInfo.enPixelType))
            {
                pixelFormat = MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;

                bitmap = new Bitmap((int)pFrameInfo.nWidth, (int)pFrameInfo.nHeight, PixelFormat.Format24bppRgb);
            }
            else if (IsMonoData(pFrameInfo.enPixelType))
            {
                pixelFormat = MvGvspPixelType.PixelType_Gvsp_Mono8;

            }
            else
            {
                Log.Warn("Don't need to convert!");
            }

            if (pixelFormat != MvGvspPixelType.PixelType_Gvsp_Undefined)
            {
                pcConvertParam.InImage.ImageAddr = pData;
                pcConvertParam.InImage.Width = pFrameInfo.nWidth;
                pcConvertParam.InImage.Height = pFrameInfo.nHeight;
                pcConvertParam.InImage.FrameLen = pFrameInfo.nFrameLen;
                pcConvertParam.InImage.PixelType = pFrameInfo.enPixelType;
                pcConvertParam.InImage.ImageSize = (uint)(pFrameInfo.nWidth * pFrameInfo.nHeight * nChannelNum);
                pcConvertParam.OutImage.PixelType = pixelFormat;


                nRet = CCamera.ConvertPixelType(ref pcConvertParam);
                if (CErrorDefine.MV_OK != nRet)
                {
                    Log.Warn($"ConvertPixelType failed!{nRet:x8}");
                }
                //bitmap = new Bitmap((int)pFrameInfo.nWidth, (int)pFrameInfo.nHeight, (int)pFrameInfo.nWidth, PixelFormat.Format8bppIndexed, pcConvertParam.OutImage.ImageAddr);
                //ColorPalette cp = bitmap.Palette;
                //for (int i = 0; i < 256; i++)
                //{
                //    cp.Entries[i] = Color.FromArgb(i, i, i);
                //}
                //bitmap.Palette = cp;

                //bitmap.Save($"E:\\04_开发项目\\02_VisionApp\\01BasicClass\\Standard\\Log单元测试\\bin\\Debug\\log\\HKCameraLog\\{new Random().Next()}.bmp");


#if DEBUG

                stopwatch.Stop();
                Log.Info($"转换图片耗时：{stopwatch.ElapsedMilliseconds}毫秒");
#endif
                //   GC.Collect();
            }


        }
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr FVObject);
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        #region 海康的公共方法
        /// <summary>
        /// 判断是否是黑白格式
        /// </summary>
        /// <param name="enGvspPixelType"></param>
        /// <returns></returns>
        protected bool IsMonoData(MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MvGvspPixelType.PixelType_Gvsp_Mono1p:
                case MvGvspPixelType.PixelType_Gvsp_Mono2p:
                case MvGvspPixelType.PixelType_Gvsp_Mono4p:
                case MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MvGvspPixelType.PixelType_Gvsp_Mono8_Signed:
                case MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                case MvGvspPixelType.PixelType_Gvsp_Mono14:
                case MvGvspPixelType.PixelType_Gvsp_Mono16:
                    return true;

                default:
                    return false;
            }
        }

        /************************************************************************
         *  @fn     IsColorData()
         *  @brief  判断是否是彩色数据
         *  @param  enGvspPixelType         [IN]           像素格式
         *  @return 成功，返回0；错误，返回-1 
         ************************************************************************/
        protected bool IsColorData(MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                    return true;

                default:
                    return false;
            }
        }
        #endregion
        /// <summary>
        /// ch:显示错误信息 | en:Show error message
        /// </summary>
        /// <param name="csMessage"></param>
        /// <param name="nErrorNum"></param>
        /// <returns></returns>
        private string ShowErrorMsg(string csMessage, int nErrorNum)
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + string.Format("{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case CErrorDefine.MV_E_HANDLE: errorMsg += " Error or invalid handle "; break;
                case CErrorDefine.MV_E_SUPPORT: errorMsg += " Not supported function "; break;
                case CErrorDefine.MV_E_BUFOVER: errorMsg += " Cache is full "; break;
                case CErrorDefine.MV_E_CALLORDER: errorMsg += " Function calling order error "; break;
                case CErrorDefine.MV_E_PARAMETER: errorMsg += " Incorrect parameter "; break;
                case CErrorDefine.MV_E_RESOURCE: errorMsg += " Applying resource failed "; break;
                case CErrorDefine.MV_E_NODATA: errorMsg += " No data "; break;
                case CErrorDefine.MV_E_PRECONDITION: errorMsg += " Precondition error, or running environment changed "; break;
                case CErrorDefine.MV_E_VERSION: errorMsg += " Version mismatches "; break;
                case CErrorDefine.MV_E_NOENOUGH_BUF: errorMsg += " Insufficient memory "; break;
                case CErrorDefine.MV_E_UNKNOW: errorMsg += " Unknown error "; break;
                case CErrorDefine.MV_E_GC_GENERIC: errorMsg += " General error "; break;
                case CErrorDefine.MV_E_GC_ACCESS: errorMsg += " Node accessing condition error "; break;
                case CErrorDefine.MV_E_ACCESS_DENIED: errorMsg += " No permission "; break;
                case CErrorDefine.MV_E_BUSY: errorMsg += " Device is busy, or network disconnected "; break;
                case CErrorDefine.MV_E_NETER: errorMsg += " Network error "; break;
            }
            return errorMsg;
        }

        public  bool CloseCameraDevice()
        {
            try
            {

                CCamera.StopGrabbing();
                CCamera.CloseDevice();
                CCamera.DestroyHandle();

            }
            catch (Exception ex)
            {
                Log.Warn($"{ex.Message.ToString()}");
                return false;
            }
            return true;

        }
    }
}
