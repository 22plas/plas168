
//生成GUID
function guid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}


//Modification
$(document).ajaxStart(function () {
    if ($.LoadingOverlay != undefined) {
        $.LoadingOverlay("show");
    }
});

$(document).ajaxStop(function () {

    if ($.LoadingOverlay != undefined) {
        $.LoadingOverlay("hide");
    }
});

//获取参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return "";
} 

//获取时间年月日
function getdatefroment(strobj) {
    var str = strobj;
    // 转换日期格式
    str = str.replace(/-/g, '/'); // "2010/08/01";
    // 创建日期对象
    var date = new Date(str);
    // 没有格式化的功能，只能一个一个取
    str = date.getFullYear() + '-'
        // 因为js里month从0开始，所以要加1
        + (parseInt(date.getMonth()) + 1) + '-'
        + date.getDate();

    return str;

}

$(function () {
    //单选
    $('input[name="radio-btn"]').wrap('<div class="radio-btn"><i></i></div>');
    $(".radio-btn").on('click', function () {
        var _this = $(this),
            block = _this.parent().parent();
        block.find('input:radio').attr('checked', false);
        block.find(".radio-btn").removeClass('checkedRadio');
        _this.addClass('checkedRadio');
        _this.find('input:radio').attr('checked', true);
    });
    $('input[name="check-box"]').wrap('<div class="check-box"><i></i></div>');
    //复选框
    $.fn.toggleCheckbox = function () {
        this.attr('checked', !this.attr('checked'));
    }
    $('.check-box').on('click', function () {
        $(this).find(':checkbox').toggleCheckbox();
        $(this).toggleClass('checkedBox');
    });
    $.each($("input[type='checkbox']"), function () {
        if ($(this).is(":checked")) {
            $(this).parent().parent().addClass("checkedBox");
        }
    })
})