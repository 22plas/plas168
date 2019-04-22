var keywork = getQueryString("key");
$(function () {
    if (keywork != "") {
        $(".search_condition_wrap").append('<span class="search-select-item" title="搜索" datatype="66" bigtitle="搜索">搜索:' + keywork + '</span>');
    }
})

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
var pagesize = 20;
var rowcount = 0;
var datas = "";
var page_indx = 0;
var count = 0;
$().ready(function () {
    InitData(0);
});

function InitData(pageindx) {
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
            //debugger;
            var productData = json.data;
            var strvar = "";
            rowcount = json.totalCount;
            $("#records").html(rowcount);
            if (rowcount != 0 && productData != "") {
                $.each(JSON.parse(productData), function (i, n) {
                    tbodyui += "<tr>";
                    tbodyui += "<td><a href=\"/PhysicalProducts/Detail/" + n.prodid + ".html\" target=\"_blank\">" + n.ProModel + "</a></td>";
                    tbodyui += "<td>" + n.PlaceOrigin + "</td>";
                    tbodyui += "<td>" + n.Name + "</td>";
                    tbodyui += "<td>" + n.ProUse + "</td>";
                    tbodyui += "<td>" + n.characteristic + "</td>";
                    tbodyui += "</tr>";
                });
                if (json.BigType != "") {
                    $.each(eval(json.BigType), function (index, item) {
                        count++;
                        typelist += '<div class="search-item">';
                        typelist += '<div class="search-item-type">' + item.attribute + '：</div>';
                        typelist += '<div class="search-item-content">';
                        typelist += '<ul class="search-item-content" data-type="' + count + '" data-typename="' + item.attribute + '">';
                        typelist += '<li data-guid="" data-value="0" class="active">全部</li>';
                        var sammlCount = 0;
                        if (json.SamllType != "") {
                            $.each(eval(json.SamllType), function (a, b) {
                                if (item.attribute == b.attribute) {
                                    sammlCount++;
                                    typelist += '<li ';
                                    if (sammlCount > 8) {
                                        typelist += ' style="display:none" name="SamllType_' + count + '"';
                                    }
                                    typelist += ' data-value="' + b.attributevalue + '"  data-guid="" >' + b.attributevalue + '</li>';
                                }
                            })
                        }
                        typelist += '</ul> </div>';
                        if (sammlCount > 8) {
                            //∧
                            typelist += '<div class="search-item-btn" data-type="' + count + '" data-typename="' + item.attribute + '">更多∨</div>';
                        }
                        typelist += '</div>';
                    })
                }


            }
            else {
                tbodyui += "<tr>";
                tbodyui += "<td colspan=\"5\" align=\"center\" class='red'>未找到数据</td>";
                tbodyui += "</tr>";
            }
            if (typelist != "")//未塞选条件分页
            {
                $("#selectTypeList").html(typelist);
            }
            $("#tempHtml_list").html(tbodyui);

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


    ///选中值，只能单选
    $(".search-item-content").find("li").click(function () {
        var dataname = $(this).parent().attr("data-typename");//名称
        var datatype = $(this).parent().attr("data-type");//类型
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
            $("ul[data-type='" + datatype + "']").find("li").removeClass("active");
            $(this).removeClass("active").addClass("active");
            //首次添加
            $(".search_condition_wrap").find("span[datatype='" + datatype + "']").remove();
            str = '<span class="search-select-item" title="' + dataval + '"  datatype="' + datatype + '" bigTitle="' + dataname + '">' + dataname + ':' + dataval + '<span class="fa fa-close" title="删除"></span></span>'
            $(".search_condition_wrap").append(str);
            ///删除已经选择的
            $(".fa-close").click(function () {
                //alert($(this).parent().html());
                $(this).parent().remove();
            })


        }
    })

    $(".fa-close").click(function () {
        //alert($(this).parent().html());
        $(this).parent().remove();
    })
    ///重置结束

    ///点击更多显示
    $(".search-item-btn").click(function () {
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

//查询
$("#Query_btnData").click(function () {
    //如果无查询条件必须输入查询条件
    var key = $.trim($("#Txtkeyword").val());
    if (key != '') {
        //重新搜索，条件必须清零
        keywork = key;
        strGuid = guid();
        for (var i = 0; i < 10; i++) {
            $(".search_condition_wrap").find("span[datatype='" + i + "']").remove();
        }
        $(".search_condition_wrap").find("span[datatype='66']").remove();
        $(".search_condition_wrap").append('<span class="search-select-item" title="搜索" datatype="66" bigtitle="搜索">搜索:' + keywork + '</span>');
    }
    else if ($("span[datatype='66']").length == 0) {
        if (key == '' || $.trim(key) == '') {
            layer.alert("请输入关键字进行搜索", { closeBtn: 0 }, function (index) {
                $("#Txtkeyword").focus();
                layer.close(index);
            });
            return false;
        }
        keywork = key;
        strGuid = guid();
        $(".search_condition_wrap").find("span[datatype='66']").remove();
        $(".search_condition_wrap").append('<span class="search-select-item" title="搜索" datatype="66" bigtitle="搜索">搜索:' + keywork + '</span>');
    }

    var Characteristic = "";//特性
    var Use = "";//用途
    var Kind = "";//种类
    var Method = "";//方法
    var Factory = "";//厂家
    var Additive = "";//添加剂
    var AddingMaterial = "";//增料

    if ($("span[bigtitle='产品特性']").length > 0) {
        Characteristic = $("span[bigtitle='产品特性']").attr("title");
    }
    if ($("span[bigtitle='产品用途']").length > 0) {
        Use = $("span[bigtitle='产品用途']").attr("title");
    }
    if ($("span[bigtitle='产品种类']").length > 0) {
        Kind = $("span[bigtitle='产品种类']").attr("title");
    }
    if ($("span[bigtitle='加工方法']").length > 0) {
        Method = $("span[bigtitle='加工方法']").attr("title");
    }
    if ($("span[bigtitle='生产厂家']").length > 0) {
        Factory = $("span[bigtitle='生产厂家']").attr("title");
    }
    if ($("span[bigtitle='添加剂']").length > 0) {
        Additive = $("span[bigtitle='添加剂']").attr("title");
    }
    if ($("span[bigtitle='填料/增强']").length > 0) {
        AddingMaterial = $("span[bigtitle='填料/增强']").attr("title");
    }
    datas = "&Characteristic=" + Characteristic + "&Use=" + Use + "&Kind=" + Kind + "&Method=" + Method + "&Factory=" + Factory + "&Additive=" + Additive + "&AddingMaterial=" + AddingMaterial;

    InitData(0);
})
