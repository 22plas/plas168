//获取数据
//type请求类型(get、post)、datastr请求数据、getpath请求接口
function GetData(type,datastr,getpath,callback)
{
   var urlstr='https://www.168plas.com/';
  //var urlstr='http://192.168.0.105:5002/';
  api.ajax({
    url: urlstr+getpath,
    method: type,
    contentType: "application/json",
    cache: true,
    timeout: 3000,
    dataType: 'json',
    data:{values:datastr}
  }, function(ret, err) {
    // alert(JSON.stringify(ret));
    // alert(JSON.stringify(err));
    if (ret) {
      callback(ret);
    }
    else {
      callback(err);
      	//  api.toast({
      	// 	 msg: '获取失败!',
      	// 	 duration: 2000,
      	// 	 location: 'bottom'
      	//  });
    }
  });
}
//生成GUID
function guid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}
