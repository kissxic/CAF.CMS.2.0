$(function () {
    //左侧导航点击下拉
    $('.itemChooseBox').find('.icon_btn').each(function () {
        var $this = $(this),
            $ul = $this.parents('h3').next('ul.list_ul');
        $this.click(function () {
            if ($this.hasClass('open') && $ul.is(':visible')) {//下拉展开状态
                $this.removeClass('open');
                $ul.slideUp();
            } else {//未下拉展开时
                $('.itemChooseBox').find('.icon_btn').removeClass('open');
                $('.itemChooseBox').find('.list_ul').slideUp();
                $this.addClass('open');
                $ul.slideDown();
            }
        })
    })

});
//分类选择下拉菜单
$(document).ready(function () {
    $('.nav-item').each(function (index) {
        $(this).mouseover(function () {
            $(this).css('z-index', '99');
            $(this).find('.category-menu-con').addClass('category-menu-on');
            $(this).find('.category-menu-main').css('display', 'block');
        }).mouseout(function () {
            $(this).css('z-index', '1');
            $(this).find('.category-menu-con').removeClass('category-menu-on');
            $(this).find('.category-menu-main').css('display', 'none');
        })
    });

});

$(document).ready(function () {
    $(document).on('mouseover', '.yet-box', function () {
        $(this).addClass('yet-on');
        $(this).find('em').click(function () {
            //$(this).parents('li').css('display' , 'none');
            $(this).parents('li').remove();
        });
    }).on('mouseout', '.yet-box', function () {
        $(this).removeClass('yet-on');
    });
    $(document).on('mouseover', '.lastdd p', function () {
        $(this).addClass('last-on');
    }).on('mouseout', '.lastdd p', function () {
        $(this).removeClass('last-on');
    });

    $('.undo').click(function () {  //撤销全部
        $('.yet-con li').remove();
    });
});
/*..s*/
$(document).ready(function () {
    $(document).on('click', '.more', function () {
        var self = this;
        if ($(self).find('b').hasClass("shou")) {
            $(self).nextAll().css('display', 'none');
            $('.morebg').text('更多选项')
            $('.morebg').css('paddingLeft', '30px');
            $('.more').find('b').removeClass('shou');
        }
        else {
            $(self).nextAll().css('display', 'block');
            $('.morebg').text('收起');
            $('.morebg').css('paddingLeft', '40px');
            $('.more').find('b').addClass('shou');
        }
    });
    $(document).on('mouseover', '.morep', function () {
        $(this).find('.mbg').addClass('mbghover');
        $(this).find('.mbs').removeClass('mbshover');
    }).on('mouseout', '.yet-box', function () {
        $(this).find('.mbg').removeClass('mbghover');
        $(this).find('.mbs').addClass('mbshover');
        $(this).find('.mbs').removeClass('mbghover');
    });

});
/*..e*/
$(document).ready(function () {
    $('.pro-con li').each(function () {
        $(this).mouseover(function () {
            $(this).addClass('on');
            if ($(this).find('.pro-botxt').find('p').text().length >= 17) {
                $(this).find('.pro-botxt').find('p').css('height', 'auto');
            }
            if ($(this).find('.pro-botxt').find('p').text().length >= 90) {
                $(this).find('.pro-botxt').find('p').css('height', '100px');
            }
        }).mouseout(function () {
            $(this).removeClass('on');
            $(this).find('.pro-botxt').find('p').css('height', '40px');
        })
    });
});
/*..s*/
$(document).ready(function () {
    $('.listop-left li').each(function () {
        $(this).click(function () {
            $(this).addClass('pro-list-on');
            $(this).siblings().removeClass('pro-list-on');
            $(this).siblings().find('i').removeClass('clicks');
            $(this).siblings().find('i').removeClass('wup');
            $(this).find('i').addClass('clicks');
        });
        $(this).mouseover(function () {
            $(this).find('a').addClass('a005');
        }).mouseout(function () {
            $(this).find('a').removeClass('a005');
        })
    });
    $('.price-icon').mouseover(function () {
        $(this).find('i').addClass('movbg');
    }).mouseout(function () {
        $(this).find('i').removeClass('movbg');
    })
});
/*..e*/

$(document).ready(function () {

    $(".price-icon").click(function () {
        var self = this;
        if ($(self).find('i').hasClass("wup")) {
            $(self).find('i').addClass('clicks');
            $(self).find('i').removeClass('wup');
        }
        else {
            $(self).find('i').addClass('wup');
            $(self).find('i').removeClass('clicks');
        }
    });
    $('.list-brand li').each(function (index) {
        if (index == 11) {
            $(this).addClass('man');
        }
    });
    $('.sort li').each(function (index) {
        if (index == 11) {
            $(this).addClass('man');
        }
        $('.list-brand .man').nextAll().css('display', 'none');
        $('.sort .man').nextAll().css('display', 'none');
    });
    /*..s*/


    $('.sort').next().click(function () {
        var self = this;
        if ($(self).find('i').hasClass("mbg")) {
            $('.sort .man').nextAll().css('display', 'block');
            $(this).find('em').text('收起');
            $(this).find('i').removeClass('mbg')
            $(this).find('i').addClass('mbs');
        }
        else {
            $('.sort .man').nextAll().css('display', 'none');
            $(this).find('em').text('更多');
            $(this).find('i').removeClass('mbs')
            $(this).find('i').addClass('mbg');
        }
    })
    $('.list-brand').next().on('click', function () {
        //$('.list-brand').next().click(function () {
        var self = this;
        if ($(self).find('i').hasClass("mbg")) {
            $('.list-brand .man').nextAll().css('display', 'block');
            $(self).find('em').text('收起');
            $(self).find('i').removeClass('mbg')
            $(self).find('i').addClass('mbs');
            $('.list-brand').addClass('scrollbar'); //滚动条
            if ($("#brandNameCount").val() > 36) {
                $(".list-brand").height(150);
            } else if ($("#brandNameCount").val() <= 6) {
                $(".list-brand").height(30);
            } else {
                $(".list-brand").height("auto");
            }
            $('.more').addClass('more-ie') //ie6
        }
        else {
            $('.list-brand').removeClass('scrollbar'); //滚动条
            $('.more').removeClass('more-ie') //ie6
            $('.list-brand .man').nextAll().css('display', 'none');
            $(".list-brand").height(50);
            $(self).find('em').text('更多');
            $(self).find('i').removeClass('mbs')
            $(self).find('i').addClass('mbg');
        }
    });
});
/*..e  */
//优惠套餐
$(document).ready(function () {
    $('.suit-tt li').each(function (indexs) {
        $(this).mouseover(function () {
            $('.suit-bon').removeClass('suit-bon');
            $('.suit-tt .su-ton').removeClass('su-ton');
            $('.suit-con').eq(indexs).addClass('suit-bon');
            $(this).addClass('su-ton');
        });
    });
});

$(document).ready(function () {
    if ($("#brandNameCount").val() <= 6) {
        $(".list-brand").height(30);
    }
    if ($("#branchLength").val() <= 6) {
        $(".sort").height(30);
    }
});

$(document).ready(function () {
    var pcwidth = window.screen.availWidth;//window.screen.width;
    if (pcwidth <= 1024) {
        $('.main').addClass('zhai')
    }
});

$(function () {
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
    $("img").lazyload({
        placeholder: "" + url + "/Themes/AlphaDefault/Content/images/loading.gif",
        effect: "fadeIn"
    });
    slidcontent("#websitelevel", 300, "div");
    
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
});