
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

//修改时间格式
function formatterTime(value, formatStr) {

    //YYYY-MM-DD HH:mm:ss

    formatStr = formatStr || "YYYY-MM-DD HH:mm:ss";

    if (value) {
        var s = value;
        s = s.replace("/Date(", "").replace(")/", "");
        var date = new Date(parseInt(s));
        date = moment(date).format(formatStr);
        return date;
    }
    else {
        return value;
    }
}