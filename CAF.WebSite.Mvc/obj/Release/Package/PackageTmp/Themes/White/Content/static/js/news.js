require(['jquery','utils'],function($,utils){

  /**
   * 设置dialog默认尺寸
   */
    require(['jquery.dialog'],function(Dialog){
      $.extend(Dialog.options,{
        width: 600,
        height: 520
      });
    });

  /**
   * 设置默认时间格式
   */
  require(['jquery.datepicker'],function(Datepicker){
    Datepicker.options.dateFormat = "yy-mm-dd";
    Datepicker.options.changeYear = true;
  });

  /**
   * body ready
   */
  $(function(){

    var $body = $(document.body);

    /**
     * 表单clearInput清除功能
     */
    
    require(['jquery.clearInput'],function(){
      $('.main>.search-tab input[type=text]:not([data-widget=datepicker])').clearInput();
    });

    /**
     * 侧边栏
     */
    require(['sidebar','jquery.widget'],function(Sidebar){
      var sidebar = new Sidebar();
      sidebar.$el.widget();
    });

    function updateNum($target,number){
      var $target = $($target);
      var num = parseInt($target.text())||0;
      num+=number;
      $target.text(num||'');
    }

    /**
     * 订阅
     */
    $body.on('click','.action-rss',function(){
      var $this;
      $this = $(this);
      $.post("/User/toRss",$(this).data()).done(function(res){
        if(res==1){
          utils.success("订阅成功！");
          $this.removeClass('btn-blue').find('span').text('已订阅');
        }else{
          utils.success("取消成功！");
          $this.addClass('btn-blue').find('span').text('订阅');
        }
      });
    });

    /**
     * 收藏
     */
    $body.on('click','.action-collect',function(){
      var $this;
      $this = $(this);
      $.post("/User/toCollect",$this.data()).done(function(res){
        if(res==1){
          utils.success("收藏成功！");
          $this.removeClass('btn-red').find('span:eq(0)').text('已收藏');
          updateNum($this.find('.total'),1);
        }else{
          utils.success("取消成功！");
          $this.addClass('btn-red').find('span:eq(0)').text('收藏');
          updateNum($this.find('.total'),-1);
        }
      });
    });

    /**
     * 点赞
     */
    $body.on('click','.action-praise',function(){
      var $this;
      $this = $(this);
      $.post("/User/toPraise",$(this).data()).done(function(res){
        if(res==1){
          utils.success("点赞成功！");
          $this.removeClass('btn-green').find('span:eq(0)').text('已赞');
          updateNum($this.find('.total'),1);
        }else{
          utils.success("取消成功！");
          $this.addClass('btn-green').find('span:eq(0)').text('点赞');
          updateNum($this.find('.total'),-1);
        }
      });
    });

    /**
     * 检查登录
     */
    require(['jquery.preon'],function(){
      $body.preon('click','.action-checklogin,.action-primsg,.action-rss,.action-collect,.action-praise',function(e){
        if($('header .user').length == 0){
          utils.alert("您还没有登录!");
          e.stopImmediatePropagation();
          e.preventDefault();
        }
      });
    });

    /**
     * 评论
     */
    require(['moment','TweenMax','jquery.loading'],function(moment,TweenMax){
      var $comment = $('#comment');
      var $commentList = $(".comment-list",$comment);
      var $commentForm = $('.comment-form',$comment);
      var $noComment = $('.no-comment',$comment);
      var $total = $('.total-number',$comment);
      var $loadmore = $comment.find('.comment-load-more>a');

      var commentTEMP = _.template([
        '<div class="item">',
          '<i class="tag">1#</i>',
          '<div class="user">',
            '<a href="#" class="img"><img src="<%=head%>" alt=""></a>',
          '</div>',
          '<div class="info">',
            '<a href="#"><%=name%></a> ',
            '<span class="cl-gray"><%=date%></span>',
          '</div>',
          '<div class="content"><%=content%></div>',
          '<div class="actions">',
          '</div>',
        '</div>'
      ].join(''));
      $commentForm.on('submit',function(){
        var $this = $(this);
        var data = $this.serializeObject();
        if(!$.trim(data.content)){
          utils.alert("评论内容不能为空!");
          return false;
        }
        if(data.content.length>300){
          utils.alert("评论内容不能超过300个字符!");
          return false;
        }
        $this.find('textarea[name=content]').val('');
        $.post($this.attr('action'),data).done(function(){
          utils.success("评论成功!");
          var $item;
          $commentList.children().each(function(){
            var $this = $(this);
            var $tag = $this.find('.tag')
            var num = parseInt($tag.text())+1;
            $tag.text(num+'#');
          });

          var opt={
            name: $commentForm.find('.user .name').text(),
            head: $commentForm.find('.img img').attr('src'),
            date: moment().format('YYYY-MM-DD'),
            content: data.content
          };
          $item = $(commentTEMP(opt));
          $noComment.hide();
          updateNum($('.action-tocomment .total'),1);
          $total.html('('+(parseInt($total.text().match(/\d+/)[0])+1)+')');
          $commentList.prepend($item);
        });
        return false;
      });

      $loadmore.on('click',function(e){
        var $this = $(this);
        var data = $this.data();
        e.preventDefault();
        e.stopPropagation();
        if(!data.p){
          data.p = 1;
        }
        data.p++;
        if(data.p > Math.ceil(data.total/data.size)){
          $this.text('没有更多了');
          return;
        }
        $this.loading();
        $.get($this.attr('href'),{id:data.id,p:data.p},function(res){
          var $res = $(res);
          $loadmore.loading('hide');
          $list = $res.find('.comment-list>');
          $list.each(function(){
            var $item = $(this);
            $commentList.append($item);
            $item.find('.tag').text($commentList.children().length+'#');
          });
          TweenMax.staggerFrom($list,0.2,{right:-500,alpha:0},0.1);
        });

      });

    });
    

  }); 

});


/**
 * 手机兼容
 */
require(['device'],function(device){
  if(device.mobile()){
    require(['news.mobile'],function(){});
  }
});