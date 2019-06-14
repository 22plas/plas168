var keywork = decodeURI(getQueryString("key"));
$(function () {
    if (keywork != "") {
        $(".search_condition_wrap").append('<span class="search-select-item" title="搜索" datatype="66" bigtitle="搜索">搜索:' + keywork + '</span>');
    }
})
function LookLoadingIcon(obj) {
    //alert("调用到了e");
    window.location.href = "/Replace/ProductReplace?PGuid=" + obj;
}
//展开更多
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



var strGuid = guid();
var istow = "0";
var pagesize = 20;
var rowcount = 0;
var datas = "";
var page_indx = 0;
var count = 0;
$().ready(function () {
    InitData(0);
});

function InitData(pageindx) {
    //   debugger;
    // alert(datas);
    page_indx = pageindx;
    var tbodyui = "";
    var typelist = "";
    // alert(keywork)
    $.ajax({
        type: "POST",
        dataType: "JSON",
        url: '/Product/MsgSearch',
        data: "pageindex=" + (pageindx + 1) + "&pagesize=" + pagesize + "&key=" + keywork + "&strGuid=" + strGuid + "" + datas,
        async: false,
        success: function (json) {
            //   debugger;
            try {
                var productData = json.data;
                var bigData = json.BigType;
                var smaillData = json.SamllType;
                var strvar = "";
                rowcount = json.totalCount;
                $("#records").html(rowcount);
                if (rowcount != 0 && productData != "") {
                    $.each(productData, function (i, n) {
                        tbodyui += "<tr>";
                        tbodyui += "<td><a href=\"/PhysicalProducts/Detail/" + n.prodid + ".html\" target=\"_blank\">" + n.ProModel + "</a></td>";
                        tbodyui += "<td>" + n.PlaceOrigin + "</td>";
                        tbodyui += "<td>" + n.Name + "</td>";
                        tbodyui += "<td>" + n.ProUse + "</td>";
                        tbodyui += "<td>" + n.characteristic + "</td>";
                        tbodyui += "<td><span class='layui-btn layui-btn-sm' onclick=\"LookLoadingIcon('" + n.prodid + "');\"><i class='Hui-iconfont'>&#xe6bd;</i> 寻找相似</span>";
                        if (n.isColl == '0') {
                            tbodyui += "<span class='layui-btn layui-btn-sm'  style='background-color:#e1e1e1'><i class='Hui-iconfont'>&#xe61f;</i> 已参与对比</span>";
                        }
                        else {
                            tbodyui += "<span class='layui-btn layui-btn-sm'><i class='Hui-iconfont'>&#xe61f;</i> 添加对比</span>";
                        }
                        
                        tbodyui += " </td>";//<span class='layui-btn layui-btn-sm'><i class='Hui-iconfont'>&#xe61f;</i> 添加对比</span>
                        tbodyui += "</tr>";
                    });

                    if (bigData != "" && istow == "0") {
                        $.each(bigData, function (index, item) {
                            count++;
                            typelist += '<div class="search-item">';
                            typelist += '<div class="search-item-type">' + item.attribute + '：</div>';
                            typelist += '<div class="search-item-content3">';
                            typelist += '<ul class="search-item-content" data-type="' + count + '" data-typename="' + item.attribute + '">';
                            typelist += "<li data-guid=\"\" data-value=\"0\" id=\"" + count + "_SamllType_0\" onClick=\"onselectobj('" + count + "', '0', '" + count + "_SamllType_0','" + item.attribute + "')\" class=\"active\">全部</li>";
                            var sammlCount = 0;
                            if (smaillData != "") {
                                //    debugger;
                                //alert(eval("(" + json.SamllType + ")"));
                                $.each(smaillData, function (a, b) {
                                    if (item.attribute == b.attribute) {
                                        sammlCount++;
                                        typelist += '<li ';
                                        if (sammlCount > 8) {
                                            typelist += ' style="display:none" name="SamllType_' + count + '"';
                                        }
                                        typelist += " data-value=" + b.attributevalue + " id=\"" + count + "_SamllType_" + sammlCount + "\" onClick=\"onselectobj('" + count + "', '" + b.attributevalue + "','" + count + "_SamllType_" + sammlCount + "','" + item.attribute + "');\"  data-guid=\"\" >" + b.attributevalue + "</li>";
                                    }
                                })
                            }
                            typelist += '</ul> </div>';
                            if (sammlCount > 8) {
                                //∧
                                typelist += '<div class="search-item-btn" name="search-item-btn" data-type="' + count + '" data-typename="' + item.attribute + '">更多∨</div>';
                            }
                            typelist += '</div>';
                        })


                   

                    }


                }
                else {
                    tbodyui += "<tr>";
                    tbodyui += "<td colspan=\"6\" align=\"center\" class='red'>未找到数据</td>";
                    tbodyui += "</tr>";
                }
                if (typelist != "")//未塞选条件分页
                {
                    $("#selectTypeList").html(typelist);
                }
                $("#tempHtml_list").html(tbodyui);
            } catch (e) {
                alert(e.msg);
                // layer.msg(e.msg, { icon: 5 });
            }

        },
        error: function () { layer.msg('Load the data failure!', { icon: 5 }); }
    });
    //暂时不需要分页
    $("#Pagination").pagination(rowcount, {
        callback: pageselectCallback,
        prev_text: '上一页',
        next_text: '下一页',
        items_per_page: pagesize,
        num_display_entries: 6,
        current_page: pageindx,
        num_edge_entries: 1
    });
    $("html, body").stop().animate({ scrollTop: $("#records").offset().top - 200 }, 400);




    ///重置结束
    ///点击更多显示
    $("div[name='search-item-btn']").click(function () {
        var ty = $(this).attr("data-type");
        if ($("li[name='SamllType_" + ty + "']").css("display") == "none") {
            $("li[name='SamllType_" + ty + "']").show();
            $(this).html("隐藏∧");
        }
        else {
            $("li[name='SamllType_" + ty + "']").hide();
            $(this).html("更多∨");
        }
    });
 

}

function pageselectCallback(page_id, jq) {
    InitData(page_id);
}


///选中值，只能单选
function onselectobj(datatype, dataval, obj, dataname) {
    istow = "1";
    //alert(objtype + "+" + objval);
    if (dataval == "0") {
        $.each($("ul[data-type='" + datatype + "']").find("li"), function (index, item) {
            $(this).removeClass("active");
        })
        $("ul[data-type='" + datatype + "']").find("li").eq(0).addClass("active");
        ///删除条件同类条件
        $(".search_condition_wrap").find("span[datatype='" + datatype + "']").remove();
    }
    else {
        $("ul[data-type='" + datatype + "']").find("li").removeClass("active");
        // alert(obj);
        $("#" + obj).removeClass("active").addClass("active");
        //首次添加
        $(".search_condition_wrap").find("span[datatype='" + datatype + "']").remove();
        str = '<span class="search-select-item" title="' + dataval + '"  datatype="' + datatype + '" bigTitle="' + dataname + '">' + dataname + ':' + dataval + "<span class=\"fa fa-close\" onClick=\"romve(this,'" + obj + "');\" title=\"删除\"></span></span>"
        $(".search_condition_wrap").append(str);

    }
    ///点击更多显示
    $("div[name='search-item-btn']").click(function () {
        var ty = $(this).attr("data-type");
        if ($("li[name='SamllType_" + ty + "']").css("display") == "none") {
            $("li[name='SamllType_" + ty + "']").show();
            $(this).html("隐藏∧");
        }
        else {
            $("li[name='SamllType_" + ty + "']").hide();
            $(this).html("更多∨");
        }
    });
    sharet();
}


function sharet() {
    //重新检索
    var Characteristic = "";//特性
    var Use = "";//用途
    var Kind = "";//种类
    var Method = "";//方法
    var Factory = "";//厂家
    var Additive = "";//添加剂
    var AddingMaterial = "";//增料

    var strdatas = "";

    //产品特性
    if ($("span[bigtitle='产品特性']").length > 0) {
        Characteristic = $("span[bigtitle='产品特性']").attr("title");
        strdatas += "&Characteristic=" + Characteristic;
    }
    //产品用途
    if ($("span[bigtitle='产品用途']").length > 0) {
        Use = $("span[bigtitle='产品用途']").attr("title");
        strdatas += "&Use=" + Use;
    }
    //产品种类
    if ($("span[bigtitle='产品种类']").length > 0) {
        Kind = $("span[bigtitle='产品种类']").attr("title");
        strdatas += "&Kind=" + Kind;
    }
    //加工方法
    if ($("span[bigtitle='加工方法']").length > 0) {
        Method = $("span[bigtitle='加工方法']").attr("title");
        strdatas += "&Method=" + Method;
    }
    //生产厂家
    if ($("span[bigtitle='生产厂家']").length > 0) {
        Factory = $("span[bigtitle='生产厂家']").attr("title");
        strdatas += "&Factory=" + Factory;
    }
    //添加剂
    if ($("span[bigtitle='添加剂']").length > 0) {
        Additive = $("span[bigtitle='添加剂']").attr("title");
        strdatas += "&Additive=" + Additive;
    }
    //填料/增强
    if ($("span[bigtitle='填料/增强']").length > 0) {
        AddingMaterial = $("span[bigtitle='填料/增强']").attr("title");
        strdatas += "&AddingMaterial=" + AddingMaterial;
    }

    datas = strdatas;

    InitData(0);
}

function romve(self, obj) {
    $(self).parent().remove();
    if (obj.length > 0) {
        $("#" + obj).removeClass("active");
        var splitval = obj.substring(0, obj.length - 1) + "0";
        $("#" + splitval).addClass("active");
    }

    sharet();
}