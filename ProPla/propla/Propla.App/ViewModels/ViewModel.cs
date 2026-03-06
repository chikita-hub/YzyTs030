using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Propla
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public class ViewModel : ObservableObject
    {
        private ObservableCollection<ProductPlanning> _Items = new();
        private ProductPlanning _Item = new();
        private ProductPlanningMst _ItemMst = new();
        private DataTable _ScreenDisplayDt;
        private string _StartTime;
        private PostgresDb _posdb;

        public ViewModel()
        {
            _posdb = new PostgresDb();
            _posdb.DbOpen("Server=" + MainWindow.Server + "; Port=5432; User Id=propla;Password=propla;Database=postgres");
            //_posdb.DbOpen("Server=192.168.14.243; Port=5432; User Id=propla;Password=propla;Database=postgres");

            _ItemMst.SetMst(_posdb);
        }

        public void SetScreenDisplayStartTime(string pProductPlanningid)
        {
            //表示履歴
            _StartTime = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
            _ScreenDisplayDt = new();
            _posdb.DbSelectDataTable("*", "h_screendisplay", " userid='' ", _ScreenDisplayDt);
            _ScreenDisplayDt.Rows.Add();
            _ScreenDisplayDt.Rows[0]["productplanningid"] = pProductPlanningid;
            _ScreenDisplayDt.Rows[0]["userid"] = MainWindow.GetLoginID();
            _ScreenDisplayDt.Rows[0]["starttime"] = _StartTime;
            _ScreenDisplayDt.Rows[0]["updatetime"] = "";
            _ScreenDisplayDt.Rows[0]["endingtime"] = "";
            _posdb.DbInsertAll(_ScreenDisplayDt, "h_screendisplay");
        }
        public void SetScreenDisplayEndingTime()
        {
            if (_ScreenDisplayDt is null)
            {
                return;
            }
            _ScreenDisplayDt.Rows[0]["endingtime"] = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
            _posdb.DbUpdateAll(_ScreenDisplayDt, "h_screendisplay", "productplanningid,userid,starttime");
            return;
        }
        public string SetScreenDisplayUpdateTime()
        {
            if (_ScreenDisplayDt is null)
            {
                return "";
            }
            string ppid = _ScreenDisplayDt.Rows[0]["productplanningid"].ToString();
            if (ppid != "")
            {
                DataTable dt = new();
                _posdb.DbParClear();
                _posdb.DbParAdd("P1", ppid, PostgresDb.STRING);
                string _Where = " productplanningid = :P1 and updatetime <>'' and endingtime='' "; //ID
                _posdb.DbSelectDataTable("*", "h_screendisplay", _Where, dt);
                if (dt.Rows.Count != 0)
                {
                    return "他の方が更新モードで表示しています。[ログインID：" + dt.Rows[0]["userid"] + "]";
                }
            }

            _ScreenDisplayDt.Rows[0]["updatetime"] = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
            _posdb.DbUpdateAll(_ScreenDisplayDt, "h_screendisplay", "productplanningid,userid,starttime");
            return "";
        }

        public bool SetScreenDisplayLockRelease(string pProductPlanningid, string pUserid, string pStarttime)
        {
            DataTable dataTable = new();

            _posdb.DbParClear();
            _posdb.DbParAdd("P1", pProductPlanningid, PostgresDb.STRING);
            _posdb.DbParAdd("P2", pUserid, PostgresDb.STRING);
            _posdb.DbParAdd("P3", pStarttime, PostgresDb.STRING);
            string _Where = " productplanningid = :P1 AND Userid = :P2 AND Starttime = :P3 "; //ID
            _posdb.DbSelectDataTable("*", "h_screendisplay", _Where, dataTable);

            if (dataTable.Rows.Count != 1)
            {
                return false;
            }
            dataTable.Rows[0]["endingtime"] = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
            _posdb.DbUpdateAll(dataTable, "h_screendisplay", "productplanningid,userid,starttime");

            return true;
        }


        public int IdsDataSelect(string pID, string pProductReleaseStatus)
        {
            DataTable dataTable = new();
            _posdb.DbParClear();
            _posdb.DbParAdd("P1", "%" + pID + "%", PostgresDb.STRING);

            string _Table = "m_productplanning";

            string _Where = MainWindow.SelectWhere();
            //_Where += "   (productplanningid like :P1 "; //ID
            //_Where += " or ProductPerson like :P1 "; //商品担当者
            //_Where += " or ProductNameOfficial like :P1 "; //正式商品名
            //_Where += " or ProductNameRuby like :P1 "; //読み仮名
            //_Where += " or ProductNameOmit1 like :P1 "; //略称
            ////_Where += " or JANCode like :P1 "; //"JANコード【外装／個包装】"
            ////_Where += " or StockID like :P1 "; //在庫ID
            //_Where += " or ManufactureName like :P1 "; //製造メーカー名
            //_Where += " or MaterialAllergens1 like :P1 "; //"原材料アレルゲン(特定原材料)"
            //_Where += " or MaterialAllergens2 like :P1 "; //"原材料アレルゲン(準ずるもの)"
            //_Where += " or MaterialAllergensOthers like :P1 "; //"原料収獲時等のアレルゲン"
            //_Where += " or FactoryAllergens like :P1 "; //"製造ラインのアレルゲン"
            //_Where += " or MaterialAttention like :P1 "; //"注意が必要な素材(アレルゲン)"
            //_Where += " or MaterialAttention2 like :P1 "; //"注意が必要な素材(アレルゲン以外)"
            //_Where += " or AdditiveFreeDisplay like :P1 "; //無添加表示
            //_Where += " or PackageFoodName like :P1 "; //名称
            //_Where += " or PackageMaterialName like :P1 "; //原材料名
            //_Where += " or PackageFactoryName like :P1 "; //製造所・加工所
            //_Where += " or SearchKeyword like :P1 "; //検索キーワード
            //_Where += " or PopUpLink like :P1 "; //PopUpLink

            ////_Where += " or Sample like :P1 "; //サンプルの有無
            ////_Where += " or ProductCategory like :P1 "; //商品区分
            ////_Where += " or ProductReleaseStatus like :P1 "; //発売の状態
            ////_Where += " or TaxRate like :P1 "; //価格の消費税率

            //// 有無 DataTable セット
            //_Where += " or (exists(select 1 from m_divisions m1 where m1.division_id='004' and m1.configuration_value = sample               and m1.display_name_1 like :P1)) ";
            //// 商品区分 DataTable セット
            //_Where += " or (exists(select 1 from m_divisions m2 where m2.division_id='001' and m2.configuration_value = productcategory      and m2.display_name_1 like :P1)) ";
            //// 企画状態 DataTable セット
            //_Where += " or (exists(select 1 from m_divisions m3 where m3.division_id='002' and m3.configuration_value = productreleasestatus and m3.display_name_1 like :P1)) ";
            //// TaxRate DataTable セット
            //_Where += " or (exists(select 1 from m_divisions m4 where m4.division_id='012' and m4.configuration_value = taxrate              and m4.display_name_1 like :P1)) ";

            //_Where += " ) ";
            if (pProductReleaseStatus != "")
            {
                _Where += " and ProductReleaseStatus='" + pProductReleaseStatus + "' ";
            }
            _Where += " and productreleasestatus<>'99' ";
            _Where += " order by productplanningid";
            _posdb.DbSelectDataTable("*", _Table, _Where, dataTable);
            Items.Clear();
            DataTableEntities.DataTableToEntities(dataTable, Items);

            if (Items.Count == 1)
            {
                Item = Items[0];
            }
            return Items.Count;
        }


        public int IdsDataSelectHistory(string pID)
        {
            DataTable dataTable = new();
            _posdb.DbParClear();
            _posdb.DbParAdd("P1", pID, PostgresDb.STRING);
            string _Where = " productplanningid = :P1 order by version"; //ID
            _posdb.DbSelectDataTable("*", "v_ProductPlanningHistory", _Where, dataTable);
            Items.Clear();
            DataTableEntities.DataTableToEntities(dataTable, Items);

            if (Items.Count == 1)
            {
                Item = Items[0];
            }
            return Items.Count;
        }



        public void IdDataNew()
        {
            ProductPlanning _i = new();
            Items.Add(_i);
            Item = Items[0];
            return;
        }
        //public int IdDataSelect(string pID)
        //{
        //    DataTable dataTable = new();
        //    _posdb.DbParClear();
        //    _posdb.DbParAdd("P1", pID, PostgresDb.STRING);
        //    string _Where = " productplanningid = :P1 "; //ID
        //    _posdb.DbSelectDataTable("*", "m_productplanning", _Where, dataTable);
        //    Items.Clear();

        //    DataTableEntities.DataTableToEntities(dataTable, Items);

        //    Item = DataTableEntities.DataTableToEntitie(dataTable, Items);
        //    if (Item is null)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return 1;
        //    }
        //}

        public int IdDataSelect(string pID)
        {
            DataTable dataTable = new();
            _posdb.DbParClear();
            _posdb.DbParAdd("P1", pID, PostgresDb.STRING);
            string _Where = " productplanningid = :P1 "; //ID
            _posdb.DbSelectDataTable("*", "m_productplanning", _Where, dataTable);
            Items.Clear();
            DataTableEntities.DataTableToEntities(dataTable, Items);

            if (Items.Count == 1)
            {
                Item = Items[0];
            }
            return Items.Count;
        }

        public string GetAutoProductPlanningid(string pOrganizationid, string pPrefix)
        {
            DataTable _autoProductPlanningid = new();

            // 商品管理ID
            string where = " productcategory in (select configuration_value from m_divisions where division_id='001' and extend_field_1='%%%') ";
            where = where.Replace("%%%", pOrganizationid);
            where += " order by productplanningid desc ";
            _posdb.DbSelectDataTable("productplanningid", "m_productplanning", where, _autoProductPlanningid);

            string s = "";
            if (_autoProductPlanningid.Rows.Count > 0)
            {
                s = _autoProductPlanningid.Rows[0]["productplanningid"].ToString();
                if (pPrefix.Length > 0)
                {
                    s = s.Replace(pPrefix, "");
                }

                try
                {
                    int i = int.Parse(s) + 1;
                    s = i.ToString("000");
                }
                catch (System.Exception)
                {
                    s = "999";
                }
            }
            else
            {
                s = "001";
            }
            return pPrefix + s;
        }


        //public string getAutoProductPlanningid()
        //{
        //    DataTable _autoProductPlanningid = new();

        //    // 商品管理ID
        //    string where = " productplanningid >=(select configuration_value from m_divisions where division_id = '014') ";
        //    where += " and length(productplanningid)=3";
        //    where += " order by productplanningid ";
        //    _posdb.DbSelectDataTable("productplanningid", "m_productplanning", where, _autoProductPlanningid);

        //    DataTable dtDivisions014 = new();
        //    _posdb.DbSelectDataTable("*", "m_divisions", " division_id='014' ", dtDivisions014);
        //    string s = "";

        //    if (_autoProductPlanningid.Rows.Count > 0)
        //    {
        //        s = _autoProductPlanningid.Rows[0]["productplanningid"].ToString();
        //    }
        //    else
        //    {
        //        s = dtDivisions014.Rows[0]["configuration_value"].ToString();
        //    }

        //    try
        //    {
        //        for (int i = int.Parse(s) + 1; i < 999; i++)
        //        {
        //            s = i.ToString("000");

        //            var query = _autoProductPlanningid.AsEnumerable()
        //                                            .Where(row => row.Field<string>("productplanningid") == s)
        //                                            .FirstOrDefault();
        //            if (query == null)
        //            {
        //                break;
        //            }

        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        s = "";
        //    }
        //    dtDivisions014.Rows[0]["configuration_value"] = s;
        //    _posdb.DbUpdateAll(dtDivisions014, "m_divisions", "division_id");


        //    return s;
        //}



        //新規登録
        public void IdDataInsert()
        {
            DataTable dataTable = new();
            DataTableEntities.EntitiesToDataTable(Items, dataTable);
            _posdb.DbInsertAll(dataTable, "m_productplanning");
        }
        public DataTable GetUpdatedatatable()
        {
            DataTable updatedataTable = new();
            string _Where = " productplanningid = '9999' and version = 9999 ";
            _posdb.DbSelectDataTable("*", "h_UpdateColumn", _Where, updatedataTable);
            return updatedataTable;
        }
        //更新登録
        public void IdDataUpdate(DataTable pUpdatedatatable)
        {
            DataTable dataTable = new();
            DataTableEntities.EntitiesToDataTable(Items, dataTable);

            if ((int)(dataTable.Rows[0]["version"]) != 1)
            {
                //データ更新
                _posdb.DbUpdateAll(dataTable, "m_productplanning", "productplanningid");
            }

            //履歴作成
            _posdb.DbSelectInsert(dataTable, "m_productplanning", "h_productplanning", "productplanningid");

            //更新カラムデータ作成
            _posdb.DbInsertAll(pUpdatedatatable, "h_UpdateColumn");


        }

        public DataTable GetColumnList()
        {
            DataTable dtColumnComment = new();
            _posdb.DbSelectDataTable("column_name,comment", "v_ColumnComment", " table_name='m_productplanning' ", dtColumnComment);
            return dtColumnComment;
        }

        public DataTable GetHistoryDetail(string pProductPlanningId, int pVersion)
        {
            DataTable dtCurrent = new();
            DataTable dtBefor = new();
            DataTable dtAfter = new();
            DataTable dtUpdateColumn = new();
            DataTable dtColumnComment = new();
            _posdb.DbSelectDataTable("*", "v_ColumnComment", " table_name='m_productplanning' ", dtColumnComment);

            _posdb.DbParClear();
            _posdb.DbParAdd("P1", pProductPlanningId, PostgresDb.STRING);
            _posdb.DbParAdd("P2", Convert.ToString(pVersion), PostgresDb.INT32);
            _posdb.DbSelectDataTable("*", "m_productplanning", " productplanningid = :P1 ", dtCurrent);

            if ((int)(dtCurrent.Rows[0]["version"]) == pVersion)
            {
                dtAfter = dtCurrent;
            }
            else
            {
                _posdb.DbSelectDataTable("*", "h_productplanning", " productplanningid = :P1 and version = :P2", dtAfter);
            }
            _posdb.DbSelectDataTable("columnname", "h_updatecolumn", " productplanningid = :P1 and version = :P2", dtUpdateColumn);

            _posdb.DbParClear();
            _posdb.DbParAdd("P1", pProductPlanningId, PostgresDb.STRING);
            _posdb.DbParAdd("P2", Convert.ToString(pVersion - 1), PostgresDb.INT32);
            _posdb.DbSelectDataTable("*", "h_productplanning", " productplanningid = :P1 and version = :P2", dtBefor);

            dtUpdateColumn.Columns.Add("jname", typeof(string));
            dtUpdateColumn.Columns.Add("befor", typeof(string));
            dtUpdateColumn.Columns.Add("after", typeof(string));

            for (int i = 0; i < dtUpdateColumn.Rows.Count; i++)
            {
                if (dtBefor.Rows.Count == 0)
                {
                    dtUpdateColumn.Rows[i]["befor"] = "";
                }
                else
                {
                    dtUpdateColumn.Rows[i]["befor"] = dtBefor.Rows[0][(string)(dtUpdateColumn.Rows[i]["columnname"])];
                }

                dtUpdateColumn.Rows[i]["after"] = dtAfter.Rows[0][(string)(dtUpdateColumn.Rows[i]["columnname"])];
                var query1 = dtColumnComment.AsEnumerable()
                            .FirstOrDefault(x => x["column_name"].ToString().ToLower() == (string)(dtUpdateColumn.Rows[i]["columnname"]).ToString().ToLower());
                if (!(query1 is null))
                {
                    dtUpdateColumn.Rows[i]["jname"] = query1["comment"].ToString();
                }
            }
            return dtUpdateColumn;
        }
        public int CheckColumnComment()
        {
            DataTable dtColumnComment = new();
            _posdb.DbSelectDataTable("*", "v_ColumnComment", " table_name='m_productplanning' ", dtColumnComment);
            return dtColumnComment.Rows.Count;
        }

        public DataTable GetScreenDisplay(bool pUpdating)
        {
            string where = "  商品ID is not null ";
            if (pUpdating)
            {
                where += " and  表示モード='★更新中★'";
            }

            DataTable dtScreenDisplay = new();
            _posdb.DbSelectDataTable("*", "v_ScreenDisplay", where, dtScreenDisplay);

            return dtScreenDisplay;
        }

        // 原材料or添加物 追加
        public void AddPackageMaterialName(DataTable pAddDt)
        {
            // 原材料or添加物 追加
            _posdb.DbInsertAll(pAddDt, "v_divisions");
            // 原材料 DataTable セット
            _ItemMst.MstPackageMaterialNameDt1 = new();
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '003' ", _ItemMst.MstPackageMaterialNameDt1);
            // 添加物 DataTable セット
            _ItemMst.MstPackageMaterialNameDt2 = new();
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '010' ", _ItemMst.MstPackageMaterialNameDt2);
        }

        public string GetMstConv(string inName, string inValue)
        {
            string outText = inValue;
            System.Data.DataTable dt;

            if (outText == "")
            {
                //空白は処理しない
                return outText;
            }

            try
            {
                switch (inName)
                {
                    case "ProductCategory":
                        dt = ItemMst.MstProductCategoryDt;
                        break;
                    case "ProductReleaseStatus":
                        dt = ItemMst.MstProductReleaseStatusDt;
                        break;
                    case "OverseasSales":
                        dt = ItemMst.MstPresenceOrAbsenceDt;
                        break;
                    case "TrademarkAcquisition":
                        dt = ItemMst.MstTrademarkAcquisitionDt;
                        break;
                    case "Sample":
                        dt = ItemMst.MstSampleDt;
                        break;
                    case "DisintegrationTestPass":
                        dt = ItemMst.MstDisintegrationTestPassDt;
                        break;
                    case "TaxRate":
                        dt = ItemMst.MstTaxRateDt;
                        break;
                    case "PriceSystem":
                        dt = ItemMst.MstPriceSystemDt;
                        break;
                    case "OrderAccepted":
                        dt = ItemMst.MstPresenceOrAbsenceDt;
                        break;
                    case "TransportationMethod":
                        dt = ItemMst.MstTransportationMethodDt;
                        break;
                    case "PackingMethod":
                        dt = ItemMst.MstPackingMethodDt;
                        break;
                    case "ShrinkWrap":
                        dt = ItemMst.MstPresenceOrAbsenceDt;
                        break;
                    default:
                        return outText;
                }

                var query2 = ((System.Data.DataTable)dt).AsEnumerable()
                    .FirstOrDefault(x => x["value"].ToString() == inValue)
                    ;
                if (query2 is null)
                {
                    return "$$$Error$$$  Mst" + inName + "Dt" + "が見つかりません：" + inValue;
                }
                return (string)query2["name"];

            }
            catch (System.Exception)
            {
                throw;
            }

        }

        public DataTable GetUpdateDivisions()
        {
            string where = " division_id in('007','016','017') ";
            DataTable dtDivisions = new();
            _posdb.DbSelectDataTable(" distinct division_id,division_name ", "m_divisions", where, dtDivisions);

            return dtDivisions;
        }

        public bool SaveDivisions(string id, string name, string value, string disp, int seq)
        {
            string setValue = value;
            if (setValue == "")
            {
                string select = " division_id,division_name,max(configuration_value) configuration_value,max(display_name_1) display_name_1,max(seq) seq  ";
                string where = " division_id in('" + id + "') group by division_id,division_name ";

                DataTable dtDivisions = new();
                _posdb.DbSelectDataTable(select, "m_divisions", where, dtDivisions);
                setValue = dtDivisions.Rows[0]["configuration_value"].ToString();
                int num = int.Parse(setValue);
                num += 1;
                setValue = num.ToString();
                // 
                dtDivisions.Rows[0]["configuration_value"] = setValue;
                dtDivisions.Rows[0]["display_name_1"] = disp;
                dtDivisions.Rows[0]["seq"] = seq;

                _posdb.DbInsert(dtDivisions, "division_id,division_name,configuration_value,display_name_1,seq", "m_divisions");
            }
            else
            {
                string select = " division_id,division_name,configuration_value,display_name_1,seq  ";
                string where = " division_id in('" + id + "') and configuration_value ='" + value + "' ";

                DataTable dtDivisions = new();
                _posdb.DbSelectDataTable(select, "m_divisions", where, dtDivisions);
                // 
                where = "division_id,configuration_value";
                dtDivisions.Rows[0]["display_name_1"] = disp;
                dtDivisions.Rows[0]["seq"] = seq;
                _posdb.DbUpdate(dtDivisions, where + ",display_name_1,seq", "m_divisions", where);

            }

            return true;
        }

        /// <summary>
        /// プロパティ　Items
        /// </summary>
        public ObservableCollection<ProductPlanning> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                SetProperty(ref _Items, value, nameof(Items));
            }
        }
        /// <summary>
        /// プロパティ　Item
        /// </summary>
        public ProductPlanning Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetProperty(ref _Item, value, nameof(Item));
            }
        }
        public ProductPlanningMst ItemMst
        {
            get
            {
                return _ItemMst;
            }
            set
            {
                SetProperty(ref _ItemMst, value, nameof(ItemMst));
            }
        }
    }

    /// <summary>
    /// ProductPlanningMst class の概要。
    /// 自動生成されたXMLドキュメント（2025-10-24）。
    /// </summary>
    public class ProductPlanningMst : ObservableObject
    {
        private DataTable _MstProductCategoryDt = new();
        private DataTable _MstProductReleaseStatusDt = new();
        private DataTable _MstPackageMaterialNameDt1 = new();
        private DataTable _MstPackageMaterialNameDt2 = new();
        private DataTable _MstPresenceOrAbsenceDt = new();
        private DataTable _MstTrademarkAcquisitionDt = new();
        private DataTable _MstMaterialAllergensDt = new();
        private DataTable _MstMaterialAllergens1Dt = new();
        private DataTable _MstMaterialAllergens2Dt = new();
        private DataTable _MstPriceSystemDt = new();
        private DataTable _MstTransportationMethodDt = new();
        private DataTable _MstOkOrNgDt = new();
        private DataTable _MstDisintegrationTestPassDt = new();
        private DataTable _MstTaxRateDt = new();
        private DataTable _MstOutTemplateDt = new();
        private DataTable _MstPackingMethodDt = new();
        private DataTable _MstSampleDt = new();

        /// posgres DB 
        PostgresDb _posdb;

        public void SetMst(PostgresDb pPosdb)
        {
            _posdb = pPosdb;

            // 商品区分 DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '001' ", _MstProductCategoryDt);
            // 企画状態 DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '002' ", _MstProductReleaseStatusDt);
            // 原材料 DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '003' ", _MstPackageMaterialNameDt1);
            // 添加物 DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '010' ", _MstPackageMaterialNameDt2);
            // 有無 DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '004' ", _MstPresenceOrAbsenceDt);
            // 商品名商標取得 DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '005' ", _MstTrademarkAcquisitionDt);
            // 原材料アレルゲン DataTable セット 8個
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '006' and extend_field_1 in('*','1') ", _MstMaterialAllergens1Dt);
            // 原材料アレルゲン DataTable セット 20個
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '006' and extend_field_1 in('*','2') ", _MstMaterialAllergens2Dt);
            // 原材料アレルゲン DataTable セット 28個
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '006' ", _MstMaterialAllergensDt);
            // 価格システム DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '007' ", _MstPriceSystemDt);
            // 運送方法 DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '008' ", _MstTransportationMethodDt);
            // OKorNG DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '009' ", _MstOkOrNgDt);
            // DisintegrationTestPass DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '011' ", _MstDisintegrationTestPassDt);
            // TaxRate DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '012' ", _MstTaxRateDt);
            // OutTemplate DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '015' ", _MstOutTemplateDt);
            // PackingMethod DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '016' ", _MstPackingMethodDt);
            // Sample DataTable セット
            _posdb.DbSelectDataTable("*", "v_divisions", " ID = '017' ", _MstSampleDt);

        }
        public DataTable MstProductCategoryDt
        {
            get { return _MstProductCategoryDt; }
            set { SetProperty(ref _MstProductCategoryDt, value, nameof(MstProductCategoryDt)); }
        }
        public DataTable MstPackageMaterialNameDt1
        {
            get { return _MstPackageMaterialNameDt1; }
            set { SetProperty(ref _MstPackageMaterialNameDt1, value, nameof(MstPackageMaterialNameDt1)); }
        }
        public DataTable MstPackageMaterialNameDt2
        {
            get { return _MstPackageMaterialNameDt2; }
            set { SetProperty(ref _MstPackageMaterialNameDt2, value, nameof(MstPackageMaterialNameDt2)); }
        }
        public DataTable MstProductReleaseStatusDt
        {
            get { return _MstProductReleaseStatusDt; }
            set { SetProperty(ref _MstProductReleaseStatusDt, value, nameof(MstProductReleaseStatusDt)); }
        }
        public DataTable MstPresenceOrAbsenceDt
        {
            get { return _MstPresenceOrAbsenceDt; }
            set { SetProperty(ref _MstPresenceOrAbsenceDt, value, nameof(MstPresenceOrAbsenceDt)); }
        }
        public DataTable MstTrademarkAcquisitionDt
        {
            get { return _MstTrademarkAcquisitionDt; }
            set { SetProperty(ref _MstTrademarkAcquisitionDt, value, nameof(MstTrademarkAcquisitionDt)); }
        }
        public DataTable MstMaterialAllergens1Dt
        {
            get { return _MstMaterialAllergens1Dt; }
            set { SetProperty(ref _MstMaterialAllergens1Dt, value, nameof(MstMaterialAllergens1Dt)); }
        }
        public DataTable MstMaterialAllergens2Dt
        {
            get { return _MstMaterialAllergens2Dt; }
            set { SetProperty(ref _MstMaterialAllergens2Dt, value, nameof(MstMaterialAllergens2Dt)); }
        }
        public DataTable MstMaterialAllergensDt
        {
            get { return _MstMaterialAllergensDt; }
            set { SetProperty(ref _MstMaterialAllergensDt, value, nameof(MstMaterialAllergensDt)); }
        }
        public DataTable MstPriceSystemDt
        {
            get { return _MstPriceSystemDt; }
            set { SetProperty(ref _MstPriceSystemDt, value, nameof(MstPriceSystemDt)); }
        }
        public DataTable MstTransportationMethodDt
        {
            get { return _MstTransportationMethodDt; }
            set { SetProperty(ref _MstTransportationMethodDt, value, nameof(MstTransportationMethodDt)); }
        }
        public DataTable MstPackingMethodDt
        {
            get { return _MstPackingMethodDt; }
            set { SetProperty(ref _MstPackingMethodDt, value, nameof(MstPackingMethodDt)); }
        }
        public DataTable MstDisintegrationTestPassDt
        {
            get { return _MstDisintegrationTestPassDt; }
            set { SetProperty(ref _MstDisintegrationTestPassDt, value, nameof(MstDisintegrationTestPassDt)); }
        }
        public DataTable MstTaxRateDt
        {
            get { return _MstTaxRateDt; }
            set { SetProperty(ref _MstTaxRateDt, value, nameof(MstTaxRateDt)); }
        }
        public DataTable MstOutTemplateDt
        {
            get { return _MstOutTemplateDt; }
            set { SetProperty(ref _MstOutTemplateDt, value, nameof(MstOutTemplateDt)); }
        }
        public DataTable MstSampleDt
        {
            get { return _MstSampleDt; }
            set { SetProperty(ref _MstSampleDt, value, nameof(MstSampleDt)); }
        }


        //インデクサの定義
        public object this[string propertyName]
        {
            get
            {
                return typeof(ProductPlanningMst).GetProperty(propertyName).GetValue(this);
            }
            set
            {
                typeof(ProductPlanningMst).GetProperty(propertyName).SetValue(this, value);
            }
        }
    }
}
