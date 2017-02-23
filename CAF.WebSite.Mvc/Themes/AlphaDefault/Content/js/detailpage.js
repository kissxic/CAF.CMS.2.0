
$(function () {

    slidcontent("#websitelevel", 300, "div");
    slidcontent("#qrcode", 300, "div");
 
    /* 更多 */
    var w = $(".effect_p").width();
    var moreText = $(".more_detail");
    var moreBtn = $(".more_btn");
    if (w >= 560) {
        moreBtn.css("display", "inline");
        $(".effect_p").css("width", "450px");
    }
    moreBtn.bind("mouseover", showfn);
    moreText.bind("mouseout", hidefn)
    function showfn() {
        moreText.show();
    }
    function hidefn() {
        moreText.hide();
    }
 

    //弹出主菜单
    $(".category_menu").hover(function () {
        $(".dropdownmenu").slideDown(300);
    }, function () {
        $(".dropdownmenu").slideUp(300);
    });
    //主菜单弹出子菜单
    $('.dd_inner > .dd_item').hover(function () {
        var eq = $('.dd_inner > .dd_item').index(this),				//获取当前滑过是第几个元素
            h = $('.dd_inner').offset().top,						//获取当前下拉菜单距离窗口多少像素
            s = $(window).scrollTop(),									//获取游览器滚动了多少高度
            i = $(this).offset().top,									//当前元素滑过距离窗口多少像素
            item = $(this).children('.dropdown_layer').height(),				//下拉菜单子类内容容器的高度
            sort = $('.dd_inner').height();						//父类分类列表容器的高度

        if (item < sort) {												//如果子类的高度小于父类的高度
            if (eq == 0) {
                $(this).children('.dropdown_layer').css('top', (i - h));
            } else {
                $(this).children('.dropdown_layer').css('top', (i - h) + 37);
            }
        } else {
            if (s > h) {												//判断子类的显示位置，如果滚动的高度大于所有分类列表容器的高度
                if (i - s > 0) {											//则 继续判断当前滑过容器的位置 是否有一半超出窗口一半在窗口内显示的Bug,
                    $(this).children('.dropdown_layer').css('top', (s - h) + 37);
                } else {
                    $(this).children('.dropdown_layer').css('top', (s - h) - (-(i - s)) + 37);
                }
            } else {
                $(this).children('.dropdown_layer').css('top', 40);
            }
        }

        $(this).addClass('item_hover');
        $(this).children('.dropdown_layer').removeClass("disappear");
    }, function () {
        $(this).removeClass('item_hover');
        $(this).children('.dropdown_layer').addClass("disappear");
    });
    //当点击跳转链接后，回到页面顶部位置
    $(".navigationtree .totop").click(function () {
        $('body,html').animate({ scrollTop: 0 }, 1000);
        return false;
    });
    //分享
    $(".div_share").hover(function () {
        $(".inshare").slideDown();
    }, function () {
        $(".inshare").slideUp();
    });
    var liuyancontent;//留言内容
    var shop_user_id, good_id;//店铺的会员id

    $("#liuyanbutton").click(function () {
        liuyancontent = $.trim($("#liuyancontent").val());
        shop_user_id = $.trim($("#shop_owner_id").val());
        good_id = $.trim($("#good_id").val());
        if (liuyancontent == "") {
            alert("请输入留言内容");
            return false;
        }
        //提交留言
        $.ajax({
            type: 'POST',
            async: false,
            url: 'http://www.goodyao.com/Detailpage/postsixin/',
            data: { sixincontent: liuyancontent, shop_user_id: shop_user_id, goods_id: good_id },
            success: function (data) {
                var obj = JSON.parse(data);
                if (obj.errorcode == -1) {
                    alert("自己不能给自己留言");
                }
                if (obj.errorcode == -2) {
                    alert("你还未登陆，暂不能留言");
                    window.location.href = "http://www.goodyao.com/login/";
                }
                if (obj.errorcode == 0) {
                    alert("留言成功,等待后台审核！");
                    window.location.reload();
                }
                if (obj.errorcode == 1) {
                    alert("传入字段不能为空");
                }
                if (obj.errorcode == 2) {
                    alert("该接收者不存在");
                }
                if (obj.errorcode == 6) {
                    alert("非系统消息必须传入接收者的key值");
                }
                if (obj.errorcode == 200) {
                    alert("用户登录验证失败，请重新登录");
                    window.location.href = "http://www.goodyao.com/login/";
                }
            }
        });
    });
    //发送私信
    $("#sendbutton").click(function () {
        var shop_owner_id = $.trim($("#shop_owner_id").val());
        var goods_id = $.trim($("#goods_id").val());
        var content = $.trim($("#sixincontent").val());
        if (content == "") {
            alert("请输入私信内容");
            $("#sixincontent").focus();
            return false;
        }
        //提交私信内容
        $.ajax({
            type: 'POST',
            async: false,
            url: 'http://www.goodyao.com/Detailpage/postsixin/',
            data: { sixincontent: content, shop_user_id: shop_owner_id, goods_id: goods_id },
            success: function (data) {
                var obj = JSON.parse(data);
                if (obj.errorcode == -1) {
                    alert("自己不能给自己发私信");
                }
                if (obj.errorcode == -2) {
                    alert("未登陆或登陆已失效");
                    window.location.href = "http://www.goodyao.com/login/";
                }
                if (obj.errorcode == 0) {
                    alert("私信发送成功");
                    $(".contract-dialog").hide();
                    $(".dialog-mask").hide();
                    $("#sixincontent").val("");
                }
                if (obj.errorcode == 1) {
                    alert("传入字段不能为空");
                }
                if (obj.errorcode == 2) {
                    alert("该接收者不存在");
                }
                if (obj.errorcode == 6) {
                    alert("非系统消息必须传入接收者的key值");
                }
                if (obj.errorcode == 200) {
                    alert("用户登录验证失败，请重新登录");
                    window.location.href = "http://www.goodyao.com/login/";
                }
            }
        });
    });

    $('.btnreach').click(function () {
        //判断是否登陆
        if ($("#loginflag").val() == 0) {
            alert("游客不能联系商家，请登陆");
            window.location.href = "/login";
        } else {
            $('.dialog-mask').fadeIn(100);
            $('.contract-dialog').slideDown(200);
        }
    })
    $('.dialog-title .close').click(function () {
        $('.dialog-mask').fadeOut(100);
        $('.contract-dialog').slideUp(200);
    });
    //添加收藏夹
    //EventBroker.subscribe("ajaxcart.item.added", function (msg, data) {

    //    _toastr("成功添加到收藏夹!", "top-right", "success", false);
    //});
});

 
//猜你喜欢换一组
var page = 1;
function Guess_like() {
    var goods_cookie = $("#cookie").val();

    if (page == 1) {
        page = 2;
    } else {
        page++;
    }
    $.ajax({
        type: 'POST',
        async: false,
        url: 'http://www.goodyao.com/Detailpage/Guess_like/',
        data: { goods_cookie: goods_cookie, page: page },
        success: function (data) {

            var obj = JSON.parse(data);
            if (obj.length > 0) {
                $(".guesslist ul").html("");
                $.each(obj, function (n, value) {
                    var producter = "暂无生产厂家信息";
                    if (value.producter) {
                        producter = value.producter;
                    }
                    $(".guesslist ul").append("<li><div class='guess_img'><a href='http://www.goodyao.com/detail/" + value.shop_id + "_" + value.Id + ".html' title='" + value.name + "'><img src='" + value.pic + "' alt='" + value.name + "'></a><div class='guess_title'><label>" + value.name + "</label></div></div><div class='guess_dec'><span>规格：<label> " + value.specifications + "</label></span><span>生产厂商：<label> " + producter + "</label></span></div></li>");
                })
            } else {
                alert("暂无你喜欢的药品！");
            }
        }
    });
}

//关注商家
function attentionthis(user_id) {
    var user_id = user_id;
    //请求后台处理关注商家
    $.ajax({
        type: 'POST',
        async: false,
        url: 'http://www.goodyao.com/Detailpage/addattention/',
        data: { user_id: user_id },
        success: function (data) {
            var obj = JSON.parse(data);
            if ((obj.status).toString() === "true") {
                if (obj.errorcode == '0') {
                    alert("关注商家成功");
                }
            } else {
                if (obj.errorcode == '1') {
                    alert("传入字段不能为空");
                }
                if (obj.errorcode == '2') {
                    alert("不能关注自己");
                }
                if (obj.errorcode == '3') {
                    alert("该被关注者ID不存在");
                }
                if (obj.errorcode == '4') {
                    alert("您已关注了该用户");
                }
                if (obj.errorcode == '200') {
                    alert("用户登录验证失败，请重新登录");
                    window.location.href = "http://www.goodyao.com/login";
                }
                if (obj.errorcode == '-1') {
                    alert("你还未登陆，暂不能关注商家");
                    window.location.href = "http://www.goodyao.com/login";
                }
                if (obj.errorcode == '-2') {
                    alert("你是广告商身份，没有关注商家功能");
                }
            }
        }
    });
}

function getLocalTime(nS) {
    return new Date(parseInt(nS) * 1000).toLocaleDateString().replace(/:\d{1,2}$/, ' ');
}
function getpage(page, _this) {
    var goods_id = $.trim($("#good_id").val());;
    $.ajax({
        type: 'POST',
        async: false,
        url: 'http://www.goodyao.com/Detailpage/book_next/',
        data: { goods_id: goods_id, page: page },
        success: function (data) {
            var time_date;
            var obj = JSON.parse(data);
            if (obj.length > 0) {
                $(".questionlist").html("");
                $(".list_page a").removeClass("btnfoucus");
                var page_a = "";

                if (Math.ceil(page - 4) > 0) {

                    for (var i = Math.ceil(page - 4) ; i <= Math.ceil(page + 4) ; i++) {
                        if (i > Math.ceil(obj.length / 5)) {
                            break;
                        }
                        if (i == page) {
                            page_a += "<a href='javascript:;' class='btnnum btnfoucus'>" + i + "<a/>";
                        } else {
                            page_a += "<a href='javascript:;' onclick='getpage(" + i + ",this)' class='btnnum'>" + i + "<a/>";
                        }
                    }
                } else {

                    for (var i = 1; i <= 8; i++) {

                        if (i <= Math.ceil(obj.length / 5) + 1) {

                            if (i == page) {
                                page_a += "<a href='javascript:;' class='btnnum btnfoucus'>" + i + "<a/>";
                            } else {
                                page_a += "<a href='javascript:;' onclick='getpage(" + i + ",this)' class='btnnum'>" + i + "<a/>";
                            }
                        }
                    }
                }

                $(".list_page").html(page_a);
                $.each(obj, function (n, value) {
                    time_date = getLocalTime(value.create_time);

                    $(".questionlist").append("<li><div class='customerinfo'><label>会员：</label><a href='javascript:;'>yzj01</a><span>" + time_date + "</span></div><div class='content'><p>" + value.message + "</p></div></li>");
                })
            } else {
                alert("当页暂无数据！");
            }
        }
    });
}
