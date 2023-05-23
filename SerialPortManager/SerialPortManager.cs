using System;
using System.Collections.Generic;
using System.Text;

using System.IO.Ports;
using System.Collections;
using System.Linq.Expressions;

namespace SerialPortManager
{
    public  class SerialPortManager
    {
        private static SerialPortManager S_P;
        private static readonly object locker=new object();
       
        private  Hashtable hashtable = new Hashtable();
        private SerialPortManager() {
        
        }

        public  Hashtable Hashtable { get => hashtable; set => hashtable = value; }

        public static SerialPortManager GetInstance()
        {
            if (S_P == null)
            {
                lock (locker)
                {
                    if (S_P == null)
                    {
                        S_P = new SerialPortManager();
                    }
                }
            }
            return S_P;
        }
        public  bool Initialize (SerialPort serialPort)
        {
            try
            {
                if (hashtable.ContainsKey(serialPort.PortName))
                {
                    return true;
                }
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
                hashtable.Add(serialPort.PortName, serialPort);
                return true;
            }
            catch (Exception ex)
            {
               return false;
            } 
        }
        public void SendMessage(ComPortType type,string str)
        {
            
           SerialPort serialPort=  hashtable[type.ToString()] as SerialPort;
           if (!serialPort.IsOpen)
           {
                serialPort.Open();
           }
           serialPort.Write(str);
        }
        public void SendMessage(ComPortType type, byte[] bytes)
        {
            try
            {
                SerialPort serialPort = hashtable[type.ToString()] as SerialPort;
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
                serialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public void Dispose()
        {
            if (hashtable != null)
            {
                hashtable.Clear();
            }
        }

    }


}
