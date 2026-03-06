using System.Windows;
using System.Windows.Controls;

namespace Propla
{
    /// <summary>
    /// TxtInputBox.xaml の相互作用ロジック
    /// </summary>
    public partial class TxtInputBox : Window
    {


        public string MsgTxt = "";

        public TxtInputBox()
        {
            InitializeComponent();

        }
        //public string txt
        //{
        //    get { return txt; }
        //    set { TextBox.Text = txt; }
        //}
        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Button _btn = (Button)sender;

            switch (_btn.Name)
            {
                case "BtnSet":
                    {
                        MsgTxt = TextBox.Text;
                        DialogResult = false;
                        break;
                    }
                case "BtnCancel":
                    {
                        MsgTxt = "";
                        DialogResult = false;
                        break;
                    }
            }
        }
    }
}
