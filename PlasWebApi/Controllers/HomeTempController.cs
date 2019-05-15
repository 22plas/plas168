using PlasModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlasWebApi.Controllers
{
    public class HomeTempController : ApiController
    {
        /// <summary>
        /// 加载行情走势接口
        /// </summary>
        /// <returns></returns>
        public List<PriceModle> GetQuotation(string SmallClass,string Manufacturer,string Model,int pageindex=1,int pagesize=4)
        {
            //var bll = new PlasBll.ProductBll();
            //var list = new List<PriceModle>();
            //var ds = new DataSet();
            //ds = bll.getPriceFile(SmallClass, Manufacturer, Model, pageindex, pagesize);
            //list = PlasCommon.ToolClass<PriceModle>.ConvertDataTableToModel(ds.Tables[0]);
            return null;
        }

    }
}
