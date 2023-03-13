using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;
using LikewizeRetailApp.Models;
using System.Net.Http;
using LikewizeRetailApp.Models.Email;
//using System.Net.Http.Json;

namespace LikewizeRetailApp.Controllers
{
    [Authorize]
    public class ProcessingOperationsController : Controller
    {

        string connectionString = @"Data Source = IND-L599\SQLEXPRESS; Initial Catalog = RetailDB; Integrated Security = True";
        // GET: ProcessingOperations
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetReceivedItems()
        {

            ReceivedItems receivedItems = new ReceivedItems();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT ReceivedItemID, CustomerID, ProductDetailsID, SerialNumber, Status, DateReceived FROM ReceivedItems";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                //sqlDa.SelectCommand.Parameters.AddWithValue("SerialNumber", serialnumber);
                sqlDa.Fill(dtblObject);
            }
            return View(dtblObject);
        }

        [HttpPost]

        public ActionResult GetReceivedItems(string serialnumber)
        {

            ReceivedItems receivedItems = new ReceivedItems();
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT top 1 ReceivedItemID, CustomerID, ProductDetailsID, SerialNumber, Status, DateReceived FROM ReceivedItems where SerialNumber=@SerialNumber order by ReceivedItemID desc";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@SerialNumber", serialnumber);
                sqlDa.Fill(dtblObject);
            }

            ViewBag.IsSuccess = "Yes";
            return View(dtblObject);
        }

        string Serialnumber = "";
        int ReceivingId = 0;


        public ActionResult EvaluateSerial(string serialnumber, int prodId, int receivingId)
        {
            ProcessedItems processedItems = new ProcessedItems();
            processedItems.SerialNumber = serialnumber;
            ViewData["SerialNumber"] = serialnumber;
            //Serialnumber = serialnumber;
            //ReceivingId = receivingId;

            TempData["SerialNumber"] = serialnumber;
            TempData["ReceivedItemID"] = receivingId;

            ProductDetails productDetails = new ProductDetails();
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT ProductType FROM ProductDetails where ProductDetailsID=@ProdId";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProdId", prodId);
                sqlDa.Fill(dtbl);
            }
            if(dtbl.Rows.Count==1)
            {
                productDetails.ProductType = dtbl.Rows[0][0].ToString();
            }


            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT QuestionID, Question FROM Questions where Category=@ProdType";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProdType", productDetails.ProductType);
                sqlDa.Fill(dtblObject);
            }
            List<Questions> Questions = new List<Questions>();
            if(dtblObject.Rows.Count > 0)
            {
                for(int i=0; i<dtblObject.Rows.Count; i++)
                {
                    Questions questions = new Questions();
                    questions.QuestionID = Convert.ToInt32(dtblObject.Rows[i][0].ToString());
                    questions.Question = dtblObject.Rows[i][1].ToString();
                    questions.Answer = "";

                    Questions.Add(questions);

                }
                
            }
            return View(Questions);
        }

       

        string grade = "";
        string repaircost = "";

        public ActionResult GetGrade(IEnumerable<LikewizeRetailApp.Models.Questions> questions, ProcessedItems processedItems)
        {
           
            //qa = q + num.ToString() + a;
            //qnum = q + num.ToString();
            int count = 0;
            List<string> list = new List<string>();
            foreach (var item in questions)
            {
                list.Add(item.Answer);
            }

            foreach(var item in list)
            {
                if(item=="Yes")
                {
                    count += 1;
                }


            }

            if(count<2)
            {
                processedItems.Grade = "A";
            }
            else if(count<5)
            {
                processedItems.Grade = "B";
            }
            else
            {
                processedItems.Grade = "C";
            }

            if(processedItems.Grade == "A")
            {
                processedItems.RepairCost = "2000";
            }
            else if(processedItems.Grade == "B")
            {
                processedItems.RepairCost = "4000";
            }
            else
            {
                processedItems.RepairCost = "6000";
            }

            ViewData["SerialNumber"] = TempData["SerialNumber"];

            grade = processedItems.Grade;
            TempData["Grade"] = processedItems.Grade;
            ViewData["Grade"] = grade;
            
            repaircost = processedItems.RepairCost;
            TempData["RepairCost"] = processedItems.RepairCost;
            ViewData["RepairCost"] = repaircost;

            TempData["SerialNumber"] = TempData["SerialNumber"];
            TempData["ReceivedItemID"] = TempData["ReceivedItemID"];

            return View(questions);
        }

       

        //public ActionResult CompleteProcessing(IEnumerable<LikewizeRetailApp.Models.Questions> questions)
        //{

        //    string q = "Q";
        //    string a = "A";
        //    int num = 1;
        //    string qnum = "";
        //    string qa = "";


        //    foreach (var item in questions)
        //    {
        //        qa = q + num.ToString() + a;
        //        qnum = q + num.ToString();

        //        if (num == 1)
        //        {
        //            using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //            {
        //                sqlCon.Open();
        //                string query = "INSERT INTO DefectDetails(ReceivedItemID, SerialNumber, Q1, Q1A) VALUES (@ReceivedItemID,@SerialNumber,@Q1, @Q1A)";
        //                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


        //                sqlCmd.Parameters.AddWithValue("@ReceivedItemID", TempData["ReceivedItemID"]);
        //                sqlCmd.Parameters.AddWithValue("@SerialNumber", TempData["SerialNumber"]);
        //                sqlCmd.Parameters.AddWithValue("@Q1", item.QuestionID);
        //                sqlCmd.Parameters.AddWithValue("@Q1A", item.Answer);

        //                sqlCmd.ExecuteNonQuery();
        //            }

        //            num += 1;
        //        }

        //        else
        //        {
        //            using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //            {
        //                sqlCon.Open();
        //                string query = "Update DefectDetails set " + qnum + " = " + "@Qnum" + ", " + qa + " = " + "@Qa " + " where ReceivedItemID = @ReceivingId";
        //                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


        //                //sqlCmd.Parameters.AddWithValue("@Qx", qnum);
        //                sqlCmd.Parameters.AddWithValue("@Qnum", item.QuestionID);
        //                //sqlCmd.Parameters.AddWithValue("@QxA", qa);
        //                sqlCmd.Parameters.AddWithValue("@Qa", item.Answer);
        //                sqlCmd.Parameters.AddWithValue("@ReceivingId", TempData["ReceivedItemID"]);

        //                sqlCmd.ExecuteNonQuery();
        //            }
        //            num += 1;
        //        }

               
        //    }

        //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //    {
        //        sqlCon.Open();
        //        string query = "INSERT INTO ProcessedItems(ReceivedItemID, SerialNumber, Grade, RepairCost, Status, DateProcessed) VALUES (@ReceivedItemID,@SerialNumber,@Grade, @RepairCost, 'Processed', GetDate())";
        //        SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


        //        sqlCmd.Parameters.AddWithValue("@ReceivedItemID", TempData["ReceivedItemID"]);
        //        sqlCmd.Parameters.AddWithValue("@SerialNumber", TempData["SerialNumber"]);
        //        sqlCmd.Parameters.AddWithValue("@Grade", TempData["Grade"]);
        //        sqlCmd.Parameters.AddWithValue("@RepairCost", TempData["RepairCost"]);

        //        sqlCmd.ExecuteNonQuery();
        //    }



        //    return RedirectToAction("StartupPage", "ReceivingOperations");
        //}

        public ActionResult CompleteProcessing(IEnumerable<LikewizeRetailApp.Models.Questions> questions)
        {
            foreach(var item in questions)
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "INSERT INTO DefectDetails(ReceivedItemID, SerialNumber, QuestionID, Answer) VALUES (@ReceivedItemID,@SerialNumber,@QuestionID, @Answer)";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


                    sqlCmd.Parameters.AddWithValue("@ReceivedItemID", TempData["ReceivedItemID"]);
                    sqlCmd.Parameters.AddWithValue("@SerialNumber", TempData["SerialNumber"]);
                    sqlCmd.Parameters.AddWithValue("@QuestionID", item.QuestionID);
                    sqlCmd.Parameters.AddWithValue("@Answer", item.Answer);

                    sqlCmd.ExecuteNonQuery();
                }
            }
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "INSERT INTO ProcessedItems(ReceivedItemID, SerialNumber, Grade, RepairCost, Status, DateProcessed) VALUES (@ReceivedItemID,@SerialNumber,@Grade, @RepairCost, 'Processed', GetDate())";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


                sqlCmd.Parameters.AddWithValue("@ReceivedItemID", TempData["ReceivedItemID"]);
                sqlCmd.Parameters.AddWithValue("@SerialNumber", TempData["SerialNumber"]);
                sqlCmd.Parameters.AddWithValue("@Grade", TempData["Grade"]);
                sqlCmd.Parameters.AddWithValue("@RepairCost", TempData["RepairCost"]);

                sqlCmd.ExecuteNonQuery();
            }


            ViewBag.Message = "Item Processed Successfully";
            return RedirectToAction("StartupPage", "ReceivingOperations");

        }


        public ActionResult GetProcessedItems()
        {
            DataTable dtblObject = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT ProcessedItemID, ReceivedItemID, SerialNumber, Grade, RepairCost, Status, DateProcessed FROM ProcessedItems";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                //sqlDa.SelectCommand.Parameters.AddWithValue("SerialNumber", serialnumber);
                sqlDa.Fill(dtblObject);
            }
            return View();
        }

        [HttpPost]
        //public ActionResult GetProcessedItems(string serialnumber)
        //{
        //    DataTable dtblObject = new DataTable();
        //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //    {
        //        sqlCon.Open();
        //        string query = "SELECT top 1 ProcessedItemID, ReceivedItemID, SerialNumber, Grade, RepairCost, Status, DateProcessed FROM ProcessedItems where SerialNumber=@SerialNumber order by ReceivedItemID desc";
        //        SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
        //        sqlDa.SelectCommand.Parameters.AddWithValue("SerialNumber", serialnumber);
        //        sqlDa.Fill(dtblObject);
        //    }

        //    ViewBag.IsSuccess = "Yes";
        //    return View(dtblObject);
        //}

        public ActionResult GetProcessedItems(string serialnumber)
        {
            DataTable dtblObject = new DataTable();
            DataTable custInfo = new DataTable();
            DataTable productInfo = new DataTable();
            DataTable gradeInfo = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT top 1 ReceivedItemID FROM ProcessedItems where SerialNumber=@SerialNumber order by ReceivedItemID desc";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("SerialNumber", serialnumber);
                sqlDa.Fill(dtblObject);
            }

            if(dtblObject.Rows.Count ==1)
            {
                DataTable custID = new DataTable();
                using(SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "SELECT CustomerID FROM ReceivedItems where ReceivedItemID=@ReceivedItemID";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("ReceivedItemID", Convert.ToInt32(dtblObject.Rows[0][0].ToString()));
                    sqlDa.Fill(custID);
                }
                
                if (custID.Rows.Count == 1)
                {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        sqlCon.Open();
                        string query = "SELECT CustomerName, Email, PhoneNumber FROM CustomerDetails where CustomerID=@CustomerID";
                        SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                        sqlDa.SelectCommand.Parameters.AddWithValue("@CustomerID", Convert.ToInt32(custID.Rows[0][0].ToString()));
                        sqlDa.Fill(custInfo);
                    }
                }

                DataTable productID = new DataTable();
                using( SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "SELECT ProductDetailsID FROM ReceivedItems where ReceivedItemID=@ReceivedItemID";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("ReceivedItemID", Convert.ToInt32(dtblObject.Rows[0][0].ToString()));
                    sqlDa.Fill(productID);
                }

                if(productID.Rows.Count==1)
                {
                    
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        sqlCon.Open();
                        string query = "SELECT ProductType, Brand, Model FROM ProductDetails where ProductDetailsID=@ProductDetailsID";
                        SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                        sqlDa.SelectCommand.Parameters.AddWithValue("@ProductDetailsID", Convert.ToInt32(productID.Rows[0][0].ToString()));
                        sqlDa.Fill(productInfo);
                    }
                }
                
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "SELECT top 1 SerialNumber, Grade, RepairCost FROM ProcessedItems where ReceivedItemID=@ReceivedItemID";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@ReceivedItemID", Convert.ToInt32(dtblObject.Rows[0][0].ToString()));
                    sqlDa.Fill(gradeInfo);
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(custInfo);
                ds.Tables.Add(productInfo);
                ds.Tables.Add(gradeInfo);
                ds.Tables.Add(dtblObject);

                ViewBag.IsSuccess = "Yes";
                return View(ds);
            }
            else
            {
                DataSet ds = new DataSet();
                ViewBag.IsSuccess = "Yes";
                return View(ds);
            }
            
            

        }

        public ActionResult PlaceOrder(string serialnumber, string receivingId)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "INSERT INTO OrderDetails(ReceivedItemID, SerialNumber, OrderDate, Status, DeliveryDate) VALUES (@ReceivedItemID,@SerialNumber, GetDate(), 'Active', 'Yet to be delivered')";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);


                sqlCmd.Parameters.AddWithValue("@ReceivedItemID", receivingId);
                sqlCmd.Parameters.AddWithValue("@SerialNumber", serialnumber);
              

                sqlCmd.ExecuteNonQuery();
            }

            EmailTrigger(serialnumber, receivingId);
            return RedirectToAction("StartupPage", "ReceivingOperations");

        }

        public ActionResult GetOrders()
        {
            OrderDetails orderDetails = new OrderDetails();
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT * FROM OrderDetails";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                //sqlDa.SelectCommand.Parameters.AddWithValue("@ProdId", prodId);
                sqlDa.Fill(dtbl);
            }
            //if (dtbl.Rows.Count > 0 )
            //{
            //    orderDetails.OrderID = Convert.ToInt32(dtbl.Rows[0][0].ToString());
            //    orderDetails.ReceivedItemID = Convert.ToInt32(dtbl.Rows[0][1].ToString());
            //    orderDetails.SerialNumber = dtbl.Rows[0][2].ToString();
            //    orderDetails.OrderDate = dtbl.Rows[0][3].ToString();
            //    orderDetails.Status = dtbl.Rows[0][4].ToString();
            //    orderDetails.DeliveryDate = dtbl.Rows[0][5].ToString();

            //    return View(orderDetails);
            //}
            return View(dtbl);
        }


        public ActionResult ViewOrderDetails(int receivingId)
        {
             DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select OrderDetails.OrderID, OrderDetails.ReceivedItemID, ReceivedItems.CustomerID,  OrderDetails.SerialNumber, ProcessedItems.Grade, ProcessedItems.RepairCost, OrderDetails.OrderDate, OrderDetails.Status, OrderDetails.DeliveryDate from ((OrderDetails inner join ReceivedItems on OrderDetails.ReceivedItemID = ReceivedItems.ReceivedItemID) inner join ProcessedItems on ProcessedItems.ReceivedItemID = OrderDetails.ReceivedItemID) where OrderDetails.ReceivedItemID = @ReceivedItemID";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ReceivedItemID", receivingId);
                sqlDa.Fill(dtbl);
            }

            return View(dtbl);
        }

        public ActionResult GetReport(string receivingId)
        {
            string OrderID = "Order ID";
            string ReceivedItemID = "Received Item ID";
            string CustomerID = "Customer ID";
            string SerialNumber = "Serial Number";
            string Grade = "Grade";
            string RepairCost = "Repair Cost";
            string OrderDate = "Order Date";
            string Status = "Status";
            string DeliveryDate = "Delivery Date";

            StringBuilder sb = new StringBuilder();
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "Select top 1 OrderDetails.OrderID, OrderDetails.ReceivedItemID, ReceivedItems.CustomerID,  OrderDetails.SerialNumber, ProcessedItems.Grade, ProcessedItems.RepairCost, OrderDetails.OrderDate, OrderDetails.Status, OrderDetails.DeliveryDate from ((OrderDetails inner join ReceivedItems on OrderDetails.ReceivedItemID = ReceivedItems.ReceivedItemID) inner join ProcessedItems on ProcessedItems.ReceivedItemID = OrderDetails.ReceivedItemID) where OrderDetails.ReceivedItemID = @ReceivedItemID";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ReceivedItemID", receivingId);
                sqlDa.Fill(dtbl);
            }

            if(dtbl.Rows.Count == 1)
            {
                sb.Append(OrderID + "," + dtbl.Rows[0][0].ToString());
                sb.Append("\r\n");
                sb.Append(ReceivedItemID + "," + dtbl.Rows[0][1].ToString());
                sb.Append("\r\n");
                sb.Append(CustomerID + "," + dtbl.Rows[0][2].ToString());
                sb.Append("\r\n");
                sb.Append(SerialNumber + "," + dtbl.Rows[0][3].ToString());
                sb.Append("\r\n");
                sb.Append(Grade + "," + dtbl.Rows[0][4].ToString());
                sb.Append("\r\n");
                sb.Append(RepairCost + "," + dtbl.Rows[0][5].ToString());
                sb.Append("\r\n");
                sb.Append(OrderDate + "," + dtbl.Rows[0][6].ToString());
                sb.Append("\r\n");
                sb.Append(Status + "," + dtbl.Rows[0][7].ToString());
                sb.Append("\r\n");
                sb.Append(DeliveryDate + "," + dtbl.Rows[0][8].ToString());
                sb.Append("\r\n");

                //OrderID = dtbl.Rows[0][0].ToString();
                //ReceivedItemID = dtbl.Rows[0][1].ToString();
                //CustomerID = dtbl.Rows[0][2].ToString();
                //SerialNumber = dtbl.Rows[0][3].ToString();
                //Grade = dtbl.Rows[0][4].ToString();
                //RepairCost = dtbl.Rows[0][5].ToString();
                //OrderDate = dtbl.Rows[0][6].ToString();
                //Status = dtbl.Rows[0][7].ToString();
                //DeliveryDate = dtbl.Rows[0][8].ToString();


            }
            string path = @"C:\Exported Data\Data ";
            path = path + DateTime.Now.ToFileTime() + ".csv";
            StreamWriter file = new StreamWriter(path);
            file.Write(sb.ToString());
            file.Close();

            return RedirectToAction("ViewOrderDetails", "ProcessingOperations", new { @receivingId = receivingId });
        }

        
        public void EmailTrigger(string serialnumber,string receivingID)
        {
            DataTable custInfo = new DataTable();
            DataTable productInfo = new DataTable();
            DataTable custID = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT CustomerID FROM ReceivedItems where ReceivedItemID=@ReceivedItemID";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("ReceivedItemID", receivingID);
                sqlDa.Fill(custID);
            }
            if(custID.Rows.Count==1)
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "SELECT CustomerName, Email FROM CustomerDetails where CustomerID=@CustomerID";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@CustomerID", Convert.ToInt32(custID.Rows[0][0].ToString()));
                    sqlDa.Fill(custInfo);
                }
            }
            DataTable productID = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT ProductDetailsID FROM ReceivedItems where ReceivedItemID=@ReceivedItemID";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("ReceivedItemID", receivingID);
                sqlDa.Fill(productID);
            }

            if (productID.Rows.Count == 1)
            {

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "SELECT ProductType, Brand, Model FROM ProductDetails where ProductDetailsID=@ProductDetailsID";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@ProductDetailsID", Convert.ToInt32(productID.Rows[0][0].ToString()));
                    sqlDa.Fill(productInfo);
                }
            }
            DataTable gradeInfo = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT top 1 SerialNumber, Grade, RepairCost FROM ProcessedItems where ReceivedItemID=@ReceivedItemID";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ReceivedItemID", receivingID);
                sqlDa.Fill(gradeInfo);
            }
            string name = custInfo.Rows[0][0].ToString();
            string to = custInfo.Rows[0][1].ToString();
            string subject = "Order Confirmation From Likewize";
            
            string body = "Hi " + name + ", \n \n" + "Your order request for " + productInfo.Rows[0][1].ToString() + " " + productInfo.Rows[0][2].ToString() + " has been successfully placed. Your product has been graded as Grade " + gradeInfo.Rows[0][1].ToString() + " and the Repair Cost is Rs. " + gradeInfo.Rows[0][2] + ". You can use the Redeem Code 'Az234ft' during delivery to avail 20% off on the total price. \n \n" + "With regards, \n" +  "Likewize Retail Team";
            
            EmailClass ec = new EmailClass();
            ec.To = to;
            ec.Subject = subject;
            ec.Body = body;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44300/api/Email");
            var consumewebapi = client.PostAsJsonAsync<EmailClass>("Email", ec);
            consumewebapi.Wait();
            var sendmail = consumewebapi.Result;
            
        }
    }
}