
//初始化vue组件
var vm = new Vue({
    el: '#allbox',
    data: {
        parmlist: [],//条件列表
        showtitle: "点击查看更多查询条件",
        selectitemlist:[]//选中的搜索条件
    }, methods: {
        //加载头部条件数据
        loaddata: function () {
            $.ajax({
                type: "get",
                dataType: "JSON",
                url: '/Price/GetTopParmList',
                data: "",
                async: true,
                success: function (json) {
                    //alert(JSON.stringify(json));
                    var state = json.State;
                    if (state == "Success") {
                        vm.parmlist = json.Result;
                    }
                    else {
                        layer.msg("系统异常！", { icon: 5 });
                    }
                },
                error: function () { layer.msg('未找到数据', { icon: 5 }); }
            });

            //$("html, body").stop().animate({ scrollTop: $("#records").offset().top - 200 }, 400);
        }
    }
});

var pagesize = 14;
    var rowcount = 0;
    var datas = "";
    var page_indx = 0;
    var strcount = 0;

    var dataArry = new Array();//时间
    var priceArry = new Array();//价格

    var SmallClass = "";
    var Manufacturer = "";
var Model = "";
var ProductGuid = "";
    var priceData = "";
    function NoneLine() {
        if ($("#Pagination").find("span").length > 0) {
            $.each($("#Pagination").find("span"), function (index, item) {
                if ($(this).html().indexOf("...") > -1) {
                    $(this).remove();
                }
            })
        }
    }

    $(function () {
        $(".navBox").find(".nav").find("li").eq(1).children("a").addClass("active");
        NoneLine();
        //修改时间格式
    })
    ///点击更多
    $(".search-item-btn").click(function () {
        var dataval = $(this).attr("data-type");
        var datatile = $(this).attr("data-typename");
        if ($("li[name='cailiaotype_" + dataval + "']").hasClass("none")) {
            $.each($("li[name='cailiaotype_" + dataval + "']"), function (index, item) {
                $(this).removeClass("none");
            });
            $("#search_" + dataval).css("display","block");
            $(this).html("收起 ∧");
        }
        else {
            $.each($("li[name='cailiaotype_" + dataval + "']"), function (index, item) {
                $(this).addClass("none");
            })
            $(this).html("更多 ∨");
            $("#search_" + dataval).css("display","none");
        }

        //layer.open({
        //    type: 2,
        //    title: 'MORE-' + datatile,
        //    shadeClose: true,
        //    shade: 0.8,
        //    maxmin: true,
        //    area: ['90%', '90%'],
        //    content: '/Price/More?rid=' + dataval + "&rname=" + datatile + "&more=1"
        //});
    })


    ///选中值，只能单选
$(".search_item_contentList").find("li").click(function () {
        var dataname = $(this).parent().attr("data-typename");//名称
        var datatype = $(this).parent().attr("data-type");//类型
        var dataguid = $(this).attr("data-guid");
        var dataval = $(this).attr("data-value");
        var str = "";
        var istrue = false;
        ///添加样式
        if (dataval == "0") {
            $.each($("ul[data-type='" + datatype + "']").find("li"), function (index, item) {
                $(this).removeClass("active");
                if (datatype=="0") {
                    deleteselectcomm("厂家选择", function () {
                    });
                }
            })
            $("ul[data-type='" + datatype + "']").find("li").eq(0).addClass("active");
            datas = "";
            InitData(0);
            ///删除条件同类条件
            //$(".search_condition_wrap").find("span[datatype='" + datatype + "']").remove();
        }
        else {
            $("ul[data-type='" + datatype + "']").find("li").removeClass("active");
            $(this).removeClass("active").addClass("active");
            //首次添加
            $(".search_condition_wrap").find("span[datatype='" + datatype + "']").remove();
            //str = '<span class="search-select-item" title="' + dataval + '"  datatype="' + datatype + '" bigTitle="' + dataname + '">' + dataname + ':' + dataval + '<span class="fa fa-close" title="删除"></span></span>'
            //$(".search_condition_wrap").append(str);
            ///删除已经选择的
      
            var strwhere = "";
            $.each($("ul[data-type='1']").find("li"), function (index, item) {
                if ($(this).hasClass("active") && $(this).attr("data-value") != "0") {
                    strwhere += '&SmallClass=' + $(this).attr("data-value");
                }
            })
            $.each($("ul[data-type='0']").find("li"), function (index, item) {
                if ($(this).hasClass("active") && $(this).attr("data-value") != "0") {
                    var tvalue = $(this).attr("data-value");
                    deleteselectcomm("厂家选择", function () {
                        var titem = { "Name": tvalue, "ParnetName": "厂家选择", "TypeName":"厂家选择" }
                        vm.selectitemlist.push(titem);
                    });
                    strwhere += '&Manufacturer=' + tvalue;
                }
            })
            datas = strwhere;
            strwhere = "";
            InitData(0);

        }





    })




    $().ready(function () {
        InitData(0);
        vm.loaddata();

    });
function InitData(pageindx) {
    page_indx = pageindx;
    //$("#pagespan").html("第" + pageindx + "页");
        var tbodyui = "";
      //  alert(datas);
        $.ajax({
            type: "POST",
            dataType: "JSON",
            url: '/Price/GetPriceList',
            data: "pageindex=" + (pageindx + 1) + "&pagesize=" + pagesize + datas,
            async: false,
            success: function (json) {
                // debugger;
                var productData = json.data;
                rowcount = json.totalCount;
                if (rowcount != 0) {
                    $.each(JSON.parse(productData), function (i, n) {
                        strcount++;
                        tbodyui += "<div class='listitem'>"
                        tbodyui += "<div smallClass='" + n.SmallClass + "' manufacturer = '" + n.ManuFacturer + "'  ProductGuid = '" + n.PriceProductGuid + "' model ='" + n.Model+ "' class='itembox "
                        if (strcount == 1) {
                            SmallClass = n.SmallClass;
                            Manufacturer = n.ManuFacturer;
                            Model = n.Model;
                            ProductGuid = n.PriceProductGuid;
                            priceData = "&priceDate=7";
                            GetPriceList();
                            onLoadEchars();
                            tbodyui += " active'>"
                        }
                        else {
                            tbodyui += "'>"
                        }                        
                        tbodyui +="<div class='itemtitle'>" + n.SmallClass + "|" + n.ManuFacturer + "</div>" 
                        tbodyui +="<div class='itemclass'>" + n.Model + "</div>" 
                        tbodyui += "<div class='itemzd'>" 
                        tbodyui +="<span class='fa " 
                            if (parseInt(n.Diff) > 0)
                            tbodyui += " fa-arrow-up text-red"
                        else
                                tbodyui += " fa-arrow-down text-blue"
                        tbodyui += "' style='margin - left: 8px;text-indent:5px;'> " + n.Diff+"</span>" 
                        tbodyui += "<span> " + n.PriDatestr + "</span >"
                        //if (parseInt(n.Diff) > 0)
                        //    tbodyui += "class='text-red' > " + n.Price + "</span >"
                        //else
                        //    tbodyui += "class='text-blue' > " + n.Price + "</span >"
                        tbodyui +="</div>"
                        tbodyui +="</div>"
                        tbodyui += "</div>";

                        //tbodyui += '<li';
                        //if (strcount == 1) {
                        //    tbodyui += ' class="active"';
                        //    SmallClass = n.SmallClass;
                        //    Manufacturer = n.ManuFacturer;
                        //    Model = n.Model;
                        //    ProductGuid = n.PriceProductGuid;
                        //    priceData="&priceDate=7"
                        //    GetPriceList();
                        //    onLoadEchars();
                        //}
                        //tbodyui += ' style="cursor: pointer" smallClass="' + n.SmallClass + '" manufacturer="' + n.ManuFacturer + '"  ProductGuid="' + n.PriceProductGuid + '" model="' + n.Model + '">';
                        //tbodyui += '<div class="left">';
                        //tbodyui += '<p>' + n.SmallClass + ' | ' + n.ManuFacturer + '</p>';
                        //tbodyui += '<p>' + n.Model + '</p>';
                        //tbodyui += '</div>';
                        //tbodyui += ' <div class="right">';
                        //tbodyui += ' <p>' + n.Price + '元/吨</p>';//  
                        //tbodyui += '<p><span class="fa';
                        //if (parseInt(n.Diff) > 0) 
                        //    tbodyui += ' fa-arrow-up text-red';
                        //else
                        //    tbodyui += ' fa-arrow-down text-green';
                        //tbodyui += '" style = "margin-left: 8px;" ></span >';
                        //tbodyui += '<span class="text-red">' + n.Diff + '</span>';
                        //tbodyui += '</p> </div > </li >';

                    });
                }
                $("#PriceListHTMLs").html(tbodyui);

            },
            error: function () { layer.msg('Load the data failure!', { icon: 5 }); }
        });

        $("#Pagination").pagination(rowcount, {
            callback: pageselectCallback,
            prev_text: '上一页',
            next_text: '下一页',
            items_per_page: pagesize,
            num_display_entries: 0,
            current_page: pageindx,
            num_edge_entries: 1
        });
        //上一页
    //$("#uppage").click(function () {
    //    if (page_indx >= 1) {
    //        var thispageindex = page_indx - 1;
    //        pageselectCallback(thispageindex);
    //    }            
    //    });
    //    //下一页
    //$("#dowpage").click(function () {
    //    var thispageindex = page_indx + 1;
    //    pageselectCallback(thispageindex);
    //    });
        NoneLine();

        //点击选中
    $("#PriceListHTMLs").find(".itembox").click(function () {
            $.each($("#PriceListHTMLs").find(".itembox"), function (index, item) {
                $(this).removeClass("active");
            })
            $(this).addClass("active");
            SmallClass = $(this).attr("smallclass");
            Manufacturer = $(this).attr("manufacturer");
            Model = $(this).attr("model");
            ProductGuid = $(this).attr("ProductGuid");
            GetPriceList();
            onLoadEchars();

        })
    }

    function pageselectCallback(page_id, jq) {

        InitData(page_id);
    }

    //搜索
    $("#shartPrice").click(function () {
        //var strwhere = "";
        //var strmodel = "";
        //if ($(".search_condition_wrap").find("span[datatype='4']").length > 0) {
        //    strwhere = '&Manufacturer=' + $(".search_condition_wrap").find("span[datatype='4']").attr("title");
        //}

        //if ($(".search_condition_wrap").find("span[datatype='1']").length > 0) {
        //    strmodel = '&SmallClass=' + $(".search_condition_wrap").find("span[datatype='1']").attr("title");
        //}
        //datas = strwhere + strmodel;
        if ($.trim($("#ProdcutModle").val()) == '') {
            layer.msg('请输入产品型号!', { icon: 5 });
            return false;
        }
        datas = "&Model=" + $.trim($("#ProdcutModle").val());
        InitData(0);
    })

//时间范围搜索beginDate endDate
$("#PriceDateSharet").click(function () {
    if ($.trim($("#beginDate").val()) == "") {
        alert("请输入开始时间");
        $("#beginDate").focus();
        return false;
    }
    if ($.trim($("#endDate").val()) == "") {
        alert("请输入结束时间");
        $("#endDate").focus();
        return false;
    }
    priceData = "&bdate=" + $.trim($("#beginDate").val()) + "&ndate=" + $.trim($("#endDate").val()) 
    GetPriceList();
    onLoadEchars();
})



$("#QuotationBtn").click(function () {
    var textval = $("#ProductGuid").val();
    $.ajax({
        type: "POST",
        dataType: "JSON",
        url: '/Price/TrueQuotation',
        data: "textval=" + textval,
        async: false,
        success: function (json) {
            if (json != null && json != "") {
                if (json.isadd) {
                    layer.msg('订阅成功', { icon: 1 });
                }
                else {
                    layer.msg(json.errMsg, { icon: 2 });
                }
            }
        },
        error: function () { layer.msg('数据请求异常', { icon: 2 }); }
    });
})


    function GetPriceList() {
        //debugger;
        //SmallClass='ABS' and Manufacturer='台湾台化' and Model='AG15A1'  
        dataArry.length = 0;
        priceArry.length = 0;
        $(".Txt_SmallClass").html(SmallClass);
        $(".Txt_Manufacturer").html(Manufacturer);
        $(".Txt_Model").html(Model);
        $("#ProductGuid").val(ProductGuid);
        $.ajax({
            type: "POST",
            dataType: "JSON",
            url: '/Price/GetPriceDateList',
            data: "SmallClass=" + SmallClass + "&Manufacturer=" + Manufacturer + "&Model=" + Model + priceData,
            async: false,
            success: function (result) {
                var product_Data = result.data;
                row_count = result.totalCount;
                if (row_count != 0) {
                    $.each(JSON.parse(product_Data), function (a, b) {
                        dataArry.push(getdatefroment(b.PriDate)); //PriDate formatterTime(b.PriDate, "YYYY-MM-DD")
                        priceArry.push(parseInt(b.Price));
                    });
                }
                //dataArry = dataArry.substring(0, dataArry.length - 1);
                //priceArry = priceArry.substring(0, priceArry.length - 1);
            },
            error: function () { layer.msg('Load the data failure!', { icon: 5 }); }
        });
    }


    ///点击近几天的时间
    $(".day").find("span").click(function () {
        var day = $(this).attr("number");
        $.each($(".day").find("span"), function (index, item) {
            $(this).removeClass("active");
        })
        $(this).addClass("active");
        priceData = "&priceDate=" + day
        GetPriceList();
        onLoadEchars();
    })


    //加载时间beginDate endDate
    jeDate({
        dateCell: "#beginDate",
        format: "YYYY-MM-DD",//YYYY年MM月DD日 hh:mm:ss
        isinitVal: false,
        isTime: true, //isClear:false,
        minDate: "2015-01-01"
    })
    jeDate({
        dateCell: "#endDate",
        format: "YYYY-MM-DD",//YYYY年MM月DD日 hh:mm:ss
        isinitVal: false,
        isTime: true, //isClear:false,
        minDate: "2015-01-01"
    })
	
	
	//echars
	    function onLoadEchars() {
        //'2019-03-16', '2019-03-17', '2019-03-18', '2019-03-19'
            var options = {
            backgroundColor: '#fffdff',
            title: {
                text: ''
            },
            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'cross',
                    label: {
                        backgroundColor: '#6a7985'
                    }
                }
            },
            legend: { data: [] },
            toolbox: {
                feature: {
                    saveAsImage: {}
                }
            },
            grid: {
                left: '3%',
                right: '3%',
                bottom: '4%',
                top: '7%',
                containLabel: true
            },
            xAxis: [
                {
                    type: 'category',
                    boundaryGap: false,
                    data: dataArry//'2019-03-16', '2019-03-17', '2019-03-18', '2019-03-19', '2019-03-20', '2019-03-21', '2019-03-22'
                    ,
                    //网格样式
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: ['#e4e3e6'],
                            width: 1,
                            type: 'solid'
                        }
                    }
                }
            ],
            yAxis: [
                {
                    type: 'value'
                    ,
                    min: function (value) {
                        return value.min;
                    },
                    //网格样式
                    splitLine: {
                        show: true,
                        
                        lineStyle: {
                            color: ['#e4e3e6'],
                            width: 1,
                            type: 'solid'
                        }
                    }
                }
            ],
            series: [
                {
                    name: '价格',
                    type: 'line',
                    areaStyle: {},
                    data: priceArry//
                    ,
                    itemStyle: {
                        normal: {
                            color: '#ddeff8',
                            lineStyle: {
                                color: '#3081c1'
                            }
                        }
                    }
                }
            ]
        };
        function rpt1(id, option, h) {
            var o = document.getElementById(id);
            var myChart = echarts.init(o);
            myChart.setOption(option);
        }
        //alert(dataArry)
        //alert(priceArry)
        rpt1('chart1', options);
    }
    //var dataArry = new Array()
    //dataArry.push("2019-03-16");
    //dataArry.push("2019-03-17");
 
	
	
//展开大类
function openitembox() {
    if (vm.showtitle == "点击查看更多查询条件") {
        vm.showtitle = "收起";
        for (var i = 0; i < vm.parmlist.length; i++) {
            var tempclass = vm.parmlist[i].classstr;
            if (tempclass == "itemboxclass") {
                vm.parmlist[i].classstr = "itemboxclassshow";
            }
        }
    }
    else {
        vm.showtitle = "点击查看更多查询条件";
        for (var i = 0; i < vm.parmlist.length; i++) {
            var tempclass = vm.parmlist[i].classstr;
            if (tempclass == "itemboxclassshow") {
                vm.parmlist[i].classstr = "itemboxclass";
            }
        }
    }   
}
//展开小类
function childmore(t)
{
    var index = t.id;
    var tempname = vm.parmlist[index].tname;
    var tlist = vm.parmlist[index].childlist;
    if (tempname =="更多 ∨") {
        vm.parmlist[index].tname = "收起 ∧";
        for (var i = 0; i < tlist.length; i++) {
            if (tlist[i].classstr =="none") {
                vm.parmlist[index].childlist[i].classstr = "childshowclass";
            }
        }
    }
    else {
        vm.parmlist[index].tname = "更多 ∨";
        for (var i = 0; i < tlist.length; i++) {
            if (tlist[i].classstr == "childshowclass") {
                vm.parmlist[index].childlist[i].classstr = "none";
            }
        }
    }
}
//选择类别属性
function selectitem(t)
{
    var indexstr = t.id;
    var indexsplit = indexstr.split(',');
    var parentindex = indexsplit[0];
    var thisindex = indexsplit[1];
    var tlist = vm.parmlist[parentindex].childlist;
    var selectname = tlist[thisindex].Name;
    var parnemtname = vm.parmlist[parentindex].Name;
    deleteselectcomm("分类", function () {
        var titem = { "Name": selectname, "ParnetName": parnemtname,"TypeName":"分类" }
        vm.selectitemlist.push(titem);
    });
    //刷新数据
    getparmstr(function (parmstr) {
        datas = parmstr;
        InitData(0);
    });
}
//删除选中的搜索条件
function deleteselectitem(ts)
{
    var index = ts.id;
    var name = vm.selectitemlist[index].TypeName;
    deleteselectcomm(name, function () {
        //刷新数据
        getparmstr(function (parmstr) {
            datas = parmstr;
            InitData(0);
        });
    });
}
function deleteselectcomm(name,callback)
{
    for (var k = 0; k < vm.selectitemlist.length; k++) {
        if (vm.selectitemlist[k].TypeName == name) {
            vm.selectitemlist.splice(k, 1);
            k--;
        }
    }
    callback();
}
//获取最终执行参数值
function getparmstr(callback)
{
    var tresultstr = "";
    for (var i = 0; i < vm.selectitemlist.length; i++) {
        var typename = vm.selectitemlist[i].TypeName;
        var name = vm.selectitemlist[i].Name;
        if (typename=="分类") {
            tresultstr += '&SmallClass=' + name;
        }
        else {
            tresultstr += '&Manufacturer=' + name;
        }        
    }
    callback(tresultstr);
}