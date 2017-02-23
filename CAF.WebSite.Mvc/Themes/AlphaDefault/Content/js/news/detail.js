var initTop = 0, scrollTop = 0;
$(window).scroll(function (e) {
    scrollTop = $(this).scrollTop();
    if (initTop < scrollTop) {   //滚动条向下滚
        $("#navWrap").css({ "position": "static" });
    } else {   //滚动条向上滚
        $("#navWrap").css({ "position": "fixed" });
    }
    setTimeout(function () {
        initTop = scrollTop;
    }, 0)
})



$(function () {
    var sw = 0;
    $(".picBox div span").hover(function () {
        sw = $(this).index();
        myShow(sw);
    });

    function myShow(i) {
        $(".picBox div span").eq(i).addClass("cur").siblings("span").removeClass("cur");
        //		$(".picBox ul li").eq(i).stop(true,true).fadeIn().siblings("li").fadeOut();
        $(".picBox ul").stop(true, true).animate({ 'margin-left': -$(".picBox ul li").width() * i }, 500);
    }

    //滑入停止动画，滑出开始
    $(".picBox").hover(function () {
        if (myTime) {
            clearInterval(myTime);
        }
    }, function () {
        myTime = setInterval(function () {
            myShow(sw)
            sw++;
            if (sw == $(".picBox ul li").length) { sw = 0; }
        }, 3000);
    });
    //自动开始
    var myTime = setInterval(function () {
        myShow(sw)
        sw++;
        if (sw == $(".picBox ul li").length) { sw = 0; }
    }, 3000);
});


$(function () {
    $("#selected").click(function () {
        $("#dropdown").toggle();
        $(this).toggleClass("act2");
    });
    $("#dropdown li").click(function () {
        $("#selected").toggleClass("act2");
        $("#selected").text($(this).text());
        $("#selected").attr("name", $(this).attr("name"));
        $(this).parent().hide();
    });

    $(document).on({
        "click": function (e) {
            var t = e.target || e.srcElement;
            if (t.id && t.id == "dropdown" || t.id == "selected") {
                return false;
            } else {
                $("#dropdown").hide();
                $("#selected").removeClass('act2');
            }
        }
    });

});
