var _returncode = "";
var _phone = "";
var _vcodereturn = false;//是否发送验证码
//获取验证码倒计时
$("#code").click(function () {
    var getcodebtn = document.getElementById("code");
    var valthis = $(getcodebtn).html();
    var phonestr = $("#phone").val();
    if (phonestr == "" || phonestr == null || phonestr == undefined) {
        layer.alert("请输入手机号！");
        return;
    }
    if (valthis == "获取验证码" || valthis == "重新获取") {
        var a = 59;
        clearInterval(timer);
        $(getcodebtn).html(a);
        //获取验证码倒计时
        var timer = setInterval(function () {
            if (a > 1) {
                a--;
                $(getcodebtn).html(a);
            } else if (a == 1) {
                clearInterval(timer);
                $(getcodebtn).html("重新获取");
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
                }
                else if (data.State == "Fail") {
                    layer.alert("发送失败！");
                }
                else {
                    layer.alert("系统异常！");
                }
            }
        });
    }
});
//保存
$("#savebtn").click(function () {
    var thiscode = $("#verificationcode").val();
    var account = $("#account").val();
    var password = $("#password").val();
    var passwordtwo = $("#passwordtwo").val();
    var phonestr = $("#phone").val();
    var recommendcode = $("#recommendcode").val();
    //验证输入的验证码是否匹配
    if (thiscode == _returncode) {
        var returnv = verification();
        if (returnv) {
            //保存用户注册信息
            $.ajax({
                type: "get",
                url: comm.action("SaveRegister", "MemberCenter"),
                data: { UserName: account, UserPwd: password, Phone: phonestr, RecommendPhone: recommendcode },
                dataType: "json",
                success: function (data) {
                    console.log(data);
                    if (data.State == "Success") {
                        //var rs = data.Result;
                        layer.msg('注册成功！', {
                            skin: 'layermsgcss',
                            time: 2000, //2s后自动关闭
                            btn: []
                        }, function () {
                            window.location.href = comm.action("Login", "MemberCenter");
                        });
                    }
                    else if (data.State == "AlreadyExist") {
                        layer.alert("该手机号已被注册！");
                    }
                    else if (data.State == "Fail") {
                        layer.alert("注册失败！");
                    }
                    else {
                        layer.alert("系统异常！");
                    }
                }
            });
        }
    }
    else {
        layer.alert("验证码不正确！");
    }
});
//验证保存数据
function verification() {
    var thiscode = $("#verificationcode").val();
    var account = $("#account").val();
    var password = $("#password").val();
    var passwordtwo = $("#passwordtwo").val();
    var phonestr = $("#phone").val();
    var returnstr = true;
    
    if (returnstr) {
        if (account == "" || account == null || account == undefined || account == "请输入帐号") {
            layer.alert("请输入账号！");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (password == "" || password == null || password == undefined || password == "请输入登录密码") {
            layer.alert("请输入登录密码！");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (passwordtwo == "" || passwordtwo == null || passwordtwo == undefined || passwordtwo == "请输入确认密码") {
            layer.alert("请输入确认密码！");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (password != passwordtwo) {
            layer.alert("两次输入密码不匹配！");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (phonestr == "" || phonestr == null || phonestr == undefined || phonestr == "请输入手机号码") {
            layer.alert("请输入手机号码！");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (thiscode == "" || thiscode == null || thiscode == undefined || thiscode == "请输入手机验证码") {
            layer.alert("请输入手机验证码！");
            returnstr = false;
        }
    }
    if (returnstr && _vcodereturn) {
        if (phonestr != _phone) {
            layer.alert("当前保存的手机号跟验证的手机号不匹配！");
            returnstr = false;
        }
    }
    return returnstr;
}