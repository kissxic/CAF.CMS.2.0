/**
 * [tab溢出测试]
 * 对一个容器进行子元素单向溢出测试，如果溢出即把溢出元素放到指定容器内。
 * @param  {[domElement]} el [作用元素]
 * @param  {[string]} direction [溢出测试方向]
 * 方法 
 * refresh 重新排版
 * @return {[object]}         [TabNavOverflow 实例]
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.tabNavOverflow',['jquery','underscore','jquery.widget','jquery.dropdownSelect'],factory);
  } else {
    factory( jQuery,_,widget );
  }
}(function($,_,widget){

  var TID = 0;

  var KEYMAP = {
    horizontal: {
      width: 'width',
      itemWidth: 'outerWidth'
    },
    vertical: {
      width: 'height',
      itemWidth: 'outerHeight'
    }
  }

  var TabNavOverflow = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(opt.el);
    this.init();
  }

  $.extend(TabNavOverflow.prototype,{
    init: function(){
      TID++;
      this.tid = 0;
      this.width = 0;
      this.maxWidth = 0;
      this.$container = $('<select style="width:100px;"></select>');
      this.$el.after(this.$container);
      this.$container.attr('name','tabnav'+TID).dropdownSelect();
      this.refresh();
    },
    refresh: function(){
      var that = this;
      this.tid = 0;
      this.width = this.$el.width();
      var list = this.$el.children().add(this.$container.children()).detach();
      this.maxWidth = 0;
      $.each(list,function(key){
        that.add(this);
        that.tid++;
      });
      this.container();
    },
    add: function(item){
      var rt = this._add(item);
      this.$container.data('dropdownSelect').$widget.toggle(!rt);
    },
    _add: function(item){
      var maxWidth
        keys = KEYMAP[this.options.direction],
        $item = $(item);

      if(this.$container.children().length>0){
        //直接测试失败
        this.$container.append(this.createOption(item));
        return false;
      }
      this.$el.append($item);
      maxWidth = this.maxWidth+$item[keys.itemWidth](true);

      if(maxWidth>this.width){
        //测试失败
        $item.detach();
        this.$container.append(this.createOption(item));
        return false;
      }
      this.maxWidth = maxWidth;
      return true;
    },
    container: function(){
      this.$container.dropdownSelect('refresh');
    },
    createOption: function(item){
      var $el = $('<option></option>');
      $el.val(this.tid);
      $el.text($(item).text());
      return $el;
    }
  });
  $.extend(TabNavOverflow,{
    options: {
      direction: 'horizontal'
    }
  });

  widget.install('tabNavOverflow',TabNavOverflow);

  return TabNavOverflow

}));