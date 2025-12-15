using System;
using System.Windows;
using System.Windows.Controls;
using ClassConnection;
using System.Linq;

namespace PhoneBook_Kazakov.Pages.PagesUser
{
    /// <summary>
    /// Логика взаимодействия для User_win.xaml
    /// </summary>
    public partial class User_win : Page
    {
        User user_loc;

        public User_win(ClassConnection.User _user)
        {
            InitializeComponent();
            user_loc = _user;

            if (_user.FioUser != null)
            {
                fio_user.Text = _user.FioUser;
                phone_user.Text = _user.PhoneNum;
                addrec_user.Text = _user.PassportData;
            }
        }

        private void Click_User_Redact(object sender, RoutedEventArgs e)
        {
            // Проверка ФИО (простая проверка на наличие хотя бы одного пробела)
            if (string.IsNullOrWhiteSpace(fio_user.Text) || !fio_user.Text.Contains(" "))
            {
                MessageBox.Show("Введите ФИО (минимум имя и фамилию через пробел)!");
                return;
            }

            // Проверка номера телефона (базовая проверка)
            if (string.IsNullOrWhiteSpace(phone_user.Text) || !IsValidPhoneNumber(phone_user.Text))
            {
                MessageBox.Show("Введите корректный номер телефона!");
                return;
            }

            if (string.IsNullOrWhiteSpace(addrec_user.Text))
            {
                MessageBox.Show("Введите паспортные данные!");
                return;
            }

            if (user_loc.FioUser == null)
            {
                // Добавление нового пользователя
                int id = MainWindow.connect.SetLastId(Connection.Tabels.Users);
                string query = $"INSERT INTO [users] ([Код], [phone_num], [FIO_user], [pasport_data]) " +
                               $"VALUES ({id}, '{phone_user.Text}', '{fio_user.Text}', '{addrec_user.Text}')";

                var pc = MainWindow.connect.QueryAccess(query);
                if (pc != null)
                {
                    pc.Close();
                    MainWindow.connect.LoadData(Connection.Tabels.Users);
                    MessageBox.Show("Успешное добавление клиента", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    //MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, 
                    //    null, null, Main.page_main.users);
                }
                else
                {
                    MessageBox.Show("Запрос на добавление клиента не был обработан", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                // Обновление существующего пользователя
                string query = $"UPDATE [users] SET [phone_num] = '{phone_user.Text}', " +
                               $"[FIO_user] = '{fio_user.Text}', " +
                               $"[pasport_data] = '{addrec_user.Text}' WHERE [Код] = {user_loc.Id}";

                var pc = MainWindow.connect.QueryAccess(query);
                if (pc != null)
                {
                    pc.Close();
                    MainWindow.connect.LoadData(Connection.Tabels.Users);
                    MessageBox.Show("Успешное изменение клиента", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    //MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, 
                    //    null, null, Main.page_main.users);
                }
                else
                {
                    MessageBox.Show("Запрос на изменение клиента не был обработан", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Click_Cancel_User_Redact(object sender, RoutedEventArgs e)
        {
            // Просто отмена без удаления
            //MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main);
        }

        private void Click_Remove_User_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.connect.LoadData(Connection.Tabels.Users);
                MainWindow.connect.LoadData(Connection.Tabels.Calls);

                // Проверяем, есть ли у пользователя звонки
                Call userFind = MainWindow.connect.calls.Find(x => x.UserId == user_loc.Id);

                if (userFind != null)
                {
                    var click = MessageBox.Show("У данного клиента есть звонки, все равно удалить его?",
                        "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (click == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                // Сначала удаляем связанные звонки
                if (userFind != null)
                {
                    string vs1 = $"DELETE FROM [calls] WHERE [user_id] = {user_loc.Id}";
                    var pc1 = MainWindow.connect.QueryAccess(vs1);
                    if (pc1 != null)
                    {
                        pc1.Close();
                    }
                }

                // Затем удаляем пользователя
                string vs = $"DELETE FROM [users] WHERE [Код] = {user_loc.Id}";
                var pc = MainWindow.connect.QueryAccess(vs);
                if (pc != null)
                {
                    pc.Close();
                    MessageBox.Show("Успешное удаление клиента", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.connect.LoadData(Connection.Tabels.Users);
                    //MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, 
                    //    null, null, Main.page_main.users);
                }
                else
                {
                    MessageBox.Show("Запрос на удаление клиента не был обработан", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Метод для проверки номера телефона
        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Убираем все нецифровые символы
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            // Проверяем, что осталось достаточно цифр (например, 10 для России без кода страны)
            return digitsOnly.Length >= 10;
        }
    }
}