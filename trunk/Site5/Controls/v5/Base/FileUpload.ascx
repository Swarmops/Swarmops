<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FileUpload.ascx.cs" Inherits="Swarmops.Controls.Base.FileUpload" %>

<script type="text/javascript">
    // The below weird construct circumvents a mono-server optimization bug that kills the string

    var imageFileStart = "\x3Cimg src='";
    var imageFileSuccess = "/Images/Icons/iconshock-invoice-greentick-32px.png";
    var imageFileFail = "/Images/Icons/iconshock-invoice-redcross-32px.png";
    var imageFileEnd = "' /\x3E"; // needs to be separate var rather than inline because of mono-server optimization bug
    var imageUploadSuccess = imageFileStart + imageFileSuccess + imageFileEnd;
    var imageUploadFail = imageFileStart + imageFileFail + imageFileEnd;

    $(document).ready(function() {

        $('#ButtonUploadHidden').fileupload({
            url: '/Automation/UploadFileHandler.ashx?Guid=<%= Guid.ToString() %>',
            dataType: 'json',
            done: function(e, data) {
                $.each(data.result, function(index, file) {
                    if (file.error == null || file.error.length < 1) {
                        $('#DivUploadCount').append(imageUploadSuccess);
                    } else {
                        $('#DivUploadCount').append(imageUploadFail); // Assume ERR_NOT_IMAGE
                        alertify.error('File "' + file.name + '" could not be uploaded. Is it really an image?');
                    }

                });
                $('#DivProgressUpload').progressbar({ value: 100 });
                $('#DivProgressUpload').fadeOut('400', function() { $('#DivUploadCount').fadeIn(); });
            },
            progressall: function(e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $("#DivProgressUpload .progressbar-value").animate(
                    {
                        width: progress + "%"
                    }, { queue: false });
            },
            add: function(e, data) {
                $('#DivUploadCount').css('display', 'none');
                $('#DivProgressUpload').progressbar({ value: 0 }); // reset in case of previous uploads on this page
                $('#DivProgressUpload').fadeIn();
                data.submit();
            }
        });


        $('#ButtonUploadVisible').bind("click", function() {
            $('#ButtonUploadHidden').click();
        });


        $('#DivProgressUpload').progressbar({ value: 0, max: 100 });

    });

</script>

<div style="height:36px;float:left"><input id="ButtonUploadVisible" class="ButtonSwarmopsUpload" type="button" /><input id="ButtonUploadHidden" type="file" name="files[]" multiple style="display:none" /></div><div style="height:36px;width:270px;margin-right:10px;float:right;border:none"><div id="DivUploadCount" style="display:none;overflow:hidden;height:64px"></div><div id="DivProgressUpload" style="width:100%;margin-top:8px;display:none"></div></div>&nbsp;<br/>

