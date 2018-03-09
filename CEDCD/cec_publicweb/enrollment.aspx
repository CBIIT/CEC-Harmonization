<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="enrollment.aspx.cs" Inherits="cec_publicweb.enrollment" %>
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
<link href="/css/chosen.css" rel="Stylesheet" type="text/css" />
<link href="/css/main.css" rel="stylesheet" type="text/css" />

<!-- web fonts -->
<link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />

<!-- javascript -->
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
<script src="/scripts/js_omni.js" type="text/javascript"></script>
<script src="/scripts/bootstrap.js" type="text/javascript"></script>
<script src="/scripts/bootstrap-tour.js" type="text/javascript"></script>
</head>
<body class="enrollment">
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
<div id="skipNav"> <a href="#cedcd-main-content" class="skip">Skip navigation</a> </div>
<!--skipNav-->

<form runat="server">
  <div id="wrapper-inner">
    <usr:nciheader runat="server" />
    <div id="cedcd-bg-wrapper"> 
      <!--for bg color/pattern-->
      <usr:header runat="server" />
      <usr:mainNav runat="server" />
      
      <!-- MAIN CONTENT --------------------------------------------------- -->
      <div id="cedcd-main-content" class="row">
        <usr:contact runat="server" /> 

        <!--<h1 class="table-title">Enrollment Counts</h1>--> 
        
        <!-- filter block -->
        <div id="filter-block" class="filter-block col-md-12">
          <div class="panel panel-default">
            <div class="panel-heading">
              <h2 class="panel-title">Specify</h2>
            </div>
            <div class="panel-body">
              <div class="filter row">
                <div class="col-sm-3 filterCol">
                  <div id="gender" class="filter-component" runat="server">
                    <h3>Gender</h3>
                    <cec:CECGenderCheckBoxList runat="server" ID="gender_list" />
                    <!-- <ul>
                      <li>
                        <label for="cb_gmale">
                          <asp:CheckBox runat="server" ID="cb_gmale" cssclass="filter-component-input" />
                          Male </label>
                      </li>
                      <li>
                        <label for="cb_gfemale">
                          <asp:CheckBox runat="server" ID="cb_gfemale" cssclass="filter-component-input" />
                          Female </label>
                      </li>
                      <li>
                        <label for="cb_gunknown">
                          <asp:CheckBox runat="server" ID="cb_gunknown" class="filter-component-input" />
                          Unknown </label>
                      </li>
                    </ul>-->
                  </div>
                  <!-- gender-area --> 
                </div>
                <div class="col-sm-3 filterCol">
                  <div id="race" class="filter-component" runat="server">
                    <h3>Race</h3>
                      <cec:CECRaceCheckBoxList runat="server" ID="race_list" />
                    <!--<ul>
                      <li>
                        <label for="cb_rnative">
                          <asp:CheckBox runat="server" ID="cb_rnative" cssclass="filter-component-input" />
                          American Indian/Alaskan Native </label>
                      </li>
                      <li>
                        <label for="cb_rasian">
                          <asp:CheckBox runat="server" ID="cb_rasian" cssclass="filter-component-input" />
                          Asian </label>
                      </li>
                      <li>
                        <label for="cb_rblack">
                          <asp:CheckBox runat="server" ID="cb_rblack" cssclass="filter-component-input" />
                          Black or African American </label>
                      </li>
                      <li>
                        <label for="cb_rpi">
                          <asp:CheckBox runat="server" ID="cb_rpi" cssclass="filter-component-input" />
                          Native Hawaiian or Other Pacific Islander </label>
                      </li>
                      <li>
                        <label for="cb_rwhite">
                          <asp:CheckBox runat="server" ID="cb_rwhite" cssclass="filter-component-input" />
                          White </label>
                      </li>
                      <li>
                        <label for="cb_runknown">
                          <asp:CheckBox runat="server" ID="cb_runknown" cssclass="filter-component-input" />
                          Unknown </label>
                      </li>
                      <li>
                        <label for="cb_rmulti">
                          <asp:CheckBox runat="server" ID="cb_rmulti" cssclass="filter-component-input" />
                          More Than One Race </label>
                      </li>
                    </ul>-->
                  </div>
                  <!-- race-area --> 
                </div>
                <div class="col-sm-3 filterCol">
                  <div id="ethnicity" class="filter-component" runat="server">
                    <h3>Ethnicity</h3>
                      <cec:CECEthnicityCheckBoxList runat="server" ID="ethnicity_list" />
                    <!--<ul>
                      <li>
                        <label for="cb_enonhis">
                          <asp:CheckBox runat="server" ID="cb_enonhis" cssclass="filter-component-input" />
                          Non-Hispanic/Latino </label>
                      </li>
                      <li>
                        <label for="cb_ehis">
                          <asp:CheckBox runat="server" ID="cb_ehis" cssclass="filter-component-input" />
                          Hispanic/Latino </label>
                      </li>
                      <li>
                        <label for="cb_eunknown">
                          <asp:CheckBox runat="server" ID="cb_eunknown" cssclass="filter-component-input" />
                          Unknown </label>
                      </li>
                    </ul> -->
                  </div>
                  <!-- ethnicity-area --> 
                </div>
                <div class="col-sm-3 filterCol last">
                  <h3>Cohorts</h3>
                  <cec:CECCohortCheckBoxList ID="select_cohort" runat="server" />
                </div>
              </div>
              <div class="row">
                <div id="submitButtonContainer" class="col-sm-3 col-sm-offset-9">
                  <asp:LinkButton CssClass="btn-filter" ID="filterClear" runat="server" text="<span class='glyphicon glyphicon-remove'></span> Clear All" OnClick="ClearOptions_Clicked" />
                  <asp:Button CssClass="btn btn-primary bttn_submit" ID="submitBtn" runat="server" Text="Submit" OnClick="Submit_Clicked"  disabled="true"/>
                </div>
              </div>
            </div>
          </div>
        </div>
        <!-- end filter block -->
        <div id="data-table" class="level2 col-md-12">
          <div id="cedcd-cohorts-inner" class="col-md-12">
            <div class="table-inner col-md-12">
              <div class="table-description">
                <p>Specify the Gender, Race, Ethnicity, and Cohort(s) to see a table of the number of participants enrolled.  All fields are required.  A table will display the number of participants enrolled by gender, race and ethnicity across the selected cohorts.</p>
              </div>
              <!-- table-description -->
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
              <!-- start tabel -->
              <div class="cedcd-table clearfix">
                <asp:GridView ID="enrollTblSum" CssClass="enrollTbl" runat="server" Caption="Enrollment: Totals" ShowHeaderWhenEmpty="true"
                                           ClientIDMode="Static" AllowSorting="false" EmptyDataText="No Results"
                                           UseAccessibleHeader="true" onrowdatabound="enrolTblSumGridView_RowDataBound" ViewStateMode="Disabled">
                  <headerstyle CssClass="col-header"/>
                </asp:GridView>
                <a id="male" ></a>
                <asp:GridView ID="enrollTblMales" CssClass="enrollTbl" runat="server" Caption="Enrollment: Males" CaptionAlign="Top" ShowHeaderWhenEmpty="true"
                                           ClientIDMode="Static" AllowSorting="false" EmptyDataText="No Results"
                                           UseAccessibleHeader="true" onrowdatabound="enrolTblMalesGridView_RowDataBound" OnDataBound="enrolTblMalesGridView_DataBound" ViewStateMode="Disabled">
                  <headerstyle CssClass="col-header"/>
                </asp:GridView>
                <a id="female" ></a>
                <asp:GridView ID="enrollTblFemales" CssClass="enrollTbl" runat="server" Caption="Enrollment: Females" CaptionAlign="Top" ShowHeaderWhenEmpty="true"
                                           ClientIDMode="Static" AllowSorting="false" EmptyDataText="No Results"
                                           UseAccessibleHeader="true" onrowdatabound="enrolTblFemalesGridView_RowDataBound" OnDataBound="enrolTblFemalesGridView_DataBound">
                  <headerstyle CssClass="col-header"/>
                </asp:GridView>
                <a id="unknown" ></a>
                <asp:GridView ID="enrollTblUnknown" CssClass="enrollTbl" runat="server" Caption="Enrollment: Unknown / Not Reported" CaptionAlign="Top" ShowHeaderWhenEmpty="true"
                                           ClientIDMode="Static" AllowSorting="false" EmptyDataText="No Results"
                                           UseAccessibleHeader="true" onrowdatabound="enrolTblUnknownGridView_RowDataBound" OnDataBound="enrolTblUnknownGridView_DataBound">
                  <headerstyle CssClass="col-header"/>
                </asp:GridView>
              </div>
              <!--cedcd-table--> 
            </div>
            <!-- table-inner --> 
          </div>
        </div>
        <!-- data-table -->
        <div class="clearFix"></div>
      </div>
      <!--cedcd-main-content-->
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
  <script src="/scripts/chosen.jquery.js" type="text/javascript"></script> 
  <script src="/scripts/westat.js" type="text/javascript"></script> 
  <script type="text/javascript">
          $(document).ready(function () {
              enableSubmit();

              if ($('#enrollTblMales').attr('has_results') || $('#enrollTblFemales').attr('has_results') || $('#enrollTblUnknown').attr('has_results')) {
                  $('html, body').animate({
                      scrollTop: $("#data-table").offset().top
                  }, 500);
              }
          });

          $('#filter-block').change(function (event) {
              enableSubmit();
          });

          function enableSubmit() {
              if ($('#cohort_list_container input:checked').length > 0 && $('#gender input:checked').length > 0 && $('#race input:checked').length > 0 && $('#ethnicity input:checked').length > 0) {
                  $('#submitBtn').removeAttr('disabled');
              }
              else {
                  $('#submitBtn').attr('disabled', 'disabled');
              }
          }
  </script> 
  <script type="text/javascript">
    var config = {
        '.chosen-select': {},
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-results': { no_results_text: 'Nothing found!' },
        '.chosen-select-width': { width: "50px" }
    }
    for (var selector in config) {
        $(selector).chosen(config[selector]);
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
		element:'#filter-block', 
		orphan:true,
		title:'Enrollment Help', 
        template: '<div class="popover start" role="tooltip"> <div class="arrow"></div> <h3 class="popover-title" style="font-weight:700; font-size:110%;"></h3> <div class="popover-content"></div> <div class="popover-navigation"><button class="btn-popover-close" data-role="end">Close</button> </div>  </div>',
        content:'Starting with Gender, <b>specify one or more</b> participant characteristics from each category and select <b>cohorts,</b> then select <b>Submit</b> to proceed to a table of participants enrolled across the selected cohorts.',
        placement: "top"
	});
</script>
</form>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
</body>
</html>
