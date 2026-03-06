using System.Windows;
using System.Windows.Controls;

namespace Propla
{
    /// <summary>
    /// TxtMsgBox.xaml の相互作用ロジック
    /// </summary>
    public partial class TxtMsgBox : Window
    {
        private string msgTxt = "";
        public TxtMsgBox(string s)
        {
            InitializeComponent();
            TextBox.Text = s;
            msgTxt = s;

        }

        public void SetEnable()
        {
            TextBox.IsReadOnly = false;
            end.Content = "登録";
        }
        public string GetText()
        {
            return msgTxt;
        }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Button _btn = (Button)sender;

            switch (_btn.Name)
            {
                //終了
                case "end":
                    {
                        msgTxt = TextBox.Text;
                        DialogResult = false;
                        break;
                    }
            }
        }
    }
}
