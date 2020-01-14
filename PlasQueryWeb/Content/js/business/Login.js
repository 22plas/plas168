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
    var phone = vm.codeloginphone;
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
                    vm.codeloginmessage = "";
                    window.location.href = comm.action("Index", "Home");
                }
                else if (data.State == "Fail") {
                    //layer.alert("登录失败！");
                    vm.codeloginmessage = "登录失败！";
                }
                else {
                    //layer.alert("系统异常！");
                    vm.codeloginmessage = "系统异常！";
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
    vm.codeloginmessage = "";
    var phonestr = vm.codeloginphone;
    if (phonestr == "" || phonestr == null || phonestr == undefined) {
        //layer.alert("请输入手机号！");
        vm.codeloginmessage = "请输入手机号！";
        return;
    }
    var myreg = /^[1][3,4,5,7,8,9][0-9]{9}$/;
    if (!myreg.test(phonestr)) {
        vm.codeloginmessage = "请输入正确的手机号！";
        return;
    }
    if (vm.codetimestr == "获取验证码" || vm.codetimestr == "重新获取") {
        var a = 59;
        clearInterval(timer);
        vm.codetimestr = a;
        //获取验证码倒计时
        var timer = setInterval(function () {
            if (a > 1) {
                a--;
                vm.codetimestr = a;
            } else if (a == 1) {
                clearInterval(timer);
                vm.codetimestr = "重新获取";
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
                    vm.codeloginmessage = "";
                }
                else if (data.State == "Fail") {
                    //layer.alert("发送失败！");
                    vm.codeloginmessage = "发送失败！";
                }
                else {
                    //layer.alert("系统异常！");
                    vm.codeloginmessage = "系统异常！";
                }
            }
        });
    }
}

//验证码登录数据验证
function verificationforcode() {
    var phone = vm.codeloginphone;
    var thiscode = vm.codelogincode;
    var returnstr = true;
    if (returnstr) {
        if (phone == "" || phone == null || phone == undefined || phone == "请输手机号") {
            //layer.alert("请输手机号！");
            vm.codeloginmessage = "请输手机号！";
            returnstr = false;
        }
    }
    if (returnstr) {
        //验证输入的验证码是否匹配
        if (thiscode=="") {
            //layer.alert("验证码不正确！");
            vm.codeloginmessage = "请输入验证码！";
            returnstr = false;
        }
    }
    if (returnstr) {
        //验证输入的验证码是否匹配
        if (thiscode != _returncode) {
            //layer.alert("验证码不正确！");
            vm.codeloginmessage = "验证码不正确！";
            returnstr = false;
        }
    }
    return returnstr;
}

//密码登录方法
function pwdlogin() {
    var usaccount = vm.loginphone;
    var uspassword = vm.loginpwd;
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
                    vm.accountloginmessage = "";
                }
                else if (data.State == "NoFind") {
                    //layer.alert("账号不存在！");
                    vm.accountloginmessage = "账号不存在！";
                }
                else if (data.State == "Fail") {
                    //layer.alert("登录失败！");
                    vm.accountloginmessage = "账号或密码错误！";
                }
                else {
                    //layer.alert("系统异常！");
                    vm.accountloginmessage = "系统异常！";
                }
            }
        });
    }
}
//验证保存数据
function verification() {
    var usaccount = vm.loginphone;
    var uspassword = vm.loginpwd;
    var returnstr = true;
    if (returnstr) {
        if (usaccount == "" || usaccount == null || usaccount == undefined || usaccount == "请输手机号") {
            //layer.alert("请输手机号！");
            vm.accountloginmessage = "请输手机号！";
            returnstr = false;
        }
    }
    if (returnstr) {
        if (uspassword == "" || uspassword == null || uspassword == undefined || uspassword == "请输入密码") {
            //layer.alert("请输入登录密码！");
            vm.accountloginmessage = "请输入登录密码！";
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
            data: { phone: vm.forgetphone },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    vm.forgetcodemessage1 = "";
                    $("#forget1").hide();
                    $("#forget2").show();
                    $("#forget3").hide();
                    //vm.forgetcode = "";
                    //vm.forgetphone = "";
                    _forgetpwdreturncode = "";
                }
                else if (data.State == "NotFind") {
                    //layer.alert("发送失败！");
                    vm.forgetcodemessage1 = "账号不存在！";
                }
                else if (data.State == "Fail") {
                    //layer.alert("发送失败！");
                    vm.forgetcodemessage1 = "发送失败！";
                }
                else {
                    //layer.alert("系统异常！");
                    vm.forgetcodemessage1 = "系统异常！";
                }
            }
        });
    }
}
//修改密码数据验证
function forgetpwdverificationforcode() {
    var forgetphone = vm.forgetphone;
    var forgetthiscode = vm.forgetcode;
    var returnstr = true;
    if (returnstr) {
        if (forgetphone == "" || forgetphone == null || forgetphone == undefined || forgetphone == "请输手机号") {
            //layer.alert("请输手机号！");
            vm.forgetcodemessage1 = "请输手机号！";
            returnstr = false;
        }
    }
    if (returnstr) {
        //验证输入的验证码是否匹配
        if (forgetthiscode == "") {
            //layer.alert("验证码不正确！");
            vm.forgetcodemessage1 = "请输入验证码！";
            returnstr = false;
        }
    }
    if (returnstr) {
        //验证输入的验证码是否匹配
        if (forgetthiscode != _forgetpwdreturncode) {
            //layer.alert("验证码不正确！");
            vm.forgetcodemessage1 = "验证码不正确！";
            returnstr = false;
        }
    }
    return returnstr;
}
//修改密码发送验证码
function forgetpwdsetcode() {
    vm.forgetcodemessage1 = "";
    var phonestr = vm.forgetphone;
    if (phonestr == "" || phonestr == null || phonestr == undefined) {
        //layer.alert("请输入手机号！");
        vm.forgetcodemessage1 = "请输入手机号！";
        return;
    }
    var myreg = /^[1][3,4,5,7,8,9][0-9]{9}$/;
    if (!myreg.test(phonestr)) {
        vm.forgetcodemessage1 = "请输入正确的手机号！";
        return;
    }
    if (vm.forgetcodetimestr == "获取验证码" || vm.forgetcodetimestr == "重新获取") {
        var a = 59;
        clearInterval(timer);
        vm.forgetcodetimestr = a;
        //获取验证码倒计时
        var timer = setInterval(function () {
            if (a > 1) {
                a--;
                vm.forgetcodetimestr = a;
            } else if (a == 1) {
                clearInterval(timer);
                vm.forgetcodetimestr = "重新获取";
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
                    vm.forgetcodemessage1 = "";
                }
                else if (data.State == "Fail") {
                    //layer.alert("发送失败！");
                    vm.forgetcodemessage1 = "发送失败！";
                }
                else {
                    //layer.alert("系统异常！");
                    vm.forgetcodemessage1 = "系统异常！";
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
            data: { phone: vm.forgetphone, newpwd: vm.confirmnewpwd },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    $("#forget1").hide();
                    $("#forget2").hide();
                    $("#forget3").show();
                    vm.forgetcodemessage2 = "";
                }
                else if (data.State == "Fail") {
                    //layer.alert("发送失败！");
                    vm.forgetcodemessage2 = "发送失败！";
                }
                else {
                    //layer.alert("系统异常！");
                    vm.forgetcodemessage2 = "系统异常！";
                }
            }
        });
    }
}
//验证修改密码
function submitverification()
{
    var returnstr = true;
    if (returnstr) {
        if (vm.newpwd == "" || vm.newpwd == null || vm.newpwd == undefined || vm.newpwd == "设置新密码") {
            //layer.alert("请输手机号！");
            vm.forgetcodemessage2 = "请输入新密码！";
            returnstr = false;
        }
    }
    if (returnstr) {
        var number = /[0-9]/i;
        var letter = /[a-z]/i;
        var numberres = number.test(vm.newpwd);
        var letterres = letter.test(vm.newpwd);
        var length = vm.newpwd.length;
        if (!numberres || !letterres || length < 8 || length > 20) {
            vm.forgetcodemessage2 = "密码长度必须为8-20位且包含字母和数字";
            returnstr = false;
        }
    }
    if (returnstr) {
        if (vm.confirmnewpwd == "" || vm.confirmnewpwd == null || vm.confirmnewpwd == undefined || vm.confirmnewpwd == "确认新密码") {
            //layer.alert("验证码不正确！");
            vm.forgetcodemessage2 = "请输入确认新密码！";
            returnstr = false;
        }
    }
    if (returnstr) {
        //对比新密码和二次输入新密码
        if (vm.newpwd != vm.confirmnewpwd) {
            vm.forgetcodemessage2 = "两次输入的密码不匹配！";
            returnstr = false;
        }
    }
    return returnstr;
}