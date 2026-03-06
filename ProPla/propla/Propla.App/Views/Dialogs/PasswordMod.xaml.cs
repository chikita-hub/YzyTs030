using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Propla
{
    /// <summary>
    /// proplaLogin.xaml の相互作用ロジック
    /// </summary>
    public partial class PasswordMod : Window
    {
        string mode = "";
        const string PassConst = "PASS";
        const string UserConst = "USER";

        public string Password = "";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PasswordMod()
        {
            //パスワード画面
            InitializeComponent();

            mode = PassConst;
            BtnMod.Content = "登録";
        }

        public PasswordMod(string iD)
        {
            //ユーザ画面からパスワード変更
            InitializeComponent();

            mode = UserConst;
            BtnMod.Content = "更新";
        }

        /// <summary>
        /// ログインボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Button _btn = (Button)sender;

            switch (_btn.Name)
            {
                /// 変更
                case "BtnMod":
                    {
                        if (string.IsNullOrEmpty(pass1.Password))
                        {
                            MessageBox.Show("パスワードを入力して下さい");
                            return;
                        }
                        if (string.IsNullOrEmpty(pass2.Password))
                        {
                            MessageBox.Show("パスワードの確認を入力して下さい");
                            return;
                        }

                        if (pass1.Password != pass2.Password)
                        {
                            MessageBox.Show("パスワードが異なっています。確認して下さい");
                            return;
                        }

                        if (mode == PassConst)
                        {
                            DataTable _AccountDt = StdLogin.GetAccount("");
                            _AccountDt.Rows[0]["password"] = pass1.Password;

                            if (StdLogin.SetAccount(_AccountDt))
                            {
                                MessageBox.Show("パスワードを変更しました。");

                                // ダイアログを閉じる
                                this.Close();
                                break;
                            }
                            MessageBox.Show("パスワードを変更に失敗しました。");
                            break;
                        }
                        else
                        {
                            Password = pass1.Password;
                            // ダイアログを閉じる
                            this.Close();
                            break;
                        }
                    }
                default:
                    MessageBox.Show("Buttonエラー：定義がない");
                    break;
            }
        }
    }
}
