using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Propla
{
    /// <summary>
    /// StdListBox.xaml 
    /// AllDataTable ⇒ マスタ全部のDataTable
    /// SelDataTable ⇒ 選択しているDataTable
    /// </summary>
    public partial class StdListBox : Window
    {
        ListBox listBoxOut;
        ListBox listBoxIn;
        TextBox textBoxSer;

        public class listItem
        {
            public string Name { get; set; }
            public int SortNumber { get; set; }
        }

        private DataTable _AllListMstDt;

        private string _SelectDataStr;
        private string _MasterAddStr;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StdListBox()
        {
            InitializeComponent();
            _SelectDataStr = "";

        }


        public void SetRow(int pRow)
        {
            if (pRow == 1)
            {
                StackPanel2.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                PanelUpDown.Visibility = Visibility.Hidden;//表示しない＆スペースを使用する。
                Grid1.Height = 500;
            }
            else
            {
                StackPanel2.Visibility = Visibility.Visible;//表示
                PanelUpDown.Visibility = Visibility.Visible;
                Grid1.Height = 230;
            }
        }

        public string SelectDataStr
        {
            get { return _SelectDataStr; }
            set { _SelectDataStr = value; }
        }

        public string MasterAddStr
        {
            get { return _MasterAddStr; }
            set { _MasterAddStr = value; }
        }

        /// <summary>
        ///  左のListBoxに値をセットする。
        ///　以下のチェックを行いエラーがあればエラーメッセージを返却する。
        ///  ・文字チェック「・」が入っていればエラー
        ///　・重複チェック
        /// </summary>
        /// <param name="pDt"></param>
        /// <param name="pStr"></param>
        /// <returns></returns>
        public string ListBoxSet(int listNo, DataTable pAllDt, string pSelStr)
        {
            _MasterAddStr = "";
            switch (listNo)
            {
                case 1:
                    listBoxOut = listBoxOut1;
                    listBoxIn = listBoxIn1;
                    textBoxSer = TextBoxSer1;
                    break;
                case 2:
                    listBoxOut = listBoxOut2;
                    listBoxIn = listBoxIn2;
                    textBoxSer = TextBoxSer2;
                    break;
                default:
                    break;
            }
            return ListBoxSet(pAllDt, pSelStr);
        }
        public string ListBoxSet(DataTable pAllDt, string pSelStr)
        {
            string _errmsg = "";

            _AllListMstDt = pAllDt;
            //_SelectDataStr = pSelStr;

            //　重複チェック　マスタリストにすでに登録されていないかチェック
            //　　　　　　　　問題なければマスタデータからマスタリストに追加
            foreach (DataRow dr in _AllListMstDt.Rows)
            {
                foreach (var o in listBoxIn.Items)
                {
                    if (((listItem)o).Name == dr["name"].ToString())
                    {
                        _errmsg += "：重複エラー：" + dr["name"].ToString();
                    }
                }
                // ---------------------------
                // ListBox 左セット
                // ---------------------------
                listBoxIn.Items.Add(new listItem { Name = dr["name"].ToString(), SortNumber = (int)dr["seq"] });
            }
            //　選択なし状態{}
            if (pSelStr == "") { return _errmsg; }
            List<string> _words = ReadingPointToWords(pSelStr);
            //  選択文字列の項目がマスタリストに登録されていなければエラー
            //　　　　　　　　問題なければマスタリストから選択リストに移動
            foreach (var _word in _words)
            {
                Boolean _findFlg = false;
                int num = 0;
                foreach (var o in listBoxIn.Items)
                {
                    if (((listItem)o).Name == _word)
                    {
                        _findFlg = true;
                        break;
                    }
                    num++;
                }
                if (!_findFlg)
                {
                    _errmsg += "：エラー：選択文字列の項目がマスタリストに登録されていない：" + _word;
                    return _errmsg;
                }

                listItem item = (listItem)(listBoxIn.Items[num]);
                listBoxIn.Items.Remove(item);
                //listBoxIn.Items.Remove(num);

                listBoxOut.Items.Add(item);
            }
            return _errmsg;
        }


        void listBoxSort(ListBox pListBox)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(pListBox.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("PropertyName", ListSortDirection.Ascending));


        }

        public List<string> ReadingPointToWords(string pString)
        {
            int _BracketsCnt = 0;//カッコのカウント
            List<string> _Words = new();
            string _value = "";
            {
                ////////////////////////////
                //   内部関数　DataTable登録
                ////////////////////////////
                void listadd()
                {
                    if (_BracketsCnt != 0) { return; }
                    if (_value == "") { return; }

                    _Words.Add(_value);
                    _value = "";
                }
                string _s = pString;
                _s = _s.Replace(")", "）");
                _s = _s.Replace("(", "（");
                _s = _s.Replace(",", "・");
                for (int i = 0; i < _s.Length; i++)//一文字ごと確認
                {
                    string word = _s.Substring(i, 1);
                    switch (word)
                    {
                        case "（":
                            _BracketsCnt += 1;
                            _value += word;
                            break;
                        case "）":
                            _BracketsCnt -= 1;
                            _value += word;
                            if (i + 1 < _s.Length && _s.Substring(i + 1, 1) != "（")
                            {
                                listadd();
                            }
                            break;
                        case "・":
                            {
                                if (_BracketsCnt == 0)
                                {
                                    listadd();
                                }
                                else
                                {
                                    _value += word;
                                }
                                break;
                            }
                        default:
                            _value += word;
                            break;
                    }
                }
                listadd();
            }
            return _Words;
        }


        /// <summary>
        /// 商品区分 ComboBox セット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            string _name = btn.Name;
            int _no = 1;

            if (Int32.TryParse(btn.Name.Substring(btn.Name.Length - 1, 1), out int numValue))
            {
                _name = btn.Name.Substring(0, btn.Name.Length - 1);
                _no = numValue;
            }

            switch (_no)
            {
                case 1:
                    listBoxOut = listBoxOut1;
                    listBoxIn = listBoxIn1;
                    textBoxSer = TextBoxSer1;
                    break;
                case 2:
                    listBoxOut = listBoxOut2;
                    listBoxIn = listBoxIn2;
                    textBoxSer = TextBoxSer2;
                    break;
                default:
                    break;
            }

            switch (_name)
            {
                // TEXT 入力
                case "BtnTextBoxSer":
                    {
                        //カンマ区切りをリストに変換
                        List<string> _words = ReadingPointToWords(textBoxSer.Text.ToString());

                        string _s = "";
                        foreach (var _word in _words)
                        {
                            Boolean _findFlg = false;
                            //入力側の存在チェック
                            int num = 0;
                            foreach (listItem o in listBoxIn.Items)
                            {
                                if (o.Name == _word)
                                {
                                    _findFlg = true;
                                    listItem item = (listItem)listBoxIn.Items[num];
                                    listBoxIn.Items.Remove(o);

                                    if (StackPanel2.Visibility == Visibility.Visible)
                                    {//原材料・添加物
                                        listBoxOut.Items.Add(o);
                                    }
                                    else
                                    {//アレルゲン
                                        int i = insertNoGet(listBoxOut, o.SortNumber);
                                        listBoxOut.Items.Insert(i, o);
                                    }
                                    //listBoxOut.Items.Add(o);
                                    break;
                                }
                                num++;
                            }

                            //出力側の存在チェック
                            foreach (listItem o in listBoxOut.Items)
                            {

                                if (o.Name == _word)
                                {
                                    _findFlg = true;
                                    break;
                                }
                            }

                            if (!_findFlg)
                            {
                                _s += "・" + _word;
                            }

                        }
                        if (_s != "")
                        {
                            _s = _s.Substring(1);

                            if (StackPanel2.Visibility != Visibility.Visible)
                            {
                                //アレルゲンならマスタ登録しない
                                textBoxSer.Text = _s;
                                break;
                            }

                            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("「" + _s + "」がマスタに登録されていません、" +
                                "マスタに登録して良いでしょうか？", "マスタ登録", System.Windows.Forms.MessageBoxButtons.YesNo);
                            if (result == System.Windows.Forms.DialogResult.Yes)
                            {
                                // Yesが選択された場合の処理
                                _MasterAddStr = _no.ToString() + ":" + _s;
                                //登録値をセット
                                SetSelectDataStr();

                                DialogResult = false;
                            }
                            else if (result == System.Windows.Forms.DialogResult.No)
                            {
                                // Noが選択された場合の処理
                            }
                        }
                        textBoxSer.Text = _s;

                        break;
                    }
                // 右へボタン
                case "toRight":
                    {
                        // 選択項目が0 => メソッドを出る
                        if (listBoxIn.SelectedItems.Count == 0)
                            return;

                        toRight(listBoxIn, listBoxOut);

                        ////　選択したITEMを取り出す →　ListBoxを直接foreachすると
                        ////　ForEach文で中身の処理中に母集合側が変化するとMoveNextでエラーになる
                        //selList = new List<listItem>();
                        //foreach (listItem selItem in listBoxIn.SelectedItems)
                        //{
                        //    selList.Add(selItem);
                        //}
                        ////　左の選択した項目を右へ移動
                        //foreach (listItem selItem in selList)
                        //{
                        //    listBoxIn.Items.Remove(selItem);
                        //    listBoxOut.Items.Add(selItem);
                        //}

                        break;
                    }

                // 左へボタン
                case "toLeft":
                    {
                        if (listBoxOut.SelectedItems.Count == 0)
                            return;

                        toLeft(listBoxIn, listBoxOut);

                        //　選択したITEMを取り出す →　ListBoxを直接foreachすると
                        //　ForEach文で中身の処理中に母集合側が変化するとMoveNextでエラーになる
                        //selList = new List<listItem>();
                        //foreach (listItem selItem in listBoxOut.SelectedItems)
                        //{
                        //    selList.Add(selItem);
                        //}
                        ////　右の選択した項目を左へ移動
                        //foreach (listItem selItem in selList)
                        //{
                        //    listBoxOut.Items.Remove(selItem);
                        //    listBoxIn.Items.Add(selItem);
                        //}
                        break;
                    }
                //　上へ
                case "toUp":
                    {
                        int _pos = listBoxOut.SelectedIndex;
                        if (_pos < (0 + 1) || (listBoxOut.Items.Count - 1) < _pos)
                            return;

                        listItem _tmp = (listItem)listBoxOut.Items[_pos - 1];
                        listBoxOut.Items[_pos - 1] = listBoxOut.Items[_pos];
                        listBoxOut.Items[_pos] = _tmp;
                        listBoxOut.SelectedIndex = _pos - 1;

                        break;
                    }

                //　下へ
                case "toDown":
                    {
                        int _pos = listBoxOut.SelectedIndex;
                        if (_pos < 0 || (listBoxOut.Items.Count - 1) <= _pos)
                            return;

                        listItem _tmp = (listItem)listBoxOut.Items[_pos + 1];
                        listBoxOut.Items[_pos + 1] = listBoxOut.Items[_pos];
                        listBoxOut.Items[_pos] = _tmp;
                        listBoxOut.SelectedIndex = _pos + 1;

                        break;
                    }

                //　登録
                case "register":  //登録
                    {
                        //登録値をセット
                        SetSelectDataStr();

                        DialogResult = false;
                        break;
                    }
                ////　アイテム新規追加
                //case "BtnNewItem":
                //    {
                //        if (StackPanel2.Visibility != Visibility.Visible)
                //        {
                //            //アレルゲンならマスタ登録しない
                //            break;
                //        }

                //        TxtInputBox _txt = new();
                //        _txt.ShowDialog();
                //        if (_txt.MsgTxt != "")
                //        {
                //            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("「" + _txt.MsgTxt + "」をマスタに登録して良いでしょうか？", "マスタ登録", System.Windows.Forms.MessageBoxButtons.YesNo);
                //            if (result == System.Windows.Forms.DialogResult.Yes)
                //            {
                //                // Yesが選択された場合の処理
                //                _MasterAddStr = _no.ToString() +  ":" + _txt.MsgTxt;
                //                DialogResult = false;
                //            }
                //            else if (result == System.Windows.Forms.DialogResult.No)
                //            {
                //                // Noが選択された場合の処理
                //            }
                //        }
                //    }
                //    break;

                //　キャンセル
                case "cancel":
                    DialogResult = false;
                    break;

                default:
                    MessageBox.Show("Button Name 設定不正！！");
                    break;
            }
        }
        void toRight(ListBox listBoxIn, ListBox listBoxOut)
        {
            //　選択したITEMを取り出す →　ListBoxを直接foreachすると
            //　ForEach文で中身の処理中に母集合側が変化するとMoveNextでエラーになる
            List<listItem> selList = new List<listItem>();
            foreach (listItem selItem in listBoxIn.SelectedItems)
            {
                selList.Add(selItem);
            }
            //　左の選択した項目を右へ移動
            foreach (listItem selItem in selList)
            {
                listBoxIn.Items.Remove(selItem);
                if (StackPanel2.Visibility == Visibility.Visible)
                {//原材料・添加物
                    listBoxOut.Items.Add(selItem);
                }
                else
                {//アレルゲン
                    int i = insertNoGet(listBoxOut, selItem.SortNumber);
                    listBoxOut.Items.Insert(i, selItem);
                }
            }
        }

        void toLeft(ListBox listBoxIn, ListBox listBoxOut)
        {
            //　選択したITEMを取り出す →　ListBoxを直接foreachすると
            //　ForEach文で中身の処理中に母集合側が変化するとMoveNextでエラーになる
            List<listItem> selList = new List<listItem>();
            foreach (listItem selItem in listBoxOut.SelectedItems)
            {
                selList.Add(selItem);
            }
            //　右の選択した項目を左へ移動
            foreach (listItem selItem in selList)
            {
                listBoxOut.Items.Remove(selItem);
                if (StackPanel2.Visibility == Visibility.Visible)
                {//原材料・添加物
                    listBoxIn.Items.Add(selItem);
                }
                else
                {//アレルゲン
                    int i = insertNoGet(listBoxIn, selItem.SortNumber);
                    listBoxIn.Items.Insert(i, selItem);
                }
            }
        }

        //アレルゲンは順番が固定
        int insertNoGet(ListBox pListBox, int pNo)
        {
            for (int i = 0; i < pListBox.Items.Count; i++)
            {
                int sortNo = ((listItem)pListBox.Items[i]).SortNumber;
                if (pNo < sortNo)
                {
                    return i;
                }
            }
            return pListBox.Items.Count;
        }

        void SetSelectDataStr()
        {
            _SelectDataStr = "";
            string sout(ListBox l)
            {
                //Listをカンマ区切りに変換
                string _s = "";
                foreach (listItem selItem in l.Items)
                {
                    if (_s != "") { _s += "・"; }
                    _s += selItem.Name;
                }
                return _s;
            }
            string _o1 = sout(listBoxOut1);
            string _o2 = sout(listBoxOut2);
            if (_o1 != "")
            {
                _SelectDataStr = _o1;
            }
            if (_o2 != "")
            {
                _SelectDataStr = _SelectDataStr + "／" + _o2;
            }
        }
        public void SetDialogResult(bool pDialogResult)
        {
            DialogResult = pDialogResult;
        }

        private void ListBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string name = ((ListBox)sender).Name;

            switch (name)
            {
                case "listBoxIn1":
                    toRight(listBoxIn1, listBoxOut1);
                    break;
                case "listBoxIn2":
                    toRight(listBoxIn2, listBoxOut2);
                    break;
                case "listBoxOut1":
                    toLeft(listBoxIn1, listBoxOut1);
                    break;
                case "listBoxOut2":
                    toLeft(listBoxIn2, listBoxOut2);
                    break;
                default:
                    break;
            }


            var selectedItems = listBoxIn1.SelectedItems;
            //string message = "";

            //foreach (var item in selectedItems)
            //{
            //    if (item is YourDataType data)
            //    {
            //        message += $"{data.Name} {data.SortNumber}\\n";
            //    }
            //}

            //MessageBox.Show(message);
        }



    }
}
