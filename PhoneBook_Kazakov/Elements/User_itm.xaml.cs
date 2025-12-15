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
using ClassModule;
using ClassConnection;

namespace PhoneBook_Kazakov.Elements
{
    /// <summary>
    /// Логика взаимодействия для User_itm.xaml
    /// </summary>
    public partial class User_itm : UserControl
    {
        User user_loc;
        public User_itm(User _user)
        {
            InitializeComponent();
            user_loc = _user;
            if (_user.FioUser != null)
            {
                name_user.Content = _user.FioUser;
                phone_user.Content = "Номер: " + _user.PhoneNum;
            }

            // создание анимации при инициализации
            DoubleAnimation opgridAnimation = new DoubleAnimation();
            opgridAnimation.From = 0;
            opgridAnimation.To = 1;
            opgridAnimation.Duration = TimeSpan.FromSeconds(0.4);
            border.BeginAnimation(StackPanel.OpacityProperty, opgridAnimation);
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            MainWindow.main.Anim_move(MainWindow.main.scroll_main, MainWindow.main.frame_main,
                MainWindow.main.frame_main, new Pages.PagesUser.User_win(user_loc));
        }

        private void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.connect.LoadData(Connection.Tabels.Users);

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
                    //    null, null, Pages.Main.page_main.users);
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
    }
}