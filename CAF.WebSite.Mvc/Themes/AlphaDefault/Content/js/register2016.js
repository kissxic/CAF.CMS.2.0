
$(function () {
    $('.empty').val('');
    document.onkeydown = function (ev) {
        var oEvent = ev || event;

        if (oEvent.keyCode == 116) {
            $('input[type="text"],input[type="password"]').val('').blur();
        }
    };

    $('.reg_cont li input').each(function (index, element) {

        $(this).focus(function (e) {
            $(this).parents('li').removeClass('nofocued');
            $(this).siblings('label').css('visibility', 'hidden');
        });
        $(this).blur(function (e) {
            if ($(this).val() == '') {
                $(this).parents('li').addClass('nofocued');
                $(this).siblings('label').css('visibility', 'visible')
            }
        });
    });

});


var getreturn = true;
/**************检测注册页面邮箱手机号码的方法*********************/
function CheckloginPhone() {
    var strPhone = trim($("#user_phone").val());
    if (strPhone == "") {
        $(".no_phone").css({
            "visibility": "visible"
        });
        $(".wrong_phone,.ishave_phone").css({
            "visibility": "hidden"
        });

        getreturn = false;
        return getreturn;
    }
    else if (strPhone != null || strPhone != "") {
        var ph = /^1[3|4|5|7|8][0-9]\d{8}$/;//验证电话号码的正则表达式
        if (ph.test(strPhone) == true) {
            $(".r_user_phone").css({
                "display": "none"
            });
            $(".no_phone,.wrong_phone").css({
                "visibility": "hidden"
            });

            //$("#btn_regEmail").css("display", "none"); //隐藏登录控件
            $("#btn_regEmail").val("检测中..."); //隐藏登录控件
            $(".ishave_phone em").html("检测中，请稍后.....");
            $(".ishave_phone").css({
                "visibility": "visible"
            });
            // $(".r_user_phone").css({
            //     "display": "block",
            //     "background-position": "-59px -144px"
            // });
            $.ajax({
                type: "POST",
                url: "/Member/CheckUserNameAvailability",
                async: false,
                data: { username: $("#user_phone").val() },
                success: function (returnDate) {

                    // $("#btn_regModile").css("display", "inline");
                    // $("#spanRegModile").css("display", "none");
                    if (returnDate.Available) {
                        // $("#HidUserPhone").val($("#user_phone").val());
                        // EmailState = true;
                        $(".ishave_phone").css({
                            "visibility": "hidden"
                        });
                        $(".r_user_phone").css({
                            "display": "block",
                            "background-position": "-59px -144px"
                        });
                        //("#btn_regEmail").css("display", "inline"); //隐藏登录控件
                        // $("#spanLogin").css("display", "none"); //显示正在验证
                        $("#btn_regEmail").val("立即注册"); //隐藏登录控件
                        getreturn = true;
                    }
                    else {
                        $(".ishave_phone em").html("<i></i>对不起！该手机号已被注册");
                        $(".ishave_phone").css({
                            "visibility": "visible"
                        });
                        $(".r_user_phone").css({
                            "display": "block"
                        });
                        //$("#btn_regEmail").css("display", "inline"); //隐藏登录控件
                        // $("#spanLogin").css("display", "none"); //显示正在验证
                        $("#btn_regEmail").val("立即注册"); //隐藏登录控件
                        EmailState = false;
                        getreturn = false;
                    }
                }
            });
            return getreturn;
        } else if (ph.test(strPhone) != true) {
            $(".wrong_phone").css({
                "visibility": "visible"
            });
            $(".ishave_phone").css({
                "visibility": "hidden"
            });
        }
        $(".no_phone").css({
            "visibility": "hidden"
        });
        getreturn = false;
        return getreturn;
    }
}
function CheckloginEmail() {
    var strEmail = trim($("#user_email").val());
    if (strEmail == "") {
        $(".no_email").css({
            "visibility": "visible"
        });
        $(".wrong_email,.ishave_email").css({
            "visibility": "hidden"
        });

        getreturn = false;
        return getreturn;
    }
    else if (strEmail != null || strEmail != "") {
        var reg = /^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$/;//验证邮箱的正则表达式
        if (reg.test(strEmail) == true) {
            $(".r_user_email").css({
                "display": "none"
            });
            $(".no_email,.wrong_email").css({
                "visibility": "hidden"
            });

            //$("#btn_regEmail").css("display", "none"); //隐藏登录控件
            $("#btn_regEmail").val("检测中..."); //隐藏登录控件
            $(".ishave_email em").html("检测中，请稍后.....");
            $(".ishave_email").css({
                "visibility": "visible"
            });
            // $(".r_user_email").css({
            //     "display": "block",
            //     "background-position": "-59px -144px"
            // });
            $.ajax({
                type: "POST",
                url: "/Member/CheckUserNameAvailability",
                async: false,
                data: { username: $("#user_email").val() },
                success: function (returnDate) {
                    // $("#btn_regModile").css("display", "inline");
                    // $("#spanRegModile").css("display", "none");
                    if (returnDate.Available) {
                        // $("#HidUserEmail").val($("#user_email").val());
                        // EmailState = true;
                        $(".ishave_email").css({
                            "visibility": "hidden"
                        });
                        $(".r_user_email").css({
                            "display": "block",
                            "background-position": "-59px -144px"
                        });
                        //("#btn_regEmail").css("display", "inline"); //隐藏登录控件
                        // $("#spanLogin").css("display", "none"); //显示正在验证
                        $("#btn_regEmail").val("立即注册"); //隐藏登录控件
                        getreturn = true;
                    }
                    else {
                        $(".ishave_email em").html("<i></i>对不起！该邮箱已被注册！");
                        $(".ishave_email").css({
                            "visibility": "visible"
                        });

                        //$("#btn_regEmail").css("display", "inline"); //隐藏登录控件
                        // $("#spanLogin").css("display", "none"); //显示正在验证
                        $("#btn_regEmail").val("立即注册"); //隐藏登录控件
                        EmailState = false;
                        getreturn = false;
                    }
                }
            });
            return getreturn;
        } else if (reg.test(strEmail) != true) {
            $(".wrong_email").css({
                "visibility": "visible"
            });
            $(".ishave_email").css({
                "visibility": "hidden"
            });
        }
        $(".no_email").css({
            "visibility": "hidden"
        });
        getreturn = false;
        return getreturn;
    }
}

function yzHide() {
    $(".wrong,.r_pwd,.pwdasg_id").css({
        "display": "none"
    });
    $(".no_id,.wrong_id,.ishave_id,.wrong_pw1,.wrong_pw2,.no_pw1,.no_pw2").css({
        "visibility": "hidden"
    });
    $("input[type=reset]").trigger("click");
    $('.pwd_l1,.pwd_l2,.user_ida').css('visibility', 'visible');
    LoadVerificationCode();
    $('#user_yzm').val("");
    flag = false;
    return flag;
}


$(function () {
    $('.user_id').blur(function () {
        CheckloginPhone();
        CheckloginEmail();
    });

    /*手机注册、邮箱注册切换*/
    $(".abc").click(function () {
        $(".phone,.dxyz").hide();
        $(".email,.yxyz").show();
        $("#user_phone").attr("name", "");
        $("#user_email").attr("name", "userName");
        $("#user_dx").attr("name", "");
        $("#user_dx1").attr("name", "codes");
        LoadVerificationCode();
    });
    $(".def").click(function () {
        $(".email,.yxyz").hide();
        $(".phone,.dxyz").show();
        $("#user_phone").attr("name", "userName");
        $("#user_email").attr("name", "");
        $("#user_dx1").attr("name", "");
        $("#user_dx").attr("name", "codes");
        LoadVerificationCode();
    });


    /*获取焦点*/
    /*id*/
    $('#user_phone').focus(function () {
        $(".r_user_phone").css({
            "display": "none"
        });
        $(".no_phone,.wrong_phone").css({
            "visibility": "hidden"
        });
    });
    $('#user_email').focus(function () {
        $(".r_user_email").css({
            "display": "none"
        });
        $(".no_email,.wrong_email").css({
            "visibility": "hidden"
        });
    });
    /*mm*/
    $("#pwd_1").focus(function () {
        $('.wrong_pw1,.no_pw1,.pwd_l1').css({
            "visibility": "hidden"
        });
        $('.r_pwd').css({
            "display": "none"
        });
    });
    $("#pwd_2").focus(function () {
        $('.pwdasg_id').css('display', 'none');
        $('.wrong_pw2,.no_pw2,.pwd_l2').css('visibility', 'hidden')
    });
    // $("#pwd_1").blur(function(){
    // 	Checkpwd();
    // 	Checkagianpwd()
    // })
    /*yzm*/
    $('#user_yzm').focus(function () {
        $(".no_yzm,.wrong_yzm").css({
            "visibility": "hidden"
        });
        $(".user_yzm").css({
            "display": "none"
        });
    })
    $(".cxdd a").bind("click", function () {
        LoadVerificationCode();
    });


    /*yzm_djs*/
    $('#user_dx').focus(function () {
        $(".dx_yzm,.no_dxyzm").css({
            "visibility": "hidden"
        });
        $('.user_dx').css({
            'display': "none"
        });
    })
    $('#user_dx').blur(function () {
        Dxyzm();
    });
    $('#user_dx1').focus(function () {
        $(".yx_yzm,.no_yxyzm").css({
            "visibility": "hidden"
        });
        $('.user_dx1').css({
            'display': "none"
        });
    })
    $('#user_dx1').blur(function () {
        Yxyzm();
    });


    $('#btn').click(function () {
        if (flag == true && getreturn == true) {
            $('#user_dx').focus();
            showtime(60);
        } else {
            if (flag == false && getreturn == true) {
                $('#user_yzm').focus();
                if (CheckloginPhone()) Yzm();
            } else {
                $('.user_id').focus();
                if (CheckloginPhone()) Yzm();
            }
        }
        return false

    });
    $('#btn1').click(function () {
        if (flag == true && getreturn == true) {
            $('#user_dx1').focus();
            showtime1(60);
        } else {
            if (flag == false && getreturn == true) {
                $('#user_yzm').focus();
                if (CheckloginEmail()) Yzm();
            } else {
                $('.user_id').focus();
                if (CheckloginEmail()) Yzm();
            }
        }
        return false
    });

    $('#checkCodeImg').click(function () {
        LoadVerificationCode();
    });
})
/*短信验证等待*/
function showtime(t) {
    document.getElementById('btn').setAttribute("disabled", "disabled");
    document.getElementById('btn').innerHTML = t + "S后重新获取";
    for (i = 1; i <= t; i++) {
        window.setTimeout("update_p(" + i + "," + t + ")", i * 1000);
    }
}

function update_p(num, t) {
    if (num == t) {
        document.getElementById('btn').innerHTML = "获取短信验证码";
        document.getElementById('btn').removeAttribute("disabled");
    }
    else {
        printnr = t - num;
        document.getElementById('btn').innerHTML = printnr + "S后重新获取";
    }
}
function showtime1(t) {
    document.getElementById('btn1').disabled = true;
    document.getElementById('btn1').innerHTML = t + "S后重新获取";
    for (i = 1; i <= t; i++) {
        window.setTimeout("update_p1(" + i + "," + t + ")", i * 1000);
    }
}

function update_p1(num, t) {
    if (num == t) {
        document.getElementById('btn1').innerHTML = "获取邮箱验证码";
        document.getElementById('btn1').removeAttribute("disabled");
    }
    else {
        printnr = t - num;
        document.getElementById('btn1').innerHTML = printnr + "S后重新获取";
    }
}

/**************检测注册页面验证码的方法*********************/

function LoadVerificationCode() {
    $('#checkCodeImg').attr('src', '/Member/GetCheckCode?time=' + new Date().getTime());
}


var flag = false;
function Yzm() {
    var $yzm = trim($('#user_yzm').val());
    if ($yzm == '' || $yzm == null) {
        $(".no_yzm").css({
            "visibility": "visible"
        });
        $('.wrong_yzm').css('visibility', 'hidden')
        return false;
    } else {
        $.ajax({
            type: "POST",
            url: "/Member/CheckCode?r=" + Math.random(),
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            data: { checkCode: $.trim($("#user_yzm").val()) },
            success: function (result) {
                if (!result.Result) {
                    $(".wrong_yzm").css({
                        "visibility": "visible"
                    });
                    $(".no_yzm").css({
                        "visibility": "hidden"
                    });
                    LoadVerificationCode();
                    flag = false;
                    return flag;
                }
                else {
                    $(".user_yzm").css({
                        "display": "block",
                        "background-position": "-59px -144px"
                    });
                    $(".no_yzm,.wrong_yzm").css({
                        "visibility": "hidden"
                    });

                    $.ajax({
                        type: "POST",
                        url: "/Member/SendCodesWhenRegister?r=" + Math.random(),
                        contentType: "application/x-www-form-urlencoded; charset=utf-8",
                        data: { userName: trim($("input[name='userName']").val()) },
                        success: function (result) {
                            if (result.Result) {
                                showtime(60);
                                showtime1(60);
                            }
                            else
                                _toastr(result.Message, "top-right", "error", false);
                            LoadVerificationCode();
                        }
                    });

                    flag = true;
                    return flag;
                }
            }

        });
        return flag
    }
}

/**************检测注册页面短信邮箱验证码的方法*********************/
function Dxyzm() {
    var $dx = trim($('#user_dx').val());
    if ($dx == '' || $dx == null) {
        $(".dx_yzm").css({
            "visibility": "hidden"
        });
        $(".no_dxyzm").css({
            "visibility": "visible"
        });

        return false;
    } else {
        $('.user_dx').css({
            'display': 'block',
            'background-position': '-59px -144px'
        });
        $(".dx_yzm,.no_dxyzm").css({
            "visibility": "hidden"
        });
    }
}
function Yxyzm() {

    var $dx1 = trim($('#user_dx1').val());
    if ($dx1 == '' || $dx1 == null) {
        $(".yx_yzm").css({
            "visibility": "hidden"
        });
        $(".no_yxyzm").css({
            "visibility": "visible"
        });
        return false;
    } else {
        $('.user_dx1').css({
            'display': 'block',
            'background-position': '-59px -144px'
        });
        $(".yx_yzm,.no_yxyzm").css({
            "visibility": "hidden"
        });
    }
}

/**************检测注册页面密码的方法*********************/
function Checkpwd() {
    var pwd1 = trim($("#pwd_1").val());
    if (pwd1 == "") {

        $(".no_pw1,.pwd_l1").css({
            "visibility": "visible"
        });
        $(".wrong_pw1").css({
            "visibility": "hidden"
        });
        return false;
    }
    else if (pwd1.length < 6 || pwd1.length > 16) {

        $(".no_pw1").css({
            "visibility": "hidden"
        });
        $(".wrong_pw1").css({
            "visibility": "visible"
        });
        return false;
    }
    else {
        $(".r_pwd").css({
            "display": "block",
            "background-position": "-59px -144px"
        });
        $(".no_pw1").css({
            "visibility": "hidden"
        });
        $(".wrong_pw1").css({
            "visibility": "hidden"
        });
        return true;
    }
}

/**************检测注册页面二次输入密码方法*********************/
function Checkagianpwd() {
    var pasignwd = trim($("#pwd_2").val());
    var pwd1 = trim($("#pwd_1").val());
    if (pasignwd == "") {
        $(".no_pw2,.pwdasg_id,.pwd_l2").css({
            "visibility": "visible"
        });
        $(".wrong_pw2").css({
            "visibility": "hidden"
        });
        return false;
    }
    else if (pasignwd.length < 6 || pasignwd.length > 16 || pasignwd != pwd1) {

        $(".no_pw2").css({
            "visibility": "hidden"
        });
        $(".wrong_pw2").css({
            "visibility": "visible"
        });
        return false;
    }
    else {
        $(".pwdasg_id").css({
            "display": "block",
            "background-position": "-59px -144px"
        });
        $(".no_pw2").css({
            "visibility": "hidden"
        });
        $(".wrong_pw2").css({
            "visibility": "hidden"
        });
        return true;
    }
}

/**************在页面提交之前对输入数据的验证方法*********************/
function Checkform() {
    if (!$(".reg_checkbox").hasClass("act")) {
        alert("请同意条款！");
        return false;
    }

    var str = trim($(".user_id").val());
    var pasignwd = trim($("#pwd_2").val());
    var pwd1 = trim($("#pwd_1").val());
    var yzm = trim($("#user_yzm").val());
    var yzm1 = trim($(".yzm").val());


    if ((CheckloginPhone() || CheckloginEmail()) && Checkpwd() && Checkagianpwd()) {
        $("#btn_regEmail").val("提交中...");

        var token = $('[name=__RequestVerificationToken]').val();
        var headers = {};
        //防伪标记放入headers
        //也可以将防伪标记放入data
        headers["__RequestVerificationToken"] = token;
        $.ajax({
            type: "POST",
            url: "?r=" + Math.random(),
            headers: headers,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            data: $("form:first").serialize(),
            success: function (result) {
                if (!result.Result) {
                    $("#btn_regEmail").val("立即注册");
                    if (result.Code == 98) {
                        $(".yx_yzm").css({
                            "visibility": "visible"
                        });
                        $(".dx_yzm").css({
                            "visibility": "visible"
                        });
                    }
                    else {
                        _toastr(result.Message, "top-right", "error", false);
                    }
                } else {
                    window.location.href = "/Member/RegisterResult?resultId=0";
                }

            }
        });
        return false;
    }
    else {
        $("#btn_regEmail").val("立即注册");
        return false;
    }

}

//*判断是否只有只包括字母，数字 函数
function ischar(s) {
    if (s.length == 0) return false;
    var regu = "^[0-9A-Za-z]*$";
    var re = new RegExp(regu);
    // alert("ssss---s.search(re):"+s.search(re));
    if (s.search(re) != -1)
        return true;
    else {
        return false;
    }
}
//去掉所有空格
function Trim(str, is_global) {
    var result;
    result = str.replace(/(^\s+)|(\s+$)/g, "");
    if (is_global.toLowerCase() == "g")
        result = result.replace(/\s/g, "");
    return result;
}
//*判断是否只有只包括数字
function isnum(s) {
    if (s.length == 0) return false;
    var regu = "^[0-9]*$";
    var re = new RegExp(regu);
    // alert("ssss---s.search(re):"+s.search(re));
    if (s.search(re) != -1)
        return true;
    else {
        return false;
    }
}
//*判断是否为数字或小数
function isnumorfloat(s) {
    if (s.length == 0) return false;
    var regu = "^[0-9]*$";
    var re = new RegExp(regu);
    // alert("ssss---s.search(re):"+s.search(re));
    if (s.search(re) != -1)
        return true;
    else {
        var regu1 = "^[0-9]*[\.][0-9]*$";
        var re1 = new RegExp(regu1);
        if (s.search(re1) != -1)
            return true;
        else
            return false;
    }
}
function isName(value) {
    return (new RegExp(/[^\w\u4e00-\u9fa5]/).test(value));
}
function GetThisUrl() {
    if (document.getElementById("UrlReferrer")) {
        document.getElementById("UrlReferrer").value = top.window.location.href;
    }
}

// *判断是否中文函数
function ischinese(s) {
    var ret = false;

    for (var i = 0; i < s.length; i++) {
        if (s.charCodeAt(i) >= 256) {
            ret = true;
            break;
        }
    }
    return ret;
}

//检查E-mail的输入是否合法
function ismail(mail) {
    return (new RegExp(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/).test(mail));
}

//*判断手机号的合法性
function checkmobile(str) {
    var reg = /^1[23456789]\d{9}$/;
    return reg.test(str);
}

//保留两位小数
function ToDouble(value) {
    return value.toFixed(2);
}

//测量有汉字时的字串实际长度，其中一个汉字占两个字符
function len(s) {
    var length = 0;
    var tmpArr = s.split("");

    for (i = 0; i < tmpArr.length; i++) {
        if (tmpArr[i].charCodeAt(0) < 299)
            length++;
        else
            length += 2;
    }
    return length;
}

// 将某一字符串去左右空格处理
function trim(str) {
    return (str + '').replace(/(\s+)$/g, '').replace(/^\s+/g, '');
}

//将某一字符串去所有空格处理
function deltrim(strs) {
    var Finds = / /g;
    strs = strs + strs.replace(Finds, "");
    return strs;
}

//全角转半角
function tot(mobnumber) {
    var result = "";
    for (var i = 0; i < mobnumber.length; i++) {
        if (mobnumber.charCodeAt(i) == 12288) {
            result += String.fromCharCode(mobnumber.charCodeAt(i) - 12256);
            continue;
        }
        if (mobnumber.charCodeAt(i) > 65280 && mobnumber.charCodeAt(i) < 65375) {
            result += String.fromCharCode(mobnumber.charCodeAt(i) - 65248);
        }
        else {
            result += String.fromCharCode(mobnumber.charCodeAt(i));
        }
    }
    return result;
}