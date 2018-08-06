<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="inputToolBar.ascx.cs" Inherits="cec_publicweb.input.usrctrls.inputToolBar" %>

<div id="toolbar" class="floatingToolbar" runat="server">
   <div> <!--id="mainNavBar-inner" -->
    <ul class="list-unstyled"> <!-- id="mainNav" -->
      <li runat="server" id="editListItem"><button runat="server" type="button" class="btn btn-primary" id="enableEditBtn" onclick=""><span class="glyphicon glyphicon-pencil"></span><br><span class="button-text">Edit</span></button></li>
      <li runat="server" id="saveListItem"><button runat="server" type="button" class="btn btn-primary" id="saveBtn" disabled="disabled"><span class="glyphicon glyphicon-floppy-disk"></span><br><span class="button-text">Save</span></button></li>
      <li runat="server" id="submitListItem"><button runat="server" type="button" class="btn btn-primary" id="submitBtn"><span class="glyphicon glyphicon-share"></span><br><span class="button-text">Submit</span></button></li>

      <li runat="server" id="rejectListItem"><button runat="server" type="button" class="btn btn-primary" id="rejectBtn"><span class="glyphicon glyphicon-remove"></span><br><span class="button-text">Reject</span></button></li>
      <li runat="server" id="approveListItem"><button runat="server" type="button" class="btn btn-primary" id="approveBtn"><span class=" glyphicon glyphicon-ok"></span><br><span class="button-text">Approve</span></button></li>
      <li runat="server" id="returnListItem"><a runat="server" class="btn btn-primary" href="/input/list.aspx?tab=pending" id="returnBtn"><span class="glyphicon glyphicon-log-out"></span><br><span class="button-text">Exit</span></a></li>
    </ul>
  </div>
</div>

<!-- save modal -->
<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="save_dialog">
  <div class="modal-dialog info-modal bg-success text-success" role="document">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title"><span class="glyphicon glyphicon-ok"></span> Save Successful!</h2>
        </div>
        <div class="modal-body">
            <div class="modal-text">
                
            </div>            
            <div class="pull-right">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>
        <div class="modal-footer">

        </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal end save modal -->

<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="hard_validation_dialog">
  <div class="modal-dialog info-modal bg-danger text-danger" role="document">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title"><span class="glyphicon glyphicon-alert"></span> Required Change(s)</h2>
        </div>
        <div class="modal-body">
            <div id="hard_validation_items"></div>
          <div class="pull-right">
            <button type="button" class="btn btn-default" data-dismiss="modal">Return</button>
          </div>
        </div>
      <div class="modal-footer">
      </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->


<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="validation_dialog">
  <div class="modal-dialog info-modal bg-warning text-warning" role="document">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title"><span class="glyphicon glyphicon-alert"></span> Before you continue check these fields:</h2>
        </div>
        <div class="modal-body">
            <div id="validation_items"></div>
            <div id="softwarning_disclaimer">
                <p><input type="checkbox" id="acknowledgeSubmissionCkbx" /> Check to acknowledge the issues above in order to proceed with submission</p>
          
                <p>If any non-required question is missing a response, you may choose to leave it blank. It will appear on the website as "N/P" for not provided.</p>
            </div>
          <div class="pull-right">
            <button type="button" class="btn btn-primary" id="validationContinueBtn">Continue</button>
            <button type="button" class="btn btn-default" data-dismiss="modal">Return</button>
          </div>
        </div>
      <div class="modal-footer">
      </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->

<!-- submit_form_modal form -->
<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="submit_form_dialog">
  <div class="modal-dialog info-modal bg-warning text-warning" role="document">
    <div class="modal-content">
        <div class="modal-header">
           <h2 class="modal-title"><span class="glyphicon glyphicon-open-file"></span> Cohort Form Submission</h2>
        </div>
        <div class="modal-body">
          <div class="modal-text">
              <p>Do you want to submit your updates to NCI?</p><p>NCI will review your form and followup with you if there are any questions</p>
          </div>
          <div class="pull-right">
            <button type="button" class="btn btn-primary" id="confirmSubmissionBtn">Submit</button>
            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
          </div>
        </div>
      <div class="modal-footer">
      </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->

<!-- submit_acknowledgement_modal submitted -->
<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="submit_acknowledgement_dialog">
  <div class="modal-dialog info-modal bg-success text-success" role="document">
    <div class="modal-content">
        <div class="modal-header">
           <h2 class="modal-title"><span class="glyphicon glyphicon-open-file"></span> Submission Successful</h2>
        </div>
        <div class="modal-body">
          <div class="modal-text">
              <p>Success! Your updates have been submitted. NCI will review your form and followup with you if there are any questions</p>
          </div>
          <div class="pull-right">
            <button type="button" class="btn btn-default" data-dismiss="modal">Finish</button>
          </div>
        </div>
      <div class="modal-footer">
      </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->


<!-- reviewer_rejection_dialog rejection -->
<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="reviewer_rejection_dialog">
  <div class="modal-dialog modal-lg info-modal bg-warning text-warning" role="document">
    <div class="modal-content">
        <div class="modal-header">
           <h2 class="modal-title"><span class="glyphicon glyphicon-remove-sign"></span> Reject Cohort Form</h2>
        </div>
        <div class="modal-body">
          <div class="modal-text">
              <p>Please confirm you are returning this form to the Cohort Representative. Select 'Reject' to return the data to Cohort Representative. Select 'Cancel' to resume your review.</p>
              <p>Enter a rationale for the rejection.</p>
              <textarea id="reviewerRejectionRationale" class="form-control" rows="10"></textarea>
          </div>
          <div class="pull-right">
            <button type="button" class="btn btn-primary" id="reviewerRejectionBtn">Reject</button>
            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
          </div>
        </div>
      <div class="modal-footer">
      </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->

<!-- reviewer_approval_dialog -->
<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="reviewer_approval_dialog">
  <div class="modal-dialog modal-lg bg-info text-info" role="document">
    <div class="modal-content">
        <div class="modal-header">
           <h2 class="modal-title"><span class="glyphicon glyphicon-exclamation-sign"></span> Cohort Form Approval</h2>
        </div>
        <div class="modal-body">
          <div class="modal-text">
              <p>Please confirm you are approving this form. Select 'Yes' to publish the data to the live CEDCD website. Select 'No' to cancel and resume your review.</p>
          </div>
          <div class="pull-right">
            <button type="button" class="btn btn-primary" id="reviewerApprovalBtn">Yes</button>
            <button type="button" class="btn btn-default" data-dismiss="modal">No</button>
          </div>
        </div>
      <div class="modal-footer">
      </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->