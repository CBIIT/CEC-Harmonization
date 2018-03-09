
function init(dateFormat, isMobileOrTablet) {
    
    //remove locaStorage keys from older versions of awesome; you can modify  ppk (awe.ppk = "myapp1_"), current value is "awe2_"
    try {
        for (var key in localStorage) {
            if (key.indexOf("awe") == 0) {
                if (key.indexOf(awe.ppk) != 0) {
                    localStorage.removeItem(key);
                }
            }
        }
    } catch(err) {}

    //by default jquery.validate doesn't validate hidden inputs
    if ($.validator) $.validator.setDefaults({
        ignore: []
    });

    setjQueryValidateDateFormat(dateFormat);

    // don't focus first input on mobile
    if (isMobileOrTablet) {
        awe.ff = function (o) {
            o.p.d.find(':tabbable').blur();//override jQueryUI dialog autofocus
        };
    }
}

//on document ready
function documentReady(root, popupMod) {
    layPage();
    
    handleAnchors();
    consistentSearchTxt();
    $(document).ajaxComplete(consistentSearchTxt);
    $(document).on('aweload', 'table.awe-ajaxlist', wrapLists);

    $(window).on('resize', layPage);
    $('#btnMenuToggle').click(function () {
        MenuToggle($('#demoMenu').is(':visible'), 1);
    });

    //parsing the unobtrusive attributes when we get content via ajax
    $(document).ajaxComplete(function () {
        $.validator.unobtrusive.parse(document);
    });

    $('pre').addClass('prettyprint');
    prettyPrint();

    //show code directive
    $('.code').hide().before('<br/>').before($('<span class="shcode">show code</span>').click(function () {
        var btn = $(this);
        btn.toggleClass("hideCode showCode");

        if (btn.hasClass("hideCode")) {
            btn.html("hide code");
            $(this).next().fadeIn();
        } else {
            btn.html("show code");
            $(this).next().fadeOut();
        }
    }));

    /*beginpopup*/
    var jqueryUIpopup = awe.popup;

    awe.popup = function (o) {
        if (popupMod == 'inline' || o.tag && o.tag.Inline) {
            return inlinePopup(o);
        }
        else if (popupMod == 'bootstrap') {
            return bootstrapPopup(o);
        }
        else {
            return jqueryUIpopup(o);
        }
    };
    /*endpopup*/

    //change popup
    $('#chPopupMod').change(function () {
        var p = $('#chPopupMod').val();
        var v = $('#chTheme').val().split("|");
        var theme = v[0];
        popupMod = p;

        $('.awe-popup').each(function () {
            if ($(this).data('api'))
                $(this).data('api').close();
        });

        $('.awe-multilookup, .awe-lookup').each(function () { $(this).data('api').initPopup(); });

        $.post(root + "Settings/Change", { theme: theme, popupMod: p });
    });

    //change theme
    $('#chTheme').change(function () {
        var newmod = $('#chPopupMod').val();
        var v = $('#chTheme').val().split("|");
        var theme = v[0];
        var jqTheme = v[1];

        $('#jqStyle').attr('href', "http://code.jquery.com/ui/1.11.1/themes/" + jqTheme + "/jquery-ui.min.css");
        $('#aweStyle').attr('href', root + "Content/themes/" + theme + "/AwesomeMvc.css?v=3");
        $('#demoStyle').attr('href', root + "Content/themes/" + theme + "/Site.css?v=3");
        $.post(root + "Settings/Change", { theme: theme, popupMod: newmod }, function () {
            setTimeout(function () {
                $('.awe-grid').each(
                    function () {
                        $(this).data('api').lay();
                    });
            }, 500);
        });
    });
}

// on ie hitting enter doesn't trigger change, 
// all searchtxt inputs will trigger change on enter in all browsers
function consistentSearchTxt() {
    $('.searchtxt').each(function () {
        if ($(this).data('searchtxth') != 1)
            $(this).data('searchtxth', 1)
                .data('myval', $(this).val())
                .on('change', function (e) {
                    if ($(this).val() != $(this).data('myval')) {
                        $(this).data('myval', $(this).val());
                    } else {
                        e.stopImmediatePropagation();
                    }
                })
                .on('keyup', function (e) {
                    if (e.which == 13) {
                        e.preventDefault();
                        if ($(this).val() != $(this).data('myval')) {

                            $(this).change();
                        }
                    }
                });
    });
}

function handleAnchors() {
    var anchor = location.hash.replace('#', '').replace(/\(|\)|:|\.|\;|\\|\/|\?|,/g, '');
    $('h3,h2').each(function (_, e) {
        var $e = $(e);
        var name = $e.html().trim().replace(/ /g, '-').replace(/\(|\)|:|\.|\;|\\|\/|\?|,/g, '');
        $e.append("<a class='anchor' name='" + name + "' href='#" + name + "'><i class='glyphicon glyphicon-link'></i></a>");
        if (name == anchor) {
            $e.addClass("awe-changing").removeClass('awe-changing', 3000);
        }
    });
}

var lastw = 0;

function layPage() {
    // .page #main #demoMenu #demoPage
    var ww = $(window).width();

    if (ww > 1200) $('.page').css('width', "93%");
    else $('.page').css('width', "100%");

    $("#main").css("min-height", ($(window).height() - 120) + "px");

    if (lastw != ww)
        MenuToggle(ww < 1050);
    lastw = ww;
}

//wrap ajaxlists for horizontal scrolling on small screens
function wrapLists() {
    $('table.awe-ajaxlist:not([wrapped])').each(function () {
        var columns = $(this).find('thead th').length;
        var mw = $(this).data('mw');
        if (columns || mw) {
            mw = mw || columns * 120;

            $(this).wrap('<div style="width:100%; overflow:auto;"></div>')
                .wrap('<div style="min-width:' + mw + 'px;padding-bottom:2px;"></div>')
                .attr('wrapped', 'wrapped');
        }
    });
}

function MenuToggle(x, keepButton) {
    if (x) {
        $('#demoMenu').hide();
        $('#demoPage').css('margin-left', "0");
        $('#btnMenuToggle').show();
    } else {
        $('#demoMenu').show();
        $('#demoPage').css('margin-left', "14.5em");
        if (!keepButton) $('#btnMenuToggle').hide();
    }
}

function setjQueryValidateDateFormat(format) {
    //setting the correct date format for jquery.validate
    jQuery.validator.addMethod(
        'date',
        function (value, element, params) {
            if (this.optional(element)) {
                return true;
            };
            var result = false;
            try {
                $.datepicker.parseDate(format, value);
                result = true;
            } catch (err) {
                result = false;
            }
            return result;
        },
        ''
    );
}

/*begin*/
awe.err = function (o, xhr, textStatus, errorThrown) {
    var msg = "unexpected error occured";
    if (xhr) {
        msg = xhr.responseText;
    }
    msg += "(you see this message because in Site.js awe.err is set to a function that shows this popup)&nbsp; ";
    var btnHide = $('<button type="button" class="awe-btn"> hide </button>').click(function () {
        $(this).parent().remove();
    });

    var c = $('<div/>').html(msg).append(btnHide);

    //decide where to attach the inline popup
    if (o.p && o.p.isOpen) { // if helper has popup and is open
        o.p.d.prepend(c); // put msg inside popup div
    } else if (o.f) {
        o.f.html(c); // put msg inside control field
    } else $('body').prepend(c);
};/*end*/

//google analytics
var _gaq = _gaq || [];
_gaq.push(['_setAccount', 'UA-27119754-1']);
_gaq.push(['_setDomainName', 'aspnetawesome.com']);
_gaq.push(['_trackPageview']);

(function () {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();