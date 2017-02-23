/**
 * 将导航和内容绑定，滚动到相应内容区导航高亮，点击相应导航滚动到相应内容区
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.activemenu',['jquery','jquery.widget','jquery.waypoints'],factory);
  } else {
    factory( jQuery,widget,Waypoint );
  }
}(function($,widget,Waypoint){

  var Activemenu = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    
    this.$el = $(this.options.el);
    this.$navs = this.$el.children();
    this.$panels = $(this.options.panels);
    this.init();
  }

  $.extend(Activemenu.prototype,{
    init: function(){
      var percent;
      //activemenuOffset优先级高于offset属性
      if(this.options.activemenuOffset || this.options.activemenuOffset==0){
        this.options.offset = this.options.activemenuOffset;
      }
      if(percent=this.options.offset.match(/\d+(?=%)/)){
        this.options.offset = $(window).height()*parseInt(percent[0])/100;
      }
      this.events();
    },
    events: function(){
      var that = this;

      this.$el.on('click','>',function(e){
        var $this = $(this);
        var index = $this.index();
        that.scrollTo(index);
        return false;
      });

      this.$panels.waypoint({
        offset : this.options.offset,
        handler: function(direction) {
          var index = $(this.element).index();
          if(direction == "up" || direction == "left") index--;
          that.select(index);
        }
      });
    },
    scrollTo: function(index){
      var $panel = this.$panels.eq(index);
      var offset = $panel.offset();
      $('html,body').stop().animate({'scrollTop':offset.top-this.options.offset+1},this.options.duration);
    },
    refresh: function(){
      Waypoint.refreshAll();
    },
    select: function(index){
      if(index<0 || index>this.$navs.length) return;
      this.$navs.eq(index).addClass(this.options.activeName)
      .siblings('.'+this.options.activeName).removeClass(this.options.activeName);
    }
  });

  $.extend(Activemenu,{
    options: {
      offset: 0,
      duration: 600,
      activeName: 'active',
      handler: $.noop
    }
  });

  widget.install('activemenu',Activemenu);

  return Activemenu;

}));
