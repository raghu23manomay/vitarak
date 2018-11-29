﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using demomilk.Models;
using System.Data.SqlClient;
using PagedList;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Xml;

namespace demomilk.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(int? page)
        {
            StaticPagedList<RouteDetails> itemsAsIPagedList;
            itemsAsIPagedList = AeraGridList(page);
            Session["MasterName"] = "AreaMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("Index", itemsAsIPagedList)
                    : View("Index", itemsAsIPagedList);
        }

        public ActionResult LoadDataForArea(int? page)
        {
            StaticPagedList<RouteDetails> itemsAsIPagedList;
            itemsAsIPagedList = AeraGridList(page);
          //  Session["MasterName"] = "AreaMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("AdminIndex", itemsAsIPagedList)
                    : View("AdminIndex", itemsAsIPagedList);
        }

        [HttpGet]
        public ActionResult AddArea_Route()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddArea_Route(Route md)
        {
            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_InsertAreaMast @Area,@CityID",
                    new SqlParameter("@Area", md.Area),
                    new SqlParameter("@CityID", 1));
                return Json("Data Added Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }
            
        }

        public ActionResult EditArea_Route()
        {
            return View();

        }


        public ActionResult FetchEditArea(int? AreaID)
        {
            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.EditRoute.SqlQuery(@"exec UC_FetchDataForUpdate_AreaMast @AreaID",
                    new SqlParameter("@AreaID", AreaID)
                   ).ToList<EditRoute>();

                EditRoute rs = new EditRoute();
                rs = res.FirstOrDefault();
                return View("EditArea_Route",rs);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }

        [HttpPost]
        public ActionResult UpdateArea_Route(EditRoute md)
        {
            
            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_UpdateAreaMast @AreaID,@Area,@CityID",
                    new SqlParameter("@Area", md.Area),
                    new SqlParameter("@CityID", 1),
                    new SqlParameter("@AreaID", md.AreaID));
                if(res == 0)
                {
                    return Json("Area is already exist");
                }
                else
                {
                    return Json("Data Updated Sucessfully");
                }

                   
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }

        [HttpPost]
        public ActionResult DeleteArea_Route(EditRoute md)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_DeleteAreaMast @AreaID",
                    new SqlParameter("@AreaID", md.AreaID));

                return Json("Data Deleted Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }

        ////================================== User Index Code ===========================================

        //public ActionResult AdminIndex(int? page)
        //{
        //    StaticPagedList<RouteDetails> itemsAsIPagedList;
        //    itemsAsIPagedList = AeraGridList(page);

        //    return Request.IsAjaxRequest()
        //            ? (ActionResult)PartialView("AdminIndex", itemsAsIPagedList)
        //            : View("AdminIndex", itemsAsIPagedList);
        //}

        //================================== Fill user Grid Code ===========================================

        public StaticPagedList<RouteDetails> AeraGridList(int? page)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalCount = 8;
            RouteDetails Ulist = new RouteDetails();

            IEnumerable<RouteDetails> result = _db.RouteDetails.SqlQuery(@"exec GetAreaList
                   @pPageIndex, @pPageSize",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize)

               ).ToList<RouteDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<RouteDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;



        }


        [HttpGet]
        public ActionResult importexcel(string MasterName = "")
        {
            Session["MasterName"] = MasterName;
            return View();

            //return Request.IsAjaxRequest()
            //? (ActionResult)PartialView("importexcel")
            //: View();
        }


        [HttpPost]
        public ActionResult importexcel(HttpPostedFileBase file, Route L)
        {
            DataTable dt1 = new DataTable();
            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/uploads/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.SetAttributes(fileLocation, FileAttributes.Normal);
                        //   System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);

                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    if (excelConnection.State == ConnectionState.Open)
                    {
                        excelConnection.Close();
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                    if (excelConnection1.State == ConnectionState.Open)
                    {
                        excelConnection1.Close();
                    }
                }
                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/uploads/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                }
                dt1 = ds.Tables[0] as DataTable;
                Session.Add("dt1", dt1);
                L.dtTable = dt1;
            }
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("importexcel", L)
                : View(L);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveAreaExcelData(List<Route> SaveLaneRate)
        {
            
            try
            {               
                JobDbContext _db = new JobDbContext();

                if (SaveLaneRate.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("AreaID", typeof(int));
                    dt.Columns.Add("Area", typeof(string));
                    dt.Columns.Add("CityId", typeof(int));
                    dt.Columns.Add("LastUpdatedDate", typeof(DateTime));


                    foreach (var item in SaveLaneRate)
                    {
                        DataRow dr = dt.NewRow();
                        dr["AreaID"] = 1;
                        dr["Area"] = item.Area;
                        dr["CityId"] = item.CityId;
                        dr["LastUpdatedDate"] = DateTime.Now;

                        if (item.Area != null)
                        {
                            dt.Rows.Add(dr);
                        }
                    }
                
                SqlParameter tvpParam = new SqlParameter();
                tvpParam.ParameterName = "@Area_name";
                tvpParam.SqlDbType = System.Data.SqlDbType.Structured;
                tvpParam.Value = dt;
                tvpParam.TypeName = "UT_AeraMaster1";

                var res = _db.Database.ExecuteSqlCommand(@"exec USP_InsertExcelData_AeraMaster @Area_name",
                 tvpParam);

                }
                // return Request.IsAjaxRequest() ? (ActionResult)PartialView("ImportLaneRate")
                //: View();
                return Request.IsAjaxRequest() ? (ActionResult)Json("Excel Imported Sucessfully")
                : Json("Excel Imported Sucessfully");
            }
            catch(Exception e)

            {
                var messege = e.Message;
                return Request.IsAjaxRequest() ? (ActionResult)Json(messege)
               : Json(messege);
            }
            
            
        }


        //=================================================  Product Master ==================================================

        public ActionResult IndexForproductMaster(int? page)
        {
            StaticPagedList<ProductDetails> itemsAsIPagedList;
            itemsAsIPagedList = ProductGridList(page);

            Session["MasterName"] = "ProductMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("IndexForproductMaster", itemsAsIPagedList)
                    : View("IndexForproductMaster", itemsAsIPagedList);
        }

        public ActionResult LoadDataForProduct(int? page)
        {
            StaticPagedList<ProductDetails> itemsAsIPagedList;
            itemsAsIPagedList = ProductGridList(page);

            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("_partialGridProductMaster", itemsAsIPagedList)
                    : View("_partialGridProductMaster", itemsAsIPagedList);
        }



        //================================== Fill Product Grid Code ===========================================

        public StaticPagedList<ProductDetails> ProductGridList(int? page)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalCount = 8;
            ProductDetails Ulist = new ProductDetails();

            IEnumerable<ProductDetails> result = _db.ProductDetails.SqlQuery(@"exec GetProductList
                   @pPageIndex, @pPageSize",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize)

               ).ToList<ProductDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<ProductDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;



        }




        //========================================== Edit Product ================================================

        public ActionResult EditProduct()
        {
            return View();

        }
                     
        public ActionResult FetchProductForUpdate(int? ProductID)
        {
            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.ProductMaster.SqlQuery(@"exec [UC_FetchDataForUpdate_ProductMaster] @ProductID",
                    new SqlParameter("@ProductID", ProductID)
                   ).ToList<ProductMaster>();

                ProductMaster rs = new ProductMaster();
                rs = res.FirstOrDefault();
                return View("EditProduct", rs);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }

        [HttpPost]
        public ActionResult Updateproduct(ProductMaster rm)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_UpdateProductMast @ProductID ,@Product ,@ProductBrandID ,@StockCount ,@SalePrice,@CrateSize ,@GST",
                    new SqlParameter("@ProductID", rm.ProductID),
                    new SqlParameter("@Product", rm.Product),
                    new SqlParameter("@ProductBrandID", rm.ProductBrandID),
                    new SqlParameter("@StockCount", rm.StockCount),
                    new SqlParameter("@SalePrice", rm.SalePrice == null ? (object)DBNull.Value : rm.SalePrice),
                    new SqlParameter("@CrateSize",rm.CrateSize),
                    new SqlParameter("@GST",rm.GST));

                return Json("Data Updated Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }


        [HttpPost]
        public ActionResult DeleteProduct(ProductMaster rm)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_DeleteProductMast @ProductID",
                    new SqlParameter("@ProductID", rm.ProductID));

                return Json("Data Deleted Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }


        //================================== Save Product Excel Data ===========================================


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveProductExcelData(List<ProductMaster> SaveLaneRate)
        {
            try
            {
                JobDbContext _db = new JobDbContext();

                if (SaveLaneRate.Count > 0)
                {
                    DataTable dt = new DataTable();
                   
                    dt.Columns.Add("ProductID"         , typeof(int)); 
                    dt.Columns.Add("Product"           , typeof(string));
                    dt.Columns.Add("ProductBrandID"    , typeof(int));
                    dt.Columns.Add("CreateDate"        , typeof(DateTime));
                    dt.Columns.Add("CreatedBy"         , typeof(int));
                    dt.Columns.Add("LastUpdatedDate"   , typeof(DateTime));
                    dt.Columns.Add("LastUpdatedBy"     , typeof(int));
                    dt.Columns.Add("isActive"          , typeof(int));
                    dt.Columns.Add("CrateSize"         , typeof(int));
                    dt.Columns.Add("GST", typeof(decimal));

                    foreach (var item in SaveLaneRate)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ProductID"] = 1;
                        dr["Product"] = item.Product;
                        dr["ProductBrandID"] = item.ProductBrandID;
                        dr["CreateDate"] = DateTime.Now;
                        dr["CreatedBy"] = 1;
                        dr["LastUpdatedDate"] = DateTime.Now;
                        dr["LastUpdatedBy"] = 1;
                        dr["isActive"] = 1;
                        dr["CrateSize"] = item.CrateSize;
                        dr["GST"] = item.GST;

                        if (item.Product != null)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    SqlParameter tvpParam = new SqlParameter();
                    tvpParam.ParameterName = "@ProductParameters";
                    tvpParam.SqlDbType = System.Data.SqlDbType.Structured;
                    tvpParam.Value = dt;
                    tvpParam.TypeName = "UT_ProductMaster";

                    var res = _db.Database.ExecuteSqlCommand(@"exec USP_InsertExcelData_ProductMaster @ProductParameters",
                     tvpParam);

                }
                // return Request.IsAjaxRequest() ? (ActionResult)PartialView("ImportLaneRate")
                //: View();
                return Request.IsAjaxRequest() ? (ActionResult)Json("Excel Imported Sucessfully")
                : Json("Excel Imported Sucessfully");
            }
            catch (Exception e)

            {
                var messege = e.Message;
                return Request.IsAjaxRequest() ? (ActionResult)Json(messege)
               : Json(messege);
            }
            
        }

        [HttpGet]
        public ActionResult Add_Product()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductMaster pm)
        {
            JobDbContext _db = new JobDbContext();
            try
            {
                
                var res = _db.Database.ExecuteSqlCommand(@"exec [UC_InsertProductMast] @Product,@ProductBrandID,@StockCount,@SalePrice,@CrateSize,@GST",
                    new SqlParameter("@Product",pm.Product),
                    new SqlParameter("@ProductBrandID",pm.ProductBrandID),
                    new SqlParameter("@StockCount", 1),
                    new SqlParameter("@SalePrice",pm.SalePrice == null ? (object)DBNull.Value : pm.SalePrice),
                    new SqlParameter("@CrateSize", pm.CrateSize),
                    new SqlParameter("@GST", pm.GST));

                return Json("Data Added Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }
            
        }


        /************************************************Add Employee************************************************************/
        [HttpGet]
        public ActionResult Add_Employee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEmployee(Employee pm)
        {
            JobDbContext _db = new JobDbContext();
            try
            {

                var res = _db.Database.ExecuteSqlCommand(@"exec uspInsertEmployee @EmployeeName,@Address,@AreaID,@Mobile",
                    new SqlParameter("@EmployeeName", pm.EmployeeName),
                    new SqlParameter("@Address", pm.Address),
                    new SqlParameter("@AreaID", 1),
                    new SqlParameter("@Mobile", pm.Mobile));

                return Json("Data Added Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }
        public ActionResult IndexForEmployeeMaster(int? page)
        {
            StaticPagedList<EmployeeDetails> itemsAsIPagedList;
            itemsAsIPagedList = EmployeeGridList(page);

            Session["MasterName"] = "EmployeeMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("IndexForEmployeeMaster", itemsAsIPagedList)
                    : View("IndexForEmployeeMaster", itemsAsIPagedList);
        }

        public StaticPagedList<EmployeeDetails> EmployeeGridList(int? page)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalCount = 8;
            EmployeeDetails Ulist = new EmployeeDetails();

            IEnumerable<EmployeeDetails> result = _db.EmployeeDetails.SqlQuery(@"exec GetEmployeeList
                   @pPageIndex, @pPageSize",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize)

               ).ToList<EmployeeDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<EmployeeDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;



        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveEmployeeExcelData(List<Employee> SaveEmployeeData)
        {
            try
            {
                JobDbContext _db = new JobDbContext();

                if (SaveEmployeeData.Count > 0)
                {
                    DataTable dt = new DataTable();

                    dt.Columns.Add("EmployeeID", typeof(int));
                    dt.Columns.Add("EmployeeName", typeof(string));
                    dt.Columns.Add("Address", typeof(string));
                    dt.Columns.Add("AreaID", typeof(int));
                    dt.Columns.Add("Mobile", typeof(string));
                    dt.Columns.Add("UserId", typeof(int));

                    foreach (var item in SaveEmployeeData)
                    {
                        DataRow dr = dt.NewRow();
                        dr["EmployeeID"] = 1;
                        dr["EmployeeName"] = item.EmployeeName;
                        dr["Address"] = item.Address;
                        dr["AreaID"] = 1;
                        dr["Mobile"] = item.Mobile;
                        dr["UserId"] = 1;
                        if (item.EmployeeName != null)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    SqlParameter tvpParam = new SqlParameter();
                    tvpParam.ParameterName = "@EmployeeParameters";
                    tvpParam.SqlDbType = System.Data.SqlDbType.Structured;
                    tvpParam.Value = dt;
                    tvpParam.TypeName = "UT_EmployeeMasters";

                    var res = _db.Database.ExecuteSqlCommand(@"exec USP_InsertExcelData_EmployeeMaster @EmployeeParameters",
                     tvpParam);

                }
                // return Request.IsAjaxRequest() ? (ActionResult)PartialView("ImportLaneRate")
                //: View();
                return Request.IsAjaxRequest() ? (ActionResult)Json("Excel Imported Sucessfully")
                : Json("Excel Imported Sucessfully");
            }
            catch (Exception e)

            {
                var messege = e.Message;
                return Request.IsAjaxRequest() ? (ActionResult)Json(messege)
               : Json(messege);
            }

        }
        /************************************************Add Vehical************************************************************/
        [HttpGet]
        public ActionResult Add_Vehical()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddVehical(Vehical pm)
        {
            JobDbContext _db = new JobDbContext();
            try
            {

                var res = _db.Database.ExecuteSqlCommand(@"exec UC_VehicleMast_Insert @Transport,@Owner,@Address,@Mobile,@VechicleNo,@RatePerTrip,@Marathi,@PrintOrder",
                    new SqlParameter("@Transport", pm.Transport),
                    new SqlParameter("@Owner", pm.Owner),
                    new SqlParameter("@Address",pm.Address),
                    new SqlParameter("@Mobile", pm.Mobile), 
                    new SqlParameter("@VechicleNo", pm.VechicleNo),
                    new SqlParameter("@RatePerTrip", pm.RatePerTrip),
                    new SqlParameter("@Marathi", pm.Marathi),
                    new SqlParameter("@PrintOrder", pm.PrintOrder)
                    );

                return Json("Data Added Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }


        public ActionResult IndexForVehicalMaster(int? page)
        {
            StaticPagedList<VehicalDetails> itemsAsIPagedList;
            itemsAsIPagedList = VehicalGridList(page);

            Session["MasterName"] = "VehicalMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("IndexForVehicalMaster", itemsAsIPagedList)
                    : View("IndexForVehicalMaster", itemsAsIPagedList);
        }

        public StaticPagedList<VehicalDetails> VehicalGridList(int? page)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalCount = 8;
            VehicalDetails Ulist = new VehicalDetails();

            IEnumerable<VehicalDetails> result = _db.VehicalDetails.SqlQuery(@"exec GetVehicalList
                   @pPageIndex, @pPageSize",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize)

               ).ToList<VehicalDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<VehicalDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;



        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveVehicalExcelData(List<Vehical> SaveVehicalData)
        {
            try
            {
                JobDbContext _db = new JobDbContext();

                if (SaveVehicalData.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("VechicleID", typeof(int));
                    dt.Columns.Add("Transport", typeof(string));
                    dt.Columns.Add("Owner", typeof(string));
                    dt.Columns.Add("Address", typeof(string));
                    dt.Columns.Add("Mobile", typeof(string));
                    dt.Columns.Add("VechicleNo", typeof(string));
                    dt.Columns.Add("RatePerTrip", typeof(decimal));
                    dt.Columns.Add("Marathi", typeof(string));
                    dt.Columns.Add("PrintOrder", typeof(int));
                    foreach (var item in SaveVehicalData)
                    {
                        DataRow dr = dt.NewRow();
                        dr["VechicleID"] = 1;
                        dr["Transport"] = item.Transport;
                        dr["Owner"] = item.Owner;
                        dr["Address"] = item.Address;
                        dr["Mobile"] = item.Mobile;
                        dr["VechicleNo"] = item.VechicleNo;
                        dr["RatePerTrip"] = item.RatePerTrip;
                        dr["Marathi"] = item.Marathi;
                        dr["PrintOrder"] = item.PrintOrder;
                        if (item.Transport != null)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    SqlParameter tvpParam = new SqlParameter();
                    tvpParam.ParameterName = "@VehicalParameters";
                    tvpParam.SqlDbType = System.Data.SqlDbType.Structured;
                    tvpParam.Value = dt;
                    tvpParam.TypeName = "UT_VehicalMaster";

                    var res = _db.Database.ExecuteSqlCommand(@"exec USP_InsertExcelData_VehicalMaster @VehicalParameters",
                     tvpParam);

                }
                // return Request.IsAjaxRequest() ? (ActionResult)PartialView("ImportLaneRate")
                //: View();
                return Request.IsAjaxRequest() ? (ActionResult)Json("Excel Imported Sucessfully")
                : Json("Excel Imported Sucessfully");
            }
            catch (Exception e)

            {
                var messege = e.Message;
                return Request.IsAjaxRequest() ? (ActionResult)Json(messege)
               : Json(messege);
            }

        }



        //=================================================  Supplier Master ==================================================

        public ActionResult IndexForSupplierMaster(int? page)
        {
            StaticPagedList<SupplierDetails> itemsAsIPagedList;
            itemsAsIPagedList = SupplierGridList(page);

            Session["MasterName"] = "SupplierMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("IndexForSupplierMaster", itemsAsIPagedList)
                    : View("IndexForSupplierMaster", itemsAsIPagedList);
        }



        public ActionResult LoadDataForSuppier(int? page)
        {
            StaticPagedList<SupplierDetails> itemsAsIPagedList;
            itemsAsIPagedList = SupplierGridList(page);

         //   Session["MasterName"] = "SupplierMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("_PartialGridSupplierList", itemsAsIPagedList)
                    : View("_PartialGridSupplierList", itemsAsIPagedList);
        }



        //================================== Fill Supplier Grid Code ===========================================

        public StaticPagedList<SupplierDetails> SupplierGridList(int? page)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalCount = 8;
            SupplierDetails Ulist = new SupplierDetails();

            IEnumerable<SupplierDetails> result = _db.SupplierDetails.SqlQuery(@"exec GetSupplierList
                   @pPageIndex, @pPageSize",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize)

               ).ToList<SupplierDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<SupplierDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;

        }

        [HttpGet]
        public ActionResult Add_Supplier()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add_Supplier(SupplierMaster pm)
        {
            JobDbContext _db = new JobDbContext();
            try
            {

                var res = _db.Database.ExecuteSqlCommand(@"exec UC_InsertVendorMast @VendorName,@Address,@AreaID,@CityID,@EmailID,@OfficePhone,@FaxNo,@ContactPerson,@PersonMobileNo,@IsActive,@CreatedBy",
                    new SqlParameter("@VendorName", pm.VendorName),
                    new SqlParameter("@Address", pm.Address),
                    new SqlParameter("@AreaID",1),
                    new SqlParameter("@CityID", 1),
                    new SqlParameter("@EmailID", pm.EmailID),
                    new SqlParameter("@OfficePhone", pm.OfficeNumber),
                    new SqlParameter("@FaxNo", pm.FaxNumber),
                    new SqlParameter("@ContactPerson", pm.ContactPerson),
                    new SqlParameter("@PersonMobileNo", pm.PersonMobileNo),
                    new SqlParameter("@IsActive", pm.IsActive),
                    new SqlParameter("@CreatedBy", 1)
                    );

                return Json("Data Added Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }
            
            
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveSupplierExcelData(List<SupplierMaster> SaveSupplierData)
        {
            try
            {
                JobDbContext _db = new JobDbContext();

                if (SaveSupplierData.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("VendorID", typeof(int));
                    dt.Columns.Add("VendorName", typeof(string));
                    dt.Columns.Add("Address", typeof(string));
                    dt.Columns.Add("AreaID", typeof(int));
                    dt.Columns.Add("CityID", typeof(int));
                    dt.Columns.Add("EmailID", typeof(string));
                    dt.Columns.Add("OfficePhone", typeof(string));
                    dt.Columns.Add("FaxNo", typeof(string));
                    dt.Columns.Add("ContactPerson", typeof(string));
                    dt.Columns.Add("PersonMobileNo", typeof(string));
                    dt.Columns.Add("IsActive", typeof(Boolean));
                    dt.Columns.Add("CreatedBy", typeof(int));
                    dt.Columns.Add("CreateDate", typeof(DateTime));
                    dt.Columns.Add("LastUpdatedDate", typeof(DateTime));
                    dt.Columns.Add("LastUpdatedBy", typeof(int));
                          
                    foreach (var item in SaveSupplierData)
                    {
                        DataRow dr = dt.NewRow();
                        dr["VendorID"] = 1;
                        dr["VendorName"] = item.VendorName;
                        dr["Address"] = item.Address;
                        dr["AreaID"] = 1;
                        dr["CityID"] = 1;
                        dr["EmailID"] = item.EmailID;
                        dr["OfficePhone"] = item.OfficeNumber;
                        dr["ContactPerson"] = item.ContactPerson;
                        dr["PersonMobileNo"] = item.PersonMobileNo;
                        dr["IsActive"] = item.IsActive;
                        dr["CreatedBy"] = 1;
                        dr["CreateDate"] = DateTime.Now;
                        dr["LastUpdatedDate"] = DateTime.Now;
                        dr["LastUpdatedBy"] = 1;
                        if (item.VendorName != null)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    SqlParameter tvpParam = new SqlParameter();
                    tvpParam.ParameterName = "@SupplierParameters";
                    tvpParam.SqlDbType = System.Data.SqlDbType.Structured;
                    tvpParam.Value = dt;
                    tvpParam.TypeName = "UT_SupplierMaster";

                    var res = _db.Database.ExecuteSqlCommand(@"exec USP_InsertExcelData_SupplierMaster @SupplierParameters",
                     tvpParam);

                }
                // return Request.IsAjaxRequest() ? (ActionResult)PartialView("ImportLaneRate")
                //: View();
                return Request.IsAjaxRequest() ? (ActionResult)Json("Excel Imported Sucessfully")
                : Json("Excel Imported Sucessfully");
            }
            catch (Exception e)

            {
                var messege = e.Message;
                return Request.IsAjaxRequest() ? (ActionResult)Json(messege)
               : Json(messege);
            }

        }


        //========================================== Edit Supplier ================================================

        public ActionResult EditSupplier()
        {
            return View();

        }

        public ActionResult FetchSupplierForUpdate(int? VendorID)
        {
            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.SupplierMaster.SqlQuery(@"exec [UC_FetchDataForUpdate_VendorMaster] @VendorID",
                    new SqlParameter("@VendorID", VendorID)
                   ).ToList<SupplierMaster>();

                SupplierMaster rs = new SupplierMaster();
                rs = res.FirstOrDefault();
                return View("EditSupplier", rs);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }

        [HttpPost]
        public ActionResult UpdateSupplier(SupplierMaster rm)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec [UC_UpdateVendorMast] @VendorID,@VendorName,@Address,@EmailID,@OfficePhone,@FaxNo,@ContactPerson,@PersonMobileNo,@IsActive,@LastUpdatedBy",
                    new SqlParameter("@VendorID", rm.VendorID),
                    new SqlParameter("@VendorName", rm.VendorName),
                    new SqlParameter("@Address", rm.Address),
                    new SqlParameter("@EmailID", rm.EmailID),
                    new SqlParameter("@OfficePhone", rm.OfficeNumber),
                    new SqlParameter("@FaxNo", rm.FaxNumber),
                    new SqlParameter("@ContactPerson", rm.ContactPerson),
                    new SqlParameter("@PersonMobileNo", rm.PersonMobileNo),
                    new SqlParameter("@IsActive", rm.IsActive),
                    new SqlParameter("@LastUpdatedBy", 1));

                return Json("Data Updated Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }

        [HttpPost]
        public ActionResult DeleteSupplier(int? VendorID)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_DeleteVendorMast @VendorID",
                    new SqlParameter("@VendorID", VendorID));

                return Json("Data Deleted Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }
    }
}