using PlasDal;
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
        private CommonDal cdal = new CommonDal();
        //获取高级搜索分类
        public List<parminfo> listparminfo(string parentid, string middlename, string type)
        {
            List<parminfo> list = new List<parminfo>();
            try
            {
                //DataTable dt = bll.GetClass(parentid, middlename, type);
                List<parminfo> templist = bll.GetClassNew(parentid, middlename, type);
                if (templist.Count > 0)
                {
                    for (int i = 0; i < templist.Count; i++)
                    {
                        parminfo m = new parminfo();
                        string tempname = templist[i].Name;
                        m.Name = tempname;
                        m.Guid = templist[i].parentguid;// dt.Rows[i]["parentguid"].ToString();
                        //如果是一级分类
                        if (type == "0")
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
        //获取微信请求code用于匹配是否请求过
        public DataTable GetWxCodeCheck(string code)
        {
            return cdal.GetWxCodeCheck(code);
        }
        //添加微信请求code
        public void AddWxCode(string code)
        {
            cdal.AddWxCode(code);
        }
    }
}
