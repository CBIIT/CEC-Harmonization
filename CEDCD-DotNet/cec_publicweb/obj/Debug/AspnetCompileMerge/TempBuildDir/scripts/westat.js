
/* added by yours truly, Stephen 26-Nov-2014 to handle
    the toggleFiltering control expand/collapse state between
    post backs
*/
//$(document).ready(function () {
//    var anychecked = $("#filter-inner").find("input[type='checkbox']").each(function () {
//        if ($(this).attr("checked") == "checked") {
//            $("#toggleFiltering").toggleClass("active");
//            $("#filter-inner").show();
//        }
//    });

//    /* handles category list control's expand/collapse state between postbacks even if nothing is checked */
//    var anyshown = $(".w-accordion").find("input[type='hidden']").each(function () {
//        if ($(this).attr("value") == "1") {

//            $("#filter-inner").show();
//            $(this).siblings("ul").show();
//            $(this).siblings("h3").toggleClass("active");
//        }
//    });
//});

//(function ($) {  
//// Filter show/hide
//    //$("#filter-inner").hide();
//    $("#toggleFiltering").click(function(){
//        $("#filter-inner").slideToggle("slow")
//        $(this).toggleClass("active");
//    });    
//    $("#toggleFiltering").keypress(function(event){
//        if(event.which == 13 || event.which == 32) {
//            $("#filter-inner").slideToggle("slow");
//            $(this).toggleClass("active");
//        }
//    });  
//})(jQuery);

(function ($) { 
// Accordion
    $(".w-accordion ul").eq('false').show();
    $(".w-accordion h3").click(function(){
        $(this).next("ul").slideToggle("slow");
        $(this).toggleClass("active");
        $(this).siblings("h3").removeClass("active");
        
        //SC:4-10-2015: to persist category state
        toggleCategoryState($(this).attr('id'));
    });
    $(".w-accordion h3").keypress(function(event){
        if(event.which == 13 || event.which == 32) {
            $(this).next("ul").slideToggle("slow");
            $(this).toggleClass("active");
            $(this).siblings("h3").removeClass("active");
            
            //SC:4-10-2015: to persist category state
            toggleCategoryState($(this).attr('id'));
        }
    });

//    $('#filter-inner ul').each(function() {
//                var ch = $(this).find('input:checked');
//                if (ch.length) {
//                    $(this).show()
//                }      
//    });
})(jQuery);

// for Addition Info (Profiles) 

(function ($) {
    // Accordion
    $(".w-accordion-prof .add-prof").eq('false').show();
    $(".w-accordion-prof h2.more-info-link").click(function () {
        $(this).next(".add-prof").slideToggle("slow");
        $(this).toggleClass("active");
        $(this).siblings("h2").removeClass("active");
    });

})(jQuery);


// STICKY MENU added 10-27-14
  /*$(document).ready(function(){
    $("#sticker").sticky({topSpacing:10, bottomSpacing:220});
  }); */


// For the new COHORT PROFILE page, added 11-20-14
(function ($) {  
// Accordion
    $(".cohortInfo").eq('false').show();
    $(".cohortInfo button").click(function(){
        $(this).toggleClass("active");

        $expanded = 'false';
        if ($(this).attr("aria-expanded") == 'false')
            $expanded = 'true';

        $(this).attr("aria-expanded", $expanded);
        $(this).next(".cohortInfoBody").attr("aria-hidden", ($expanded == 'false' ? 'true' : 'false'));

        $(this).next(".cohortInfoBody").slideToggle("slow");
    });
    
})(jQuery);


// For the new read more button on new SELECT COHORTS page, added 12-1-14 
(function ($) {  
    $("#sc-intro-pt2").hide();
    $("#readMore").click(function(){
        $("#sc-intro-pt2").slideToggle("slow");
        $(this).toggleClass("active");
    });    
})(jQuery);

// Affixing the floating submit button on cohortselect.aspx page
(function ($) {
  if ($('#floatingSubmitButtonContainer').length) {
    var toptrigger = $('#cohortGridView').offset().top+60;
    var viewportSize = $(window).height();
    var totalHeight = $('#cohortSelectPage').height();
    var floatwidth = $('#cedcd-home-cohorts-inner').outerWidth();
    
    //console.log('Top Trigger: '+toptrigger);
    //console.log('Viewport Size: '+viewportSize);
    
    if ($(document).scrollTop() === 0 && viewportSize > toptrigger) {
      //console.log("You can see me at the bottom");
      $('#floatingSubmitButtonContainer').addClass('floatingFixed');
      floatBar(viewportSize, totalHeight, toptrigger);
    }
    else if ($(document).scrollTop() === 0 && viewportSize < toptrigger) {
      //console.log("You can't see me yet");
      floatBar(viewportSize, totalHeight, toptrigger);
    }
 
    $('#floatingSubmitButtonContainer').css('width', floatwidth);
  }
  function floatBar(viewportSize, totalHeight, toptrigger) {
    $(document).scroll(function() {
      var scrollDistance = $(document).scrollTop();
      var pos = scrollDistance+viewportSize;
      //console.log('Where we are: '+pos+'/'+totalHeight);
      $('#floatingSubmitButtonContainer').removeClass('floatingFixed').addClass('offScreen');

      if (viewportSize+scrollDistance < toptrigger) {
        //console.log("You can't see me");
      }
      else if (viewportSize+scrollDistance > toptrigger && viewportSize+scrollDistance < totalHeight-200) {
        //console.log("Now you can see me and I'm sticky");
        $('#floatingSubmitButtonContainer').removeClass('offScreen').removeClass('.atHome').addClass('floatingFixed');
      }
      else if (viewportSize+scrollDistance > totalHeight-200) {
        //console.log("You can see me and I'm in my final resting place");
        $('#floatingSubmitButtonContainer').removeClass('.floatingFixed').addClass('atHome');
  }
      else {
        //console.log("I'm not sure where I should be");
      }
    });
}
})(jQuery);

// Affixing the floating edit/admin toolbar on edit.aspx
(function ($) {
  if ($('.floatingToolbar').length) {
    $(window).load(setFloatingBar);
    $(window).load(setFloatingBarVertical);
    $(window).resize(setFloatingBar);
    $(window).resize(setFloatingBarVertical);
    $(document).scroll(setFloatingBarVertical);
  }
  
  function setFloatingBar() {
    var viewportwidth = $(window).width();
    var contentwidth = $('#cedcd-main-content').width();
    var toolbarwidth = $('.floatingToolbar').width();
    var openmargin = (viewportwidth - contentwidth)/2;
    var toolbarpositionhorizontal = (openmargin-toolbarwidth-15);
    //console.log('Viewport Width: '+viewportwidth);
    //console.log('Content Width: '+contentwidth);
    //console.log('Toolbar Width: '+toolbarwidth);
    //console.log('Open Margin Right: '+openmargin);
    //console.log('Toolbar Position Right: '+toolbarpositionhorizontal);
    
    if (openmargin >= toolbarwidth) {
      //console.log('We have space!');
      $('.floatingToolbar').css('right', toolbarpositionhorizontal+'px');
      if ($('#section').attr('style')) {
        $('#section').attr('style','');
      }
    }
    if (openmargin < toolbarwidth) {
      //console.log('We need to make space');
      $('#section').css('margin-right',toolbarwidth+15+'px');
      $('.floatingToolbar').css('right', 0);
    }
  }
  
  function setFloatingBarVertical () {
    var topTrigger = $('#inputTabs').offset().top+65;
    var finallock = 225;
    //console.log('Viewport Height: '+viewportheight);
    //console.log('Toolbar Height: '+toolbarheight);
    //console.log('Tollbar Position top: '+toolbarpositionvertical);
    //console.log('finallock: '+finallock);
    
    if ($(document).scrollTop() === 0) {
      //console.log('At The Top!');
      $('.floatingToolbar').css('top', topTrigger);
    }
    else if ($(document).scrollTop() <= topTrigger-finallock) {
      //console.log('We should move the toolbar');
      $('.floatingToolbar').css('top', (topTrigger-$(document).scrollTop()));
    }
    else if ($(document).scrollTop() > topTrigger-finallock) {
      //console.log('We should lock the toolbar');
      $('.floatingToolbar').css('top', finallock);
    }
  }
})(jQuery);

// Dealing with the menu for logged in users, mostly just a precaution to prevent logged in users from bouncing to other parts of the site and not being able to get back
(function ($) {
  $(window).load(function() {
    if ($('.loggedin').length) {
      //console.log('User is logged in');
      var notusernav = $('#mainNavBar li a').not('li#liLogin a').not('li#liHelp a');
      notusernav.addClass('hidden');
      //notusernav.css('cursor','not-allowed').css('transition', 'none');
      notusernav.click(function(e){
        e.preventDefault();
        console.log('Nope');
      });
    }
    else {
      //console.log('Anonymous User');
    }
  });
})(jQuery);

// Misc class addition that's would take too long to do properly
(function ($) {
  if ($('table.enrollTbl').length) {
    //console.log($('table.enrollTbl').parent());
    $('table.enrollTbl').parent().addClass('interiorTable');
  }
  
  if ($('#edit_cohort').length) {
    //console.log('edit cohort page!');

    var sPageURL = window.location.search.substring(1);
    var sPageURLparts = sPageURL.split("&");
    var currentSection;
    var contentClass;
    //console.log("Base URL Bit="+sPageURL);
    //console.log("Parts Array="+sPageURLparts);
    
    for (var i=0; i < sPageURLparts.length; i++) {
      if (sPageURLparts[i].substring(0,7) == "section") {
        //console.log(sPageURLparts[i]);
        currentSection=sPageURLparts[i];
        //console.log(currentSection);
      }
    }
    if (currentSection.indexOf('\=') > -1) {
      contentClass = currentSection.replace("=","-");
      //console.log('Cleaning ContentClass');
      //console.log(contentClass);
    }
    else {
      contentClass = currentSection;
      //console.log('ContentClass good as is');
    }
    
    $('#section').addClass(contentClass);
  }
})(jQuery);

// Back to Top functionality 
$(document).ready(function(){
  $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('#back-to-top').fadeIn();
        } else {
            $('#back-to-top').fadeOut();
        }
    });
    // scroll body to 0px on click
    $('#back-to-top').click(function () {
        $('#back-to-top').tooltip('hide');
        $('body,html').animate({
            scrollTop: 0
        }, 800);
        return false;
    });

    $('#back-to-top').tooltip('show');

  // back to previous page link
  $('a.back').click(function(){
      parent.history.back();
      return false;
    });
});

// sticky header script


(function createStickyElements() {

  var table = document.querySelector('.cedcd-table'); // select the element we'll be working in
  var tableTop = (function getPosition(element) { // get distance from top of page to table
              // var xPosition = 0;
              var yPosition = 0;

              while(element) {
                  // xPosition += (element.offsetLeft - element.scrollLeft + element.clientLeft);
                  yPosition += (element.offsetTop - element.scrollTop + element.clientTop);
                  element = element.offsetParent;
              }

              // return { x: xPosition, y: yPosition };
              return yPosition;
          })(table);

  const path = document.URL.split('/(.westat.com|.gov)/')[1].split('.')[0];
  //const path = document.URL.split(/\.\w\w\w\//)[1].split('.')[0];  // .gov seems safer, so we should go back to that for production

/* the following section acts as a controller, calling functions based on the url */


  if (path === 'compare') {
    var originalRow = document.querySelector('tr#sticker'); // select row to be copied
    const rowEls = createTopRow(originalRow); // create copy
    const rowWrapper = rowEls[0];
    const rowElement = rowEls[1];
    attachScrollListenersForScreenHeader(rowWrapper, rowElement, {padding: -2}); // stick row to screen
    createAndAppendSingleColumn(rowElement);

    document.querySelector('.cedcd-table').appendChild(rowWrapper);

    setTimeout(function(){ // allows for table reflow
      document.querySelector('.row-header').click(); 
      document.querySelector('.row-header').click();
    },10)
  }

  if (path === 'cancer' || path === 'biospecimen')  {
    var originalRow = document.querySelector('.col-header');
    const rowEls = createTopRow(originalRow);
    const rowWrapper = rowEls[0];
    const rowElement = rowEls[1];
    attachScrollListenersForScreenHeader(rowWrapper, rowElement, {padding: -75});

    document.querySelector('.cedcd-table').appendChild(rowWrapper);
    rowWrapper.style.top = table.offsetTop + 5 + 'px';
    createAndAppendColumns(table);
  }

  if (path === 'enrollment')  {
    
    var tables = table.querySelectorAll('table'); // select all tables
    for (let k = 0; k < tables.length; k++) { // iterate through each table
      var originalRow = tables[k].querySelector('.col-header'); // get row in table
      let corner = createTopRow(originalRow, true, tables[k], true); // element that is sticky both ways
      let rowEls = createTopRow(originalRow, true, tables[k]); // sticky row
      let rowWrapper = rowEls[0];
      let rowElement = rowEls[1];

      tables[k].parentElement.appendChild(rowWrapper);
      setHScroll(rowWrapper, tables[k].parentElement, true, true); // stick row to table top
      // setVScroll(rowWrapper, tables[k].parentElement);

      tables[k].parentElement.style.position = 'relative';
      var column = createAndAppendColumns(tables[k].parentElement, true);
      setVScroll(column, tables[k].parentElement, true);

      tables[k].parentElement.appendChild(corner[0]);
      setHScroll(corner[0], tables[k].parentElement, true);
      setVScroll(corner[0], tables[k].parentElement, true);

      column.classList.add('fixed-header-column--enroll');
      rowWrapper.classList.add('fixed-header-row--enroll');
      corner[0].classList.add('fixed-header-row--enroll');
      corner[0].classList.add('fixed-header-row--corner');
    }
  }

  if (path === 'input/edit' && document.querySelector('#inputTabs li:nth-child(3)[class="active"]')) { // route of c questions
    var header = document.querySelector('.panel-heading');
    var elTop = (function getPosition(element) { // get distance from top of page to table
              // var xPosition = 0;
              var yPosition = 0;

              while(element) {
                  // xPosition += (element.offsetLeft - element.scrollLeft + element.clientLeft);
                  yPosition += (element.offsetTop - element.scrollTop + element.clientTop);
                  element = element.offsetParent;
              }

              // return { x: xPosition, y: yPosition };
              return yPosition;
          })(header);
    var elBottom = elTop + header.parentElement.offsetHeight;
    attachScrollListenersForScreenHeader(header, false, {padding: 0, elTop: elTop, elBottom: elBottom});
  }

  if (path === 'input/edit' && document.querySelector('#inputTabs li:nth-child(4)[class="active"]')) {
    console.log('correct routing')

    table = document.querySelector('.panel-body');
    tableTop = (function getPosition(element) { // get distance from top of page to table
              // var xPosition = 0;
              var yPosition = 0;

              while(element) {
                  // xPosition += (element.offsetLeft - element.scrollLeft + element.clientLeft);
                  yPosition += (element.offsetTop - element.scrollTop + element.clientTop);
                  element = element.offsetParent;
              }

              // return { x: xPosition, y: yPosition };
              return yPosition;
          })(table);

    var originalRow = document.querySelector('thead tr');
    const rowEls = createTopRow(originalRow, true, table, false, 500);
    const rowWrapper = rowEls[0];
    const rowElement = rowEls[1];
    const argOffset = 10;
    rowWrapper.classList.add('fixed-header-row-wrapper--phase-2');
    console.log(originalRow.offsetTop)
    // rowWrapper.style.top = '61px';
    checkHScroll(pageYOffset, rowWrapper, {padding: 0, argOffset: argOffset, end: table.offsetHeight - 140});

    attachScrollListenersForScreenHeader(rowWrapper, rowElement, {padding: 0, argOffset: argOffset, end: table.offsetHeight - 140});
    table.appendChild(rowWrapper);
  }

  if (path === 'input/edit' && document.querySelector('#inputTabs li:nth-child(7)[class="active"]')) {
    console.log('correct routing')

    table = document.querySelectorAll('.panel-body')[3];
    tableTop = (function getPosition(element) { // get distance from top of page to table
              // var xPosition = 0;
              var yPosition = 0;

              while(element) {
                  // xPosition += (element.offsetLeft - element.scrollLeft + element.clientLeft);
                  yPosition += (element.offsetTop - element.scrollTop + element.clientTop);
                  element = element.offsetParent;
              }

              // return { x: xPosition, y: yPosition };
              return yPosition;
          })(table);

    var originalRow = document.querySelector('thead tr');
    const rowEls = createTopRow(originalRow, true, table, false, 700);
    const rowWrapper = rowEls[0];
    const rowElement = rowEls[1];
    const padding = window.matchMedia('(min-width: 1160px').matches ? 0 : -19;
    console.log(padding);
    rowWrapper.classList.add('fixed-header-row-wrapper--textwrap');
    rowWrapper.classList.add('fixed-header-row-wrapper--phase-2');
    rowWrapper.style.top = '102px';

    checkHScroll(pageYOffset, rowWrapper, {padding: 0, argOffset: 10, end: table.offsetHeight - 140});

    attachScrollListenersForScreenHeader(rowWrapper, rowElement, {padding: padding, argOffset: 10, end: table.offsetHeight - 140});
    table.appendChild(rowWrapper);
  }

/* the following section is the first function. it creates the header row */


  function createTopRow(originalRow, caption, table, corner, timeExtender){
    
    // create the generic rowWrapper and rowElement elements
    var rowWrapper = document.createElement('div');
    var rowElement = document.createElement('div');

    // if corner (optional), only add two cells
    var count = corner ? 2 : originalRow.children.length;

    // if caption (optional), add caption element above row of cells
    if (caption) {
      var rowCaption = document.createElement('div');
      rowCaption.classList.add('fixed-header-row-caption');
      rowCaption.innerHTML = table.querySelector('caption').innerHTML;
      rowWrapper.appendChild(rowCaption)
    }

    rowWrapper.classList.add('fixed-header-row-wrapper');
    rowElement.classList.add('fixed-header-row');
    // rowElement.id = 'fixed-header-row'; // I'm not sure why I did this, but it will cause issues, so hopefully it's not necessary

    rowWrapper.appendChild(rowElement);

    // pseudo-clone the originalRow into rowElement after a slight delay to account for table reflowing
    setTimeout(function(){
      console.log('check')
      for (let i=0; i<count; i++) {
        let cell = document.createElement('div');
        let oldCell = originalRow.children[i];
        cell.className = "fixed-row__cell";
        cell.innerHTML = oldCell.innerHTML;
        cell.style.height = oldCell.offsetHeight + 'px';
        cell.style.width = oldCell.offsetWidth + 'px';
        rowElement.appendChild(cell);
      }
      timeExtender ? rowWrapper.style.width = originalRow.offsetWidth + 2 + 'px' : null;
    }, timeExtender || 10)

    return [rowWrapper, rowElement]; // return the elements, so they can be further customized
  }

/* -------- the following section defines a header that sticks to the top of the screen -------- */
  
  function attachScrollListenersForScreenHeader (rowWrapper, rowElement, options) {
    // create scroll listeners, applying throttling principles from https://developer.mozilla.org/en-US/docs/Web/Events/scroll
    var last_known_scroll_position = 0;
    var ticking = false;  

    window.addEventListener('scroll', function(e) {

      last_known_scroll_position = pageYOffset;

      if (!ticking) {

        window.requestAnimationFrame(function() {
          if (rowElement || options.argOffset) {
            checkHScroll(last_known_scroll_position, rowWrapper, {padding: options.padding, argOffset: options.argOffset, end: options.end});
          } else {
            setHeaderScroll(last_known_scroll_position, rowWrapper, options.elTop, options.elBottom);
          }

          ticking = false;
        });
         
        ticking = true;

      }
      
    });



    if(rowElement){
      setVScroll(rowElement, table);
    }
    


    


  }

  // check to see if document is scrolled past top of table
  function checkHScroll(pos, rowWrapper, options) {
    // define rowheader top offset
    var offset = table.offsetTop;
    var offsetTwo = options.argOffset || 0;
    var padding = options.padding;
    var end = options.end || Infinity;
    console.log(offset)
    var pageOffset = padding || 0;
    let corner = document.querySelector('.table-header');
    if (pos >= tableTop + offset + offsetTwo && pos <= tableTop + end) {
      console.log(pos - tableTop);
      rowWrapper.style.top = (pos - tableTop) + offset + pageOffset + "px";
      rowWrapper.classList.add('fixed-header-row-wrapper--scroll');
      if (corner) {
        table.classList.add('fixed');
      }
    } else {
      console.log('less')     
      rowWrapper.style.top = offset + offsetTwo + 5 + 'px';
      rowWrapper.classList.remove('fixed-header-row-wrapper--scroll');
      if (corner) {
        table.classList.remove('fixed');
      }
    } // needs option for scrolling past the end of the table
  }

  // set fixed row's vertical scroll to match that of the table
  function setVScroll(element, table, reverse) {
    var last_known_table_scroll_position = 0;
    var tableTicking = false;

    table.addEventListener('scroll', function(e) {

      last_known_table_scroll_position = table.scrollLeft;

      if (!tableTicking) {

        window.requestAnimationFrame(function() {
          let update = (reverse ? '' : '-') + last_known_table_scroll_position + "px";
          element.style.transform = element.style.transform.replace(/translateX\(.\w*\)/, '');
        element.style.transform += "translateX(" + update + ")";
            tableTicking = false;
        });
         
        tableTicking = true;

      }
      
    });
  }

  function setHeaderScroll(pos, el, elTop, elBottom) {
    // var offset = table.offsetTop; 
    


    if (pos >= elTop && pos <= elBottom - 60) {
      el.classList.add('panel-heading--fixed');
      el.classList.add('col-sm-12');
      el.parentElement.classList.add('parent--fixed');
    } else {   
      el.classList.remove('panel-heading--fixed');
      el.classList.remove('col-sm-12');
      el.parentElement.classList.remove('parent--fixed');
    }
  }


  function setHScroll(element, table, reverse, row) {
    var last_known_table_scroll_position = 0;
    var tableTicking = false;

    table.addEventListener('scroll', function(e) {

      last_known_table_scroll_position = table.scrollTop;

      if (!tableTicking) {

        window.requestAnimationFrame(function() {
          let update = (reverse ? '' : '-') + last_known_table_scroll_position + 'px';
          element.style.transform = element.style.transform.replace(/translateY\(.\w*\)/, '');
        element.style.transform += "translateY(" + update + ")";

        if (row) {
          element.classList.add('fixed-header-row-wrapper--scrolling');
          if (last_known_table_scroll_position <= 0) {
            element.classList.remove('fixed-header-row-wrapper--scrolling');
          }
        }

            tableTicking = false;
        });
         
        tableTicking = true;

      }
      
    }); 
  }
  


/* -------- the following section mostly deals with single-width sticky left-side column -------- */
  function createAndAppendSingleColumn(rowElement){
    var rowHeaders = document.querySelectorAll('tbody th'); // all th elements in the first column
    var splitHeaders = []; // an array of arrays representing expand/collapse rows
    var innerSplits = []; // an array of arrays representing inner expand/collapse rows, like those on the Specimen Overview tab
    var splitIndex = -1;
    var innerIndex = -1;
    var bodyRowIndex = 0;
    var innerBodyIndex = 0;


    // find all th elements in the first column, sort them by type and apply rules
    for (let i=0;i<rowHeaders.length;i++) {
      if (rowHeaders[i].classList.contains('compareGroup-header')) { // for expand/collapse rows
        splitIndex++;
        splitHeaders[splitIndex] = [];
        rowHeaders[i].setAttribute('data-ref', splitIndex); // attr will link original el to its clone

        // add ability to expand/collapse dependent clone cells
        rowHeaders[i].addEventListener('click', function(e){ 
          let ref = e.currentTarget.getAttribute('data-ref'); // because splitIndex will likely change values before this call
          let section = document.querySelectorAll('.table-section')[ref]; // selects the section containing dependent clone cells
          
          
          section.classList.toggle('table-section--active'); // expands or collapses

          let headers = document.querySelectorAll('.body-header:not(.body-header--child)'); // all clone cells
          let rows = document.querySelectorAll('tbody th:not(.compareGroup-header):not(.compareChildRecord)'); // all original cells

          // console.log(headers);
          // console.log(rows);


          // reassign all clone heights every time any expand/collapse action is taken.
          setTimeout(function(){
            for (let j=0;j<headers.length;j++) { 
              let refIndex = headers[j].getAttribute('data-ref');
              // let height = parseFloat(window.getComputedStyle(rows[refIndex]).height) - 2 + "px"; // this is finicky. 2 may need to be adjusted
              // let height = rows[refIndex].offsetHeight;
              headers[j].style.height = rows[refIndex].offsetHeight  - 2 + 'px';
              headers[j].style.width = rows[refIndex].offsetWidth - 20 + 'px';
              
            }

            // resize top row cell
            // let topHeight = parseFloat(window.getComputedStyle(document.querySelector('#DataCollected')).height) - 1 + "px";
            let topHeight = document.querySelector('#sticker > th:first-child').offsetHeight - 1 + 'px';
            let topWidth = document.querySelector('#sticker > th:first-child').offsetWidth + 'px';
            document.querySelector('.fixed-header-column > .table-header').style.height = topHeight;
            document.querySelector('.fixed-header-column > .table-header').style.width = topWidth;

            // resize the previously created sticky header
            let originalHeader = document.querySelector('tr#sticker');
            for (let i=0; i<originalHeader.children.length; i++) {
              rowElement.children[i].style.height = originalHeader.children[i].offsetHeight + 'px';
              rowElement.children[i].style.width = originalHeader.children[i].offsetWidth + 'px';
            }
          }, 10)


        });

      } else if (rowHeaders[i].parentElement.classList.contains('compare-parent-row')) {
        innerIndex++;
        innerSplits[innerIndex] = [];
        rowHeaders[i].parentElement.setAttribute('data-ref', innerIndex);

        rowHeaders[i].classList.add('compare-parent-header');

        rowHeaders[i].parentElement.addEventListener('click', function(e){
          let ref = e.currentTarget.getAttribute('data-ref'); // because splitIndex will likely change values before this call
          let section = document.querySelectorAll('.hidden-section')[ref]; // selects the section containing dependent clone cells
          
          
          section.classList.toggle('hidden-section--visible'); // expands or collapses

          document.querySelectorAll('.body-header .row-expand')[ref].classList.toggle('active');

          let oldCells = document.querySelectorAll('.compareChildRecord');
          let newCells = document.querySelectorAll('.body-header--child');

          setTimeout(function(){
            console.log(oldCells.length)
            for (let j=0;j<newCells.length;j++){
              // let refIndex = newCells[j].getAttribute('data-inner');

              let oldCellIndex = newCells[j].getAttribute('data-ref');

              newCells[j].style.height = oldCells[oldCellIndex].offsetHeight - 2 + 'px';
              newCells[j].style.width = oldCells[oldCellIndex].offsetWidth - 40 + 'px';

              console.log(oldCells[j].offsetHeight)
              console.log(newCells[j])
            }
          }, 10)
        })

        let ref = bodyRowIndex;
        bodyRowIndex++;
        rowHeaders[i].parentElement.addEventListener('mouseenter', function(){
          document.querySelectorAll('.body-header:not(.body-header--child)')[ref].classList.add('body-header--hover');
        })
        rowHeaders[i].parentElement.addEventListener('mouseleave', function(){
          document.querySelectorAll('.body-header:not(.body-header--child)')[ref].classList.remove('body-header--hover');
        })
        splitHeaders[splitIndex].push(rowHeaders[i]);

      } else if (rowHeaders[i].classList.contains('compareChildRecord')) {
        let ref = innerBodyIndex;
        innerBodyIndex++;
        rowHeaders[i].parentElement.addEventListener('mouseenter', function(){
          let clone = document.querySelector('.body-header--child[data-ref="' + ref + '"]');
          clone.classList.add('body-header--hover');
        })
        rowHeaders[i].parentElement.addEventListener('mouseleave', function(){
          let clone = document.querySelector('.body-header--child[data-ref="' + ref + '"]');
          clone.classList.remove('body-header--hover');
        })
        

        rowHeaders[i].setAttribute('data-ref', ref);

        innerSplits[innerIndex].push(rowHeaders[i]);

        // console.log(rowHeaders[i])

      } else {
        // each dependent cell is pushed into the previously instantiated array that represents its expand/collapse element
        let ref = bodyRowIndex;
        bodyRowIndex++;
        rowHeaders[i].parentElement.addEventListener('mouseenter', function(){
          document.querySelectorAll('.body-header:not(.body-header--child)')[ref].classList.add('body-header--hover');
        })
        rowHeaders[i].parentElement.addEventListener('mouseleave', function(){
          document.querySelectorAll('.body-header:not(.body-header--child)')[ref].classList.remove('body-header--hover');
        })
        splitHeaders[splitIndex].push(rowHeaders[i]);

      
      }
    }

    // based on model created above, create and insert column element

    var refIndex = 0;
    var compareHeaders = document.querySelectorAll('.compareGroup-header');

    var column = document.createElement('div');
    column.className = 'fixed-header-column';
    column.setAttribute('aria-hidden', 'true');

    var tableHeader = document.createElement('div');
    tableHeader.className = 'table-header';
    tableHeader.textContent = 'Data Collected';
    tableHeader.setAttribute('data-text', 'Data Collected');
    column.appendChild(tableHeader);

    console.log(innerSplits)

    for (let i=0;i<splitHeaders.length;i++) { // for each section
      
      var section = document.createElement('div');
      section.className = 'table-section';
      i === 0 ? section.classList.add('table-section--active') : null; // start the first section active
      column.appendChild(section);

      var rowHeader = document.createElement('div');
      rowHeader.className = 'row-header';
      rowHeader.textContent = compareHeaders[i].textContent;
      rowHeader.setAttribute('data-ref', i)
      rowHeader.addEventListener('click', function(e){compareHeaders[e.currentTarget.getAttribute('data-ref')].click();});
      section.appendChild(rowHeader);

      for (let j=0;j<splitHeaders[i].length;j++) {
        let originalCell = splitHeaders[i][j];
        // let currentHeight = parseFloat(window.getComputedStyle(originalCell).height) - 2 + "px";
        let currentHeight = originalCell.offsetHeight - 2 + 'px';
        let currentText = originalCell.innerHTML;
        
        var insertRow = document.createElement('div');
        insertRow.className = 'body-header';
        insertRow.style.height = currentHeight;
        insertRow.setAttribute('data-ref', refIndex);
        insertRow.innerHTML = currentText;
        insertRow.addEventListener('mouseenter', function columnEnter(e){
          originalCell.parentElement.classList.add('compare-row--hover');
          e.currentTarget.classList.add('body-header--hover');
        })
        insertRow.addEventListener('mouseleave', function columnLeave(e){
          originalCell.parentElement.classList.remove('compare-row--hover');
          e.currentTarget.classList.remove('body-header--hover');
        })

        section.appendChild(insertRow);

        if (originalCell.classList.contains('compare-parent-header')) {
          insertRow.classList.add('body-header--parent');
          insertRow.addEventListener('click', function() {
            originalCell.click();
          })

          var hiddenCells = innerSplits[originalCell.parentElement.getAttribute('data-ref')];
          var hiddenSection = document.createElement('div');
          hiddenSection.className = 'hidden-section';
          section.appendChild(hiddenSection);

          for (let k=0;k<hiddenCells.length/2;k++) { // /2 because, for some reason, hiddenCells has two copies of each cell
            let originalCell = hiddenCells[k];
            let currentText = originalCell.innerHTML;

            var insertCell = document.createElement('div');
            insertCell.className = 'body-header body-header--child';
            insertCell.innerHTML = currentText;
            insertCell.setAttribute('data-ref', originalCell.getAttribute('data-ref'));

            insertCell.addEventListener('mouseenter', function columnEnter(e){
              originalCell.parentElement.classList.add('compare-row--hover');
              e.currentTarget.classList.add('body-header--hover');
            })
            insertCell.addEventListener('mouseleave', function columnLeave(e){
              originalCell.parentElement.classList.remove('compare-row--hover');
              e.currentTarget.classList.remove('body-header--hover');
            })


            hiddenSection.appendChild(insertCell);

          }
        }

        refIndex++;
      }
    }

    // append column
    document.querySelector('.cedcd-table').appendChild(column);
  }

/* --------------------- multi-column sticky column ---------------------*/

  function createAndAppendColumns(table, offset) {
    var colOne;
    var colTwo;

    // get elements to be copied
    if (table.querySelector('tbody tr > th[rowspan]')) {
      colOne = table.querySelectorAll('tbody tr > th[rowspan]');
      colTwo = table.querySelectorAll('tbody tr th:not([rowspan])');
    } else {
      colOne = table.querySelectorAll('tbody tr > th:first-child');
      colTwo = table.querySelectorAll('tbody tr th + th');
    }

    // create column element
    
    var column = document.createElement('div');
    column.className = 'fixed-header-column  fixed-header-column--double';
    column.setAttribute('aria-hidden', 'true');

    offset ? null : column.style.top = colOne[0].offsetParent.offsetTop + 'px';


    var divOne = document.createElement('div');
    var divTwo = document.createElement('div');
    divOne.className = 'header-column__half';
    divTwo.className = 'header-column__half';
    column.appendChild(divOne);
    column.appendChild(divTwo);

    cloneCell(table.querySelector('thead tr th'), divOne, 'table-header', {'data-text': 'innerHTML'});
    cloneCell(table.querySelectorAll('thead tr th')[1], divTwo, 'table-header', {'data-text': 'innerHTML'});


    
    for (let i=0;i<colOne.length;i++) {
      cloneCell(colOne[i], divOne, 'column__cell-1');
    }

    for (let i=0;i<colTwo.length;i++) {
      cloneCell(colTwo[i], divTwo, 'column__cell-2');
    }

    table.appendChild(column);

    return column;
  }

  function cloneCell (target, parent, className, options) {
    let cell = document.createElement('div');
    cell.className = className;
    cell.style.height = target.offsetHeight + 'px';
    cell.style.width = target.offsetWidth + 'px';
    cell.innerHTML = target.innerHTML;

    for (let set in options) {
      cell.setAttribute(set, target[options[set]])
    }

    parent.appendChild(cell);
  }

/* --------------------- multi-column sticky column for multiple tables ---------------------*/


  // I think some race condition can cause the initial stickies render to happen before the
  // table cell sizes have finished reflowing. The following function solves this by
  // opening and closing the first collapsable section to rerender the stickies after
  // the table cells have reflowed.
  // if (path === 'compare') {
    // setTimeout(function(){ 
    //  document.querySelector('.row-header').click(); 
    //  document.querySelector('.row-header').click();
    // },10)
  // }


  

})();