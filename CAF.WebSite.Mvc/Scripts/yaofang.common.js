(function ($, window, document, undefined) {

	window.setLocation = function (url) {
		window.location.href = url;
	}

	window.OpenWindow = function (query, w, h, scroll) {
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;

		// TODO: (MC) temp only. Global viewport is larger now.
		// But add this value to the callers later.
		h += 100;

		winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
		if (scroll) winprops += ',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}

	window.htmlEncode = function (value) {
		return $('<div/>').text(value).html();
	}

	window.htmlDecode = function (value) {
		return $('<div/>').html(value).text();
	}

	window.displayNotification = function (message, type, sticky, delay) {
		if (window.EventBroker === undefined || window._ === undefined)
			return;

		var notify = function (msg) {
			EventBroker.publish("message", {
				text: msg,
				type: type,
				delay: delay || 5000,
				hide: !sticky
			})
		};

		if (_.isArray(message)) {
			$.each(message, function (i, val) {
				notify(val)
			});
		}
		else {
			notify(message);
		}
	}


	window.createCircularSpinner = function (size, active, strokeWidth, boxed, white) {
	    var spinner = $('<div class="spinner"></div>');
	    if (active) spinner.addClass('active');
	    if (boxed) spinner.addClass('spinner-boxed').css('font-size', size + 'px');
	    if (white) spinner.addClass('white');

	    if (!_.isNumber(strokeWidth)) {
	        strokeWidth = 6;
	    }

	    //var svg = '<svg style="width:{0}px; height:{0}px" viewBox="0 0 64 64"><circle cx="32" cy="32" r="{1}" fill="none" stroke-width="{2}" stroke-miterlimit="10"></circle></svg>'.format(size, 32 - strokeWidth, strokeWidth);
	    //spinner.append($(svg));

	    return spinner;
	}

    // on document ready
	$(function () {

		//if (!Modernizr.csstransitions) {
		//	$.fn.transition = $.fn.animate;
		//}

		// adjust pnotify global defaults
		if ($.pnotify) {
			$.extend($.pnotify.defaults, {
				history: false,
				animate_speed: "fast"
			});
		}

		// global notification subscriber
		if (window.EventBroker && window._ && $.pnotify) {
			var stack_bottomright = { "dir1": "up", "dir2": "left", "firstpos1": 25, "firstpos2": 25 };
			//var stack_topright = { "dir1": "down", "dir2": "left", "firstpos1": 60 };
			EventBroker.subscribe("message", function (message, data) {
				var opts = _.isString(data) ? { text: data } : data;
				opts.stack = stack_bottomright;
				opts.addclass = "stack-bottomright";
				$.pnotify(opts);
			});
		}

		// Handle ajax notifications
		$(document)
			.ajaxSuccess(function (ev, xhr) {
				var msg = xhr.getResponseHeader('X-Message');
				if (msg) {
					displayNotification(msg, xhr.getResponseHeader('X-Message-Type'));
				}
			})
			.ajaxError(function (ev, xhr) {
				displayNotification(xhr.responseText, "error");
		});

    });

})( jQuery, this, document );

