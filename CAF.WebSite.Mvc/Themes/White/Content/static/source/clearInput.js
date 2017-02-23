/**
 * [清除文本表单内容]
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.clearInput',['jquery','jquery.widget'],factory);
  } else {
    factory( jQuery,widget );
  }
}(function($,widget){
  function ClearInput(opt){
    this.options = $.extend(true,{},ClearInput.options,opt);
    this.$el = $(opt.el);
    this.init();
  }

  $.extend(ClearInput.prototype,{
    init : function(){
      var _this = this;
      this.options.height = this.$el.outerHeight();
      var height = this.options.height;
      this.$button = $('<a href="javascript:;">&times;</a>')
        .css({
          position : 'absolute',
          display : 'none',
          height : height,
          width : height,
          zIndex : 10000,
          fontSize : Math.ceil(height*0.8),
          fontWeight : 700,
          color : "#000",
          lineHeight : height-2+'px',
          verticalAlign : 'middle',
          textDecoration : 'none',
          textAlign : 'center'
        }).appendTo($(document.body));
      this.$button.on('mousedown',$.proxy(this.clear,this));
      this.$el.on('focus keyup input propertychange',function(e){
        if(e.type == 'focus') _this.position();
        _this.action();
      });
      this.$el.on('blur',$.proxy(this.hide,this));
      this.position();
      this.$el.on('remove',$.proxy(this.destory,this));
    },
    show : function(){
      this.$button.show();
    },
    hide : function(){
      this.$button.hide();
    },
    position : function(){
      var width = this.$el.outerWidth();
      var offset = this.$el.offset();
      offset.left = offset.left + width - this.options.height;
      this.$button.css(offset);
    },
    action : function(){
      if(this.$el.val() !== ""){
        this.show();
      }else{
        this.hide();
      }
    },
    clear : function(){
      this.$el.val('').trigger('focus');
    },
    destory : function(){
      this.$button.remove();
    }
  });

  $.extend(ClearInput,{
    options : {
    }
  });

  widget.install('clearInput',ClearInput);

  return ClearInput;

}));
