<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="list.aspx.cs" Inherits="cec_publicweb.input.List_Cohorts" %>
<%@ Register Src="/usrctrls/footer.ascx" TagName="footer" TagPrefix="usr" %>
<%@ Register Src="/usrctrls/header.ascx" TagName="header" TagPrefix="usr" %>
<%@ Register Src="/usrctrls/nci-header.ascx" TagName="nciheader" TagPrefix="usr" %>
<%@ Register Src="/usrctrls/contact.ascx" TagName="contact" TagPrefix="usr" %>
<%@ Register Src="/usrctrls/mainbarNav.ascx" TagName="mainNav" TagPrefix="usr" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en-us">
<head runat="server">
    <title>Cancer Epidemiology Descriptive Cohort Database (CEDCD)</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta http-equiv="Content-Type" content="text/html;charset=ISO-8859-1" /> 

    <!-- style links -->
    <link href="/css/bootstrap.css" rel="Stylesheet" type="text/css" />
    <link href="/css/main.css" rel="stylesheet" type="text/css" />
    <link href="/input/css/input.css" rel="stylesheet" type="text/css" />
    <link href="/input/css/jquery.fileupload.css" rel="stylesheet" type="text/css" />

    <!-- web fonts -->
	<link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />
	
    <!-- javascript -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
    <script src="/scripts/js_omni.js" type="text/javascript"></script>
    <script src="/scripts/bootstrap.js" type="text/javascript"></script>
    <script src="/input/scripts/input_omni.js" type="text/javascript"></script>
    <script src="/input/scripts/jquery.ui.widget.js"></script>
    <script src="/input/scripts/jquery.iframe-transport.js"></script>
    <script src="/input/scripts/jquery.fileupload.js"></script>

 
</head>
<body class="loggedin"> 
    <!--Accessibility feature to skip Navigation-->
	<div id="skipNav">
		<a href="#contact-main" class="skip">Skip navigation</a>
    </div><!--skipNav-->
    
    <form id="edit_cohort" runat="server">
    <div id="wrapper-inner">
        <usr:nciheader runat="server" />
        <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
            <usr:header runat="server" />
            <usr:mainNav runat="server" />
            

            <!-- MAIN CONTENT --------------------------------------------------- -->
            <usr:contact runat="server" /> 
              <div class="row">
                <div id="edit_intro" class="row" runat="server">
                  <div class="col-sm-12 no-gutters">
                    <h2 class="pg-title">Admin Section</h2>
                    <button id="addUserBtn" runat="server" class="pull-right btn btn-primary" type="button"><span class="glyphicon glyphicon-user"></span>Add User</button>
                </div>
                </div>
                <div id="cedcd-main-content">
                  <div class="row">
                    <div class="col-sm-8 no-gutters">
                      <ul id="navTabs" runat="server" name="navTabs" class="nav nav-tabs nav-justified-flex">
                      </ul>
                    </div>
                    <div class="col-sm-4 no-gutters">
                      <div class="admin-search-container">
                        <div class="input-group">
                          <asp:TextBox ID="inSearch" class="form-control" runat="server" TextMode="SingleLine" />
                          <span class="input-group-btn"><asp:LinkButton ID="btnSearch" OnClick="btnSearch_Click" class="btn btn-adminsearch" runat="server" Text="Search"><span class="glyphicon glyphicon-search"></span></asp:LinkButton></span>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div id="section" runat="server" class="clearfix">
                      <asp:GridView ID="cohortList" CssClass="table table-bordered table-hover table-responsive" runat="server" ShowHeaderWhenEmpty="true" 
                          EmptyDataText="Nothing to show" UseAccessibleHeader="true" ShowHeader="true" AllowSorting="true" >
                        <headerstyle CssClass="col-header"/>
                      </asp:GridView>
                  </div>                
              </div><!--cedcd-main-content-->
            </div> <!-- end row-->
            <usr:footer runat="server" />

        </div><!--cedcd-bg-wrapper--> 
    </div><!-- wrapper-inner -->
    </form>    

    <!-- popup modals -->
    <div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="new_user_dialog">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h2 class="modal-title">New User</h2>
          </div>
          <div class="modal-body">
            <div class="row">
              <div class="col-sm-12" id="user_type_section">
                  <p>Select User Type:</p>
                  <ul class="list-unstyled">
                      <li><input type="radio" name="user_type" id="user_type_cohort_rep" value="cohort_rep" /> <label for="user_type_cohort_rep">Cohort Representative</label></li>
                      <li><input type="radio" name="user_type" id="user_type_reviewer" value="reviewer" /> <label for="user_type_reviewer">NCI Reviewer</label></li>
                      <li><input type="radio" name="user_type" id="user_type_admin" value="admin" /> <label for="user_type_admin">Admin</label></li>
                  </ul>
              </div>

              <div class="col-sm-8" id="user_details" style="display:none;">
                  <p>Please Enter the Following:</p>
                  <div id="user_details_cohort_rep_fields"  style="display:none;">
                      <div class="form-group">
                          <label for="user_details_cohort_name" class="sr-only">Cohort Name</label>
                          <input class="form-control" type="text" placeholder="Cohort Name" id="user_details_cohort_name" />
                        </div>

                      <div class="form-group">
                            <label for="user_details_cohort_acronym" class="sr-only">Cohort Acronym</label>
                            <input class="form-control" type="text" placeholder="Cohort Acronym" id="user_details_cohort_acronym" />
                      </div>
                  </div>
                  <div id="user_details_username_field" style="display:none;" class="form-group">
                      <label for="user_details_username" class="sr-only">Username</label>
                      <input class="form-control" type="text" placeholder="Username" id="user_details_username" />
                  </div>
                  <div class="form-group">
                      <label for="user_details_displayname" class="sr-only">Display Name</label>
                      <input class="form-control" type="text" placeholder="Display Name" id="user_details_displayname" />
                  </div>
                  <div class="form-group">
                      <label for="user_details_email" class="sr-only">Email Address</label>
                      <input class="form-control" type="text" placeholder="Email Address" id="user_details_email" />
                  </div>
                  <div class="form-group">
                      <label for="user_details_password" class="sr-only">Password</label>
                      <input class="form-control" type="password" placeholder="Password" id="user_details_password" />
                      <small>Password must contain at least 8 characters, 1 uppercase letter, 1 lowercase letter, and 1 number</small>
                  </div>
              </div>
            </div>
            <div class="pull-right button-container">
              <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
              <button type="button" class="btn btn-primary" id="submitUserBtn">Submit</button>
            </div>           
          </div>
          <div class="modal-footer">
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->

    <!-- popup modals -->
    <div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="edit_user_dialog">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h2 class="modal-title">Edit User: <span id="edit_label_username"></span></h2>
          </div>
          <div class="modal-body">
            <div class="row">
              <div class="col-sm-8">
                <div class="form-group">
                    <label for="edit_details_displayname" class="sr-only">Display Name</label>
                    <input class="form-control" type="text" placeholder="Display Name" id="edit_details_displayname" />
                </div>
                <div class="form-group">
                    <label for="edit_details_email" class="sr-only">Email Address</label>
                    <input class="form-control" type="text" placeholder="Email Address" id="edit_details_email" />
                </div>
                <div class="form-group">
                    <label for="edit_details_password" class="sr-only">Password</label>
                    <input class="form-control" type="password" placeholder="Password" id="edit_details_password" />
                    <small>Password must contain at least 8 characters, 1 uppercase letter, 1 lowercase letter, and 1 number</small>
                </div>
              </div>
            </div>
            <div class="pull-right button-container">
              <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
              <button type="button" class="btn btn-primary" id="editUserBtn">Save Changes</button>
            </div>           
          </div>
          <div class="modal-footer">
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->

    <!-- popup modals -->
    <div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="quick_publish_dialog">
      <div class="modal-dialog bg-info text-info" role="document">
        <div class="modal-content">
          <div class="modal-header">
              <h2 class="modal-title"><span class="glyphicon glyphicon-alert"></span> Publish Record</h2>
          </div>
          <div class="modal-body">
            <div class="modal-text">
                Are you sure?
            </div>  
            <div class="pull-right button-container">
              <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
              <button type="button" class="btn btn-primary" id="quickPublishBtn">Publish</button>
            </div>           
          </div>
          <div class="modal-footer">
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->

    <!-- busy backdrop -->
    <div id="spinner-bg">
        <div class="spinner">
          <span class="glyphicon glyphicon-refresh spinny"></span>
        </div>
    </div>

    <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
     <!--javascript-->
    <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script>
    <script src="/scripts/westat.js" type="text/javascript"></script>    
    <script type="text/javascript">
        var jt = new CECInputFormService();

        $('div#reports a').click(function (event) {
            $('#spinner-bg').css('display', 'flex');
        });

        $('#inSearch').on('keypress', function (event) {
            if (event && event.keyCode == 13) {
                event.preventDefault();
                __doPostBack('btnSearch', '');
            }
        });

        $(':button[id*="quick_action"]').click(function (event) {
            var btn = $(this);
            $('#quickPublishBtn').attr('record_id', $(btn).attr('record_id'));
            $('#quickPublishBtn').attr('action', $(btn).attr('action'));
            
            if ($(btn).attr('action') == 'unpublish') {
                $('#quick_publish_dialog .modal-title').html('<span class="glyphicon glyphicon-alert"></span> Unpublish Record');
                $('#quickPublishBtn').text('Unpublish');
            }
            $('#quick_publish_dialog').modal('show');
        });
        $('#addUserBtn').click(function (event) {
            $('#new_user_dialog').modal('show');
        });
        $('#new_user_dialog input[type="text"]').blur(function (event) {
            var infield = $(this);
            var field_name = '';
            var table_name = '';
            switch ($(infield).attr('id')) {
                case 'user_details_cohort_name':
                    field_name = 'cohort_name';
                    table_name = 'tbl_web_cohorts_v4_0';
                    break;
                case 'user_details_cohort_acronym':
                    field_name = 'cohort_acronym';
                    table_name = 'tbl_web_cohorts_v4_0';
                    break;
                case 'user_details_username':
                    field_name = 'username';
                    table_name = 'tbl_user_accounts';
                    break;
                case 'user_details_email':
                    field_name = 'email';
                    table_name = 'tbl_user_accounts';
                    break;
            }
            if (field_name != '') {
                //console.log(/[\w\._\-]+@[\w\._\-]+\.\w{2,}/.test($(infield).val()));
                if (field_name == 'email') {
                    if (/[\w\._\-]+@[\w\._\-]+\.\w{2,}/.test($(infield).val()) === false) {
                        $(infield).addClass('bg-danger text-danger');
                        $(infield).parent().addClass('bg-danger text-danger');
                        $('#submitUserBtn').prop("disabled", true);
                        return;
                    }
                    else {
                        $(infield).removeClass('bg-danger text-danger');
                        $(infield).parent().removeClass('bg-danger text-danger');
                        $('#submitUserBtn').removeProp('disabled');
                    }
                }

                if (field_name != 'email') {
                    jt.doesValueExist($.parseJSON($('#userToken').val()), table_name, field_name, $(infield).val(), function (resp) {
                        if (resp['error']) {
                            console.warn(resp['error']['message']);
                            alert(resp['error']['message']);
                        }
                        else {
                            var exists = resp['result'];
                            if (exists) {
                                $(infield).parent().append("<span class=\"glyphicon glyphicon-remove\"></span>");
                                $(infield).parent().addClass('bg-danger text-danger');
                                $(infield).addClass('bg-danger text-danger');
                                $('#submitUserBtn').prop("disabled", true);
                            }
                            else {
                                $(infield).siblings('span').remove();
                                $(infield).parent().removeClass('bg-danger text-danger');
                                $(infield).removeClass('bg-danger text-danger');
                                $('#submitUserBtn').removeProp('disabled');
                            }
                        }
                    });
                }
            }
        });
         $('#edit_user_dialog #edit_details_email').blur(function (event) {
            var infield = $(this);
            var field_name = '';
            var table_name = '';
            switch ($(infield).attr('id')) {
                case 'edit_details_email':
                    field_name = 'email';
                    table_name = 'tbl_user_accounts';
                    break;
            }
            if (field_name != '') {
                //console.log(/[\w\._\-]+@[\w\._\-]+\.\w{2,}/.test($(infield).val()));
                if (field_name == 'email') {
                    if (/[\w\._\-]+@[\w\._\-]+\.\w{2,}/.test($(infield).val()) === false) {
                        $(infield).addClass('bg-danger text-danger');
                        $(infield).parent().addClass('bg-danger text-danger');
                        $('#editUserBtn').prop("disabled", true);
                    }
                    else {
                        $(infield).removeClass('bg-danger text-danger');
                        $(infield).parent().removeClass('bg-danger text-danger');
                        $('#editUserBtn').removeProp('disabled');
                    }
                }

                /*jt.doesValueExist($.parseJSON($('#userToken').val()), table_name, field_name, $(infield).val(), function (resp) {
                    if (resp['error']) {
                        console.warn(resp['error']['message']);
                        alert(resp['error']['message']);
                    }
                    else {
                        var exists = resp['result'];
                        if (exists) {
                            $(infield).parent().append("<span class=\"glyphicon glyphicon-remove\"></span>");
                            $(infield).parent().addClass('bg-danger text-danger');
                            $('#editUserBtn').prop("disabled", true);
                        }
                        else {
                            $(infield).siblings('span').remove();
                            $(infield).parent().removeClass('bg-danger text-danger');
                            $('#editUserBtn').removeProp('disabled');
                        }
                    }
                });*/
            }
         });
         $('#edit_user_dialog').on('hidden.bs.modal', function (event) {
             $('#edit_user_dialog div').each(function () {
                 $(this).removeClass('bg-danger text-danger');
             });
             $('#edit_user_dialog input').each(function () {
                 $(this).val('');
                 $(this).removeClass('bg-danger text-danger');
             });
         });
        $('input:radio[name="user_type"]').change(function (event) {
            $('#user_details').show();
            if ($('input:radio[name="user_type"]:checked').val() == "cohort_rep") {
                $('#user_details_cohort_rep_fields').show();
                $('#user_details_username_field').hide();
            }
            else {
                $('#user_details_username_field').show();
                $('#user_details_cohort_rep_fields').hide();
            }
        });
        $('#new_user_dialog').on('hidden.bs.modal', function (event) {
            $('#user_details').hide();
            $('#user_details_cohort_rep_fields').hide();
            $('#user_details_username_field').hide();
            $('#new_user_dialog div').each(function () {
                $(this).removeClass('bg-danger text-danger');
            });
            $('#new_user_dialog input').each(function () {
                $(this).val('');
                $(this).removeClass('bg-danger text-danger');
            });
            $('input:radio[name="user_type"]:checked').removeProp("checked");
        });

        $('#quickPublishBtn').click(function (event) {
            var payload = {};
            payload.id = $(this).attr('record_id');
            if ($(this).attr('action') == "publish") {
                payload.published = 1;
                payload.status = 'published';
            }
            else {
                payload.published = 0;
                payload.status = 'unpublished';
            }

            jt.setCohortWebData($.parseJSON($('#userToken').val()), payload.id, JSON.stringify(payload), function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    $('#quick_publish_dialog').modal('hide');
                    location.reload();
                }
            });
        });
        $('#submitUserBtn').click(function (event) {
            var error_condition = false;
            if ($('input:radio[name="user_type"]:checked').val() == "cohort_rep") {
                if ($('#user_details_cohort_acronym').val() == '') {
                    $('#user_details_cohort_acronym').parent().addClass('bg-danger text-danger');
                    error_condition = true;
                }
                if ($('#user_details_cohort_name').val() == '') {
                    $('#user_details_cohort_name').parent().addClass('bg-danger text-danger');
                    error_condition = true;
                }
            }
            else {
                if ($('#user_details_username').val() == '') {
                    $('#user_details_username').parent().addClass('bg-danger text-danger');
                    error_condition = true;
                }
            }

            if ($('#user_details_displayname').val() == '') {
                $('#user_details_displayname').parent().addClass('bg-danger text-danger');
                error_condition = true;
            }
            if ($('#user_details_email').val() == '') {
                $('#user_details_email').parent().addClass('bg-danger text-danger');
                error_condition = true;
            }
            if ($('#user_details_password').val() == '') {
                $('#user_details_password').parent().addClass('bg-danger text-danger');
                error_condition = true;
            }

            if (error_condition)
                return;

            var nuser = {};
            if ($('input:radio[name="user_type"]:checked').val() == "cohort_rep") {
                nuser.user_name = $('#user_details_cohort_acronym').val(); 
                nuser.display_name = $('#user_details_cohort_name').val() + "|" + $('#user_details_displayname').val();
                nuser.access_level = 100;
            }
            else {
                nuser.user_name = $('#user_details_username').val();
                nuser.display_name = $('#user_details_displayname').val();
                if ($('input:radio[name="user_type"]:checked').val() == "admin") {
                    nuser.access_level = 300;
                }
                else {
                    nuser.access_level = 200;
                }
            }
            nuser.email = $('#user_details_email').val();

            jt.createUser($.parseJSON($('#userToken').val()), nuser, function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    var userData = resp['result'];
                    nuser.password = CryptoJS.SHA256($('#user_details_password').val()).toString();
                    jt.setUserPassword($.parseJSON($('#userToken').val()), userData.userid, nuser.password, function (resp) {
                        if (resp['error']) {
                            console.warn(resp['error']['message']);
                            alert(resp['error']['message']);
                        }
                        else {
                            location.reload();
                        }
                    });
                }
            });
        });
        $(':button[id*="unlock_user"]').click(function (event) {
            var btn = $(this);
            jt.unlockUserAccount($.parseJSON($('#userToken').val()), $(btn).attr('uid'), function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    location.reload();
                }
            });
        });
        $(':button[id*="deactivate_user"]').click(function (event) {
            var btn = $(this);
            jt.lockUserAccount($.parseJSON($('#userToken').val()), $(btn).attr('uid'), function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    location.reload();
                }
            });
        });
        $(':button[id*="modify_user"]').click(function (event) {
            var btn = $(this);
            jt.getUserInformationByUserID($.parseJSON($('#userToken').val()), $(btn).attr('uid'), function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    var userData = resp['result'];
                    $('#edit_label_username').html(userData.user_name);
                    $('#edit_details_displayname').val(userData.display_name);
                    $('#edit_details_email').val(userData.email);
                    $('#edit_user_dialog').attr('uid', userData.userid);

                    $('#edit_user_dialog').modal('show');
                }
            });
        });
        $('#editUserBtn').click(function (event) {
            var error_condition = false;
            if ($('#edit_details_displayname').val() == '') {
                $('#edit_details_displayname').parent().addClass('bg-danger text-danger');
                error_condition = true;
            }
            if ($('#edit_details_email').val() == '') {
                $('#edit_details_email').parent().addClass('bg-danger text-danger');
                error_condition = true;
            }

            if (error_condition)
                return;

            var nuser = {};
            nuser.userid = $('#edit_user_dialog').attr('uid');
            nuser.display_name = $('#edit_details_displayname').val();
            nuser.email = $('#edit_details_email').val();
            jt.setUserInformation($.parseJSON($('#userToken').val()), nuser, function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                    return;
                }
                var userData = resp['result'];
                if ($('#edit_details_password').val() != '') {
                    nuser.password = CryptoJS.SHA256($('#edit_details_password').val()).toString();
                    jt.setUserPassword($.parseJSON($('#userToken').val()), userData.userid, nuser.password, function (resp) {
                        if (resp['error']) {
                            console.warn(resp['error']['message']);
                            alert(resp['error']['message']);
                        }
                        else {
                            location.reload();
                        }
                    });
                }
                else {
                    location.reload();
                }
            });
        });
    </script>

</body>
</html>
