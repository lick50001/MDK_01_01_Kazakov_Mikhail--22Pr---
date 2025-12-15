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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassConnection;

namespace PhoneBook_Kazakov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Connection connect;
        public static Pages.Main main;
        public MainWindow()
        {
            InitializeComponent();
            connect = new Connection();

            connect.LoadData(Connection.Tabels.Users);
            connect.LoadData(Connection.Tabels.Calls);

            main = new Pages.Main();

            OpenPageMain();
        }

        public void OpenPageMain()
        {
            DoubleAnimation opgriAnimation = new DoubleAnimation();
            opgriAnimation.From = 1;
            opgriAnimation.To = 0;
            opgriAnimation.Duration = TimeSpan.FromSeconds(0.6);
            opgriAnimation.Completed += delegate
            {
                frame.Navigate(main);

                DoubleAnimation opgrisAnimation = new DoubleAnimation();
                opgrisAnimation.From = 0;
                opgrisAnimation.To = 1;
                opgrisAnimation.Duration = TimeSpan.FromSeconds(1.2);

                frame.BeginAnimation(Frame.OpacityProperty, opgriAnimation);
            };

            frame.BeginAnimation(Frame.OpacityProperty, opgriAnimation);
        }
    }
}
