require(['jquery','browser','jquery.sliderbox','jquery.waypoints','jquery.easing','jquery.ui','jquery.widget'],function($,browser){
  $(function(){

    var $body = $(document.body);

    /**
     * header-bar
     */
    !(function(){
      var $headerLayer = $(".header-layer"),
          $form = $headerLayer.find('form'),
          $select = $form.find('.search-type-select')
          $search = $form.find('.search'),
          $btn = $form.find('.search-btn');

      $select.on('change',function(){
        var data = $select.find('option:selected').data();
        $form.attr('action',data.action);
        $search.attr('name',data.key);
      });
      $btn.on('click',function(){
        $form.submit();
      });
    })();

    /**
     * 首页box
     */
    !(function(){
      var $slider = $(".slider-box");
      $slider.sliderbox();
      $slider.append('<a href="javascript:;" class="arrow-left"></a><a href="javascript:;" class="arrow-right"></a>');
      $slider.on('click','.arrow-left',function(){
        $slider.sliderbox('prev');
      });
      $slider.on('click','.arrow-right',function(){
        $slider.sliderbox('next');
      });
    })();


    /**
     * 商城
     */
     /*brand*/
    +(function(){
      var $brand = $('.brand'),
          $slider = $brand.find('.sliders');

      $slider.sliderbox({
        control: false,
        hoverPause: false 
      });
      $brand.hover(function(){
        $slider.sliderbox('closeAuto');
      },function(){
        $slider.sliderbox('openAuto');
      });
      $brand.on('click','.arrow-left',function(){
        $slider.sliderbox('prev');
      });
      $brand.on('click','.arrow-right',function(){
        $slider.sliderbox('next');
      });
    })();

    /**
     * 侧边栏
     */
    +(function(){
      var $sidebar = $(".side-bar");
      var $main = $body.children('.main');
      var $goTop = $sidebar.find(".go-top");
      //返回顶部
      $body.waypoint({
        offset: "-100%",
        handler: function(direction){
          $goTop.toggleClass('hide',direction == "up");
        }
      });
      $sidebar.on('click','.go-top',function(){
        $("html,body").animate({scrollTop:0},200);
      });

      //分享
      $sidebar.on('click','.share',function(){
        if(!window.bShare) return;
        bShare.more();
      });

      //客服
      $body.on('click','.kf-toggle',function(){
        $(".zxkf").toggleClass('hide');
      });

    })();
  })
  /**
   * 手机兼容
   */
  require(['device'],function(device){
    if(device.mobile()){
      require(['fastclick'],function(){
        var $dbRightBox = $('<div data-widget="tab">'); 
        $dbRightBox.append('<a href="javascript:;">热门数据库</a>')
        .append($('.box-db .box-right .remen'))
        .append('<a href="javascript:;">最新上线数据库</a>')
        .append($('.jjsx'));
        $('.box-db').append($dbRightBox);
        window.$dbRightBox = $dbRightBox;

        var $mmenuLayer = $('.m-menu-layer');
        var $msearchLayer = $('.m-search-layer');

        $(document).on('click','.m-page .close',function(){
          $(this).closest('.m-page').removeClass('open');
        });
        $('.show-menu').on('click',function(){
          $mmenuLayer.addClass('open');
        });
        $('.show-search').on('click',function(){
          $msearchLayer.addClass('open');
        });

        var $form = $msearchLayer.find('form'),
        $tabnav = $form.find('.tab-nav'),
        $search = $form.find('.search'),
        $btn = $form.find('.search-btn');

        $tabnav.on('click','>a',function(){
          var $this = $(this);
          $this.addClass('active').siblings().removeClass('active');
          var data = $this.data();
          $form.attr('action',data.action);
          $search.attr('name',data.key);
        });
        $btn.on('click',function(){
          $form.submit();
        });
        $tabnav.find('>a:eq(0)').trigger('click');

        var $mnavBar = $('.m-nav-bar');
        $mnavBar.on('click','>a',function(e){
          var $this = $(this);
          var $target = $($this.attr('href'));
          $(document.body).animate({scrollTop:$target.offset().top-60},200);
          return false;
        });

      });
        
    }else{
      /**
       * 数据库
       */
      +(function(){
        var $boxDb = $('.box-db');
        var $pinc = $('.pinc',$boxDb);
        var $list = $('.list',$pinc);
        $list.sliderbox({
          displayNumber: 2,
          control:false
        });
        $pinc.hover(function(){
          $list.sliderbox('closeAuto');
        },function(){
          $list.sliderbox('openAuto');
        });
        $pinc.on('click','.prev',function(){
          $list.sliderbox('prev');
        });
        $pinc.on('click','.next',function(){
          $list.sliderbox('next');
        });
      })();
    }
  });







  /**
   * 时间轴
   */
  (function(){
    var $footer = $('.footer');
    var $timeline = $footer.find('.timeline');
    var $list = $timeline.find('.list');
    var $right = $timeline.find('.right a');
    var $left = $timeline.find('.left a');

    var total = $list.children().length;
    var current = 0;
    function selectTarget(num){
      var offset;
      if(num!=undefined){current = num;}
      if(current>total-1){
        current=total-1;
      }
      if(current>total-2){        
        $right.hide();
      }else{
        $right.show();
      }

      if(current<0){
        current=0;
      }
      if(current<1){
        $left.hide();
      }else{
        $left.show();
      }

      if(current>total-3){
        offset = 8-total;
      }else if(current>5){
        offset = 5-current
      }else{
        offset = 0;
      }
      var $target = $list.children().eq(current);
      $target.addClass('hover').siblings().removeClass('hover');
      $list.stop(true).animate({left:offset*140});
    }
    $right.on('click',function(){
      current+=1;
      selectTarget();
    });
    $left.on('click',function(){
      current-=1;
      selectTarget();
    });
    $list.on('click','.circle',function(){
      selectTarget($(this).parent().index());
    });
    $list.on('mouseover','.item',function(){
      $(this).addClass('hover').siblings().removeClass('hover');
    });

    selectTarget(total-1);

  })();
  




});
