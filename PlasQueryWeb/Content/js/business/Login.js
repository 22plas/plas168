var logintype = "pwdlogin";//登录类型 pwdlogin：密码登录 codelogin：验证码登录
//提交登录
$("#savelogin").click(function () {
    if (logintype == "pwdlogin") {
        pwdlogin();
    }
    else {
        codelogin();
    }
});
//验证码登录
function codelogin() {
    var yzreturn = verificationforcode();
    var refreshpagename = $(".showlogin").attr("id");
    var phone = $("#codeloginphone").val();
    if (yzreturn) {
        $.ajax({
            type: "post",
            url: comm.action("AccountLogin", "MemberCenter"),
            data: { account: phone },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    //var rs = data.Result;
                    $("#codeloginmessage").html("");
                    window.location.href = comm.action("Index", "Home");
                }
                else if (data.State == "Fail") {
                    //layer.alert("登录失败！");
                    $("#codeloginmessage").html("登录失败！");
                }
                else {
                    //layer.alert("系统异常！");
                    $("#codeloginmessage").html("系统异常！");
                }
            }
        });
    }
}

var _returncode = "";
var _phone = "";
var _vcodereturn = false;//是否发送验证码
//发送验证码
function setcode() {
    $("#codeloginmessage").html("");
    var phonestr = $("#codeloginphone").val();
    if (phonestr == "" || phonestr == null || phonestr == undefined) {
        //layer.alert("请输入手机号！");
        $("#codeloginmessage").html("请输入手机号！");
        return;
    }
    var myreg = /^[1][3,4,5,7,8,9][0-9]{9}$/;
    if (!myreg.test(phonestr)) {
        $("#codeloginmessage").html("请输入正确的手机号！");
        return;
    }
    var timecode = $("#code").html();
    if (timecode.trim() == "获取验证码" || timecode.trim() == "重新获取") {
        var a = 59;
        clearInterval(timer);
        $("#code").html(a);
        //获取验证码倒计时
        var timer = setInterval(function () {
            if (a > 1) {
                a--;
                $("#code").html(a);
            } else if (a == 1) {
                clearInterval(timer);
                $("#code").html("重新获取");
            }
        }, 1000);
        //发送验证码
        $.ajax({
            type: "get",
            url: comm.action("SetCode", "MemberCenter"),
            data: { phone: phonestr },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    var rs = data.Result;
                    _returncode = rs.code;
                    _phone = phonestr;
                    _vcodereturn = true;
                    $("#codeloginmessage").html("");
                }
                else if (data.State == "Fail") {
                    //layer.alert("发送失败！");
                    $("#codeloginmessage").html("发送失败！");
                }
                else {
                    //layer.alert("系统异常！");
                    $("#codeloginmessage").html("系统异常！");
                }
            }
        });
    }
}

//验证码登录数据验证
function verificationforcode() {
    var phone = $("#codeloginphone").val();
    var thiscode = $("#codelogincode").val();
    var returnstr = true;
    if (returnstr) {
        if (phone == "" || phone == null || phone == undefined || phone == "请输手机号") {
            //layer.alert("请输手机号！");
            $("#codeloginmessage").html("请输手机号！");
            returnstr = false;
        }
    }
    if (returnstr) {
        //验证输入的验证码是否匹配
        if (thiscode=="") {
            //layer.alert("验证码不正确！");
            $("#codeloginmessage").html("请输入验证码！");
            returnstr = false;
        }
    }
    if (returnstr) {
        //验证输入的验证码是否匹配
        if (thiscode != _returncode) {
            //layer.alert("验证码不正确！");
            $("#codeloginmessage").html("验证码不正确！");
            returnstr = false;
        }
    }
    return returnstr;
}

//密码登录方法
function pwdlogin() {
    var usaccount = $("#loginphone").val();
    var uspassword = $("#loginpwd").val();
    var refreshpagename = $(".showlogin").attr("id");
    var returnv = verification();
    if (returnv) {
        //保存用户注册信息
        $.ajax({
            type: "get",
            url: comm.action("GetLogin", "MemberCenter"),
            data: { account: usaccount, password: uspassword },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    //var rs = data.Result;
                    window.location.href = comm.action("Index", "Home");
                    $("#accountloginmessage").html("");
                }
                else if (data.State == "NoFind") {
                    //layer.alert("账号不存在！");
                    $("#accountloginmessage").html("账号不存在！");
                }
                else if (data.State == "Fail") {
                    //layer.alert("登录失败！");
                    $("#accountloginmessage").html("账号或密码错误！");
                }
                else {
                    //layer.alert("系统异常！");
                    $("#accountloginmessage").html("系统异常！");
                }
            }
        });
    }
}
//验证保存数据
function verification() {
    var usaccount = $("#loginphone").val();
    var uspassword = $("#loginpwd").val();
    var returnstr = true;
    if (returnstr) {
        if (usaccount == "" || usaccount == null || usaccount == undefined || usaccount == "请输手机号") {
            //layer.alert("请输手机号！");
            $("#accountloginmessage").html("请输手机号！");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (uspassword == "" || uspassword == null || uspassword == undefined || uspassword == "请输入密码") {
            //layer.alert("请输入登录密码！");
            $("#accountloginmessage").html("请输入登录密码！");
            returnstr = false;
        }
    }
    return returnstr;
}
//切换验证码登录
//$("#codelogin").click(function () {
//    $("#pwdlogin").css("display", "block");
//    $("#codelogin").css("display", "none");
//    $("#pwdlogindivbox").css("display", "none");
//    $("#codelogindivbox").css("display", "block");
//    logintype = "codelogin";
//});
////切换密码登录
//$("#pwdlogin").click(function () {
//    $("#pwdlogin").css("display", "none");
//    $("#codelogin").css("display", "block");
//    $("#pwdlogindivbox").css("display", "block");
//    $("#codelogindivbox").css("display", "none");
//    logintype = "pwdlogin";
//});




//重置密码
var _forgetpwdreturncode = "";
var _forgetpwdphone = "";
var _forgetpwdvcodereturn = false;//是否发送验证码
//忘记密码第一步跳转下一步
function tonextforget() {
    var r = forgetpwdverificationforcode();
    if (r) {
        $.ajax({
            type: "get",
            url: comm.action("GetUserByPhone", "MemberCenter"),
            data: { phone: $("#forgetphone").val() },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    $("#forgetcodemessage1").html("");
                    $("#forget1").hide();
                    $("#forget2").show();
                    $("#forget3").hide();
                    //vm.forgetcode = "";
                    //vm.forgetphone = "";
                    _forgetpwdreturncode = "";
                }
                else if (data.State == "NotFind") {
                    //layer.alert("发送失败！");
                    $("#forgetcodemessage1").html("账号不存在！");
                }
                else if (data.State == "Fail") {
                    //layer.alert("发送失败！");
                    $("#forgetcodemessage1").html("发送失败！");
                }
                else {
                    //layer.alert("系统异常！");
                    $("#forgetcodemessage1").html("系统异常！");
                }
            }
        });
    }
}
//修改密码数据验证
function forgetpwdverificationforcode() {
    var forgetphone = $("#forgetphone").val();
    var forgetthiscode = $("#forgetcode").val();
    var returnstr = true;
    if (returnstr) {
        if (forgetphone == "" || forgetphone == null || forgetphone == undefined || forgetphone == "请输手机号") {
            //layer.alert("请输手机号！");
            $("#forgetcodemessage1").html("请输手机号！");
            returnstr = false;
        }
    }
    if (returnstr) {
        //验证输入的验证码是否匹配
        if (forgetthiscode == "") {
            //layer.alert("验证码不正确！");
            $("#forgetcodemessage1").html("请输入验证码！");
            returnstr = false;
        }
    }
    if (returnstr) {
        //验证输入的验证码是否匹配
        if (forgetthiscode != _forgetpwdreturncode) {
            //layer.alert("验证码不正确！");
            $("#forgetcodemessage1").html("验证码不正确！");
            returnstr = false;
        }
    }
    return returnstr;
}
//修改密码发送验证码
function forgetpwdsetcode() {
    $("#forgetcodemessage1").html("");
    var phonestr = $("#forgetphone").val();
    if (phonestr == "" || phonestr == null || phonestr == undefined) {
        //layer.alert("请输入手机号！");
        $("#forgetcodemessage1").html("请输入手机号！");
        return;
    }
    var myreg = /^[1][3,4,5,7,8,9][0-9]{9}$/;
    if (!myreg.test(phonestr)) {
        $("#forgetcodemessage1").html("请输入正确的手机号！");
        return;
    }
    var tempforgetcodetimestr = $("#forgetcodetimestr").html();
    if (tempforgetcodetimestr.trim() == "获取验证码" || tempforgetcodetimestr.trim() == "重新获取") {
        var a = 59;
        clearInterval(timer);
        $("#forgetcodetimestr").html(a);
        //获取验证码倒计时
        var timer = setInterval(function () {
            if (a > 1) {
                a--;
                $("#forgetcodetimestr").html(a);
            } else if (a == 1) {
                clearInterval(timer);
                $("#forgetcodetimestr").html("重新获取");

            }
        }, 1000);
        //发送验证码
        $.ajax({
            type: "get",
            url: comm.action("SetCode", "MemberCenter"),
            data: { phone: phonestr },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    var rs = data.Result;
                    _forgetpwdreturncode = rs.code;
                    _forgetpwdphone = phonestr;
                    _forgetpwdvcodereturn = true;
                    $("#forgetcodemessage1").html("");
                }
                else if (data.State == "Fail") {
                    //layer.alert("发送失败！");
                    $("#forgetcodemessage1").html("发送失败！");
                }
                else {
                    //layer.alert("系统异常！");
                    $("#forgetcodemessage1").html("系统异常！");
                }
            }
        });
    }
}
//提交修改密码
function tosumbitforget()
{
    var rs = submitverification();
    if (rs) {
        $.ajax({
            type: "post",
            url: comm.action("UpdateUserPwd", "MemberCenter"),
            data: { phone: $("#forgetphone").val(), newpwd: $("#confirmnewpwd").val() },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    $("#forget1").hide();
                    $("#forget2").hide();
                    $("#forget3").show();
                    $("#forgetcodemessage2").html();
                }
                else if (data.State == "Fail") {
                    //layer.alert("发送失败！");
                    $("#forgetcodemessage2").html("发送失败！");
                }
                else {
                    //layer.alert("系统异常！");
                    $("#forgetcodemessage2").html("系统异常！");
                }
            }
        });
    }
}
//验证修改密码
function submitverification()
{
    var tempconfirmnewpwd = $("#confirmnewpwd").val();
    var tempnewpwd = $("#newpwd").val();
    var returnstr = true;
    if (returnstr) {
        if (tempnewpwd == "" || tempnewpwd == null || tempnewpwd == undefined || tempnewpwd == "设置新密码") {
            //layer.alert("请输手机号！");
            $("#forgetcodemessage2").html("请输入新密码！");
            returnstr = false;
        }
    }
    if (returnstr) {
        var number = /[0-9]/i;
        var letter = /[a-z]/i;
        var numberres = number.test(tempnewpwd);
        var letterres = letter.test(tempnewpwd);
        var length = tempnewpwd.length;
        if (!numberres || !letterres || length < 8 || length > 20) {
            $("#forgetcodemessage2").html("密码长度必须为8-20位且包含字母和数字！");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (tempconfirmnewpwd == "" || tempconfirmnewpwd == null || tempconfirmnewpwd == undefined || tempconfirmnewpwd == "确认新密码") {
            //layer.alert("验证码不正确！");
            $("#forgetcodemessage2").html("请输入确认新密码！");
            returnstr = false;
        }
    }
    if (returnstr) {
        //对比新密码和二次输入新密码
        if (tempnewpwd != tempconfirmnewpwd) {
            $("#forgetcodemessage2").html("两次输入的密码不匹配！");
            returnstr = false;
        }
    }
    return returnstr;
}