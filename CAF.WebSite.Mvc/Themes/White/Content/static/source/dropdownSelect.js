/**
 * [下拉选择]
 * @param  {[string]} evtnt [触发事件]
 * @return {[object]}         [DropdownSelect实例]
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.dropdownSelect',['jquery','underscore','jquery.widget','jquery.dropdown'],factory);
  } else {
    factory( jQuery,_,widget );
  }
}(function($,_,widget){
  var DropdownSelect = function(opt){
    if(opt.event == "hover"){
      opt.event = "mouseenter";
    }
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(opt.el);
    this.options.width = this.$el.innerWidth();
    this.$el.hide();
    this.init();
  }

  $.extend(DropdownSelect.prototype,{
    init : function(){
      var _this = this;
      this.createWidget();
      this.addClass()
      this.events();
    },
    events: function(){
      var that = this;
      this.$widget.on('click','.ui-dropdown-menu li',function(){
        var value = $(this).data('value');
        that.$el.val(value);
        that.$el.trigger('change');
        that.$el.trigger('focusout');
      });
      this.$el.on('change',$.proxy(this.changeValue,this));
    },
    changeValue: function(){
      var $selected = this.$el.find('option:selected');
      var value = $selected.val();
      var text = $selected.text();
      this.$selectName.text(text);
      this.$selectMenu.find('>ul>li[data-value="'+value+'"]').addClass('selected')
      .siblings('.selected').removeClass('selected');
    },
    createWidget: function(){
      this.list = this.$el.children().map(function(){
        var $this = $(this);
        return {value:$this.val(),text:$this.text()}
      });
      var css = {
        width: this.options.width+2
      }
      if(!this.$widget){
        this.$widget = $('<span class="ui-dropdown-select ui-dropdown"></span>').insertAfter(this.$el);
        this.$widget.css(css);
      }else{
        this.$el.detach();
        this.$widget.empty();
      }
      this.$widget.html(this._temp({list:this.list})).append(this.$el);
      this.$selectName = this.$widget.find('.ui-dropdown-select-name');
      this.$selectMenu = this.$widget.find('.ui-dropdown-menu');
      this.$selectMenu.css({minWidth:this.options.width});
      this.changeValue();
    },
    addClass: function(name){
      name = name || this.options.className;
      this.$widget.addClass(name);
    },
    refresh: function(){
      this.createWidget();
    },
    destory: function(){
      this.$widget.before(this.$el);
      this.$widget.remove();
      this.$el.data('dropdownSelect',null);
      this.$el.show();
    },
    _temp: _.template([
      '<i class="ui-dropdown-arrow"></i>',
      '<a href="javascript:;" data-widget="dropdown" class="ui-dropdown-select-name"></a>',
      '<div class="ui-dropdown-menu">',
        '<ul>',
        '<% _.each(list,function(item){ %>',
          '<li data-value="<%= item.value %>"><a href="javascript:;"><%= item.text %></a></li>',
        '<% }) %>',
        '</ul>',
      '</div>',
    ].join(''))
  });
  $.extend(DropdownSelect,{
    options : {
      className: '',
      event: 'click'
    }
  });

  widget.install('dropdownSelect',DropdownSelect);

  return DropdownSelect

}));
