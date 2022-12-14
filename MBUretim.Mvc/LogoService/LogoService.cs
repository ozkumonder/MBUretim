using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using log4net;
using MBUretim.Mvc.LogoHelper;
using MBUretim.Mvc.Extensions;
using UnityObjects;

namespace MBUretim.Mvc.Models.LogoService
{

    public static class LogoService
    {
        static ILog log = LogManager.GetLogger(typeof(LogoService));
        //public static UnityApplication UnityApp = new UnityObjects.UnityApplication();

        #region Const
        public const string ProductOrder = @"SELECT 
	 	STFICHE.DATE_ AS STFICHE_DATE
	,STFICHE.LOGICALREF AS STFICHE_LOGICALREF
	,STFICHE.BRANCH AS STFICHE_BRANCH
	,STFICHE.FICHENO AS STFICHE_FICHENO
	,ITEM.LOGICALREF AS ITEM_LOGICALREF
	,ITEM.CODE AS ITEM_CODE
	,ITEM.NAME AS ITEM_NAME
	,SUM(STLINE.AMOUNT) AS STLINE_AMOUNT
	,UNITSETL.LOGICALREF AS UNITSETL_LOGICALREF
	,UNITSETL.CODE AS UNITSETL_CODE
	,UNITSETL.CODE AS UNITSETL_NAME
FROM LG_{0}_{1}_STLINE STLINE
LEFT OUTER JOIN LG_{0}_{1}_STFICHE STFICHE
	ON STFICHE.LOGICALREF = STLINE.STFICHEREF
LEFT OUTER JOIN LG_{0}_ITEMS ITEM
	ON ITEM.LOGICALREF = STLINE.STOCKREF
LEFT OUTER JOIN LG_{0}_UNITSETL UNITSETL
	ON UNITSETL.UNITSETREF = ITEM.UNITSETREF

WHERE STFICHE.TRCODE IN (7, 8)
AND ITEM.CARDTYPE IN (11, 12)
AND STFICHE.DATE_ BETWEEN @STARTDATE AND @ENDDATE 
AND STFICHE.BRANCH IN( @BRANCHNO) 
AND STLINE.STOCKREF NOT IN (SELECT ItemLogicalref FROM MBGOP_ProductOrder WHERE IsThere = 1 AND StficheFicheNo = STFICHE.FICHENO) 

GROUP BY	STFICHE.DATE_
			,STFICHE.LOGICALREF
			,STFICHE.FICHENO
			,ITEM.LOGICALREF
			,ITEM.CODE
			,ITEM.NAME
			,UNITSETL.LOGICALREF
			,STLINE.ORDFICHEREF
			,STFICHE.BRANCH
			,UNITSETL.CODE
			,UNITSETL.NAME ORDER BY STFICHE_FICHENO";
        public const string ProductOrderGroup = @"SELECT
	 STFICHE.BRANCH AS STFICHE_BRANCH
	,ITEM.LOGICALREF AS ITEM_LOGICALREF
	,ITEM.CODE AS ITEM_CODE
	,ITEM.NAME AS ITEM_NAME
	,SUM(STLINE.AMOUNT) AS STLINE_AMOUNT
	,UNITSETL.LOGICALREF AS UNITSETL_LOGICALREF
	,UNITSETL.CODE AS UNITSETL_CODE
	,UNITSETL.CODE AS UNITSETL_NAME
FROM LG_017_01_STLINE STLINE
LEFT OUTER JOIN LG_017_01_STFICHE STFICHE
	ON STFICHE.LOGICALREF = STLINE.STFICHEREF
LEFT OUTER JOIN LG_017_ITEMS ITEM
	ON ITEM.LOGICALREF = STLINE.STOCKREF
LEFT OUTER JOIN LG_017_UNITSETL UNITSETL
	ON UNITSETL.UNITSETREF = ITEM.UNITSETREF

WHERE STFICHE.TRCODE IN (7, 8)
AND ITEM.CARDTYPE IN (11, 12)
AND STFICHE.DATE_ BETWEEN @STARTDATE AND @ENDDATE 
AND STFICHE.BRANCH IN( @BRANCHNO) 
--AND STFICHE.FICHENO NOT IN (SELECT  StficheFicheNo FROM MBGOP_ProductOrder)
AND STLINE.STOCKREF NOT IN (SELECT ItemLogicalref FROM MBGOP_ProductOrder WHERE IsThere = 1 AND StficheFicheNo = STFICHE.FICHENO) 
GROUP BY	            
			 ITEM.LOGICALREF
			,ITEM.CODE
			,ITEM.NAME
			,UNITSETL.LOGICALREF
			,STLINE.ORDFICHEREF
			,STFICHE.BRANCH
			,UNITSETL.CODE
			,UNITSETL.NAME";

        public const string Ordered = @"SELECT
	CASE STFICHE.TRCODE
		WHEN 13 THEN 'Üretimden Giriş Fişi'
	END AS 'STFICHE_TRCODE'
	,STFICHE.FICHENO AS 'STFICHE_FICHENO'
	,PRODORD.FICHENO AS 'PRODORD_FICHENO'
	,ITEM.CODE AS 'ITEM_CODE'
	,ITEM.NAME AS 'ITEM_NAME'
	,STLINE.AMOUNT AS 'STLINE_AMOUNT'
FROM LG_{0}_{1}_STFICHE STFICHE
LEFT OUTER JOIN LG_{0}_PRODORD PRODORD
	ON PRODORD.LOGICALREF = STFICHE.PRODORDERREF
LEFT OUTER JOIN LG_{0}_{1}_STLINE STLINE
	ON STFICHE.LOGICALREF = STLINE.STFICHEREF
LEFT OUTER JOIN LG_{0}_ITEMS ITEM
	ON ITEM.LOGICALREF = STLINE.STOCKREF
WHERE STLINE.STFICHEREF NOT IN (SELECT DISTINCT StficheLogicalref FROM MBGOP_ProductOrder)
AND STFICHE.TRCODE = 13 AND PRODORD.FICHENO IS NOT NULL
GROUP BY	STFICHE.TRCODE
			,STFICHE.FICHENO
			,PRODORD.FICHENO
			,ITEM.CODE
			,ITEM.NAME
			,STLINE.AMOUNT";
        public const string CapiDiv = @"SELECT
	 CAPIDIV.LOGICALREF AS CAPIDIV_LOGICALREF
	,CAPIDIV.FIRMNR AS CAPIDIV_FIRMNR
	,CAPIDIV.NR AS CAPIDIV_NR
	,CAPIDIV.NAME AS CAPIDIV_NAME
FROM L_CAPIDIV CAPIDIV
WHERE CAPIDIV.FIRMNR = @FIRMNR AND CAPIDIV.NR NOT IN(0,5,10)";

        public const string Items = @"SELECT LOGICALREF,CODE,NAME FROM LG_{0}_ITEMS WHERE CARDTYPE <> 22";
        public const string BomMaster = @"SELECT LOGICALREF,CODE,NAME FROM LG_{0}_BOMASTER";
        public const string BomRev = @"SELECT LOGICALREF, CODE FROM LG_{0}_BOMREVSN WHERE BOMMASTERREF = @BOMMASTERREF";
        public const string ProdOrderRef = @"SELECT LOGICALREF FROM LG_{0}_PRODORD WHERE FICHENO = @FICHENO";
        public const string StFicheControl = @"SELECT StficheFicheNo FROM MBGOP_ProductOrder WHERE StficheFicheNo IN(STFICHENO)";
        public const string ProdOrderFicheNo = @"SELECT FICHENO FROM LG_{0}_PRODORD WHERE FICHENO = @FICHENO";

        public const string ProdOrderLastFicheNo =
            @"SELECT SUBSTRING(FICHENO,4,8) AS FICHENO from LG_{0}_PRODORD WHERE FICHENO LIKE 'URT%' AND FICHENO = (SELECT MAX(FICHENO) FROM LG_{0}_PRODORD WHERE FICHENO LIKE 'URT%') GROUP BY FICHENO";

        public const string ProdOrderBegTime = @"UPDATE LG_{0}_PRODORD SET BEGTIME = 369098752, PLNBEGTIME = 369098752, ACTBEGTIME = 369373440, PLNENDTIME = 369101568, ACTENDTIME = 369426432 WHERE FICHENO LIKE 'URT%' AND LOGICALREF = @BEGTIME";

        public const string ProdSlipControl = @"SELECT COUNT(LOGICALREF) AS 'COUNT' FROM LG_017_01_STLINE WHERE STOCKREF NOT IN (SELECT STOCKREF FROM LG_{0}_{1}_STLINE WHERE STFICHEREF = (SELECT LOGICALREF FROM LG_{0}_{1}_STFICHE	WHERE TRCODE = 12 AND PRODSTAT = 0 AND  PORDERFICHENO = @FICHENO)) AND STFICHEREF = (SELECT LOGICALREF FROM LG_{0}_{1}_STFICHE WHERE TRCODE = 12 AND PRODSTAT = 1 AND  PORDERFICHENO = @FICHENO)";
        public const string ProdSlipDiff = @"SELECT ITEM.CODE ,ITEM.NAME,SOURCEINDEX FROM LG_017_01_STLINE LEFT OUTER JOIN LG_017_ITEMS ITEM ON ITEM.LOGICALREF = STOCKREF WHERE STOCKREF NOT IN (SELECT STOCKREF FROM LG_{0}_{1}_STLINE WHERE STFICHEREF = (SELECT LOGICALREF FROM LG_{0}_{1}_STFICHE	WHERE TRCODE = 12 AND PRODSTAT = 0 AND  PORDERFICHENO = @FICHENO)) AND STFICHEREF = (SELECT LOGICALREF FROM LG_{0}_{1}_STFICHE WHERE TRCODE = 12 AND PRODSTAT = 1 AND  PORDERFICHENO = @FICHENO)";

        #endregion

        #region Extensions
        public static string ToConvertFirms(this string text)
        {
            string firm = GetFirmNr();//_unityApp.CurrentFirm.ToString();
            string period = "1"; //_unityApp.ActivePeriod.ToString();

            switch (firm.Length)
            {
                case 1: firm = "00" + firm; break;
                case 2: firm = "0" + firm; break;
                default: break;
            }
            switch (period.Length)
            {
                case 1: period = "0" + period; break;
                default: break;
            }


            if (text.Contains("{1}"))
            {
                return string.Format(text, firm, period);
            }
            else
            {
                return string.Format(text, firm);
            }
        }
        #endregion

        #region Methods Local
        private static string GetConnectionString => ConfigurationManager.ConnectionStrings["LOGOConnectionString"].ConnectionString;

        public static List<MBGOP_ProductOrder> GetProductOrderByDateAndByBranch(DateTime date, DateTime endDate, string[] branch)
        {
            var dt = new DataTable();
            List<MBGOP_ProductOrder> result = null;
            var branchArray = (string[])branch;
            try
            {

                foreach (string t in branchArray)
                {
                    var conn = new SqlConnection(GetConnectionString);

                    var adap = new SqlDataAdapter(ProductOrder.ToConvertFirms(), conn);
                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();
                    adap.SelectCommand.Parameters.AddWithValue("@STARTDATE", date);
                    adap.SelectCommand.Parameters.AddWithValue("@ENDDATE", endDate);
                    adap.SelectCommand.Parameters.AddWithValue("@BRANCHNO", t);
                    adap.Fill(dt);
                    result = dt.AsEnumerable()
                        .Select(s => new MBGOP_ProductOrder
                        {
                            StficheLogicalref = s.Field<int>("STFICHE_LOGICALREF"),
                            StficheDate = s.Field<DateTime>("STFICHE_DATE"),
                            StficheFicheNo = s.Field<string>("STFICHE_FICHENO"),
                            StficheBranch = s.Field<short>("STFICHE_BRANCH"),
                            ItemLogicalref = s.Field<int>("ITEM_LOGICALREF"),
                            ItemCode = s.Field<string>("ITEM_CODE"),
                            ItemName = s.Field<string>("ITEM_NAME"),
                            UnitSetLLogicalref = s.Field<int>("UNITSETL_LOGICALREF"),
                            UnitSetLCode = s.Field<string>("UNITSETL_CODE"),
                            UnitSetLName = s.Field<string>("UNITSETL_NAME"),
                            StlineAmount = s.Field<double>("STLINE_AMOUNT")

                        }).ToList();
                    adap.Dispose();
                    conn.Close();
                    conn.Dispose();
                    dt.Dispose();
                }

            }
            catch (Exception exp)
            {
                throw exp;
            }


            return result;
        }
        public static List<MBGOP_ProductOrder> GetProductOrderByDateAndByBranchGroup(DateTime date, DateTime endDate, string[] branch)
        {
            var dt = new DataTable();
            List<MBGOP_ProductOrder> result = null;
            var branchArray = (string[])branch;
            try
            {

                foreach (string t in branchArray)
                {
                    var conn = new SqlConnection(GetConnectionString);
                    var adap = new SqlDataAdapter(ProductOrderGroup.ToConvertFirms(), conn);
                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();
                    adap.SelectCommand.Parameters.AddWithValue("@STARTDATE", date);
                    adap.SelectCommand.Parameters.AddWithValue("@ENDDATE", endDate);
                    adap.SelectCommand.Parameters.AddWithValue("@BRANCHNO", t);
                    adap.Fill(dt);
                    result = dt.AsEnumerable()
                        .Select(s => new MBGOP_ProductOrder
                        {

                            StficheBranch = s.Field<short>("STFICHE_BRANCH"),
                            ItemLogicalref = s.Field<int>("ITEM_LOGICALREF"),
                            ItemCode = s.Field<string>("ITEM_CODE"),
                            ItemName = s.Field<string>("ITEM_NAME"),
                            UnitSetLLogicalref = s.Field<int>("UNITSETL_LOGICALREF"),
                            UnitSetLCode = s.Field<string>("UNITSETL_CODE"),
                            UnitSetLName = s.Field<string>("UNITSETL_NAME"),
                            StlineAmount = s.Field<double>("STLINE_AMOUNT")

                        }).ToList();
                    adap.Dispose();
                    conn.Close();
                    conn.Dispose();
                    dt.Dispose();
                }

            }
            catch (Exception exp)
            {
                throw exp;
            }


            return result;
        }
        public static List<CapiDiv> GetCapiDivs(int firmNr)
        {
            var dt = new DataTable();
            List<CapiDiv> result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(CapiDiv.ToConvertFirms(), conn);
            adap.SelectCommand.Parameters.AddWithValue("@FIRMNR", firmNr);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable()
                    .Select(s => new CapiDiv
                    {
                        CAPIDIV_LOGICALREF = s.Field<int>("CAPIDIV_LOGICALREF"),
                        CAPIDIV_FIRMNR = s.Field<short>("CAPIDIV_FIRMNR"),
                        CAPIDIV_NR = s.Field<short>("CAPIDIV_NR"),
                        CAPIDIV_NAME = s.Field<string>("CAPIDIV_NAME")


                    }).ToList();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }

        public static List<Item> GetItems(int firmNr)
        {
            var dt = new DataTable();
            List<Item> result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(Items.ToConvertFirms(), conn);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable()
                    .Select(s => new Item
                    {
                        Id = s.Field<int>("LOGICALREF"),
                        Code = s.Field<string>("CODE"),
                        Name = s.Field<string>("NAME")


                    }).ToList();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }
        public static List<Item> GetBomMaster(int firmNr)
        {
            var dt = new DataTable();
            List<Item> result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(BomMaster.ToConvertFirms(), conn);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable()
                    .Select(s => new Item
                    {
                        Id = s.Field<int>("LOGICALREF"),
                        Code = s.Field<string>("CODE"),
                        Name = s.Field<string>("NAME")


                    }).ToList();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }
        public static List<MBGOP_ProductOrder> GetProductorderList()
        {
            using (var context = new LOGOProductOrderContext())
            {
                return context.MBGOP_ProductOrder.ToList();
            }
        }

        public static MBGOP_ProductOrder GetProductOrderByFicheNo(string ficheNo)
        {
            using (var context = new LOGOProductOrderContext())
            {
                return context.MBGOP_ProductOrder.FirstOrDefault(x => x.StficheFicheNo == ficheNo);
            }
        }
        public static MBGOP_ProductOrder SaveProductorders(List<MBGOP_ProductOrder> productorder)
        {
            using (var context = new LOGOProductOrderContext())
            {
                foreach (var item in productorder)
                {
                    context.MBGOP_ProductOrder.Add(item);
                    context.SaveChanges();
                }
            }
            return null;
        }
        public static bool SaveProductorder(MBGOP_ProductOrder productorder)
        {
            using (var context = new LOGOProductOrderContext())
            {
                context.MBGOP_ProductOrder.Add(new MBGOP_ProductOrder
                {
                    StficheLogicalref = productorder.StficheLogicalref,
                    StficheFicheNo = productorder.StficheFicheNo,
                    StficheDate = productorder.StficheDate,
                    StficheBranch = productorder.StficheBranch,
                    ItemLogicalref = productorder.ItemLogicalref,
                    ItemName = productorder.ItemName,
                    ItemCode = productorder.ItemCode,
                    CapiDivName = productorder.CapiDivName,
                    BomMasterLogicalRef = productorder.BomMasterLogicalRef,
                    BomRevSnLogicalref = productorder.BomRevSnLogicalref,
                    UnitSetLLogicalref = productorder.UnitSetLLogicalref,
                    UnitSetLName = productorder.UnitSetLName,
                    UnitSetLCode = productorder.UnitSetLCode,
                    StlineAmount = productorder.StlineAmount,
                    IsThere = productorder.IsThere

                });
                var result = context.SaveChanges();
                return result > 0;
            }
        }
        public static MBGOP_ProdOrderFiche SaveProductorderFiche(MBGOP_ProdOrderFiche productOrderFiche)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var result = context.MBGOP_ProdOrderFiche.Add(productOrderFiche);
                context.SaveChanges();
                return result;
            }
        }
        public static bool EditProductorderFiche(MBGOP_ProdOrderFiche productOrderFiche)
        {
            using (var context = new LOGOProductOrderContext())
            {
                context.Entry(context.MBGOP_ProdOrderFiche.Find(productOrderFiche.Id)).CurrentValues.SetValues(productOrderFiche);
                var result = context.SaveChanges();
                return result > 0;
            }
        }
        public static List<MBGOP_ProdOrderFiche> GetProductorderFiche(MBGOP_ProdOrderFiche productOrderFiche)
        {
            using (var context = new LOGOProductOrderContext())
            {

                var result = context.MBGOP_ProdOrderFiche.ToList();
                return result;
            }
        }
        public static MBGOP_ProdOrderFiche GetProductorderFicheById(int id)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var result = context.MBGOP_ProdOrderFiche.FirstOrDefault(x => x.Id == id);
                return result;
            }
        }
        public static MBGOP_ProdOrderFiche GetProductorderFicheByFicheNo(string ficheNo)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var result = context.MBGOP_ProdOrderFiche.FirstOrDefault(x => x.ProdOrderFicheNo == ficheNo);
                return result;
            }
        }
        public static string GetFirmNr()
        {
            string result = string.Empty;
            using (var context = new LOGOProductOrderContext())
            {
                result = context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr.ToString();
            }
            return result;
        }
        public static BomRevSn GetBomRevSn(int bomMasterRef)
        {
            var dt = new DataTable();
            BomRevSn result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(BomRev.ToConvertFirms(), conn);
            adap.SelectCommand.Parameters.AddWithValue("@BOMMASTERREF", bomMasterRef);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable().Select(s => new BomRevSn
                {

                    BomRevId = s.Field<int>("LOGICALREF"),
                    BomRevCode = s.Field<string>("CODE")

                }).FirstOrDefault();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }

        public static int GetProdOrderFicheRef(string prodOrderFicheNo)
        {
            var dt = new DataTable();
            int result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(ProdOrderRef.ToConvertFirms(), conn);
            adap.SelectCommand.Parameters.AddWithValue("@FICHENO", prodOrderFicheNo);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable().Select(s => s.Field<int>("LOGICALREF")).FirstOrDefault();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }

        public static bool IsThereOrder(string[] stficheNo)
        {

            DataTable dt = new DataTable();
            string result = string.Empty;
            var re = (string[])stficheNo;

            foreach (var s in stficheNo)
            {
                SqlConnection conn = new SqlConnection(GetConnectionString);
                SqlDataAdapter adap = new SqlDataAdapter(StFicheControl.ToConvertFirms(), conn);
                adap.SelectCommand.Parameters.AddWithValue("@STFICHENO", stficheNo);
                try
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();

                    adap.Fill(dt);
                    result = dt.Columns[1].ToString();
                }
                catch (Exception exp)
                {
                    throw exp;
                }
                finally
                {
                    conn.Close();

                    adap.Dispose();
                    conn.Dispose();
                    dt.Dispose();
                }

            }
            if (result.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetBomMasterRefByPairingTable(int itemref, short branch)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var productPairing = context.MBGOP_ProductPairing.FirstOrDefault(x => x.ItemId == itemref && x.DivisionCode == branch);
                if (productPairing != null)
                {
                    return (int)productPairing.BomId;
                }
                else
                {
                    return 0;
                }
            }
        }
        public static int GetBomRevSnRefByPairingTable(int bomref)
        {
            using (var context = new LOGOProductOrderContext())
            {
                try
                {
                    var result = context.MBGOP_ProductPairing.FirstOrDefault(x => x.BomId == bomref);
                    return result?.BomRevId ?? 0;
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled) log.Error("GetBomRevSnRefByPairingTable", ex);
                    throw;
                }
            }
        }

        public static string GetProdOrderFicheNo(string prodOrderFicheNo)
        {
            var dt = new DataTable();
            string result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(ProdOrderFicheNo.ToConvertFirms(), conn);
            adap.SelectCommand.Parameters.AddWithValue("@FICHENO", prodOrderFicheNo);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable().Select(s => s.Field<string>("FICHENO")).FirstOrDefault();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }
        public static string GetProdOrderLastFicheNo()
        {
            var dt = new DataTable();
            string result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(ProdOrderLastFicheNo.ToConvertFirms(), conn);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable().Select(s => s.Field<string>("FICHENO")).FirstOrDefault();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }

        public static List<OrderedFiche> GetOrderedFicheList()
        {
            var dt = new DataTable();
            List<OrderedFiche> result = null;
            try
            {
                var conn = new SqlConnection(GetConnectionString);
                var adap = new SqlDataAdapter(Ordered.ToConvertFirms(), conn);
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();
                adap.Fill(dt);
                result = dt.AsEnumerable()
                    .Select(s => new OrderedFiche
                    {
                        FicheType = s.Field<string>("STFICHE_TRCODE"),
                        StFicheNo = s.Field<string>("STFICHE_FICHENO"),
                        ProdFicheNo = s.Field<string>("PRODORD_FICHENO"),
                        ItemCode = s.Field<string>("ITEM_CODE"),
                        ItemName = s.Field<string>("ITEM_NAME"),
                        StlineAmount = s.Field<double>("STLINE_AMOUNT")

                    }).ToList();
                adap.Dispose();
                conn.Close();
                conn.Dispose();
                dt.Dispose();


            }
            catch (Exception exp)
            {
                throw exp;
            }


            return result;
        }
        public static List<OrderedFiche> GetUnOrderedFicheList()
        {
            var dt = new DataTable();
            List<OrderedFiche> result = null;
            try
            {
                var conn = new SqlConnection(GetConnectionString);
                var adap = new SqlDataAdapter(Ordered.ToConvertFirms(), conn);
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();
                adap.Fill(dt);
                result = dt.AsEnumerable()
                    .Select(s => new OrderedFiche
                    {
                        FicheType = s.Field<string>("STFICHE_TRCODE"),
                        StFicheNo = s.Field<string>("STFICHE_FICHENO"),
                        ProdFicheNo = s.Field<string>("PRODORD_FICHENO"),
                        ItemCode = s.Field<string>("ITEM_CODE"),
                        ItemName = s.Field<string>("ITEM_NAME"),
                        StlineAmount = s.Field<double>("STLINE_AMOUNT")

                    }).ToList();
                adap.Dispose();
                conn.Close();
                conn.Dispose();
                dt.Dispose();


            }
            catch (Exception exp)
            {
                throw exp;
            }


            return result;
        }

        public static int UpdateProdOrderTime(int logicalRef)
        {
            var dt = new DataTable();
            int result;
            var conn = new SqlConnection(GetConnectionString);
            var command = new SqlCommand(ProdOrderBegTime.ToConvertFirms(), conn);
            command.Parameters.AddWithValue("@BEGTIME", logicalRef);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();
                result = command.ExecuteNonQuery();


            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }

        public static int ProdSlipCheck(string prodOrderFicheNo)
        {
            var dt = new DataTable();
            int result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(ProdSlipControl.ToConvertFirms(), conn);
            adap.SelectCommand.Parameters.AddWithValue("@FICHENO", prodOrderFicheNo);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable().Select(s => s.Field<int>("COUNT")).FirstOrDefault();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }
        public static List<ProductOrderDTO> ProdSlipGetDiff(string prodOrderFicheNo)

        {
            var dt = new DataTable();
            List<ProductOrderDTO> result;
            var conn = new SqlConnection(GetConnectionString);
            var adap = new SqlDataAdapter(ProdSlipDiff.ToConvertFirms(), conn);
            adap.SelectCommand.Parameters.AddWithValue("@FICHENO", prodOrderFicheNo);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                adap.Fill(dt);
                result = dt.AsEnumerable()
                    .Select(s => new ProductOrderDTO
                    {
                        CODE = s.Field<string>("CODE"),
                        NAME = s.Field<string>("NAME"),
                        SOURCEINDEX = s.Field<short>("SOURCEINDEX")

                    }).ToList();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                conn.Dispose();
                dt.Dispose();
            }

            return result;
        }

        public static bool DeleteProductOrderFiche(int productOrderFicheId)
        {
            var result = false;
            using (var context = new LOGOProductOrderContext())
            {
                var del = context.MBGOP_ProdOrderFiche.FirstOrDefault(x => x.Id == productOrderFicheId);
                if (del != null)
                {
                    var delete = context.MBGOP_ProdOrderFiche.Remove(del);
                    if (delete != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        #endregion

        #region Methods Logo Production

        public static ResultType AddProdOrders(List<MBGOP_ProductOrder> prodOrder, bool groupParam, DateTime orderDate)
        {

            var result = new ResultType();
            UnityObjects.ProductionApplication prodApp = GlobalParameters.UnityApp.NewProductionApplication();
            foreach (var item in prodOrder)
            {
                if (log.IsInfoEnabled) log.Info("Uretim Emirleri Baslatıldı----------------------------------------**********************************------------------------------------------------");
                #region Fiş Numaralama


                string fisNumarasi = string.Empty;
                using (var context = new LOGOProductOrderContext())
                {
                    var prodOrderFiche = (from p in context.MBGOP_ProdOrderFiche select p).AsEnumerable().LastOrDefault();

                    if (prodOrderFiche != null)
                    {
                        var ficheNoLocal = prodOrderFiche.ProdOrderFicheNo.Substring(3);
                        var prodOrderLastFicheNo = GetProdOrderLastFicheNo();
                        string mbgopProdOrderFiche = string.Empty;
                        mbgopProdOrderFiche = ficheNoLocal.ToInt32() > prodOrderLastFicheNo.ToInt32() ? prodOrderFiche.ProdOrderFicheNo.Substring(3) : GetProdOrderLastFicheNo();
                        int i;
                        //string fisno = GetProdOrderFicheNo(prodOrderFiche.ProdOrderFicheNo);
                        //if (fisno != null)
                        //{
                        i = int.Parse(mbgopProdOrderFiche) + 1;
                        //}
                        //else
                        //{
                        //    i = int.Parse(mbgopProdOrderFiche) + 2;
                        //}

                        fisNumarasi = i.ToString(new string('0', mbgopProdOrderFiche.Length));

                    }
                }
                #endregion
                string ficheNo = "URT" + fisNumarasi;
                int itemRef = item.ItemLogicalref.Value;
                int BOMRef = GetBomMasterRefByPairingTable(item.ItemLogicalref.Value, (short)item.StficheBranch);
                int RevRef = GetBomRevSnRefByPairingTable(BOMRef);
                int FactoryNr = 0;
                double PlnAmount = item.StlineAmount.Value;
                int uomR = item.UnitSetLLogicalref.Value;
                DateTime targetDate;
                DateTime ficheDate;
                if (groupParam)
                {
                    targetDate = orderDate;
                    ficheDate = orderDate;
                }
                else
                {
                    targetDate = item.StficheDate.Value == null ? orderDate : item.StficheDate.Value;
                    ficheDate = item.StficheDate.Value == null ? orderDate : item.StficheDate.Value;
                }
                UnityObjects.PrdItmClsLines itemClsLines = prodApp.NewPrdItmClsLines();
                if (log.IsInfoEnabled) log.Info("Üretilen" + "Malzeme Referansı: " + itemRef + " Reçete Referansı: " + BOMRef + " Revizyon Referansı: " + RevRef + " Üretilecek Miktar: " + PlnAmount.ToString() + " " + " " + " İrsaliye Tarihi: " + ficheDate.ToString() + " Üretim Fişi No " + ficheNo + " Mamül Birim Referansı " + uomR);
                try
                {
                    int prodOrdRef = prodApp.ProdOrderAutomaticGenerate(itemRef, BOMRef, RevRef, targetDate, FactoryNr,
                    PlnAmount, itemClsLines, ficheDate, ficheNo, uomR);
                    if (log.IsInfoEnabled) log.Info("Üretim Emri Oluştruldu");
                    if (prodOrdRef == 0)
                    {

                        var error = prodApp.GetLastError().ToString() + " : " + prodApp.GetLastErrorString().ToString();
                        if (log.IsWarnEnabled) log.Warn("Irsaliye No:" + " " + item.StficheFicheNo.ToString() + " " + "MalzemeRef" + " " + itemRef + "Urun Kodu:" + " " + item.ItemCode.ToString() + " " + "Urun Adı:" + " " + item.ItemName.ToString() + "Sube Kodu:" + " " + item.StficheBranch.ToString() + " " + "Hata Aciklaması:" + " " + error);
                        var errorUnity = GlobalParameters.UnityApp.GetLastError() + GlobalParameters.UnityApp.GetLastErrorString();
                        if (log.IsErrorEnabled) log.Error(errorUnity);
                        result.ErrorId = 0;
                        result.ErrorMessage = error;
                        Logger.Logger.LogError(error);
                    }
                    else
                    {
                        result.ErrorId = 0;
                        result.ErrorMessage = "";
                        result.Result = prodOrdRef;
                        if (groupParam)
                        {
                            if (log.IsInfoEnabled) log.Info("Irsaliye No:" + " " + "Gruplanmis Uretim" + "Urun Kodu:" + " " + item.ItemCode.ToString() + "Urun Adı:" + " " + item.ItemName.ToString() + "Sube Kodu:" + " " + item.StficheBranch.ToString() + " " + "Miktar:" + " " + item.StlineAmount.ToString() + " " + "uretildi");
                        }
                        else
                        {

                            if (log.IsInfoEnabled) log.Info("Üretim Emri Bilgileri: Irsaliye No:" + " " + item.StficheFicheNo.ToString() + " Urun Kodu:" + " " + item.ItemCode.ToString() + "Urun Adı:" + " " + item.ItemName.ToString() + "Sube Kodu:" + " " + item.StficheBranch.ToString() + " " + "Miktar:" + " " + item.StlineAmount.ToString() + " " + "için üretim emri oluşturuldu");
                        }
                        MBGOP_ProductOrder productOrder = null;
                        try
                        {
                            productOrder = new MBGOP_ProductOrder
                            {
                                BomMasterLogicalRef = item.BomMasterLogicalRef,
                                BomRevSnLogicalref = item.BomRevSnLogicalref,
                                CapiDivName = item.CapiDivName,
                                IsThere = item.IsThere,
                                ItemCode = item.ItemCode,
                                ItemLogicalref = item.ItemLogicalref,
                                ItemName = item.ItemName,
                                StficheBranch = item.StficheBranch,
                                StficheDate = item.StficheDate ?? orderDate,
                                StficheFicheNo = item.StficheFicheNo ?? "Gruplanmis Uretim",
                                StficheLogicalref = item.StficheLogicalref ?? 0,
                                StlineAmount = item.StlineAmount,
                                UnitSetLCode = item.UnitSetLCode,
                                UnitSetLLogicalref = item.UnitSetLLogicalref,
                                UnitSetLName = item.UnitSetLName

                            };
                            var ficheResult = new MBGOP_ProdOrderFiche
                            {
                                ProdOrderRef = prodOrdRef,
                                ItemRef = item.ItemLogicalref.Value,
                                BomRef = BOMRef,
                                ProdOrderFicheNo = ficheNo,
                                FactoryNr = FactoryNr,
                                PlnAmount = PlnAmount,
                                RevRef = RevRef,
                                UomRef = uomR
                            };
                            //var ficheResult = SaveProductorderFiche(new MBGOP_ProdOrderFiche
                            //{
                            //    ProdOrderRef = prodOrdRef,
                            //    ItemRef = item.ItemLogicalref.Value,
                            //    BomRef = BOMRef,
                            //    ProdOrderFicheNo = ficheNo,
                            //    FactoryNr = FactoryNr,
                            //    PlnAmount = PlnAmount,
                            //    RevRef = RevRef,
                            //    UomRef = uomR
                            //});
                            //if (ficheResult)
                            //{
                            #region ProductOrder_Continue
                            result.State = groupParam ? UpdateStatus(ficheNo, orderDate, groupParam, productOrder).State : UpdateStatus(ficheNo, item.StficheDate.Value, groupParam, productOrder).State;
                            if (result.State)
                            {
                                #region ProductOrder_GenerateFiche
                                var slips = FastRealizeFicheGenerate(ficheNo, orderDate, groupParam, productOrder, ficheResult);
                                if (slips.State)
                                {
                                    #region ProductOrder_Completed

                                    var completed = UpdateStatusCompleted(ficheNo);
                                    if (completed.State)
                                    {
                                        #region ProductOrderStatus_Closed
                                        if (!UpdateStatusColesed(ficheNo).State)
                                        {
                                            Logger.Logger.LogError("Üretim Emri durumu kapandı olarak değiştirilirken hata meydana geldi.");
                                            result.State = false;
                                            result.ErrorId = (int)ChangePOAndWOStatusError.Completed;
                                            result.ErrorMessage = "Üretim Emri durumu kapandı olarak değiştirilirken hata meydana geldi.";
                                            result.Result = null;

                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        Logger.Logger.LogError("Üretim Emri durumu tamamlandı olarak değiştirilirken hata meydana geldi kayıtlar geri alınıyor.");
                                        result.State = false;
                                        result.ErrorId = (int)ChangePOAndWOStatusError.Completed;
                                        result.ErrorMessage = "Üretim Emri durumu tamamlandı olarak değiştirilirken hata meydana geldi kayıtlar geri alınıyor.";
                                        result.Result = null;
                                        RollBackProdOrder(ficheNo, productOrder);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    Logger.Logger.LogError(result.ErrorMessage);
                                    result.State = false;
                                    result.ErrorId = (int)ChangePOAndWOStatusError.Slips;
                                    result.ErrorMessage = slips.ErrorMessage;
                                    result.Result = null;
                                }
                                #endregion
                            }
                            else
                            {
                                Logger.Logger.LogError(result.ErrorMessage);
                                result.State = false;
                                result.ErrorId = (int)ChangePOAndWOStatusError.Continue;
                                result.ErrorMessage = result.ErrorMessage;
                                result.Result = null;
                                RollBackProdOrder(ficheNo, productOrder);
                            }
                            #endregion
                            //}
                        }
                        catch (Exception)
                        {
                            RollBackProdOrder(ficheNo, productOrder);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled) log.Error(ex.Message.ToString());
                }
            }
            if (log.IsInfoEnabled) log.Info("Uretim emri olusturuldu----------------------------------------**********************************------------------------------------------------ \n");
            return result;
        }
        public static ResultType UpdateStatus(string ficheNumber, DateTime tarih, bool group, MBGOP_ProductOrder productOrder)
        {
            var result = new ResultType();
            var prodApp = new UnityObjects.ProductionApplication();
            string ficheNo = ficheNumber;
            int status = 1;
            int typ = 1;
            bool opTrans = true;
            short delStFc = 1;

            result.State = prodApp.ChangePOAndWOStatus(ficheNo, status, typ, opTrans, delStFc);

            if (!result.State)
            {
                var error = prodApp.GetLastError().ToString() + " : " + prodApp.GetLastErrorString().ToString();
                if (log.IsErrorEnabled) log.Error("Fis durum devam ediyor Hata: " + error);
                result.ErrorId = 2;
                result.ErrorMessage = error.ToString();

            }
            else
            {
                result.ErrorId = 0;
                if (log.IsInfoEnabled) log.Info(ficheNumber + " 'nolu üretim emri durumu devam ediyor olarak ayarlandi");
                //FastRealizeFicheGenerate(ficheNumber, tarih, group, productOrder);
                //UpdateStatusCompleted(ficheNumber);
            }

            return result;
        }
        public static ResultType FastRealizeFicheGenerate(string ficheNumber, DateTime tarih, bool group, MBGOP_ProductOrder productOrder, MBGOP_ProdOrderFiche productOrderFiche)
        {
            var result = new ResultType();
            using (var context = new LOGOProductOrderContext())
            {

                //var prodOrderFiche = context.MBGOP_ProdOrderFiche.FirstOrDefault(x => x.ProdOrderFicheNo == ficheNumber);
                //var fichdate = context.MBGOP_ProductOrder.FirstOrDefault(x => x.StficheFicheNo == ficheNumber).StficheDate;
                var prodApp = new UnityObjects.ProductionApplication();
                //var prodApp = new UnityObjects.UnityApplication().NewProductionApplication();

                //if (prodOrderFiche != null)
                //{
                int prodOrdRef = productOrderFiche.ProdOrderRef;
                int ItemRef = productOrderFiche.ItemRef;
                double prodAmnt = productOrderFiche.PlnAmount;
                int uomRef = productOrderFiche.UomRef;
                int method = 1;
                bool removeSidePrdct = true;
                var date = tarih.AddHours(22).AddMinutes(30).AddSeconds(00);
                DateTime fcDate = date;
                int vrntRef = 0;

                UnityObjects.FastRealizeSlipRefLists SlipLists = prodApp.NewSlipRefLists();

                result.State = prodApp.FastRealizeFicheGenerate(prodOrdRef, ItemRef, prodAmnt, uomRef, method, removeSidePrdct, SlipLists, fcDate, vrntRef);

                string txt = "";
                if (result.State)
                {

                    if (log.IsInfoEnabled) log.Info(ficheNumber + "'nolu üretim emrine bagli uretimden giris ve sarf fisleri olusturuldu");

                    txt = "";
                    for (int i = 0; i < SlipLists.InputfromProdSlips.Count; i++)
                    {
                        txt = txt + "\n Üretimden Giriş Fişi Referansı \t : " + SlipLists.InputfromProdSlips.Item[i].lref.ToString();
                    }

                    for (int i = 0; i < SlipLists.UsageSlips.Count; i++)
                    {
                        txt = txt + "\n Sarf Fişi Referansı            \t : " + SlipLists.UsageSlips.Item[i].lref.ToString();
                    }

                    for (int i = 0; i < SlipLists.ScarpSlips.Count; i++)
                    {
                        txt = txt + "\n Fire Fişi Referansı            \t : " + SlipLists.ScarpSlips.Item[i].lref.ToString();
                    }

                    if (log.IsInfoEnabled) log.Info(ficheNumber + " Uretim Emri Referans: " + prodOrdRef + " Malzeme Referans: " + ItemRef + " Uretim Miktarı: " + prodAmnt + " Bagli Fis Referanslari: " + txt);
                    var check = ProdSlipCheck(ficheNumber);
                    if (check == 0)
                    {
                        //UpdateStatusCompleted(ficheNumber);
                        UpdateProdOrderTime(prodOrdRef);
                        var savedOrder = SaveProductorder(
                            new MBGOP_ProductOrder
                            {
                                BomMasterLogicalRef = productOrder.BomMasterLogicalRef,
                                BomRevSnLogicalref = productOrder.BomRevSnLogicalref,
                                CapiDivName = productOrder.CapiDivName,
                                IsThere = true,
                                ItemCode = productOrder.ItemCode,
                                ItemLogicalref = productOrder.ItemLogicalref,
                                ItemName = productOrder.ItemName,
                                StficheBranch = productOrder.StficheBranch,
                                StficheDate = productOrder.StficheDate,
                                StficheFicheNo = productOrder.StficheFicheNo ?? "Gruplanmis Uretim",
                                StficheLogicalref = productOrder.StficheLogicalref ?? 0,
                                StlineAmount = productOrder.StlineAmount,
                                UnitSetLCode = productOrder.UnitSetLCode,
                                UnitSetLLogicalref = productOrder.UnitSetLLogicalref,
                                UnitSetLName = productOrder.UnitSetLName
                            });
                        //var ficheResult = SaveProductorderFiche(productOrderFiche);
                        //var deleteProductOrderFiche = DeleteProductOrderFiche(ficheResult.Id);
                        result.ErrorId = 0;
                        result.ErrorMessage = "";
                        result.Result = txt;
                    }
                    else
                    {
                        var diff = ProdSlipGetDiff(ficheNumber);
                        string error = null;
                        int counter = 0;
                        foreach (var item in diff)
                        {
                            counter++;
                            error += counter.ToString() + ")" + item.CODE + " / " + item.NAME +
                                    " / " + item.SOURCEINDEX+"\n";
                        }
                        counter = 0;

                        if (log.IsInfoEnabled)
                            log.Info(ficheNumber + "'nolu fişe ait gerçekleşen üretim fişinde olmayan satırlar \n" + error);
                        if (log.IsInfoEnabled) log.Info("Üretim Emrine bağlı planlanan ve gerçekleşen satırı uyumsuzluğu yüzünden kayıtlar çıkartılıyor.");

                        result.State = false;
                        result.ErrorId = (int)ChangePOAndWOStatusError.Slips;
                        result.ErrorMessage = "Üretim Emri planlanan ve gerçekleşen sarf fişleri uyumsuzluğu var sarf satırlarını stok miktarları kontrol edilmelidir. Kayıtlar geri alınıyor.\n------------------****************------------------\n";
                        result.Result = null;
                        Logger.Logger.LogError("\nŞube No: " + productOrder.StficheBranch + " İrsaliye Tarihi: " + productOrder.StficheDate.ToString() + " İrsaliye No: " + productOrder.StficheFicheNo + " Malzeme Kodu: " + productOrder.ItemCode + " Malzeme Adı: " + productOrder.ItemName + " Üretilecek Miktar: " + productOrder.StlineAmount + " " + ficheNumber + "'nolu fişe ait gerçekleşen üretim fişinde olmayan satırlar \n" + error +  result.ErrorMessage);
                        RollBackProdOrder(ficheNumber, productOrder);
                    }
                }
                else
                {
                    RollBackProdOrder(ficheNumber, productOrder);

                    var error = prodApp.GetLastError().ToString() + " : " + prodApp.GetLastErrorString().ToString();
                    if (log.IsErrorEnabled) log.Error("Gerceklesen miktar girisi hata aciklamasi" + " " + error);
                    if (log.IsErrorEnabled) log.Error("Bagli fisler hata" + " " + txt);
                    result.ErrorId = 3;
                    result.ErrorMessage = error.ToString();
                }
                //}
                if (log.IsInfoEnabled) log.Info(ficheNumber + "'nolu fişe ait gerceklesen miktar girisi metodu tamamlandi");
                return result;
            }


        }
        public static ResultType UpdateStatusCompleted(string ficheNumber)
        {
            var result = new ResultType();
            //if (log.IsInfoEnabled) log.Info("Fis durum tamamlandi çalişti");
            var prodApp = new UnityObjects.ProductionApplication();

            string ficheNo = ficheNumber;   // Üretim emri veya iş emrinin numarası
            int status = 3;                     // 0 (Başlamadı), 1 (Devam ediyor), 2 (Durduruldu), 3 (Tamamlandı), 4 (Kapandı) 
            int typ = 1;                        // 1 (üretim emri), 2 (iş emri) 
            bool opTrans = true;                // True : Transaction kullanılsın, False : Transaction kullanılmasın
            short delStFc = 1;                  // 0 (bağlantıları kopar), 1 (fişleri sil), 2 (silme)

            result.State = prodApp.ChangePOAndWOStatus(ficheNo, status, typ, opTrans, delStFc);

            if (!result.State)
            {
                var error = prodApp.GetLastError().ToString() + " : " + prodApp.GetLastErrorString().ToString();
                if (log.IsErrorEnabled) log.Error(ficheNumber + "'nolu Fis durum tamamlandi Hata: " + error);
                result.ErrorId = 4;
                result.ErrorMessage = error.ToString();
            }
            else
            {
                result.ErrorId = 0;
                if (log.IsInfoEnabled) log.Info(ficheNumber + "'nolu üretim emri durumu tamamlandi olarak güncellendi");
                //FastRealizeFicheGenerate(ficheNumber);
                UpdateStatusColesed(ficheNumber);
            }
            //if (log.IsInfoEnabled) log.Info("Fis durum tamamlandi bitti");
            return result;
        }
        public static ResultType UpdateStatusColesed(string ficheNumber)
        {
            var result = new ResultType();
            //if (log.IsInfoEnabled) log.Info("Fis durum kapandi basladi");
            var prodApp = new UnityObjects.ProductionApplication();

            string ficheNo = ficheNumber;   // Üretim emri veya iş emrinin numarası
            int status = 4;                     // 0 (Başlamadı), 1 (Devam ediyor), 2 (Durduruldu), 3 (Tamamlandı), 4 (Kapandı) 
            int typ = 1;                        // 1 (üretim emri), 2 (iş emri) 
            bool opTrans = true;                // True : Transaction kullanılsın, False : Transaction kullanılmasın
            short delStFc = 1;                  // 0 (bağlantıları kopar), 1 (fişleri sil), 2 (silme)

            result.State = prodApp.ChangePOAndWOStatus(ficheNo, status, typ, opTrans, delStFc);

            if (!result.State)
            {
                var error = prodApp.GetLastError().ToString() + " : " + prodApp.GetLastErrorString().ToString();
                if (log.IsErrorEnabled) log.Error(ficheNumber + "'nolu Fis durum kapandi Hata: " + error);
                result.ErrorId = 5;
                result.ErrorMessage = error.ToString();
            }
            else
            {
                if (log.IsInfoEnabled) log.Info(ficheNumber + "'nolu üretim emri durumu kapandi olarak güncellendi");
            }

            return result;
        }
        public static ResultType ProdOrderAutomaticRealize(string ficheNumber)
        {
            var result = new ResultType();
            var prodApp = GlobalParameters.UnityApp.NewProductionApplication();
            int pro = prodApp.ProdOrderAutomaticRealize(GetProdOrderFicheRef(ficheNumber), 100);
            if (pro > 0)
            {
                //MessageBox.Show("ok");
            }
            else
            {
                //MessageBox.Show(ProdApp.GetLastErrorString());
            }
            return result;
        }
        public static void RollBackProdOrder(string ficheNumber, MBGOP_ProductOrder productOrder)
        {
            var prodApp = new UnityObjects.ProductionApplication();
            //var listUniq = orderList.GroupBy(x => new { x.ProductName, x.Quantity }).Select(group => group.First()).ToList();
            using (var context = new LOGOProductOrderContext())
            {
                var listUniq = context.MBGOP_ProductOrder
                    .FirstOrDefault(x => x.IsThere == false && x.StficheDate == productOrder.StficheDate &&
                                x.ItemCode == productOrder.ItemCode && x.StficheBranch == productOrder.StficheBranch && x.StficheFicheNo == productOrder.StficheFicheNo && x.StlineAmount == productOrder.StlineAmount);
                if (listUniq == null)
                {
                    var untranfer = SaveProductorder(new MBGOP_ProductOrder
                    {
                        BomMasterLogicalRef = productOrder.BomMasterLogicalRef,
                        BomRevSnLogicalref = productOrder.BomRevSnLogicalref,
                        CapiDivName = productOrder.CapiDivName,
                        IsThere = false,
                        ItemCode = productOrder.ItemCode,
                        ItemLogicalref = productOrder.ItemLogicalref,
                        ItemName = productOrder.ItemName,
                        StficheBranch = productOrder.StficheBranch,
                        StficheDate = productOrder.StficheDate,
                        StficheFicheNo = productOrder.StficheFicheNo ?? "Gruplanmis Uretim",
                        StficheLogicalref = productOrder.StficheLogicalref ?? 0,
                        StlineAmount = productOrder.StlineAmount,
                        UnitSetLCode = productOrder.UnitSetLCode,
                        UnitSetLLogicalref = productOrder.UnitSetLLogicalref,
                        UnitSetLName = productOrder.UnitSetLName
                    });
                }

            }

            var prodOrderContinue = prodApp.ChangePOAndWOStatus(ficheNumber, 1, 1, true, 1);
            if (prodOrderContinue)
            {
                var prodOrderNotStart = prodApp.ChangePOAndWOStatus(ficheNumber, 0, 1, true, 1);
                if (prodOrderNotStart)
                {
                    prodApp.ProdOrderCancel(GetProdOrderFicheRef(ficheNumber), true);
                    if (log.IsInfoEnabled)
                        log.Info("Planlanan ve gerçekleşen üretim satırı uyumsuzluğu sebebiyle " + " İrsaliye Tarihi: " + productOrder.StficheDate + " İrsaliye No: " + productOrder.StficheFicheNo);

                }
            }

        }
        #endregion
    }

    public class ProductOrderDTO
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public short SOURCEINDEX { get; set; }
    }
}