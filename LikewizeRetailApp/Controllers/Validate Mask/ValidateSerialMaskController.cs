using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LikewizeRetailApp.Models.Validate_Serial_Mask;
using LikewizeRetailApp.ServiceReference1;
using Newtonsoft.Json;

namespace LikewizeRetailApp.Controllers.Validate_Mask
{
    public class ValidateSerialMaskController : Controller
    {
        // GET: ValidateSerialMask
        //public ActionResult ValidateSerialMask()
        //{
        //    ViewBag.ErrorMessage = null;
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult ValidateSerialMask(ValidateSerialMask maskObj)
        //{
        //    SnsServicesClient snsServicesClient = new SnsServicesClient();
            
        //    string CpnyID = maskObj.CpnyID;
        //    string InvtID = maskObj.InvtID;
        //    string SerialNumber = maskObj.SerialNumber;
        //    string TranType = maskObj.TranType;
        //    string PalletNumber = maskObj.PalletNumber;
            
        //   ReturnStatus errMsg = snsServicesClient.ValidateSerialMask(CpnyID, InvtID, SerialNumber, Convert.ToInt32(TranType), PalletNumber);
        //    var val = JsonConvert.SerializeObject(errMsg);

        //    ViewBag.ErrorMessage = errMsg.Message;
        //    return View();
        //}

        public JsonResult ValidateSerialMask(string serialnumber)
        {
            string CpnyID = "311";
            string InvtID = "MPTL2B/A";
            int TranType = 1;
            string PalletNumber = "J6966759";
            SnsServicesClient snsServicesClient = new SnsServicesClient();
            ReturnStatus errMsg = snsServicesClient.ValidateSerialMask(CpnyID, InvtID, serialnumber, TranType, PalletNumber);
            string msg = errMsg.Message.ToString();
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}