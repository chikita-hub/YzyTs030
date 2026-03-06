using System;
using System.Data;
//using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Propla
{// データクラスの定義
    public class Account
    {
        public string UserId { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// proplaLogin.xaml の相互作用ロジック
    /// </summary>
    public partial class StdLogin : Window
    {
        private static PostgresDb _posdb;

        private string _ProgramId = "";
        private string _Server = "";

        private static string _UserId = "";
        private string _UserName = "";
        private string _MailAddress = "";

        private string _AccessAuthorityId = "";
        private string _OrganizationId = "";

        private string _StartTime = "";
        private DataTable _LoginDt;

        private string _LoginOk = "";

        string directoryPath = @"C:\YzyTs\temp";
        string filePath = @"C:\YzyTs\temp\account.json";


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StdLogin()
        {
            InitializeComponent();

            _LoginOk = "";

            try
            {

                bool directoryExists = DirectoryExists(directoryPath);
                if (!directoryExists)
                {
                    //ディレクトリが存在しないので作成
                    CreateDirectory(directoryPath);
                }
                bool fileExists = FileExists(filePath);
                if (fileExists)
                {
                    Console.WriteLine("ファイルが存在します");
                    LoadJsonFile();
                    login.Text = _UserId;
                    if (_UserId == "828")
                    {
                        pass.Password = _UserId;
                        BtnLogin.Focus();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        // ディレクトリを作成するためのメソッド
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        // ディレクトリの存在を確認するためのメソッド
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        // ファイルの存在を確認するためのメソッド
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
        // JSONファイルの作成
        public void CreateJsonFile(string pUserId)
        {
            // データを準備する
            Account account = new Account { UserId = pUserId, Name = "" };

            // JSONファイルに書き込む
            string jsonString = JsonSerializer.Serialize(account);
            File.WriteAllText(filePath, jsonString);
        }
        // JSONファイルの読み込み
        public void LoadJsonFile()
        {
            // ファイルからJSON文字列を読み込む
            string jsonString = File.ReadAllText(filePath);

            // JSON文字列からオブジェクトを復元する
            Account account = JsonSerializer.Deserialize<Account>(jsonString);

            // 読み込んだデータを使用する
            _UserId = account.UserId;
        }


        public string GetLoginID() { return _UserId; }
        public string GetLoginName() { return _UserName; }

        public string GetLoginOk() { return _LoginOk; }

        public string GetOrganizationId() { return _OrganizationId; }

        /// <summary>
        /// アクセス権限の確認
        /// </summary>
        /// <param name="pAuthority"></param>
        /// <returns></returns>
        public bool GetAccessPermission(string pAuthority, string pOrganization)
        {
            if (pOrganization == _OrganizationId)
            {
                if (Int32.Parse(_AccessAuthorityId) >= Int32.Parse(pAuthority))
                {
                    return true;
                }
            }
            return false;
        }

        public void SetLoginEndingTime()
        {
            if (_LoginDt is null)
            {
                return;
            }

            _LoginDt.Rows[0]["endingtime"] = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
            _posdb.DbUpdateAll(_LoginDt, "h_login", "programid,userid,starttime");
            return;
        }

        public void SetProgramId(string pProgramId)
        {
            _ProgramId = pProgramId;
            return;
        }

        public void SetServer(string pServer)
        {
            _Server = pServer;
            return;
        }

        /// <summary>
        /// ログインボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnClick(object sender, RoutedEventArgs e)
        {


            if (_ProgramId == "")
            {
                MessageBox.Show("システムエラープログラムID");
                return;
            }
            Button _btn = (Button)sender;

            switch (_btn.Name)
            {
                /// ログイン
                case "BtnLogin":
                    {
                        if (loginExecution())
                        {
                            _LoginOk = "OK";
                            DialogResult = false;
                        }
                        break;
                    }
                default:
                    MessageBox.Show("Buttonエラー：定義がない");
                    break;
            }

        }

        private string GetUser(string pUserId, string pPassword)
        {
            _posdb = new PostgresDb();
            //_posdb.DbOpen("Server=192.168.13.235; Port=5432; User Id=yzytscom;Password=yzytscom;Database=postgres");
            _posdb.DbOpen("Server=" + _Server + "; Port=5432; User Id=yzytscom;Password=yzytscom;Database=postgres");

            DataTable dataTable = new();
            _posdb.DbParClear();
            _posdb.DbParAdd("P1", pUserId, PostgresDb.STRING);
            string _Where = " userid = :P1 ";
            _posdb.DbSelectDataTable("*", "m_account", _Where, dataTable);
            if (dataTable.Rows.Count == 0)
            {
                return "ログインIDが登録されていません";
            }
            if (dataTable.Rows[0]["password"].ToString() != pPassword)
            {
                return "パスワードが異なります";
            }

            DataTable dataTable2 = new();
            _posdb.DbParClear();
            _posdb.DbParAdd("P1", pUserId, PostgresDb.STRING);
            _Where += " and programid = '" + _ProgramId + "'";
            _Where += " and accessauthorityid <> '00' ";

            _posdb.DbSelectDataTable("*", "m_accountauthority", _Where, dataTable2);
            if (dataTable2.Rows.Count == 0)
            {
                return "ログイン権限がありません";
            }
            //ログインOK
            _UserId = pUserId;
            _UserName = dataTable.Rows[0]["username"].ToString();
            _MailAddress = dataTable.Rows[0]["mailaddress"].ToString();

            _AccessAuthorityId = dataTable2.Rows[0]["accessauthorityid"].ToString();
            _OrganizationId = dataTable2.Rows[0]["organizationid"].ToString();

            //ログイン履歴
            _StartTime = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
            _LoginDt = new();
            _posdb.DbSelectDataTable("*", "h_login", " programid='' ", _LoginDt);
            _LoginDt.Rows.Add();
            _LoginDt.Rows[0]["programid"] = _ProgramId;
            _LoginDt.Rows[0]["userid"] = _UserId;
            _LoginDt.Rows[0]["starttime"] = _StartTime;
            _LoginDt.Rows[0]["endingtime"] = "";
            _posdb.DbInsertAll(_LoginDt, "h_login");



            return "";
        }
        public static DataTable GetAccessauthority(string pPgName)
        {
            DataTable _AccessauthorityDt = new();
            _posdb.DbParClear();
            string _Where = " programid = '" + pPgName + "'  ";

            _posdb.DbSelectDataTable("*", "m_accessauthority", _Where, _AccessauthorityDt);
            if (_AccessauthorityDt.Rows.Count != 0)
            {
                return _AccessauthorityDt;
            }
            return null;
        }

        public static DataTable GetAccountList(string pPgName)
        {
            DataTable _AccountDt = new();
            _posdb.DbParClear();
            string _Where = " userid <> '' AND  programid = '" + pPgName + "'  ";
            _posdb.DbSelectDataTable("*", "v_accountauthority", _Where, _AccountDt);
            if (_AccountDt.Rows.Count != 0)
            {
                return _AccountDt;
            }
            return null;
        }

        public static DataTable GetAccount(string pUserId)
        {
            string _ID = pUserId;
            if (_ID == "")
            {
                _ID = _UserId;
            }

            DataTable _AccountDt = new();
            _posdb.DbParClear();
            _posdb.DbParAdd("P1", _ID, PostgresDb.STRING);
            string _Where = " userid = :P1 ";
            _posdb.DbSelectDataTable("*", "m_account", _Where, _AccountDt);

            switch (_AccountDt.Rows.Count)
            {
                case 0:
                    _AccountDt.Rows.Add();
                    _AccountDt.Rows[0]["userid"] = _ID;
                    _AccountDt.Rows[0]["username"] = "";
                    _AccountDt.Rows[0]["password"] = "";
                    _AccountDt.Rows[0]["mailaddress"] = "";
                    _posdb.DbInsertAll(_AccountDt, "m_account");
                    return _AccountDt;
                case 1:
                    return _AccountDt;
                default:
                    return null;
            }
        }

        public static bool SetAccount(DataTable pAccountDt)
        {
            try
            {
                _posdb.DbUpdateAll(pAccountDt, "m_account", "userid");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetAccountauthority(string pProgramId, string pUserid, string pAccessauthorityid)
        {
            try
            {
                DataTable _AccountDt = new();
                _posdb.DbParClear();
                _posdb.DbParAdd("P1", pProgramId, PostgresDb.STRING);
                _posdb.DbParAdd("P2", pUserid, PostgresDb.STRING);
                string _Where = " programid = :P1 and userid = :P2 ";
                _posdb.DbSelectDataTable("*", "m_accountauthority", _Where, _AccountDt);

                switch (_AccountDt.Rows.Count)
                {
                    case 0:
                        _AccountDt.Rows.Add();
                        _AccountDt.Rows[0]["programid"] = pProgramId;
                        _AccountDt.Rows[0]["userid"] = pUserid;
                        _AccountDt.Rows[0]["accessauthorityid"] = pAccessauthorityid;
                        _AccountDt.Rows[0]["organizationid"] = MainWindow.GetOrganizationId();
                        _posdb.DbInsertAll(_AccountDt, "m_accountauthority");
                        return true;

                    case 1:
                        _AccountDt.Rows[0]["accessauthorityid"] = pAccessauthorityid;
                        _posdb.DbUpdateAll(_AccountDt, "m_accountauthority", "programid,userid");
                        return true;

                    default:
                        return false;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }



        bool loginExecution()
        {
            string userId = login.Text;
            var passwd = pass.Password;

            // 空チェック
            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("ログインIDを入力してください");
                return false;
            }
            if (string.IsNullOrEmpty(passwd) && userId != "80025")
            {
                MessageBox.Show("パスワードを入力してください");
                return false;
            }

            string msg = GetUser(userId, passwd);
            if (msg != "")
            {
                MessageBox.Show(msg);
                return false;
            }

            CreateJsonFile(userId);


            return true;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (loginExecution())
                {
                    _LoginOk = "OK";
                    DialogResult = false;
                }
            }
        }



        //private void TextBoxSizePreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    // 0-9のみ
        //    e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        //}
        //private void textBoxSize_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        //{
        //    // 貼り付けを許可しない
        //    if (e.Command == ApplicationCommands.Paste)
        //    {
        //        e.Handled = true;
        //    }
        //}


    }
}
