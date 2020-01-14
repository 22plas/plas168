using PlasModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasBll
{
   public class CommonBll
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        //获取高级搜索分类
        public  List<parminfo> listparminfo(string parentid, string middlename, string type)
        {
            List<parminfo> list = new List<parminfo>();
            try
            {
                DataTable dt = bll.GetClass(parentid, middlename, type);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        parminfo m = new parminfo();
                        string tempname= dt.Rows[i]["Name"].ToString();
                        m.Name = tempname;
                        m.Guid = dt.Rows[i]["parentguid"].ToString();
                        //如果是一级分类
                        if (type=="0")
                        {
                            if (tempname.Equals("工程塑料"))
                            {
                                m.Image = "../Content/img/gcsl.png";
                            }
                            else if (tempname.Equals("通用塑料"))
                            {
                                m.Image = "../Content/img/tysl.png";
                            }
                            else if (tempname.Equals("特种工程塑料"))
                            {
                                m.Image = "../Content/img/tzgcsl.png";
                            }
                            else if (tempname.Equals("热塑弹性体"))
                            {
                                m.Image = "../Content/img/rstxt.png";
                            }
                            else if (tempname.Equals("降解塑料"))
                            {
                                m.Image = "../Content/img/xjsl.png";
                            }
                            else if (tempname.Equals("橡胶"))
                            {
                                m.Image = "../Content/img/xiangjiaosl.png";
                            }
                            else if (tempname.Equals("热固性塑料"))
                            {
                                m.Image = "../Content/img/rgxsl.png";
                            }
                            else if (tempname.Equals("其它"))
                            {
                                m.Image = "../Content/img/qtsl.png";
                            }
                        }
                        list.Add(m);
                    }
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }
    }
}
