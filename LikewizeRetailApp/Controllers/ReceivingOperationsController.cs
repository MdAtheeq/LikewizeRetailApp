using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using LikewizeRetailApp.Models;

namespace LikewizeRetailApp.Controllers
{
    [Authorize]
    public class ReceivingOperationsController : Controller
    {
        string connectionString = @"Data Source = IND-L599\SQLEXPRESS; Initial Catalog = RetailDB; Integrated Security = True";
        // GET: ReceivingOperations
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartupPage()
        {
            return View();
        }

        public ActionResult GetSerialDetails()
        {
            DataTable dtblproduct = new DataTable();
            return View(dtblproduct);
        }

        [HttpPost]
        public ActionResult GetSerialDetails(string serialnumber)
        {
            OrderDetails orderDetails = new OrderDetails();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select top 1 ReceivedItemID, SerialNumber, Status from OrderDetails where SerialNumber=@SerialNumber order by OrderID desc";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("SerialNumber", serialnumber);
                sqlDa.Fill(dtblObject);
            }

            ViewBag.IsSuccess = "Yes";

            return View(dtblObject);
        }

        string tempnumber = "";
        [HttpGet]
        public ActionResult UpdateDeviceDetails()
        {
            GetProductType();
            ReceivedItems receivedItems = new ReceivedItems();  
            //DataTable dtblObject = new DataTable();
            //using (SqlConnection sqlCon = new SqlConnection(connectionString))
            //{
            //    sqlCon.Open();
            //    string query = "Select top 1 SerialNumber from OrderDetails where SerialNumber=@SerialNumber order by OrderID desc";
            //    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
            //    sqlDa.SelectCommand.Parameters.AddWithValue("SerialNumber", serialnumber);
            //    sqlDa.Fill(dtblObject);
            //}

            //if(dtblObject.Rows.Count==1)
            //{
            //    receivedItems.SerialNumber = dtblObject.Rows[0][0].ToString();
            //    tempnumber = receivedItems.SerialNumber;
            //}
            //tempnumber = receivedItems.SerialNumber;
            return View(receivedItems);
        }


        [HttpPost]
        public ActionResult UpdateDeviceDetails(string email)
        {
            GetProductType();
            ReceivedItems receivedItems = new ReceivedItems();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from CustomerDetails where Email = @Email";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Email", email);
                sqlDa.Fill(dtblObject);
            }

            if (dtblObject.Rows.Count == 1)
            {
                receivedItems.CustomerID = Convert.ToInt32(dtblObject.Rows[0][0].ToString());
                receivedItems.CustomerName = dtblObject.Rows[0][1].ToString();
                receivedItems.Email = dtblObject.Rows[0][2].ToString();
                receivedItems.PhoneNumber = dtblObject.Rows[0][3].ToString();
                receivedItems.SerialNumber = tempnumber;
            }

            ViewBag.IsSuccess = "Yes";
            return View(receivedItems);
        }

        public void GetProductType()
        {
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select Distinct(ProductType) from ProductDetails";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
               
                sqlDa.Fill(dtblObject);
            }

            List<SelectListItem> list = new List<SelectListItem>();
            foreach(DataRow dr in dtblObject.Rows)
            {
                list.Add(new SelectListItem { Text = dr["ProductType"].ToString(), Value = dr["ProductType"].ToString() });
            }
            ViewBag.ProductTypeList = list;
        }

        [HttpPost]

        public JsonResult GetBrand(string prodType)
        {
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select Distinct(Brand) from ProductDetails where ProductType=@ProductType";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProductType", prodType);

                sqlDa.Fill(dtblObject);
            }

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dtblObject.Rows)
            {
                list.Add(new SelectListItem { Text = dr["Brand"].ToString(), Value = dr["Brand"].ToString() });
            }

            ViewBag.BrandTypeList = list;
            return Json(new SelectList(list, "Value", "Text"));
        }

        public JsonResult GetBrandById(int prodID)
        {
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select Distinct(Brand) from ProductDetails where ProductDetailsID=@ProdID";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProdID", prodID);

                sqlDa.Fill(dtblObject);
            }

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dtblObject.Rows)
            {
                list.Add(new SelectListItem { Text = dr["Brand"].ToString(), Value = dr["Brand"].ToString() });
            }

            //ViewBag.BrandTypeList = list;
            return Json(new SelectList(list, "Value", "Text"));
        }

        public JsonResult GetModel(string prodType, string brand)
        {
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select Distinct(Model) from ProductDetails where ProductType=@ProductType and Brand=@Brand";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProductType", prodType);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Brand", brand);

                sqlDa.Fill(dtblObject);
            }

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dtblObject.Rows)
            {
                list.Add(new SelectListItem { Text = dr["Model"].ToString(), Value = dr["Model"].ToString() });
            }
            return Json(new SelectList(list, "Value", "Text"));
        }

        public ActionResult ReceiveItem(ReceivedItems receivedItems)
        {
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select ProductDetailsID from ProductDetails where ProductType = @ProductType and Brand = @Brand and Model = @Model";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProductType", receivedItems.ProductType);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Brand", receivedItems.Brand);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Model", receivedItems.Model);
                sqlDa.Fill(dtblObject);

            }
            if(dtblObject.Rows.Count == 1)
            {
                receivedItems.ProductDetailsID = Convert.ToInt32(dtblObject.Rows[0][0]);
            }
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Insert into ReceivedItems(CustomerID, ProductDetailsID, SerialNumber, Status, DateReceived) values(@CustomerID, @ProductDetailsID, @SerialNumber, 'Received', GetDate())";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@CustomerID", receivedItems.CustomerID);
                sqlCmd.Parameters.AddWithValue("@ProductDetailsID", receivedItems.ProductDetailsID);
                sqlCmd.Parameters.AddWithValue("@SerialNumber", receivedItems.SerialNumber);
                //sqlCmd.Parameters.AddWithValue("@Status", receivedItems.Status);
                sqlCmd.ExecuteNonQuery();
            }

            ViewBag.Message = "Item Received Successfully";
            return RedirectToAction("StartupPage", "ReceivingOperations");
        }
    }
}