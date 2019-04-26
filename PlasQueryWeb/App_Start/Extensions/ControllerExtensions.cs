using Newtonsoft.Json;
using PlasCommon;
using PlasModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasModel.App_Start
{
    public static class ControllerExtensions
    {

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="path">路径</param>
        /// <param name="requestName">目标的Request.Name,为空时候全部上传</param>
        /// <returns></returns>
        public static ICollection<string> UploadFile(this Controller controller, string path = "~/Upload",
            string requestName = null, bool isResetName = false)
        {
            var Server = controller.Server;
            var Request = controller.Request;
            string dir = controller.Server.MapPath(path);

            string[] images = new string[Request.Files.Count];
            if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0)
            {
                return new string[0];
            }
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            List<string> fileNames = new List<string>();
            Action<HttpPostedFileBase> saveFile = item =>
            {
                FileInfo file = new FileInfo(item.FileName);
                string filePath;
                string name = $"{path}/{DateTime.Now:yyyyMMddHHmmss}{Comm.Random.Next(1000, 9999)}{file.Extension}";

                do
                {

                    if (isResetName)
                    {
                        name = $"{path}/{file.Name}";
                    }
                    fileNames.Add(name);
                    filePath = Server.MapPath(name);
                } while (File.Exists(filePath));
                item.SaveAs(filePath);
                var cloudFile = new FileInfo(Server.MapPath(name));

            };
            if (string.IsNullOrWhiteSpace(requestName))
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    saveFile(Request.Files[i]);
                }
            }
            else
            {
                if (Request.Files[requestName] == null)
                {
                    throw new Exception($"文件{requestName}不存在");
                }
                saveFile(Request.Files[requestName]);
            }

            return fileNames;
        }
        public static void SetAccountData(this Controller controller, AccountData data)
        {
            controller.Response.Cookies["AccountData"].Value = Common.Encrypt(JsonConvert.SerializeObject(data));
            controller.Response.Cookies["AccountData"].Expires = DateTime.Now.AddYears(1);
        }

        public static AccountData GetAccountData(this Controller controller)
        {
            if (controller.Request.Cookies["AccountData"] == null)
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