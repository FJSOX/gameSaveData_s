using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace game
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        //private void TF_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    TwentyFour twentyFour = new TwentyFour();
        //    twentyFour.Show();
        //    this.Close();
        //}

        //private void Hanoi_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Hanoi hanoi = new Hanoi();
        //    hanoi.Show();

        //}

        /// <summary>
        /// 测试窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_LiBat(object sender, RoutedEventArgs e)
        {
            LiBat libatWindow = new LiBat();
            libatWindow.Show();
            //this.Close();
            //this.Hide();
        }
    }
}
