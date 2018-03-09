<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="contact.ascx.cs" Inherits="cec_publicweb.usrctrls.contact" %>

<div id="contactFlag" class="rightSideFlag" data-toggle="modal" data-target="#contactOverlay"><a href="#"><span class="glyphicon glyphicon-envelope"></span> <span>Contact Us</span></a> </div>

<div id="contactOverlay" class="modal fade modal-straigt-corners" tabindex="-1" role="dialog" aria-labelledby="Contact Form">
<div class="modal-dialog" role="document">  
    <div class="modal-content">  
        <div id="contactForm" class="row">
          <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            <div id="contact-main" class="col">
		
                <div id="contact-header" class="col-md-12">
			        <h1 class="pg-title">Contact</h1>	            
		        </div><!--prof-header-->            
		
                <div id="contact-col-1" class="col-md-6 contact-col">        
                              
                    <p runat="server" id="rg_errorMsg" class="bg-danger"></p>
                        
                    <div id="div_firstname" runat="server" class="contact-us-field">
                    <label class="oneLineLabel" for="cu_firstName">First Name <span class="required">*</span></label>
                    <asp:TextBox ID="cu_firstName" runat="server" />
                    </div>

                    <div id="div_lastname" runat="server" class="contact-us-field">
                    <label class="oneLineLabel" for="cu_lastName">Last Name <span class="required">*</span></label>
                    <asp:TextBox ID="cu_lastName" runat="server" />
                    </div>

                    <div id="div_organization" runat="server" class="contact-us-field">
                    <label class="oneLineLabel" for="cu_organization">Organization <span class="required">*</span></label>
                    <asp:TextBox ID="cu_organization" runat="server" /> 
                    </div>

                    <div id="div_phone" runat="server" class="contact-us-field">
                    <label class="oneLineLabel" for="cu_phone">Phone Number</label>
                    <asp:TextBox ID="cu_phone" runat="server" placeholder="(   )   -" />
                    </div>

                    <div id="div_email" runat="server" class="contact-us-field">
                    <label class="oneLineLabel" for="cu_email">Email <span class="required">*</span></label>
                    <asp:TextBox ID="cu_email" runat="server" />
                    </div>

                    <div id="div_topic" class="contact-us-field">
                    <label class="oneLineLabel" for="cu_topic">Topic <span class="required">*</span></label>
                    <asp:DropDownList ID="cu_topic" runat="server" CssClass="textEntrySmall">
                        <asp:ListItem Text="General CEDCD Inquiry" Value="1" />
                        <asp:ListItem Text="Report a Bug" Value="2" />
                        <asp:ListItem Text="Question for NCI CEDCD Researcher Team" Value="3" />
                        <asp:ListItem Text="Other Issues" Value="4" />
                    </asp:DropDownList>
                    </div>

                    <div id="div_message" runat="server" class="contact-us-field">
                    <label class="oneLineLabel" for="cu_message">Message <span class="required">*</span></label>
                    <asp:TextBox ID="cu_message" runat="server" TextMode="MultiLine" Rows="4" />
                    </div>

                    <div class="bttn-group">
                        <asp:LinkButton ID="rg_registerBtn" runat="server" OnClick="submitBtnClicked" CssClass="bttn_submit" Text="Submit" />
                        <asp:LinkButton ID="fg_cancelBtn" runat="server" OnClick="cancelBtnClicked" CssClass="bttn_cancel" Text="Cancel" />
                    </div>
                </div> <!-- contact-col-1 -->

                <div id="contact-col-2" class="col-md-6 contact-col">

			        <h2>General Inquiries</h2>
            	        <p>For questions about the website or the CEDCD project, fill out the contact form. To help us respond to your query, please provide as much detail as possible.</p>
         	        <h2>Website Technical Assistance</h2>
            	        <p>For technical issues or bugs, include the following information:</p>
            	        <ul>
            		        <li>Platform (i.e., PC, Mac)</li>
             		        <li>Operating system (i.e., Windows XP, Windows 7, Mac OS X)</li> 
              		        <li>Browser name and version (i.e., Internet Explorer 6, Chrome 18)</li>
           	        </ul>
         	        <h2>Contacting a Specific Cohort</h2> 
            	        <p>You can find contact info for each cohort on their <span style="font-weight:bold;">Cohort Profile</span> page.</p>
                </div><!--contact-col-2-->

            </div> <!-- contact-main -->
        </div><!--cedcd-main-content-->
    </div>
</div>
</div>