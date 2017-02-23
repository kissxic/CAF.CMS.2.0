/**
 * jquery validate
 */
(function( factory ) {
  if ( typeof define === "function" && define.amd ) {
    define('jquery.validateForm',['jquery','jquery.widget','jquery.validate'],factory);
  } else {
    factory( jQuery,widget );
  }
}(function($,widget){


  var ValidateForm = function(opt){
    this.options = $.extend(true,{},arguments.callee.options,opt);
    this.$el = $(this.options.el);
    if(this.$el[0].nodeName.toLowerCase() == "form"){
      this.$form = this.$el;
    }else{
      this.$form = this.$el.find('form');
    }
    this.$form.validate(opt);
  }

  $.extend(ValidateForm.prototype,{});
  $.extend(ValidateForm,{});

  widget.install('validateForm',ValidateForm);

  return ValidateForm

}));