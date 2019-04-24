//提交登录
$("#savelogin").click(function () {
    var usaccount = $("#usaccount").val();
    var uspassword = $("#uspassword").val();
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
                }
                else if (data.State == "NoFind") {
                    layer.alert("账号不存在！");
                }
                else if (data.State == "Fail") {
                    layer.alert("登录失败！");
                }
                else {
                    layer.alert("系统异常！");
                }
            }
        });
    }
});
//验证保存数据
function verification() {
    var usaccount = $("#usaccount").val();
    var uspassword = $("#uspassword").val();
    var returnstr = true;

    if (returnstr) {
        if (usaccount == "" || usaccount == null || usaccount == undefined || usaccount == "请输手机号") {
            layer.alert("请输手机号！");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (uspassword == "" || uspassword == null || uspassword == undefined || uspassword == "请输入密码") {
            layer.alert("请输入登录密码！");
            returnstr = false;
        }
    }
    return returnstr;
}