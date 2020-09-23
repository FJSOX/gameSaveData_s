using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;


namespace game
{
    /// <summary>
    /// LiBat.xaml 的交互逻辑
    /// </summary>
    public partial class LiBat : Window
    {
        
        public const uint BMS_Elec_Volt = 0x18f212f3;
        public const uint BMS_TEMP = 0x184f17f3;
        //System.Threading.Timer recTimer1;
        public LiBat()
        {
            InitializeComponent();
        }

        //~LiBat()
        //{
        //    MessageBox.Show("窗口关闭");            
        //}
        //public void CanRecv()
        //{
        //    //CAN can = new CAN();
        //    //can.ReadCANData();
        //    //Thread.Sleep(1000);
        //}

        /// <summary>
        /// 显示数据
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num"></param>
        private void showMsg(string s, int num)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                switch (num)
                {
                    case 0:
                        Volt0.Text = s;
                        break;
                    case 1:
                        this.Volt1.Text=s;
                        break;
                    case 2:
                        this.Volt2.Text=s;
                        break;
                    case 3:
                        this.Volt3.Text=s;
                        break;
                    case 4:
                        this.Volt4.Text=s;
                        break;
                    case 5:
                        this.Current.Text = s;
                        break;
                    case 6:
                        this.soc.Text = s;
                        break;
                    case 7:
                        this.tem1.Text = s;
                        break;
                    case 8:
                        this.tem2.Text = s;
                        break;
                    case 9:
                        this.num1.Text = s;
                        break;
                    case 10:
                        this.num2.Text = s;
                        break;
                    case 11:
                        this.num3.Text = s;
                        break;
                    case 12:
                        this.num4.Text = s;
                        break;
                    case 13:
                        this.num5.Text = s;
                        break;
                    case 14:
                        this.num6.Text = s;
                        break;
                    case 15:
                        this.num7.Text = s;
                        break;
                    case 16:
                        this.num8.Text = s;
                        break;
                    case 17:
                        this.num9.Text = s;
                        break;
                    case 18:
                        this.num10.Text = s;
                        break;
                    case 19:
                        this.num11.Text = s;
                        break;
                    case 20:
                        this.num12.Text = s;
                        break;
                    case 21:
                        this.num13.Text = s;
                        break;
                    case 22:
                        this.num14.Text = s;
                        break;
                    case 23:
                        this.num15.Text = s;
                        break;
                    case 24:
                        this.num16.Text = s;
                        break;
                    case 25:
                        this.soh.Text = s;
                        break;

                    default:
                        break;
                }
                
            }));
        }

        /// <summary>
        /// 打开usbcan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Can_Open(object sender, RoutedEventArgs e)
        {
            CAN can = new CAN();
            can.disPlayCanData += new CAN.HandleReciveCanDataDelegate(this.showMsg);
            can.Open();
   
        }

        //private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{
            
        //}

        //private void TextBox_TextChanged_1(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{

        //}

        
        private static System.Timers.Timer tmr = new System.Timers.Timer(3000);//初始化tmr，间隔为30s

        private FileStream filesw;//声明文件流

        /// <summary>
        /// 点击“保存数据”按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string path = @"D:\C\test\SaveData_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";//在D:\BMSData位置建立以时间命名的记录文档
            //定义写文件流
            filesw = new FileStream(path, FileMode.OpenOrCreate);
            //写入的内容
            string FormHead = "时间\t\t\t电压\t电流\tSoc\t温度1\t温度2\t电压1\t电压2\t电压3\t电压4\t1\t2\t3\t4\t5\t6\t7\t8\t9\t10\t11\t12\t13\t14\t15\t16\n";
            //字符串转byte[]
            byte[] writeBytes = Encoding.UTF8.GetBytes(FormHead);
            //写入
            filesw.Write(writeBytes, 0, writeBytes.Length);

            tmr.Enabled = true;//打开tmr
            tmr.Elapsed += Tmr_Elapsed;//设置tmr.Elapsed事件
        }

        /// <summary>
        /// tmr.Elapsed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //声明inputStr
            string inputStr;

            this.Dispatcher.Invoke(new Action(() =>
            {
                //将BMS数据放入inputStr
                inputStr = DateTime.Now.ToString() + "\t" + Volt0.Text + "\t" + Current.Text + "\t" + soc.Text + "\t" +
                tem1.Text + "\t" + tem2.Text + "\t" +
                Volt1.Text + "\t" + Volt2.Text + "\t" + Volt3.Text + "\t" + Volt4.Text + "\t" +
                num1.Text + "\t" + num2.Text + "\t" + num3.Text + "\t" + num4.Text + "\t" +
                num5.Text + "\t" + num6.Text + "\t" + num7.Text + "\t" + num8.Text + "\t" +
                num9.Text + "\t" + num10.Text + "\t" + num11.Text + "\t" + num12.Text + "\t" +
                num13.Text + "\t" + num14.Text + "\t" + num15.Text + "\t" + num16.Text + "\t" +
                "\n";
                //字符串转byte[]
                byte[] writeBytes = Encoding.UTF8.GetBytes(inputStr);
                //写入
                this.filesw.Write(writeBytes, 0, writeBytes.Length);
            }));
        }

        /// <summary>
        /// “停止”按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOver_Click(object sender, RoutedEventArgs e)
        {
            //关闭文件流
            filesw.Close();

            tmr.Enabled = false;//关闭tmr
            MessageBox.Show("Over");//在屏幕上打印Over
        }

        /// <summary>
        /// 没太看懂估计是将数据传送到数据库的函数
        /// </summary>
        /// <param name="obj"></param>
        public void WriteToMysql(object obj)
        {
            String connetStr = "server=47.96.143.205;port=3306;user=thng_qgf;password=123456; database=qigangfeng_test;";

            string tableName = DateTime.Now.ToString("yyyy-MM-dd");
            MySqlConnection conn = new MySqlConnection(connetStr);
            conn.Open();
            MySqlDataAdapter adp = new MySqlDataAdapter();
            DataTable dt = conn.GetSchema();
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from `" + tableName + "`", conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1146:
                        MessageBox.Show(tableName + "不存在，系统正在为您创建！");
                        //string mySelectQuery = "CREATE TABLE `" + tableName + "`( Time DateTime,Bat Float(6,3))";
                        string mySelectQuery = "CREATE TABLE `" + tableName + "`(Time DateTime" +
                            ",Bat Float(6,3),Cur Double(6,3),Temp1 Float(6,1),Temp2 Float(6,1),Soc Float(3,1)" +
                            ", BatOne Float(6,3), BatTwo Float(6,3) , BatThree Float(6,3), BatFour Float(6,3)" +
                            ", Bat1 Float(6,3), Bat2 Float(6,3) , Bat3 Float(6,3), Bat4 Float(6,3)" +
                            " ,Bat5 Float(6,3), Bat6 Float(6,3) , Bat7 Float(6,3), Bat8 Float(6,3)" +
                            " ,Bat9 Float(6,3), Bat10 Float(6,3) , Bat11 Float(6,3), Bat12 Float(6,3)" +
                            ", Bat13 Float(6,3), Bat14 Float(6,3) , Bat15 Float(6,3), Bat16 Float(6,3))";
                        MySqlCommand cmd = new MySqlCommand(mySelectQuery, conn);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(tableName + "创建成功！");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
            }
            string sqlData = "INSERT INTO `" + tableName + "`(Time,Bat,Cur,Temp1,Temp2,Soc,BatOne,BatTwo,BatThree,BatFour" +
                ",Bat1,Bat2,Bat3,Bat4,Bat5,Bat6,Bat7,Bat8,Bat9,Bat10,Bat11,Bat12,Bat13,Bat14,Bat15,Bat16) " +
                "values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + (CAN.vb[0] * 0.02) + "'" +
                ",'" + (CAN.cur[0] * 0.1 - 3200) + "','" + (CAN.tem[0] - 40) + "','" + (CAN.tem[1] - 40) + "','" + ((CAN.soc[0] * 0.4)) + "'" +
                ",'" + ((CAN.vb[1] + CAN.vb[2] + CAN.vb[3] + CAN.vb[4]) * 0.001) + "','" + ((CAN.vb[5] + CAN.vb[6] + CAN.vb[7] + CAN.vb[8]) * 0.001) + "'" +
                ",'" + ((CAN.vb[9] + CAN.vb[10] + CAN.vb[11] + CAN.vb[12]) * 0.001) + "','" + ((CAN.vb[13] + CAN.vb[14] + CAN.vb[15] + CAN.vb[16]) * 0.001) + "'" +
                ",'" + ((CAN.vb[1] * 0.001)) + "','" + ((CAN.vb[2] * 0.001)) + "','" + ((CAN.vb[3] * 0.001)) + "','" + ((CAN.vb[4] * 0.001)) + "'" +
                ",'" + ((CAN.vb[5] * 0.001)) + "','" + ((CAN.vb[6] * 0.001)) + "','" + ((CAN.vb[7] * 0.001)) + "','" + ((CAN.vb[8] * 0.001)) + "'" +
                ",'" + ((CAN.vb[9] * 0.001)) + "','" + ((CAN.vb[10] * 0.001)) + "','" + ((CAN.vb[11] * 0.001)) + "','" + ((CAN.vb[12] * 0.001)) + "'" +
                ",'" + ((CAN.vb[13] * 0.001)) + "','" + ((CAN.vb[14] * 0.001)) + "','" + ((CAN.vb[15] * 0.001)) + "','" + ((CAN.vb[16] * 0.001)) + "')";


            MySqlCommand insertCommand = new MySqlCommand(sqlData, conn);
            insertCommand.ExecuteNonQuery();

        }
    }
}
