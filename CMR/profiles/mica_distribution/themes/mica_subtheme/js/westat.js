(function($){ 
    Drupal.behaviors.skipNav = {
	attach: function (context, settings) {
	    $("#skip-link a").click(function () {
                $('#main-content').attr('tabIndex', -1).focus();
            });
	    if ($(window).width() > 980) {
		$('body').css({'padding-top': $('#navbar').outerHeight() + 6});
	    }
	}
    }
    
    Drupal.behaviors.utilityFunctions = {
	attach: function (context, settings) {
    //utility functions
	    function goToUrl(url){
		    window.location.href = url;
	    }
	    function getUrlVars(){
		var vars = [], hash;
		var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
		for(var i = 0; i < hashes.length; i++)
		{
		    hash = hashes[i].split('=');
		    vars.push(hash[0]);
		    vars[hash[0]] = hash[1];
		}
		return vars;
	    }
	    function scrollTo(){
		    // scroll to top after opening coming from
		    var targetOffset = $('#page-wrapper').offset().top - $("#header").outerHeight(true);
		    $('body, html').animate({
			    scrollTop: targetOffset
		    }, 30);
	    }
	    $('.cancel-go-home').click(function(){
		goToUrl('/');
	    });
	}
    }
    
    Drupal.behaviors.loginBlock = {
	attach: function (context, settings) {
            $('#block-mica-bootstrap-config-bootstrap-login a').click(function(e) {;
	       e.preventDefault();
            });
	    
	}
    }
    
    Drupal.behaviors.menuHover = {
	attach: function (context, settings) {
	    $('#block-system-main-menu').on('focus', ".dropdown-submenu", function() {
		$(this).find('.dropdown-menu')
		    .css({'display':'block', 'left':'auto', 'right':'100%'});
            });
	}
    }
    
    Drupal.behaviors.textOver = {
	attach: function (context, settings) {
	    $('#block-facetapi-oy34mxn1nxjl14ut9rodwqerveb48mjo .block-title a').text('Cohort Design');
	    $('#block-facetapi-52fiqdeixccrw6l0oshzx1fbluxvfrxq .block-title a').text('Cohort');
	    $('.node-variable .field-name-title-field > .field-label').html('Cohorts:&nbsp;');
	    $('.node-study-variable-attributes .field-name-field-sva-study > .field-label').html('Cohort:&nbsp;');
	    $('#block-mica-studies-study-general-design .field-name-field-design > .field-label').html('Cohort design:&nbsp;');
	    $('.field-name-field-dataset-type .field-item').text(function () {
		return $(this).text().replace('Study dataset', 'Cohort dataset');
	    });
	    setTimeout(function(){
	    $('.region-sidebar-first li').each(function(){
		$(this).html(function () {
		    var text = $(this).html();
		    if (text.indexOf('Study') > -1) {
			text = text.replace('Study', 'Cohort');
			$(this).html(text);
		    }
		    //var text = $(this).html().replace('Study dataset', 'Cohort dataset');
		    
		});
	    });
	    }, 2000);
	//    
	//    $('#block-facetapi-6tzh3vkoxmw4fmxjek42ct31rr37et3c .facetapi-facetapi-links #facetapi-link--46').each(function(){
	//	var text = $(this).html().replace('Study dataset', 'Cohort dataset');
	//	$(this).html(text);
	//    });
	//    
	}
    }
    
    Drupal.behaviors.hideColumns = {
	attach: function (context, settings) {
	    $('.node-type-study .pop-dce thead th:nth-child(2)').addClass('noBor');
	    $('.node-type-study .pop-dce thead th:nth-child(3)').addClass('toHide');
	    $('.node-type-study .pop-dce thead th:nth-child(4)').addClass('toHide');
	    $('.node-type-study .pop-dce tbody td:nth-child(2)').addClass('noBor');
	    $('.node-type-study .pop-dce tbody td:nth-child(3)').addClass('toHide');
	    $('.node-type-study .pop-dce tbody td:nth-child(4)').addClass('toHide');
	    
	   $('.views-table tbody tr').each(function(){
	    var td = $(this).find('td');
	    if (td.length == '') {
		$(this).remove();
	    }
	   });
	}
    }
    
    Drupal.behaviors.noClick = {
	attach: function (context, settings) {
	    $('#block-nice-menus-1 .nice-menu > .menuparent > a').click(function(e){e.preventDefault();});
	    $('#uMTitle').click(function(e) {
		e.stopPropagation();
	    });
	    $(document).click(function(e) {
		if ($('#block-westat-nice-menus-2').length && $('#block-westat-nice-menus-2 .block-content').attr('aria-hidden') == 'false') {
		    $('#block-westat-nice-menus-2 .block-content')
			.attr('aria-hidden', 'true')
			.hide();
		    $('#block-westat-nice-menus-2 .collapsiblock').addClass('collapsiblockCollapsed');
		    $('#block-westat-nice-menus-2 .collapsiblock > a').attr('aria-expanded', 'false');
		}
	    });
	}
    }
    
    Drupal.behaviors.filterSelect = {
	attach: function (context, settings) {
	    //var url = window.location.href.split('%');
	    //var urlLast = url[url.length-1];
	    $('[id^="block-facetapi"] a').click(function() {
		$.cookie("activeFilter", $(this).attr('id'), { expires : 1, path: '/' });
		//var l = $(this).attr('href').split('%');
		//var lLast = l[l.length-1];
		//if (lLast == urlLast) {
		//   $(this).focus();
		//}
	    });
	    $(window).load(function(){
		var cookieValue = $.cookie("activeFilter");
		var facet = $('[id^="block-facetapi"] .block-content');
		if (facet.length) {
		    facet.find('a').each(function(){
			if (($(this).attr('id') == cookieValue) && ($(this).attr('class').indexOf('facetapi-active') > -1)) {
			    $(this).focus();
			}
		    })
		}
	    })
	}
    }
    
    Drupal.behaviors.tou = {
	attach: function (context, settings) {
	    $('.form-item-terms-of-use a, .touClose, .alert a[href*="cmr-user-agreement"]').click(function(e){
		e.preventDefault();
		$('#touBlock').toggleClass('open');
		setTimeout(function(){
		    $('#touBlock.open').attr('tabIndex', -1).focus();
		}, 50);
		$("#touBlock.open").on('keydown', '.touClose', function(e) { 
		    var keyCode = e.keyCode || e.which; 
		    if (keyCode == 9) { 
		      e.preventDefault(); 
		      $(this).parents("#touBlock").focus();
		    } 
		});
		$("#touBlock .block-content > div").slimScroll({
		    height: '460px',
		    alwaysVisible: true,
		    color: '#222',
		    size: '8px',
		});
	    });
	    
	}
    }
    
    Drupal.behaviors.dataH = {
	attach: function (context, settings) {
	    $('.node-type-dataset .nice-menu li.menu-2361').addClass('active-trail active');
	    $('.node-type-dataset .nice-menu li.menu-2361 > a').addClass('active');
	    //setTimeout(function(){
		if ($('#harmonization_overview').length) {	
		    var tableOffset = $("#harmonization_overview").offset().top;
		    var target = $('#harmonization_overview > thead');
		    var target_children = target.find('tr').children();
		    var $header = target.clone();
		    $($header).find('tr').children().width(function(i,val) {
		        return target_children.eq(i).width();
		    });
		    var $fixedHeader = $("#header-fixed")
			.css({'top': $('#navbar').outerHeight(), 'width': $('#harmonization_overview').outerWidth()})
			.append($header);
		    
		    $(window).bind("scroll", function() {
			var offset = $(this).scrollTop();
		    
			if (offset >= tableOffset && $fixedHeader.is(":hidden")) {
			    $fixedHeader.show();
			}
			else if (offset < tableOffset) {
			    $fixedHeader.hide();
			}
		    });
		}
	    //}, 100);
	}
    }
    
    Drupal.behaviors.loadingBlock = {
	attach: function (context, settings) {
	    $('a[href$="/dataset-harmonization"]').click(function(){
		//var h = $(this).attr('href').split('/');
		$('#loading-block').show();
	    });
	    window.onbeforeunload = function(e) {
		if ($('.page-node-dataset-harmonization').length) {
		    $('#loading-block').show();
		}
	    }
	    $('.page-node-dataset-harmonization #loading-block')
			.show()
			.fadeOut( 3500 );
	}
    }
    
    Drupal.behaviors.profileSynch = {
	attach: function (context, settings) {
	    $('input[id^="edit-profile-main-field-profile-first-name"]').change(function() {
		if ($('input[id^="edit-field-first-name"]').length) {
		    $('input[id^="edit-field-first-name"]').val($(this).val());
		}
	    });
	    $('input[id^="edit-profile-main-field-profile-first-name"]').keyup(function() {
		if ($('input[id^="edit-field-first-name"]').length) {
		    $('input[id^="edit-field-first-name"]').val($(this).val());
		}
	    });
	    
	    $('input[id^="edit-profile-main-field-profile-last-name"]').change(function() {
		if ($('input[id^="edit-profile-main-field-profile-last-name"]').length) {
		    $('input[id^="edit-field-last-name"]').val($(this).val());
		}
	    });
	    $('input[id^="edit-profile-main-field-profile-last-name"]').keyup(function() {
		if ($('input[id^="edit-profile-main-field-profile-last-name"]').length) {
		    $('input[id^="edit-field-last-name"]').val($(this).val());
		}
	    });    
	}
    }
    
    Drupal.behaviors.updateInfo = {
	attach: function (context, settings) {
	    $('#updateinfo .notetrigger').click(function(e) {
		$(this).parent('.field-content').toggleClass('active-detail');
		e.preventDefault();
		e.stopPropagation();
	    });
	    $(document).click(function() {
		if ($('.field-content.active-detail').length) {
		    $('.field-content.active-detail').removeClass('active-detail');
		}
	    });
	}
    }
    Drupal.behaviors.hhsWarning = {
	attach: function (context, settings) {	    
	    // Submiting login form
	    var currentForm;
	    $('#user-login-form').submit(function(e){
		currentForm = this;
		$("#dialog-confirm")
		    .html('<p>This warning banner provides privacy and security notices consistent with applicable federal laws, directives, ' +
			  'and other federal guidance for accessing this Government system, which includes (1) this computer network, (2) all ' +
			  'computers connected to this network, and (3) all devices and storage media attached to this network or to a computer on this network.</p>' +
			    '<p>This system is provided for Government-authorized use only.</p>' +
			    '<p>Unauthorized or improper use of this system is prohibited and may result in disciplinary action and/or civil and criminal penalties.</p>' +
			    '<p>Personal use of social media and networking sites on this system is limited as to not interfere with official work duties and is subject to monitoring.</p>' +				
			    '<p>By using this system, you understand and consent to the following:</p>' +				
			    '<p>The Government may monitor, record, and audit your system usage, including usage of personal devices and email systems for ' +
			    'official duties or to conduct HHS business. Therefore, you have no reasonable expectation of privacy regarding any communication ' +
			    'or data transiting or stored on this system. At any time, and for any lawful Government purpose, the government may monitor, ' +
			    'intercept, and search and seize any communication or data transiting or stored on this system.</p>' +				
			    '<p>Any communication or data transiting or stored on this system may be disclosed or used for any lawful Government purpose.</p>')
		    .dialog({
			dialogClass: "hhs-warning",
			resizable: false,
			closeText: "x",
			modal: true,
			title: "HHS Warning",
			height: 550,
			width: 900,
			buttons: {
			  "Proceed": function () {
			    currentForm.submit();
			  },
			    "Cancel": function () {
				$(this).dialog('close');
				currentForm.focus();
			    }
			}
		    });
		    $(".ui-dialog-titlebar-close").html('x');
		return false;
	    
		$('.ui-dialog-titlebar-close').click(function(){
		    currentForm.focus();
		});
		$(".ui-dialog").attr('tabIndex', -1).focus();
	    });
	}
    }
})(jQuery);