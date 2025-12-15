using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClassConnection;

namespace PhoneBook_Kazakov.Pages.PagesUser
{
    public partial class Call_win : Page
    {
        Call call_itm;
        public Call_win(Call _call)
        {
            InitializeComponent();
            call_itm = _call;

            if (_call.TimeStart != null)
            {
                string[] dateTimeStart = _call.TimeStart.Split(' ');
                if (dateTimeStart.Length >= 2)
                {
                    string[] dateStart = dateTimeStart[0].Split('.');
                    if (dateStart.Length >= 3)
                    {
                        date_start_call.SelectedDate = new DateTime(
                            int.Parse(dateStart[2]),
                            int.Parse(dateStart[1]),
                            int.Parse(dateStart[0]));
                    }
                    time_start.Text = dateTimeStart[1];

                    if (_call.TimeEnd != null)
                    {
                        string[] dateTimeEnd = _call.TimeEnd.Split(' ');
                        if (dateTimeEnd.Length >= 2)
                        {
                            string[] dateEnd = dateTimeEnd[0].Split('.');
                            if (dateEnd.Length >= 3)
                            {
                                date_end_call.SelectedDate = new DateTime(
                                    int.Parse(dateEnd[2]),
                                    int.Parse(dateEnd[1]),
                                    int.Parse(dateEnd[0]));
                            }
                            time_finish.Text = dateTimeEnd[1];
                        }
                    }
                }
            }
            else
            {
                time_start.Text = "00:00";
                time_finish.Text = "00:00";
            }

            ComboBoxItem combItm = new ComboBoxItem();
            combItm.Tag = 1;
            combItm.Content = "Исходящий";
            if (_call.CategoryCall == 1) combItm.IsSelected = true;
            call_category_text.Items.Add(combItm);

            ComboBoxItem combItm1 = new ComboBoxItem();
            combItm1.Tag = 2;
            combItm1.Content = "Входящий";
            if (_call.CategoryCall == 2) combItm1.IsSelected = true;
            call_category_text.Items.Add(combItm1);

            MainWindow.connect.LoadData(Connection.Tabels.Users);
            foreach (User itm in MainWindow.connect.users)
            {
                ComboBoxItem combUser = new ComboBoxItem();
                combUser.Tag = itm.Id;
                combUser.Content = itm.FioUser;
                if (_call.UserId == itm.Id) combUser.IsSelected = true;

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

            if (date_start_call.SelectedDate != null && date_end_call.SelectedDate != null)
            {
                DateTime dateStart = (DateTime)date_start_call.SelectedDate;
                DateTime dateFinish = (DateTime)date_end_call.SelectedDate;

                if (dateFinish >= dateStart)
                {
                    User id_temp_user = null;
                    if (user_select.SelectedItem != null)
                    {
                        ComboBoxItem selectedUser = (ComboBoxItem)user_select.SelectedItem;
                        id_temp_user = MainWindow.connect.users.Find(x => x.Id == Convert.ToInt32(selectedUser.Tag));
                    }

                    if (id_temp_user == null)
                    {
                        MessageBox.Show("Запрос не был обработан. Вы не указали пользователя", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    int id_calls_categ = 0;
                    if (call_category_text.SelectedItem != null)
                    {
                        ComboBoxItem selectedCategory = (ComboBoxItem)call_category_text.SelectedItem;
                        id_calls_categ = Convert.ToInt32(selectedCategory.Tag);
                    }
                    else
                    {
                        MessageBox.Show("Запрос не был обработан. Вы не указали категории звонка", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (call_itm.TimeEnd == null || string.IsNullOrEmpty(call_itm.TimeEnd))
                    {
                        int id = MainWindow.connect.SetLastId(Connection.Tabels.Calls);

                        string query = $"INSERT INTO [calls] ([Код], [user_id], [category_call], [date], [time_start], [time_end]) " +
                            $"VALUES ({id}, {id_temp_user.Id}, {id_calls_categ}, " +
                            $"'{dateStart:yyyy-MM-dd}', " +
                            $"'{dateStart:yyyy-MM-dd} {time_start.Text}', " +
                            $"'{dateFinish:yyyy-MM-dd} {time_finish.Text}')";

                        var pc = MainWindow.connect.QueryAccess(query);
                        if (pc != null)
                        {
                            pc.Close();
                            MainWindow.connect.LoadData(Connection.Tabels.Calls);
                            MessageBox.Show("Успешное добавление звонка", "Успешно",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            //MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.calls);
                        }
                        else
                        {
                            MessageBox.Show("Запрос на добавление звонка не был обработан", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        string query = $"UPDATE [calls] SET [user_id] = {id_temp_user.Id}, " +
                            $"[category_call] = {id_calls_categ}, " +
                            $"[date] = '{dateStart:yyyy-MM-dd}', " +
                            $"[time_start] = '{dateStart:yyyy-MM-dd} {time_start.Text}', " +
                            $"[time_end] = '{dateFinish:yyyy-MM-dd} {time_finish.Text}' " +
                            $"WHERE [Код] = {call_itm.Id}";

                        var pc = MainWindow.connect.QueryAccess(query);
                        if (pc != null)
                        {
                            pc.Close();
                            MainWindow.connect.LoadData(Connection.Tabels.Calls);
                            MessageBox.Show("Успешное изменение звонка", "Успешно",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            //MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.calls);
                        }
                        else
                        {
                            MessageBox.Show("Запрос на изменение звонка не был обработан", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Дата старта больше чем дата конца");
                }
            }
            else
            {
                MessageBox.Show("Вы не указали дату");
            }
        }

        private void Click_Cancel_Call_Redact(object sender, RoutedEventArgs e)
        {
            // отмена действий и переход на главное окно
            //MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main);
        }

        private void Click_Remove_Call_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.connect.LoadData(Connection.Tabels.Calls);

                string query = "DELETE FROM [calls] WHERE [Код] = " + call_itm.Id;
                var pc = MainWindow.connect.QueryAccess(query);
                if (pc != null)
                {
                    pc.Close();
                    MessageBox.Show("Успешное удаление звонка", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.connect.LoadData(Connection.Tabels.Calls);
                    //MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.calls);
                }
                else
                {
                    MessageBox.Show("Запрос на удаление звонка не был обработан", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool CheckTime(string str)
        {
            string[] str1 = str.Split(':');
            if (str1.Length == 2)
            {
                if (str1[0].Trim() != "" && str1[1].Trim() != "")
                {
                    if (int.TryParse(str1[0], out int hours) &&
                        int.TryParse(str1[1], out int minutes))
                    {
                        if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}