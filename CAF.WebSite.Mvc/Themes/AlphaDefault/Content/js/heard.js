var url = "http://" + window.location.host;
$(function () {

    //获取登陆状态
    $.ajax({
        url: 'http://localhost:8081/Member/IsLogin',
        type: "GET",
        dataType: "jsonp",
        jsonpCallback: "localJsonpCallback"
    });
  
});
function localJsonpCallback(json) {
    var html = "";
    if (json.Code == true) {
        html = '<span>欢迎您:' + json.UserName + '</span>'
                 + '<a class="red" href="' + url + '/Logout">【退出登录】</a>';
    } else {
        html = '<a href="' + url + '/login" rel="nofollow" target="_blank">请登录</a><text>&nbsp;</text>'
             + '<a href="' + url + '/register" rel="nofollow" target="_blank">免费注册</a>';
    }
    $("#loginfo").append(html);
}

