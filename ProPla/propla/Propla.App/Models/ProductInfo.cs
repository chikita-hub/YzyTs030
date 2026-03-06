using System.Data;

namespace Propla
{
    /// <summary>
    /// ■■■■■　自動生成　■■■■■
    /// 各項目の情報を管理する
    /// </summary>
    public class ProductInfo
    {
        //DataTable
        DataTable dtProductInfo = new();
        public ProductInfo()
        {
            //カラムセット
            dtProductInfo.Columns.Add("プログラム用英名", typeof(string));
            dtProductInfo.Columns.Add("一般食品", typeof(bool));
            dtProductInfo.Columns.Add("栄養機能食品", typeof(bool));
            dtProductInfo.Columns.Add("機能性表示食品", typeof(bool));
            dtProductInfo.Columns.Add("ペットフード", typeof(bool));
            dtProductInfo.Columns.Add("化粧品", typeof(bool));
            dtProductInfo.Columns.Add("医薬部外品", typeof(bool));
            dtProductInfo.Columns.Add("医薬品", typeof(bool));
            dtProductInfo.Columns.Add("他部署閲覧権限", typeof(bool));
            dtProductInfo.Columns.Add("検索元", typeof(bool));
            dtProductInfo.Columns.Add("出力項目", typeof(bool));


            //データセット
            setData("ProductPlanningID", true, true, true, true, true, true, true, true, true, true);
            setData("ProductCategory", true, true, true, true, true, true, true, true, true, true);
            setData("ProductPerson", true, true, true, true, true, true, true, true, true, true);
            setData("ProductNameOfficial", true, true, true, true, true, true, true, true, true, true);
            setData("ProductNameRuby", true, true, true, true, true, true, true, true, true, true);
            setData("ProductNameOfficialE", true, true, true, true, true, true, true, true, true, true);
            setData("ProductNameOmit1", true, true, true, true, true, true, true, true, true, true);
            setData("ProductNameReceipt", true, true, true, true, true, true, true, true, false, true);
            setData("ProductReleaseStatus", true, true, true, true, true, true, true, true, true, true);
            setData("ProductTestReleaseDate", true, true, true, true, true, true, true, true, false, true);
            setData("ProductReleaseDate", true, true, true, true, true, true, true, true, false, true);
            setData("ProductAnniversary", true, true, true, true, true, true, true, true, false, false);
            setData("ProductRenewalHistory", true, true, true, true, true, true, true, true, true, true);
            setData("TrademarkAcquisitionList", true, true, true, true, true, true, true, true, false, false);
            setData("ProductWeight", true, true, true, true, true, true, true, true, false, true);
            setData("ProductForm", true, true, true, true, true, true, true, true, true, true);
            setData("JANCode", true, true, true, true, true, true, true, true, false, true);
            setData("StockID", true, true, true, true, true, true, true, true, false, true);
            setData("ShipmentID", true, true, true, true, true, true, true, true, false, true);
            setData("Sample", true, true, true, true, true, true, false, true, false, true);
            setData("SampleDetails", true, true, true, true, true, true, false, true, false, true);
            setData("SampleNumber", true, true, true, true, true, true, false, true, false, true);
            setData("ManufactureName", true, true, true, true, true, true, true, true, true, true);
            setData("FactoryNumber", true, true, true, false, false, false, false, true, false, true);
            setData("FactoryNumberList", true, true, true, false, false, false, false, false, false, false);
            setData("BestByDateJapan", true, true, true, true, false, false, false, true, false, true);
            setData("BestByDateOverseas", true, true, true, true, false, false, false, true, false, true);
            setData("DisintegrationTestResult", true, true, true, false, false, false, false, false, false, false);
            setData("MaterialOriginList", true, true, true, true, false, false, false, true, false, false);
            setData("MaterialOriginListLink", true, true, true, true, false, false, false, false, false, false);
            setData("ProductStandards", true, true, true, true, false, false, false, false, false, false);
            setData("ExpirationDate", false, false, false, false, true, true, true, true, false, true);
            setData("MaterialOriginList2", false, false, false, false, true, true, true, true, false, false);
            setData("MaterialOriginListLink2", false, false, false, false, true, true, true, false, false, false);
            setData("ProductStandards2", false, false, false, false, true, true, true, false, false, false);
            setData("FeaturesMaterial", true, true, true, true, true, true, true, false, true, true);
            setData("FeaturesStandard", true, true, true, true, true, true, true, false, false, false);
            setData("MaterialAllergens1", true, true, true, true, false, true, true, true, true, true);
            setData("MaterialAllergens2", true, true, true, true, false, true, true, true, true, true);
            setData("MaterialAttention", true, true, true, true, true, true, true, true, true, true);
            setData("MaterialAllergensOthers", true, true, true, true, false, true, true, true, true, true);
            setData("FactoryAllergens", true, true, true, true, false, true, true, true, true, true);
            setData("MaterialAttention2", true, true, true, true, true, true, true, true, true, true);
            setData("MaterialAllergensTalk", true, true, true, true, true, true, true, true, true, true);
            setData("ProductFeatures", true, true, true, true, true, true, true, true, true, true);
            setData("AdvertisingRequirements", true, true, true, true, true, true, true, true, true, true);
            setData("AdditiveFreeDisplay", true, true, true, true, true, true, false, true, true, true);
            setData("NGOKDisplay", true, true, true, true, true, true, true, true, false, false);
            setData("LatestInitialDocumentLink", true, true, true, true, true, true, true, true, false, false);
            setData("ProductStudyMaterials", true, true, true, true, true, true, true, true, false, false);
            setData("SampleSetAttention", true, true, true, true, false, false, false, true, false, true);
            setData("SampleSetAttentionLink", true, true, true, true, false, false, false, false, false, false);
            setData("TaxRate", true, true, true, true, true, true, true, true, false, true);
            setData("Dummy1", true, true, true, true, true, true, true, true, false, false);
            setData("GeneralPriceWithoutTax", true, true, true, true, true, true, true, true, false, true);
            setData("GeneralPriceWithoutTax2", true, true, true, true, true, true, true, true, false, true);
            setData("GeneralPriceWithoutTax3", true, true, true, true, true, true, true, true, false, true);
            setData("GeneralPrice", true, true, true, true, true, true, true, true, false, true);
            setData("GeneralPrice2", true, true, true, true, true, true, true, true, false, true);
            setData("GeneralPrice3", true, true, true, true, true, true, true, true, false, true);
            setData("RegularCoursePrice", true, true, true, true, true, true, true, true, false, true);
            setData("RegularCoursePrice2", true, true, true, true, true, true, false, true, false, true);
            setData("RegularCoursePrice3", true, true, true, true, true, true, false, true, false, true);
            setData("PriceDifference", true, true, true, true, true, true, true, true, false, true);
            setData("PriceDifference2", true, true, true, true, true, true, false, true, false, true);
            setData("PriceDifference3", true, true, true, true, true, true, false, true, false, true);
            setData("RegularCourseAttention", true, true, true, true, true, true, true, true, false, true);
            setData("PriceSystem", true, true, true, true, true, true, true, true, false, true);
            setData("PriceSystemAttention", true, true, true, true, true, true, true, true, false, true);
            setData("DeliveryFee", true, true, true, true, true, true, true, true, false, true);
            setData("DeliveryFeeSystem", true, true, true, true, true, true, true, true, false, true);
            setData("OrderAccepted", true, true, true, true, true, true, true, true, false, true);
            setData("Dummy2", true, true, true, true, true, true, true, false, false, false);
            setData("ProductCost", true, true, true, true, true, true, true, false, false, true);
            setData("GrossProfitMargin2", true, true, true, true, true, true, true, false, false, true);
            setData("ProductCost2", true, true, true, true, true, true, true, false, false, true);
            setData("GrossProfitMargin", true, true, true, true, true, true, true, false, false, true);
            setData("SaveQuotation", true, true, true, true, true, true, true, false, false, false);
            setData("ChangeHistory", true, true, true, true, true, true, true, false, false, true);
            setData("TransportationMethod", true, true, true, true, true, true, true, true, false, true);
            setData("DeliveryMethod", true, true, true, true, true, true, true, true, false, true);
            setData("FirstPackingGoods", true, true, true, true, true, true, true, true, false, true);
            setData("PackingMethod", true, true, true, true, true, true, true, true, false, true);
            setData("PackingAttention", true, true, true, true, true, true, true, true, false, true);
            setData("ProductLot", true, true, true, true, true, true, true, false, false, false);
            setData("ProductCalorie", true, true, true, true, false, true, false, true, false, true);
            setData("SpecifiedValueComponent", true, true, true, true, false, false, false, true, true, true);
            setData("EmphasisComponent", true, true, true, true, false, false, false, true, true, true);
            setData("FoodEquivalent", true, true, true, true, false, false, false, true, false, true);
            setData("FoodEquivalentLink", true, true, true, true, false, false, false, false, false, false);
            setData("NutrientsComparison", true, true, true, true, false, false, false, true, false, false);
            setData("NutrientsCertificate", true, true, true, true, false, false, false, true, false, false);
            setData("NutrientsCertificateLink", true, true, true, true, false, false, false, false, false, false);
            setData("PackageImage", true, true, true, true, true, true, true, true, false, false);
            setData("PackagePaperData", true, true, true, true, true, true, true, true, false, false);
            setData("PackagePhotoData", true, true, true, true, true, true, true, true, false, false);
            setData("PackageDesignCompany", true, true, true, true, true, true, true, true, false, true);
            setData("PackagePrintingCompany", true, true, true, true, true, true, true, true, false, true);
            setData("PackageLatestNumber", true, true, true, true, true, true, true, true, false, true);
            setData("PackageRevisionHistory", true, true, true, true, true, true, true, true, false, false);
            setData("ProductSwitchList", true, true, true, true, true, true, true, false, false, false);
            setData("PackageMaterialDisplay", true, true, true, true, true, true, true, true, false, true);
            setData("ShrinkWrap", true, true, true, true, true, true, true, true, false, true);
            setData("PackageSize", true, true, true, true, true, true, true, true, false, true);
            setData("EnclosedGoods", true, true, true, true, true, true, true, true, false, true);
            setData("PackageFoodName", true, true, true, true, false, false, false, true, true, true);
            setData("PackageMaterialName", true, true, true, true, false, false, false, true, true, true);
            setData("PackageProductWeight", true, true, true, true, false, false, false, true, false, true);
            setData("PackageBestByDate", true, true, true, true, false, false, false, true, false, true);
            setData("BestByDateDisplay", true, true, true, true, false, false, false, true, false, true);
            setData("BestByDateExampleOutside", true, true, true, true, false, false, false, true, false, true);
            setData("BestByDateExampleInside", true, true, true, true, false, false, false, true, false, true);
            setData("PackageStorageMethod", true, true, true, true, false, false, false, true, false, true);
            setData("PackageAfterOpening", true, true, true, true, false, false, false, true, false, true);
            setData("PackageCountryOrigin", true, true, true, true, false, false, false, true, false, true);
            setData("PackageSellerName", true, true, true, true, false, false, false, true, false, true);
            setData("PackageFactoryName", true, true, true, false, false, false, false, true, true, true);
            setData("PackageOthersMandatory", true, true, true, true, false, false, false, true, false, true);
            setData("PackageAttention", true, true, true, true, false, false, false, true, false, true);
            setData("PackageHowToEat", true, true, true, false, false, false, false, true, false, true);
            setData("PackageHowToFeed", false, false, false, true, false, false, false, true, false, true);
            setData("AmountFeedPerDay", false, false, false, true, false, false, false, true, false, true);
            setData("PackageNutritionalContent", true, true, true, true, false, false, false, true, false, true);
            setData("NutrientFunctionClaims", false, true, false, false, false, false, false, true, false, true);
            setData("NutrientFunctionAttention", false, true, false, false, false, false, false, true, false, true);
            setData("FunctionClaimsSubmitted", false, false, true, false, false, false, false, true, false, true);
            setData("FunctionClaimsMandatory", false, false, true, false, false, false, false, true, false, true);
            setData("FunctionClaimsOthersMandatory", false, false, true, false, false, false, false, true, false, true);
            setData("FunctionClaimsAttention", false, false, true, false, false, false, false, true, false, true);
            setData("DrugDateOfRevision", false, false, false, false, false, false, true, true, false, true);
            setData("DrugPackageInsertAttention", false, false, false, false, false, false, true, true, false, true);
            setData("DrugSalesName", false, false, false, false, true, true, true, true, false, true);
            setData("DrugMedicinalEffectName", false, false, false, false, false, false, true, true, false, true);
            setData("DrugRiskCategory", false, false, false, false, false, true, true, true, false, true);
            setData("CosmeCategory", false, false, false, false, true, true, false, true, false, true);
            setData("DrugWeight", false, false, false, false, true, true, true, true, false, true);
            setData("DrugProductFeatures", false, false, false, false, false, false, true, true, false, true);
            setData("DrugThingsImpossible", false, false, false, false, false, true, true, true, false, true);
            setData("DrugConsult", false, false, false, false, false, true, true, true, false, true);
            setData("DrugOthersMandatory", false, false, false, false, false, true, true, true, false, true);
            setData("DrugAdvertisingRequirements", false, false, false, false, true, true, true, true, false, true);
            setData("DrugEfficacy", false, false, false, false, false, true, true, true, false, true);
            setData("DrugDosage", false, false, false, false, false, true, true, true, false, true);
            setData("DrugDosageDetail", false, false, false, false, false, true, true, true, false, true);
            setData("DrugDosageAttention", false, false, false, false, false, true, true, true, false, true);
            setData("DrugIngredientQuantity", false, false, false, false, false, true, true, true, true, true);
            setData("DrugIngredientAttention", false, false, false, false, false, true, true, true, false, true);
            setData("CosmeAllIngredients", false, false, false, false, true, true, false, true, true, true);
            setData("CosmeUsing", false, false, false, false, true, true, false, true, false, true);
            setData("DrugHandlingAttention", false, false, false, false, true, true, true, true, false, true);
            setData("DrugManufacturerName", false, false, false, false, true, true, true, true, true, true);
            setData("DrugSellerName", false, false, false, false, true, true, true, true, true, true);
            setData("DrugContactInformation", false, false, false, false, true, true, true, true, false, true);
            setData("DrugSideEffectsInformation", false, false, false, false, false, false, true, true, false, true);
            setData("CosmeCountryOrigin", false, false, false, false, true, true, false, true, false, true);
            setData("DrugExpirationDateDisplay", false, false, false, false, true, true, true, true, false, true);
            setData("DrugExpirationDateExample", false, false, false, false, true, true, true, true, false, true);
            setData("DrugSerialNumberDateDisplay", false, false, false, false, true, true, true, true, false, true);
            setData("DrugSerialNumberExample", false, false, false, false, true, true, true, true, false, true);
            setData("DrugSpecificSales", false, false, false, false, false, false, true, true, false, true);
            setData("DrugOthersMandatory2", false, false, false, false, true, true, true, true, false, true);
            setData("PackageInformationRemarks", true, true, true, true, true, true, true, true, false, true);
            setData("SearchKeyword", true, true, true, true, true, true, true, true, true, true);
            setData("PopUpLink", false, false, false, false, false, false, false, true, true, true);

        }

        void setData(
            string itemName,
            bool p1,
            bool p2,
            bool p3,
            bool p4,
            bool p5,
            bool p6,
            bool p7,
            bool p8,
            bool p9,
            bool p10

            )
        {
            DataRow row = dtProductInfo.NewRow();
            row["プログラム用英名"] = itemName;
            row["一般食品"] = p1;
            row["栄養機能食品"] = p2;
            row["機能性表示食品"] = p3;
            row["ペットフード"] = p4;
            row["化粧品"] = p5;
            row["医薬部外品"] = p6;
            row["医薬品"] = p7;
            row["他部署閲覧権限"] = p8;
            row["検索元"] = p9;
            row["出力項目"] = p10;

            dtProductInfo.Rows.Add(row);
        }

        /// <summary>
        /// 項目を表示できるかの判断を行う。
        /// </summary>
        /// <param name="pItemName">項目名</param>
        /// <param name="pOrganizationId">商品部署</param>
        /// <param name="pProductDispType">商品表示種別</param>
        /// <returns>true:表示OK</returns>
        public bool IsDispVisible(string pItemName, string pOrganizationId, string pProductDispType)
        {
            for (int i = 0; i < dtProductInfo.Rows.Count; i++)
            {
                if (dtProductInfo.Rows[i]["プログラム用英名"].ToString().ToLower() == pItemName.ToLower())
                {
                    // 商品表示種別が×なので表示を行わない
                    if ((bool)(dtProductInfo.Rows[i][pProductDispType]) == false)
                    {
                        return false;
                    }
                    // 他部署閲覧OKであれば表示を行う
                    if ((bool)(dtProductInfo.Rows[i]["他部署閲覧権限"]) == true)
                    {
                        return true;
                    }
                    // 自部署の管理商品であれば表示を行う
                    if (MainWindow.GetOrganizationId() == pOrganizationId)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 項目をEXCELに出力できるかの判断を行う。
        /// </summary>
        /// <param name="pItemName">項目名</param>
        /// <returns>true:出力OK</returns>
        public bool IsOutExcelColum(string pItemName)
        {
            for (int i = 0; i < dtProductInfo.Rows.Count; i++)
            {
                if (dtProductInfo.Rows[i]["プログラム用英名"].ToString().ToLower() == pItemName.ToLower())
                {
                    // "出力項目"が×なので表示を行わない
                    if ((bool)(dtProductInfo.Rows[i]["出力項目"]) == false)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 項目を表示できるかの判断を行う。
        /// </summary>
        /// <param name="pItemName">項目名</param>
        /// <param name="pOrganizationId">商品部署</param>
        /// <returns>true:表示OK</returns>
        public bool IsOutExcelData(string pItemName, string pOrganizationId)
        {
            for (int i = 0; i < dtProductInfo.Rows.Count; i++)
            {
                if (dtProductInfo.Rows[i]["プログラム用英名"].ToString().ToLower() == pItemName.ToLower())
                {
                    // 他部署閲覧OKであれば表示を行う
                    if ((bool)(dtProductInfo.Rows[i]["他部署閲覧権限"]) == true)
                    {
                        return true;
                    }
                    // 自部署の管理商品であれば表示を行う
                    if (MainWindow.GetOrganizationId() == pOrganizationId)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public string SelectWhere()
        {
            bool header = true;
            string _Where = "";

            for (int i = 0; i < dtProductInfo.Rows.Count; i++)
            {
                // "検索元"が〇なので検索を行う
                if ((bool)(dtProductInfo.Rows[i]["検索元"]) == true)
                {
                    string itemName = dtProductInfo.Rows[i]["プログラム用英名"].ToString().ToLower();
                    if (header)
                    {
                        header = false;
                        _Where += "    ( " + itemName + " like :P1 ";
                    }
                    else
                    {
                        _Where += "  or " + itemName + " like :P1 ";
                    }
                }
            }
            // 有無 DataTable セット
            _Where += " or (exists(select 1 from m_divisions m1 where m1.division_id='004' and m1.configuration_value = sample               and m1.display_name_1 like :P1)) ";
            // 商品区分 DataTable セット
            _Where += " or (exists(select 1 from m_divisions m2 where m2.division_id='001' and m2.configuration_value = productcategory      and m2.display_name_1 like :P1)) ";
            // 企画状態 DataTable セット
            _Where += " or (exists(select 1 from m_divisions m3 where m3.division_id='002' and m3.configuration_value = productreleasestatus and m3.display_name_1 like :P1)) ";
            // TaxRate DataTable セット
            _Where += " or (exists(select 1 from m_divisions m4 where m4.division_id='012' and m4.configuration_value = taxrate              and m4.display_name_1 like :P1)) ";

            _Where += " ) ";
            return _Where;
        }

    }
}
