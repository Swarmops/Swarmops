<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Falconwing.Frontend.Dashboard" %>
<%@ Import Namespace="Falconwing.Logic" %>
<%@ Import Namespace="Falconwing.Logic.ExtensionMethods" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyTextBox" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
   
    <script src="/Plugins/FalconwingMedia/Public/jquery.editinplace.js" language="javascript" type="text/javascript" ></script>

    <%-- ReSharper disable once InvalidValue --%>
    <script type="text/javascript">
        
        function preload(arrayOfImages) {
            $(arrayOfImages).each(function () {
                (new Image()).src = this;
            });
        }

        preload([
            '/Images/Abstract/ajaxloader-medium.gif',
            '/Plugins/FalconwingMedia/Icons/iconshock-balloon-wrench-128x96px-hot.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-balloon-defer-128x96px-hot.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-wrench-128x96px.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-padlock-128x96px.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-warning-128x96px.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-red-cross-128x96px-disabled.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-green-tick-128x96px-disabled.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-green-plus-circled-128x96px.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-wrench-128x96px-centered.png',
            '/Plugins/FalconwingMedia/Icons/iconshock-balloon-128x96px-hot.png',
            '/Images/Icons/iconshock-balloon-no-128x96px-hot.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-hot.png',
            '/Images/Icons/iconshock-green-tick-128x96px.png',
            '/Images/Icons/iconshock-red-cross-128x96px.png',
            '/Images/Icons/iconshock-red-cross-circled-128x96px.png'
        ]);


        $(document).ready(function () {
            updateEditQueueCount();
            initialRender();
            bindImageEvents();

            initializeSocket();

            $('a#linkSelectPhoto').click(function(event) {
                $('input#textEditPhotoSourceUrl').val('');
                $('input#textEditPhotoCredit').val('');
                $('input#textEditPhotoDescription').val('');
                $('input#textEditPhotoPersonName').val('');
                <%=DialogPhoto.ClientID%>_open();
                event.stopImmediatePropagation();
            });

            loadAllStoryEdits();

            $('input#buttonCreatePhoto').click(function() {
                if ($('input#textEditPhotoSourceUrl').val().length < 2) {
                    alertify.error("Please specify a source URL.");
                    return;
                }
                if ($('input#textEditPhotoDescription').val().length < 2) {
                    alertify.error("Please describe the image, so colleagues can find and reuse this image later.");
                    return;
                }
                if ($('input#textEditPhotoCredit').val().length < 2) {
                    alertify.error("Please credit the photographer. If no real name is available, handles will do (like Twittername on Twitter).");
                    return;
                }

                SwarmopsJS.ajaxCall(
                    baseAjaxUrl + "CreatePhoto",
                    {
                        sourceUrl: $('input#textEditPhotoSourceUrl').val(),
                        description: encodeURIComponent($('input#textEditPhotoDescription').val()),
                        personName: encodeURIComponent($('input#textEditPhotoPersonName').val()),
                        photoCredit: encodeURIComponent($('input#textEditPhotoCredit').val())
                    },
                    function(result) {
                        if (result.Success) {
                            currentPhotoId = parseInt(result.DisplayMessage);
                            $('span#spanSelectedPhotoDescription').text($('input#textEditPhotoDescription').val());
                            <%=DialogPhoto.ClientID%>_close();
                        } else {
                            alertify.error(result.DisplayMessage);
                        }
                    });
            });

            $('#buttonEditCommit').click(function () {

                if ($('#<%=DropCategories.ClientID%>').val() == 0) {
                    alertify.error("A topic is required.");
                    return;
                }

                var headlineLength = $('#textEditHeadline').val().length;

                if (headlineLength >= 84) {
                    alertify.error("Headline is too long: 83 characters max, you have " + headlineLength + ".");
                    return;
                }

                alertify.log("Committing...");

                SwarmopsJS.ajaxCall(
                    baseAjaxUrl + "CommitStoryEdit",
                    {
                        storyId: currentStoryId,
                        revisionId: 0, // don't care $('img.imageStoryThumbnail[storyId="' + currentStoryId + '"]').attr('revisionId'),
                        countryCode: $('#<%=DropCountries.ClientID%>').val(),
                        topicId: $('#<%=DropCategories.ClientID%>').val(),
                        headline: encodeURIComponent($('#textEditHeadline').val()),
                        body: encodeURIComponent($('#textEditBody').val()),
                        sources: $('#textEditSources').val(),
                        comment: encodeURIComponent($('#textEditReason').val()),
                        photoId: currentPhotoId
                    },
                    function (result) {
                        if (result.Success) {
                            alertify.set({
                                labels: {
                                    ok: 'Alright, sounds good',
                                    cancel: "Uh, let's not do that"
                                }
                            });

                            if (currentStoryId == 0) {
                                alertify.log("Story created.");
                                alertify.alert("Your story has been created.");
                                refillEditPool();
                                refillAuthor();
                            } else {
                                var jsonData = { messageType: "StoryLockDisengage", storyId: currentStoryId };
                                socket.send($.toJSON(jsonData));

                                alertify.log("Story edited.");
                            }
                            <%=this.DialogEdit.ClientID %>_close();
                        } else {
                            alertify.alert("There was an error [ButtonEditCommit]:<br/><br/>" + result.DisplayMessage);
                        }
                    });
                });

            $('span.spanAddComment').click(enterComment);


        }); // end of document.ready


        function loadAllStoryEdits() {
            $('div#divTabEdit div.divStoryWrapper').each(function(index, element) {
                var storyId = $(element).attr("storyId");
                getStoryEdits(storyId);
            });

            $('div#divTabPublished div.divStoryWrapper, div#divTabRejected div.divStoryWrapper, div#divTabStars div.divStoryWrapper, div#divTabAuthor div.divStoryWrapper').each(function(index, element) {
                var storyId = $(element).attr("storyId");
                getStoryHistory(storyId);
            });
        }

        function watchHeartbeat() {
            if (heartbeatTimer != null) {
                clearTimeout(heartbeatTimer);
                heartbeatTimer = null;
            }

            if (pingTimer != null)
            {
                clearTimeout(pingTimer);
                pingTimer = null;
            };

            heartbeatTimer = setTimeout(function() {
                // if this is reached, we've missed TWO heartbeats from the server, and consider ourselves disconnected. Retry.

                if (!heartbeatsLost) {
                    $('#tdSocketConnected').text("No");
                    alertify.log("Connection to the backend server has been lost. Attempting reconnect.");
                    heartbeatsLost = true;
                }

                initializeSocket();
                watchHeartbeat(); // Restart timer to try again if the init above fails

            }, 25000);

            pingTimer = setTimeout(function() {
                // If this is reached, we need to ping the server for a new heartbeat

                pingTimer = null; // timeout reached, ping for heartbeat
                socket.send('{"serverRequest":"Ping"}');

            }, 15000);
        }


        function reloadStory(storyId) {
            SwarmopsJS.ajaxCall(
                baseAjaxUrl + "GetStory",
                { storyId: storyId },
                function(result) {
                    if (result.Success) {
                        var storyShown = $('div.divStoryWrapper[storyId="' + result.StoryId + '"]').length;

                        if (storyShown) {
                            $('p.pStoryHeadline[storyId="' + result.StoryId + '"]').text(decodeURIComponent(result.Headline));
                            $('p.pStoryBody[storyId="' + result.StoryId + '"]').html(decodeURIComponent(result.Body));
                            $('span.spanTopicGeography[storyId="' + result.StoryId + '"]').text(result.TopicGeography);
                            renderThumbnail($('img.imageStoryThumbnail[storyId="' + result.StoryId + '"]'));
                        }

                    } else {
                        alertify.alert("An exception was thrown [reloadStory]:<br/><br/>" + result.DisplayMessage);
                    }
                });
        }


        function initializeSocket() {
            if (socket != null) {
                socket.close();
                socket = null;
            }

            socket = new WebSocket("wss://opswss.falconwing.org/Editing?Auth=" + encodeURIComponent('<%=CurrentAuthority.ToEncryptedXml()%>'));
            socket.onopen = function(data) {
                $('#tdSocketConnected').text("Yes");

                if (heartbeatsLost) {
                    // Reload all edits and the edit pool. If there are discrepancies, we can probably live with them. (Detect editing?)
                    alertify.log("Backend connection restored without interruption.");

                    loadAllStoryEdits(); // reloads events that happened while disconnected
                    refillEditPool(); // reloads edit queue (other pools will trigger from the loaded events)

                    heartbeatsLost = false;
                }

                watchHeartbeat();
            };
            socket.onclose = function(data) { $('#tdSocketConnected').text = ("Lost"); };
            socket.onerror = function(data) { $('#tdSocketConnected').text = ("Error"); };
            socket.onmessage = function(data) {

                console.log(data.data);

                var message = $.parseJSON(data.data);

                if (message.messageType == "Heartbeat") {
                    watchHeartbeat();
                }

                if (message.messageType == "EditorCount") {
                    $('#tdSocketEditorCount').text(message.editorCount);
                }

                if (message.messageType == "Timestamp") {
                    lastKnownTimestamp = message.Timestamp;
                    updateEditTimestamps();
                }

                if (message.messageType == "AddStoryEdit") {
                    if (addEdit(message.StoryId, message) && message.EditType == "Edit") {
                        reloadStory(message.StoryId);
                    }
                }

                if (message.messageType == "StoryLockEngage") {
                    $('img.imageStoryAction[storyId="' + message.storyId + '"]').attr('src', '/Plugins/FalconwingMedia/Icons/iconshock-padlock-128x96px.png').show();
                    $('img.imageStoryEdit[storyId="' + message.storyId + '"]').fadeOut();
                    $('img.imageStoryYes[storyId="' + message.storyId + '"]').fadeOut();
                    $('img.imageStoryNo[storyId="' + message.storyId + '"]').fadeOut();
                    $('img.imageStoryThumbnail[storyId="' + message.storyId + '"]').css('-webkit-filter', 'blur(2px) brightness(1.2) grayscale(0.8)');  // Chrome
                    $('img.imageStoryThumbnail[storyId="' + message.storyId + '"]').css('filter', 'blur(2px) brightness(1.2) grayscale(0.8)');          // Firefox
                }

                if (message.messageType == "StoryLockDisengage") {
                    $('img.imageStoryAction[storyId="' + message.storyId + '"]').hide();
                    $('img.imageStoryEdit[storyId="' + message.storyId + '"]').fadeIn();
                    $('img.imageStoryYes[storyId="' + message.storyId + '"]').fadeIn();
                    $('img.imageStoryNo[storyId="' + message.storyId + '"]').fadeIn();
                    $('img.imageStoryThumbnail[storyId="' + message.storyId + '"]').css('-webkit-filter', '');
                    $('img.imageStoryThumbnail[storyId="' + message.storyId + '"]').css('filter', '');
                }

                if (message.messageType == "QueueInfo") {
                    $('#tdSocketEditQueueCount').text(message.EditCount);
                    $('#tdSocketPubQueueCount').text(message.PubCount);
                    $('#tdSocketPubQueueNext').text(message.PubNext);
                    $('#tdSocketPubQueueExtent').text(message.PubExtent);
                }

                if (message.messageType == "StoryEdited") {

                    var storyShown = $('div.divStoryWrapper[storyId="' + message.StoryId + '"]').length;

                    if (storyShown) {
                        $('p.pStoryHeadline[storyId="' + message.StoryId + '"]').text(decodeURIComponent(message.Headline));
                        $('p.pStoryBody[storyId="' + message.StoryId + '"]').html(decodeURIComponent(message.Body) + " " + message.Sources);
                        $('span.spanTopicGeography[storyId="' + message.StoryId + '"]').text(message.TopicGeography);
                        renderThumbnail($('img.imageStoryThumbnail[storyId="' + message.StoryId + '"]'));  // disable this later let this happen when the edit is added

                        // Invalidate previous green- and redlights
                        $('div.divStoryWrapper[storyId="' + message.StoryId + '"] tr.trEdit[editType="Greenlight"] td.tdEditImage img').each(function(index, element) {
                            $(element).attr ("src", '/Plugins/FalconwingMedia/Icons/iconshock-green-tick-128x96px-disabled.png');
                            $(element).parents('tr.trEdit').attr("editType", "GreenlightInvalidated");
                            $(element).parent().next().next().append(" (invalidated by later edit)"); // adds to text field
                        });

                        $('div.divStoryWrapper[storyId="' + message.StoryId + '"] tr.trEdit[editType="Redlight"] td.tdEditImage img').each(function(index, element) {
                            $(element).attr ("src", '/Plugins/FalconwingMedia/Icons/iconshock-red-cross-128x96px-disabled.png');
                            $(element).parents('tr.trEdit').attr("editType", "RedlightInvalidated");
                            $(element).parent().next().next().append(" (invalidated by later edit)"); // adds to text field
                        });

                    } else {
                        refillEditPool(); // if it's not visible, maybe bring it back into view?
                    }
                }

                if (message.messageType == "StoryPublished") {
                    removeStory(message.StoryId);
                    refillPublished();
                }

                if (message.messageType == "StoryCreated") {
                    refillEditPool();
                    updateEditQueueCount();
                }
            };
        }


        function enterComment() {
            $(this).hide();
            $(this).after($('<input style="width:95%" type="text" placeholder="Type your comment here..." id="inputCommentEntry" />'));
            $('#inputCommentEntry').focus();

            $('#inputCommentEntry').blur($.proxy(function() {
                var commentValue = $('#inputCommentEntry').val();

                if (commentValue != "") {
                
                    var storyId = $(this).parents(".tableEdits").attr("storyId");

                    SwarmopsJS.ajaxCall(
                        baseAjaxUrl + "LightStory",
                        { storyId: storyId, revisionId: 0, lightType: "comment", comment: encodeURIComponent(commentValue) },
                        function(result) {
                            if (!result.Success) {
                                alertify.alert("Exception thrown:<br/><br/>" + result.DisplayMessage);
                            }
                        });
                }

                $('#inputCommentEntry').remove();
                $(this).show();
            }, this));

            $('#inputCommentEntry').keydown(function(key) {
                if (key.keyCode == 27) // escape
                {
                    $('#inputCommentEntry').val(''); // empty the comment box, canceling comment
                }
                if (key.keyCode == 13 || key.keyCode == 27) { // enter (or escape)
                    $('#inputCommentEntry').blur();
                }
            });

        }


        function unbindImageEvents() {
            $('img.imageStoryEdit').unbind();
            $('img.imageStoryDefer').unbind();
            $('img.imageStoryYes').unbind();
            $('img.imageStoryNo').unbind();
            $('img.imageStoryReturnEditing').unbind();
        }


        function bindImageEvents() {
            $('img.imageStoryEdit').mouseover(function () {
                $(this).attr("src", "/Plugins/FalconwingMedia/Icons/iconshock-balloon-wrench-128x96px-hot.png");
            });

            $('img.imageStoryEdit').mouseout(function () {
                $(this).attr("src", "/Plugins/FalconwingMedia/Icons/iconshock-balloon-wrench-128x96px.png");
            });

            $('img.imageStoryDefer').mouseover(function () {
                $(this).attr("src", "/Plugins/FalconwingMedia/Icons/iconshock-balloon-defer-128x96px-hot.png");
            });

            $('img.imageStoryDefer').mouseout(function () {
                $(this).attr("src", "/Plugins/FalconwingMedia/Icons/iconshock-balloon-defer-128x96px.png");
            });

            $('img.imageStoryNo').mouseover(function () {
                $(this).attr("src", "/Images/Icons/iconshock-balloon-no-128x96px-hot.png");
            });

            $('img.imageStoryNo').mouseout(function () {
                $(this).attr("src", "/Images/Icons/iconshock-balloon-no-128x96px.png");
            });

            $('img.imageStoryYes').mouseover(function () {
                $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px-hot.png");
            });

            $('img.imageStoryYes').mouseout(function () {
                $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
            });

            $('img.imageStoryReturnEditing').mouseover(function () {
                $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px-hot.png");
            });

            $('img.imageStoryReturnEditing').mouseout(function () {
                $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
            });

            $('img.imageStoryEdit').click(onClickEdit);
            $('img.imageStoryDefer').click(onClickDefer);
            $('img.imageStoryYes').click(onClickGreenlight);
            $('img.imageStoryNo').click(onClickRedlight);

            $('img.imageStoryReturnEditing').click(function() { alertify.log("Work in progress..."); });
        }

        

        function onClickDefer() {
            currentStoryId = $(this).attr("storyId");

            SwarmopsJS.ajaxCall(
                baseAjaxUrl + "LightStory",
                { storyId: currentStoryId, revisionId: 0, lightType: "defer", comment: "" },
                function (ajaxResult) {
                    if (ajaxResult) {
                        setTimeout($.proxy(function () {
                            $(this).animate({ height: 0, opacity: 0 }, 'slow');
                        }, $('div.divStoryWrapper[storyId="' + currentStoryId + '"]')), 250);
                        $('div.divStoryWrapper[storyId="' + currentStoryId + '"]+div.divSeparator').slideUp();
                        alertify.log("Story deferred to other editors.");
                        editQueueCount--;
                        updateEditQueueCount();
                        removeStoryRefillQueue(currentStoryId);
                    } else {
                        alertify.error("There was a concurrency error (somebody else was also writing to this story). Try again?");
                        // REFRESH STORY
                    }
                });

        }

        function onClickRedlight() {
            currentStoryId = $(this).attr("storyId");
            $('div.divStoryWrapper[storyId="' + currentStoryId + '"]').css('background-color', '#FCC');

            alertify.set({
                labels: {
                    ok: 'Proceed with redlighting',
                    cancel: "Uh, let's not do that"
                }
            });
            alertify.prompt("You are redlighting this story, preventing its publication and possibly rejecting it altogether. Why?<br/><br/>",
                function (result, foobar) {
                    $('div.divStoryWrapper[storyId="' + currentStoryId + '"]').css('background-color', 'inherit');
                    if (result) {
                        SwarmopsJS.ajaxCall(
                            baseAjaxUrl + "LightStory",
                            { storyId: currentStoryId, revisionId: 0, lightType: "red", comment: encodeURIComponent(foobar) },
                            function (ajaxResult) {
                                if (ajaxResult.Success) {
                                    $('img.imageStoryAction[storyId="' + currentStoryId + '"]').attr('src', '/Images/Icons/iconshock-red-cross-128x96px.png').show();
                                    editQueueCount--;
                                    updateEditQueueCount();
                                    removeStory(currentStoryId);
                                    refillEditPool();
                                    refillRejected();
                                } else {
                                    alertify.alert("An exception was thrown [OnClickRedlight]:<br/><br/>" + ajaxResult.DisplayMessage);
                                    // REFRESH STORY
                                }
                            });
                    } else {
                        // do nothing                            
                    }
                });

        }



        function onClickGreenlight() {
            currentStoryId = $(this).attr("storyId");
            $('div.divStoryWrapper[storyId="' + currentStoryId + '"]').css('background-color', '#CFC');

            alertify.set({
                labels: {
                    ok: 'Yes, confirmed',
                    cancel: 'Uh, wait a minute'
                }
            });
            alertify.confirm("You are greenlighting this story for publication. Please confirm you have checked this story for FACTS against its sources, the heartbeat FORMAT, and FIT against our values: every story needs to be informative, relevant, and funny.<br/><br/>(This confirmation doesn't show after a while.)",
                function (response) {
                    $('div.divStoryWrapper[storyId="' + currentStoryId + '"]').css('background-color', 'inherit');
                    if (response) {
                        var revisionId = $('img.imageStoryThumbnail[storyId="' + currentStoryId + '"]').attr('revisionId');
                        SwarmopsJS.ajaxCall(
                            baseAjaxUrl + "LightStory",
                            { storyId: currentStoryId, revisionId: revisionId, lightType: "green", comment: "" },
                            function (ajaxResult) {
                                if (ajaxResult.Success) {
                                    $('img.imageStoryAction[storyId="' + currentStoryId + '"]').attr('src', '/Images/Icons/iconshock-green-tick-128x96px.png').show();
                                    editQueueCount--;
                                    updateEditQueueCount();
                                    removeStory(currentStoryId);
                                    refillEditPool();
                                    refillPublishQueue(); // if moved
                                } else {
                                    if (ajaxResult.DisplayMessage == "Reload") {
                                        alert("The story was edited since it was sent to your browser. Reloading page.");
                                        document.reload();
                                        // TODO:
                                        //reloadStory(currentStoryId);
                                        //alertify.log("The story you tried to greenlight has been edited. Loading the latest version before your greenlight.");
                                    }
                                    else if (ajaxResult.DisplayMessage == "Concurrency") {
                                        // reloadStory(currentStoryId);
                                        alertify.error("There was a concurrency error (somebody else was also writing to this story). Try again?");
                                    } else {
                                        alertify.alert("An exception was thrown [OnClickGreenlight]:<br/><br/>" + ajaxResult.DisplayMessage);
                                    }
                                }
                            });
                    } else {
                        // cancelled - do nothing
                    }
                });
        }

        function onClickEdit() {
            currentStoryId = $(this).attr("storyId");
            $('.spanEditField').show();
            $('#spanEditStoryId').text(currentStoryId);
            $('#textEditBody').val('');
            $('#textEditHeadline').val('');
            $('#textEditSources').val('');
            $('#textEditReason').val('');

            SwarmopsJS.ajaxCall
            (baseAjaxUrl + "LockStoryForEditing",
                {
                    storyId: currentStoryId,
                    revisionId: $('img.imageStoryThumbnail[storyId="' + currentStoryId + '"]').attr('revisionId')
                },
                function (result) {
                    if (result.Success) {
                        
                        var jsonData = { messageType: "StoryLockEngage", storyId: currentStoryId };
                        socket.send($.toJSON(jsonData));

                        <%=this.DialogEdit.ClientID %>_open();
                        $('#textEditBody').val(decodeURIComponent(result.StoryBody));
                        currentPhotoId = result.StoryPhotoId;
                        $('span#spanSelectedPhotoDescription').text(decodeURIComponent(result.StoryPhotoDescription));
                        $('#textEditHeadline').val(decodeURIComponent(result.StoryHeadline));
                        $('#<%=DropCategories.ClientID%>').val(result.StoryTopicId);
                        $('#<%=DropCountries.ClientID%>').val(result.StoryCountryCode);
                        $('#textEditSources').val(result.StorySources);
                        $('#textEditReason').focus();
                        renderEditPreview(); // initalizes preview rotation
                    } else {
                        alertify.log("This story is being edited by someone else at the moment, and therefore locked.");
                    }
                });
        }


        function createStory() {
            currentStoryId = 0;
            currentPhotoId = 0;
            $('.spanEditField').hide();
            $('#spanEditStoryId').text("[NEW]");
            $('#textEditBody').val('');
            $('span#spanSelectedPhotoDescription').html("&mdash;");
            $('#textEditHeadline').val('');
            $('#textEditSources').val('');
            $('#textEditReason').val('');
            $('#<%=DropCategories.ClientID%>').val('8');
            $('#<%=DropCountries.ClientID%>').val('--');
            if ($("#<%=DropCountries.ClientID%> option[value='" + userDefaultCountry + "']").val() !== undefined) {
                $('#<%=DropCountries.ClientID%>').val(userDefaultCountry);
            }
            <%=this.DialogEdit.ClientID %>_open();
            renderEditPreview();
        }

        function renderEditPreview() {
            SwarmopsJS.ajaxCall
            (baseAjaxUrl + "RenderPreview",
                {
                    geographyCode: $('#<%=this.DropCountries.ClientID%>').val(),
                    topicId: $('#<%=this.DropCategories.ClientID%>').val(),
                    headline: $('#textEditHeadline').val(),
                    body: $('#textEditBody').val(),
                    photoId: currentPhotoId
                },
                function (result) {
                    if (result.Success) {
                        if (oddImage) {
                            $('#imagePreview1').attr('src', 'data:image/png;base64,' + result.DisplayMessage);
                            $('#imagePreview2').fadeOut();
                        } else {
                            $('#imagePreview2').attr('src', 'data:image/png;base64,' + result.DisplayMessage);
                            $('#imagePreview2').fadeIn();
                        }

                        oddImage = !oddImage;

                        nextRenderTimer = setTimeout(function () { renderEditPreview(); }, 2500);  // re-render in 2.5 seconds
                    } else {
                        alertify.alert('An exception was thrown:<br/><br/>' + result.DisplayMessage);
                    }
                });
        }


        function onEditClose() {
            if (nextRenderTimer != null) {
                clearTimeout(nextRenderTimer);
                nextRenderTimer = null;
            }

            if (currentStoryId != 0) {

                var jsonData = { messageType: "StoryLockDisengage", storyId: currentStoryId };
                socket.send($.toJSON(jsonData));

                SwarmopsJS.ajaxCall
                (baseAjaxUrl + "CancelStoryEdit",
                    { storyId: currentStoryId },
                    function () {
                        // do nothing
                    });
                currentStoryId = 0;
            }
        }

        function updateEditQueueCount() {
            SwarmopsJS.ajaxCall(
                baseAjaxUrl + "GetEditPoolCount",
                {},
                function(result) {
                    if (result.Success) {
                        $('span#spanEditQueueCount').text(result.DisplayMessage);
                    } else {
                        $('span#spanEditQueueCount').text('--');
                    }
                });
        }



        function removeStory(storyId) {

            // Nicely animates story to vanish, along with nearest separator, then removes it from DOM entirely

            var storyElement = $('div.divStoryWrapper[storyId="' + storyId + '"]');

            var separator = $(storyElement).next();
            if (!$(separator).hasClass("divSeparator")) {
                separator = $(storyElement).prev();
            }

            if ($(separator).hasClass("divSeparator")) {
                setTimeout(function(element) { $(element).slideUp(); }, 250, separator);
                setTimeout(function(element) { $(element).remove(); }, 750, separator);
            }

            setTimeout(function(element) {
                $(element).animate({ height: 0, opacity: 0 }, 'slow');
            }, 250, storyElement);

            setTimeout(function(element) {
                $(element).remove();
            }, 1000, storyElement);

            $('img.imageStoryThumbnail[storyId="' + currentStoryId + '"]').animate({ opacity: 0.5 });
        }


        function removeStoryRefillQueue(storyId) {
            removeStory(storyId);
            refillEditPool();
        }

        function refillEditPool() {
            refillTab("edit");
        }

        function refillPublishQueue() {
            refillTab("queue");
        }

        function refillPublished() {
            refillTab("published");
        }

        function refillRejected() {
            refillTab("rejected");
        }

        function refillAuthor() {
            refillTab("author");
        }

        var refillFunction = {};
        refillFunction["edit"] = "GetStoryEditPool";
        refillFunction["queue"] = "GetStoryPublishQueue";
        refillFunction["published"] = "GetStoriesPublished";
        refillFunction["rejected"] = "GetStoriesRejected";
        refillFunction["author"] = "GetStoriesAuthor";

        var tabId = {};
        tabId["edit"] = "div#divTabEdit";
        tabId["queue"] = "div#divTabQueue";
        tabId["published"] = "div#divTabPublished";
        tabId["rejected"] = "div#divTabRejected";
        tabId["author"] = "div#divTabAuthor";

        function refillTab (tab) {
            SwarmopsJS.ajaxCall(
                baseAjaxUrl + refillFunction[tab],
                { count: storyCount },
                function(result) {
                    if (result.Success) {
                        // For each of the stories in result.Stories, check if it exists on the current Dashboard,
                        // and if not, add it last. Normally, this should add exactly one story, last - but there
                        // are concurrency situations that cause exceptions, and force this kind of defensive
                        // coding.

                        var storyCreated = false;

                        result.Stories.forEach(function(value) {
                            var exists = $(tabId[tab] + ' div.divStoryWrapper[storyId="' + value.StoryId + '"]').length;

                            if (!exists) {
                                // Add this story at the bottom. To do so, first find the last story on the page, and append it after that story.

                                var newStory = (newStoryTemplates[tab]).replace(/%%STORYID%%/g, value.StoryId).replace(/%%CREATEDTIMESTAMP%%/g, value.CreatedTimestampUnix).replace(/%%PUBLISHEDTIMESTAMP%%/g, value.PublishedTimestampUnix);
                                storyCreated = true;

                                // If there is a story at all (the dash may be empty), put the new story after the last story. Otherwise, put it inside the content wrapper.

                                var storyWrappers = $(tabId[tab] + ' div.divStoryWrapper');

                                if ($(storyWrappers).length) {
                                    if (tab == "edit" || tab == "queue") {
                                        var last = $(storyWrappers).last();
                                        $(last).after($(newStory));
                                        $(last).after($(newStorySeparatorTemplate));
                                    } else {
                                        var first = $(storyWrappers).first();
                                        $(first).before($(newStory));
                                        $(first).before($(newStorySeparatorTemplate));
                                    }

                                } else {
                                    $(tabId[tab] + ' h2').after($(newStory));
                                }

                                // The element has been created. Populate it.

                                $('p.pStoryHeadline[storyId="' + value.StoryId + '"]').text(decodeURIComponent(value.Headline));
                                $('p.pStoryBody[storyId="' + value.StoryId + '"]').html(decodeURIComponent(value.Body));
                                $('span.spanTopicGeography[storyId="' + value.StoryId + '"]').text(value.TopicGeography);
                                $('img.imageStoryThumbnail[storyId="' + value.StoryId + '"]').attr("revisionId", value.RevisionId);

                                renderThumbnail($('img.imageStoryThumbnail[storyId="' + value.StoryId + '"]'));

                                // Bind "add comment" to enterComment

                                $(tabId[tab] + ' div.divStoryWrapper[storyId="' + value.StoryId + '"] span.spanAddComment').click(enterComment);

                                // Finally, bring the new story into view

                                $(tabId[tab] + ' div.divStoryWrapper[storyId="' + value.StoryId + '"]').slideDown();

                                // Load editor comments to story

                                if (tab == "edit") {
                                    getStoryEdits(value.StoryId);
                                }else if (tab == "published") {
                                    getStoryHistory(value.StoryId);
                                }
                            }
                        });

                        if (storyCreated) {
                            // Rebind events to buttons
                            unbindImageEvents();
                            bindImageEvents();
                        }
                    }
                }
            );
        }


        function getStoryEdits(storyId) {
            SwarmopsJS.ajaxCall(
                baseAjaxUrl + "GetStoryEdits",
                { storyId: storyId },
                function(result) {
                    if (result.Success) {
                        var storyEdited = false;

                        result.StoryEdits.forEach(function(editValue) {
                            if (addEdit(result.StoryId, editValue) && editValue.EditType == "Edit") {
                                storyEdited = true;
                            }
                        });

                        if (storyEdited) {
                            reloadStory(result.StoryId);
                        }
                    } else {
                        alertify.log("Could not retrieve comments for new story");
                    }
                });
        }

        function getStoryHistory(storyId) {
            SwarmopsJS.ajaxCall(
                baseAjaxUrl + "GetStoryHistory",
                { storyId: storyId },
                function(result) {
                    if (result.Success) {
                        result.StoryEdits.forEach(function(editValue) {
                            addEdit(result.StoryId, editValue); // same stuff, different set
                        });
                    } else {
                        alertify.log("Could not retrieve history for story");
                    }
                });
        }


        function initialRender() {
            $('img.imageStoryThumbnail').each(function (index, value) {
                renderThumbnail(value);
            });
        }

        function renderThumbnail(thumbnailImage) {
            SwarmopsJS.ajaxCall
            (
                baseAjaxUrl + "RenderThumbnail",
                { storyId: $(thumbnailImage).attr("storyId") },
                $.proxy(function (result) {
                    if (result.Success) {
                        $(this).attr('src', 'data:image/png;base64,' + result.DisplayMessage);
                    } else {
                        alertify.alert("An exception was thrown [RenderThumbnail]:<br/><br/>" + result.DisplayMessage);
                    }
                }, thumbnailImage));
        }


        var editIconLookup = {};
        editIconLookup["Warning"] =               "/Plugins/FalconwingMedia/Icons/iconshock-warning-128x96px.png";
        editIconLookup["Rejected"] =              '/Images/Icons/iconshock-red-cross-circled-128x96px.png';
        editIconLookup["TimedOut"] =              '/Images/Icons/iconshock-red-cross-circled-128x96px.png';
        editIconLookup["Redlight"] =              '/Images/Icons/iconshock-red-cross-128x96px.png';
        editIconLookup["RedlightInvalidated"] =   '/Plugins/FalconwingMedia/Icons/iconshock-red-cross-128x96px-disabled.png';
        editIconLookup["ToPublishQueue"] =        '/Plugins/FalconwingMedia/Icons/iconshock-green-plus-circled-128x96px.png';
        editIconLookup["PublishInvalidated"] =    '/Plugins/FalconwingMedia/Icons/iconshock-green-plus-circled-128x96px-disabled.png';
        editIconLookup["ReturnEditing"] =         '/Plugins/FalconwingMedia/Icons/undo-128x96px.png';
        editIconLookup["Greenlight"] =            '/Images/Icons/iconshock-green-tick-128x96px.png';
        editIconLookup["GreenlightInvalidated"] = '/Plugins/FalconwingMedia/Icons/iconshock-green-tick-128x96px-disabled.png';
        editIconLookup["Edit"] =                  '/Plugins/FalconwingMedia/Icons/iconshock-wrench-128x96px-centered.png';
        editIconLookup["Comment"] =               '/Plugins/FalconwingMedia/Icons/iconshock-balloon-128x96px-hot.png';
        editIconLookup["Published"] =             '/Plugins/FalconwingMedia/Icons/iconshock-network-128x96px.png';
        editIconLookup["StarAward0"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-0-128x96px.png';
        editIconLookup["StarAward1"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-1-128x96px.png';
        editIconLookup["StarAward2"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-2-128x96px.png';
        editIconLookup["StarAward3"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-3-128x96px.png';
        editIconLookup["StarAward4"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-4-128x96px.png';
        editIconLookup["StarAward5"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-5-128x96px.png';
        editIconLookup["StarAward6"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-6-128x96px.png';
        editIconLookup["StarAward7"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-7-128x96px.png';
        editIconLookup["StarAward8"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-8-128x96px.png';
        editIconLookup["StarAward9"] =            '/Plugins/FalconwingMedia/Icons/iconshock-stars-9-128x96px.png';
        


        function addEdit(storyId, edit) {
            if ($('tr#trEdit' + edit.StoryEditId).length) {
                // this edit is already on screen

                return false;
            }

            var imageSrc = editIconLookup[edit.EditType];
            var newEdit = newStoryEditTemplate.replace(/%%STORYEDITID%%/g, edit.StoryEditId).replace(/%%TIMESTAMP%%/g, edit.EditTimestamp).replace(/%%IMAGESRC%%/g,imageSrc).replace(/%%EDITTYPE%%/g, edit.EditType).replace(/%%COMMENT%%/g, decodeURIComponent(edit.Comment)).replace(/%%PERSON%%/g, edit.PersonIdString);

            if ($('table.tableEdits[storyId="' + storyId + '"] tr.trAddComment').length) {
                $('table.tableEdits[storyId="' + storyId + '"] tr.trAddComment').before(newEdit);
            } else {
                $('table.tableEdits[storyId="' + storyId + '"]').append(newEdit);
            }

            if (edit.PersonIdString.length) // always on edit screen if this is > 0, other tabs otherwise
            {
                updateOneEditTimestamp($('tr#trEdit' + edit.StoryEditId + " td.tdEditTicker"));

                if (edit.EditType == "Rejected" || edit.EditType == "ToPublishQueue" || edit.EditType == "TimedOut") {

                    // This story should be removed from display, it's been removed from the edit queue.

                    if (edit.EditType == "ToPublishQueue") {
                        $('img.imageStoryAction[storyId="' + storyId + '"]').attr('src', '/Images/Icons/iconshock-green-tick-128x96px.png').show();
                    } else {
                        $('img.imageStoryAction[storyId="' + storyId + '"]').attr('src', '/Images/Icons/iconshock-red-cross-128x96px.png').show();
                    }

                    $('img.imageStoryEdit[storyId="' + storyId + '"]').fadeOut();
                    $('img.imageStoryDefer[storyId="' + storyId + '"]').fadeOut();
                    $('img.imageStoryYes[storyId="' + storyId + '"]').fadeOut();
                    $('img.imageStoryNo[storyId="' + storyId + '"]').fadeOut();
                    $('img.imageStoryThumbnail[storyId="' + storyId + '"]').animate({ opacity: 0.5 });

                    removeStory(storyId);
                    // Refill the publish queue
                    refillPublishQueue();

                }
            } else {
                updateOneClosedTimestamp ($('tr#trEdit' + edit.StoryEditId + " td.tdEditTicker"));
            }

            if (imageSrc.indexOf("iconshock-stars") >= 0 || edit.EditType == "Rejected" || edit.EditType == "TimedOut") {
                // Status for a story in the "published" section

                $('img.imageStoryAction[storyId="' + storyId + '"]').attr('src', imageSrc).show();

                if (edit.EditType == "Rejected" || edit.EditType == "Timedout") {
                    $('div.divStoryWrapper[storyId="' + storyId + '"] div.divStoryMetrics').hide();
                }
            }

            return true;
        }



        function updateEditTimestamps() {
            $('div#divTabEdit td.tdEditTicker').each(function(index, element) {
                updateOneEditTimestamp(element);
            });
        }
        
        function updateOneEditTimestamp(element) {
            var thisTimestamp = $(element).parent().attr("timestamp");
            var ageSeconds = lastKnownTimestamp - thisTimestamp;

            if (ageSeconds < 0) {  // this is possible as newer-timestamps get to the Dashboard, with the last known timestamp being up to a minute ago
                ageSeconds = 0;
            }

            var hours = Math.floor(ageSeconds / 60 / 60);
            var minutes = Math.floor(ageSeconds / 60);
            minutes %= 60;

            var marker = "&#x2212;";

            if (hours == 0 && minutes == 0) {
                marker = '&#x2007;'; // figure-width space
            }

            var ticker = (hours < 10 ? "0" + hours : hours) + ":" + (minutes < 10 ? "0" + minutes: minutes);
            $(element).html(marker + ticker);
        }


        function updateClosedTimestamps() {
            $('td.tdEditTicker').not('div#divTabEdit td.tdEditTicker').each(function(index, element) {
                updateOneClosedTimestamp(element);
            });
        }

        function updateOneClosedTimestamp(element) {
            var thisTimestamp = $(element).parent().attr("timestamp");
            var storyPublished = parseInt($(element).parents("div.divStoryWrapper").attr("publishedTimestamp"));
            var storyCreated = parseInt($(element).parents("div.divStoryWrapper").attr("createdTimestamp"));
            var baseTime = storyPublished > 0 ? storyPublished : storyCreated;
            var ageSeconds = thisTimestamp - baseTime;

            var marker = '+';
            if (ageSeconds < 0) {
                marker = '&#x2212;'; // unicode minus
                ageSeconds = -ageSeconds;
            } else if (ageSeconds == 0) {
                marker = '&#x2007;'; // figure-width space
            }

            var hours = Math.floor(ageSeconds / 60 / 60);
            var minutes = Math.floor(ageSeconds / 60);
            minutes %= 60;

            var ticker = (hours < 10 ? "0" + hours : hours) + ":" + (minutes < 10 ? "0" + minutes: minutes);
            $(element).html(marker + ticker);
        }


        var baseAjaxUrl = "/Dashboard.aspx/";

        var oddImage = false;
        var currentStoryId = 0;
        var currentPhotoId = 0;
        var storyData = null;
        var nextRenderTimer = null;
        var userDefaultCountry = '<%= CurrentUser.CountryId != 0? CurrentUser.Country.Code : "--" %>';
        var editQueueCount = <%= Stories.GetGlobalEditQueueLength (CurrentUser) %>;
        var storyCount = 10;

        var lastKnownTimestamp = <%= DateTime.UtcNow.ToUnix() %>;

        var heartbeatTimer = null;
        var pingTimer = null;
        var heartbeatsLost = false;

        var socket = null;

        var newStoryTemplateStart =
            '<div class="divStoryWrapper" storyId="%%STORYID%%" createdTimestamp="%%CREATEDTIMESTAMP%%" publishedTimestamp="%%PUBLISHEDTIMESTAMP%%" style="display:none">' +
            '<div style="position:relative">' +
            '<div style="position: absolute">' +
            '<img class="imageStoryThumbnail" storyId="%%STORYID%%" revisionId="0" width ="" alt="Story Thumbnail"/>' +
            '</div>' +
            '<div style="position:absolute;top: 20px; left: 50px">' +
            '<img class="imageStoryAction" storyId="%%STORYID%%" height="96" width="128" src="" style="display:none"/>' +
            '</div>';

        var newStoryTemplateMid = 
            '</div>' +
            '<div style="margin-left:290px;min-height: 150px">' +
            '<span class="spanTopicGeography" storyId="%%STORYID%%">...</span><br/>' +
            '<p class="pStoryHeadline" storyId="%%STORYID%%">...</p>' +
            '<p class="pStoryBody" storyId="%%STORYID%%">...</p>';


        var newStoryTemplateEnd =                 
            '</div>' +
            '</div>';

        var newStoryButtonsEdit =
            '<div style="position: absolute; top: -5px; left: 220px; cursor: pointer">' +
            '<img class="imageStoryEdit" storyId="%%STORYID%%" height="32" src="/Plugins/FalconwingMedia/Icons/iconshock-balloon-wrench-128x96px.png"/>' +
            '</div>' +
            '<div style="position: absolute; top: 30px; left: 220px; cursor: pointer">' +
            '<img class="imageStoryDefer" storyId="%%STORYID%%" height="32" src="/Plugins/FalconwingMedia/Icons/iconshock-balloon-defer-128x96px.png"/>' +
            '</div>' +
            '<div style="position: absolute; top: 60px; left: 220px; cursor: pointer">' +
            '<img class="imageStoryNo" storyId="%%STORYID%%" height="32" src="/Images/Icons/iconshock-balloon-no-128x96px.png"/>' +
            '</div>' +
            '<div style="position: absolute; top: 90px; left: 220px; cursor: pointer">' +
            '<img class="imageStoryYes" storyId="%%STORYID%%" height="32" src="/Images/Icons/iconshock-balloon-yes-128x96px.png"/>' +
            '</div>';

        var newStoryButtonsQueue =
            '<div style="position: absolute; top: -5px; left: 220px; cursor: pointer">' +
            '<img class="imageStoryReturnEditing" storyId="%%STORYID%%" height="32" src="/Images/Icons/iconshock-balloon-undo-128x96px.png"/>' +
            '</div>';

        var newStoryEditDisplay =
            '<table class="tableEdits" storyId="%%STORYID%%" width="100%">' +
            '<tr class="trAddComment"><td width="100%" colspan="4"><span class="spanAddComment" style="cursor:pointer;margin-top:3px;margin-bottom:2px;display:inline-block">Add a comment...</span></td></tr>' +
            '</table>';

        var newStoryHistoryDisplay = 
            '<table class="tableEdits" storyId="%%STORYID%%" width="100%">' +
            '</table>';

        var newStorySeparatorTemplate = '<div style="margin-top:10px;margin-bottom:10px" class="divSeparator"><hr/></div>';


        var newStoryEditTemplate =
            '<tr class="trEdit" id="trEdit%%STORYEDITID%%" timestamp="%%TIMESTAMP%%" editType="%%EDITTYPE%%"><td class="tdEditImage" width="20px" style="width:20px"><img src="%%IMAGESRC%%" width="20" height="15" /></td><td class="tdEditTicker">--:--</td><td width="100%">%%COMMENT%%</td><td align="right">%%PERSON%%</td></tr>';

        var newStoryTemplates = {};
        newStoryTemplates["edit"] = newStoryTemplateStart + newStoryButtonsEdit + newStoryTemplateMid + newStoryEditDisplay + newStoryTemplateEnd;
        newStoryTemplates["queue"] = newStoryTemplateStart + newStoryButtonsQueue + newStoryTemplateMid + newStoryTemplateEnd;
        newStoryTemplates["published"] = newStoryTemplateStart + newStoryTemplateMid + newStoryHistoryDisplay + newStoryTemplateEnd;
        newStoryTemplates["rejected"] = newStoryTemplateStart + newStoryTemplateMid + newStoryHistoryDisplay + newStoryTemplateEnd;
        newStoryTemplates["stars"] = newStoryTemplateStart + newStoryTemplateMid + newStoryHistoryDisplay + newStoryTemplateEnd;
        newStoryTemplates["author"] = newStoryTemplateStart + newStoryTemplateMid + newStoryHistoryDisplay + newStoryTemplateEnd;

    </script>
    
    <style type="text/css">
        .entryFields textarea {
            font-size: 14px !important;
        }

        p {
            margin-top: 5px;
        }

        p.pStoryHeadline {
            font-weight: bold;
            font-size: 14px;
            padding-bottom: 5px !important;
        }

        p.pStoryBody {
            font-size: 12px;
        }

        table.tableEdits {
            font-size: 11px;
            font-weight: 300;
            color: blue;
            font-style: italic;
        }

        img.imageStoryThumbnail {
            width: 240px;
            height: 146px;
        }

        #textEditReason, #textEditHeadline, #textEditSources, #textEditPhotoSourceUrl, #textEditPhotoPersonName, #textEditPhotoDescription, #textEditPhotoCredit {
            width: 560px;
        }

        #textEditBody {
            margin-top: 5px;
            width: 564px;
            font-size: 80%;
        }

        #<%=this.DropCategories.ClientID%> {
            width: 250px;
        }

        tr.trEdit {
            vertical-align: top;
        }

        td.tdEditImage {
            position: relative;
            top: -2px;
        }

        td.tdEditTicker {
            white-space: nowrap;
            text-align: right;
        }

        html {
           overflow-y:scroll;
        }

        table.tableStoryMetrics {
            text-align: right;
        }

    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div id="divTabEdit" title="<img src='/Plugins/FalconwingMedia/Icons/iconshock-wrench-128px.png' width='64px' height='64px' />">
            <h2 id="h2EditQueue">STORIES IN YOUR EDIT POOL (<span id="spanEditQueueCount">...</span>)</h2>
            <asp:Repeater runat="server" ID="RepeaterStories">
                <ItemTemplate>
                    <div class="divStoryWrapper" storyId="<%#Eval ("StoryId") %>" priority="<%# Eval ("Priority") %>">
                        <div style="position:relative">
                            <div style="position: absolute">
                                <img class="imageStoryThumbnail" storyId="<%# Eval("StoryId") %>" revisionId="<%#Eval ("RevisionCount") %>" width ="" alt="Story Thumbnail"/>
                            </div>
                            <div style="position:absolute;top: 20px; left: 50px">
                                <img class="imageStoryAction" storyId="<%# Eval("StoryId") %>" height="96" width="128" src="/Plugins/FalconwingMedia/Icons/iconshock-padlock-128x96px.png" style="display:none"/>
                            </div>
                            <div style="position: absolute; top: -5px; left: 225px; cursor: pointer">
                                <img class="imageStoryEdit" storyId="<%# Eval("StoryId") %>" height="32"  src="/Plugins/FalconwingMedia/Icons/iconshock-balloon-wrench-128x96px.png"/>
                            </div>
                            <div style="position: absolute; top: 30px; left: 225px; cursor: pointer">
                                <img class="imageStoryDefer" storyId="<%# Eval("StoryId") %>" height="32" src="/Plugins/FalconwingMedia/Icons/iconshock-balloon-defer-128x96px.png"/>
                            </div>
                            <div style="position: absolute; top: 60px; left: 225px; cursor: pointer">
                                <img class="imageStoryNo" storyId="<%# Eval("StoryId") %>" height="32" src="/Images/Icons/iconshock-balloon-no-128x96px.png"/>
                            </div>
                            <div style="position: absolute; top: 90px; left: 225px; cursor: pointer">
                                <img class="imageStoryYes" storyId="<%# Eval("StoryId") %>" height="32" src="/Images/Icons/iconshock-balloon-yes-128x96px.png"/>
                            </div>
                        </div>
                        <div style="margin-left:290px;min-height: 150px">
                            <%#Eval ("Topic.Name") %>, <%# Eval("GeographyName") %><br/>
                            <p class="pStoryHeadline" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Headline")) %></p>
                            <p class="pStoryBody" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Body")) %> <%# Eval("SourceLinksHtml") %></p>
                            <table class="tableEdits" storyId="<%#Eval ("StoryId") %>" width="100%">
                                <tr class="trAddComment"><td width="100%" colspan="4"><span class="spanAddComment" style="cursor:pointer;margin-top:3px;margin-bottom:2px;display:inline-block">Add a comment...</span></td></tr>
                            </table>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div style="margin-top:10px;margin-bottom:10px" class="divSeparator"><hr/></div>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
        <div id="divTabQueue" title="<img src='/Plugins/FalconwingMedia/Icons/iconshock-hourglass-128px.png' width='64px' height='64px' />">
            <h2 id="h2PublishQueue">STORIES IN PUBLICATION QUEUE</h2>
            <asp:Repeater runat="server" ID="RepeaterPublishQueue">
                <ItemTemplate>
                    <div class="divStoryWrapper" storyId="<%#Eval ("StoryId") %>" priority="<%# Eval ("Priority")%>" >
                        <div style="position:relative">
                            <div style="position: absolute">
                                <img class="imageStoryThumbnail" storyId="<%# Eval("StoryId") %>" revisionId="<%#Eval ("RevisionCount") %>" width ="" alt="Story Thumbnail"/>
                            </div>
                            <div style="position:absolute;top: 20px; left: 50px">
                                <img class="imageStoryAction" storyId="<%# Eval("StoryId") %>" height="96" width="128" src="/Images/Icons/iconshock-red-cross-128x96px.png" style="display:none"/>
                            </div>
                            <div style="position: absolute; top: -5px; left: 225px; cursor: pointer">
                                <img class="imageStoryReturnEditing" storyId="<%# Eval("StoryId") %>" height="32"  src="/Images/Icons/iconshock-balloon-undo-128x96px.png"/>
                            </div>
                        </div>
                        <div style="margin-left:290px;min-height: 150px">
                            <%#Eval ("Topic.Name") %>, <%# Eval("GeographyName") %><br/>
                            <p class="pStoryHeadline" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Headline")) %></p>
                            <p class="pStoryBody" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Body")) %> <%# Eval("SourceLinksHtml") %></p>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div style="margin-top:10px;margin-bottom:10px" class="divSeparator"><hr/></div>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
        <div id="divTabPublished" title="<img src='/Plugins/FalconwingMedia/Icons/iconshock-green-tick-128px.png' width='64px' height='64px' />">
            <h2 id="h2Published">RECENTLY PUBLISHED</h2>
            <asp:Repeater runat="server" ID="RepeaterPublished">
                <ItemTemplate>
                    <div class="divStoryWrapper" storyId="<%#Eval ("StoryId") %>" priority="<%# Eval ("Priority")%>" createdTimestamp="<%# Eval("CreatedUnix") %>" publishedTimestamp ="<%# Eval("PublishedUnix") %>">
                        <div style="position:relative">
                            <div style="position: absolute">
                                <img class="imageStoryThumbnail" storyId="<%# Eval("StoryId") %>" revisionId="<%#Eval ("RevisionCount") %>" width ="" alt="Story Thumbnail"/>
                            </div>
                            <div style="position:absolute;top: 20px; left: 50px">
                                <img class="imageStoryAction" storyId="<%# Eval("StoryId") %>" height="96" width="128" src="/Images/Icons/iconshock-red-cross-128x96px.png" style="display:none"/>
                            </div>
                            <div style="position: absolute; top: 150px">
                                <table class="tableStoryMetrics" width="240px" style="width:240px">
                                    <thead>
                                        <tr><td>&nbsp;</td><td>Shares</td><td>Stars</td><td>Views</td></tr>
                                    </thead>
                                    <tbody>
                                        <tr><td>Twitter</td><td><%# Eval ("Metrics.Twitter.Shares") %></td><td><%# Eval ("Metrics.Twitter.Stars") %></td><td><%# Eval ("Metrics.Twitter.Views") %></td></tr>
                                        <tr><td>Facebook</td><td><%# Eval ("Metrics.Facebook.Shares") %></td><td><%# Eval ("Metrics.Facebook.Stars") %></td><td><%# Eval ("Metrics.Facebook.Views") %></td></tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div style="margin-left:290px;min-height: 150px">
                            <%#Eval ("Topic.Name") %>, <%# Eval("GeographyName") %><br/>
                            <p class="pStoryHeadline" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Headline")) %></p>
                            <p class="pStoryBody" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Body")) %> <%# Eval("SourceLinksHtml") %></p>
                            <table class="tableEdits" storyId="<%#Eval ("StoryId") %>" width="100%">
                            </table>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div style="margin-top:10px;margin-bottom:10px" class="divSeparator"><hr/></div>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
        <div id="divTabRejected" title="<img src='/Plugins/FalconwingMedia/Icons/iconshock-red-cross-128px.png' width='64px' height='64px' />">
            <h2>RECENTLY REJECTED</h2>
            <asp:Repeater runat="server" ID="RepeaterRejected">
                <ItemTemplate>
                    <div class="divStoryWrapper" storyId="<%#Eval ("StoryId") %>" priority="<%# Eval ("Priority")%>" createdTimestamp="<%# Eval("CreatedUnix") %>" publishedTimestamp ="<%# Eval("PublishedUnix") %>">
                        <div style="position:relative">
                            <div style="position: absolute">
                                <img class="imageStoryThumbnail" storyId="<%# Eval("StoryId") %>" revisionId="<%#Eval ("RevisionCount") %>" width ="" alt="Story Thumbnail"/>
                            </div>
                            <div style="position:absolute;top: 20px; left: 50px">
                                <img class="imageStoryAction" storyId="<%# Eval("StoryId") %>" height="96" width="128" src="/Images/Icons/iconshock-red-cross-128x96px.png" style="display:none"/>
                            </div>
                        </div>
                        <div style="margin-left:290px;min-height: 150px">
                            <%#Eval ("Topic.Name") %>, <%# Eval("GeographyName") %><br/>
                            <p class="pStoryHeadline" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Headline")) %></p>
                            <p class="pStoryBody" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Body")) %> <%# Eval("SourceLinksHtml") %></p>
                            <table class="tableEdits" storyId="<%#Eval ("StoryId") %>" width="100%">
                            </table>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div style="margin-top:10px;margin-bottom:10px" class="divSeparator"><hr/></div>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
        <div id="divTabStars" title="<img src='/Plugins/FalconwingMedia/Icons/iconshock-star-gold-128px.png' width='64px' height='64px' />">
            <h2>MOST SHARES AND STARS (NOT DYNAMIC YET)</h2>
            <asp:Repeater runat="server" ID="RepeaterStars">
                <ItemTemplate>
                    <div class="divStoryWrapper" storyId="<%#Eval ("StoryId") %>" priority="<%# Eval ("Priority")%>" createdTimestamp="<%# Eval("CreatedUnix") %>" publishedTimestamp ="<%# Eval("PublishedUnix") %>">
                        <div style="position:relative">
                            <div style="position: absolute">
                                <img class="imageStoryThumbnail" storyId="<%# Eval("StoryId") %>" revisionId="<%#Eval ("RevisionCount") %>" width ="" alt="Story Thumbnail"/>
                            </div>
                            <div style="position:absolute;top: 20px; left: 50px">
                                <img class="imageStoryAction" storyId="<%# Eval("StoryId") %>" height="96" width="128" src="/Images/Icons/iconshock-red-cross-128x96px.png" style="display:none"/>
                            </div>
                            <div style="position: absolute; top: 150px">
                                <table class="tableStoryMetrics" width="240px" style="width:240px">
                                    <thead>
                                        <tr><td>&nbsp;</td><td>Shares</td><td>Stars</td><td>Views</td></tr>
                                    </thead>
                                    <tbody>
                                        <tr><td>Twitter</td><td><%# Eval ("Metrics.Twitter.Shares") %></td><td><%# Eval ("Metrics.Twitter.Stars") %></td><td><%# Eval ("Metrics.Twitter.Views") %></td></tr>
                                        <tr><td>Facebook</td><td><%# Eval ("Metrics.Facebook.Shares") %></td><td><%# Eval ("Metrics.Facebook.Stars") %></td><td><%# Eval ("Metrics.Facebook.Views") %></td></tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div style="margin-left:290px;min-height: 150px">
                            <%#Eval ("Topic.Name") %>, <%# Eval("GeographyName") %><br/>
                            <p class="pStoryHeadline" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Headline")) %></p>
                            <p class="pStoryBody" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Body")) %> <%# Eval("SourceLinksHtml") %></p>
                            <table class="tableEdits" storyId="<%#Eval ("StoryId") %>" width="100%">
                            </table>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div style="margin-top:10px;margin-bottom:10px" class="divSeparator"><hr/></div>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
        <div id="divTabAuthor" title="<img src='/Images/Icons/iconshock-redshirt-128px.png' width='64px' height='64px' />">
            <h2 id="h3">YOUR STORIES</h2>
            <asp:Repeater runat="server" ID="RepeaterAuthor">
                <ItemTemplate>
                    <div class="divStoryWrapper" storyId="<%#Eval ("StoryId") %>" priority="<%# Eval ("Priority")%>" createdTimestamp="<%# Eval("CreatedUnix") %>" publishedTimestamp ="<%# Eval("PublishedUnix") %>">
                        <div style="position:relative">
                            <div style="position: absolute">
                                <img class="imageStoryThumbnail" storyId="<%# Eval("StoryId") %>" revisionId="<%#Eval ("RevisionCount") %>" width ="" alt="Story Thumbnail"/>
                            </div>
                            <div style="position:absolute;top: 20px; left: 50px">
                                <img class="imageStoryAction" storyId="<%# Eval("StoryId") %>" height="96" width="128" src="/Images/Icons/iconshock-red-cross-128x96px.png" style="display:none"/>
                            </div>
                            <div class="divStoryMetrics" style="position: absolute; top: 150px">
                                <table class="tableStoryMetrics" width="240px" style="width:240px">
                                    <thead>
                                        <tr><td>&nbsp;</td><td>Shares</td><td>Stars</td><td>Views</td></tr>
                                    </thead>
                                    <tbody>
                                        <tr><td>Twitter</td><td><%# Eval ("Metrics.Twitter.Shares") %></td><td><%# Eval ("Metrics.Twitter.Stars") %></td><td><%# Eval ("Metrics.Twitter.Views") %></td></tr>
                                        <tr><td>Facebook</td><td><%# Eval ("Metrics.Facebook.Shares") %></td><td><%# Eval ("Metrics.Facebook.Stars") %></td><td><%# Eval ("Metrics.Facebook.Views") %></td></tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div style="margin-left:290px;min-height: 150px">
                            <%#Eval ("Topic.Name") %>, <%# Eval("GeographyName") %><br/>
                            <p class="pStoryHeadline" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Headline")) %></p>
                            <p class="pStoryBody" storyId="<%#Eval("StoryId") %>"><%# Uri.UnescapeDataString ((string) Eval("Body")) %> <%# Eval("SourceLinksHtml") %></p>
                            <table class="tableEdits" storyId="<%#Eval ("StoryId") %>" width="100%">
                            </table>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div style="margin-top:10px;margin-bottom:10px" class="divSeparator"><hr/></div>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
        <div id="div2" title="<img src='/Plugins/FalconwingMedia/Icons/bitcoin-icon-128px.png' width='64px' height='64px' />">
            <h2 id="h1">YOUR REVENUE (UNDER CONSTRUCTION)</h2>
        </div>
    </div>

    <Swarmops5:ModalDialog ID="DialogEdit" OnClientClose="onEditClose" runat="server">
        <DialogCode>
            <h2>EDITING STORY #<span id="spanEditStoryId">X,XYZ</span></h2>
            <div class="entryFields" style="width: 600px">
                <span class="spanEditField"><input type="text" id="textEditReason"/>&#8203;<br/></span>
                <asp:DropDownList runat="server" ID="DropCategories"/>&#8203;
                <asp:DropDownList runat="server" ID="DropCountries"/><br/>
                <input type="text" id="textEditHeadline"/>&#8203;<br/>
                <span id="spanSelectedPhotoDescription">&mdash;</span>
                <input type="text" id="textEditSources"/>&#8203;<br/>
                <textarea id="textEditBody" rows="8"></textarea>&#8203;<br/>
                <input type="button" value="Commit" id="buttonEditCommit" class="buttonAccentColor NoInputFocus" />
            </div>
            <div class="entryLabels" style="height:450px">
                <span class="spanEditField">Reason for editing<br /></span>
                Topic, Country<br/>
                Headline<br/>
                Photo (<a id="linkSelectPhoto" href="#">select...</a>)<br/>
                Sources (space separated links)<br/>
                Body<br/>
                <div id="divPreviewRotator" style="position:relative; margin-top:15px; margin-bottom:5px">
                    <img id="imagePreview1" class="imageRotate" width="260px" style="position:absolute;left:0" />
                    <img id="imagePreview2" class="imageRotate" width="260px" style="position:absolute;left:0;display:none" />
                </div>
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>
    
    <Swarmops5:ModalDialog ID="DialogPhoto" runat="server">
        <DialogCode>
            <h2>CREATING PHOTO</h2>
            <p>We're publishing under EU laws, which means that we may freely republish anything already published <strong>openly</strong> on the web without
            coming into conflict with copyright monopolies (see ECJ Svensson v. Svensson 2014). However, we still need to provide proper photo credit.
            If the photo is of a person, write their full name for later searchability (like "Yanis Varoufakis").</p>
            <div class="entryFields" style="width: 600px">
                <input type="text" id="textEditPhotoSourceUrl" placeholder="http://example.org/photo.jpg"/>&#8203;<br/>
                <input type="text" id="textEditPhotoDescription" placeholder="Yanis Varoufakis smiling in victory"/>&#8203;<br/>
                <input type="text" id="textEditPhotoPersonName" placeholder="Yanis Varoufakis"/>&#8203;<br/>
                <input type="text" id="textEditPhotoCredit" placeholder="Agency and/or Photographer Name">&#8203;<br/>
                <input type="button" value="Fetch" id="buttonCreatePhoto" class="buttonAccentColor NoInputFocus" />
            </div>
            <div class="entryLabels" style="height:390px">
                Photo source URL<br/>
                Description<br/>
                Person name (if person)<br/>
                Photo Credit<br/>
                <div id="div1" style="position:relative; margin-top:15px; margin-bottom:5px">
                    <img id="Img1" class="imageRotate" width="260px" style="position:absolute;left:0" />
                    <img id="Img2" class="imageRotate" width="260px" style="position:absolute;left:0;display:none" />
                </div>
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>

</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue">ACTIONS 2<span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" onclick="createStory();" >
                <div class="link-row-icon" style="background-image:url('/Images/Icons/iconshock-add-16px.png');position:relative;top:-1px;left:-3px"></div>
                Create New Story
            </div>
        </div>
    </div>

    <h2 class="blue">LAB<span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <table border="0" cellpadding="0" cellspacing="2" style="width:100%">
                <tr><td>Connected</td><td align="right" id="tdSocketConnected">---</td></tr>
                <tr><td>Editors online</td><td align="right" id="tdSocketEditorCount">---</td></tr>
                <tr><td>Global edit pool</td><td align="right" id="tdSocketEditQueueCount">---</td></tr>
                <tr><td>Publication queue</td><td align="right" id="tdSocketPubQueueCount">---</td></tr>
                <tr><td>Next publication</td><td align="right" id="tdSocketPubQueueNext">---</td></tr>
                <tr><td>Pubqueue lasts until</td><td align="right" id="tdSocketPubQueueExtent">---</td></tr>
            </table>
        </div>
    </div>

</asp:Content>

