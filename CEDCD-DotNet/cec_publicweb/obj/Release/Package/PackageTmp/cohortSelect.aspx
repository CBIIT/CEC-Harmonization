<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="cohortSelect.aspx.cs" Inherits="cec_publicweb.cohortSelect" EnableViewState="true" %>
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

    <!--javascript-->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
    <script src="/scripts/js_omni.js" type="text/javascript"></script>
    <script src="/scripts/bootstrap.js" type="text/javascript"></script>
    <script src="/scripts/bootstrap-tour.js" type="text/javascript"></script>
    <script type="text/javascript">
    
        // function to cause form postback when a checkbox is toggled
        function postBackByObject(mEvent) {
            var o;
            // Internet Explorer    
            if (mEvent.srcElement) {
                o = mEvent.srcElement;
            }
            // Netscape and Firefox
            else if (mEvent.target) {
                o = mEvent.target;
            }
            if (o.tagName == "INPUT" && o.type == "checkbox") {
                __doPostBack("webfilter_checkbox", o.id);
            }
        }
    </script>

    <!-- web fonts -->
    <link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />
    </head>
<body id="cohortSelectPage">
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

  <form runat="server">
    <div id="wrapper-inner">
      <usr:nciheader runat="server" />
      <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
        <usr:header runat="server" />
        <usr:mainNav runat="server" />
        <!-- MAIN CONTENT --------------------------------------------------- -->
        <div id="cedcd-main-content" class="row clearfix">
          <usr:contact runat="server" />

          <div id="cedcd-home-filter" class="filter-block home col-md-12">
            <div class="panel panel-default">
              <div class="panel-heading">
                <h2 class="panel-title">Filter</h2>
              </div>
              <div class="panel-body">
                <div class="filter row">
                  <div class="col-sm-3 filterCol">
                    <div class="filter-component">
                      <h3>Type of Participant</h3>
                      <!-- testing one two three -->
                      <prg:CECFilteringOptions id="wf_typeGender" runat="server" PathSeparator="|" RootCategoryID="106" RenderAsDropDown="true" />

                      <prg:CECFilteringOptions ID="wf_typeRace" runat="server" PathSeparator="|" RootCategoryID="107" RenderAsDropDown="true" />

                      <prg:CECFilteringOptions ID="wf_typeEthnic" runat="server" PathSeparator="|" RootCategoryID="108" RenderAsDropDown="true" />

                      <prg:CECFilteringOptions ID="wf_typeAge" runat="server" PathSeparator="|" RootCategoryID="109" RenderAsDropDown="true" />

                    </div>
                  </div>
                  <!--filterCol-->
                  <div class="filterCol col-sm-6">
                    <div class="filter-component">
                      <h3>Data and Specimens Collected</h3>
                      <div class="row">
                        <div class="col-sm-4"> 
                          <!-- custom filter control -->
                          <prg:CECFilteringOptions id="wf_collectData" runat="server" PathSeparator="|" RootCategoryID="101" RenderAsDropDown="true" />
                        </div>
                        <div class="col-sm-4"> 
                          <!-- custom filter control -->
                          <prg:CECFilteringOptions id="wf_collectSpecimen" runat="server" PathSeparator="|" RootCategoryID="105" RenderAsDropDown="true" />
                        </div>
                        <div class="col-sm-4"> 
                          <!-- custom filter control -->
                          <prg:CECFilteringOptions id="wf_cancerCollect" runat="server" PathSeparator="|" RootCategoryID="103" RenderAsDropDown="true"/>
                        </div>
                      </div>
                    </div>
                  </div>
                  <!--filterCol-->
                  <div class="filterCol col-sm-3 last">
                    <div class="filter-component">
                      <h3>Study Design</h3>
                      <!-- custom filter control -->
                      <prg:CECFilteringOptions id="wf_design" runat="server" PathSeparator="|" RootCategoryID="102" RenderAsDropDown="true" />
                    </div>
                  </div>
                  <!--filterCol--> 
                </div>
                <div class="row">
                  <div id="submitButtonContainer" class="col-sm-3 col-sm-offset-9">
                    <asp:LinkButton CssClass="btn-filter" ID="filterClear" runat="server" text="<span class='glyphicon glyphicon-remove'></span> Clear All" OnClick="FilterClear_Clicked" />
                    <asp:Button CssClass="btn btn-primary bttn_submit btn-filter" ID="filterEngage" runat="server" Text="Apply Filter" OnClick="FilterEngage_Clicked" />
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!--cedcd-home-filter-->
          <div id="cedcd-home-cohorts" class="home col-md-12">
            <div id="cedcd-home-cohorts-inner" class="col-md-12">
              <div class="table-inner col-md-12">
                <div class="table-description">
                  <p>Browse the list of cohorts or use the filter options to shorten the list of cohorts according to types of participants, data, and specimens.  Then select the cohorts about which you&apos;d like to see details and select the <b>Submit</b> button.</p>
                </div>
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
                          <asp:Label ID="cohortCount" runat="server" />
                      </li>
                    </ul>
                      <!--table-controls--> 
                  </div>
                  <div id="tableExport" class="col-md-2 col-md-offset-4">
                      <asp:LinkButton ID="exportTblBtn" runat="server" Text="Export Table <span class='glyphicon glyphicon-export'></span>" CommandName="export" />
                  </div>
                  <!--table-export--> 
                </div>
                <!-- start table -->
                <div class="cedcd-table home">
                  <asp:GridView GridLines="None" BorderStyle="None" ID="cohortGridView" 
                                runat="server" ShowHeaderWhenEmpty="true" 
                                AllowPaging="true" PagerSettings-Visible="false" PageSize="15" 
                                AllowSorting="True" EmptyDataText="Nothing to display"
                                EnableViewState="true"
                                OnPageIndexChanging="cohortGridView_PageChanging"
                                onsorting="cohortGridView_Sorting" 
                                onrowdatabound="cohortGridView_RowDataBound"
                                OnDataBound="cohortGridView_Bound"
                                CellPadding="5" UseAccessibleHeader="true" ShowHeader="true" >
                          <headerstyle CssClass="col-header"/>
                  </asp:GridView>
                </div>
                <!--cedcd-table-->
                <div class="table-pager" id="cohortPager" runat="server" enableviewstate="false" ></div>
                <!--table-pager--> 
              </div>
              <!--table-inner-->
              <%-- <div id="cedcd-home-results" class="col home">
                    <usr:sidebarNav id="nav" runat="server" />
              </div> --%>
              <!--cedcd-home-results--> 
              <!-- floating Submit Button -->
              <div id="floatingSubmitButtonContainer" class="row col-md-12 clearfix">
                <div class="submit-button">
                  <!-- <span class="floatingSubmisText">View Selections </span> -->
                  <asp:Button CssClass="btn btn-primary bttn_submit btn-filter" ID="submitBtn" runat="server" Text="Submit Cohort(s)" OnClick="Submit_Clicked" />
                </div>
              </div>
              <!-- end Floating Submit Button --> 
            </div>
              <!--cedcd-home-cohorts-inner--> 
          </div>
          <!--cedcd-home-cohorts--> 
        </div>
          <!--cedcd-main-content-->
        <usr:footer runat="server" />
      </div>
      <!--cedcd-bg-wrapper--> 
    </div>
    <!-- wrapper-inner --> 
    <!--javascript--> 
    <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="/scripts/jquery.sticky.js"></script>  
    <script src="/scripts/westat.js" type="text/javascript"></script> 
    <script type="text/javascript">
      $(function () { $('[data-toggle="tooltip"]').tooltip() })
      $(function () { $('[data-toggle="popover"]').popover() })

      $(document).ready(function () {
          //if ($('#cohortGridView').attr('has_results')) {
          //    $('html, body').animate({
          //        scrollTop: $("#cedcd-home-cohorts").offset().top
          //    }, 500);
          //}
      });
    </script> 

    <script type="text/javascript">
      var tour = new Tour({
          name:'tour',
          backdrop: true,
          onShown: function () {
              $(":input, a").not("div.popover-navigation :button").attr("tabindex", "-1");
              $("#cedcd-home-filter h3[tabindex='0']").attr("tabindex", "-1");
              $("button[data-role='next']").focus();
          },
          onEnd: function () {
              $(":input, a").removeAttr("tabindex");
              $("#cedcd-home-filter h3").attr("tabindex", "0");
              $("#helpFlag > a").focus();
          }
          });
      tour.addStep({
          element:'#filter-inner', 
          orphan:true,
          title:'1 of 3', 
              content:'<b>Filter</b> the list of cohorts by applying a set of <b>specific criteria.</b> Each <b>category</b> can be <b>expanded by clicking on the subject header.</b>  After a selections are made, select <b>Apply Criteria,</b> to see the list of cohorts that match to the selections.',
              placement:'top',
              prev: -1
        });
      tour.addStep({
          element:'#cohortGridView', 
          title:'2 of 3', 
          content:'<b>Select one or more</b> cohorts <b>to compare Cohort Details</b> across cohorts. ',
          placement: 'top'
        });
      tour.addStep({
          element: '#submitBtn',
          title:'3 of 3', 
          content:'Select <b>Submit</b> to see the detail information available.',
          placement: 'left',
          next:-1
        });
    </script>
    </form>
  <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
</body>
</html>