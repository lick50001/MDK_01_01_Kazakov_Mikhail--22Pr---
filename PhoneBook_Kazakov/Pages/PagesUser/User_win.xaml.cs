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
using ClassModule;
using static System.Net.Mime.MediaTypeNames;

namespace PhoneBook_Kazakov.Pages.PagesUser
{
    /// <summary>
    /// Логика взаимодействия для User_win.xaml
    /// </summary>
    public partial class User_win : Page
    {
        User user_loc;
        public User_win(User _user)
        {
            InitializeComponent();
            user_loc = _user;

            if (_user.fio_user != null)
            {
                fio_user.Text = _user.fio_user;
                phone_user.Text = _user.phone_num;
                addrec_user.Text = _user.passport_data;
            }
        }

        private void Clock_User_Redact(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.connect.ItsOnlyFIO(fio_user.Text))
            {
                MessageBox.Show("Пиши ФИИИИО!!!");
                return;
            }
            if (!MainWindow.connect.ItsNumber(phone_user.Text))
            {
                MessageBox.Show("Пиши НОМЕР!!!");
                return;
            }
            if (addrec_user.Text.Trim() =="")
            {
                MessageBox.Show("Пиши ПАСПОРТ!!!");
                return;
            }
            if (user_loc.fio_user == null)
            {
                int id = MainWindow.connect.SetLastId(ClassConnection.Connection.tabels.users);
                string query = $"INSERT INTO [users]([Код], [phone_num], [FIO_user], [pasport_data]) VALUES ({id.ToString()}, "+
                    $"'{phone_user.Text}', '{fio_user.Text}', .{addrec_user.Text}')";

                var pc = MainWindow.connect.QueryAccess(query);
                if (pc != null)
                {
                    MainWindow.connect.LoadData(ClassConnection.Connection, tabels.users);
                    MessageBox.Show("Успешное добавление клиента", "Успешное", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.users);
                }
                else
                    MessageBox.Show("Запрос на добавление клиента не был обработан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string query = $"UPDATE [users] SET [phone_num] = '{phone_user.Text}', " +
                    $"[FIO_user]='{fio_user.Text}', " +
                    $"[pasport_data]='{addrec_user.Text}' WHERE Код = {user_loc.id}";

                var pc = MainWindow.connect.QueryAccess(query);
                if(pc != null)
                {
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
                    MessageBox.Show("Успешное изменение клиента", "Успешное", MessageBoxButton.OK, MessageBoxImage.Information);

                    MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page.main.users)
                }
                else
                    MessageBox.Show("Запрос на изменение клиене был обработан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Click_Cancel_User_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.connect.LoadData(ClassConnection.Conecction.tabels.users);

                call userFind = MainWindow.connect.calls.Find(x => x.user_id == user_loc.id);

                if (userFind != null)
                {
                    var click = MessageBox.Show("У данного клиента есть звонки, все равно удалить его?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (click == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                string vs1 = $"DELETE FROM [calls] WHERE [user_id] = '{user_loc.id.ToString()}'";
                var pc1 = MainWindow.connect.QueryAccess(vs1);

                string vs = "DELETE FROM [users] WHERE [Код] = " + user_loc.id.ToString() + "";
                var pc = MainWindow.connect.QueryAccess(vs);
                if (pc != null && pc1 != null)
                {
                    MessageBox.Show("Успешное удаление клиента", "Успешное", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
                    MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.users);
                }
                else
                    MessageBox.Show("Запрос на удаление клиента не был обработан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
