<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="cec_publicweb.input.Edit_Cohort" %>
<%@ Register Src="~/usrctrls/footer.ascx" TagName="footer" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/header.ascx" TagName="header" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/nci-header.ascx" TagName="nciheader" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/contact.ascx" TagName="contact" TagPrefix="usr" %>
<%@ Register Src="~/input/usrctrls/inputTabBar.ascx" TagName="inputTabBar" TagPrefix="usr" %>
<%@ Register Src="~/input/usrctrls/inputToolBar.ascx" TagName="inputToolBar" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/mainbarNav.ascx" TagName="mainNav" TagPrefix="usr" %>
<%@ Register Src="/input/usrctrls/editIntroBlock.ascx" TagName="editIntroBlock" TagPrefix="usr" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en-us">
<head runat="server">
    <title>Cancer Epidemiology Descriptive Cohort Database (CEDCD)</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta http-equiv="Content-Type" content="text/html;charset=ISO-8859-1" /> 

    <!-- style links -->
    <link href="/css/bootstrap.css" rel="Stylesheet" type="text/css" />
    <link href="/css/bootstrap-tour.css" rel="stylesheet" type="text/css" />
    <link href="/css/main.css" rel="stylesheet" type="text/css" />
    <link href="/input/css/input.css" rel="stylesheet" type="text/css" />
    <link href="/input/css/jquery.fileupload.css" rel="stylesheet" type="text/css" />

    <!-- web fonts -->
	<link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />
	
    <!-- javascript -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
    <script src="/scripts/js_omni.js" type="text/javascript"></script>
    <script src="/scripts/bootstrap.js" type="text/javascript"></script>
    <script src="/scripts/bootstrap-tour.js" type="text/javascript"></script>
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
            <usr:inputToolBar ID="edit_toolbar" runat="server" />
              <div class="clearfix">
                  <usr:editIntroBlock id="edit_intro" runat="server" class="clearfix" />
              <div id="cedcd-main-content">
                <div class="row">
                  <usr:inputTabBar ID="edit_tabbar" runat="server" />
                </div>

                <div id="section" class="clearfix" runat="server">
                    <span id="save_timestamp" runat="server"></span>
    
                </div>
                
                <input type="hidden" id="draftCohortId" value="" runat="server" />
                <input type="hidden" id="dirty_fields" value="" />
            </div><!--cedcd-main-content-->
            </div> <!-- end container-->
            <usr:footer runat="server" />

        </div><!--cedcd-bg-wrapper--> 
    </div><!-- wrapper-inner -->
    </form>

    <!-- popup modals, may move into input_omni.js -->
    <div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="attachmentModal">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h2 class="modal-title">Attachment</h2>
          </div>
          <div class="modal-body">
            <div class="row">
              <div class="col-sm-6">
                  <label>Attachment Type</label>
                  <ul class="list-unstyled">
                      <li><input type="radio" name="attachment_type" id="attachment_type_url" value="url" /> Website</li>
                      <li><input type="radio" name="attachment_type" id="attachment_type_file" value="file" /> Document</li>
                  </ul>
              </div>
              <div class="col-sm-6">
                  <label>Category</label>
                  <select class="form-control input-sm" id="document_type_id">
                      <option value="-1">Select one</option>
                      <option value="1">Questionnaires</option>
                      <option value="2">Main cohort protocol</option>
                      <option value="3">Data sharing policy</option>
                      <option value="4">Biospecimen sharing policy</option>
                      <option value="5">Publication (authorship) policy</option>
                  </select>
              </div>
              <hr />
            </div>
            <div class="row" id="div_url">
                <div class="col-sm-12">
                  <label>Web Address</label>
                  <input type="url" class="form-control" id="url" />
                </div>
            </div>
            <div class="row" id="div_file">
                <div class="col-sm-12">
                  <p>File Upload</p>
                    <span class="btn btn-success fileinput-button">
                        <i class="glyphicon glyphicon-plus"></i> Select file...
                        <input type="file" class="btn" id="fileupload" accept=".pdf" />
                    </span>
                    <div id="progress">
                        <div class="bar" style="width: 0%;"></div>
                    </div>
                </div>
            </div>
            <div class="pull-right button-container">
              <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
              <button type="button" class="btn btn-primary" id="save_attachment_edit" attachment_id="">Save</button>
            </div>           
          </div>
          <div class="modal-footer">
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->
    

    <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
    
    <!-- bootstrap tour -->
    <script type="text/javascript">
        var upperTourBoundry = "5";
        if ($('#submitBtn').length == 0) {
            upperTourBoundry = "4";
        }

        var tour = new Tour({
            name: 'tour',
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
            element: '#cedcd-main-content',
            title: '1 of ' + upperTourBoundry,
            content: 'This form contains all your previously submitted information.',
            placement: 'top',
            prev: -1
        });
        tour.addStep({
            element: '#inputTabs',
            title: '2 of ' + upperTourBoundry,
            content: "Select the content tabs and scroll through the pages to review your cohort's information.",
            placement: 'bottom'
        });
        tour.addStep({
            element: '#enableEditBtn',
            title: '3 of ' + upperTourBoundry,
            content: 'If you want to make changes select <b>Edit.</b>  This will make all the form fields editable.',
            placement: 'left'
        });
        if (upperTourBoundry == "5") {
            tour.addStep({
                element: '#saveBtn',
                title: '4 of ' + upperTourBoundry,
                content: '<p>As you make changes select <b>Save</b> to keep your changes.</p> <p><i>Be sure to select <b>Save</b> if you plan to logout and edit the form later.</i></p>',
                placement: 'left'
            });

            tour.addStep({
                element: '#submitBtn',
                title: '5 of ' + upperTourBoundry,
                content: 'Once you have made your changes, select <b>Submit</b> to send form to NCI for review.  You will receive and email with the result of their review.',
                placement: 'left',
                next: -1
            });
        }
        else {
            tour.addStep({
                element: '#saveBtn',
                title: '4 of ' + upperTourBoundry,
                content: '<p>As you make changes select <b>Save</b> to keep your changes.</p> <p><i>Be sure to select <b>Save</b> if you plan to logout and edit the form later.</i></p>',
                placement: 'left',
                next: -1
            });
        }
    </script>

    <!--javascript-->
    <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script>
    <script src="/scripts/westat.js" type="text/javascript"></script>
    <script type="text/javascript">
        var uploadfiles = [];
        var jt = new CECInputFormService();

        $('#section').change(function (event) {
            var inp = event.target;
            var fields = $('#dirty_fields').val();
            if (fields.indexOf($(inp).attr("id")) == -1) {
                fields = fields + $(inp).attr("id") + ';';
            }
            $('#dirty_fields').val(fields);

            evaluateSkips($(inp).attr("id"));
        });

        $('#attachments_table button[action="delete"]').click(function (event) {
            jt.deleteCohortAttachment($.parseJSON($('#userToken').val()), $(this).attr('id'), function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    location.reload();
                }
            });
        });

        $('#add[action="add"]').click(function (event) {
            $('#progress .bar').css('width', '0%');
            $('#attachment_type_url').prop('checked', true);
            $('#div_file').hide();
            $('#attachmentModal').modal('show');
        });

        $('input[name="attachment_type"]').click(function (event) {
            if ($(this).val() == "url") {
                $('#div_url').show();
                $('#div_file').hide();
            }
            else {
                $('#div_file').show();
                $('#div_url').hide();
            }
        });

        $('#attachments_table button[action="edit"]').click(function (event) {
            jt.getCohortAttachment($.parseJSON($('#userToken').val()), $(this).attr('id'), function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    var cohort_data = resp['result'][0];
                    if (cohort_data['attachment_type'] == 'url') {
                        $('#attachment_type_url').prop('checked', true);
                        $('#url').val(cohort_data['url']);

                        $('#div_url').show();
                        $('#div_file').hide();
                    }
                    else if (cohort_data['attachment_type'] == 'file') {
                        $('#attachment_type_file').prop('checked', true);

                        $('#div_file').hide();
                        $('#div_url').hide();
                    }
                    $('input[name="attachment_type"]').prop({ disabled: true });

                    $('#document_type_id').val(cohort_data['document_type_id']);
                    $('#save_attachment_edit').attr('attachment_id', cohort_data['id']);
                }
                $('#attachmentModal').modal('show');
            });
        });

        $('#save_attachment_edit').click(function (event) {
            if ($('#document_type_id').val() == -1) {
                console.log('document_type_id not defined');
                $('#document_type_id').parent().addClass(' field-required');
            }
            else if ($('#attachmentModal :radio[name="attachment_type"]:checked').val() == "url" && $('#url').val().length == 0) {
                console.log('url must be defined');
                $('#url').parent().addClass(' field-required');
            }
            else if ($('#attachmentModal :radio[name="attachment_type"]:checked').val() == "url" && /(https?:\/\/|\w+).+\.\w{2,}/g.test($('#url').val()) == false) {
                console.log('need good url');
                $('#url').parent().addClass(' bg-danger text-danger');
            }
            else if ($(this).attr("attachment_id") == "" && $('#attachmentModal :radio[name="attachment_type"]:checked').val() == "file" && uploadfiles.length == 0) {
                console.log('file must be defined');
                $('#div_file').addClass(' bg-danger text-danger');
            }
            else {
                if ($(this).attr("attachment_id") != "") {
                    jt.updateCohortAttachment($.parseJSON($('#userToken').val()), $(this).attr("attachment_id"), $('#document_type_id').val(), $('#url').val(), function (resp) {
                        if (resp['error']) {
                            console.warn(resp['error']['message']);
                            alert(resp['error']['message']);
                        }
                        else {
                            $('#attachmentModal').modal('hide');
                            location.reload();
                        }
                    });
                }
                else {
                    if (uploadfiles.length > 0) {
                        $.each(uploadfiles, function (i, val) {
                            jt.createCohortAttachment($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), $('#attachmentModal :radio[name="attachment_type"]:checked').val(), $('#document_type_id').val(), "", val, function (resp) {
                                if (resp['error']) {
                                    alert(resp['error']['message']);
                                }
                                else {
                                    $('#attachmentModal').modal('hide');
                                    location.reload();
                                }
                            });
                        });
                    }
                    else {
                        jt.createCohortAttachment($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), $('#attachmentModal :radio[name="attachment_type"]:checked').val(), $('#document_type_id').val(), $('#url').val(), "", function (resp) {
                            if (resp['error']) {
                                alert(resp['error']['message']);
                            }
                            else {
                                $('#attachmentModal').modal('hide');
                                location.reload();
                            }
                        });
                    }
                }
            }
        });

        $(function () {
            $('#fileupload').fileupload({
                url: '/input/uploader.ashx?id='+($.parseJSON($('#userToken').val()))["userid"],
                dataType: 'json',
                dropZone: null,
                done: function (e, data) {
                    $.each(data.files, function (index, file) {
                        console.log(file.name + ' uploaded');
                        uploadfiles.push(file.name);

                        $('<p/>').text(file.name).appendTo('#progress');
                    });
                },
                progressall: function (e, data) {
                    var progress = parseInt(data.loaded / data.total * 100, 10);
                    $('#progress .bar').css(
                        'width',
                        progress + '%'
                    );
                }
            });
        });

        $('#saveBtn').on('click', function (event) {
            saveCohortForm(true);
        });

        $(window).on('beforeunload', function (event) {
            if (getCookie('edit_mode') == "edit") {
                saveCohortForm(false);
            }
        });

        $('#submitBtn').click(function (event) {
            enable_edit_input(false);
            saveCohortForm(false);
            jt.validateCohortRecord($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    var result = resp['result'];
                    if (result.length > 0) {
                        var v_items = '';
                        var hv_items = '';
                        var item_counter = 0;
                        var label_text = '';
                        var fields = result.split(';');
                        for (i = 0; i < fields.length; i++) {
                            if (fields[i].length == 0) {
                                continue;
                            }
                            var field = fields[i].split('|');
                            var hardError = false;
                            
                            var check_msg = "";
                            if (field[2].toLowerCase() == "required") {
                                hardError = true;
                                check_msg = " <b>has required fields</b>";
                                item_counter++;
                            }
                            else if (field[2].toLowerCase() == "invalid_pattern") {
                                hardError = true;
                                check_msg = " <b>should be reviewed for accuracy</b>";
                                item_counter++;
                            }
                            else if (field[2].toLowerCase() == "missing") {
                                check_msg = " <b>has missing fields</b>";
                                item_counter++;
                            }

                            if (hardError && ((i + 1 < fields.length) && (field[1] != fields[i + 1].split('|')[1] || fields[i + 1].split('|')[2] != "required"))) {
                                label_text = " <span class=\"validation-item-text\">" + field[1] + " " + check_msg + " (" + item_counter + ")</span>";
                                hv_items += "<li>" + label_text + "</li>";

                                labe_text = '';
                                item_counter = 0;
                            }
                            else if ((i + 1 < fields.length) && field[1] != fields[i + 1].split('|')[1]) {
                                label_text = " <span class=\"validation-item-text\">" + field[1] + " " + check_msg + " (" + item_counter + ")</span>";
                                if (hardError) {
                                    hv_items += "<li>" + label_text + "</li>";
                                }
                                else {
                                    v_items += "<li>" + label_text + "</li>";
                                }

                                labe_text = '';
                                item_counter = 0;
                            }
                            else if (i + 1 >= fields.length) {
                                label_text = " <span class=\"validation-item-text\">" + field[1] + " " + check_msg + " (" + item_counter + ")</span>";
                                if (hardError) {
                                    hv_items += "<li>" + label_text + "</li>";
                                }
                                else {
                                    v_items += "<li>" + label_text + "</li>";
                                }
                            }
                        }
                        if (hv_items != '') {
                            $('#hard_validation_dialog #hard_validation_items').html('').append('<ul>' + hv_items + '</ul>');
                            $('#hard_validation_dialog').modal('show');
                        }
                        else {
                            $('#validation_dialog p.bg-danger').remove();
                            $('#acknowledgeSubmissionCkbx').parent().css('color', 'inherit').css('font-weight', 'inherit');
                            $('#validation_dialog #validation_items').html('').append('<ul>' + v_items + '</ul>');
                            $('#validation_dialog').modal('show');
                        }
                    }
                    else {
                        $('#submit_form_dialog').modal('show');
                    }
                }
            });
        });

        $('#validationContinueBtn').click(function (event) {
            if ($('#acknowledgeSubmissionCkbx').is(':not(:checked)')) {
                $('#acknowledgeSubmissionCkbx').parent().css('color', 'red').css('font-weight', 'bold');
                if ($('#validation_dialog .modal-body p').html().indexOf('Error: You must') == -1) {
                    $('#validation_dialog .modal-body').prepend('<p class="bg-danger">Error: You must check the acknowledgement checkbox to proceed.</p>');
                }
            }
            else {
                $('#validation_dialog').modal('hide');
                $('#submit_form_dialog').modal('show');
            }
        });

        $('#confirmSubmissionBtn').click(function (event) {
            jt.submitCohortRecord($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), function (resp) {
                $('#submit_form_dialog').modal('hide');
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    $('#submit_acknowledgement_dialog').modal('show');
                    location.reload();
                }
            });
        });

        $('#rejectBtn').click(function (event) {
            $('#reviewer_rejection_dialog').modal('show');
        });
        $('#reviewerRejectionBtn').click(function (event) {
            jt.rejectCohortRecord($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), $('#reviewerRejectionRationale').val(), function (resp) {
                $('#reviewer_rejection_dialog').modal('hide');
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    location.href = '/input/list.aspx?tab=pending';
                }
            });
        });
        $('#approveBtn').click(function (event) {
            $('#reviewer_approval_dialog').modal('show');
        });
        $('#reviewerApprovalBtn').click(function (event) {
            jt.publishCohortRecord($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), function (resp) {
                if (resp['error']) {
                    console.warn(resp['error']['message']);
                    alert(resp['error']['message']);
                }
                else {
                    console.log('publish successful');
                }
                $('#reviewer_approval_dialog').modal('hide');
                location.href = '/input/list.aspx?tab=pending';
            });
            
        });
        

        $(function () {

            $(':input[type="number"]').on('keypress', function (event) {
                if (!$.isNumeric(event.key) && /[a-z]/gi.test(event.key)) {
                    event.preventDefault();
                    if ($(this).attr('type') == "number" && !$(this).parent().hasClass('bg-danger')) {
                        $(this).parent().addClass('bg-danger text-danger');
                        if (($(this).siblings('label').length > 0) && $(this).siblings('label').text().indexOf("number expected") == -1) {
                            $(this).siblings("label").append(': <b><u>number expected</u></b>');
                        }
                        else {
                            $(this).parent().prepend('<span class="sr-only">number expected</span>');  
                        }

                    }

                    if ($('#caption_alert').length > 0) {
                        $('#caption_alert').addClass('bg-danger text-danger');
                        $('#caption_alert').parents('.panel-body').addClass('validation-error');
                    }
                }
                else {
                    $(this).parent().removeClass('bg-danger text-danger');
                    $(this).siblings('.sr-only').remove();
                    //$('#caption_alert').parents('.panel-body').removeClass('validation-error');
                }
            });

            if ($('#draftCohortId').val() > 0) {
                jt.getCohortRecordByWebId($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), function (resp) {
                    if (resp['error']) {
                        console.warn(resp['error']['message']);
                        alert(resp['error']['message']);
                        return;
                    }

                    cohort_data = resp['result'][0];
                    for (var property in cohort_data) {
                        if (!cohort_data.hasOwnProperty(property)) {
                            continue;
                        }
                        var jqProp = $('#' + property);
                        try {
                            if (jqProp.length == 0) {
                                var findToReplace = /[\W|\s]+/ig;
                                if ($('#' + property + '_' + cohort_data[property].toString().replace(findToReplace, '').toLowerCase()).length != 0) {
                                    jqProp = $('#' + property + '_' + cohort_data[property].toString().replace(findToReplace, '').toLowerCase());
                                }
                                else {
                                    //console.log('unable to find control ' + property);
                                    continue;
                                }
                            }
                        }
                        catch (error) {
                            //console.warn('error encountered::' + error.message);
                            continue;
                        }

                        if (cohort_data[property] === null)
                            continue;

                        if (jqProp.attr('type') == 'checkbox' && (cohort_data[property] == true || cohort_data[property].toString().toLowerCase() == 'yes')) {
                            jqProp.prop('checked', true);
                            jqProp.attr('value', cohort_data[property]);
                        }
                        else if (jqProp.attr('type') == 'radio') {
                            jqProp.prop('checked', 'checked');
                        }
                        else {
                            if (cohort_data[property] == "-1") {
                                continue;
                            }
                            jqProp.val(cohort_data[property]);
                        }
                    }
                    enable_edit_input(getCookie("edit_mode"));
                    evaluateSkips();
                    set_associate_text_field_state();

                    enrollment_automatic_totals();
                });
            }
        });
    </script>
</body>
</html>
