/**
 * 即将废弃
 * [轮播展示盒子]
 * @param  {[string]} direction [方向]
 * @param  {[string]} event [触发事件类型]
 * @param  {[number]} displayNumber [显示条数]
 * @param  {[boole]} control [是否显示控制面板]
 * @param  {[boole]} auto [是否自动播放]
 * @param  {[boole]} hoverPause [hover状态是否暂停自动播放]
 * @param  {[number]} delay [自动播放迟延]
 * @param  {[number]} duration [补间持续时间]
 * @param  {[string]} easing [缓动函数]
 * @return {[type]}         [description]
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.sliderbox',['jquery','underscore','jquery.widget','jquery.easing'],factory);
  } else {
    factory( $,_,widget );
  }
}(function($,_,widget){
  var SliderBox = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(opt.el);
    this.init();
  }
  $.extend(SliderBox.prototype,{
    init : function(){
      var _this = this;
      this.$el.css({
        position:'relative',
        overflow:'hidden'
      });
      this.$items = this.$el.children();  
      this.$items.css({
        width : this.$items.width(),
        height : this.$items.height()
      });
      this.$items.detach().addClass('ui-sliderbox-item');
      if(this.options.direction == "vertical") this.$items.css({float:'left'});
      this.$itemBox = $('<div class="ui-sliderbox-box"></div>').css({
        position:'absolute',left:0,top:0
      }).append(this.$items).appendTo(this.$el);

      if(this.options.direction == "horizontal") this.$itemBox.css({width:'100%'});

      this.current = null;
      var width = this.$items.eq(0).outerWidth(true),
          height = this.$items.eq(0).outerHeight(true);
      this.options.item = {
        width : width,
        height : height
      }

      if(this.options.direction == "horizontal"){
        this.$itemBox.css({height:height*this.$items.length});
        this.$el.css({
          width : width,
          height : height*this.options.displayNumber
        });
      }else{
        this.$itemBox.css({width:width*this.$items.length});
        this.$el.css({
          width : width*this.options.displayNumber,
          height : height
        });
      }
      
      this.$el.addClass('ui-sliderbox');
      this.initControl();
      this.slider(0);
      if(this.options.auto){
        if(this.options.hoverPause){
          this.$el.hover($.proxy(this.closeAuto,this),$.proxy(this.openAuto,this));
        }
        this.openAuto();
      }
    },
    initControl : function(){
      var _this = this,total;
      if(!this.options.control){
        this.$controlBox = $('<div>');
        return ;
      }
      this.$controlBox = $('<div class="ui-sliderbox-control"></div>').appendTo(this.$el);

      total = Math.ceil(this.$items.length/_this.options.controlSkip);
      _.each(_.range(total),function(item,i){
        var $ct;
        if(!!_this.options.createControl){
          $ct = _this.options.createControl($(this),i);
        }else{
          $ct = $('<a href="javascript:;">'+i+'</a>');
        }
        _this.$controlBox.append($ct);
      });

      this.$controlBox.on(this.options.event,'a',function(){
        var $this = $(this);
        _this.slider($this.index()*_this.options.controlSkip);
      });
      this.$el.on('slider',function(e){
        var index;
        index = Math.floor(e.index/_this.options.controlSkip);
        _this.$controlBox.children().eq(index).addClass('active')
        .siblings('.active').removeClass('active');
      });

      if(_this.options.controlSkip>1){
        this.prev = function(){
          var target;
          target = this.getIndex()-1*_this.options.controlSkip;
          if(target<0) target=Math.ceil(this.$items.length/_this.options.controlSkip);
          if(target>this.$items.length) target=0;
          this.slider(target);
        },
        this.next = function(){
          var target;
          target = this.getIndex()+1*_this.options.controlSkip;
          if(target<0) target=Math.ceil(this.$items.length/_this.options.controlSkip);
          if(target>this.$items.length) target=0;
          this.slider(target);
        }

      }

    },
    slider : function(index,direction){
      var _this = this;

      index = (this.$items.length+index) % this.$items.length
      
      if(this.options.direction == "horizontal"){
        endCss = {top:-this.options.item.height*index}
      }else{
        endCss = {left:-this.options.item.width*index}
      }
      this.$itemBox.stop(true).animate(endCss,{
        duration : this.options.duration,
        easing : this.options.easing
      });

      if(this.current == index) return false;//去除重复调用
      this.current = index;
      var event = $.Event('slider',{index : index});
      this.$el.trigger(event);
    },
    getIndex: function(){
      return this.current || 0;
    },
    prev : function(){
      this.slider(this.getIndex()-1);
    },
    next : function(){
      this.slider(this.getIndex()+1);
    },
    openAuto: function(){
      clearInterval(this.timer);
      this.timer = setInterval($.proxy(this.next,this),this.options.delay);
    },
    closeAuto: function(){
      clearInterval(this.timer);
    }
  });
  $.extend(SliderBox,{
    options : {
      direction : 'vertical',//horizontal,vertical
      event : 'click',
      displayNumber : 1,
      control : true,
      controlSkip : 1,//控制器长度
      auto : true,
      hoverPause: true,
      delay : 3000,
      duration : 400,
      easing : 'easeOutExpo'
    }
  });


  widget.install('sliderbox',SliderBox);

  return SliderBox

}));