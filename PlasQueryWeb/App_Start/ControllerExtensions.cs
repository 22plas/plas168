using Newtonsoft.Json;
using PlasCommon;
using PlasModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.App_Start
{
    public static class ControllerExtensions
    {
        public static void SetAccountData(this Controller controller, AccountData data)
        {
            controller.Response.Cookies["AccountData"].Value = Common.Encrypt(JsonConvert.SerializeObject(data));
            controller.Response.Cookies["AccountData"].Expires = DateTime.Now.AddYears(1);
        }

        public static AccountData GetAccountData(this Controller controller)
        {
            if (controller.Request.Cookies["AccountData"] != null)
            {
                //throw new Exception("AccountData is null");
                return JsonConvert.DeserializeObject<AccountData>("");
            }
            var data = controller.Request.Cookies["AccountData"].Value;
            var decData = Common.Decrypt(data);
            return JsonConvert.DeserializeObject<AccountData>(decData);
        }
    }
}