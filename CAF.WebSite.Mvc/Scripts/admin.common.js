var currentModalId = "";
function closeModalWindow() {
    var modal = $('#' + currentModalId).data('modal');
    if (modal)
        modal.hide();
    return false;
}
function openModalWindow(modalId) {
    currentModalId = modalId;
    $('#' + modalId).data('modal').show();
}
$(document).ready(function () {
    $('input.multi-site-override-option').on('switchChange.bootstrapSwitch', function (event, state) {
        Admin.checkOverriddenSiteValue(this);
    });
});
// global Admin namespace
var Admin = {

    dialog: function (n) {
        var t, i;
        n = n || {};
        t = {
            id: "zm-dialog",
            title: "对话框",
            width: 560,
            height: 400
        };
        $.extend(t, n);
        i = dialog(t);
        i.showModal()
    },

    checkboxCheck: function (obj, checked) {
        if (checked)
            $(obj).attr('checked', 'checked');
        else
            $(obj).removeAttr('checked');
        if (typeof ($(obj).attr("onoffswitch")) != "undefined") $(obj).bootstrapSwitch("state", checked);
    },

    checkAllOverriddenSiteValue: function (obj) {
        $('input.multi-site-override-option').each(function (index, elem) {
            Admin.checkboxCheck(elem, obj.checked);
            Admin.checkOverriddenSiteValue(elem);

        });
    },

    checkOverriddenSiteValue: function (checkbox) {
        var parentSelector = $(checkbox).attr('data-parent-selector').toString(),
            parent = (parentSelector.length > 0 ? $(parentSelector) : $(checkbox).closest('.onoffswitch-container').parent()),
            checked = $(checkbox).is(':checked');

        parent.find(':input:not([type=hidden])').each(function (index, elem) {
            if ($(elem).is('select')) {
                $(elem).select2(checked ? 'enable' : 'disable');
            }
            else if (!$(elem).hasClass('multi-site-override-option')) {
                var tData = $(elem).data('tTextBox');

                if (tData != null) {
                    if (checked)
                        tData.enable();
                    else
                        tData.disable();
                }
                else {
                    if (checked) {
                        $(elem).removeAttr('disabled');
                        //$(elem).bootstrapSwitch("toggleDisabled");  
                    }
                    else
                        $(elem).attr('disabled', 'disabled');
                    if (typeof ($(elem).attr("onoffswitch")) != "undefined") $(elem).bootstrapSwitch("disabled", !checked);
                }
            }
        });
    },

    TaskWatcher: (function () {
        var interval;

        return {
            startWatching: function (opts) {
                function poll() {
                    $.ajax({
                        cache: false,
                        type: 'POST',
                        global: false,
                        url: opts.pollUrl,
                        dataType: 'json',
                        success: function (data) {
                            data = data || [];
                            var runningElements = [];
                            $.each(data, function (i, task) {
                                var el = $(opts.elementsSelector + '[data-task-id=' + task.id + ']');
                                if (el.length) {
                                    runningElements.push(el[0]);
                                    if (el.data('running') && el.text()) {
                                        // already running
                                        el.find('.text').text(task.message || opts.defaultProgressMessage);
                                        el.find('.percent').text(task.percent >> 0 ? task.percent + ' %' : "");
                                    }
                                    else {
                                        // new task
                                        var row1 = $('<div class="hint clearfix" style="position: relative"></div>').appendTo(el);
                                        row1.append($('<div class="text pull-left">' + (task.message || opts.defaultProgressMessage) + '</div>'));
                                        row1.append($('<div class="percent pull-right">' + (task.percent >> 0 ? task.percent + ' %' : "") + '</div>'));
                                        var row2 = $('<div class="loading-bar" style="margin-top: 4px"></div>').appendTo(el);
                                        el.attr('data-running', 'true').data('running', true);
                                        if (_.isFunction(opts.onTaskStarted)) {
                                            opts.onTaskStarted(task, el);
                                        }
                                        el.removeClass('hide');
                                    }
                                }
                            });

                            // remove runningElements for finished tasks (the ones currently running but are not in 'runningElements'
                            var currentlyRunningElements = opts.context.find(opts.elementsSelector + '[data-running=true]');
                            $.each(currentlyRunningElements, function (i, el) {
                                var shouldRun = _.find(runningElements, function (val) { return val == el; });
                                if (!shouldRun) {
                                    // restore element to it's init state
                                    var jel = $(el);
                                    jel.addClass('hide').html('').attr('data-running', 'false').data('running', false);
                                    if (_.isFunction(opts.onTaskCompleted)) {
                                        opts.onTaskCompleted(jel.data('task-id'), jel);
                                    }
                                }
                            });
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            window.clearInterval(interval);
                        }
                    });
                }
                window.setTimeout(poll, 50);
                interval = window.setInterval(poll, 2500);
            }
        }
    })()
};

$.loading = function (bool, text) {
    var $loadingpage = top.$("#loadingPage");
    var $loadingtext = $loadingpage.find('.loading-content');
    if (bool) {
        $loadingpage.show();
    } else {
        if ($loadingtext.attr('istableloading') == undefined) {
            $loadingpage.hide();
        }
    }
    if (!!text) {
        $loadingtext.html(text);
    } else {
        $loadingtext.html("数据加载中，请稍后…");
    }
    $loadingtext.css("left", (top.$('body').width() - $loadingtext.width()) / 2 - 50);
    $loadingtext.css("top", (top.$('body').height() - $loadingtext.height()) / 2);
}
$.modalOpen = function (options) {
    var defaults = {
        id: null,
        title: '系统窗口',
        width: "100px",
        height: "100px",
        url: '',
        shade: 0.3,
        btn: ['确认', '关闭'],
        btnclass: ['btn btn-primary', 'btn btn-danger'],
        callBack: null
    };
    var options = $.extend(defaults, options);
    var _width = top.$(window).width() > parseInt(options.width.replace('px', '')) ? options.width : top.$(window).width() + 'px';
    var _height = top.$(window).height() > parseInt(options.height.replace('px', '')) ? options.height : top.$(window).height() + 'px';
    top.layer.open({
        id: options.id,
        type: 2,
        shade: options.shade,
        title: options.title,
        fix: false,
        area: [_width, _height],
        content: options.url,
        btn: options.btn,
        btnclass: options.btnclass,
        yes: function () {
            options.callBack(options.id)
        }, cancel: function () {
            return true;
        }
    });
}
$.modalConfirm = function (content, callBack) {
    top.layer.confirm(content, {
        icon: "fa-exclamation-circle",
        title: "系统提示",
        btn: ['确认', '取消'],
        btnclass: ['btn btn-primary', 'btn btn-danger'],
    }, function () {
        callBack(true);
    }, function () {
        callBack(false)
    });
}
$.modalAlert = function (content, type) {
    var icon = "";
    if (type == 'success') {
        icon = "fa-check-circle";
    }
    if (type == 'error') {
        icon = "fa-times-circle";
    }
    if (type == 'warning') {
        icon = "fa-exclamation-circle";
    }
    top.layer.alert(content, {
        icon: icon,
        title: "系统提示",
        btn: ['确认'],
        btnclass: ['btn btn-primary'],
    });
}
$.modalMsg = function (content, type) {
    if (type != undefined) {
        var icon = "";
        if (type == 'success') {
            icon = "fa-check-circle";
        }
        if (type == 'error') {
            icon = "fa-times-circle";
        }
        if (type == 'warning') {
            icon = "fa-exclamation-circle";
        }
        top.layer.msg(content, { icon: icon, time: 4000, shift: 5 });
        top.$(".layui-layer-msg").find('i.' + icon).parents('.layui-layer-msg').addClass('layui-layer-msg-' + type);
    } else {
        top.layer.msg(content);
    }
}
$.modalClose = function () {
    var index = top.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
    var $IsdialogClose = top.$("#layui-layer" + index).find('.layui-layer-btn').find("#IsdialogClose");
    var IsClose = $IsdialogClose.is(":checked");
    if ($IsdialogClose.length == 0) {
        IsClose = true;
    }
    if (IsClose) {
        top.layer.close(index);
    } else {
        location.reload();
    }
}

$.submitForm = function (options) {
    var defaults = {
        url: "",
        param: [],
        loading: "正在提交数据...",
        success: null,
        close: true
    };
    var options = $.extend(defaults, options);
    $.loading(true, options.loading);
    window.setTimeout(function () {
        if ($('[name=__RequestVerificationToken]').length > 0) {
            options.param["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
        }
        $.ajax({
            url: options.url,
            data: options.param,
            type: "post",
            dataType: "json",
            success: function (data) {
                if (data.state == "success") {
                    options.success(data);
                    $.modalMsg(data.message, data.state);
                    if (options.close == true) {
                        $.modalClose();
                    }
                } else {
                    $.modalAlert(data.message, data.state);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.loading(false);
                $.modalMsg(errorThrown, "error");
            },
            beforeSend: function () {
                $.loading(true, options.loading);
            },
            complete: function () {
                $.loading(false);
            }
        });
    }, 500);
}
$.deleteForm = function (options) {
    var defaults = {
        prompt: "注：您确定要删除该项数据吗？",
        url: "",
        param: [],
        loading: "正在删除数据...",
        success: null,
        close: true
    };
    var options = $.extend(defaults, options);
    if ($('[name=__RequestVerificationToken]').length > 0) {
        options.param["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    $.modalConfirm(options.prompt, function (r) {
        if (r) {
            $.loading(true, options.loading);
            window.setTimeout(function () {
                $.ajax({
                    url: options.url,
                    data: options.param,
                    type: "post",
                    dataType: "json",
                    success: function (data) {
                        if (data.state == "success") {
                            options.success(data);
                            $.modalMsg(data.message, data.state);
                        } else {
                            $.modalAlert(data.message, data.state);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $.loading(false);
                        $.modalMsg(errorThrown, "error");
                    },
                    beforeSend: function () {
                        $.loading(true, options.loading);
                    },
                    complete: function () {
                        $.loading(false);
                    }
                });
            }, 500);
        }
    });

}
$.fn.formSerialize = function (formdate) {
    var element = $(this);
    if (!!formdate) {
        for (var key in formdate) {
            var $id = element.find('#' + key);
            var value = $.trim(formdate[key]).replace(/&nbsp;/g, '');
            var type = $id.attr('type');
            if ($id.hasClass("select2-hidden-accessible")) {
                type = "select";
            }
            switch (type) {
                case "checkbox":
                    if (value == "true") {
                        $id.attr("checked", 'checked');
                    } else {
                        $id.removeAttr("checked");
                    }
                    break;
                case "select":
                    $id.val(value).trigger("change");
                    break;
                default:
                    $id.val(value);
                    break;
            }
        };
        return false;
    }
    var postdata = {};
    element.find('input,select,textarea').each(function (r) {
        var $this = $(this);
        var id = $this.attr('id');
        var type = $this.attr('type');
        switch (type) {
            case "checkbox":
                postdata[id] = $this.is(":checked");
                break;
            default:
                var value = $this.val() == "" ? "&nbsp;" : $this.val();
                if (!$.request("keyValue")) {
                    value = value.replace(/&nbsp;/g, '');
                }
                postdata[id] = value;
                break;
        }
    });
    if ($('[name=__RequestVerificationToken]').length > 0) {
        postdata["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }
    return postdata;
};

