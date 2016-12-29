<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="FileUpload.ascx.cs" Inherits="Swarmops.Controls.Base.FileUpload" %>

<asp:HiddenField runat="server" ID="HiddenGuid"/>

<script type="text/javascript">
    // The below weird construct circumvents a mono-server optimization bug that kills the string

    var imageFileStart = "\x3Cimg src='";
    var imageFileSuccess = "/Images/Icons/iconshock-invoice-greentick-32px.png";
    var imageFileFail = "/Images/Icons/iconshock-invoice-redcross-32px.png";
    var imageFileEnd = "' /\x3E"; // needs to be separate var rather than inline because of mono-server optimization bug
    var imageUploadSuccess = imageFileStart + imageFileSuccess + imageFileEnd;
    var imageUploadFail = imageFileStart + imageFileFail + imageFileEnd;
    
    $(document).ready(function() {

        $('#<%=this.ClientID %>_ButtonUploadHidden').fileupload({
            url: '/Automation/UploadFileHandler.ashx?Guid=<%= GuidString %>&Filter=<%=Filter %>',
            dataType: 'json',
            done: function(e, data) {
                $.each(data.result, function(index, file) {
                    if (file.error == null || file.error.length < 1) {
                        $('#<%=this.ClientID %>_DivUploadCount').append(imageUploadSuccess);
                        
                        <% if (!String.IsNullOrEmpty(this.ClientUploadCompleteCallback))
                           { %>
                        
                        <%= this.ClientUploadCompleteCallback %>();
                        
                        <% } %>

                    } else {
                        $('#<%=this.ClientID %>_DivUploadCount').append(imageUploadFail); // Assume ERR_NOT_IMAGE
                        alertify.error('File "' + file.name + '" could not be uploaded. Is it really an image?');
                    }

                });
                $('#<%=this.ClientID %>_DivProgressUpload').progressbar({ value: 100 });
                $('#<%=this.ClientID %>_DivProgressUpload').fadeOut('400', function () { $('#<%=this.ClientID %>_DivUploadCount').fadeIn(); });
            },
            progressall: function(e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $("#<%=this.ClientID %>_DivProgressUpload .progressbar-value").animate(
                    {
                        width: progress + "%"
                    }, { queue: false });
            },
            add: function(e, data) {
                $('#<%=this.ClientID %>_DivUploadCount').css('display', 'none');
                $('#<%=this.ClientID %>_DivProgressUpload').progressbar({ value: 0 }); // reset in case of previous uploads on this page
                $('#<%=this.ClientID %>_DivProgressUpload').fadeIn();
                data.submit();
            }
        });


        $('#<%=this.ClientID %>_ButtonUploadVisible').bind("click", function () {
            $('#<%=this.ClientID %>_ButtonUploadHidden').click();
        });


        $('#<%=this.ClientID %>_DivProgressUpload').progressbar({ value: 0, max: 100 });

    });

</script>

<div class="stacked-input-control"><div class="buttonUploadFile"><input id="<%=this.ClientID %>_ButtonUploadVisible" class="ButtonSwarmopsUpload NoInputFocus" type="button" <%= this.HideTrigger? "style='display:none'" : string.Empty %> /><input id="<%=this.ClientID %>_ButtonUploadHidden" type="file" name="files[]" multiple style="display:none" /></div><div style="height:36px;padding-top:4px;width:270px;margin-right:10px;float:right;border:none"><div id="<%=this.ClientID %>_DivUploadCount" style='display:none;overflow:hidden;height:'<%=this.DisplayCount < 9? 32: 64%>px'></div><div id="<%=this.ClientID %>_DivProgressUpload" style="width:100%;margin-top:8px;display:none"></div></div></div>

