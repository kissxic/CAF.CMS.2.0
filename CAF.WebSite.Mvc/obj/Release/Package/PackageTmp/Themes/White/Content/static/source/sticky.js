/**
 * 借用jqueryui tabs
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.tabs',['jquery','jquery.widget','jquery.ui'],factory);
  } else {
    factory( jQuery,widget );
  }
}(function($,widget){

  //保存jquery-ui的tabs方法
  var $_fn_tabs = $.fn.tabs;

  var Tabs = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(this.options.el);
    //使用jquery-ui的tabs方法
    $_fn_tabs.call(this.$el,this.options);

    this.$el.fadeIn(500);
  }

  $.extend(Tabs.prototype,{});
  $.extend(Tabs,{});

  //注册组件，覆盖jquery-ui的tabs方法
  widget.install('tabs',Tabs);

  return Tabs

}));