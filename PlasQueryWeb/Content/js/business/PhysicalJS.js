$(function () {
    $(".navBox").find(".nav").find("li").eq(2).children("a").addClass("active");
    //需要验证是否登录

    $.each($("table[isread='1']"), function (index, item) {
        $.each($("table[isread='1']").find("select[name='UnitFaceKey']"), function (a, b) {
            //alert($(this).attr("smalltypename"));
            onLoadUnit(this, $(this).attr("bigtypename"), $(this).attr("smalltypename"));
        })
    })
})

//获取列表
function onLoadUnit(obj, bigname, samllname) {
    if (bigname != "" && samllname != "" && obj != "") {
        $.ajax({
            type: "POST",
            dataType: "JSON",
            url: '/PhysicalProducts/GetUnitList',
            data: "bigname=" + bigname + "&samllname=" + samllname ,
            async: false,
            success: function (json) {
                if (json != null) {
                    var str = "";
                    $.each(json.list, function (k, j) {
                        str += "<option value=" + j.unit + ">" + j.unit + "</option>";
                    })
                    $(obj).empty().append(str);
                }
            },
            error: function () { /*alert('加载数据失败');*/  }
        });
    }
}

function LookLoadingIcon(obj) {
    //alert("调用到了e");
    window.location.href = "/Replace/ProductReplace?PGuid=" + obj;
}

$(".search-more-btn").click(function () {
    if ($("div[name='AtrrNone']").css("display") == "none") {

        $("div[name='AtrrNone']").slideDown();
        $(".search-more-btn").find(".fa").removeClass("fa-angle-down").addClass("fa-angle-up");
    }
    else {
        $("div[name='AtrrNone']").slideUp();
        $(".search-more-btn").find(".fa").removeClass("fa-angle-up").addClass("fa-angle-down");
    }
})

///点击更多
$(".search-item-btn").click(function () {
    var dataval = $(this).attr("data-type");
    var datatile = $(this).attr("data-typename");
    layer.open({
        type: 2,
        title: '选择—' + datatile,
        shadeClose: true,
        shade: 0.8,
        maxmin: true,
        area: ['90%', '90%'],
        content: '/PhysicalProducts/More?rid=' + dataval + "&rname=" + datatile + "&more=2"
    });
})

///点击切换
$(".orange-border").find("li").click(function () {

    var idx = $(this).index();
    $.each($(".orange-border").find("li"), function (index, item) {
        $(this).removeClass("active");
    })
    $(this).addClass("active");
    var id = $(this).attr("id");
    $("table[name='AttrRightList']").hide();
    $("#Attr_" + id).removeClass("none");
    $("#Attr_" + id).show();
    if ($("#Attr_" + id).attr("isread") == "0") {
        $.each($("#Attr_" + id).find("select[name='UnitFaceKey']"), function (index, item) {
            onLoadUnit(this, $(this).attr("bigtypename"), $(this).attr("smalltypename"));
        })
        $("#Attr_" + id).attr("isread", "1");
    }
})
///选中值，可多选择
$(".search-item-content").find("li").click(function () {
    var dataname = $(this).parent().attr("data-typename");//名称
    var datatype = $(this).parent().attr("data-type");//类型
    var dataguid = $(this).attr("data-guid");//ID
    var dataval = $(this).attr("data-value");
    var str = "";
    var istrue = false;
    ///添加样式
    if (dataval == "0") {
        $.each($("ul[data-type='" + datatype + "']").find("li"), function (index, item) {
            $(this).removeClass("active");
        })
        $("ul[data-type='" + datatype + "']").find("li").eq(0).addClass("active");
        ///删除条件同类条件
        $(".search_condition_wrap").find("span[datatype='" + datatype + "']").remove();
    }
    else {
        $("ul[data-type='" + datatype + "']").find("li").eq(0).removeClass("active");
        if (datatype == 3) {//阻燃只允许单选
            $("ul[data-type='" + datatype + "']").find("li").removeClass("active");
        }
        $(this).removeClass("active").addClass("active");



        //首次添加
        if ($(".search_condition_wrap").find(".search-select-item").length == 0) {
            str = '<span class="search-select-item" dataguid="' + dataguid + '"  title="' + $(this).attr("data-value") + '"  datatype="' + datatype + '" bigTitle="' + dataname + '">' + dataname + ':' + $(this).attr("data-value") + '<span class="fa fa-close" title="删除"></span></span>'
        }
        else {//否则判断是否已经添加
            $.each($(".search_condition_wrap").find(".search-select-item"), function (index, item) {
                if (datatype == 3) {//3 阻燃等级特殊处理，只允许单选
                    if ($(this).attr("datatype") == "3") {
                        $(this).remove();
                    }
                }
                else if ($(this).attr("title") == dataval && datatype != 3) {
                    istrue = true;
                }

            })
            if (!istrue) {
                str = '<span class="search-select-item" dataguid="' + dataguid + '"  title="' + dataval + '"  datatype="' + datatype + '" bigTitle="' + dataname + '">' + dataname + ':' + dataval + '<span class="fa fa-close" title="删除"></span></span>'
            }
        }
        $(".search_condition_wrap").append(str);
        ///删除已经选择的
        $(".fa-close").click(function () {
            // alert("清理");
            //alert($(this).parent().html());
            var datatype = $(this).parent().attr("datatype");
            var dataval = $(this).parent().attr("title");
            //  alert(datatype + "+" + dataval);
            if ($("ul[data-type='" + datatype + "']").find("li[data-value='" + dataval + "']").length > 0) {
                $("ul[data-type='" + datatype + "']").find("li[data-value='" + dataval + "']").removeClass("active");
            }
            $(this).parent().remove();
        })


    }


})

//输入框值
$("input[name='MinValue'],input[name='MaxValue']").blur(function () {
    var names = $(this).attr("name");
    var str = "";
    var txtval = $(this).val().replace(/ /g, "");
    var idx = 0;
    if (names == "MinValue") {
        idx = $("input[name='MinValue']").index(this);
    }
    else {
        idx = $("input[name='MaxValue']").index(this);
    }

    if (txtval != "") {
        var minval = $("input[name='MinValue']").eq(idx).val();
        var maxval = $("input[name='MaxValue']").eq(idx).val();
        var unit = $("select[name='UnitFaceKey']").eq(idx).val();//$("input[name='UnitFaceKey']").eq(idx).val();
        var bigname = $("input[name='MinValue']").eq(idx).attr("bigtypename");
        var samlname = $("input[name='MinValue']").eq(idx).attr("smalltypename");
        // alert(samlname);
        if (maxval != "" && minval != "") {
            if ($(".search_condition_wrap").find(".search-select-item").length == 0) {
                str = '<span class="search-select-item" dataguid="" title="' + bigname + '" unit="' + unit + '" minval="' + minval + '" maxval="' + maxval + '" datatype="11" bigTitle="' + samlname + '">' + samlname + '：' + minval + "-" + maxval + '/' + unit + '<span class="fa fa-close" title="删除"></span></span>';
            }
            else {
                //  var istrue = false;
                if ($(".search_condition_wrap").find("span[bigtitle='" + samlname + "']").length > 0) {
                    // alert("存在")
                    $(".search_condition_wrap").find("span[bigtitle='" + samlname + "']").remove();
                }
                str = '<span class="search-select-item" dataguid="" title="' + bigname + '" unit="' + unit + '" minval="' + minval + '" maxval="' + maxval + '" datatype="11" bigTitle="' + samlname + '">' + samlname + '：' + minval + "-" + maxval + '/' + unit + '<span class="fa fa-close" title="删除"></span></span>';
            }

        }
    }

    $(".search_condition_wrap").append(str);

    $(".fa-close").click(function () {
        // alert("清理");
        //alert($(this).parent().html());
        var datatype = $(this).parent().attr("datatype");
        var dataval = $(this).parent().attr("title");
        //  alert(datatype + "+" + dataval);
        if ($("ul[data-type='" + datatype + "']").find("li[data-value='" + dataval + "']").length > 0) {
            $("ul[data-type='" + datatype + "']").find("li[data-value='" + dataval + "']").removeClass("active");
        }
        $(this).parent().remove();
    })

    //alert(txtval);
})



///查询数据
var pagesize = 20;
var rowcount = 0;
var datas = "";
var page_indx = 0;
var guidstr = guid();
function InitData(pageindx, isNavLink) {
    //debugger;
    page_indx = pageindx;
    var tbodyui = "";
    // alert(isNavLink);
    // alert(guidstr)
    $.ajax({
        type: "POST",
        dataType: "JSON",
        url: '/PhysicalProducts/SuperMsgSearch',
        data: "pageindex=" + (pageindx + 1) + "&pagesize=" + pagesize + "&guidstr=" + guidstr + "&isNavLink=" + isNavLink + datas,
        async: false,
        success: function (json) {
            //debugger;
            var state = json.state;
            if (state == "Success") {
                //alert(JSON.stringify(json));
                var productData = json.data;
                var strvar = "";
                rowcount = json.totalCount;
                $("#records").html(rowcount);
                if (rowcount != 0 && productData != "") {
                    $.each(JSON.parse(productData), function (i, n) {
                        tbodyui += "<tr title='" + n.ProUse + "'>";
                        tbodyui += "<td><a href=\"/PhysicalProducts/Detail/" + n.productid + ".html\" style='color:#535353' target=\"_blank\">" + n.ProModel + "</a></td>";
                        tbodyui += "<td>" + n.PlaceOrigin + "</td>";
                        tbodyui += "<td>" + n.Name + "</td>";
                        tbodyui += "<td>" + n.ProUse + "</td>";
                        tbodyui += "<td>" + n.characteristic + "</td>";
                        tbodyui += "<td><span class='layui-btn layui-btn-sm' onclick=\"LookLoadingIcon('" + n.productid + "');\"><i class='Hui-iconfont'>&#xe6bd;</i> 寻找相似s</span>";
                        tbodyui += "<span class='layui-btn layui-btn-sm' id=\"Contrast_" + n.productid + "\" onClick=\"onColl('" + n.productid + "');\"><i class='Hui-iconfont'>&#xe61f;</i> 添加对比</span>";
                        tbodyui += "</td>";// <span class='layui-btn layui-btn-sm'><i class='Hui-iconfont'>&#xe61f;</i> 添加对比</span>
                        tbodyui += "</tr>";
                    });
                }
                else {
                    tbodyui += "<tr>";
                    tbodyui += "<td colspan=\"6\" align=\"center\" class='red'>未找到数据</td>";
                    tbodyui += "</tr>";
                }
                $("#DataList").html(tbodyui);
            }
            else if (state == "NedLogin") {
                pageindx = 1;
                layer.msg("请先登录后再查看！", { icon: 5 });
            }
            else {
                layer.msg("系统异常！", { icon: 5 });
            }
        },
        error: function () { layer.msg('Load the data failure!', { icon: 5 }); }
    });

    $("#Pagination").pagination(rowcount, {
        callback: pageselectCallback,
        prev_text: '上一页',
        next_text: '下一页',
        items_per_page: pagesize,
        num_display_entries: 8,
        current_page: pageindx,
        num_edge_entries: 1
    });
    $("html, body").stop().animate({ scrollTop: $("#records").offset().top - 200 }, 400);

}

//function pageselectCallback(page_id, jq) {
//    InitData(page_id, '1');
//}

//查询
$("#QueryBtnSuper").click(function () {
    //组合查询
    //debugger;
    var titlestr = "";
    var str = "";
    if ($(".search_condition_wrap").find(".search-select-item").length > 0) {
        for (var i = 1; i < 10; i++) {
            if ($(".search_condition_wrap").find("span[datatype='" + i + "']").length > 0) {
                str = "{";//{产品特性=>''高光;阻燃'',0,0} {'产品用途=>''电视外壳''==>'''''}
                str += $(".search_condition_wrap").find("span[datatype='" + i + "']").attr("bigtitle");
                str += "=>''";
                var count = 0;
                $.each($(".search_condition_wrap").find("span[datatype='" + i + "']"), function (index, item) {
                    count++;
                    var isval = "";
                    if ($.trim($(this).attr("dataguid")) != "") {
                        isval = $.trim($(this).attr("dataguid"));
                    }
                    else {
                        isval = $.trim($(this).attr("title"));
                    }
                    str += isval;
                    if (count < $(".search_condition_wrap").find("span[datatype='" + i + "']").length) {
                        str += ";"
                    }
                })
                str += "''==>''''"
                str += "}"
                //alert(i+"_"+str);
            }
            titlestr += str;
            str = "";
        }
        // alert(titlestr);
    }
    //2019-2-24{'物理性能=)密度=>1.0,1.5==>''g/cm³'''}
    $.each($(".search_condition_wrap").find("span[datatype='11']"), function (index, item) {
        str += "{" + $(this).attr("title") + "=)" + $(this).attr("bigtitle") + "=>" + $(this).attr("minval") + "," + $(this).attr("maxval") + "==>''" + $(this).attr("unit") + "''}";
    })
    titlestr += str
    datas = "&searchstr=" + titlestr;
    //alert(datas);
    guidstr = guid();//重置UGID
    InitData(0, '');
    // alert(titlestr);
})

///添加对比
function onColl(ProductId) {
    var counts = 0;
    if ($("#ContrentNumber").length > 0) {
        counts = parseInt($("#ContrentNumber").html());
    }
    if (counts < 3) {
        $.ajax({
            type: "POST",
            dataType: "JSON",
            url: '/MemberCenter/AddContrast',
            data: "ProductId=" + ProductId,
            async: false,
            success: function (json) {
                if (json != null && json != '') {
                    if (json.isContonl == true) {
                        layer.msg('已添加对比！', { icon: 1 });
                        //<i class='Hui-iconfont'>&#xe61f;</i> 已参与对比
                        $("#Contrast_" + ProductId).removeAttr("onlick");
                        $("#Contrast_" + ProductId).html("<i class='Hui-iconfont'>&#xe61f;</i> 已参与对比");
                        $("#Contrast_" + ProductId).css("background-color", "#e1e1e1 !important");
                    }
                    else {
                        layer.msg(json.errmsg, { icon: 2 });
                    }
                }
            },
            error: function () { layer.msg('数据请求异常', { icon: 2 }); }
        });
    }
    else {
        layer.msg('只能对比三条数据！', { icon: 2 });
    }
  

}
///点击数字输入值
$("td[name='samllHit']").click(function () {
    var idx = $(this).attr("number");
    if ($("tr[name='samll_" + idx + "']").css("display") == "none") {
        $("tr[name='samll_" + idx + "']").show();
        $("tr[name='samll_" + idx + "']").removeClass("none");
        $(this).html("点击收起&nbsp;↑");
    }
    else {
        $("tr[name='samll_" + idx + "']").hide();
        $("tr[name='samll_" + idx + "']").addClass("none");
        $(this).html("点击更多&nbsp;↓");
    }
});

