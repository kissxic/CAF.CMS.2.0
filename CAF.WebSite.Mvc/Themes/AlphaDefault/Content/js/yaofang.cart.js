$(document).ready(function () {
    /* 购物动画 */
    var obj = $("#paycart");
    var aa = $(".pro-con li").eq(0).children('div').find('.imgbig').html();
    var objX;
    var objY;
    function start(n) {
        var box1 = $(".pro-con li").eq(n).children('div').find('.pro_box');
        objX = obj.offset().left - box1.offset().left;
        objY = obj.offset().top - box1.offset().top;
        var $appmt = $("<span class=minbox>" + $(".pro-con li").eq(n).children('div').find('.imgbig').html() + "</span>");
        box1.append($appmt);
        $(".pro-con li").eq(n).siblings().children('div').find(".pro_box .minbox").hide();
        var imgobj = $(".pro-con li").eq(n).children('div').find(".pro_box .minbox");
        imgobj.animate({
            top: objY + "px",
            left: objX + "px"
        }, 800, clear);
        return true;
    }
    function clear() {
        var numbbx = $(".pro_box span");
        var numbbx2 = $(".pro_box2 span");
        var munb = $("#paycart").html();
        if (numbbx.length > 0) {
            numbbx.eq(0).remove();
            numbbx.css("display", "none");
        }
        if (numbbx2.length > 0) {
            numbbx2.eq(0).remove();
            numbbx2.css("display", "none");
        }
        $("#paycart").html(munb);
    }

    $(".addCart").click(function (event) {
        var index = $(this).parents('li').index();
        start(index);
    });
});
function goods_buy(ProductCode, ProductStatusType, PrescriptionType, OurPrice) {
    var productDate = { 'pid': ProductCode, 'ProductStatusType': ProductStatusType, 'PrescriptionType': PrescriptionType, 'OurPrice': OurPrice };
    AddCart(productDate, '1');
    return false;
}
//加入购物车
function AddCart(jsonDate, pcount) {
    if (InitCart(jsonDate) == false) {
        return false;
    }
    $.ajax({
        type: "GET",
        url: Domain + "/purchase/InitCart",
        data: { productCode: jsonDate.pid, pcount: pcount, sourceUrl: hostUrl, NoCache: Math.random() },
        dataType: 'jsonp',
        cache: "false",
        success: function (result) {
            if (result != "") {
              
                if (result.pcount != undefined) {
                    if (!IsPcount(result.pcount, result.IsControl, result.ControlNumber)) {
                        return false;
                    }
                }
                if (jsonDate.AddType == 2) {
                    $("#paycart").html(result[0][0].TotalNum);
                    $("#topCount").html(result[0][0].TotalNum);
                    mesgbox('去结算', "商品已成功加入购物车！", function () { window.location.href = "http://cart.jianke.com/purchase/shoppingcart?backurl=" + escape(document.URL); });

                }
                else {
                    $("#paycart").html(result[0][0].TotalNum);
                    $("#topCount").html(result[0][0].TotalNum);
                }

            }

        }
    });
}

function detailCar() {
    $.ajax({
        type: 'get',
        url: 'http://www.jianke.com/topuser/strShopCar',
        data: { NoCache: Math.random() },
        dataType: 'jsonp',
        error: function () {
        },
        success: function (result) {
            var htmlShopCar = "";
            for (i in result) {
                if (i > 1) {
                    htmlShopCar += "<li id=\"mtr_" + result[i].PorductCode + "\">";
                    htmlShopCar += "<span class=\"pic_img_mi\">";
                    if (result[i].Mark == 7 || result[i].Mark == 5 || result[i].Mark == 6)
                    { htmlShopCar += "<a><img src=\"http://img1.jianke.net/" + result[i].PorductImage + "\"width=\"50\" height=\"50\" /></a>"; }
                    else {
                        htmlShopCar += "<a href=\"/product/" + result[i].PorductCode + ".html\" target=\"_blank\"><img src=\"http://img1.jianke.net/" + result[i].PorductImage + "\"width=\"50\" height=\"50\" /></a>";
                    }
                    htmlShopCar += "</span>";
                    htmlShopCar += "<div class=\"cart_text\">";
                    htmlShopCar += "<h4>" + result[i].PorductName + "</h4>";
                    htmlShopCar += "<p><em>" + result[i].Packing + "</em><b>￥" + result[i].Preferential + "</b></p>";
                    htmlShopCar += " </div>";
                    htmlShopCar += "<em>" + result[i].Quantity + "</em>";
                    htmlShopCar += " <i>";
                    htmlShopCar += "<a id=\"btn_" + result[i].PorductCode + "\" onclick=\"RomoveProduct(" + result[i].PorductCode + "," + result[i].Mark + "," + result[i].ShopCartType + ");setTimeout(EmptyCart, 500);\">x</a>";
                    htmlShopCar += "</i>";
                    htmlShopCar += "</li>";
                }
            }
            $("#cartli").html(htmlShopCar);
        }
    });
};


 