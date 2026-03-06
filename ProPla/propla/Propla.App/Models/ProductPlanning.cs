namespace Propla
{
    /// <summary>
    /// ■■■■■　自動生成　■■■■■
    /// ProductPlanning
    /// </summary>
    public class ProductPlanning : ObservableObject
    {
        public string ProductPlanningID { get; set; } = "";
        public string ProductCategory { get; set; } = "0";
        public string ProductPerson { get; set; } = "／ ・";
        public string ProductNameOfficial { get; set; } = "『』";
        public string ProductNameRuby { get; set; } = "";
        public string ProductNameOfficialE { get; set; } = "";
        public string ProductNameOmit1 { get; set; } = "";
        public string ProductNameReceipt { get; set; } = "";
        public string ProductReleaseStatus { get; set; } = "0";
        public string ProductTestReleaseDate { get; set; } = "";
        public string ProductReleaseDate { get; set; } = "";
        public string ProductAnniversary { get; set; } = "";
        public string ProductRenewalHistory { get; set; } = "";
        public string TrademarkAcquisitionList { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E5%95%86%E6%A8%99/%E3%80%90%E6%9C%80%E6%96%B0%E3%80%91%E5%8F%96%E5%BE%97%E6%B8%88%E5%95%86%E6%A8%99%E4%B8%80%E8%A6%A7.xlsx?d=w7d82c597fcd14c5a828039f71348a18f&csf=1&web=1&e=cADtS6\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"blue\">\r\n商標取得一覧ﾃﾞｰﾀﾘﾝｸ\r\n</font>\r\n</a>";
        public string ProductWeight { get; set; } = "";
        public string ProductForm { get; set; } = "";
        public string JANCode { get; set; } = "／";
        public string StockID { get; set; } = "";
        public string ShipmentID { get; set; } = "";
        public string Sample { get; set; } = "0";
        public string SampleDetails { get; set; } = "";
        public string SampleNumber { get; set; } = "";
        public string ManufactureName { get; set; } = "";
        public string FactoryNumber { get; set; } = "／";
        public string FactoryNumberList { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-ki/Shared%20Documents/General/02.%E5%95%86%E5%93%81%E7%AE%A1%E7%90%86%EF%BC%88%E5%93%81%E8%B3%AA%E3%83%BBFAQ%E3%83%BB%E3%83%91%E3%82%B1%E3%81%AA%E3%81%A9%EF%BC%89/08.%E8%A3%BD%E9%80%A0%E6%89%80%E5%9B%BA%E6%9C%89%E8%A8%98%E5%8F%B7/%E8%A3%BD%E9%80%A0%E6%89%80%E5%9B%BA%E6%9C%89%E8%A8%98%E5%8F%B7%E4%B8%80%E8%A6%A7.xlsx?d=wb1336479249b417a861f408cd2de415b&csf=1&web=1&e=vXs9H3\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">\r\n製造所固有記号一覧ﾃﾞｰﾀﾘﾝｸ\r\n</font>\r\n</a>";
        public string BestByDateJapan { get; set; } = "年";
        public string BestByDateOverseas { get; set; } = "年";
        public string DisintegrationTestResult { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-ki/Shared%20Documents/General/02.%E5%95%86%E5%93%81%E7%AE%A1%E7%90%86%EF%BC%88%E5%93%81%E8%B3%AA%E3%83%BBFAQ%E3%83%BB%E3%83%91%E3%82%B1%E3%81%AA%E3%81%A9%EF%BC%89/00.%E5%93%81%E8%B3%AA%EF%BC%88%E8%A3%BD%E5%93%81%E8%A6%8F%E6%A0%BC%E6%9B%B8%E3%83%BB%E5%AE%89%E5%85%A8%E6%80%A7%E3%83%BB%E8%A9%A6%E9%A8%93%E3%81%AA%E3%81%A9%EF%BC%89/02%20%E5%B4%A9%E5%A3%8A%E6%80%A7%E8%A9%A6%E9%A8%93?csf=1&web=1&e=SAexwR\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">\r\n崩壊性試験の結果ﾃﾞｰﾀﾘﾝｸ\r\n</font>\r\n</a>";
        public string MaterialOriginList { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:b:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E5%8E%9F%E6%96%99%E3%81%AE%E7%94%B1%E6%9D%A5%E4%B8%80%E8%A6%A7/%E2%98%85FAQ%E6%9B%B4%E6%96%B0%E7%94%A8%E2%98%85%E5%95%86%E5%93%81%E5%88%A5%E3%83%BB%E7%94%A3%E5%9C%B0%E3%80%81%E7%94%B1%E6%9D%A5%E5%8E%9F%E6%96%99%E4%B8%80%E8%A6%A7.pdf?csf=1&web=1&e=jrnifd\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"blue\">\r\n原料の由来一覧リンク\r\n</font>\r\n</a>";
        public string MaterialOriginListLink { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-ki/Shared%20Documents/General/02.%E5%95%86%E5%93%81%E7%AE%A1%E7%90%86%EF%BC%88%E5%93%81%E8%B3%AA%E3%83%BBFAQ%E3%83%BB%E3%83%91%E3%82%B1%E3%81%AA%E3%81%A9%EF%BC%89/01.%E3%83%91%E3%83%83%E3%82%B1%E3%83%BC%E3%82%B8/%E2%98%85FAQ%E6%9B%B4%E6%96%B0%E7%94%A8%E2%98%85%E5%95%86%E5%93%81%E5%88%A5%E3%83%BB%E7%94%A3%E5%9C%B0%E3%80%81%E7%94%B1%E6%9D%A5%E5%8E%9F%E6%96%99%E4%B8%80%E8%A6%A7.xlsx?d=w6c6874f619f44be189f78178d5e32843&csf=1&web=1&e=cbrQ0j\r\ntarget=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"blue\">\r\n原料の由来一覧ﾃﾞｰﾀﾘﾝｸ\r\n</font>\r\n</a>";
        public string ProductStandards { get; set; } = "";
        public string ExpirationDate { get; set; } = "年";
        public string MaterialOriginList2 { get; set; } = "";
        public string MaterialOriginListLink2 { get; set; } = "";
        public string ProductStandards2 { get; set; } = "";
        public string FeaturesMaterial { get; set; } = "";
        public string FeaturesStandard { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-ki/Shared%20Documents/General/02.%E5%95%86%E5%93%81%E7%AE%A1%E7%90%86%EF%BC%88%E5%93%81%E8%B3%AA%E3%83%BBFAQ%E3%83%BB%E3%83%91%E3%82%B1%E3%81%AA%E3%81%A9%EF%BC%89/10.%E5%B7%A5%E5%A0%B4%E5%8F%96%E5%BE%97%E8%B3%87%E6%A0%BC/%E8%A3%BD%E9%80%A0%E5%B7%A5%E5%A0%B4%E3%83%BB%E5%8F%96%E5%BE%97%E8%A6%8F%E6%A0%BC%E4%B8%80%E8%A6%A7.xlsx?d=w53a162e2d72c42898fda1eefc72aa7cc&csf=1&web=1&e=H1AwPg\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"blue\">\r\n工場の取得規格一覧\r\n</font>\r\n</a>";
        public string MaterialAllergens1 { get; set; } = "";
        public string MaterialAllergens2 { get; set; } = "";
        public string MaterialAttention { get; set; } = "";
        public string MaterialAllergensOthers { get; set; } = "";
        public string FactoryAllergens { get; set; } = "";
        public string MaterialAttention2 { get; set; } = "";
        public string MaterialAllergensTalk { get; set; } = "";
        public string ProductFeatures { get; set; } = "";
        public string AdvertisingRequirements { get; set; } = "";
        public string AdditiveFreeDisplay { get; set; } = "";
        public string NGOKDisplay { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E5%9F%BA%E6%9C%AC%E8%A1%A8%E8%A8%98/%E5%95%86%E5%93%81%E3%81%AE%E8%A1%A8%E7%8F%BENG%E3%83%BBOK%E9%9B%86.xlsx?d=we6c82d9e7fec457b9c1374145a0d8307&csf=1&web=1&e=3iTwLx\r\ntarget=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"blue\">\r\n商品の表現NG・OK集\r\n</font>\r\n</a>";
        public string LatestInitialDocumentLink { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E5%95%86%E5%93%81%E8%B3%87%E6%96%99%E4%BF%9D%E5%AD%98?csf=1&web=1&e=fXMFfA\r\ntarget=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"blue\">\r\n商品資料(初回資料等）\r\n</font>\r\n</a>";
        public string ProductStudyMaterials { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E5%8F%82%E8%80%83%E8%B3%87%E6%96%99?csf=1&web=1&e=htX3V1\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">\r\n参考資料データリンク\r\n</font>\r\n</a>";
        public string SampleSetAttention { get; set; } = "";
        public string SampleSetAttentionLink { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-all/Shared%20Documents/General/01.%E3%82%A4%E3%83%99%E3%83%B3%E3%83%88%E6%AF%8E/31.%E5%95%86%E5%93%81%E9%96%A2%E9%80%A3%E3%81%AE%E8%A1%A8%E8%A8%98/%E3%81%8A%E8%A9%A6%E3%81%97%E3%82%BB%E3%83%83%E3%83%88%E4%BD%9C%E6%88%90%E6%99%82%E6%B3%A8%E6%84%8F%E4%BA%8B%E9%A0%85.xlsx?d=wd025f4d22562405ab031a1cd4be8e17d&csf=1&web=1&e=N46wVd\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">\r\nお試しｾｯﾄ作成時注意事項ﾃﾞｰﾀﾘﾝｸ\r\n</font>\r\n</a>";
        public string TaxRate { get; set; } = "0";
        public string GeneralPriceWithoutTax { get; set; } = "";
        public string GeneralPriceWithoutTax2 { get; set; } = "";
        public string GeneralPriceWithoutTax3 { get; set; } = "";
        public string GeneralPrice { get; set; } = "";
        public string GeneralPrice2 { get; set; } = "";
        public string GeneralPrice3 { get; set; } = "";
        public string RegularCoursePrice { get; set; } = "";
        public string RegularCoursePrice2 { get; set; } = "";
        public string RegularCoursePrice3 { get; set; } = "";
        public string PriceDifference { get; set; } = "";
        public string PriceDifference2 { get; set; } = "";
        public string PriceDifference3 { get; set; } = "";
        public string RegularCourseAttention { get; set; } = "※定期コースは中止連絡をいただくまで自動的にお届け。変更・中止の場合お届け予定日の10日前までに電話等でご連絡ください。";
        public string PriceSystem { get; set; } = "0";
        public string PriceSystemAttention { get; set; } = "";
        public string DeliveryFee { get; set; } = "円";
        public string DeliveryFeeSystem { get; set; } = "";
        public string OrderAccepted { get; set; } = "0";
        public string ProductCost { get; set; } = "円";
        public string GrossProfitMargin2 { get; set; } = "";
        public string ProductCost2 { get; set; } = "円";
        public string GrossProfitMargin { get; set; } = "";
        public string SaveQuotation { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-ki/Shared%20Documents/General/34.%E8%A6%8B%E7%A9%8D%E6%9B%B8%E4%BF%9D%E5%AD%98%E5%85%88?csf=1&web=1&e=e6Ujra\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">\r\n見積書保存先\r\n</font>\r\n</a>";
        public string ChangeHistory { get; set; } = "";
        public string TransportationMethod { get; set; } = "0";
        public string DeliveryMethod { get; set; } = "";
        public string FirstPackingGoods { get; set; } = "";
        public string PackingMethod { get; set; } = "0";
        public string PackingAttention { get; set; } = "";
        public string ProductLot { get; set; } = "";
        public string ProductCalorie { get; set; } = "○○約○○kcal";
        public string SpecifiedValueComponent { get; set; } = "";
        public string EmphasisComponent { get; set; } = "";
        public string FoodEquivalent { get; set; } = "";
        public string FoodEquivalentLink { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-ki/Shared%20Documents/General/03.%E6%88%90%E5%88%86%E5%88%86%E6%9E%90/03.%E9%A3%9F%E5%93%81%E6%8F%9B%E7%AE%97?csf=1&web=1&e=o1MurJ\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">\r\n食品換算データリンク\r\n</font>\r\n</a>";
        public string NutrientsComparison { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E6%A0%84%E9%A4%8A%E6%AF%94%E8%BC%83?csf=1&web=1&e=Kc8xKp\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">\r\n栄養比較データリンク\r\n</font>\r\n</a>";
        public string NutrientsCertificate { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E6%A0%84%E9%A4%8A%E6%88%90%E5%88%86%E8%A8%BC%E6%98%8E%E6%9B%B8(%E6%9C%80%E6%96%B0)?csf=1&web=1&e=jTAqDl\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\" color=\"blue\">\r\n最新値なのでﾊﾟｯｹｰｼﾞ等と異なる場合あり\r\n</font>\r\n</a>";
        public string NutrientsCertificateLink { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-ki/Shared%20Documents/General/03.%E6%88%90%E5%88%86%E5%88%86%E6%9E%90/04.%E6%A0%84%E9%A4%8A%E6%88%90%E5%88%86%E8%A8%BC%E6%98%8E%E6%9B%B8?csf=1&web=1&e=ielUdl\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  color=\"black\">\r\n栄養成分証明書ﾃﾞｰﾀﾘﾝｸ\r\n</font>\r\n</a>";
        public string PackageImage { get; set; } = "";
        public string PackagePaperData { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E3%83%91%E3%83%83%E3%82%B1%E3%83%BC%E3%82%B8%E6%A0%A1%E4%BA%86%E3%83%87%E3%83%BC%E3%82%BF?csf=1&web=1&e=TNcZh3\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  collor=\"black\">\r\nパッケージ校了ﾃﾞｰﾀﾘﾝｸ\r\n</font>\r\n</a>";
        public string PackagePhotoData { get; set; } = "<a href=\r\nhttps://yazuyaoffice.sharepoint.com/:f:/r/sites/yazuya-all/Shared%20Documents/General/01.%E3%82%A4%E3%83%99%E3%83%B3%E3%83%88%E6%AF%8E/06.%E5%90%84%E7%A8%AE%E6%92%AE%E5%BD%B1%E3%83%87%E3%83%BC%E3%82%BF/01.%E5%95%86%E5%93%81%E9%96%A2%E9%80%A3?csf=1&web=1&e=vYOeSC\r\n target=\"_blank\">\r\n<font face=\"BIZ UDPゴシック\" size=\"2\"  collor=\"black\">\r\n撮影データリンク\r\n</font>\r\n</a>";
        public string PackageDesignCompany { get; set; } = "／";
        public string PackagePrintingCompany { get; set; } = "／";
        public string PackageLatestNumber { get; set; } = "／";
        public string PackageRevisionHistory { get; set; } = "";
        public string ProductSwitchList { get; set; } = "";
        public string PackageMaterialDisplay { get; set; } = "／";
        public string ShrinkWrap { get; set; } = "0";
        public string PackageSize { get; set; } = "縦㎝　横㎝　高さ㎝／縦㎝　横㎝　高さ㎝";
        public string EnclosedGoods { get; set; } = "";
        public string PackageFoodName { get; set; } = "";
        public string PackageMaterialName { get; set; } = "";
        public string PackageProductWeight { get; set; } = "";
        public string PackageBestByDate { get; set; } = "";
        public string BestByDateDisplay { get; set; } = "／";
        public string BestByDateExampleOutside { get; set; } = "";
        public string BestByDateExampleInside { get; set; } = "";
        public string PackageStorageMethod { get; set; } = "";
        public string PackageAfterOpening { get; set; } = "";
        public string PackageCountryOrigin { get; set; } = "";
        public string PackageSellerName { get; set; } = "";
        public string PackageFactoryName { get; set; } = "";
        public string PackageOthersMandatory { get; set; } = "";
        public string PackageAttention { get; set; } = "";
        public string PackageHowToEat { get; set; } = "";
        public string PackageHowToFeed { get; set; } = "";
        public string AmountFeedPerDay { get; set; } = "";
        public string PackageNutritionalContent { get; set; } = "";
        public string NutrientFunctionClaims { get; set; } = "";
        public string NutrientFunctionAttention { get; set; } = "";
        public string FunctionClaimsSubmitted { get; set; } = "";
        public string FunctionClaimsMandatory { get; set; } = "";
        public string FunctionClaimsOthersMandatory { get; set; } = "";
        public string FunctionClaimsAttention { get; set; } = "";
        public string DrugDateOfRevision { get; set; } = "";
        public string DrugPackageInsertAttention { get; set; } = "";
        public string DrugSalesName { get; set; } = "";
        public string DrugMedicinalEffectName { get; set; } = "";
        public string DrugRiskCategory { get; set; } = "";
        public string CosmeCategory { get; set; } = "";
        public string DrugWeight { get; set; } = "";
        public string DrugProductFeatures { get; set; } = "";
        public string DrugThingsImpossible { get; set; } = "";
        public string DrugConsult { get; set; } = "";
        public string DrugOthersMandatory { get; set; } = "";
        public string DrugAdvertisingRequirements { get; set; } = "";
        public string DrugEfficacy { get; set; } = "";
        public string DrugDosage { get; set; } = "";
        public string DrugDosageDetail { get; set; } = "";
        public string DrugDosageAttention { get; set; } = "";
        public string DrugIngredientQuantity { get; set; } = "";
        public string DrugIngredientAttention { get; set; } = "";
        public string CosmeAllIngredients { get; set; } = "";
        public string CosmeUsing { get; set; } = "";
        public string DrugHandlingAttention { get; set; } = "";
        public string DrugManufacturerName { get; set; } = "";
        public string DrugSellerName { get; set; } = "";
        public string DrugContactInformation { get; set; } = "";
        public string DrugSideEffectsInformation { get; set; } = "";
        public string CosmeCountryOrigin { get; set; } = "";
        public string DrugExpirationDateDisplay { get; set; } = "";
        public string DrugExpirationDateExample { get; set; } = "";
        public string DrugSerialNumberDateDisplay { get; set; } = "";
        public string DrugSerialNumberExample { get; set; } = "";
        public string DrugSpecificSales { get; set; } = "";
        public string DrugOthersMandatory2 { get; set; } = "";
        public string PackageInformationRemarks { get; set; } = "";
        public string SearchKeyword { get; set; } = "";
        public string PopUpLink { get; set; } = "";
        public int version { get; set; } = 0;
        public string updateuser { get; set; } = "";
        public string updatedate { get; set; } = "";

        //�C���f�N�T�̒�`
        public object this[string propertyName]
        {
            get
            {
                return typeof(ProductPlanning).GetProperty(propertyName).GetValue(this);
            }
            set
            {
                typeof(ProductPlanning).GetProperty(propertyName).SetValue(this, value);
            }
        }
    }
}
