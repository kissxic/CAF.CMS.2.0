/**
 * [遮罩一个元素]
 * 
 * @param  {[domElement]} el [作用元素]
 * @return {[object]}         [Shade 实例]
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.shade',['jquery','underscore','jquery.widget'],factory);
  } else {
    factory( jQuery,_,widget );
  }
}(function($,_,widget){
  var Shade = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(opt.el);
    this.init();
  }

  $.extend(Shade.prototype,{
    init : function(){
      var attr,$body;
      $body = $(document.body);
      attr = this.$el.offset();
      client = {
        width:$body.width(),
        height:$body.height()
      }

      attr.width= this.$el.outerWidth();
      attr.height= this.$el.outerHeight();
      attr.right= client.width-attr.width-attr.left;
      attr.bottom= client.height-attr.height-attr.top;
      this.attr = attr;
      this.createWidget();
    },
    createWidget: function(){
      var that = this;
      var $content,$overlayer;
      this.$widget = $(this.template());
      $content = this.$widget.find('.ui-shade-content');
      $overlayer = this.$widget.find('.ui-shade-overlayer');
      $content.css({
        width:this.attr.width,
        height:this.attr.height,
        top:this.attr.top,
        left:this.attr.left
      });
      $overlayer.css({
        width:this.attr.width,
        height:this.attr.height,
        borderTopWidth:this.attr.top,
        borderRightWidth:this.attr.right,
        borderBottomWidth:this.attr.bottom,
        borderLeftWidth:this.attr.left
      }).on('click',function(){
        that.remove();
      });
      $content.html(this.options.content);
      $(document.body).append(this.$widget);
    },
    remove: function(){
      this.$el.data('shade',null);
      this.$widget.remove();
    },
    template: _.template([
      '<div class="ui-shade">',
        '<div class="ui-shade-content"></div>',
        '<div class="ui-shade-overlayer"></div>',
      '</div>'
    ].join(''))
  });
  $.extend(Shade,{
    options : {
      content: ''
    }
  });

  widget.install('shade',Shade);

  return Shade

}));