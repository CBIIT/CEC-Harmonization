(function($){ 
    Drupal.behaviors.skipNav = {
	attach: function (context, settings) {
	    $("#skip-link a").click(function () {
                $('#main-content').attr('tabIndex', -1).focus();
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
	    $('#block-facetapi-6tzh3vkoxmw4fmxjek42ct31rr37et3c .facetapi-facetapi-links #facetapi-link--46').each(function(){
		var text = $(this).html().replace('Study dataset', 'Cohort dataset');
		$(this).html(text);
	    });
	    }, 300);
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
})(jQuery);