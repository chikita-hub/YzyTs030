using System.Windows;


namespace Propla
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //MessageBox.Show("終了します。");// 終了時の処理をここに書く
            Propla.MainWindow.SetLoginEndingTime();

        }
    }


}
