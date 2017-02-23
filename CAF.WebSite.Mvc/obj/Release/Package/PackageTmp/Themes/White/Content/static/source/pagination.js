/**
 * [响应式表格]
 * @param  {[number]} offset [上边距偏移量（解决与顶部固定条重叠问题）]
 * @param  {[Boole]} hide [是否初始化时隐藏该列]
 * @param  {[number]} priority [表格列优先级，值越高越先隐藏使用data-priority设置在thead>tr>th上]
 * @return {[type]}         [description]
 */

(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.responsivetable',['jquery','underscore','browser','jquery.widget','jquery.waypoints','jquery.ui','jquery.dropdown','jquery.sticky'],factory);
  } else {
    factory( $,_,browser,widget,Waypoint );
  }
}(function($,_,browser,widget,Waypoint){
  var Responsivetable = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(opt.el);
    this.options.className = this.$el.attr('class');
    this.init();
  }

  $.extend(Responsivetable.prototype,{
    init:function(){
      this.$wrap = $(this._wrapTemp(this.options)).addClass('responsive-table-wrapper').insertAfter(this.$el);
      this.$tableWrap = this.$wrap.children('.responsive-table');
      this.$tableWrap.append(this.$el);
      this.$el = this.$elClone = this.$el;
      this.$actions = this.$tableWrap.find('>.actions');
      this.$head = this.$el.find('thead');
      this.$body = this.$el.find('tbody');
      this.initColumn();
      this.createToolbar();

      this.createStickyHeader();
      this.events();
      this.updateCheckState();
    },
    events: function(){
      var _this = this;
      this.$toolbar.on('change','input[type=checkbox]',function(){
        var $this = $(this);
        var checked = $this.prop('checked');
        var value = parseInt($this.val())+1;
        var $cells = _this.$elClone.find('tr>:nth-child('+value+')')
        $cells.toggleClass('show',checked).toggleClass('hide',!checked);
      });
      $(window).on('resize',function(e){
        _.delay(function(){
          _this.$sticky.css({width:_this.$tableWrap.width()});
        },0);
        _this.windowResize();
      });
      this.$tableWrap.on('scroll',function(){
        _this.$sticky.scrollLeft(_this.$tableWrap.scrollLeft());
      });
      this.$checkGroup.on('click',function(e){
        e.stopPropagation();
      });
      this.$toolbar.on('click','button[data-action=show-all]',function(){
        _this.$checkGroup.find('input[type=checkbox]:not(:checked)').prop('checked',true).trigger('change');
      });
    },
    initColumn: function(){
      var _this = this;
      this.$head.find('th').each(function(index){
        var $this = $(this);
        var data = $this.data();
        data.priority = data.priority || 1;
        $this.toggleClass('hide',!!(data && data.hide));
      });
      this.$body.children().each(function(){
        var idStart = 0;
        $(this).children().each(function(index){
          var $cell = $(this);
          var data = _this.$head.find('th').eq(index).data();
          $cell.toggleClass('hide',!!(data && data.hide));
          $cell.addClass('priority'+(data && data.priority || 1));
        });
      });
    },
    createToolbar: function(){
      var _this = this;
      this.$toolbar = $(this._tollbarTemp());
      this.$toolbar.prepend(this.$actions);
      this.$tableWrap.before(this.$toolbar);
      
      this.createCheckGroup();
    },
    createCheckGroup: function(){
      var _this = this;
      this.$checkGroup = $('<ul class="ui-dropdown-menu ui-dropdown-impede">').appendTo(this.$toolbar.find('.ui-dropdown'));
      this.$head.find('th').each(function(index){
        var $this = $(this);
        var data = $this.data();
        $this.data('priority',data.priority || 1);
        data = $this.data();
        $this.addClass('priority'+data.priority);
        _.extend(data,{_text:$this.text(),_index:index});
        var $li = $(_this._liTemp(data));
        _this.$checkGroup.append($li);
      });
    },
    updateCheckState: function(){
      var _this = this;
      this.$checkGroup.children().each(function(index){
        var $this = $(this);
        var $checkbox = $this.find('input[type=checkbox]');
        var $th = _this.$head.find('th:eq('+$checkbox.val()+')');
        $checkbox.prop('checked',$th.css('display') != "none");
      });
    },
    createStickyHeader: function(){
      //ie6及以下浏览器不支持fixed定位，所以不创建stickyHeader
      if(browser < 7){
        this.$sticky = $('<div>');
        return;
      }
      var _this = this;
      var $el = this.$el.clone().addClass('responsivetable-clone').removeAttr('data-widget');
      this.$elClone = this.$elClone.add($el);
      this.$sticky = $('<div class="sticky-table-header">').append($el);
      this.$el.before(this.$sticky);
      this.$sticky.css({
        height:this.$head.height()+1,
        width:this.$tableWrap.width(),
        top: this.options.offset,
        display: 'none'
      });

      _this.$tableWrap.waypoint({
        offset: _this.options.offset,
        handler: function(direction){
          if(direction == 'down'){
            _this.$sticky.css({
              display: 'block',
              position: 'fixed'
            });
          }else{
            _this.$sticky.css({
              display: 'none'
            });
          }
        }
      });

    },
    refreshSticky: function(){
      Waypoint.refreshAll();
    },
    windowResize: _.debounce(function(){
      //ie 6,7,8修改列导致window resize修改列产生冲突,导致修改列失败，所以去除resize修改列功能
      if(browser > 8){
        this.$el.find('tr>.show').removeClass('show');
        this.$el.find('tr>.hide').removeClass('hide');
      }
      this.updateCheckState();
    },200),
    _liTemp: _.template([
      '<li><label><input type="checkbox" name="column" value="<%=_index%>" /> <%= _text %></label></li>'
    ].join('')),
    _tollbarTemp: _.template([
      '<div class="responsive-table-toolbar">',
        '<div class="btn-group">',
          '<button class="btn btn-sm" data-action="show-all" type="button">显示全部</button>',
          '<div class="ui-dropdown pull-right ml5">',
            '<i class="ui-dropdown-arrow"></i>',
            '<a class="btn btn-sm dropdown-toggle" data-widget="dropdown">显示&nbsp;&nbsp;</a>',
          '</div>',
        '</div>',
      '</div>'
    ].join('')),
    _wrapTemp: _.template([
      '<div class="<%= className %>"><div class="responsive-table"></div></div>'
    ].join(''))
  });
  $.extend(Responsivetable,{
    options : {
      offset: 0
    }
  });

  widget.install('responsivetable',Responsivetable);

  return Responsivetable

}));