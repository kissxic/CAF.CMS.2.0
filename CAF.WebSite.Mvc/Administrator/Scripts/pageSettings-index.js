$(function () {
    //  InitUpload();
    //  SetLogo();
    // SetKeyWords();
    SetRecommand();
    SetArticleRecommand();
});

function InitUpload() {
    $("#uploadImg").smallUpload(
   {
       displayImgSrc: logo,
       imgFieldName: "Logo",
       title: 'LOGO:',
       imageDescript: '270*60',
       dataWidth: 8
   });
}

//设置LOGO
function SetLogo() {
    $('.logo-area').click(function () {
        $.dialog({
            title: 'LOGO设置',
            lock: true,
            width: 350,
            id: 'logoArea',
            content: document.getElementById("logosetting"),
            padding: '20px 10px',
            okVal: '保存',
            ok: function () {
                var logosrc = $("input[name='Logo']").val();
                if (logosrc == "") {
                    $.dialog.tips("请上传一张LOGO图片！");
                    return false;
                }

                $.post(setlogoUrl, { logo: logosrc },
                    function (data) {

                        if (data.success) {
                            $.dialog.succeedTips("LOGO修改成功！");
                            $("input[name='Logo']").val(data.logo);
                            logo = data.logo;
                        }
                        else { $.dialog.errorTips("LOGO修改失败！") }
                    });
            }
        });
    });
}


//热门关键字设置
function SetKeyWords() {
    $('.search-area').click(function () {
        $("#txtkeyword").val(keyword);
        $("#txthotkeywords").val(hotkeywords);
        $.dialog({
            title: '热门关键字设置',
            lock: true,
            id: 'searchArea',
            content: document.getElementById("keywordsSettting"),
            okVal: '保存',
            ok: function () {
                var word = $("#txtkeyword").val();
                var words = $("#txthotkeywords").val();
                if (word == "" || words == "") {
                    $.dialog.tips("请填写关键字！");
                    return false;
                }

                $.post(setkeyWords, { keyword: word, hotkeywords: words },
                     function (data) {

                         if (data.success) {
                             $.dialog.succeedTips("关键字设置成功！");
                             logo = data.logo;
                             keyword = word;
                             hotkeywords = words;
                         }
                         else { $.dialog.succeedTips(data.msg); }
                     });
            }
        });
    });
}

//LOGO设置

function SetRecommand() {

    //橱窗1编辑
    $('.imageAdRecommend').click(function () {
        var that = this;
        var thisPic = $(this).attr('pic');
        var thisUrl = $(this).attr('url');
        var thisPId = $(this).attr('pid');
        var value = $(this).attr('value');
        var imageDescript = "180*300";

        switch (parseInt(value)) {
            case 1:
            case 2:
                break;
        }

        //页面层
        layer.open({
            type: 1,
            skin: 'layui-layer-rim', //加上边框
            area: ['420px', '240px'], //宽高
            content: ['<div class="dialog-form">',
                '<div id="HandSlidePic" class="form-group upload-img clearfix">',
                '</div>',
                '<div class="form-group clearfix">',
                    '<label class="label-inline" for="">跳转链接</label>',
                    '<input class="form-control input-sm" type="text" id="url">',
                '</div>',
            '</div>'].join(''),
            success: function (layero, index) {
                $("#HandSlidePic").smallUpload(
             {
                 title: '显示图片',
                 imageDescript: imageDescript,
                 displayImgSrc: thisPic,
                 displayId: thisPId,
                 imgFieldName: "HandSlidePic",
                 dataWidth: 8
             });
                $("#url").val(thisUrl);
            },
            btn: ['保存']
             , yes: function (index, layero) {
                 var valida = false;
                 var id = parseInt($(that).attr('value'));
                 var url = $("#url").val();
                 var pic = $("#HandSlidePic").smallUpload('getImgSrc');
                 var pid = $("#HandSlidePic").smallUpload('getImgPid');

                 if (url.length === 0) { $("#url").focus(); toastr['error']('链接地址不能为空.', ''); return valida; }
                 if (pic.length === 0) { toastr['error']('图片不能为空.', ''); return valida; }

                 $.ajax({
                     type: 'POST',
                     url: '/admin/pagesettings/updateimagead',
                     cache: false,
                     data: { url: url, pic: pid, id: id },
                     dataType: 'json',
                     success: function (data) {
                         if (data.Result) {
                             toastr['success']('推荐商品修改成功.', '');
                             location.reload();
                         }
                     },
                     error: function (data) {
                         toastr['error']('操作失败,请稍候尝试', '');
                     }
                 });
             }


        });

    });
}


function SetArticleRecommand() {

    //橱窗1编辑
    $('.articleAdRecommend').click(function () {
        var that = this;
        var thisType = $(this).attr('typeId');
        var value = $(this).attr('value');

        //页面层
        layer.open({
            type: 1,
            skin: 'layui-layer-rim', //加上边框
            area: ['420px', '240px'], //宽高
            content: ['<div class="dialog-form">',
                '<div class="form-group clearfix">',
                    '<label class="col-md-2 label-inline" for="">关联ID</label>',
                    //'<input class="form-control input-sm" type="text" id="RelateIds">',
                    '<div class="col-md-10 input-group"><input class="form-control input-xs pLink" type="text" id="RelateIds" /><span class="input-group-btn"><button type="button" class="btn blue" data-loading-text="加载&hellip;"><i class="fa fa-search"></i>&nbsp;搜索</button></span></div>',
                '</div>',
            '</div>'].join(''),
            success: function (layero, index) {

                // add required products
                $("button").on('click', function () {
                    $('#RelateIds').entityPicker('loadDialog', {
                        url: '/Common/EntityPicker',
                        caption: thisType == 1 ? '商家' : "资讯",
                        entity: thisType == 1 ? 'vendor' : "article",
                        returnValueDelimiter: ',',
                        onLoadDialogBefore: function () {
                            $('button').button('loading').prop('disabled', true);
                        },
                        onLoadDialogComplete: function () {
                            $('button').prop('disabled', false).button('reset');
                        }
                    });

                });

            },
            btn: ['保存']
             , yes: function (index, layero) {
                 var valida = false;
                 var id = parseInt($(that).attr('value'));
                 var url = $("#url").val();
                 var pic = $("#HandSlidePic").smallUpload('getImgSrc');
                 var pid = $("#HandSlidePic").smallUpload('getImgPid');

                 if (url.length === 0) { $("#url").focus(); toastr['error']('链接地址不能为空.', ''); return valida; }
                 if (pic.length === 0) { toastr['error']('图片不能为空.', ''); return valida; }

                 $.ajax({
                     type: 'POST',
                     url: '/admin/pagesettings/updateimagead',
                     cache: false,
                     data: { url: url, pic: pid, id: id },
                     dataType: 'json',
                     success: function (data) {
                         if (data.Result) {
                             toastr['success']('推荐商品修改成功.', '');
                             location.reload();
                         }
                     },
                     error: function (data) {
                         toastr['error']('操作失败,请稍候尝试', '');
                     }
                 });
             }


        });

    });
}