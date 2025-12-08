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

namespace PhoneBook_Kazakov.Pages.PagesUser
{
    /// <summary>
    /// Логика взаимодействия для Call_win.xaml
    /// </summary>
    public partial class Call_win : Page
    {
        call call_itm;
        public Call_win(call _call)
        {
            InitializeComponent();
            call_itm = _call;

            if (_call.time_start != null )
            {
                string[] dateTimeStart = _call.time_start.Split(' ');
                string[] dateStart = dateTimeStart[0].Split('.');
                date_start_call.SelectedDate - new DateTime(int.Parse(dateStart[2]), int.Parse(dateStart[1]), int.Parse(dateStart[0]));
                time_start.Text = dateTimeFinish[1];
            }
            else
            {
                time_start.Text = "00:00";
                time_finish.Text = "00:00";
            }

            ComboBoxItem combItm = new ComboBoxItem();
            combItm.Tag = 1;
            combItm.Content = "Исходящий";
            if (_call.category_call == 1) combItm.IsSelected = true;
            call_category_text.Items.Add(combItm);

            ComboBoxItem combItm1 = new ComboBoxItem();
            combItm1.Tag = 2;
            combItm1.Content = "Входящий";
            if ((_call.category_call == 2)) combItm1.IsSelected = true;
            call_category_text.Items.Add(combItm1);

            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
            foreach (UserControl itm in MainWindow.connect.users)
            {
                ComboBoxItem combUser = new ComboBoxItem();
                combUser.Tag = itm.id;
                combUser.Content = itm.fio_user;
                if (_call.category_call == itm.id) combUser.IsSelected = true;

                user_select.Items.Add(combUser);
            }
        }

        private void Click_Call_Redact(object sender, RoutedEventArgs e)
        {
            if (!CheckTime(time_start.Text))
            {
                MessageBox.Show("Время старта не правильно");
                return;
            }
            if (!CheckTime(time_finish.Text))
            {
                MessageBox.Show("Время конца неправильно");
                return;
            }
            if (dat)
            {

            }
        }
    }
}
