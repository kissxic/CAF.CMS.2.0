
function slidcontent(level1,dytime,sliddiv) {
    $(level1).stop().hover(function(){
        var varthis = $(this);
        varthis.find(sliddiv).addClass("disappear");
        delytime=setTimeout(function(){
            varthis.find(sliddiv).slideDown();
            varthis.find(sliddiv).removeClass("disappear");
        },dytime);
    },function(){
        var varthis = $(this);
        clearTimeout(delytime);
        varthis.find(sliddiv).slideUp();
        varthis.find(sliddiv).addClass("disappear");
    });
}

//首页楼层选项卡切换
function floornavshow(floornav,foucuscss,fcontentname,childmodel){
    $(floornav).mouseover(function(e){
        $(floornav).removeClass(foucuscss);
        $(this).addClass(foucuscss);
        $(fcontentname).children(childmodel).addClass("disappear");
        var indexspan= $(this).index();
        console.info(indexspan);
        $(fcontentname).children(childmodel+':eq(' + indexspan + ')').removeClass("disappear");
    });
}

function showtable(tbflag,tbnum,curtable,tdyes,tdno)
{
    for(i=1; i<=tbnum;i++)
    {
        if(i==curtable)
        {
            document.getElementById(tbflag+i).className=tdyes;
            document.getElementById("div_"+i).style.display='';
        }
        else
        {
            document.getElementById(tbflag+i).className=tdno;
            document.getElementById("div_"+i).style.display='none';
        }
    }
}

var countdown=60;
function settime(val) {
    if (countdown == 0) {
        document.getElementById("checkCodeImg").click();
        val.removeAttribute("disabled");
        val.value="获取验证码";
        countdown = 60;
    } else {
        val.setAttribute("disabled", true);
        val.value="重新发送(" + countdown + ")";
        countdown--;
        setTimeout(function() {
            settime(val);
        },1000)
    }
}

function settime1(val) {
    if (countdown == 0) {
        document.getElementById("checkCode1").click();
        val.removeAttribute("disabled");
        val.value="获取验证码";
        countdown = 60;
    } else {
        val.setAttribute("disabled", true);
        val.value="重新发送(" + countdown + ")";
        countdown--;
        setTimeout(function() {
            settime1(val);
        },1000)
    }
}

//广告倒计时
function timercountdown(duedate,innerid){
    //兼容火狐Date方法
    var duedates = duedate.split(' ');
    var date1=duedates[0].split('-');
    var date2 =duedates[1].split(':');
    var ts = parseInt(new Date(date1[0],date1[1],date1[2],date2[0],date2[1],date2[2]).getTime()) - parseInt(new Date().getTime());//计算剩余的毫秒数
    var dd = parseInt(ts / 1000 / 60 / 60 / 24, 10);//计算剩余的天数
    var hh = parseInt(ts / 1000 / 60 / 60 % 24, 10);//计算剩余的小时数
    var mm = parseInt(ts / 1000 / 60 % 60, 10);//计算剩余的分钟数
    //var ss = parseInt(ts / 1000 % 60, 10);//计算剩余的秒数
    dd = checkTime(dd);
    hh = checkTime(hh);
    mm = checkTime(mm);
    //ss = checkTime(ss);
    var htmlstr ="<ul><li><label>"+dd+"</label><span>天</span></li>";
    htmlstr +="<li><label>"+hh+"</label><span>小时</span></li>";
    htmlstr +="<li><label>"+mm+"</label><span>分钟</span></li></ul>";
    document.getElementById(innerid).innerHTML = htmlstr;

    setInterval("timercountdown('"+duedate+"','"+innerid+"')",1000*60);
}
function checkTime(i)
{
    if (i < 10 && i > 0) {
        i = "0" + i;
    }else if(i < 0){
        i="00";
    }
    return i;
}

//信息随机排列
function randinfo(listname){
    $(listname).each(function(){
        if(parseInt(Math.random()*2)==0){
            $(this).prependTo($(this).parent());
        }
    });
}
//详情页展开收起
function showdetail(clickname,showname,heightnum){
    if ($(clickname).html() == "展开") {
        $(showname).css({height:'auto'});
        $(clickname).html("收起");
    } else if ($(clickname).html() == "收起") {
        $(showname).css({height:heightnum});
        $(clickname).html("展开");
    }
}


//文本框验证
function validatetxt(validateid,message,falsecss){
    $(validateid).focus(function () {
        if( $(validateid).val() == message){
            $(validateid).val("").css("color", "#000").removeClass(falsecss);
        }
        else{
            $(validateid).css("color", "#000").removeClass(falsecss);
        }
    }).blur(function () {
        var validatevalue = $.trim($(validateid).val());
        if (validatevalue == "" || validatevalue == message) {
            $(validateid).addClass(falsecss);
            $(validateid).css("color", "#ef0000").val(message);
            return false;
        }
        $(validateid).css("color", "#000").removeClass(falsecss);
    });
}

//密码验证
function validatepassword(validateid,message,falsecss){
    $(validateid).focus(function () {//密码
        if($(validateid).val() == message)
        {
            $(validateid).val("").attr("type", "password").css("color", "#000");
        }else{
            $(validateid).attr("type", "password").css("color", "#000");
        }
    }).blur(function () {
        var pwdval = $.trim($(validateid).val());
        if (pwdval == "" || pwdval == message) {
            $(validateid).addClass("input_false");
            $(validateid).attr("type", "text").css("color", "#ef0000").val(message);
            return false;
        }
        $(validateid).attr("type", "password").css("color", "#000").removeClass(falsecss);
    });
}

//重复验证密码
function validatepasswordagain(validateid,contrastid,message,message2,falsecss){
    $(validateid).focus(function () {//密码
        if($(validateid).val() == message || $(validateid).val() ==message2)
        {
            $(validateid).val("").attr("type", "password").css("color", "#000");
        }else{
            $(validateid).attr("type", "password").css("color", "#000");
        }
    }).blur(function () {
        var  pwd = $.trim($(contrastid).val());

        var pwdrepeat = $.trim($(validateid).val());

        if (pwdrepeat == "" || pwdrepeat == message || pwdrepeat==message2) {
            $(validateid).addClass("input_false");
            $(validateid).attr("type", "text").css("color", "#ef0000").val(message);
            return false;
        }
        if (pwdrepeat != pwd) {
            $(validateid).addClass("input_false");
            $(validateid).attr("type", "text").css("color", "#ef0000").val(message2);
            return false;
        }
        $(validateid).attr("type", "password").css("color", "#000").removeClass(falsecss);
    });
}

//验证码
function validatecode(validateid,message,falsecss){
    $(validateid).focus(function() {//验证码
        if ($(validateid).val() == message ) {
            $(validateid).val("").css("color", "#000").removeClass(falsecss);
        }else{
            $(validateid).css("color", "#000").removeClass(falsecss);
        }
    }).blur(function(){
       var phonecode = $(validateid).val().trim();
        if (phonecode == "" || phonecode == message ) {
            $(validateid).addClass(falsecss);
            $(validateid).css("color", "#ef0000").val(message);
            return false;
        }
        $(validateid).css("color", "#000").removeClass(falsecss);
    });

}

//div切换
function togglediv(clicknav,navhoverclass,containerdiv){
    $(clicknav).click(function () {
        $(clicknav).removeClass(navhoverclass);
        $(this).addClass(navhoverclass);
        $(containerdiv).children("div").addClass("disappear");
        $(containerdiv).children('div:eq(' + $(this).index() + ')').removeClass("disappear");
    });
}


function _toastr(_message, _position, _notifyType, _onclick) {
    var _btn = $(".toastr-notify");

    if (_btn.length > 0 || _message != false) {


        /** JAVSCRIPT / ON LOAD
         ************************* **/
        if (_message != false) {

            if (_onclick != false) {
                onclick = function () {
                    window.location = _onclick;
                }
            } else {
                onclick = null
            }

            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": true,
                "positionClass": "toast-" + _position,
                "preventDuplicates": false,
                "onclick": onclick,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }

            setTimeout(function () {
                toastr[_notifyType](_message);
            }, 0); // delay 1.5s
        }
    }

}

//首页主菜单弹出
$(function(){
//$(".category_menu").append('<div class="dd dropdownmenu disappear"style="display: none;"><div class="dd_inner"><div class="dd_item"><div class="item_top"><a href="http://service.goodyao.com/"class="itemtop_a topa1"target="_blank">综合服务</a></div><div class="item_con"><a href="http://service.goodyao.com/"target="_blank">医药金融</a><a href="http://service.goodyao.com/"target="_blank">企业宣传</a><a href="http://service.goodyao.com/"target="_blank">医药人脉</a><a href="http://service.goodyao.com/"target="_blank">转让</a><a href="http://service.goodyao.com/"target="_blank">众筹</a><a href="http://service.goodyao.com/"target="_blank">交易平台</a></div></div><div class="dd_item"><div class="item_top"><a href="http://www.goodyao.com/list/0_46_0_0_0_0.html"class="itemtop_a topa1">中西药品</a></div><div class="item_con"><a href="http://www.goodyao.com/list/0_46_18_0_0_0.html">OTC</a><a href="http://www.goodyao.com/list/0_46_1273_0_0_0.html">处方药</a></div><div class="dropdown_layer disappear"style="top: 40px;"><div class="dditem"><div class="subitems"><div class="itemstop"></div><div class="itemscontent"><dl><dt><a href="http://www.goodyao.com/list/0_46_18_0_0_0.html"target="_blank">OTC</a></dt><dd><a href="http://www.goodyao.com/list/0_46_18_0_0_0__20.html"target="_blank">清热消炎</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__21.html"target="_blank">感冒发热</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__32.html"target="_blank">抗微生物</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__33.html"target="_blank">消化系统</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__34.html"target="_blank">肝胆用药</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__35.html"target="_blank">妇科用药</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__36.html"target="_blank">儿童用药</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__37.html"target="_blank">眼科</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__38.html"target="_blank">骨伤用药</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__39.html"target="_blank">皮肤用药</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__40.html"target="_blank">五官科</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__41.html"target="_blank">呼吸系统</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__42.html"target="_blank">滋补用药</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__43.html"target="_blank">维矿物质</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__44.html"target="_blank">心脑血管</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__45.html"target="_blank">内分泌</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__131.html"target="_blank">生殖系统</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__132.html"target="_blank">泌尿系统</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__133.html"target="_blank">神经系统</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__134.html"target="_blank">血液系统</a><a href="http://www.goodyao.com/list/0_46_18_0_0_0__135.html"target="_blank">肿瘤药品</a></dd></dl><dt><a href="http://www.goodyao.com/list/0_46_1273_0_0_0.html"target="_blank">处方药</a></dt><dd></dd></div></div><div class="itemrecommond"><div><dl><dt>热门品牌</dt><dd><a href="javascript::"><img src="/application/views/images/img_pinpai01.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai04.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a></dd></dl></div><div><dl><dt>推荐商家</dt><dd></dd></dl></div></div></div></div></div><div class="dd_item"><div class="item_top"><a href="http://www.goodyao.com/list/0_27_0_0_0_0.html"class="itemtop_a topa2">进口产品</a></div><div class="item_con"><a href="http://www.goodyao.com/list/0_27_482_0_0_0.html">进口乳品</a><a href="http://www.goodyao.com/list/0_27_486_0_0_0.html">进口茶饮</a><a href="http://www.goodyao.com/list/0_27_488_0_0_0.html">营养保健</a></div><div class="dropdown_layer disappear"style="top: 40px;"><div class="dditem"><div class="subitems"><div class="itemscontent"><dl><dt><a href="http://www.goodyao.com/list/0_27_482_0_0_0.html"target="_blank">进口乳品</a></dt><dd><a href="http://www.goodyao.com/list/0_27_482_0_0_0__490.html"target="_blank">进口牛奶</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__491.html"target="_blank">进口酸奶</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__492.html"target="_blank">进口酸奶粉</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__493.html"target="_blank">全脂牛奶</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__494.html"target="_blank">脱脂牛奶</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__495.html"target="_blank">低脂牛奶</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__496.html"target="_blank">低温牛奶</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__497.html"target="_blank">德国牛奶</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__498.html"target="_blank">新西兰牛奶</a><a href="http://www.goodyao.com/list/0_27_482_0_0_0__499.html"target="_blank">澳洲牛奶</a></dd></dl><dt><a href="http://www.goodyao.com/list/0_27_486_0_0_0.html"target="_blank">进口茶饮</a></dt><dd><a href="http://www.goodyao.com/list/0_27_486_0_0_0__530.html"target="_blank">进口茶叶</a><a href="http://www.goodyao.com/list/0_27_486_0_0_0__531.html"target="_blank">进口奶茶</a><a href="http://www.goodyao.com/list/0_27_486_0_0_0__532.html"target="_blank">进口天然粉</a><a href="http://www.goodyao.com/list/0_27_486_0_0_0__533.html"target="_blank">进口麦片</a><a href="http://www.goodyao.com/list/0_27_486_0_0_0__534.html"target="_blank">进口咖啡</a><a href="http://www.goodyao.com/list/0_27_486_0_0_0__535.html"target="_blank">冲调果汁</a></dd><dt><a href="http://www.goodyao.com/list/0_27_488_0_0_0.html"target="_blank">营养保健</a></dt><dd><a href="http://www.goodyao.com/list/0_27_488_0_0_0__1115.html"target="_blank">营养健康</a><a href="http://www.goodyao.com/list/0_27_488_0_0_0__1116.html"target="_blank">营养成分</a><a href="http://www.goodyao.com/list/0_27_488_0_0_0__1117.html"target="_blank">滋补养生</a></dd></div></div><div class="itemrecommond"><div><dl><dt>热门品牌</dt><dd><a href="javascript::"><img src="/application/views/images/img_pinpai01.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai04.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a></dd></dl></div><div><dl><dt>推荐商家</dt><dd></dd></dl></div></div></div></div></div><div class="dd_item"><div class="item_top"><a href="http://www.goodyao.com/list/0_24_0_0_0_0.html"class="itemtop_a topa3">保健品</a></div><div class="item_con"><a href="http://www.goodyao.com/list/0_24_1220_0_0_0.html">免疫增强</a><a href="http://www.goodyao.com/list/0_24_1221_0_0_0.html">辅助降脂</a><a href="http://www.goodyao.com/list/0_24_1222_0_0_0.html">辅助降糖</a></div><div class="dropdown_layer disappear"style="top: 40px;"><div class="dditem"><div class="subitems"><div class="itemscontent"><dl><dt><a href="http://www.goodyao.com/list/0_24_1220_0_0_0.html"target="_blank">免疫增强</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1220_0_0_0__1247.html"target="_blank">调节免疫</a></dd></dl><dt><a href="http://www.goodyao.com/list/0_24_1221_0_0_0.html"target="_blank">辅助降脂</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1221_0_0_0__1248.html"target="_blank">降血脂</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1222_0_0_0.html"target="_blank">辅助降糖</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1222_0_0_0__1249.html"target="_blank">降血糖</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1223_0_0_0.html"target="_blank">抗氧化</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1223_0_0_0__1250.html"target="_blank">抗氧化</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1224_0_0_0.html"target="_blank">记忆改善</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1224_0_0_0__1251.html"target="_blank">增强记忆</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1225_0_0_0.html"target="_blank">视疲劳</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1225_0_0_0__1252.html"target="_blank">缓解视力疲劳</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1226_0_0_0.html"target="_blank">促进排铅</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1226_0_0_0__1253.html"target="_blank">增强排铅</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1227_0_0_0.html"target="_blank">清咽利喉</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1227_0_0_0__1254.html"target="_blank">清咽</a><a href="http://www.goodyao.com/list/0_24_1227_0_0_0__1255.html"target="_blank">护喉</a><a href="http://www.goodyao.com/list/0_24_1227_0_0_0__1256.html"target="_blank">护嗓</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1228_0_0_0.html"target="_blank">辅助降压</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1228_0_0_0__1257.html"target="_blank">降血压</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1229_0_0_0.html"target="_blank">改善睡眠</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1229_0_0_0__1258.html"target="_blank">增强睡眠</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1230_0_0_0.html"target="_blank">促进泌乳</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1230_0_0_0__1259.html"target="_blank">增强泌乳</a></dd><dt><a href="http://www.goodyao.com/list/0_24_1231_0_0_0.html"target="_blank">缓解疲劳</a></dt><dd><a href="http://www.goodyao.com/list/0_24_1231_0_0_0__1260.html"target="_blank">缓解体力疲劳</a></dd></div></div><div class="itemrecommond"><div><dl><dt>热门品牌</dt><dd><a href="javascript::"><img src="/application/views/images/img_pinpai01.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai04.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a></dd></dl></div><div><dl><dt>推荐商家</dt><dd></dd></dl></div></div></div></div></div><div class="dd_item"><div class="item_top"><a href="http://www.goodyao.com/list/0_25_0_0_0_0.html"class="itemtop_a topa4">医疗器械</a></div><div class="item_con"><a href="http://www.goodyao.com/list/0_25_56_0_0_0.html">测量仪器</a><a href="http://www.goodyao.com/list/0_25_57_0_0_0.html">家用医疗</a><a href="http://www.goodyao.com/list/0_25_58_0_0_0.html">助行器</a></div><div class="dropdown_layer disappear"style="top: 40px;"><div class="dditem"><div class="subitems"><div class="itemstop"></div><div class="itemscontent"><dl><dt><a href="http://www.goodyao.com/list/0_25__0_0_0.html"target="_blank">测量仪器</a></dt><dd><a href="http://www.goodyao.com/list/0_25_56_0_0_0__95.html"target="_blank">血压计</a><a href="http://www.goodyao.com/list/0_25_56_0_0_0__96.html"target="_blank">血糖仪</a><a href="http://www.goodyao.com/list/0_25_56_0_0_0__97.html"target="_blank">电子体温计</a></dd></dl><dt><a href="http://www.goodyao.com/list/0_25__0_0_0.html"target="_blank">家用医疗</a></dt><dd><a href="http://www.goodyao.com/list/0_25_57_0_0_0__98.html"target="_blank">眼科护理</a><a href="http://www.goodyao.com/list/0_25_57_0_0_0__99.html"target="_blank">光学仪器</a><a href="http://www.goodyao.com/list/0_25_57_0_0_0__100.html"target="_blank">实验箱类</a><a href="http://www.goodyao.com/list/0_25_57_0_0_0__164.html"target="_blank">针灸用药</a><a href="http://www.goodyao.com/list/0_25_57_0_0_0__165.html"target="_blank">按摩仪器</a></dd><dt><a href="http://www.goodyao.com/list/0_25__0_0_0.html"target="_blank">助行器</a></dt><dd><a href="http://www.goodyao.com/list/0_25_58_0_0_0__101.html"target="_blank">拐杖</a><a href="http://www.goodyao.com/list/0_25_58_0_0_0__102.html"target="_blank">轮椅</a></dd><dt><a href="http://www.goodyao.com/list/0_25__0_0_0.html"target="_blank">注射类</a></dt><dd><a href="http://www.goodyao.com/list/0_25_161_0_0_0__416.html"target="_blank">注射针类</a><a href="http://www.goodyao.com/list/0_25_161_0_0_0__417.html"target="_blank">采血容器</a><a href="http://www.goodyao.com/list/0_25_161_0_0_0__418.html"target="_blank">输液器类</a></dd><dt><a href="http://www.goodyao.com/list/0_25__0_0_0.html"target="_blank">手术械包</a></dt><dd><a href="http://www.goodyao.com/list/0_25_162_0_0_0__411.html"target="_blank">卫生敷料</a><a href="http://www.goodyao.com/list/0_25_162_0_0_0__412.html"target="_blank">粘合剂类</a><a href="http://www.goodyao.com/list/0_25_162_0_0_0__413.html"target="_blank">缝合材料</a><a href="http://www.goodyao.com/list/0_25_162_0_0_0__414.html"target="_blank">外科器械</a><a href="http://www.goodyao.com/list/0_25_162_0_0_0__415.html"target="_blank">橡胶用品</a></dd><dt><a href="http://www.goodyao.com/list/0_25__0_0_0.html"target="_blank">消毒用品</a></dt><dd><a href="http://www.goodyao.com/list/0_25_163_0_0_0__404.html"target="_blank">碘酒</a><a href="http://www.goodyao.com/list/0_25_163_0_0_0__405.html"target="_blank">棉签</a><a href="http://www.goodyao.com/list/0_25_163_0_0_0__406.html"target="_blank">绷带</a><a href="http://www.goodyao.com/list/0_25_163_0_0_0__407.html"target="_blank">纱布</a><a href="http://www.goodyao.com/list/0_25_163_0_0_0__408.html"target="_blank">消毒液</a><a href="http://www.goodyao.com/list/0_25_163_0_0_0__409.html"target="_blank">碘伏</a><a href="http://www.goodyao.com/list/0_25_163_0_0_0__410.html"target="_blank">酒精</a></dd><dt><a href="http://www.goodyao.com/list/0_25__0_0_0.html"target="_blank">护理护具</a></dt><dd><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1132.html"target="_blank">隐形眼镜</a><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1133.html"target="_blank">护理液</a><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1134.html"target="_blank">口罩</a><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1135.html"target="_blank">眼罩/耳塞</a><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1136.html"target="_blank">跌打损伤</a><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1137.html"target="_blank">暖贴</a><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1138.html"target="_blank">鼻喉护理</a><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1139.html"target="_blank">眼部保健</a><a href="http://www.goodyao.com/list/0_25_1131_0_0_0__1140.html"target="_blank">美体护理</a></dd></div></div><div class="itemrecommond"><div><dl><dt>热门品牌</dt><dd><a href="javascript::"><img src="/application/views/images/img_pinpai01.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai04.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a></dd></dl></div><div><dl><dt>推荐商家</dt><dd></dd></dl></div></div></div></div></div><div class="dd_item"><div class="item_top"><a href="http://www.goodyao.com/list/0_28_0_0_0_0.html"class="itemtop_a topa6">母婴产品</a></div><div class="item_con"><a href="http://www.goodyao.com/list/0_28_650_0_0_0.html">奶粉</a><a href="http://www.goodyao.com/list/0_28_668_0_0_0.html">营养辅食</a><a href="http://www.goodyao.com/list/0_28_669_0_0_0.html">尿裤湿巾</a></div><div class="dropdown_layer disappear"style="top: 233px;"><div class="dditem"><div class="subitems"><div class="itemstop"></div><div class="itemscontent"><dl><dt><a href="http://www.goodyao.com/list/0_28_650_0_0_0.html"target="_blank">奶粉</a></dt><dd><a href="http://www.goodyao.com/list/0_28_650_0_0_0__659.html"target="_blank">婴幼奶粉</a><a href="http://www.goodyao.com/list/0_28_650_0_0_0__663.html"target="_blank">成人奶粉</a></dd></dl><dt><a href="http://www.goodyao.com/list/0_28_668_0_0_0.html"target="_blank">营养辅食</a></dt><dd><a href="http://www.goodyao.com/list/0_28_668_0_0_0__678.html"target="_blank">DHA</a><a href="http://www.goodyao.com/list/0_28_668_0_0_0__679.html"target="_blank">钙铁锌/维生素</a><a href="http://www.goodyao.com/list/0_28_668_0_0_0__680.html"target="_blank">益生菌/初乳</a><a href="http://www.goodyao.com/list/0_28_668_0_0_0__681.html"target="_blank">清火/开胃</a><a href="http://www.goodyao.com/list/0_28_668_0_0_0__682.html"target="_blank">米粉/菜粉</a><a href="http://www.goodyao.com/list/0_28_668_0_0_0__683.html"target="_blank">果泥/果汁</a><a href="http://www.goodyao.com/list/0_28_668_0_0_0__684.html"target="_blank">面条/粥</a><a href="http://www.goodyao.com/list/0_28_668_0_0_0__685.html"target="_blank">宝宝零食</a></dd><dt><a href="http://www.goodyao.com/list/0_28_669_0_0_0.html"target="_blank">尿裤湿巾</a></dt><dd><a href="http://www.goodyao.com/list/0_28_669_0_0_0__686.html"target="_blank">婴儿尿裤</a><a href="http://www.goodyao.com/list/0_28_669_0_0_0__687.html"target="_blank">拉拉裤</a><a href="http://www.goodyao.com/list/0_28_669_0_0_0__688.html"target="_blank">成人尿裤</a><a href="http://www.goodyao.com/list/0_28_669_0_0_0__689.html"target="_blank">湿巾</a></dd><dt><a href="http://www.goodyao.com/list/0_28_670_0_0_0.html"target="_blank">洗护用品</a></dt><dd><a href="http://www.goodyao.com/list/0_28_670_0_0_0__690.html"target="_blank">宝宝护肤</a><a href="http://www.goodyao.com/list/0_28_670_0_0_0__691.html"target="_blank">宝宝洗浴</a><a href="http://www.goodyao.com/list/0_28_670_0_0_0__692.html"target="_blank">理发器</a><a href="http://www.goodyao.com/list/0_28_670_0_0_0__693.html"target="_blank">洗衣液/皂</a><a href="http://www.goodyao.com/list/0_28_670_0_0_0__694.html"target="_blank">奶瓶清洗</a><a href="http://www.goodyao.com/list/0_28_670_0_0_0__695.html"target="_blank">日常护理</a><a href="http://www.goodyao.com/list/0_28_670_0_0_0__696.html"target="_blank">座便器</a><a href="http://www.goodyao.com/list/0_28_670_0_0_0__697.html"target="_blank">驱蚊防蚊</a></dd><dt><a href="http://www.goodyao.com/list/0_28_671_0_0_0.html"target="_blank">喂养用品</a></dt><dd><a href="http://www.goodyao.com/list/0_28_671_0_0_0__698.html"target="_blank">奶瓶奶嘴</a><a href="http://www.goodyao.com/list/0_28_671_0_0_0__699.html"target="_blank">吸奶器</a><a href="http://www.goodyao.com/list/0_28_671_0_0_0__700.html"target="_blank">牙胶安抚</a><a href="http://www.goodyao.com/list/0_28_671_0_0_0__701.html"target="_blank">暖奶消毒</a><a href="http://www.goodyao.com/list/0_28_671_0_0_0__702.html"target="_blank">辅食料理机</a><a href="http://www.goodyao.com/list/0_28_671_0_0_0__703.html"target="_blank">碗盘叉勺</a><a href="http://www.goodyao.com/list/0_28_671_0_0_0__704.html"target="_blank">水壶/水杯</a></dd><dt><a href="http://www.goodyao.com/list/0_28_675_0_0_0.html"target="_blank">妈妈专区</a></dt><dd><a href="http://www.goodyao.com/list/0_28_675_0_0_0__723.html"target="_blank">妈咪包/背婴带</a><a href="http://www.goodyao.com/list/0_28_675_0_0_0__724.html"target="_blank">待产/新生</a><a href="http://www.goodyao.com/list/0_28_675_0_0_0__725.html"target="_blank">产后塑身</a><a href="http://www.goodyao.com/list/0_28_675_0_0_0__726.html"target="_blank">文胸/内裤</a><a href="http://www.goodyao.com/list/0_28_675_0_0_0__727.html"target="_blank">防辐射服</a><a href="http://www.goodyao.com/list/0_28_675_0_0_0__728.html"target="_blank">孕妇装</a><a href="http://www.goodyao.com/list/0_28_675_0_0_0__729.html"target="_blank">月子装</a><a href="http://www.goodyao.com/list/0_28_675_0_0_0__730.html"target="_blank">孕期营养</a><a href="http://www.goodyao.com/list/0_28_675_0_0_0__731.html"target="_blank">孕妈美容</a></dd></div></div><div class="itemrecommond"><div><dl><dt>热门品牌</dt><dd><a href="javascript::"><img src="/application/views/images/img_pinpai01.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai04.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai02.jpg"></a><a href="javascript::"><img src="/application/views/images/img_pinpai03.jpg"></a></dd></dl></div><div><dl><dt>推荐商家</dt><dd></dd></dl></div></div></div></div></div></div></div>');
	
    //主菜单弹出子菜单
    $('.dd_inner > .dd_item').hover(function(){
        var eq = $('.dd_inner > .dd_item').index(this),				//获取当前滑过是第几个元素
            h = $('.dd_inner').offset().top,						//获取当前下拉菜单距离窗口多少像素
            s = $(window).scrollTop(),									//获取游览器滚动了多少高度
            i = $(this).offset().top,									//当前元素滑过距离窗口多少像素
            item = $(this).children('.dropdown_layer').height(),				//下拉菜单子类内容容器的高度
            sort = $('.dd_inner').height();						//父类分类列表容器的高度

        if ( item < sort ){												//如果子类的高度小于父类的高度
            if ( eq == 0 ){
                $(this).children('.dropdown_layer').css('top', (i-h));
            } else {
                $(this).children('.dropdown_layer').css('top', (i-h)+37);
            }
        } else {
            if ( s > h ) {												//判断子类的显示位置，如果滚动的高度大于所有分类列表容器的高度
                if ( i-s > 0 ){											//则 继续判断当前滑过容器的位置 是否有一半超出窗口一半在窗口内显示的Bug,
                    $(this).children('.dropdown_layer').css('top', (s-h)+37 );
                } else {
                    $(this).children('.dropdown_layer').css('top', (s-h)-(-(i-s))+37 );
                }
            } else {
                $(this).children('.dropdown_layer').css('top', 40 );
            }
        }

        $(this).addClass('item_hover');
        $(this).children('.dropdown_layer').removeClass("disappear");
    },function(){
        $(this).removeClass('item_hover');
        $(this).children('.dropdown_layer').addClass("disappear");
    });
   

});



 