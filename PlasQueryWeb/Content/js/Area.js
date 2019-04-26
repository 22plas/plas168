var Area = function (p, c, a) {
    var $p = $(p);
    var $c = $(c);
    var $a = $(a);

    //选择省份事件
    $p.change(function () {
        var selectValue = $p.children('option:selected').val();
        $c.children("option").remove()
        $a.children("option").remove()
        $.ajax({
            type: "POST",
            url: comm.action("Getarea", "Area"),
            data: { parentname: selectValue, level:"1" },
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    var rs = data.Result.data;
                    for (var i = 0; i < rs.length; i++) {
                        $c.append("<option value='" + rs[i] + "'>" + rs[i] + "</option>");
                    }
                    var cname = $c.children('option:selected').val();
                    setdistrict(selectValue, cname);
                }
            }
        });
    });
    //选择城市事件
    $c.change(function () {
        var cname = $c.children('option:selected').val();
        $a.children("option").remove()
        setdistrict(cname);
    });
    //设置区域
    function setdistrict(cname) {
        $.ajax({
            type: "POST",
            url: comm.action("Getarea", "Area"),
            data: { parentname: cname, level: "2" },
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    var rs = data.Result.data;
                    for (var i = 0; i < rs.length; i++) {
                        $a.append("<option value='" + rs[i] + "'>" + rs[i] + "</option>");
                    }
                }
            }
        });
    }
}