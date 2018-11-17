using System;
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

            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("Index", itemsAsIPagedList)
                    : View("Index", itemsAsIPagedList);
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
            }
            catch (Exception ex)
            {
                string message = ex.Message;

            }
            return View();
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
            const int pageSize = 5;
            int totalCount = 5;
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
        public ActionResult importexcel()
        {

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
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("ImportLaneRate", L)
                : View(L);
        }


    }
}