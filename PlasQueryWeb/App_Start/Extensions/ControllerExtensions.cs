using Newtonsoft.Json;
using PlasCommon;
using PlasModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PlasCommon.Enums;

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
        /// <summary>
        /// 跳转到错误页面
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="head"></param>
        /// <param name="message"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public static ActionResult ToError(this Controller controller, string head,
            string message, string returnUrl = null, Layout layout = Layout.Manage)
        {
            var view = new ViewResult();
            view.ViewName = "Error";
            view.ViewBag.Head = string.IsNullOrWhiteSpace(head) ? "错误" : head;
            view.ViewBag.Message = message + (string.IsNullOrWhiteSpace(returnUrl) ? "" : $"<a href='{controller.Url.Content(returnUrl)}'>点击返回</a>");
            view.ViewBag.Layout = layout;
            return view;
        }
        public static string Download(this Controller controller, string url, string path = "~/Upload",
          string requestName = null, bool isResetName = false)
        {
            var request = System.Net.WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/octet-stream";
            using (var response = request.GetResponse())
            {
                if (response.ContentLength == 0)
                    return null;

                string name = null;

                string extension = null;
                switch (response.ContentType.ToLower())
                {
                    case "image/jpeg":
                        extension = ".jpg";
                        break;
                    case "image/png":
                        extension = ".png";
                        break;
                    case "image/gif":
                        extension = ".gif";
                        break;
                    case "video/mpeg4":
                        extension = ".mp4";
                        break;
                    case "audio/mp3":
                        extension = ".mp3";
                        break;
                    default:
                        extension = "." + response.ContentType.Remove(0, response.ContentType.IndexOf("/"));
                        break;
                }
                string filePath;
                do
                {

                    name = $"{path}/{DateTime.Now:yyyyMMddHHmmss}{Comm.Random.Next(1000, 9999)}{extension}";

                    filePath = controller.Server.MapPath(name);
                } while (File.Exists(filePath));
                using (var responseStream = response.GetResponseStream())
                {

                    using (var stream = new MemoryStream())
                    {
                        const int bufferLength = 1024;
                        int actual;
                        byte[] buffer = new byte[bufferLength];
                        while ((actual = responseStream.Read(buffer, 0, bufferLength)) > 0)
                        {
                            stream.Write(buffer, 0, actual);
                        }
                        stream.Position = 0;
                        using (FileStream fileStream = System.IO.File.Create(filePath, (int)stream.Length))
                        {
                            // Fill the bytes[] array with the stream data
                            byte[] bytesInStream = new byte[response.ContentLength];
                            stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                            // Use FileStream object to write to the specified file
                            fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                        }
                    }


                }

                return name;
            }
        }
    }
}