/**
 * 借用jqueryui datepicker
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.datepicker',['jquery','jquery.widget','jquery.ui'],factory);
  } else {
    factory( jQuery,widget );
  }
}(function($,widget){

  //保存jquery-ui的datepicker方法
  window.$_fn_datepicker = $.fn.datepicker;

  var Datepicker = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(this.options.el);

    //使用jquery-ui的datepicker方法
    $_fn_datepicker.call(this.$el,$.extend(this.options,{
      closeText: "关闭", // Display text for close link
      prevText: "下个月", // Display text for previous month link
      nextText: "上个月", // Display text for next month link
      currentText: "今天", // Display text for current month link
      monthNames: ["一月","二月","三月","四月","五月","六月","七月","八月","九月","十月","十一月","十二月"], // Names of months for drop-down and formatting
      monthNamesShort: ["一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二"], // For formatting
      dayNames: ["星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"], // For formatting
      dayNamesShort: ["周日", "周一", "周二", "周三", "周四", "周五", "周六"], // For formatting
      dayNamesMin: ["日", "一", "二", "三", "四", "五", "六"]
    }));

    //由于jquery-ui的datepicker缓存属性与widget冲突，所以返回jqueryui给widget使用。
    this._datepicker = this.$el.data('datepicker');
    return this._datepicker;

  }
  $.extend(Datepicker.prototype,{});
  $.extend(Datepicker,{
    options: {}
  });

  //注册组件，覆盖jquery-ui的datepicker方法
  widget.install('datepicker',Datepicker);

  return Datepicker

}));