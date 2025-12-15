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
using System.Xml.Linq;
using PhoneBook_Kazakov.Elements;

// Псевдонимы для разрешения конфликта имен
using ModuleUser = ClassModule.User;
using ModuleCall = ClassModule.Call;

namespace PhoneBook_Kazakov.Pages
{
    public partial class Main : Page
    {
        // Перечисление страниц
        public enum PageMain
        {
            Users,
            Calls,
            None
        }

        // Статическое поле для отслеживания текущей выбранной страницы
        public static PageMain PageSelect;

        public Main()
        {
            InitializeComponent();
            // Инициализируем статическое поле
            PageSelect = PageMain.None;
        }

        // Метод для кнопки "Клиенты"
        private void Click_Phone(object sender, RoutedEventArgs e)
        {
            if (MainWindow.main.frame_main.Visibility == Visibility.Visible)
            {
                MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main);
            }
            if (PageSelect != PageMain.Users)
            {
                PageSelect = PageMain.Users;

                // Анимация исчезновения текущего содержимого
                DoubleAnimation opgridAnimation = new DoubleAnimation();
                opgridAnimation.From = 1;
                opgridAnimation.To = 0;
                opgridAnimation.Duration = TimeSpan.FromSeconds(0.2);
                opgridAnimation.Completed += delegate
                {
                    // Очищаем контейнер
                    parrent.Children.Clear();
                    // Анимация появления нового содержимого
                    DoubleAnimation opr1Animation = new DoubleAnimation();
                    opr1Animation.From = 0;
                    opr1Animation.To = 1;
                    opr1Animation.Duration = TimeSpan.FromSeconds(0.2);
                    opr1Animation.Completed += async delegate
                    {
                        await Task.Delay(1);
                        MainWindow.connect.LoadData(ClassConnection.Connection.Tabels.Users);

                        foreach (var user_itm in MainWindow.connect.users)
                        {
                            if (PageSelect == PageMain.Users)
                            {
                                // Приводим к правильному типу
                                var moduleUser = new ModuleUser
                                {
                                    Id = user_itm.Id,
                                    PhoneNum = user_itm.PhoneNum,
                                    FioUser = user_itm.FioUser,
                                    PassportData = user_itm.PassportData
                                };
                                parrent.Children.Add(new User_itm(moduleUser));
                                await Task.Delay(90);
                            }
                        }

                        if (PageSelect == PageMain.Users)
                        {
                            var ff = new Pages.PagesUser.User_win(new ModuleUser());
                            parrent.Children.Add(new Add_itm(ff));
                        }
                    };

                    parrent.BeginAnimation(StackPanel.OpacityProperty, opr1Animation);
                };
                parrent.BeginAnimation(StackPanel.OpacityProperty, opgridAnimation);
            }
        }

        // Метод для кнопки "История звонков"
        private void Click_History(object sender, RoutedEventArgs e)
        {
            if (MainWindow.main.frame_main.Visibility == Visibility.Visible)
            {
                MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main);
            }
            if (PageSelect != PageMain.Calls)
            {
                PageSelect = PageMain.Calls;

                // Анимация исчезновения текущего содержимого
                DoubleAnimation opgridAnimation = new DoubleAnimation();
                opgridAnimation.From = 1;
                opgridAnimation.To = 0;
                opgridAnimation.Duration = TimeSpan.FromSeconds(0.2);
                opgridAnimation.Completed += delegate
                {
                    parrent.Children.Clear();
                    // Анимация появления нового содержимого
                    DoubleAnimation opr1Animation = new DoubleAnimation();
                    opr1Animation.From = 0;
                    opr1Animation.To = 1;
                    opr1Animation.Duration = TimeSpan.FromSeconds(0.2);
                    opr1Animation.Completed += async delegate
                    {
                        await Task.Delay(1);
                        MainWindow.connect.LoadData(ClassConnection.Connection.Tabels.Calls);

                        foreach (var call_itm in MainWindow.connect.calls)
                        {
                            if (PageSelect == PageMain.Calls)
                            {
                                // Создаем элемент для отображения звонка
                                parrent.Children.Add(CreateCallItem(call_itm));
                                await Task.Delay(90);
                            }
                        }

                        if (PageSelect == PageMain.Calls)
                        {
                            var moduleCall = new ModuleCall();
                            var ff = new Pages.PagesUser.Call_win(moduleCall);
                            parrent.Children.Add(new Add_itm(ff));
                        }
                    };

                    parrent.BeginAnimation(StackPanel.OpacityProperty, opr1Animation);
                };
                parrent.BeginAnimation(StackPanel.OpacityProperty, opgridAnimation);
            }
        }

        // Метод для создания элемента звонка
        private UIElement CreateCallItem(ClassConnection.Call call)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(5)
            };

            var stackPanel = new StackPanel();

            // ID звонка
            var idText = new TextBlock
            {
                Text = $"ID: {call.Id}",
                FontWeight = FontWeights.Bold
            };
            stackPanel.Children.Add(idText);

            // Категория звонка
            var categoryText = new TextBlock
            {
                Text = $"Категория: {GetCategoryName(call.CategoryCall)}",
                Margin = new Thickness(0, 5, 0, 0)
            };
            stackPanel.Children.Add(categoryText);

            // Дата и время
            var dateTimeText = new TextBlock
            {
                Text = $"Дата: {call.Date}, Время: {call.TimeStart} - {call.TimeEnd}",
                Margin = new Thickness(0, 5, 0, 0)
            };
            stackPanel.Children.Add(dateTimeText);

            // ID пользователя
            var userIdText = new TextBlock
            {
                Text = $"ID пользователя: {call.UserId}",
                Margin = new Thickness(0, 5, 0, 0)
            };
            stackPanel.Children.Add(userIdText);

            border.Child = stackPanel;
            return border;
        }

        // Метод для получения названия категории
        private string GetCategoryName(int categoryId)
        {
            switch (categoryId)
            {
                case 1: return "Исходящий";
                case 2: return "Входящий";
                default: return "Неизвестно";
            }
        }

        // Функция анимированного перехода по страницам
        public void Anim_move(Control control1, Control control2, Frame frame_main = null,
                             Page pages = null, PageMain page_restart = PageMain.None)
        {
            if (page_restart != PageMain.None)
            {
                if (page_restart == PageMain.Users)
                {
                    PageSelect = PageMain.None;
                    Click_Phone(new object(), new RoutedEventArgs());
                }
                else if (page_restart == PageMain.Calls)
                {
                    PageSelect = PageMain.None;
                    Click_History(new object(), new RoutedEventArgs());
                }
            }
            else
            {
                DoubleAnimation opgridAnimation = new DoubleAnimation();
                opgridAnimation.From = 1;
                opgridAnimation.To = 0;
                opgridAnimation.Duration = TimeSpan.FromSeconds(0.3);
                opgridAnimation.Completed += delegate
                {
                    if (pages != null)
                    {
                        frame_main.Navigate(pages);
                    }

                    DoubleAnimation opr1Animation = new DoubleAnimation();
                    opr1Animation.From = 0;
                    opr1Animation.To = 1;
                    opr1Animation.Duration = TimeSpan.FromSeconds(0.4);

                    control2.BeginAnimation(ScrollViewer.OpacityProperty, opr1Animation);
                };

                control1.BeginAnimation(ScrollViewer.OpacityProperty, opgridAnimation);
            }
        }
    }
}