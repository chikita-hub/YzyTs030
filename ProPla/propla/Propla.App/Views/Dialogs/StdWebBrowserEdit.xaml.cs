using System.Windows;
using System.Windows.Controls;
namespace Propla
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class StdWebBrowserEdit : Window
    {

        public static string HtmlHheader = "<html> <head><meta http-equiv=\'Content-Type\' content=\'text/html;charset=UTF-8\'></head><body>";
        public static string HtmlFooter = "</body></html>";

        public StdWebBrowserEdit()
        {
            InitializeComponent();

            //WebDisplay.Source = "https://www.yazuya.com/";

            //WebBrow.NavigateToString(@HtmlHheader+ HtmlFooter);

        }

        string txtSave = "";

        public void SetText(string pText)
        {
            EditTextBox.Text = pText;
            txtSave = pText;
        }
        public string GetText()
        { return EditTextBox.Text; }


        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Button _btn = (Button)sender;

            switch (_btn.Name)
            {
                /// 確認
                case "BtnTextBoxDisp":
                    {

                        string _s = HtmlHheader;
                        _s += EditTextBox.Text;
                        _s += HtmlFooter;
                        WebBrow.NavigateToString(@_s);
                        //.NavigateToString(@_s);

                        break;
                    }
                //登録
                case "register":
                    {
                        DialogResult = false;
                        break;
                    }
                //キャンセル
                case "cancel":
                    {
                        EditTextBox.Text = txtSave;
                        DialogResult = false;
                        break;
                    }
                //リンク雛形
                case "template1":
                    {
                        string _s = EditTextBox.Text + "\r\n";
                        _s += "<a href=" + "\r\n";
                        _s += "☆☆☆☆　この行にリンク先を入力　☆☆☆☆" + "\r\n";
                        _s += " target=\"_blank\">" + "\r\n";
                        _s += "<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">" + "\r\n";
                        _s += "☆☆☆☆　この行にリンクの説明を入力　☆☆☆☆" + "\r\n";
                        _s += "</font>" + "\r\n";
                        _s += "</a>";

                        EditTextBox.Text = _s;
                        break;
                    }
                //文字雛形
                case "template2":
                    {
                        string _s = EditTextBox.Text + "\r\n";
                        _s += "<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">" + "\r\n";
                        _s += "☆☆☆☆　この行に文書を入力　☆☆☆☆" + "\r\n";
                        _s += "</font>";

                        EditTextBox.Text = _s;
                        break;
                    }
                //画像の表示リンク雛形
                case "template3":
                    {
                        string _s = "";
                        //_s += "<!DOCTYPE html>" + "\r\n";
                        //_s += "<html lang=\"ja\">" + "\r\n";
                        //_s += "    <head>" + "\r\n";
                        //_s += "        <meta charset=\"UTF-8\">" + "\r\n";
                        //_s += "        <title>CSS練習</title>" + "\r\n";
                        //_s += "        <style>" + "\r\n";
                        //_s += "            .td1 {" + "\r\n";
                        //_s += "                width: 120px;" + "\r\n";
                        //_s += "                line-height: 20px;" + "\r\n";
                        //_s += "            }" + "\r\n";
                        //_s += "            .td2 {" + "\r\n";
                        //_s += "                width: 57px;" + "\r\n";
                        //_s += "                line-height: 20px;" + "\r\n";
                        //_s += "            }" + "\r\n";
                        //_s += "            .td3 {" + "\r\n";
                        //_s += "                width: 700;" + "\r\n";
                        //_s += "                line-height: 20px;" + "\r\n";
                        //_s += "                font-size: 14px;" + "\r\n";
                        //_s += "            }" + "\r\n";
                        //_s += "            .td4 {" + "\r\n";
                        //_s += "                width: 20px;" + "\r\n";
                        //_s += "                line-height: 20px;" + "\r\n";
                        //_s += "            }" + "\r\n";
                        //_s += "            .td5 {" + "\r\n";
                        //_s += "                width: 94px;" + "\r\n";
                        //_s += "                line-height: 20px;" + "\r\n";
                        //_s += "            }" + "\r\n";
                        //_s += "        </style>" + "\r\n";
                        //_s += "    </head>" + "\r\n";
                        //_s += "    <body>" + "\r\n";
                        //_s += "        <table  width=\"1200\">" + "\r\n";
                        //_s += "            <tr class=\"tr1\">" + "\r\n";
                        //_s += "                <td class=\"tdx\">" + "\r\n";
                        //_s += "☆☆☆☆　この行に説明を入力　☆☆☆☆" + "\r\n";
                        //_s += "                    <p>" + "\r\n";
                        //_s += "                       <img src=" + "\r\n";
                        //_s += "☆☆☆☆　この行に画像のリンク先を入力　☆☆☆☆" + "\r\n";
                        //_s += "                        alt=" + "\r\n";
                        //_s += "☆☆☆☆　この行に画像が表示できないときのメッセージを入力　☆☆☆☆" + "\r\n";
                        //_s += "                       width=\"100\" height=\"200\" ></p>" + "\r\n";
                        //_s += "                </td>" + "\r\n";
                        //_s += "                <td class=\"tdx\">" + "\r\n";
                        //_s += "☆☆☆☆　この行に説明を入力　☆☆☆☆" + "\r\n";
                        //_s += "                    <p>" + "\r\n";
                        //_s += "                       <img src=" + "\r\n";
                        //_s += "☆☆☆☆　この行に画像のリンク先を入力　☆☆☆☆" + "\r\n";
                        //_s += "                        alt=" + "\r\n";
                        //_s += "☆☆☆☆　この行に画像が表示できないときのメッセージを入力　☆☆☆☆" + "\r\n";
                        //_s += "                       width=\"100\" height=\"200\" ></p>" + "\r\n";

                        //_s += "<!DOCTYPE html>" + "\r\n";
                        //_s += "<html lang=\"ja\">" + "\r\n";
                        //_s += "<head>" + "\r\n";
                        //_s += "<meta charset=\"UTF-8\">" + "\r\n";
                        //_s += "</head>" + "\r\n";
                        //_s += "<table  width=\"600\">" + "\r\n";
                        //_s += "<tr class=\"tr1\">" + "\r\n";
                        //_s += "<td class=\"tdx\">" + "\r\n";
                        //_s += "                </td>" + "\r\n";
                        //_s += "                <td class=\"tdx\">" + "\r\n";
                        //_s += "☆☆☆☆　この行に説明を入力　☆☆☆☆" + "\r\n";
                        //_s += "                    <p>" + "\r\n";
                        //_s += "                       <img src=" + "\r\n";
                        //_s += "☆☆☆☆　この行に画像のリンク先を入力　☆☆☆☆" + "\r\n";
                        //_s += "                </td>" + "\r\n";
                        //_s += "                <td class=\"tdx\">" + "\r\n";
                        //_s += "☆☆☆☆　この行に説明を入力　☆☆☆☆" + "\r\n";
                        //_s += "                    <p>" + "\r\n";
                        //_s += "                       <img src=" + "\r\n";
                        //_s += "☆☆☆☆　この行に画像のリンク先を入力　☆☆☆☆" + "\r\n";
                        //_s += "                </td>" + "\r\n";
                        //_s += "                <td class=\"tdx\">" + "\r\n";
                        //_s += "☆☆☆☆　この行に説明を入力　☆☆☆☆" + "\r\n";
                        //_s += "                    <p>" + "\r\n";
                        //_s += "                       <img src=" + "\r\n";
                        //_s += "☆☆☆☆　この行に画像のリンク先を入力　☆☆☆☆" + "\r\n";
                        _s += "<!DOCTYPE html><html lang=\"ja\"><head><meta charset=\"UTF-8\"></head>" + "\r\n";
                        _s += "<body><table id=\"im\"><tr>" + "\r\n";
                        _s += "" + "\r\n";
                        _s += "<td><p>" + "\r\n";
                        _s += "☆☆☆☆この行に説明を入力☆☆☆☆" + "\r\n";
                        _s += "</p><div class=\"image-wrapper\"><img src=\"" + "\r\n";
                        _s += "☆☆☆☆この行に画像のリンク先を入力☆☆☆☆" + "\r\n";
                        _s += "\"></div></td>" + "\r\n";
                        _s += "" + "\r\n";
                        _s += "<td><p>" + "\r\n";
                        _s += "☆☆☆☆この行に説明を入力☆☆☆☆" + "\r\n";
                        _s += "</p><div class=\"image-wrapper\"><img src=\"" + "\r\n";
                        _s += "☆☆☆☆この行に画像のリンク先を入力☆☆☆☆" + "\r\n";
                        _s += "\"></div></td>" + "\r\n";
                        _s += "" + "\r\n";
                        _s += "</tr></table></body>" + "\r\n";
                        _s += "<style>table#im td   {padding: 0 15px;}.image-wrapper {width: 250px;  height: 250px; display: flex;justify-content: center;align-items: center;overflow: hidden;  }.image-wrapper img {width: 100%;height: 100%;object-fit: contain;}</style>" + "\r\n";
                        _s += "</html>" + "\r\n";
                        EditTextBox.Text = _s;
                        break;
                    }
                //サンプル表示
                case "sample":
                    {
                        string _s = "";
                        _s += "<a href=\"https://www.yazuya.com/\" target=\"_blank\">別画面を開いてやずやサイトへ飛びます。</a>" + "\r\n";
                        _s += "<p><font size=\"2\">このフォントサイズは2です。</font></p>" + "\r\n";
                        _s += "<p><font color=\"red\">このフォントの色は赤です。</font></p>" + "\r\n";
                        _s += "<br>を入力すると改行できます。" + "\r\n";

                        TxtMsgBox _txt = new(_s);
                        _txt.ShowDialog();

                        break;
                    }

            }


        }
    }
}
