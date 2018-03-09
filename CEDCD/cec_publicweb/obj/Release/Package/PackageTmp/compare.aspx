<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="compare.aspx.cs" Inherits="cec_publicweb.compare" %>
<%@ Register Src="~/usrctrls/footer.ascx" TagName="footer" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/header.ascx" TagName="header" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/nci-header.ascx" TagName="nciheader" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/detailTabs.ascx" TagName="detailTabs" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/sidebarFilter.ascx" TagName="sidebarFilter" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/mainbarNav.ascx" TagName="mainNav" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/contact.ascx" TagName="contact" TagPrefix="usr" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en-us">
<head id="Head1" runat="server">
<title>Cancer Epidemiology Descriptive Cohort Database (CEDCD)</title>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />

<!-- style links -->
<link href="/css/bootstrap-tour.css" rel="stylesheet" type="text/css" />
<link href="/css/bootstrap.css" rel="Stylesheet" type="text/css" />
<link href="/css/main.css" rel="stylesheet" type="text/css" />

<!-- web fonts -->
<link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />

<!-- javascript -->
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
<script src="/scripts/bootstrap.js" type="text/javascript"></script>
<script src="/scripts/bootstrap-tour.js" type="text/javascript"></script>
<%--<script src="/Scripts/jquery.ba-throttle-debounce.js" language="javascript" type="text/javascript"></script>--%>
<script src="/scripts/jquery.sticky.js" type="text/javascript" ></script>
<%--<script src="/scripts/jquery.stickyheader.js" type="text/javascript" /> --%>
</head>
<body>
<script type="text/javascript">
    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
  m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

    ga('create', 'UA-62994246-1', 'auto');
    ga('send', 'pageview');

</script> 

<!--Accessibility feature to skip Navigation-->
<div id="skipNav"> <a href="#data-table" class="skip">Skip navigation</a> </div>
<!--skipNav-->

<form id="Form1" runat="server">
  <div id="wrapper-inner">
    <usr:nciheader ID="Nciheader1" runat="server" />
    <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
      <usr:header runat="server" />
      <usr:mainNav runat="server" />
      
      <!-- MAIN CONTENT --------------------------------------------------- -->
      <div id="cedcd-main-content" class="row">
        <usr:contact runat="server" />
        
        <!--<a class="back-to-filters" href="cohortSelect.aspx" data-toggle="tooltip" title="Go back to previous page"><span class="glyphicon glyphicon-chevron-left"></span><span>Back to Filter Options</span></a>--> <!-- This back option doesn't do what is expected. It should bounce back to the select page and the user should still be able to see the selected filter options. This goes back, but the filter options are not shown as selected, but are still applied to the cohort list.-->
        <!-- filters -->
        <div id="filterLabels" class="filter-block col-md-12 lockedFilter">
          <div class="panel panel-default">
            <div class="panel-heading">
              <h2 class="panel-title">Filter</h2>
            </div>
            <div class="panel-body">
              <div class="filter row">
                <div class="col-md-3 filterCol">
                  <ul class="results-nav" id="fl_include" runat="server" visible="false" />
                  
                </div>
                <div class="col-md-3 filterCol">
                  <ul class="results-nav" id="fl_collectData" runat="server" visible="false" />
                  
                  <ul class="results-nav" id="fl_collectSpecimen" runat="server" visible="false" />
                </div>
                <div class="col-md-3 filterCol">
                  <ul class="results-nav" id="fl_cancer" runat="server" visible="false" />
                  
                </div>
                <div class="col-md-3 filterCol last">
                  <ul class="results-nav" id="fl_design" runat="server" visible="false" />
                  
                </div>
              </div>
              <div class="row">
                <div id="submitButtonContainer" class="col-md-3 col-md-offset-9"> <a id="filterClear" class="btn-filter" href="./cohortSelect.aspx"><span class='glyphicon glyphicon-remove'></span> Clear All</a> </div>
              </div>
            </div>
          </div>
        </div>
        <!-- End Filters -->
        <div id="data-table" class="level2 col-md-12"> 
          <!--<h1 class="page-title">Cohort Overview</h1>-->
          
          <div id="table-header" class="">
            <div>
              <usr:detailTabs runat="server" id="detailTabs" />
            </div>
            <div id="table-intro" class="col-md-12">
              <h2 class="table-title"><span class="subtitle" runat="server" id="tabLabel" /></h2>
              <!-- updated 11/23/16 -->
              <div class="table-description">
                <p>The Cohort Overview compares the cohort design and the types of data and specimens collected across the cohorts you selected. To view more information about a specific cohort, select the acronym of the cohort at the top of the table.</p>
              </div>
              <!-- table-description --> 
            </div>
          </div>
          <!--table-header-->
          
          <div id="cedcd-cohorts-inner" class="col-md-12 activeArea">
            <div class="table-inner col-md-12">
              <div class="table-legend col-sm-9"> <span class="">N/A: Not Applicable; N/P: Not Provided</span> </div>
              <!-- table legend -->
              
              <div class="table-export col-sm-3">
                <asp:LinkButton ID="exportTblBtn" runat="server" Text="Export Table <span class='glyphicon glyphicon-export'></span>" CommandName="export" />
              </div>
              <!--table-export-->
              <div class="clearFix"></div>
              <!-- start tabel -->
              <div class="cedcd-table">
                <asp:GridView ID="compareGridView" AllowPaging="false" runat="server" 
                                ShowHeaderWhenEmpty="true" 
                                AllowSorting="false" 
                                EmptyDataText="No cohort selected. Please select one or more cohorts from the table on the 'Select Cohorts' page, and try again." 
                                OnRowDataBound="compareGridView_RowDataBound"
                                UseAccessibleHeader="true" ShowHeader="true">
                  <headerstyle CssClass="col-header"/>
                </asp:GridView>
              </div>
              <!-- cedcd-table --> 
            </div>
            <!-- table-inner --> 
          </div>
          <!-- data-table --> 
        </div>
      </div>
      <!-- main-content -->
      
      <div class="clearFix"></div>
      <usr:footer ID="Footer1" runat="server" />
      <div class="clearFix"></div>
    </div>
    <!--cedcd-bg-wrapper--> 
    
  </div>
  <!-- wrapper-inner --> 
  
  <!--javascript--> 
  <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script> 
  <script src="/scripts/js_omni.js" type="text/javascript"></script> 
  <script src="/scripts/westat.js" type="text/javascript"></script>
  <script type="text/javascript">
    $(function () { $('[data-toggle="tooltip"]').tooltip() });
    $(function () { $('[data-toggle="popover"]').popover() });

    $(document).ready(function () {    
        //        $("#sticker").sticky({topSpacing:0});

        if ($('#compareGridView').attr('has_results')) {
            $('html, body').animate({
                scrollTop: $("#data-table").offset().top
            }, 500);
        }
    });
</script> 
  <script type="text/javascript">
    var tour = new Tour({
		    name:'tour',
		    backdrop: true,
		    onShown: function () {
		        $(":input, a").not("div.popover-navigation :button").attr("tabindex", "-1");
		        $("button[data-role='next']").focus();
		    },
		    onEnd: function () {
		        $(":input, a").removeAttr("tabindex");
		        $("#helpFlag > a").focus();
		    }
		    });
		
//tour.addStep({
//		element:'', 
//		orphan:true,
//		title:'Welcome', 
//        template: '<div class="popover start" role="tooltip"> <div class="arrow"></div> <h3 class="popover-title" style="font-weight:700; font-size:110%;"></h3> <div class="popover-content"></div> <div class="popover-navigation"> <div class="btn-group"> <button class="btn btn-sm btn-default btn-popover prev" data-role="prev">No thanks</button> <button class="btn btn-sm btn-default btn-popover next" data-role="next">Start</button> <button class="btn btn-sm btn-default" data-role="pause-resume" data-pause-text="Pause" data-resume-text="Resume">Pause</button> </div> <button class="btn btn-sm btn-default btn-popover-close" data-role="end">x</button> </div> </div>',
      //		content:'This tour will walk you through the features on this site. View this tour at any time by selecting <strong>Help</strong> in the upper right corner.',
      //onShown: function () {
      //    $("button[data-role='next']").focus();
      //}
//	});
</script>
</form>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
</body>
</html>
