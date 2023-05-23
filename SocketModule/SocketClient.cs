using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using JM.LogModule.IOC;
using System.Linq;

namespace SocketModule
{
    public  class SocketClient
    {
        private TcpClient _tcpClient;
        private bool _Connected = false;
        private SocketClientOptions SocketClientOptions { get; set; }
        private Log Log { get; set; }
        private bool IsReceive = false;
        private byte[] RBuffers = new byte[1024];
        private byte[] WBuffers = new byte[1024];
        private byte[] ReceiveData = null;
        public Action<List<SocketClientDataVariable>> VariableValueChangedAction;
        private List<SocketClientDataVariable> SocketVariable { get { return SocketClientOptions.SocketClientDataVariable; } }
        public SocketClient(SocketClientOptions socketClientOptions, Log log)
        {
            SocketClientOptions = socketClientOptions;
            this.Log = log;
            
        }
        public void Initialize()
        {
            Thread thread = new Thread(Process);
            thread.Name = "Socket循环线程";
            thread.IsBackground = true;
            thread.Start();
        }
        public void Process()
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
            }
            int maxSetupConnectionTryCount = 5;//最大尝试连接次数 
           
            while (true)
            {
                if (_Connected)
                {
                    LoopPlcData();
                    Thread.Sleep(1000);
                }
                else
                {
                   
                    _tcpClient.Connect(IPAddress.Parse(SocketClientOptions.IpAddress), SocketClientOptions.IpPort);//链接socket服务端
                    _Connected= _tcpClient.Client.Poll(1000, SelectMode.SelectWrite);//测试是否可连接
                    if (_Connected)
                    {
                        maxSetupConnectionTryCount = 0;
                        _Connected = true;
                        Thread.Sleep(10);
                    }
                    else
                    {
                        maxSetupConnectionTryCount--;
                        if (maxSetupConnectionTryCount == 0)
                        {
                            Log.Warn("无法建立连接：已超过最大尝试连接次数！");
                            _Connected = false;
                            break;
                        }
                    }
                }
            }
        }
        public void LoopPlcData()
        {
            List<SocketClientDataVariable> changedVars = new List<SocketClientDataVariable>();
            
            _tcpClient.Client.BeginReceive(RBuffers, 0, RBuffers.Length, SocketFlags.None, new AsyncCallback(AsyBackGetMessage), _tcpClient.Client);

            if (IsReceive)
            {
                foreach (var item in SocketVariable)
                {
                    if (item.VariableAccessTypeEnum == VariableAccessTypeEnum.Readable)
                    {
                        object val = null;
                        switch (item.ValueType.Type.ToString())
                        {
                            case "System.String":
                                val = GetStringAt(item.Start, item.Length).Trim();
                                break;
                            default:
                                break;
                        }
                        if (!val.Equals(item.Value))
                        {
                            item.Value = val;
                            changedVars.Add(item);
                        }
                    }
                }
                IsReceive = false;
            }
            if (changedVars.Count > 0)
            {
                List<SocketClientDataVariable> socketDataVariables = new List<SocketClientDataVariable>();
                SocketVariable.ForEach(p => socketDataVariables.Add(p.Clone()));
                VariableValueChangedAction?.Invoke(socketDataVariables);
            }
            foreach (var item in SocketVariable)
            {
                if (item.VariableAccessTypeEnum == VariableAccessTypeEnum.Writable)
                {
                    switch (item.ValueType.ToString())
                    {
                        case "System.String":
                            SetStringAt(item.Start, Convert.ToString(item.Value));
                            break;
                        default:
                            break;
                    }
                }
            }
            if (ReceiveData == null)
            {
                ReceiveData = (byte[])WBuffers.Clone();
            }

            if (!ReceiveData.SequenceEqual(WBuffers))//比较发送给plc的值有没有改变
            {
                ReceiveData = (byte[])WBuffers.Clone();
                int reslut = _tcpClient.Client.Send(WBuffers);
                if (reslut == 0)
                {
                    Log.Warn("Socket发动数据失败！");
                }
            }
        }   
        public void SetStringAt(int start, string value)
        {
            Encoding.ASCII.GetBytes(value,0, value.Length, WBuffers, start * 2);
        }
        private string GetStringAt(int start, int length)
        {         
            string tempstr = Encoding.ASCII.GetString(RBuffers, 0, RBuffers.Length);
            return tempstr.Substring(start, length);
        }

        private void AsyBackGetMessage(IAsyncResult iar)
        {
            Socket client = iar.AsyncState as Socket;
            string ipStr = client.RemoteEndPoint.ToString();
            try
            {
                int length = client.EndReceive(iar);//结束异步读取
                //将套接字获取到的字符数组转换为人可以看懂的字符窜
                string str = Encoding.Default.GetString(RBuffers, 0, length);
                //接收下一个消息(因为这是一个递归的调用，所以这样就可以一直接收消息了）   
                IsReceive = true;
                _tcpClient.Client.BeginReceive(RBuffers, 0, RBuffers.Length, SocketFlags.None, new AsyncCallback(AsyBackGetMessage), client);
            }
            catch (Exception)
            {

            }
        }

    }
}
