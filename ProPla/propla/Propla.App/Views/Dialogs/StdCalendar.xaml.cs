using System;
using System.Windows;
using System.Windows.Controls;

namespace Propla
{
    /// <summary>
    /// StdCalendar.xaml の相互作用ロジック
    /// </summary>
    public partial class StdCalendar : Window
    {
        public bool dialog = false;
        public static string CalData = "";
        DateTime dt;

        public StdCalendar()
        {
            InitializeComponent();

            //string mode = "y";
            string y = "";
            string m = "";
            string d = "";
            //try
            //{
            //    for (int i = 0; i < CalData.Length; i++)
            //    {
            //        string s=CalData.Substring(i, 1);
            //        string nums = "0123456789";
            //        if (nums.IndexOf(s) >= 0 )
            //        {
            //            switch (mode)
            //            {
            //                case "y":
            //                    {
            //                        y += s;
            //                        break;
            //                    }
            //                case "m":
            //                    {
            //                        m += s;
            //                        break;
            //                    }
            //                case "d":
            //                    {
            //                        d += s;
            //                        break;
            //                    }
            //                default:
            //                    break;
            //            }
            //        }
            //        else
            //        {
            //            switch (mode)
            //            {
            //                case "y":
            //                    {
            //                        if (s=="/" || s=="年" )
            //                        {
            //                            mode = "m";
            //                        }
            //                        break;
            //                    }
            //                case "m":
            //                    {
            //                        if (s == "/" || s == "月")
            //                        {
            //                            mode = "d";
            //                        }
            //                        break;
            //                    }
            //                case "d":
            //                    {
            //                        mode = "e";
            //                        break;
            //                    }
            //                default:
            //                    break;
            //            }

            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //    DialogResult = false;
            //    return;
            //}

            string s = DateAnz(CalData);
            if (s == "")
            {
                DialogResult = false;
                return;
            }

            string[] parts = s.Split('/');
            foreach (string part in parts)
            {
                if (y == "")
                {
                    y = part;
                }
                else
                {
                    if (m == "")
                    {
                        m = part;
                    }
                    else
                    {
                        d = part;
                    }
                }
            }


            try
            {
                if (y == "")
                {
                    dt = DateTime.Today;
                }
                else
                {
                    int i1 = int.Parse(y);
                    int i2 = 1;
                    int i3 = 1;
                    if (m != "")
                    {
                        i2 = int.Parse(m);
                    }
                    if (d != "")
                    {
                        i3 = int.Parse(d);
                    }
                    dt = new DateTime(i1, i2, i3);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("不正な日付です。");
                return;
            }


            Cal.DisplayDate = dt;
            Cal.SelectedDate = dt;
            dialog = true;

        }

        public static string DateAnz(string date)
        {
            string mode = "y";
            string y = "";
            string m = "";
            string d = "";
            try
            {
                for (int i = 0; i < date.Length; i++)
                {
                    string s = date.Substring(i, 1);
                    string nums = "0123456789";
                    if (nums.IndexOf(s) >= 0)
                    {
                        switch (mode)
                        {
                            case "y":
                                {
                                    y += s;
                                    break;
                                }
                            case "m":
                                {
                                    m += s;
                                    break;
                                }
                            case "d":
                                {
                                    d += s;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (mode)
                        {
                            case "y":
                                {
                                    if (s == "/" || s == "年")
                                    {
                                        mode = "m";
                                    }
                                    break;
                                }
                            case "m":
                                {
                                    if (s == "/" || s == "月")
                                    {
                                        mode = "d";
                                    }
                                    break;
                                }
                            case "d":
                                {
                                    mode = "e";
                                    break;
                                }
                            default:
                                break;
                        }

                    }
                }
                return y + "/" + m + "/" + d;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return "";
            }

        }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Button _btn = (Button)sender;

            switch (_btn.Name)
            {
                /// 引用
                //case "BtnUpdate":
                //    {


                //        CalData = Cal.SelectedDate.ToString();

                //        CalData = CalData.Substring(0, 4) + "年" + CalData.Substring(5, 2) + "月" + CalData.Substring(8, 2) + "日";

                //        DialogResult = false;
                //        break;
                //    }
                /// 引用
                case "BtnEnd":
                    {
                        CalData = "";
                        DialogResult = false;
                        break;
                    }
            }
        }
        private void CalendarSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dialog == false)
            {
                return;
            }
            DateTime? selectedDate = Cal.SelectedDate;
            CalData = selectedDate?.ToString("yyyy年M月d日");

            //CalData = Cal.SelectedDate.ToString();

            //CalData = CalData.Substring(0, 4) + "年" + CalData.Substring(5, 2) + "月" + CalData.Substring(8, 2) + "日";

            DialogResult = false;
        }
        //private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    Console.WriteLine("btn3");
        //}

    }
}
