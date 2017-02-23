﻿
window.onbeforeunload = function (e) {
    
    if ($("#myTab>li[tagid]").length > 0) {
        return "存在未关闭的选项卡，离开此页可能会导致未保存数据丢失，您确定离开此页吗？"
    }
};
$(function () {
    InitLeftMenu();
    InitTab();
    ace.settings.navbar_fixed(null, true);
    ace.settings.sidebar_fixed(null, true);
    $(window).triggerHandler("resize.navbar");

    $("body").click(function () {
        $("div.RMclickmenu").css({
            display: "none"
        })
    });
    $("div.RMclickmenu").unbind("mouseleave").bind("mouseleave", function (e) {
        $("div.RMclickmenu").css({
            display: "none"
        })
    });

    resize_sidebar();
    resizetabs();
    resize_dhbar();
});
 

function InitLeftMenu() {
    SetMenusNew();
    ShowFastMenu();
}
function SetMenusNew() {
    //$.each(menus, function (e, t) {
    //    var a = $(getMenuBtn(t));
    //    a.appendTo("#menubar")
    //});
    //$("#sidebar li").addClass("hover");
    $("li.menuclick>a", "#menubar").on("click.menuclick touchend.menuclick", function () {
        $(".hover-show.hover-shown", "#menubar").removeClass("hover-show hover-shown");
        var e = $(this);
        if (!e.attr("url")) {
            return
        }
        var t = "/" + e.attr("url");
        var a = e.attr("objid");
        var n = e.text();
        var i = e.attr("icon");
        if (i == "null") {
            i = ""
        }
        var r = e.attr("tagid");
        hidemenu();
        setTimeout(addTab(r, n, t, i, a), 100)
    });
    setTimeout(function () {

    }, 1e3)
}
var getMenuBtn = function (e) {
    var t = "";
    var a = "";
    if (e.treedata != null && e.treedata.length > 0) {
        t = "dropdown-toggle";
        a = '<b class="arrow fa fa-angle-right"></b>'
    }
    var n = '<li name="PMenu"><a name="item" href="###" class="' + t + '" tagid="' + e.id + '"> <i class="menu-icon fa ' + e.iconCls + '"></i><span class="menu-text">' + e.title + " </span>" + a + '</a><b class="arrow"></b>';
    if (e.treedata) {
        n = n + '	<ul class="submenu">';
        $.each(e.treedata, function (e, t) {
            n = n + getMenuStr(t)
        });
        n = n + "</ul>"
    }
    n = n + "</li>";
    return n
};
var getMenuStr = function (e) {
    var t = "";
    var a = "";
    var n = "";
    var i = "";
    if (e.children != null && e.children.length > 0) {
        t = "dropdown-toggle";
        a = '<b class="arrow fa fa-angle-right"></b>'
    } else {
        n = 'url="' + e.attributes.url + '" objid ="' + e.attributes.objid + '" icon="' + e.iconCls + '"  ';
        if (e.attributes.url) {
            i = "menuclick"
        }
    }
    var r = '<li  class="' + i + '"><a name="item" href="###" class="' + t + '" ' + n + ' tagid="' + e.id + '"> <i class="menu-icon fa ' + e.iconCls + '"></i>' + e.text + " " + a + '</a><b class="arrow"></b>';
    if (e.children) {
        r = r + '	<ul class="submenu">';
        $.each(e.children, function (e, t) {
            r = r + getMenuStr(t)
        });
        r = r + "</ul>"
    }
    r = r + "</li>";
    return r
};
var activeMenu = function (e) {
    $("li[name='PMenu']").removeClass("active");
    $(".menuclick").removeClass("active");
    $('a[tagid="' + e + '"][name="item"]', "#menubar").closest("li[name='PMenu']").addClass("active");
    $('a[tagid="' + e + '"][name="item"]').parent("li").addClass("active");
};
var hidemenu = function () {
    var e = $("button.menu-toggler");
    var t = $(e.attr("data-target"));
    if (t.length == 0) return;
    if (!e.hasClass("display")) {
        return
    }
    t.toggleClass("display");
    e.toggleClass("display");
    var a = ace.click_event + ".ace.autohide";
    var n = t.attr("data-auto-hide") === "true";
    if (e.hasClass("display")) {
        if (n) {
            $(document).on(a, function (n) {
                if (t.get(0) == n.target || $.contains(t.get(0), n.target)) {
                    n.stopPropagation();
                    return
                }
                t.removeClass("display");
                e.removeClass("display");
                $(document).off(a)
            })
        }
        if (t.attr("data-sidebar-scroll") == "true") t.ace_sidebar_scroll("reset")
    } else { }
};
var getMenuByID = function (e) { };


function InitTab() {
    $("a", "#myTab>li:first").tab("show");
    $("#myTab").tabdrop({
        text: '更多'
    }).on("shown.bs.tab", function (e) {
        var t = $($(e.target).attr("href"));
        $("#home-help").attr("href", "/Home/goToHelp?id=" + $(e.target).attr("tagid"));
        $("iframe", t).each(function () {
            if (this.contentWindow && this.contentWindow.new_resize) {
                this.contentWindow.new_resize()
            }
            if (this.contentWindow && this.contentWindow.smallscreen_reset) {
                this.contentWindow.smallscreen_reset()
            }
        });
        resize_dhbar()
    })
}
var resizeTab = function () { };

function RefThisTab() {
    $("body").showLoading("正在加载页面...");
    var e = $($("#myTab").find("li.active"));
    var t = $("#ifarme-" + e.attr("tagid"));
    t.attr("src", t.attr("src"))
}
function CloseThisTab() {
    var e = $($("#myTab").find("li.active"));
    closeTab(e)
}
function CloseAllTab(e) {
    var t = $("#myTab").find("li");
    $.each(t, function (t, a) {
        if (!$(a).hasClass("active")) {
            if ($(a).attr("tagid") != null) {
                closeTab($(a), true)
            }
        } else {
            if (e != null) {
                closeTab($(a), true)
            }
        }
    })
}
function addTabByID(e) {
    var t = $('a[name="item"][tagid="' + e + '"]');
    if (t.length <= 0) {
        return
    }
    var a = t.text();
    var n = "/" + t.attr("url") + "?FastOpen=true";
    var i = t.attr("icon");
    var r = t.attr("objid");
    addTab(e, a, n, i, r)
}
function addTabByCountMark(e, t) {
    if (!e) return;
    var a = $('a[name="item"][tagid="' + e + '"]');
    if (a.length <= 0) {
        return
    }
    var n = a.text();
    var i = "/" + a.attr("url") + "?Type=" + t;
    var r = a.attr("icon");
    var o = a.attr("objid");
    var s = $('[tagid="' + e + '"]', "#myTab");
    if (s.length > 0) {
        $("a", s).tab("show");
        activeMenu(e);
        $("#ifarme-" + e).attr("src", i);
        return
    }
    addTab(e, n, i, r, o)
}
function addTabByIDAndParam(e, t) {
    var a = $('a[name="item"][tagid="' + e + '"]');
    if (a.length <= 0) {
        return
    }
    var n = a.text();
    var i = a.attr("icon");
    var r = "/" + a.attr("url") + t;
    var o = a.attr("objid");
    addTab(e, n, r, i, o)
}
function addTab(e, t, a, n, i) {
    var r = $('[tagid="' + e + '"]', "#myTab");
    if (r.length > 0) {
        $("a", r).tab("show");
        activeMenu(e);
        if (e == "printid") {
            var o = $("#ifarme-printid");
            o.attr("src", a)
        }
        return
    }
    $("body").showLoading("正在加载页面...");
    var s = '<li tagid="' + e + '"><a data-toggle="tab" href="#tab-' + e + '" tagid="' + e + '"><i class="green ace-icon fa ' + n + '"></i>' + t + " </a></li>";
    r = $(s).bind("contextmenu", function (e) {
        var t = !e ? window.event : e;
        e.preventDefault();
        return false
    }).mouseup(function (e) {
        if (e.which == 3) {
            e.preventDefault();
            $("a", $(this)).tab("show");
            var t = $("div.RMclickmenu");
            var a = e.clientX;
            var n = e.clientY;
            if (n + t.height() > $(document).height()) {
                n = e.clientY - t.height()
            }
            t.css({
                left: a,
                top: n,
                display: "block"
            })
        }
    });
    $("#myTab").append(r);
    if (e != "menuFirstPage") {
        var l = '<i name="tabclose" class="ace-icon fa fa-times fa-lg"></i>';
        l = $(l).appendTo($("a", r));
        l.bind("mouseenter", function () {
            $(this).addClass("red")
        }).bind("mouseleave", function () {
            $(this).removeClass("red")
        }).bind("click", function () {
            closeTabByA(this);
            return false
        })
    }
    var c = '<div id="tab-' + e + '" class="tab-pane fade" ><iframe id="ifarme-' + e + '" objid = "' + i + '"  icon="' + n + '" title="' + t + '" class="iframe_tabpage"  frameborder="0"  src="' + a + '" style="width:100%;" helpurl></iframe></div>';

    var d = $(c).appendTo("#myTabContent");
    $("a", r).tab("show");
    $("iframe.iframe_tabpage").off("load.page").on("load.page", function () {
        setTimeout(function () {
            $("body").hideLoading()
        }, 100)
    });
    $("#myTab").tabdrop("layout");
    activeMenu(e);
    resize_dhbar();
    resizetabs()
}
function closeTabByA(e) {
    var t = $(e).closest("li");
    closeTab(t)
}
function closeTab(e, t) {
    var a = e.attr("tagid");
    var n = $("#tab-" + a);
    var i = n.find("iframe");
    if (i && i.length > 0) {
        i[0].contentWindow.document.write("");
        if (i[0].contentWindow.document.clear) {
            i[0].contentWindow.document.clear()
        }
        i[0].contentWindow.close();
        i.find("*").remove();
        i.remove();
        if ($.browser.msie) {
            CollectGarbage()
        }
    }
    n.remove();
    if (e.hasClass("active")) {
        if (e.prev("li").length > 0) {
            $("a", e.prev("li")).tab("show")
        } else if (e.next("li").length > 0) {
            $("a", e.next("li")).tab("show")
        }
        activeMenu(a)
    }
    e.remove();
    if (t) { } else {
        $("#myTab").tabdrop("layout")
    }
}

var showFirstPage = function () {
    var e = $("#tab-FirstPage");
    $("a", e).tab("show")
};
var returnFirstPage = function () {
    showFirstPage();
    resize_dhbar()
};

var exresize = function () {
    resize_dhbar();
    resize_sidebar();
    resizetabs();

};
var resizetabs = function () {
    var e = $(window).width();
    var t = $(window).height();
    var a = $("#myTabContent").offset();
    $("#myTabContent").find("div.tab-pane.fade.active").height(t - a.top - 14).width(e - a.left);
    $("#myTabContent iframe.iframe_tabpage").height(t - a.top - 14)
};
var resize_sidebar = function () {
    var e = $(window).height();
    if (!$.browser.useapp) {
        if ($("#sidebar").height() + $("#sidebar").offset().top > e) {
            $("#sidebar").addClass("mx-sidebar");
            $("#sidebar").removeClass("compact")
        }
    } else {
        $("#sidebar").removeClass("compact");
        $("#sidebar li.hover").removeClass("hover")
    }
};
var resize_dhbar = function () {
    var e = $($("#myTab").find("li.active:not(.tabdrop)"));
    var t = e.attr("tagid");
    var a = false;
    if (t && t != "menuFirstPage") {
        a = true
    } else {
        a = false
    }
    if ($.browser.useapp) {
        $("body").addClass("mx-xs")
    } else {
        $("body").removeClass("mx-xs")
    }
    if ($.browser.useapp && a) {
        $("#navbar").addClass("hidden");
        $("#myTab").closest(".widget-header").addClass("hidden");
        hidemenu()
    } else {
        $("#navbar").removeClass("hidden");
        $("#myTab").closest(".widget-header").removeClass("hidden")
    }
};


var lockscreen_waitTime = 0;
var timeOutTirm = null;
var parment;

function setOutTim() {
    if (timeOutTirm == null) {
        timeOutTirm = window.setInterval(addTime, 1e3 * 60)
    }
}
function addTime() {
    if (lockscreen_waitTime >= 10) {
        clearTime()
    } else {
        lockscreen_waitTime++
    }
}
function clearTime() {
    if (timeOutTirm !== null) {
        window.clearInterval(timeOutTirm);
        setTimeout(function () {
            lockscreen_waitTime = 0;
            
        }, 10)
    }
}
 

function ShowFastMenu() {
    var a = $("#fuelux-wizard").find("a[name='item']");
    var span = $(".menuleft").find("span[name='item']");
    $.each(a, function (index, ae) {
        bingAddTab(ae)
    })
    $.each(span, function (index, ae) {
        bingAddTab(ae)
    })
}
function bingAddTab(obj)
{
    var self = $(obj);
    var n = jQuery.trim(self.text());
    var i = self.attr("url");
    var r = "fa-" + self.attr("icon");
    var o = self.attr("objid");
    var e = self.attr("tagid");
    var s = $('[tagid="' + e + '"]', "#myTab");
    if (s.length > 0) {
        $("a", s).tab("show");
        activeMenu(e);
        $("#ifarme-" + e).attr("src", i);
        return
    }

    self.click(function () {
        addTab(e, n, i, r, o)
    })
}