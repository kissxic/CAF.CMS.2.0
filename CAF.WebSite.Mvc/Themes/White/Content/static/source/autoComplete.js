/**
 * [自动完成]
 * @param  {[string]} remote [远程数据源]
 * 数据格式为{state:1,msg:"成功",data:["aaa","bbb",...]}
 * @param  {[string]} field [查询字段名]
 * @param  {[object]} params [额外查询的参数]
 * 
 * @return {[object]}         [AutoComplete实例]
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.autoComplete',['jquery','underscore','jquery.widget','effect','outer'],factory);
  } else {
    factory( jQuery,_,widget,Effect );
  }
}(function($,_,widget,Effect){

  var AutoComplete = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this._list = [];
    this.state = "hide";
    this.$el = $(opt.el);
    this.$el[0].autocomplete = "off";
    this.init();
  }

  $.extend(AutoComplete.prototype,{
    init: function(){
      this.$el.wrap('<div class="ui-autocomplete"></div>');
      this.$wrap = this.$el.parent();
      this.$box = $('<div class="ui-autocomplete-box"><ul class="ui-autocomplete-list"></ul></div>');
      this.$list = this.$box.find('.ui-autocomplete-list');
      this.$el.after(this.$box);
      this.events();
    },
    events: function(){
      var that = this;
      var inBox = false;
      this.$el.on('focus',$.proxy(this.show,this));
      this.$el.on('keydown input',function(e){
        switch(e.keyCode){
          case 9:
          case 37:
          case 13:
          case 39:{
            break;
          }
          case 38:{
            //up
            e.preventDefault();
            e.stopPropagation();
            that.move(-1);
            return false;
          }
          case 40:{
            //down
            e.preventDefault();
            e.stopPropagation();
            that.move(1);
            return false;
          }
          default:{
            that.debounceReq();
          }
        }
      });
      this.$el.on('autoCompleteChange',function(e,data){
        that.$el.val(data);
      });
      this.$list.on('click','>li',function(e){
        var index = $(this).index();
        that.$el.trigger('autoCompleteChange',that._list[index]);
        if(!e.isTrigger) that.hide();
      });
      this.$list.on('outer',function(e){
        if(e.target == that.$el[0]) return;
        that.hide();
      });
    },
    req: function(key){
      var that = this;
      if(!key){
        this.render([],key);
        return;
      }
      var params = {}
      params[this.options.field] = key;
      $.get(this.options.remote,$.extend(params,this.options.params)).done(function(res){
        if(!res) res=[];
        that.render(res,key);
      });
    },
    debounceReq: _.debounce(function(){
      this.req(this.$el.val());
    },300),
    render: function(list,key){
      var that = this;
      this._list = list;
      this.$list.empty();
      _.each(list,function(item,i){
        that.$list.append(that._liTemp({item:item,key:key}));
      });

      if(list.length == 0){
        this.hide();
      }else{
        this.show();
      }
    },
    move: function(step){
      var len = this._list.length;
      var $children = this.$list.children();
      var index = $children.filter('.active').removeClass('active').index();
      var active,num = index+step;
      if(num<0){
        if(index == -1){
          num++;
        }
        active = num%len+len;
      }else{
        active = num%len;
      }
      $children.eq(active).addClass('active').trigger('click');
    },
    show: function(){
      if(this.state == 'show' || this._list.length == 0) return;
      this.$wrap.addClass('open');
      new Effect({
        el: this.$box,
        type: 'fadeInDown',
        speed: 'fast'
      });
      this.state = 'show';
    },
    hide: function(){
      var that = this;
      if(this.state == 'hide') return;
      new Effect({
        el: this.$box,
        type: 'fadeOutUp',
        speed: 'fast'
      }).done(function(){
        that.$wrap.removeClass('open');
        that.state = 'hide';
      });
    },
    _liTemp: _.template([
      '<li class="<% if(key == item){ %>active<% } %>"><span><%= item %></span></li>'
    ].join(''))
  });

  $.extend(AutoComplete,{
    options: {
      field: 'keyword',
      params: {}
    }
  });

  widget.install('autoComplete',AutoComplete);

  return AutoComplete

}));