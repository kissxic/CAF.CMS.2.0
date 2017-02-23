require.config({
  shim: {
    "news.ueditor": {
      deps: [
        'css!/Themes/White/Content/public/ueditor/themes/default/css/ueditor.css',
        '/Themes/White/Content/public/ueditor/ueditor.config.js'
      ],
      exports: 'UE'
    }
  },
  paths: {
    //lib
    "news.utils": "/static/js/lib/utils",
    "news.mobile": "/static/js/lib/mobile",
    "news.ueditor": "/public/ueditor/ueditor.all",
    //admin
    "news.admin.zhuanti": "/static/js/admin/zhuanti",
    //page
    "news.pages.detail": "/static/js/pages/detail",
    "news.pages.list": "/static/js/pages/list",
    "news.pages.user": "/static/js/pages/user",
    "news.pages.other": "/static/js/pages/other",
    "news.pages.zhuanti": "/static/js/pages/zhuanti",
    "news.notfound": "/static/js/pages/notfound",

    //widgets
    "jquery.newsSlider2": "/static/js/widget/newsSlider2",
    "jquery.newsSlider3": "/static/js/widget/newsSlider3",
    "jquery.newsNotify": "/static/js/widget/newsNotify",
    "jquery.ueditor": "/static/js/widget/ueditor",
    "jquery.insertImg": "/static/js/widget/insertImg",
    "jquery.newsAjaxTab": "/static/js/widget/newsAjaxTab",
    "jquery.bdShare": "/static/js/widget/bdShare",

    //小动画widgets
    "jquery.dbNoData": "/static/js/widget/dbNoData",

    //template
    "news.template.usamarket": "/static/js/template/usamarket"
  }
});