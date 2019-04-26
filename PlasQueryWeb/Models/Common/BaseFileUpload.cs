using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasQueryWeb.Models.Common
{
    public class BaseFileUpload
    {
        /// <summary>
        /// 路径
        ///  <para>为空表示使用默认路径</para>
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 取消重命名
        /// </summary>
        public bool IsResetName { get; set; }

        /// <summary>
        /// 图片服务器
        /// </summary>
        public UploadServer Server { get; set; } = UploadServer.Local;
    }
}