/* build : 564493634@qq.com 2016-07-2611:05:55 */
define("alert",["jquery","underscore","jquery.dialog"],function(t,i,e){function n(i){this.options=t.extend({},arguments.callee.options,i),this.$el=t(this._template(this.options)),this.init(),this.events()}return t.extend(n.prototype,{init:function(){var i=this;this.dialog=new e(t.extend({el:this.$el,close:function(){i.$el.remove()}},this.options))},events:function(){var t=this;this.$el.on("click","[data-action=cancel]",function(){t.dialog.close()})},_template:i.template(['<div class="ui-alert" title="提示">','<div class="ui-alert-content"><%= content %></div>','<div class="ui-dialog-toolbar">','<a class="btn btn-sm" data-action="cancel">确 定</a>',"</div>","</div>"].join(""))}),t.extend(n,{options:{width:280,height:160,minWidth:280,minHeight:158,modal:!0,content:""}}),n});
!function(t){"function"==typeof define&&define.amd?define("jquery.confirm",["jquery","underscore","jquery.dialog","jquery.widget","jquery.preon"],t):t(jQuery,_,Dialog,widget)}(function(t,e,i,n){function a(e){t.extend(this,arguments.callee.options,e),this.dialogOpt=t.extend({},arguments.callee.dialogOpt,e),delete this.dialogOpt.el,this.init(),this.events()}function o(e){e.el.preon("click",function(i,n){if(!n||!n.confirmPass){i.stopImmediatePropagation(),i.preventDefault();{var o=this,s=t(this);new a(t.extend({pass:function(){1==t._data(o).events.click.length?window.open(o.href,o.target||"_self"):s.trigger("click",{confirmPass:!0})}},e))}}})}return t.extend(a.prototype,{init:function(){var e=this;this.$wrap=t(this._template(this)),this.dialog=new i(t.extend({el:this.$wrap,close:function(){e.$wrap.remove()}},this.dialogOpt))},events:function(){var t=this;this.$wrap.on("click","[data-action=pass]",function(){t.dialog.close(),t.pass()}),this.$wrap.on("click","[data-action=cancel]",function(){t.dialog.close(),t.cancel()})},_template:e.template(['<div class="ui-confirm" title="确认">','<div class="ui-confirm-content"><%= content %></div>','<div class="ui-dialog-toolbar">','<a class="btn btn-blue btn-sm" data-action="pass"><%= passName %></a>','<a class="btn btn-sm ml10" data-action="cancel"><%= cancelName %></a>',"</div>","</div>"].join(""))}),t.extend(a,{options:{content:"确定要继续操作吗？",passName:"确 定",cancelName:"取 消",pass:t.noop,cancel:t.noop},dialogOpt:{width:280,height:160,minWidth:280,minHeight:158,modal:!0}}),n.install("confirm",o),a});
!function(i){"function"==typeof define&&define.amd?define("jquery.dialog",["jquery","jquery.widget","jquery.ui"],i):i(jQuery,widget)}(function(i,t){var o=i.fn.dialog,e=function(t){this.options=i.extend(!0,{},arguments.callee.options,t),this.$el=i(this.options.el),o.call(this.$el,this.options),this._dialog=this.$el.data("uiDialog")};return i.extend(e.prototype,{setTitle:function(i){this._dialog.option({title:i})},setOption:function(i){this._dialog.option(i)},destroy:function(){this._dialog.destroy()},open:function(){this._dialog.open()},close:function(){this._dialog.close()}}),i.extend(e,{options:{}}),t.install("dialog",e),e});
define("input",["jquery"],function(e){function t(e){return/^INPUT|TEXTAREA$/.test(e.nodeName)}"oninput"in document||(e.event.special.input={setup:function(){var n=this;return t(n)?(e.data(n,"@oldValue",n.value),void e.event.add(n,"propertychange",function(t){e.data(this,"@oldValue")!==this.value&&e.event.trigger("input",null,this),e.data(this,"@oldValue",this.value)})):!1},teardown:function(){var n=this;return t(n)?(e.event.remove(n,"propertychange"),void e.removeData(n,"@oldValue")):!1}})});
!function(i){"function"==typeof define&&define.amd?define("jquery.loading",["jquery","underscore","jquery.widget"],i):i(jQuery,_,widget)}(function(i,t,e){function s(t){i.extend(this,arguments.callee.options,t),this.$el=i(this.el),this.init(),this.autoShow&&this.show()}return i.extend(s.prototype,{init:function(){(this.$el.width()<124||this.$el.height()<50)&&(this.className+=" ui-loading-small",this.$el.html("<span>"+this.$el.text()+"</span>")),this.$widget=this.$el.find(".ui-loading"),0==this.$widget.length&&(this.$widget=i(this._temp(this)).addClass(this.className)),this.$overlay=this.$widget.find(".ui-loading-overlay"),this.$box=this.$widget.find(".ui-loading-box")},show:function(){var i=this;this.isShow=!0,"absolute"!=this.$el.css("position")&&this.$el.addClass("ui-loading-parent"),this.$el.append(this.$widget),this._timer=setTimeout(function(){i.hide()},this.wait)},hide:function(){clearTimeout(this._timer),this._timer=null,this.isShow=!1,this.$el.removeClass("ui-loading-parent"),this.$widget.detach()},destroy:function(){this.hide(),this.$widget.remove(),this.$el.data("loading",null)},_temp:t.template(['<div class="ui-loading">','<div class="ui-loading-overlay"></div>','<div class="ui-loading-box"><i class="ui-loading-icon"></i><span class="ui-loading-text"><%= text %></span></div>',"</div>"].join(""))}),i.extend(s,{options:{type:"layer",text:"Loading...",autoShow:!0,duration:300,className:"",wait:1e4,easing:"easeOutCirc"}}),e.install("loading",s),s});
define("outer",["jquery","underscore"],function(t,e){var n,r;n=[],r=function(r){var o,u;o=r.target,u=t(r.target).parents().toArray(),e.each(n,function(n){if(n!=o&&-1==e.indexOf(u,n)){var r=t.Event("outer",{target:o});t.event.trigger(r,null,n)}})},t.event.special.outer={setup:function(){0==n.length&&t(document).on("click.outer",r),n.push(this)},teardown:function(){n=e.without(n,this),0==n.length&&t(document).off("click.outer",r)}}});
!function(n){"function"==typeof define&&define.amd?define("jquery.preon",["jquery","underscore"],n):n(jQuery,_)}(function(n,e){n.fn.extend({preon:function(t,r,i,u){var f=arguments;return u=null==i&&null==u?r:i,this.each(function(){var i;n.fn.on.apply(n(this),f);var o=n._data(this).events[t],a=e.findIndex(o,function(n){return n.handler==u}),d=o.splice(a,1)[0];i="string"==typeof r?0:o.delegateCount,o.splice(i,0,d)})}})});
!function(t){"function"==typeof define&&define.amd?define("jquery.prompt",["jquery","underscore","jquery.dialog","jquery.widget","input"],t):t(jQuery,_,Dialog,widget)}(function(t,i,e,n){function a(i){t.extend(this,arguments.callee.options,arguments.callee.dialogOpt,i),this.dialogOpt=t.extend({},arguments.callee.dialogOpt,{width:this.width,height:this.height,minWidth:this.minWidth,minHeight:this.minHeight,modal:this.modal}),this.init(),this.events()}return t.extend(a.prototype,{init:function(){var i=this;this.$wrap=t(this._template(this)),this.dialog=new e(t.extend({el:this.$wrap,close:function(){i.$wrap.remove()}},this.dialogOpt))},events:function(){var i=this;this.$wrap.find(".ui-prompt-input input").on("input",function(){i.value=t.trim(t(this).val())}),this.$wrap.on("click","[data-action=pass]",function(){i.dialog.close(),i.pass(i.value)}),this.$wrap.on("click","[data-action=cancel]",function(){i.dialog.close(),i.cancel()})},_template:i.template(['<div class="ui-prompt" title="<%= title %>">','<div class="ui-prompt-content">','<div class="ui-prompt-tips"><%= content %></div>','<div class="ui-prompt-input"><input type="text" value="<%= value %>"></div>',"</div>",'<div class="ui-dialog-toolbar">','<a class="btn btn-blue btn-sm" data-action="pass">确 定</a>','<a class="btn btn-sm ml10" data-action="cancel">取 消</a>',"</div>","</div>"].join(""))}),t.extend(a,{options:{title:"提示",content:"请输入...",value:"",pass:t.noop,cancel:t.noop},dialogOpt:{width:280,height:180,minWidth:280,minHeight:158,modal:!0}}),a});
!function(t){"function"==typeof define&&define.amd?define("sidebar",["jquery","underscore","jquery.waypoints"],t):t(jQuery,_)}(function(t,e){function i(e){t.extend(this,arguments.callee.options,e),this.$el=t(this.el),t(t.proxy(this.init,this))}return t.extend(i.prototype,{init:function(){this.$el=t(this._temp(this)),this.$el.appendTo(document.body),this.event()},event:function(){var e=t(document.body),i=t("a[data-action=gotop]",this.$el);e.waypoint({offset:this.offset,handler:function(t){i.toggleClass("hide","up"==t)}}),this.$el.on("click","a[data-action=gotop]",function(){t("html,body").animate({scrollTop:0},200)})},destroy:function(){this.$el.remove()},_temp:e.template(['<div class="ui-sidebar">','<a class="item hide" data-action="gotop" href="javascript:;"><i class="fa"></i></a>',"</div>"].join(""))}),t.extend(i,{options:{offset:"-50%"}}),i});
define("tips",["jquery","underscore","jquery.easing"],function(t,i){function e(i){t.extend(this,arguments.callee.options,i),this.init()}return t.extend(e.prototype,{init:function(){var i=this;this.$el=t(this._template(this)).appendTo(document.body);var e=parseInt(this.$el.css("marginTop"))||0,n={marginTop:e-this.offset,opacity:0},s={marginTop:e,opacity:1},a={marginTop:e+this.offset,opacity:0};this.$el.css(n).animate(s,{duration:this.duration,easing:this.easing}).delay(this.delay).animate(a,{duration:this.duration,easing:this.easing,done:function(){i.$el.remove()}})},_template:i.template(['<div class="ui-tips">','<div class="ui-tips-content ui-tips-<%= type %>"><%= content %></div>',"</div>"].join(""))}),t.extend(e,{options:{type:"info",content:"",offset:60,duration:300,delay:2600,easing:"easeOutCirc"}}),e});
//# sourceMappingURL=../sourcemaps/js/module.js.map