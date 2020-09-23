using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;

namespace game
{
    public class CAN
    {
        //2.定义CAN信息帧的数据类型。
        unsafe public struct VCI_CAN_OBJ  //使用不安全代码
        {
            public uint ID;
            public uint TimeStamp;        //时间标识
            public byte TimeFlag;         //是否使用时间标识
            public byte SendType;         //发送标志。保留，未用
            public byte RemoteFlag;       //是否是远程帧
            public byte ExternFlag;       //是否是扩展帧
            public byte DataLen;          //数据长度
            public fixed byte Data[8];    //数据
            public fixed byte Reserved[3];//保留位

        }

        //3.定义初始化CAN的数据类型
        public struct VCI_INIT_CONFIG
        {
            public UInt32 AccCode;      //验证码
            public UInt32 AccMask;      //屏蔽码
            public UInt32 Reserved;
            public byte Filter;   //0或1接收所有帧。2标准帧滤波，3是扩展帧滤波。
            public byte Timing0;  //波特率参数，具体配置，请查看二次开发库函数说明书。
            public byte Timing1;
            public byte Mode;     //模式，0表示正常模式，1表示只听模式,2自测模式
        }
        /*------------兼容ZLG的函数描述---------------------------------*/
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);


        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);

        /*------------其他函数描述---------------------------------*/

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ConnectDevice(UInt32 DevType, UInt32 DevIndex);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_UsbDeviceReset(UInt32 DevType, UInt32 DevIndex, UInt32 Reserved);

        /*------------函数描述结束---------------------------------*/

        public static UInt32 d_Type = 4;//USBCAN2
        UInt32 m_bOpen = 0;
        UInt32 m_devind = 0;
        UInt32 m_canind = 0;

        public static int[] soc = new int[1];
        public static byte[] recflag = new byte[6];
        public static int[] vb = new int[17];
        public static int[] tem = new int[2];
        public static int[] cur = new int[1];
        public static int[] soh = new int[1];

        byte[] candata = new byte[8];
        VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[100];
        //VCI_CAN_OBJ[] m_Sendobj = new VCI_CAN_OBJ[100];
        //private string[] m_recString = new string[1000];

        //public delegate void CanRecviveDataHandler(VCI_CAN_OBJ msg);//委托 申明delegate的对象

        //public CanRecviveDataHandler canRecvData;           //？

        public delegate void CanSendDataHandler(uint id, byte[] data);//委托 申明delegate的对象

        public CanSendDataHandler canSendData;

        public delegate void CanRecviveDataHandler(uint id, byte[] msg);//委托接收

        public CanRecviveDataHandler canRecvData;


        public delegate void HandleReciveCanDataDelegate(string msg,int num);//委托 申明delegate的对象

        public HandleReciveCanDataDelegate disPlayCanData;


        System.Threading.Timer recTimer;

        private static bool isRunTimer = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CAN()
        {

            if (m_bOpen == 0)
            {
                ////打开CAN 分析仪
                //if (VCI_OpenDevice(m_devtype, 0, 0) == 0)
                //{
                //    MessageBox.Show("打开设备失败,请检查设备类型和设备索引号是否正确");
                //}
                //else
                //{
                //    m_bOpen = 1;
                //}
            }

        }

        /// <summary>
        /// //我 发送数据
        /// </summary>
        /// <param name="data"></param>
        unsafe public void SendCANData(byte[] data)
        {
            Byte i = 0;
            VCI_CAN_OBJ m_Sendobj = new VCI_CAN_OBJ();
            m_Sendobj.ID = 0x00010005;
            for (i = 0; i < 8; i++)
                m_Sendobj.Data[i] = data[i];
            m_Sendobj.DataLen = 8;
            VCI_Transmit(d_Type, m_devind, m_canind, ref m_Sendobj, 8);
        }
        
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="obj"></param>
        unsafe private void ReadCANData(object obj)
        {
            byte[] rdata = new byte[8];
            //byte* data;//我//
            UInt32 res = new UInt32();
            UInt32 id;
            
            res = VCI_Receive(d_Type, m_devind, m_canind, ref m_recobj[0], 1000, 100);
            if (!isRunTimer)
            {
                recTimer.Dispose();
            }
            else
            {
                for (UInt32 i = 0; i < res; i++)
                {
                    id = m_recobj[i].ID;

                    if (m_recobj[i].RemoteFlag == 0)
                    {
                        byte len = (byte)(m_recobj[i].DataLen % 9);
                        fixed (VCI_CAN_OBJ* m_recobj1 = &m_recobj[i])
                        {
                            if ((id & 0x00ffff00) == 0xf21200)//拿can数据和0x00ffff00作&
                            {
                                soc[0] = m_recobj1->Data[0];
                                vb[0] =  m_recobj1->Data[1] * 256 + m_recobj1->Data[2];
                                cur[0] = (m_recobj1->Data[3] * 256 + m_recobj1->Data[4]);
                                soh[0]= m_recobj1->Data[7];//我加的
                            }
                            if ((id & 0x00ffff00) == 0xf21400)
                            {



                            }
                            if ((id & 0x00ffff00) == 0x241700)
                            {
                                vb[1] = m_recobj1->Data[1] * 256 + m_recobj1->Data[2];
                                vb[2] = m_recobj1->Data[3] * 256 + m_recobj1->Data[4];
                                vb[3] = m_recobj1->Data[5] * 256 + m_recobj1->Data[6];
                            }
                            if ((id & 0x00ffff00) == 0x251700)
                            {
                                vb[4] = m_recobj1->Data[1] * 256 + m_recobj1->Data[2];
                                vb[5] = m_recobj1->Data[3] * 256 + m_recobj1->Data[4];
                                vb[6] = m_recobj1->Data[5] * 256 + m_recobj1->Data[6];
                            }
                            if ((id & 0x00ffff00) == 0x261700)
                            {

                                vb[7] = m_recobj1->Data[1] * 256 + m_recobj1->Data[2];
                                vb[8] = m_recobj1->Data[3] * 256 + m_recobj1->Data[4];
                                vb[9] = m_recobj1->Data[5] * 256 + m_recobj1->Data[6];

                            }
                            if ((id & 0x00ffff00) == 0x271700)
                            {
                                vb[10] = m_recobj1->Data[1] * 256 + m_recobj1->Data[2];
                                vb[11] = m_recobj1->Data[3] * 256 + m_recobj1->Data[4];
                                vb[12] = m_recobj1->Data[5] * 256 + m_recobj1->Data[6];
                            }
                            if ((id & 0x00ffff00) == 0x281700)
                            {
                                vb[13] = m_recobj1->Data[1] * 256 + m_recobj1->Data[2];
                                vb[14] = m_recobj1->Data[3] * 256 + m_recobj1->Data[4];
                                vb[15] = m_recobj1->Data[5] * 256 + m_recobj1->Data[6];
                            }
                            if ((id & 0x00ffff00) == 0x291700)
                            {
                                vb[16] = m_recobj1->Data[1] * 256 + m_recobj1->Data[2];

                            }
                            if ((id & 0x00ffff00) == 0x4f1700)
                            {
                                tem[0] = m_recobj1->Data[1];
                                tem[1] = m_recobj1->Data[2];

                            }

                            disPlayCanData(System.Convert.ToString(System.Convert.ToInt32(soh[0]) * 0.4), 25);//我加的

                            for (int s = 0;s<16;s++)
                            {
                                disPlayCanData(System.Convert.ToString(System.Convert.ToInt32(vb[s+1 ] ) * 0.001) + "V", (s + 9));

                            }

                            for (int k = 0; k < 4; k++)
                            {
                                disPlayCanData(System.Convert.ToString(System.Convert.ToInt32(vb[k*4+1] + vb[k * 4 + 2] + vb[k * 4 + 3] + vb[k * 4 + 4])*0.001)+"V", (k+1));
                            }
                            

                            disPlayCanData(System.Convert.ToString(System.Convert.ToInt32(vb[0])*0.02)+"V",0 );
                            disPlayCanData(System.Convert.ToString(System.Convert.ToInt32(cur[0])*0.1 -3200)+"A", 5);
                            disPlayCanData(System.Convert.ToString(System.Convert.ToInt32(soc[0])*0.4),6);
                            disPlayCanData(System.Convert.ToString(tem[0]-40), 7);
                            disPlayCanData(System.Convert.ToString(tem[1]-40), 8);
                 
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// //我 打开usbcan
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            int m_bOpen = Convert.ToInt16(VCI_OpenDevice(d_Type, 0, 0));//解决can通讯问题
            if (m_bOpen == 1)                //1表示高频，表示打开
            {
                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();   //can数据 
                config.AccCode = 0x00000000;
                config.AccMask = 0xFFFFFFFF;   //接收所有ID
                switch (2)//(MainWindow.baudrate)
                {
                    case 0:
                        config.Timing0 = 0x03;//125K ,波特率
                        config.Timing1 = 0x1c;
                        break;
                    case 1:
                        config.Timing0 = 0x01;//250K ,波特率
                        config.Timing1 = 0x1c;
                        break;
                    case 2:
                        config.Timing0 = 0x00;//500K ,波特率
                        config.Timing1 = 0x1c;
                        break;
                    default:
                        break;
                }
                config.Filter = 1; //滤波模式，接收所有类型
                config.Mode = 0;  //正常工作模式
                if (VCI_InitCAN(d_Type, m_devind, m_canind, ref config) != 1)
                {
                    VCI_CloseDevice(d_Type, 0);
                    m_bOpen = 0;
                    return false;
                }
                //开始CAN设备
                VCI_StartCAN(d_Type, m_devind, m_canind);

                isRunTimer = true;
                recTimer = new Timer(new TimerCallback(ReadCANData), null, 0, 100);  //1s调用ReadCANData函数
            }
            else
            {
                //打开CAN 分析仪
                if (VCI_OpenDevice(d_Type, 0, 0) == 0)
                {
                    MessageBox.Show("打开设备失败,请检查设备类型和设备索引号是否正确");
                }
                else
                {
                    m_bOpen = 1;
                }
            }
            return true;
        }

        public void Stop()
        {
            //recTimer.Dispose();
            //recTimer.Change();
            isRunTimer = false;           //关闭can分析仪
        }
    }
}
