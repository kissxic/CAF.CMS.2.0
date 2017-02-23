var url= "http://"+window.location.host;
$(function(){
//获取登陆状态
        $.ajax({
            type: 'POST',
            async: false,
            url: ''+url+'/Welcome/isLogincheck/',
            data: {name:"1"},
            success: function(data){
                var obj = JSON.parse(data);
                var _html="<a class='top_home' href='/'>药坊网首页</a><span>&nbsp;&nbsp;|&nbsp;&nbsp;</span>";
                if(obj.errorcode==1)
                {
                    if(obj.agent_advertise_flag==1)
                    {
                        _html+="<span>欢迎您:"+obj.username+"，<a href='/Advertisecenter/'>会员中心</a></span><a class='red' href='javascript:;' onclick='loginout()'>【退出登录】</a>";
                    }
                    if(obj.agent_advertise_flag==2)
                    {
                        _html+="<span>欢迎您:"+obj.username+"，<a href='/Agentcenter/'>会员中心</a></span><a class='red' href='javascript:;' onclick='loginout()'>【退出登录】</a>";
                    }
                }else
                {
                    _html+="<a href='/Login' rel='nofollow' target='_blank'>请登录</a>&nbsp;<a href='/Register/' rel='nofollow' target='_blank'>免费注册</a>";
                }
                $("#loginstatus").append(_html);
            }
        });
		$(document).keydown(function(event){
				if(event.keyCode==13){
					$("#btnsearch").click();
				}
				});
		 $("#btnsearch").click(function () {
                                var keywords= $.trim($("#mq").val());
                                if(keywords=="")
                                {
                                    alert("请输入药品名称或者疾病名称");
                                    return false;
                                }
                                //类型
                                 var swords= $("#stype option:selected").val();
                                window.location.href="/list/0_0_0_0_0_0_"+swords+"_1_"+keywords+".html";
                            });
});
  function loginout()
    {
        $.ajax({
            type: 'POST',
            async: false,
            url: ''+url+'/Login/loginout/',
            success: function(data){
                alert("退出成功");
                window.location.href=""+url+"/Login/";
            }
        });
    }