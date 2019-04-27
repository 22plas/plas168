var myarea = new Area("#Province", "#City", "#Area");

//删除
function deleteitem(t)
{
    layer.msg('确定要删除吗？', {
        time: 0 //不自动关闭
        , btn: ['删除', '取消']
        , yes: function (index) {
            layer.close(index);
            $.ajax({
                type: "post",
                url: comm.action("DeleteCompanyInfo", "MemberCenter"),
                data: { id: t.id },
                dataType: "json",
                success: function (data) {
                    console.log(data);
                    if (data.State == "Success") {
                        var rs = data.Result;
                        layer.msg('删除成功！', {
                            skin: 'layermsgcss',
                            time: 2000, //2s后自动关闭
                            btn: []
                        }, function () {
                            window.location.href = comm.action("CompanyInfo", "MemberCenter");
                        });
                    }
                    else if (data.State == "Fail") {
                        layer.alert("删除失败！");
                    }
                    else {
                        layer.alert("系统异常！");
                    }
                }
            });
        }
    });
}