$(function () {


    initNiceScroll();

    var imageUrl = $('input[name="imageCategoryImageUrl"]').val();
    initImageUploader();
    bindTextLinkBtnClickEvent();
    bindPLinkBtnClickEvent();
    bindSubmitClickEvent();
    bindHideSelector();

    $('.floor-ex-img .ex-btn').hover(function () {
        $(this).parent().toggleClass('active');
    });
});


function bindHideSelector() {

    $('.choose-checkbox').each(function () {
        var _this = $(this);
        $(this).find('a').click(function () {
            $(this).parent().hide().siblings().fadeIn();
            var h = _this.find('.scroll-box').height();
            _this.find('.enter-choose').show();
            _this.animate({ height: h }, 200);
            _this.css({ paddingBottom: '50px' });
        })

        $(this).find('.btn').click(function () {
            _this.height(_this.find('.scroll-box').height());
            _this.css({ padding: 0 }).find('.scroll-box').hide().siblings('.choose-selected').fadeIn();
            _this.find('.enter-choose').hide();
            _this.animate({ height: '43px' }, 200);
            var id = $(this).attr('name');
            getSelectedText(id);
        });
    });
}



function getSelectedText(type) {
    var text = [];
    if (type == 'categoryBtn') {
        var categoryCheckBoxes = $('input[name="category"]:checked');
        $.each(categoryCheckBoxes, function () {
            text.push($(this).parent().text());
        });
        $('#selectedCategories').html(text.join(','));
    }
    else {
        var brandsCheckBoxes = $('input[name="brand"]:checked');
        $.each(brandsCheckBoxes, function () {
            text.push($(this).parent().text());
        });
        $('#selectedBrands').html(text.join(','));
    }



}


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


function getSelectedBrands() {
    //获取选择品牌

    var brandsCheckBoxes = $('.choose-brand input[class="brandCheckbox"]');
    var brands = [];
    $.each(brandsCheckBoxes, function () {
        brands.push({ id: $(this).val() });
    });
    console.log(brands);
    return brands;
}

function initImageUploader(imageUrl) {
    //初始化图片上传
    for (var i = 1; i <= 10; i++) {
        $("#up" + i).smallUpload({
            title: '',
            imageDescript: '211*260',
            displayImgSrc: $("#up" + i).attr('url'),
            displayId: $("#up" + i).attr('pid'),
            imgFieldName: ""
        });
    }
   
}

function bindTextLinkBtnClickEvent() {


    var html = ['<tr type="textLink" name="">',
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
            caption: '商家',
            entity: "vendor",
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

function bindPLinkBtnClickEvent() {

    var plinks = $(".pLink");
    $.each(plinks, function () {
        var self = $(this);
        var $buttonSearch = $(this).parent().find("button");
        $buttonSearch.click(function () {
            $("#" + self.attr("id")).entityPicker('loadDialog', {
                url: '/Common/EntityPicker',
                caption: '商品',
                // entity: "manufacturer",
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


    });
}

function bindArticlesBtnClickEvent() {
    var html = ['<tr type="Articles" name="">',
                     , '<td><input class="form-control input-xs" type="text" name="name" value="" /></td>'
                     , '<td><span ids="">0</span></td>'
                     , '<td class="td-operate"><span class="btn-a"><a>选择商品</a></span><span class="btn-del"><a href="#">删除</a></span></td>'
             , '</tr>'].join();

    $('#addArticles').click(function () {
        if ($('tr[type="Articles"]').length === 4) { toastr['error']('选项卡最多添加4个.', ''); return; }
        $('tbody[type="Articles"]').append(html);
        $('tbody[type="Articles"] span[class="btn-a"] a').unbind("click");
        $('tbody[type="Articles"] span[class="btn-del"] a').unbind("click");
        $('tbody[type="Articles"] span[class="btn-a"] a').click(function () {
            var aSelf = this;
            var ids = $(aSelf).parents("tr").find("td span")[0].getAttribute("ids");
            var selectids = [];

            if (ids != null && ids != undefined && ids.length > 0) {
                var array = ids.split(',');
                for (var i = 0 ; i < array.length; i++) {
                    selectids.push(parseInt(array[i]));
                }
            }
            $.articleSelector.show(selectids, function (selectedArticles) {
                var ids = [];
                $.each(selectedArticles, function () {
                    ids.push(this.id);
                });

                $(aSelf).parents("tr").find("td span")[0].innerHTML = ids.length;
                $(aSelf).parents("tr").find("td span")[0].setAttribute("ids", ids);
            });
        })
        $('tbody[type="Articles"] span[class="btn-del"] a').click(function () {
            $(this).parent().parent().parent().remove();
        })
    });


    $('tbody[type="Articles"] span[class="btn-a"] a').click(function () {
        var aSelf = this;
        var ids = $(aSelf).parents("tr").find("td span")[0].getAttribute("ids");
        var selectids = [];

        if (ids != null && ids != undefined && ids.length > 0) {
            var array = ids.split(',');
            for (var i = 0 ; i < array.length; i++) {
                selectids.push(parseInt(array[i]));
            }
        }
        $.articleSelector.show(selectids, function (selectedArticles) {
            var ids = [];
            $.each(selectedArticles, function () {
                ids.push(this.id);
            });

            $(aSelf).parents("tr").find("td span")[0].innerHTML = ids.length;
            $(aSelf).parents("tr").find("td span")[0].setAttribute("ids", ids);
        });
    })

    $('tbody[type="Articles"] span[class="btn-del"] a').click(function () {
        $(this).parent().parent().parent().remove();
    })

}

function getArticles() {
    var articleRows = $('tr[type="Articles"]');
    var textLinks = [];
    $.each(articleRows, function () {
        var id = $(this).attr('name');
        var name = $(this).find('input[name="name"]').val();
        var ids = $(this).find("td span")[0].getAttribute("ids").split(',');
        if (ids.length == 1 && ids[0] == "") {
            throw Error('请为选项卡选择商品');
        }
        var tabs = [];
        for (var idx = 0 ; idx < ids.length; idx++) {
            tabs.push({ tabId: id, articleId: ids[idx] });
        }
        if (!name) {
            throw Error('请将选项卡名称填写完整');
        }
        else if (GetStringLength(name) > 12) {
            throw Error('选项卡名称不能超过6个字符');
        }
        textLinks.push({ id: id ? id : 0, name: name, detail: tabs });
    });
    return textLinks;
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
        else if (GetStringLength(name) > 12) {
            throw Error('文本模块名称不能超过6个字符');
        }
        textLinks.push({ id: id ? id : 0, name: name, url: url, pictureId: 0 });
    });
   
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

function getPLink() {
    var plinks = $(".pLink");
    var pLinksArray = [];
    var index = 1;
    $.each(plinks, function () {
        var position = $(this).attr("position");
        var imageURL = $(this).parent().parent().prev().find(".hiddenImgSrc").val();
        var imagePictureId = $(this).parent().parent().prev().find(".hiddenPictureId").val();
        var url = $(this).val();
        //if (!imageURL && position != 29 && position != 30 && position != 31 && position != 32) {
        //    throw Error('第' + index + '个区域未上传图片');
        //}
        //if (!url && position != 29 && position != 30 && position != 31 && position != 32) {
        //    throw Error('第' + index + '个区域未填写链接');
        //}
        pLinksArray.push({ id: position, name: imageURL, url: url, pictureId: imagePictureId });
        index++;
    });
    console.log(pLinksArray);
    return pLinksArray;
}





function bindSubmitClickEvent() {
    var floorDetail = {
        id: null,
        brands: [],
        textLinks: [],
    };

    $('button[name="submit"]').click(function () {
        floorDetail.id = $('#homeFloorId').val();
        floorDetail.DefaultTabName = $('#DefaultTabName').val();
        floorDetail.ProductIds = $('#ProductIds').val();
        floorDetail.CategoryIds = $('#CategoryIds').val();
        floorDetail.ManufacturerIds = $('#ManufacturerIds').val();
        try {
            floorDetail.textLinks = getTextLinks();
            floorDetail.brands = getSelectedBrands();
            floorDetail.articleLinks = getPLink();
            floorDetail.name = getFloorName();
            floorDetail.subName = $("#SubName").val();
            floorDetail.tabs = getArticles();
            floorDetail.styleLevel = 0;
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


