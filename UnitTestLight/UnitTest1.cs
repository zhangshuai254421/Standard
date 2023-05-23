using LightSourceProgram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialPortManager;
using System;
using System.IO.Ports;

namespace UnitTestLight
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           
            SerialPort serialPort=new SerialPort(ComPortType.COM1.ToString());
            SerialPortManager.SerialPortManager.GetInstance().Initialize (serialPort);
            LightFactoryManager lightFactoryManager = new LightSourceProgram.JLLightFactoryManager();
            // lightFactoryManager=new CSTLightFactoryManager();
            Light JLlight232= lightFactoryManager.Initialize(LightType.RS232);

            //1代表光源控制器的通道1   12代表光源需要改变的值  返回的是发送给光源的指令
            string  SendMsg= JLlight232.CovertTo(1, "12");
            SerialPortManager.SerialPortManager.GetInstance().SendMessage(ComPortType.COM1, SendMsg);
            

            SerialPortManager.SerialPortManager.GetInstance().Dispose();



        }
        [TestMethod]
        public void TestMethod2()
        {
            RessRS232 ressRS232 = new RessRS232();
            string str= ressRS232.CovertTo(1, "12");
        }

        }
    }
