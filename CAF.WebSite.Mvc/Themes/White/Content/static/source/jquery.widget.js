/**
 * widget全局组件支持
 * install : 注册一个组件以便使用[data-widget]方式调用
 * update : 实例化子节点所有组件
 *   $el => 节点（默认为document）
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.widget',['jquery'],factory);
  } else {
    factory( jQuery );
  }
}(function($){

  var widget = {}  

  $.extend(widget,{
    install: function(name,className){
      var obj={};
      if(!name || !className) return;

      obj[name] = function(opt){
        opt = opt || {},obj = {};
        var args = Array.prototype.slice.apply(arguments);
        args.shift();
        return this.each(function(){
          var $this = $(this);
          var data = $this.data(name);

          if(!data){
            if($.type(opt) == "object"){
              obj = $.extend(obj,opt);
            }
            obj = $.extend(obj,{el:$this});
            data = new className(obj);
            $this.data(name,data);
          }

          if(data && $.type(opt) == 'string') data[opt].apply(data,args);

          return this;
        });
      }
      $.fn.extend(obj);
    },
    update: function($el){
      $el = $el || $(document);
      var def = $.Deferred();
      var $widgets = $el.find('[data-widget]');
      if($el.is('[data-widget]')){
        $widgets = $widgets.add($el);
      }
      var defs = $.map($widgets,function(el){
        var itemDef = $.Deferred();
        var $el = $(el);
        var data = $el.data();
        var widgetDefs = [];
        var widgets = data.widget.split(',');

        widgetDefs = $.map(widgets,function(item){
          var widgetDef = $.Deferred();
          require(['jquery.'+item],function(){
            $el[item] && $el[item](data);
            widgetDef.resolve();
          });
          return widgetDef.promise();
        });

        $.when.apply(window,widgetDefs).done(itemDef.resolve);
        return itemDef.promise();
      });

      $.when.apply(window,defs).done(def.resolve);
      return def.promise();
    }
  });

  $.fn.extend({
    widget: function(){
      var $this = $(this);
      var def = widget.update($this);
      return def
    }
  })

  return widget;

}));
