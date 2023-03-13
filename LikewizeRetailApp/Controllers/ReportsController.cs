using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Dynamic;
using LikewizeRetailApp.Models;

namespace LikewizeRetailApp.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        string connectionString = @"Data Source = IND-L599\SQLEXPRESS; Initial Catalog = RetailDB; Integrated Security = True";
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetBrands()
        {

            ProductDetails productDetails = new ProductDetails();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select Distinct(Brand) from ProductDetails";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                //sqlDa.SelectCommand.Parameters.AddWithValue("SerialNumber", serialnumber);
                sqlDa.Fill(dtblObject);
            }
            //List<string> brand = new List<string>();
            List<LikewizeRetailApp.Models.BrandReport.BrandReport> brand = new List<LikewizeRetailApp.Models.BrandReport.BrandReport>();
            foreach (DataRow dr in dtblObject.Rows)
            {
                //brand.Add(dr.ToString());
                var brand1 = dr["Brand"];
                DataTable dtblReport = new DataTable();
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "Select Count(Brand) from ProductDetails where Brand = @Brand";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("Brand", brand1);
                    sqlDa.Fill(dtblReport);
                }

                if(dtblReport.Rows.Count ==1)
                {
                    LikewizeRetailApp.Models.BrandReport.BrandReport report = new LikewizeRetailApp.Models.BrandReport.BrandReport();
                    report.Brand = brand1.ToString(); /*dr.ToString();*/
                    report.Count = Convert.ToInt32(dtblReport.Rows[0][0].ToString());

                    brand.Add(report);
                }
            }

            ViewBag.Brand = JsonConvert.SerializeObject(brand);

            return View(brand);





            
        }


        [HttpPost]
        public JsonResult GetBrandChart()
        {
            ProductDetails productDetails = new ProductDetails();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select Distinct(Brand) from ProductDetails";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                //sqlDa.SelectCommand.Parameters.AddWithValue("SerialNumber", serialnumber);
                sqlDa.Fill(dtblObject);
            }
            //List<string> brand = new List<string>();
            List<LikewizeRetailApp.Models.BrandReport.BrandReport> brand = new List<LikewizeRetailApp.Models.BrandReport.BrandReport>();
            foreach (DataRow dr in dtblObject.Rows)
            {
                //brand.Add(dr.ToString());
                var brand1 = dr["Brand"];
                DataTable dtblReport = new DataTable();
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "Select Count(Brand) from ProductDetails where Brand = @Brand";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("Brand", brand1);
                    sqlDa.Fill(dtblReport);
                }

                if (dtblReport.Rows.Count == 1)
                {
                    LikewizeRetailApp.Models.BrandReport.BrandReport report = new LikewizeRetailApp.Models.BrandReport.BrandReport();
                    report.Brand = brand1.ToString(); /*dr.ToString();*/
                    report.Count = Convert.ToInt32(dtblReport.Rows[0][0].ToString());

                    brand.Add(report);
                }
            }

            ViewBag.Brand = JsonConvert.SerializeObject(brand);

            return Json(brand, JsonRequestBehavior.AllowGet);

        }



        public ActionResult GetReceivedBrandByDate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetReceivedBrandByDate(string date)
        {
            TempData["Date"] = date;
            ViewBag.IsSuccess = "Yes";
            return View();
        }



       


        public JsonResult GetReceivedBrandChartByDate(string date)
        {
            TempData["Date"] = date;
            var x = Convert.ToDateTime(TempData["Date"]);
            var y = x.ToLongDateString();
            var z = y.ToString();

            var slice1 = z.Slice(0, 2);
            var slice2 = z.Slice(3, 6);
            var slice3 = slice1.Slice(0, 1);
            var slice4 = "";
            var slice5 = "";

            if (slice3 == "0")
            {
                slice1 = z.Slice(1, 2);
                slice4 = z.Substring(z.Length - 4);

                slice5 = slice2 + "  " + slice1 + " " + slice4;
            }
            else
            {
                slice1 = z.Slice(0, 2);
                slice4 = z.Substring(z.Length - 4);

                slice5 = slice2 + " " + slice1 + " " + slice4;
            }
           
            //string str = "Jan 3 2023";
            //var datetime = new DateTime(long.Parse(date));
            //var actualDate = datetime.ToLongDateString();
            ProductDetails productDetails = new ProductDetails();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select ProductDetailsID from ReceivedItems where DateReceived like '%' + @DateReceived + '%'";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("DateReceived", slice5);
                sqlDa.Fill(dtblObject);
            }


            DataTable dtblProduct = new DataTable();
            foreach (DataRow row in dtblObject.Rows)
            {
                var productID = row["ProductDetailsID"];
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "Select Brand from ProductDetails where ProductDetailsID= @ProductDetailsID";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("ProductDetailsID", productID);
                    sqlDa.Fill(dtblProduct);
                }

            }

            List<string> brand = new List<string>();
            if (dtblProduct.Rows.Count > 0)
            {

                foreach (DataRow dr in dtblProduct.Rows)
                {
                    var Brand = dr["Brand"];
                    brand.Add(Brand.ToString());
                }
            }



            var brandList = brand.GroupBy(k => k).Where(g => g.Count() >= 1).Select(l => new { Brand = l.Key, Count = l.Count() }).ToList();

            

            return Json(brandList, JsonRequestBehavior.AllowGet);

        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Get the string slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// </summary>
        public static string Slice(this string source, int start, int end)
        {
            if (end < 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            int len = end - start;               // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }
    }
}