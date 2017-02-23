$(function () {
    initNiceScroll();
    bindTextLinkBtnClickEvent();
    bindSubmitClickEvent();

    $('.floor-ex-img .ex-btn').hover(function () {
        $(this).parent().toggleClass('active');
    });
});


function initNiceScroll() {
    //初始化NiceScroll
    if (+[1, ]) {
        $(".scroll-box").niceScroll({
            styler: 'fb',
            cursorcolor: "#7B7C7E",
            cursorwidth: 6,
        });
    }
}




function bindTextLinkBtnClickEvent() {


    var html = ['<tr type="textLink" name="">'
                     , '<td><input class="form-control input-xs" type="text" name="name" value="" /></td>'
                     , ' <td><div class="col-md-12 input-group"><input class="form-control input-xs " type="text" id="tLink{0}" name="url" value="" /><span class="input-group-btn"><button type="button" class="btn blue" data-loading-text="加载&hellip;"><i class="fa fa-search"></i>&nbsp;搜索</button></span></div></td>'
                     , '<td class="td-operate"><span class="btn-a"><a>删除</a></span></td>'
             , '</tr>'].join();

    //添加textLink表单
    $('#addTextLink').click(function () {
        if ($('tr[type="textLink"]').length === 8) { toastr['error']('文本链接最多添加8个.', ''); return; }
        var index = $('tr[type="textLink"]').length + 1;
        $('tbody[type="textLink"]').append(html.replace("{0}", index));
    });

    //搜索操作
    $('tbody[type="textLink"]').on('click', 'button', function () {
        var $element = $(this).parent().parent().find('input[name="url"]');
        $("#" + $element.attr("id")).entityPicker('loadDialog', {
            url: '/Common/EntityPicker',
            caption: '内容',
            returnField: 'url',
            maxReturnValues: 1,
            returnValueDelimiter: ',',
            onLoadDialogBefore: function () {
                $(this).button('loading').prop('disabled', true);
            },
            onLoadDialogComplete: function () {
                $(this).prop('disabled', false).button('reset');
            }
        });
    });

}


function getTextLinks() {
    var textLinkRows = $('tr[type="textLink"]');
    var textLinks = [];
    $.each(textLinkRows, function () {
        var id = $(this).attr('name');
        var name = $(this).find('input[name="name"]').val();
        var url = $(this).find('input[name="url"]').val();
        if (!name || !url) {
            throw Error('请将文字链接填写完整');
        }
        else if (GetStringLength(name) > 100) {
            throw Error('文本模块名称不能超过50个字符');
        }
        textLinks.push({ id: id ? id : 0, name: name, url: url, pictureId: 0 });
    });
    if (textLinks.length === 0) {
        throw Error('未设置任何文本模块');
    }
    return textLinks;
}

function GetStringLength(str) {
    ///获得字符串实际长度，中文2，英文1 
    ///要获得长度的字符串 
    var realLength = 0, len = str.length, charCode = -1;
    for (var i = 0; i < len; i++) {
        charCode = str.charCodeAt(i);
        if (charCode >= 0 && charCode <= 128)
            realLength += 1;
        else realLength += 2;
    }
    return realLength;
};



function bindSubmitClickEvent() {
    var floorDetail = {
        id: null,
        brands: [],
        textLinks: [],
    };

    $('button[name="submit"]').click(function () {
        floorDetail.id = $('#homeFloorId').val();
        try {
            floorDetail.textLinks = getTextLinks();
            floorDetail.name = getFloorName();
            floorDetail.styleLevel = 2;
            submit(floorDetail);
        }
        catch (e) {
            toastr['error'](e.message, '');
        }
    });
}

function getFloorName() {
    var name = $("#Name").val();
    if (name.replace(/[ ]/g, "").length == 0) {
        $("#Name").focus();
        throw Error('请填写楼层名称');
    }
    if (name.length <= 1) {
        $("#Name").focus();
        throw Error('楼层名称最少为两位字符');
    }
    return name;
}


function submit(floorDetail) {
    var json = JSON.stringify(floorDetail);
    var url = '/admin/pagesettings/savehomefloordetail';
    $.post(url, { floorDetail: json }, function (result) {
        if (result.success) {
            toastr['success']('保存成功.', '');
            window.location.href = '/admin/pagesettings/homefloor';
        }
        else
            toastr['error']('保存失败！' + result.msg, '');

    });

}


