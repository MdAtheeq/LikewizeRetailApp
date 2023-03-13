using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using LikewizeRetailApp.Models;
using PagedList.Mvc;
using PagedList;

namespace LikewizeRetailApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        string connectionString = @"Data Source = IND-L599\SQLEXPRESS; Initial Catalog = RetailDB; Integrated Security = True";
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCustomerDetails_Mst(int? id)
        {
            List<CustomerDetails> customerDetailsList = new List<CustomerDetails>();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from CustomerDetails ";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);

                sqlDa.Fill(dtblObject);

            }
            if(dtblObject.Rows.Count > 0)
            {
                for (int i = 0; i < dtblObject.Rows.Count; i++)
                {
                    CustomerDetails customerDetails = new CustomerDetails();
                    customerDetails.CustomerDetailsID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                    customerDetails.CustomerName = dtblObject.Rows[i][1].ToString();
                    customerDetails.Email = dtblObject.Rows[i][2].ToString();
                    customerDetails.PhoneNumber = dtblObject.Rows[i][3].ToString();
                    customerDetailsList.Add(customerDetails);
                }
            }
            return View(customerDetailsList.ToPagedList(id ?? 1,3));
        }

        [HttpPost]
        public ActionResult GetCustomerDetails_Mst(string email, int? id)
        {
            List<CustomerDetails> customerDetailsList = new List<CustomerDetails>();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from CustomerDetails where Email = @Email";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Email", email);
                sqlDa.Fill(dtblObject);

            }
            if (dtblObject.Rows.Count > 0)
            {
                for (int i = 0; i < dtblObject.Rows.Count; i++)
                {
                    CustomerDetails customerDetails = new CustomerDetails();
                    customerDetails.CustomerDetailsID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                    customerDetails.CustomerName = dtblObject.Rows[i][1].ToString();
                    customerDetails.Email = dtblObject.Rows[i][2].ToString();
                    customerDetails.PhoneNumber = dtblObject.Rows[i][3].ToString();
                    customerDetailsList.Add(customerDetails);
                }
            }
            return View(customerDetailsList.ToPagedList(id ?? 1, 3));
        }


        public ActionResult CreateCustomerDetails_Mst()
        {
            return View(new CustomerDetails());
        }

        [HttpPost]
        public ActionResult CreateCustomerDetails_Mst(CustomerDetails customerDetails)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "INSERT INTO CustomerDetails VALUES (@CustomerName,@Email,@PhoneNumber)";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


                sqlCmd.Parameters.AddWithValue("@CustomerName", customerDetails.CustomerName);
                sqlCmd.Parameters.AddWithValue("@Email", customerDetails.Email);
                sqlCmd.Parameters.AddWithValue("@PhoneNumber", customerDetails.PhoneNumber);

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetCustomerDetails_Mst", "Admin");
        }

        public ActionResult UpdateCustomerDetails_Mst(int custID)
        {
            CustomerDetails customerDetails = new CustomerDetails();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from CustomerDetails where CustomerID=@CustId ";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@CustId", custID);
                sqlDa.Fill(dtblObject);

            }
            if (dtblObject.Rows.Count == 1)
            {
                customerDetails.CustomerDetailsID = Convert.ToInt32(dtblObject.Rows[0][0].ToString());
                customerDetails.CustomerName = dtblObject.Rows[0][1].ToString();
                customerDetails.Email = dtblObject.Rows[0][2].ToString();
                customerDetails.PhoneNumber = dtblObject.Rows[0][3].ToString();
                return View(customerDetails);
            }
            return View();

        }

        [HttpPost]
        public ActionResult UpdateCustomerDetails_Mst(CustomerDetails customerDetails)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Update CustomerDetails set CustomerName=@CustomerName, Email=@Email, PhoneNumber=@PhoneNumber where CustomerID= @CustomerDetailsID ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);

                sqlCmd.Parameters.AddWithValue("@CustomerName", customerDetails.CustomerName);
                sqlCmd.Parameters.AddWithValue("@Email", customerDetails.Email);
                sqlCmd.Parameters.AddWithValue("@PhoneNumber", customerDetails.PhoneNumber);
                sqlCmd.Parameters.AddWithValue("@CustomerDetailsID", customerDetails.CustomerDetailsID);

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetCustomerDetails_Mst", "Admin");
        }

        public ActionResult DeleteCustomerDetails_Mst(int custId)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Delete from CustomerDetails where CustomerID= @CustomerID ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


                sqlCmd.Parameters.AddWithValue("@CustomerID", custId);

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetCustomerDetails_Mst", "Admin");
        }


        public ActionResult GetProductDetails_Mst(int? id, string ProductType, string Brand)
        {
            GetProductType();
            List<ProductDetails> products = new List<ProductDetails>();

            ProductDetails productDetails = new ProductDetails();
            DataTable dtblObject = new DataTable();
            
            if(ProductType == null && Brand == null)
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "Select * from ProductDetails ";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);

                    sqlDa.Fill(dtblObject);

                }
                if (dtblObject.Rows.Count > 0)
                {
                    for (int i = 0; i < dtblObject.Rows.Count; i++)
                    {
                        ProductDetails pd = new ProductDetails();
                        pd.ProductDetailsID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                        pd.ProductType = dtblObject.Rows[i][1].ToString();
                        pd.Brand = dtblObject.Rows[i][2].ToString();
                        pd.Model = dtblObject.Rows[i][3].ToString();
                        products.Add(pd);
                    }

                }
                return View(products.ToPagedList(id ?? 1, 3));
            }
            else
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "Select * from ProductDetails where ProductType = @ProductType and Brand = @Brand";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@ProductType", ProductType);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@Brand", Brand);


                    sqlDa.Fill(dtblObject);

                }
                if (dtblObject.Rows.Count > 0)
                {
                    for (int i = 0; i < dtblObject.Rows.Count; i++)
                    {
                        ProductDetails pd = new ProductDetails();
                        pd.ProductDetailsID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                        pd.ProductType = dtblObject.Rows[i][1].ToString();
                        pd.Brand = dtblObject.Rows[i][2].ToString();
                        pd.Model = dtblObject.Rows[i][3].ToString();
                        products.Add(pd);
                    }

                }
                return View(products.ToPagedList(id ?? 1, 3));
            }
                
            
            
            

        }
        [HttpPost]
        public ActionResult GetProductDetails_Mst(int? id, string productType, string Brand, string x)
        {
            GetProductType();
            List<ProductDetails> products = new List<ProductDetails>();

            ProductDetails productDetails = new ProductDetails();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from ProductDetails where ProductType=@ProductType and Brand = @Brand ";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProductType", productType);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Brand", Brand);

                sqlDa.Fill(dtblObject);

            }
            if (dtblObject.Rows.Count > 0)
            {
                for (int i = 0; i < dtblObject.Rows.Count; i++)
                {
                    ProductDetails pd = new ProductDetails();
                    pd.ProductDetailsID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                    pd.ProductType = dtblObject.Rows[i][1].ToString();
                    pd.Brand = dtblObject.Rows[i][2].ToString();
                    pd.Model = dtblObject.Rows[i][3].ToString();
                    products.Add(pd);
                }

            }
            return View(products.ToPagedList(id ?? 1, 4));

        }





        public ActionResult CreateProductDetails_Mst()
        {
            GetProductType();
            //ViewBag.BrandList1 = new SelectList(ViewBag.BrandList, "Text", "Value");
            //ViewBag.BrandList = new SelectList(new List<string>(), "Text", "Value");
            return View(new ProductDetails());
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
            foreach (DataRow dr in dtblObject.Rows)
            {
                list.Add(new SelectListItem { Text = dr["ProductType"].ToString(), Value = dr["ProductType"].ToString() });
            }
            ViewBag.ProductTypeList = list;
        }

        [HttpPost]

        public ActionResult CreateProductDetails_Mst(ProductDetails productDetails)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "INSERT INTO ProductDetails VALUES (@ProductType,@Brand,@Model)";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);

                
                sqlCmd.Parameters.AddWithValue("@ProductType", productDetails.ProductType);
                sqlCmd.Parameters.AddWithValue("Brand", productDetails.Brand);
                sqlCmd.Parameters.AddWithValue("@Model", productDetails.Model);

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetProductDetails_Mst", "Admin");
        }

        public ActionResult UpdateProductDetails_Mst(int prodId)
        {
            GetProductType();
            ProductDetails productDetails = new ProductDetails();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from ProductDetails where ProductDetailsID=@ProdId ";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProdId", prodId);
                sqlDa.Fill(dtblObject);

            }
            if (dtblObject.Rows.Count == 1)
            {
                productDetails.ProductDetailsID = Convert.ToInt32(dtblObject.Rows[0][0].ToString());
                productDetails.ProductType = dtblObject.Rows[0][1].ToString();
                productDetails.Brand = dtblObject.Rows[0][2].ToString();
                productDetails.Model = dtblObject.Rows[0][3].ToString();
                GetBrandName(productDetails.ProductType);
                return View(productDetails);

               

            }

            return View();
        }

        public void GetBrandName(string prodType)
        {
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select Distinct(Brand) from ProductDetails";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                //sqlDa.SelectCommand.Parameters.AddWithValue("@ProductType", prodType);

                sqlDa.Fill(dtblObject);
            }

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dtblObject.Rows)
            {
                list.Add(new SelectListItem { Text = dr["Brand"].ToString(), Value = dr["Brand"].ToString() });
            }
            ViewBag.BrandTypeList = list;
        }

        [HttpPost]
        public ActionResult UpdateProductDetails_Mst(ProductDetails productDetails)
        {

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Update ProductDetails set ProductType=@ProductType, Brand=@Brand, Model=@Model where ProductDetailsID= @ProductDetailsID ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);

                sqlCmd.Parameters.AddWithValue("@ProductType", productDetails.ProductType);
                sqlCmd.Parameters.AddWithValue("@Brand", productDetails.Brand);
                sqlCmd.Parameters.AddWithValue("@Model", productDetails.Model);
                sqlCmd.Parameters.AddWithValue("@ProductDetailsID", productDetails.ProductDetailsID);

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetProductDetails_Mst", "Admin");
        }



        public ActionResult DeleteProductDetails_Mst(int prodId)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Delete from ProductDetails where ProductDetailsID= @ProductDetailsID ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


                sqlCmd.Parameters.AddWithValue("@ProductDetailsID", prodId);

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetProductDetails_Mst", "Admin");
        }

        public ActionResult GetQuestions_Mst(int? id, string ProductType)
        {
            GetProductType();
            DataTable dtblObject = new DataTable();
            if(ProductType == null)
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "Select * from Questions ";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);

                    sqlDa.Fill(dtblObject);

                }
                List<Questions> Questions = new List<Questions>();
                if (dtblObject.Rows.Count > 0)
                {
                    for (int i = 0; i < dtblObject.Rows.Count; i++)
                    {
                        Questions question = new Questions();
                        question.QuestionID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                        question.Question = dtblObject.Rows[i][1].ToString();
                        question.Answer = dtblObject.Rows[i][2].ToString();

                        Questions.Add(question);
                    }

                }

                return View(Questions.ToPagedList(id ?? 1, 3));
            }
            else
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "Select * from Questions where Category = @ProductType ";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@ProductType", ProductType);

                    sqlDa.Fill(dtblObject);

                }
                List<Questions> Questions = new List<Questions>();
                if (dtblObject.Rows.Count > 0)
                {
                    for (int i = 0; i < dtblObject.Rows.Count; i++)
                    {
                        Questions question = new Questions();
                        question.QuestionID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                        question.Question = dtblObject.Rows[i][1].ToString();
                        question.Answer = dtblObject.Rows[i][2].ToString();

                        Questions.Add(question);
                    }

                }

                return View(Questions.ToPagedList(id ?? 1, 3));
            }
           
           
        }

        [HttpPost]

        public ActionResult GetQuestions_Mst(int? id, string productType, string x)
        {
            GetProductType();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from Questions where Category = @Category ";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Category", productType);

                sqlDa.Fill(dtblObject);

            }
            List<Questions> Questions = new List<Questions>();
            if (dtblObject.Rows.Count > 0)
            {
                for (int i = 0; i < dtblObject.Rows.Count; i++)
                {
                    Questions question = new Questions();
                    question.QuestionID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                    question.Question = dtblObject.Rows[i][1].ToString();
                    question.Answer = dtblObject.Rows[i][2].ToString();

                    Questions.Add(question);
                }

            }

            return View(Questions.ToPagedList(id ?? 1, 10));

        }

        public ActionResult CreateQuestions_Mst()
        {
            GetProductType();

            return View(new Questions());
        }

        [HttpPost]
        public ActionResult CreateQuestions_Mst(Questions questions)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "INSERT INTO Questions VALUES (@Question,@Category)";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


                sqlCmd.Parameters.AddWithValue("@Question", questions.Question);
                sqlCmd.Parameters.AddWithValue("Category", questions.Answer);
              

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetQuestions_Mst", "Admin");
        }

        public ActionResult UpdateQuestions_Mst(int questId)
        {
            GetProductType();
            Questions questions = new Questions();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from Questions where QuestionID=@QuestId ";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@QuestId", questId);
                sqlDa.Fill(dtblObject);

            }
            if (dtblObject.Rows.Count == 1)
            {
                questions.QuestionID = Convert.ToInt32(dtblObject.Rows[0][0].ToString());
                questions.Question = dtblObject.Rows[0][1].ToString();
                questions.Answer = dtblObject.Rows[0][2].ToString();
                
                return View(questions);
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateQuestions_Mst(Questions questions)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Update Questions set Question=@Question, Category=@Category where QuestionID = @QuestionID ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);

                sqlCmd.Parameters.AddWithValue("@Question", questions.Question);
                sqlCmd.Parameters.AddWithValue("@Category", questions.Answer);
                sqlCmd.Parameters.AddWithValue("@QuestionID", questions.QuestionID);
                

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetQuestions_Mst", "Admin");
        }

        public ActionResult DeleteQuestions_Mst(int questId)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Delete from Questions where QuestionID= @QuestionID ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


                sqlCmd.Parameters.AddWithValue("@QuestionID", questId);

                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetQuestions_Mst", "Admin");
        }



        public JsonResult ValidateEmail(string email)
        {
            string Count = "";
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from CustomerDetails where Email = @Email";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Email", email);
        

                sqlDa.Fill(dtblObject);
            }

            if(dtblObject.Rows.Count > 0)
            {
                Count = "Yes";
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Count = "No";
                return Json(Count, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult CheckModel(string model)
        {
            string Count = "";
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select * from ProductDetails where Model = @Model";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Model", model);


                sqlDa.Fill(dtblObject);
            }

            if (dtblObject.Rows.Count > 0)
            {
                Count = "Yes";
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Count = "No";
                return Json(Count, JsonRequestBehavior.AllowGet);

            }
        }
    }
}