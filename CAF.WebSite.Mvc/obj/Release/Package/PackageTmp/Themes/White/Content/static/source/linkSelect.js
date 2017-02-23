/**
 * 级联下拉选择(已和药智数据耦合较深，其它项目慎用)
 * 参数
 * names : 分别的字段名，字符串(用逗号隔开)或数组(每一个的名字)
 * default : 默认值，字符串或数组，同理names参数
 * list : [
 *          {
 *            name:"",
 *            value:"",
 *            list:[{name:"",value:""}]
 *          }
 *        ]
 * src : 远程拉取数据,数据格式为list
 * 
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.linkSelect',['jquery','underscore','jquery.widget','jquery.dropdownSelect'],factory);
  } else {
    factory( jQuery,_,widget );
  }
}(function($,_,widget){
  var LinkSelect = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    var $el = $(opt.el);
    this.options.className = $el.attr('class');
    if(!this.options.defaults && typn" && define.amd ) {
    define('jquery.pagination',['jquery','underscore','jquery.widget','queryString'],factory);
  } else {
    factory( jQuery,_,widget,queryString );
  }
}(function($,_,widget,queryString){
  function Pagination(opt){
    this.$el = $(opt.el);
    var _data = this.$el.data(),data = {};
    //对药智数据的参数兼容
    _.mapObject(_data,function(v,k){
      switch(k){
        case 'page': {
          data.currentPage = parseInt(v);
          break;
        }
        case 'size': {
          data.pageSize = parseInt(v);
          break;
        }
        case 'max': {
          data.maxPage = parseInt(v);
          break;
        }
        default: {
          data[k] = v;
        }
      }
    });
    this.options = $.extend(this,arguments.callee.options,data,opt);
    this.init();
  }

  $.extend(Pagination.prototype,{
    init: function(){
      this.$el.addClass('pagination');
      this._events();
      this.render();
    },
    setOptions: function(opt){
      $.extend(this,opt);
      var pageChange = $.Event('pageChange',{page : this.currentPage});
      this.$el.trigger(pageChange);
      this.render();
    },
    setPage: function(number){
      this.setOptions({currentPage:number});
      var href = this._createHref({page:number});
      //处理hash兼容
      if(href.search("#") == 0){
        location.hash = href.substring(1);
      }else{
        location.href = href;
      }
    },
    setPageSize: function(number){
      this.setOptions({pageSize:number});
      this.setPage(this.currentPage);
    },
    render: function(){
      var self = this;
      var totalPage = Math.ceil(this.total/this.pageSize);
      if(this.maxPage && this.maxPage<totalPage){
        totalPage = this.maxPage;
      }
      var items = [];
      this.$el.empty();
      if(totalPage!=0 && this.currentPage>totalPage){
        this.setPage(totalPage);
        return;
      };
      //添加当前页
      items.push({page: this.currentPage,html: this.currentPage,style:'current'});

      //添加显示条数
      _.times(this.displayEdges,_.bind(function(index){
        index = index+1;
        var prev = this.currentPage-index,
            next = this.currentPage+index;
        if(prev > 0){
          items.unshift({page: prev,html: prev});
        }
        if(next < totalPage+1){
          items.push({page: next,html: next});
        }
      },this));
      
      //添加边缘
      _.times(this.edges,_.bind(function(index){
        index = this.edges-index;
        var prevStart = this.currentPage-this.displayEdges;
        var nextStart = this.currentPage+this.displayEdges;
        var prev = index,
            next = totalPage+1-index;

        if(prev<prevStart){
          //添加省略
          if(index == this.edges && (prevStart>1+index)){
            items.unshift({page: 0,html: this.ellipseText,style:'ellipse'});
          }
          items.unshift({page: prev,html: prev});
          
        }
        if(next>nextStart){
          //添加省略
          if(index == this.edges &&(nextStart<totalPage-index)){
            items.push({page: 0,html: this.ellipseText,style:'ellipse'});
          }
          items.push({page: next,html: next});
        }

      },this));
      //上一页
      if(this.currentPage!=1){
        items.unshift({page: this.currentPage-1,html: this.prevHtml});
      }
      //下一页
      if(this.currentPage!=totalPage){
        items.push({page: this.currentPage+1,html: this.nextHtml});
      }

      //使用items列表生成节点
      var $els = this._createElements(items);
      this.$el.append($els);

      //附加功能
      this.showPages && this.$el.append('<span class="total-page">共 '+totalPage+' 页</span>');
      this.showNums && this.$el.prepend('<span class="total-nums">共 '+this.total+' 条</span>');
      this.skipPage && this._skipPage();
      this.pageSizeSelect && this._pageSizeSelect();
    },
    _events: function(){
      var self = this;
      this.$el.on('click','.page',function(){
        var $this = $(this);
        if($this.hasClass('ellipse')) return;
        if($this.hasClass('current')) return;
        self.setPage($this.data('page'));
        return false;
      });

      this.$el.on('keydown','.skip',function(e){
        if(e.keyCode != 13) return;
        skip();
      });
      this.$el.on('click','.skip-page .jump',skip);
      this.$el.on('change','.page-size',function(){
        self.setPageSize($(this).val());
      });
      function skip(){
        var $skip = self.$el.find('.skip');
        var totalPage = Math.ceil(self.total/self.pageSize);
        var val = parseInt($skip.val());
        if(!isNaN(val) && val<=totalPage){
          self.setPage(val);
        }else{
          $skip.val('');
        }
      }
    },
    _createHref: function(opt){
      var href,params;
      href= _.template(this.href)(opt);
      params= queryString.parse(queryString.extract(href));
      params.pageSize= this.options.pageSize;
      return href.split('?')[0]+'?'+queryString.stringify(params);
    },
    _createElements: function(items){
      var self = this,$els = $();
      ($.type(items) == "object") && (items = [items]);
      $.each(items,function(index,item){
        item.href = item.page ? self._createHref({page:item.page}) : "javascript:;";
        var opt = $.extend({style:'',href:"javascript:;"},item);
        var $el = $(self._elementTemp(opt));
        $els = $els.add($el);
        $el.data('page',opt.page);
      });
      return $els;
    },
    _skipPage: function(){
      var $el = $(this._skipPageTemp({
        items: this.pageSizeArray,
        pageSize: this.pageSize
      }));
      this.$el.append($el);
    },
    _pageSizeSelect: function(){
      var $el = $(this._pageSizeSelectTemp({
        items: this.pageSizeArray,
        pageSize: this.pageSize
      }));
      this.$el.append($el);
    },
    _skipPageTemp: _.template([
      '<span class="skip-page">跳转到 <input type="text" class="skip" value="" /> 页 <a href="javascript:;" class="jump">跳转</a></span>'
    ].join('')),
    _pageSizeSelectTemp: _.template([
      '<select class="page-size">',
        '<% _.each(items,function(item,index){ %>',
        '<option value="<%= item %>" <% if(item == pageSize){ %>selected="selected"<%}%> ><%= item %></option>',
        '<% }); %>',
      '</select>'
    ].join('')),
    _elementTemp: _.template([
      '<a href="<%= href%>" class="page <%= style %>"><%= html %></a>'
    ].join(''))
  });

  $.extend(Pagination,{
    options : {
      currentPage: 1,
      pageSize: 10,
      total: 0,
      edges: 1,
      displayEdges: 2,
      prevHtml: "上一页",
      nextHtml: "下一页",
      href: "#page-<%= page %>",
      ellipseText: "&hellip;",
      pageSizeSelect: false,
      pageSizeArray: [10,20,50,100],//分页选择控制
      skipPage: false,//是否允许跳转到指定页
      showPages: false,//是否显示总页数
      showNums: false,//是否显示总条数
      maxPage: 0,//最大显示页数
      onChange: $.noop
    }
  });

  widget.install('pagination',Pagination);

  return Pagination;

}));
