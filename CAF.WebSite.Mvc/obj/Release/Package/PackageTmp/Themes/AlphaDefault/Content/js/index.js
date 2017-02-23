
$(function () {
    jQuery("#slideBox01").slide({ mainCell: ".bd ul", effect: "fold", autoPlay: true, delayTime: 1000, interTime: 5000 });

    var url = "http://" + window.location.host;

    window.onscroll = function () {
        var s = $(window).scrollTop();
        if (s >= 300) {
            $(".navigationtree").css("top", "100px");
        }
        else {
            $(".navigationtree").css("top", "300px");
        }
    }

    $(".dropdownmenu").removeClass("disappear");
    $("img").lazyload({
        placeholder: "" + url + "/Themes/AlphaDefault/Content/images/loading.gif",
        effect: "fadeIn"
    });

    slidcontent("#websitelevel", 300, "div");
    slidcontent("#qrcode", 300, "div");
    slidcontent(".user_nav > ul > li", 200, ".child-nav");

  

    new Marquee(
        {
            MSClass: ["smallbanner1", "smallbanner2"],
            PrevBtnID: "banner_prev",
            NextBtnID: "banner_next",
            Direction: 2,
            Step: 0.4,
            Width: 996,
            Height: 160,
            Timer: 100,
            DelayTime: 3000,
            WaitTime: 5000,
            ScrollStep: 996,
            AutoStart: 1
        });


    floornavshow("#recom_nav li", "itemfocus", ".recomcontent_c_item", "div");


    $(".navigationtree .totop").click(function () {
        $('body,html').animate({ scrollTop: 0 }, 1000);
        return false;
    });

    var numtitle = $(".floor").size();

    var arr = new Array();
    for (var i = 0; i < numtitle; i++) {
        var number = i + 1;
        var name = $(".floor").eq("" + i).find(".lefttitle span").text();
        $(".floor:eq(" + i + ")").attr("id", "" + i);
        if (i == 0) {
            $(".flooguide .floorlist").append("<li class='floorcur'><a href='#" + i + "'><span class='floornum'>" + number + "F" + "</span><span class='floorname disappear'>" + name + "</span></a></li>");
        } else {
            $(".flooguide .floorlist").append("<li><a href='#" + i + "'><span class='floornum'>" + number + "F" + "</span><span class='floorname disappear'>" + name + "</span></a></li>");
        }
        arr[i] = $(".floor").eq("" + i).position().top;
    }


    $(".floorlist li").click(function () {
        $(".floorlist li").removeClass("floorcur");
        $(this).addClass("floorcur");
    });
    $(".floorlist li").hover(function () {
        $(this).find("span").removeClass("disappear");
        $(this).find(".floornum").addClass("disappear");
    }, function () {
        $(this).find("span").removeClass("disappear");
        $(this).find(".floorname").addClass("disappear");
    });


    $(window).scroll(function () {
        var scrollbar = $(document).scrollTop();
        for (var i = 0; i < arr.length ; i++) {
            if (scrollbar >= arr[i] && scrollbar < arr[i + 1] - 100) {

                $(".flooguide li").removeClass("floorcur");
                $(".flooguide li").eq("" + i).addClass("floorcur");
            }
            else if (scrollbar >= arr[arr.length - 1]) {
                $(".flooguide li").removeClass("floorcur");
                $(".flooguide li").eq("" + i).addClass("floorcur");
            }
        }
    });


    jQuery(".txtScroll-top").slide({ mainCell: ".bd ul", autoPage: true, effect: "top", autoPlay: true, vis: 3 });
});