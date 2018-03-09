<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="select.aspx.cs" Inherits="cec_publicweb.summary" EnableViewState="true" %>
<%@ Register Src="~/usrctrls/footer.ascx" TagName="footer" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/header.ascx" TagName="header" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/nci-header.ascx" TagName="nciheader" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/mainbarNav.ascx" TagName="mainNav" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/contact.ascx" TagName="contact" TagPrefix="usr" %>
<%@ Register Namespace="cec_publicweb" Assembly="cec_publicweb" TagPrefix="prg"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en-us">
    <head runat="server">
    <title>Cancer Epidemiology Descriptive Cohort Database (CEDCD)</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />

    <!-- style links -->
    <link href="/css/bootstrap-tour.css" rel="stylesheet" type="text/css" />
    <link href="/css/bootstrap.css" rel="Stylesheet" type="text/css" />
    <link href="/css/main.css" rel="stylesheet" type="text/css" />

    <!-- web fonts -->
    <link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />

    <!--javascript-->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
    <script src="/scripts/js_omni.js" type="text/javascript"></script>
    <script src="/scripts/bootstrap.js" type="text/javascript"></script>
    <script src="/scripts/bootstrap-tour.js" type="text/javascript"></script>
    </head>
    <body id="selectPage">
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
<div id="skipNav"> <a href="#cedcd-home-filter" class="skip">Skip navigation</a> </div>
<!--skipNav-->
        <form  runat="server">
      <div id="wrapper-inner">
    <usr:nciheader runat="server" />
    <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
          <usr:header runat="server" />
          <usr:mainNav runat="server" />
          
        
          <!-- MAIN CONTENT --------------------------------------------------- -->
          <div id="cedcd-main-content" class="row">

        <usr:contact runat="server" />
        <p class="welcome">Welcome! Below is the list of cohorts participating in the Cancer Epidemiology Descriptive Cohort Database (CEDCD). Search for a cohort by name or select a cohort to view a brief description and contact information. If you want to know more about one or more cohorts, select one of the options from the menu at the top.</p>

        <div id="cedcd-home-filter" class="home col-md-12">
          <div class="search-wrapper col-md-12">
            <label for="inKeyword">Search for Cohorts by name or acronym</label>
            <!-- note that for some reason this all has to stay together on one line... otherwise the button falls off to the next line -->
            <span class="searchField"><asp:TextBox ID="inKeyword" runat="server" TextMode="SingleLine" Text="Search for Cohorts by name or acronym" OnFocus="(function(){ inKeyword.value=''; })();" OnKeyDown="(function(event){ if (event && event.keyCode == 13) { event.preventDefault(); WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('btKeyword', '', false, '', '#', false, true)) }})(event);" /></span><span class="searchBttn"><asp:LinkButton ID="btKeyword" PostBackUrl="#" runat="server" Text="" OnClick="KeywordSearch_Engage"><span class="glyphicon glyphicon-search"></span></asp:LinkButton></span>
          </div>
          <!--search-wrapper--> 
        </div>
        <!--cedcd-home-filter-->
        
        <div id="cedcd-home-cohorts" class="home col-md-12">
          <div id="cedcd-home-cohorts-inner" class="col-md-12">
            <div class="table-inner col-md-12">
              <div class="tableTopMatter row">
                <div id="tableControls" class="col-md-6">
                      <ul class="table-controls">
                    <%--<li><label class="invisibleLabel" for="cohortVisibleCount">Rows Per Page</label>
                                <asp:DropDownList ID="cohortVisibleCount" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cohortVisibleCount_Changed" CssClass="button-white">
                                    <asp:ListItem Text="15" Value="15" />
                                    <asp:ListItem Text="25" Value="25" />
                                    <asp:ListItem Text="50" Value="50" />
                                    <asp:ListItem Text="75" Value="75" />
                                    <asp:ListItem Text="All" Value="1000" />
                                </asp:DropDownList></li> --%>
                    <li class="total">
                          <asp:Label ID="summaryCount" runat="server" />
                        </li>
                  </ul>
                      <!--table-controls--> 
                    </div>
                <div id="tableExport" class="col-md-2 col-md-offset-4">
                      <asp:LinkButton ID="exportTblBtn" runat="server" Text="Export Table <span class='glyphicon glyphicon-export'></span>" CommandName="export" />
                    </div>
                <!--table-export--> 
              </div>
                  <div class="clearFix"></div>
                  
                  <!-- start table -->
                  <div class="cedcd-table home">
                <asp:GridView GridLines="None" BorderStyle="None" ID="summaryGridView" runat="server" ShowHeaderWhenEmpty="true" 
                                AllowPaging="true" PagerSettings-Visible="false" PageSize="15" AllowSorting="True" EmptyDataText="Nothing to display"
                                EnableViewState="true"
                                onsorting="summaryGridView_Sorting"
                                OnPageIndexChanging="summaryGridView_PageChanging"
                                onrowdatabound="summaryGridView_RowDataBound"
                                OnDataBound="summaryGridView_Bound"
                                UseAccessibleHeaders="true"
                                ShowHeaders="true"
                                CellPadding="5">
                      <headerstyle CssClass="col-header"/>
                    </asp:GridView>
              </div>
                  <!--cedcd-table-->
                  <div class="table-pager" id="summaryPager" runat="server" enableviewstate="false" ></div>
                  <!--table-pager--> 
                </div>
            <!--table-inner--> 
            
          </div>
              <!--cedcd-home-cohorts-inner--> 
            </div>
        <!--cedcd-home-cohorts-->
        
        <div class="clearFix"></div>
      </div>
          <!--cedcd-main-content-->
               
          
          <div class="clearFix"></div>
          <div class="clearFix"></div>
          <usr:footer runat="server" />
          <div class="clearFix"></div>
        </div>
    <!--cedcd-bg-wrapper--> 
           
  </div>
      <!-- wrapper-inner --> 
            </form>
      
      <!--javascript--> 
      <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script> 
      <script src="/scripts/westat.js" type="text/javascript"></script> 
      <script type="text/javascript" src="./scripts/jquery.sticky.js"></script> 
      <script type="text/javascript">
 $(function () { $('[data-toggle="tooltip"]').tooltip() })
 $(function () { $('[data-toggle="popover"]').popover() })
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
		
tour.addStep({
		element:'#summaryGridView', 
		orphan:true,
		title:'1 of 3', 
        /*template: '<div class="popover start" role="tooltip"> <div class="arrow"></div> <h3 class="popover-title" style="font-weight:700; font-size:110%;"></h3> <div class="popover-content"></div> <div class="popover-navigation"> <div class="btn-group"> <button class="btn btn-sm btn-default btn-popover prev" data-role="prev">No thanks</button> <button class="btn btn-sm btn-default btn-popover next" data-role="next">Start</button> <button class="btn btn-sm btn-default" data-role="pause-resume" data-pause-text="Pause" data-resume-text="Resume">Pause</button> </div> <button class="btn btn-sm btn-default btn-popover-close" data-role="end">x</button> </div> </div>', */
		content:'This table lists all the <b>participating cohorts,</b> which can be <b>sorted by cohort name or acronym.</b>',
        placement:'top',
        prev: -1
	    });

tour.addStep({
		element:'#inKeyword', 
		title:'2 of 3', 
		content:'You can also <b>search</b> for a specific cohort <b>by name or acronym.</b>',
		placement: 'top'
	});

tour.addStep({
		element:'#exportTblBtn', 
		title:'3 of 3', 
		content:'<b>Table data</b> can be <b>exported</b> in an <b>Excel</b> format.  This is available on every table.',
		placement: 'left'
	});
</script>

<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
</body>
</html>