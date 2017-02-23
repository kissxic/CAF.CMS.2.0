/**
 * 借用jqueryui timepicker
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.timepicker',['jquery','jquery.widget','jquery.ui','jquery.datetimepicker'],factory);
  } else {
    factory( jQuery,widget );
  }
}(function($,widget){

  //保存jquery-ui的timepicker方法
  var $_fn_timepicker = $.fn.datetimepicker;

  $.timepicker.regional['zh-CN'] = {
    timeOnlyTitle: '选择时间',
    timeText: '时间',
    hourText: '小时',
    minuteText: '分钟',
    secondText: '秒钟',
    millisecText: '毫秒',
    microsecText: '微秒',
    timezoneText: '时区',
    currentText: '现在时间',
    closeText: '关闭',
    timeFormat: 'HH:mm',
    timeSuffix: '',
    amNames: ['AM', 'A'],
    pmNames: ['PM', 'P'],
    isRTL: false
  };
  $.timepicker.setDefaults($.timepicker.regional['zh-CN']);

  var Timepicker = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(this.options.el);
    //使用jquery-ui的timepicker方法
    $_fn_timepicker.call(this.$el,this.options);
  }

  $.extend(Timepicker.prototype,{});
  $.extend(Timepicker,{});

  //注册组件，覆盖jquery-ui的timepicker方法
  widget.install('timepicker',Timepicker);

  return Timepicker

}));