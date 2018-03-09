<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="biospecimen.aspx.cs" Inherits="cec_publicweb.biospecimen" %>
<%@ Register Src="~/usrctrls/footer.ascx" TagName="footer" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/header.ascx" TagName="header" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/nci-header.ascx" TagName="nciheader" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/mainbarNav.ascx" TagName="mainNav" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/contact.ascx" TagName="contact" TagPrefix="usr" %>
<%@ Register Namespace="cec_publicweb" Assembly="cec_publicweb" TagPrefix="cec"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en-us">
<head runat="server">
<title>Cancer Epidemiology Descriptive Cohort Database (CEDCD)</title>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />

<!-- style links -->
<link href="/css/bootstrap.css" rel="Stylesheet" type="text/css" />
<link href="/css/bootstrap-tour.css" rel="stylesheet" type="text/css" />
<link href="/css/main.css" rel="stylesheet" type="text/css" />

<!-- web fonts -->
<link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />

<!-- javascript -->
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
<script src="/scripts/js_omni.js" type="text/javascript"></script>
<script src="/scripts/bootstrap.js" type="text/javascript"></script>
<script src="/scripts/bootstrap-tour.js" type="text/javascript"></script>
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

<form runat="server">
  <div id="wrapper-inner">
    <usr:nciheader runat="server" />
    <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
      <usr:header runat="server" />
      <usr:mainNav runat="server" />
      
      <!-- MAIN CONTENT --------------------------------------------------- -->
      <div id="cedcd-main-content" class="row">
        <usr:contact runat="server" /> 

        <!--<h1 class="table-title">Biospecimen Counts</h1>--> 
        <!-- filter block -->
        <div id="filter-block" class="filter-block col-md-12">
          <div class="panel panel-default">
            <div class="panel-heading">
              <h2 class="panel-title">Specify</h2>
            </div>
            <div class="panel-body">
              <div class="row">
                <div class="col-sm-4 filterCol">
                  <h3>Specimen Type</h3>
                  <cec:CECSpecimenCheckBoxList ID="select_specimenTypes" runat="server" />
                </div>
                <div class="col-sm-4 filterCol">
                  <h3>Cancer Type</h3>
                  <cec:CECCancerCheckBoxList ID="select_cancer" runat="server"  ShowAllCancerOption="true" ShowNoCancerOption="true" />
                </div>
                <div class="col-sm-4 filterCol last">
                  <h3>Cohorts</h3>
                  <cec:CECCohortCheckBoxList ID="select_cohort" runat="server" />
                </div>
              </div>
              <div class="row">
                <div id="submitButtonContainer" class="col-sm-3 col-sm-offset-9">
                  <asp:LinkButton CssClass="btn-filter" ID="filterClear" runat="server" text="<span class='glyphicon glyphicon-remove'></span> Clear All" OnClick="ClearOptions_Clicked" />
                  <asp:Button ID="submitBtn" class="btn btn-primary bttn_submit" runat="server" Text="Submit" OnClick="Submit_Clicked" />
                </div>
              </div>
            </div>
          </div>
        </div>
        <!-- filter block -->
        <div id="data-table" class="level2 col-md-12">
          <div id="cedcd-cohorts-inner" class="col-md-12">
            <div class="table-inner col-md-12">
              <div class="table-description">
                <p>To display biospecimens across cohorts, specify the Specimen Type, one or more Cancer Type/All Cancers/No Cancer, and Cohort(s) and then select the submit button. All fields are required. A table will display the number of biospecimens across the selected cohorts by cancer type.</p>
              </div>
              <div class="tableTopMatter row">
                <div id="tableLegend" class="col-md-10">
                  <p>N/A: Not Applicable; N/P: Not Provided</p>
                </div>
                <!-- table legend -->
                <div id="tableExport" class="col-md-2">
                  <asp:LinkButton ID="exportTblBtn" runat="server" Text="Export Table <span class='glyphicon glyphicon-export'></span>" CommandName="export" />
                </div>
                <!--table-export--> 
              </div>
              <div class="clearFix"></div>
              
              <!-- start table -->
              <div class="cedcd-table">
                <asp:GridView ID="bioGridView" runat="server" ShowHeaderWhenEmpty="true" AllowSorting="false" EmptyDataText="No Results"
                             onrowdatabound="bioGridView_RowDataBound"
                             UseAccessibleHeader="true" ShowHeader="true" >
                  <headerstyle CssClass="col-header"/>
                </asp:GridView>
              </div>
              <!-- cedcd-table --> 
            </div>
            <!-- table-inner --> 
          </div>
        </div>
        <!-- data-table --> 
      </div>
      <!-- main-content -->
      
      <div class="clearFix"></div>
      <usr:footer runat="server" />
      <div class="clearFix"></div>
    </div>
    <!--cedcd-bg-wrapper--> 
  </div>
  <!-- wrapper-inner --> 
  
  <!--javascript--> 
  <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script> 
  <script src="/scripts/jquery.sticky.js" type="text/javascript"></script> 
  <script src="/scripts/westat.js" type="text/javascript"></script> 
  <script type="text/javascript">
      $(document).ready(function () {
          enableSubmit();

          if ($('#bioGridView').attr('has_results')) {
              $('html, body').animate({
                  scrollTop: $("#bioGridView").offset().top
                  }, 500);
          }
      });

      $('#filter-block').change(function (event) {
          enableSubmit();
      });

      function enableSubmit() {
          if ($('#cancer_options_list input:checked').length > 0 && $('#specimen_options_list input:checked').length > 0 && $('#cohort_list_container input:checked').length > 0) {
              $('#submitBtn').removeAttr('disabled');
          }
          else {
              $('#submitBtn').attr('disabled', 'disabled');
          }
      }
  </script> 
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
		        $("button[data-role='end']").focus();
		    },
		    onEnd: function () {
		        $(":input, a").removeAttr("tabindex");
		        $("#helpFlag > a").focus();
		    }
		    });

	tour.addStep({
		element: '#filter-block',
		orphan: true,
		title: 'Biospecimen Help',
		template: '<div class="popover start" role="tooltip"> <div class="arrow"></div> <h3 class="popover-title" style="font-weight:700; font-size:110%;"></h3> <div class="popover-content"></div> <div class="popover-navigation"><button class="btn-popover-close" data-role="end">Close</button> </div>  </div>',
		content: 'Starting with Gender, <b>specify one or more</b> participant characteristics from each category and select <b>cohorts,</b> then select <b>Submit</b> to proceed to a table of biospecimen counts across the selected cohorts.',
		placement: "top"
	});
</script>
</form>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
</body>
</html>
