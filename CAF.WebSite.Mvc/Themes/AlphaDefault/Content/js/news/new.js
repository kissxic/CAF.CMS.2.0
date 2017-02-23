//更多
$(document).ready(function(){
	$(".more").mouseover(function(){
		$(this).find('.moreb').addClass('onre');
		$(this).find('.more-con').css('visibility', 'visible');
	}).mouseout(function(){
		$(this).find('.moreb').removeClass('onre');
		$(this).find('.more-con').css('visibility', 'hidden');
	});
});
//搜索下拉
$(document).ready(function(){
	$(".select-box").mouseover(function(){
		$(this).addClass('offselect-box');
	}).mouseout(function(){
		$(this).removeClass('offselect-box')
	});
	//下拉文字
	$(".select-con li").each(function(){
		$(this).click(function(){
		 $("#search-txt").text($(this).text());
		 $(".select-box").removeClass('offselect-box');
		 $("#search-txt").attr("name",$(this).attr("name"));
		});
	});
});

/*详细文字数*/
$(document).ready(function(){
	$(".txt").each(function(){
		var this_obj = $(this).find("span");
		var this_txt = this_obj.text();
		var new_ttxt = this_txt.substr(0,46);
		if(this_txt.length>=46){
			this_obj.text(new_ttxt +"...");
		}
	})
});
/*tab*/
$(document).ready(function(){
	$(".info-nav li").not('.tabos').each(function(index){
		$(this).mouseover(function(){
			$(".info-left").eq(index).addClass("on-box");
			$(".info-left").eq(index).siblings().removeClass("on-box");
			$(this).addClass("on-tab");
			$(this).siblings().removeClass("on-tab");
		});
	});
});
