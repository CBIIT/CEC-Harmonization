

function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
}

function goBack() {
    window.history.go(-2);
    return false;
}

function shadeBackground() {
    var loadCnt = document.getElementById("loaderCnt");
    loadCnt.style.visibility = "visible";
}


function showModuleControl(id) {
    shadeBackground();

    var cmod = document.getElementById(id);
    cmod.style.zIndex = "1001";
}

function toggleAccordion(obj) {
    // Accordion
    $(obj).prev("h2").toggleClass("active");
    $(obj).slideToggle("slow");
}

function redirectToURLInFiveSeconds(url) {

    window.setInterval(function () { location.assign(url) }, 5000);
}

/* commented region below was an attempt at creating sticky headers for long-width tables
    never got it working, nor does it handle sticky columns; just sticky rows
function floatNavMenuToTop(menuID) {

var menu = document.getElementById(menuID);

// does the Y axis on the page exceed the menu's top position
if (menu.offsetTop < window.pageYOffset) {
if (menu.style.position != "absolute")
menu.style.position = "absolute";

var topPos = window.pageYOffset.toString() + "px";
menu.style.top = topPos;
}
else if ((window.pageYOffset > menu.offsetHeight) && menu.offsetTop > window.pageYOffset) {
if (menu.style.position != "absolute")
menu.style.position = "absolute";

var topPos = window.pageYOffset.toString() + "px";
menu.style.top = topPos;
}
}

function cloneHeaderRow(headerRowID, parentTableID) {
var table = document.getElementById(parentTableID);

var row = document.getElementById(headerRowID + "_floater");
if (row == null) {
row = document.getElementById(headerRowID).cloneNode(true);

row.id = headerRowID + "_floater";

row.style.position = "absolute";
row.style.left = table.offsetLeft.toString() + "px";
row.style.width = table.offsetWidth.toString() + "px";

document.body.onresize = function () { row.parentNode.removeChild(row); }

table.insertBefore(row, table.firstChild);
}

return row;
}

function floatHeaderRowToTop(headerRowID, parentTableID) {

var table = document.getElementById(parentTableID);
var row;

if (window.pageYOffset < (table.offsetHeight + table.offsetTop) && window.pageYOffset > table.offsetTop) {
var row = cloneHeaderRow(headerRowID, parentTableID);

if (row.offsetTop < window.pageYOffset || row.offsetTop > window.pageYOffset)
row.style.top = window.pageYOffset.toString() + "px";

}
else if (window.pageYOffset > (table.offsetHeight + table.offsetTop) || window.pageYOffset < table.offsetTop) {
row = document.getElementById(headerRowID.toString() + "_floater");
if (!(row == null))
row.parentNode.removeChild(row);
}
} */

/* unused?
function cancer_noCancerChecked(element) {
    if (element.checked) {
        var cboxes = document.getElementsByTagName("input");
        for (var i = 0; i < cboxes.length; i++) {
            if (cboxes[i].type != "checkbox")
                continue;

            if (cboxes[i].id == "noCancer")
                continue;
            else
                cboxes[i].checked = false;
        }
    }
}
*/

function specimen_populateSelectList() {
    var specimenSelected = $('#specimen_options_list input:checked').length;
    $('#specimen_list_count').text(specimenSelected);

    $('#selected_specimen_list').empty();
    var labelCount = 0;
    $('#specimen_options_list input:checked').each(function () {
        if (this.id == 'specimen_all') {
            return true;
        }

        if (labelCount < 4) {
            $('#selected_specimen_list').append("<li>" + $(this).attr("specimen_name") + "</li>");
        }
        else {
            $('#selected_specimen_list').append("<li>and " + (specimenSelected - 4) + " more...</li>");
            return false;
        }
        labelCount++;
    });
}

function cancer_allCancerChecked(element) {
    var isChecked = element.checked;
    $('#cancer_options_list input[type="checkbox"]').each(function () {
        // skip "all cancer"
        if ($(this).attr("id") == "cancer_all")
            return true;

        $(this).prop('checked', isChecked);
    });
    cancer_populateSelectList();
}

function cancer_populateSelectList() {
    var cancerSelected = $('#cancer_options_list input:checked').length;
    $('#cancer_list_count').text(cancerSelected);

    $('#selected_cancer_list').empty();
    var labelCount = 0;
    $('#cancer_options_list input:checked').each(function () {
        if (this.id == 'cancer_all') {
            return true;
        }

        if (labelCount < 4) {
            $('#selected_cancer_list').append("<li>" + $(this).attr("cancer_name") + "</li>");
        }
        else {
            $('#selected_cancer_list').append("<li>and " + (cancerSelected - 4) + " more...</li>");
            return false;
        }
        labelCount++;
    });
}

function cohorts_allCohortsChecked(element) {
    var isChecked = element.checked;
    $('#cohort_list_container input[type="checkbox"]').each(function () {
        // skip "all cohort"
        if ($(this).attr("id") == "cohort_all")
            return true;

        $(this).prop('checked', isChecked);
    });
    cohorts_populateSelectList();
}

function cohorts_populateSelectList() {
    var cohortSelected = $('#cohort_list_container input:checked').length;
    $('#cohort_list_count').text(cohortSelected);

    $('#selected_cohort_list').empty();
    var labelCount = 0;
    $('#cohort_list_container input:checked').each(function () {
        if (this.id == 'cohort_all') {
            return true;
        }

        if (labelCount < 4) {
            $('#selected_cohort_list').append("<li>" + $(this).attr("cohort_name") + "</li>");
        }
        else {
            $('#selected_cohort_list').append("<li>and " + (cohortSelected - 4) + " more...</li>");
            return false;
        }
        labelCount++;
    });
}

function toggleCategoryState(categoryID) {
    var catObj = document.getElementById(categoryID);
    var stateObj = document.getElementById(categoryID + '_state');

    if (catObj.classList.contains('active'))
        stateObj.value = 1;
    else
        stateObj.value = 0;
}

function expandFirstSection() {
    var firstSec = $('tbody > tr').first();
    firstSec.find('a[class="section-expand"]').toggleClass('active');
    var rowsBelow = firstSec.find('a').attr('target-rows');
    if (rowsBelow == null)
        rowsBelow = 1;
    for (i = 0; i < rowsBelow; i++) {
        if (firstSec.hasClass('compare-section-hidden')) {
            firstSec.removeClass('compare-section-hidden');
        }
        else
            i = i - 1;

        firstSec = firstSec.next();
    }
}

$(document).ready(function () {

    $('#cohort_list_container').click(function (event) {
        if ($('#show_cohort_list_btn').attr('aria-expanded') == 'true' && event.target.nodeName != 'BUTTON') {
            event.stopPropagation();
        }
    });
    $('#cohort_list_container').change(function (event) {
        cohorts_populateSelectList();
    });
    $('#cohort_list_container').ready(function (event) {
        cohorts_populateSelectList();
    });

    $('#cancer_options_list').click(function (event) {
        if ($('#cancer_dropdown').attr('aria-expanded') == 'true' && event.target.nodeName != 'BUTTON') {
            event.stopPropagation();
        }
    });
    $('#cancer_options_list').change(function (event) {
        cancer_populateSelectList();
    });
    $('#cancer_options_list').ready(function (event) {
        cancer_populateSelectList();
    });

    $('#specimen_options_list').click(function (event) {
        if ($('#specimen_dropdown').attr('aria-expanded') == 'true' && event.target.nodeName != 'BUTTON') {
            event.stopPropagation();
        }
    });
    $('#specimen_options_list').change(function (event) {
        specimen_populateSelectList();
    });
    $('#specimen_options_list').ready(function (event) {
        specimen_populateSelectList();
    });

    $('tr.compare-parent-row > th, tr.compare-parent-row > td').click(function () {
        var btn = $(this).parent().find('a[class*="row-expand"]');
        btn.toggleClass('active');
        var rowsBelow = btn.attr('target-rows');
        var siblingRow = btn.parent().parent();
        for (i = 0; i < rowsBelow; i++) {
            siblingRow = siblingRow.next();
            siblingRow.toggleClass('compare-child-row-hidden');
        }
    });
    $('a[class="row-expand"]').click(function (event) {
        $(this).toggleClass('active');
        var rowsBelow = $(this).attr('target-rows');
        var siblingRow = $(this).parent().parent();
        for (i = 0; i < rowsBelow; i++) {
            siblingRow = siblingRow.next();
            siblingRow.toggleClass('compare-child-row-hidden');
        }
        event.stopPropagation();
    });
    $('th.compareGroup-header').on("click keypress", function (event) {
        if (event.keyCode == 9) {
            return;
        }

        var btn = $(this).find('a[class*="section-expand"]');
        btn.toggleClass('active');
        var rowsBelow = btn.attr('target-rows');
        var siblingRow = btn.parent().parent().next();
        var isCollapsing = !(siblingRow.hasClass('compare-section-hidden'));
        var done = false;
        while (!done) {
            if (siblingRow.hasClass('compare-parent-row') || siblingRow.hasClass('compare-row')) {
                if (isCollapsing)
                    siblingRow.addClass('compare-section-hidden');
                else
                    siblingRow.removeClass('compare-section-hidden');

                siblingRow.find('a[class*="row-expand"]').each(function () {
                    $(this).removeClass('active');
                });
            }
            else if (siblingRow.hasClass('compare-child-row')) {
                if (isCollapsing)
                    siblingRow.addClass('compare-child-row-hidden');
            }
            else
                done = true;

            siblingRow = siblingRow.next();
        }
    });
    $('a[class="section-expand"]').click(function (event) {
        $(this).toggleClass('active');
        var rowsBelow = $(this).attr('target-rows');
        var siblingRow = $(this).parent().parent();
        for (i = 0; i < rowsBelow; i++) {
            siblingRow = siblingRow.next();
            if (!siblingRow.hasClass("compare-child-row-hidden"))
                siblingRow.toggleClass('compare-section-hidden');
            else
                i--;
        }
        event.stopPropagation();
    });
});