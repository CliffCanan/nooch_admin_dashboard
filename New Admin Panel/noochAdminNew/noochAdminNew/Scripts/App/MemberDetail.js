var NoochId = '';
var operationtoperform = 0;
var escapeKeyPressed = false;

$(document).ready(function () {
    if ($('#DocStatus').val() == "Success")
    {
        swal("Awesome!", "File Uploaded Successfully!", "success");
        $('#DocStatus').val('');
    }
    else if ($('#DocStatus').val() == "Failed")
        sweetAlert("Oops...", "Something went wrong!", "error");

    $('#generatePwBtn').change(function () {
        if ($('#pwd').val().length > 0)
            $('#pwd').val('');
        if ($(this).is(':checked'))
            Member.GenerateNewPassword();
    })

    $("#MemberMenuExpander").trigger("click");

    $('[data-toggle="tooltip"]').tooltip()

    NoochId = $('#nId').attr('data-val');
    console.log(NoochId);

    // Format the contact number if present
    if ($("#contactNumber").val().length > 1)
        $("#contactNumber").val(function (i, text) {
            text = text.replace(/(\d{3})(\d{3})(\d{4})/, "($1) $2-$3");
            return text;
        });

    $('.locLink').click(function () {
        var lat = $(this).attr('data-lat');
        var longi = $(this).attr('data-long');
        var locText = $(this).attr('data-locText');

        //console.log('LAT AND LONG: ' + lat + ", " + longi);

        var src = 'https://www.google.com/maps/embed/v1/place?q=' + lat + ',' + longi + '&center=' + lat + ',' + longi + '&key=AIzaSyDrUnX1gGpPL9fWmsWfhOxIDIy3t7YjcEY&zoom=12';
        $('#googleFrame').attr('src', src);

        $("#citystate").text(locText);

        $('#modal-transferLocation').modal();
    });

    $('#userLoc').click(function () {
        var lat = $(this).attr('data-lat');
        var long = $(this).attr('data-long');

        var latLong = new google.maps.LatLng(Number(lat), Number(long));

        console.log('LAT AND LONG: ' + latLong);

        var mapDivInModal = document.getElementById("userMapInModal");

        var map = new google.maps.Map(mapDivInModal, {
            zoom: 11,
            center: latLong
        });

        var marker = new google.maps.Marker({
            position: latLong,
            map: map,
            title: 'Last Location'
        });

        mapDivInModal.style.height = '420px';

        $('#modal-userLocation').modal();
    });

    $('.idDocImg').click(function () {
        var src = $(this).attr('src');

        $('#idImageBig').attr('src', src);

        $('#idDocModal').modal();
    })

    $(document).keyup(function (e) {
        if (e.keyCode == 27) // escape key maps to keycode `27`
            escapeKeyPressed = true;
    });
});


function checkIfUserLocationExists() {

    var lat = $('#userLoc').attr('data-lat');
    console.log(lat);

    if (lat != "none")
    {
        console.log("Lat did not = 'none'. ");

        var mapDiv = document.getElementById("userMap");
        var long = $('#userLoc').attr('data-long');
        var latLong = new google.maps.LatLng(Number(lat), Number(long));

        var map = new google.maps.Map(mapDiv, {
            zoom: 7,
            center: latLong
        });

        var marker = new google.maps.Marker({
            position: latLong,
            map: map,
            title: 'Last Location'
        });

        mapDiv.style.height = '180px';
    }
    else
    {
        console.log("Lat = 'none' ");

        $('#userLoc').html('<span style="color:rgba(88,90,92,.8) !important;"><em class="center-block text-center m-t-md">No location available.</em></span>');
    }
}

var notifySuspendedUser = false;

function suspendUserPrompt() {
    swal({
        title: "Notify this user about being suspended?",// + firstName + "?",
        text: "Should this user to be notified about being suspended?",
        type: "warning",
        showCancelButton: true,
        cancelButtonText: "No",
        confirmButtonColor: "#3fabe1",
        confirmButtonText: "Yes",
        html: true,
        customClass: "securityAlert confirmBtnFullWidth"
    }, function (isConfirm) {
        setTimeout(function () {
            if (escapeKeyPressed == true)
                escapeKeyPressed = false;
            else
            {
                if (isConfirm) notifySuspendedUser = true;
                else notifySuspendedUser = false;

                Member.ApplyChoosenOperation(1);
            }
        }, 200);
    });

}


$('#DeleteUser').click(function () {
    $('#myModalConfirmDelete').modal('show');
});


$('#ChangePassword').click(function () {
    $('#changePwd').modal('show');
});

function showBlockUI(text) {
    $.blockUI({
        message: '<span><i class="fa fa-refresh fa-spin fa-loading"></i></span><br/><span class="loadingMsg">' + text + '</span>',
        css: {
            border: 'none',
            padding: '26px 8px 20px',
            backgroundColor: '#000',
            '-webkit-border-radius': '16px',
            '-moz-border-radius': '16px',
            'border-radius': '16px',
            opacity: '.75',
            margin: '0 auto',
            color: '#fff'
        }
    });
}

var Member = function () {
    function applyOperation(operation) {
        if (NoochId == '' || NoochId == null)
        {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        var url = "../Member/ApplyOperation";
        var data = {};
        data.operation = operation;
        data.noochIds = NoochId;

        if (operation == 1) // Suspend User
            data.sendEmail = notifySuspendedUser;

        $.post(url, data, function (result) {
            if (result.IsSuccess == true)
            {
                console.log(result.Message);
                console.log(JSON.stringify(result));

                // iterating through all innerclass objects

                $.each(result.MemberOperationsOuterClass, function (key, value) {
                    if (value.IsSuccess == true)
                    {
                        toastr.success(value.Message, value.NoochId);

                        if (operation == 1) // Suspend User
                            $("#memberStatus > *").html('Suspended');
                        else if (operation == 4)
                            $("#memberStatus > *").html('Active');
                        else if (operation == 5)
                        {
                            $('#myModalConfirmDelete').modal('hide');
                            window.location.replace("../Member/ListAll");
                        }
                    }
                    else
                    {
                        $('#myModalConfirmDelete').modal('hide');
                        toastr.error(value.Message, value.NoochId);
                    }
                });

                if (operation < 4 && operation != 1)
                    location.reload(true);
            }
            else
                toastr.error('An error occured on the server, please try again!', 'Error');
        })
		.done(function () {
		    toastr.success('Operation ' + operation + ' Successful');
		})
		.fail(function () {
		    toastr.error('Operation ' + operation + ' Failed');
		});
    }


    function editCip() {
        $('#mdc_cipTag').css('display', 'block');
        $('#spnCip_Tag').css('display', 'none');
    }


    function editdetails() {
        if (NoochId == '')
        {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        var url = "../Member/EditMemberDetails";
        var data = {};
        data.contactno = $("#contactNumber").val().replace(/\D/g, '');
        data.streetaddress = $("#streetaddress").val();
        data.city = $("#city").val().trim();
        data.secondaryemail = $("#secondaryemail").val();
        data.recoveryemail = $("#recoveryemail").val();
        data.state = $("#stateinput").val();
        data.zip = $("#zipcodeinput").val();
        data.noochid = NoochId;
        data.ssn = $("#ssninput").val().trim();
        data.dob = $("#dobinput").val().trim();
        data.transferLimit = $("#transferLimitinput").val().trim();
        if ($('#mdc_cipTag').css('display') != 'none')
            data.cip_tag = $('#mdc_cipTag').val();
        else
            data.cip_tag = "";
        console.log(data);

        $.post(url, data, function (result) {
            if (result.IsSuccess == true)
            {
                toastr.success(result.Message, 'Success');

                $("#contactNumber").val(result.contactnum);
                $("#streetaddress").val(result.Address);
                $("#city").val(result.City);
                $("#secondaryemail").val(result.secondaryemail);
                $("#recoveryemail").val(result.recoveryemail)
                $("#stateinput").val(result.state);
                $("#zipcodeinput").val(result.zip);
                $("#ssninput").val(result.ssn);
                $("#dobinput").val(result.dob);

                toastr.info('Reloading this page...', 'FYI', { timeOut: '2500' });
                setTimeout(function () { location.reload(true) }, 2500);
            }
            else
            {
                console.log("ERROR!");
                console.log(result);
                toastr.error(result.Message, 'Error');
            }
        });
    }


    function restpin() {
        if (NoochId == '')
        {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        var url = "../Member/ResetPin";
        var data = {};
        data.noochId = NoochId;

        $.post(url, data, function (result) {
            if (result.IsSuccess == true)
            {
                toastr.success(result.Message, 'Succcess');
                $("#pinNumber").html(result.Pin);
            }
            else
            {
                toastr.error(result.Message, 'Error');
            }
        });
    }


    // Manually set bank account's status to 'Verified'
    function verifyBankAccount() {
        var accountId = $('#bnkIdHidden').val();

        if (accountId == '')
        {
            toastr.error('No bank account was selected...', 'Error');
            return;
        }

        swal({
            title: "Send Email?",
            text: "Should this user be notified about this action:<br/>" +
                  "<span class='text-center' style='margin: 10px auto; font-weight:500;'>Verify Bank Account</span>",
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#3fabe1",
            confirmButtonText: "Send Email",
            cancelButtonText: "No Notification",
            closeOnConfirm: true,
            allowEscapeKey: true,
            html: true
        }, function (isConfirm) {

            setTimeout(function () {
                if (escapeKeyPressed == true)
                {
                    escapeKeyPressed = false;
                }
                else
                {
                    var data = {};
                    data.accountId = accountId;

                    if (isConfirm)
                    {
                        data.sendEmail = true;
                    }
                    else
                    {
                        data.sendEmail = false;
                    }

                    var url = "../Member/VerifyAccount";

                    $.post(url, data, function (result) {
                        console.log(result);

                        if (result.IsSuccess == true)
                        {
                            toastr.success(result.Message, 'Success');

                            $('#bankAccountStatusDiv').html('');
                            $('#bankAccountStatusDiv').html("<span class='text-success' style='display: inline-block'>Verified</span>");
                        }
                        else
                        {
                            toastr.error(result.Message, 'Error');
                        }
                    });
                }
            }, 200);
        });
    }


    // Manually set bank account's status to 'Pending Review'
    function unVerifyBankAccount() {
        var accountId = $('#bnkIdHidden').val();
        if (accountId == '')
        {
            toastr.error('No bank account was selected...', 'Error');
            return;
        }

        var url = "../Member/UnVerifyAccount";
        var data = {};
        data.accountId = accountId;

        $.post(url, data, function (result) {
            if (result.IsSuccess == true)
            {
                toastr.success(result.Message, 'Success');

                $('#bankAccountStatusDiv').html('');
                $('#bankAccountStatusDiv').html("<span class='text-warning' style='display: inline-block'>Pending Review</span>");
            }
            else
            {
                toastr.error(result.Message, 'Error');
            }
        });
    }


    // Open AdminNotes Modal
    function AdminNoteAboutUserModalPopup() {
        if (NoochId == '')
        {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        var url = "../Member/AdminNoteAboutUserModalPopup";
        var data = {};
        data.noochId = NoochId;

        $.post(url, data, function (result) {
            if (result.IsSuccess == true)
            {
                $('#Modal-AdminNotes').modal();
                $("#AmdinNotesAboutUser").val(result.AdminNote);
            }
            else
            {
                toastr.error(result.Message, result.Message);
            }
        });
    }


    // Provide Info About the User In ModalPopup
    function SaveAdminNoteForUser() {
        if (NoochId == '')
        {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        if ($("#AmdinNotesAboutUser").val() == " ")
        {
            $('#Modal-AdminNotes').modal('hide');
        }

        var url = "../Member/SaveAdminNoteForUser";
        var data = {};
        data.noochid = NoochId;
        data.AdminNote = $("#AmdinNotesAboutUser").val();
        $.post(url, data, function (result) {
            if (result == "Success")
            {
                toastr.success(result.Message, 'Admin Note edited successfully.');
                $('#Modal-AdminNotes').modal('hide');
            }
            else
            {
                toastr.error(result.Message, 'Error');
                $('#Modal-AdminNotes').modal('hide');
            }
        });
    }


    // Manually set bank account's status to 'Verified'
    function getSynapseInfo() {

        var authKey = $('#synAuthKey').text();

        if (authKey == '')
        {
            toastr.error('No Synapse Auth key was selected!', 'Error');
            return;
        }

        var url = "https://synapsepay.com/api/v2/user/show";
        $.ajax({
            type: 'POST',
            url: url,
            data: JSON.stringify({
                "oauth_consumer_key": authKey
            }),
            success: function (data) {
                console.log(data);

                if (data.success == true)
                {
                    toastr.success(data.reason, 'Success');
                    alert(JSON.stringify(data));
                }
                else
                {
                    toastr.error(data.reason, 'Error');
                }
            },
            error: function (e) {
                console.log(e);
            },
            dataType: "json",
            contentType: "application/json"
        });
    }


    // Manually set bank account's status to 'Verified'
    function sendSmsReminderForVerification() {
        var contactNumber = $('#contactNumber').val();
        var data = {};

        data.noochIds = NoochId;
        var url = "../Member/ReSendVrificationSMS";

        $.post(url, data, function (result) {
            if (result.IsSuccess)
                toastr.success(result.Message, 'Verification message sent successfully.');
            else
                toastr.error(result.Message, 'Error');
        });

    }


    function ChangePassword() {
        if ($("#pwd").val() == '')
            return false;

        $("#btnChangePassword").text('Updating...');
        $('#btnChangePassword').attr('disabled', 'disabled');
        var sendEmail = ($('#pwChangeSendEmail').is(':checked'));

        var data = {};
        data.newPassword = $("#pwd").val();
        data.noochId = NoochId;
        data.sendEmail = sendEmail;
        var url = "../Member/UpdatePassword";

        $.post(url, data, function (result) {
            if (result.IsSuccess)
                toastr.success(result.Message, 'Updated Successfully.');
            else
                toastr.error(result.Message, 'Error');

            $('#changePwd').modal('toggle');
            $("#btnChangePassword").text('Yes - Update');
            $('#pwd').val('');
            $('#btnChangePassword').removeAttr("disabled");
        });
    }


    function GenerateNewPassword() {
        var data = {};
        var url = "../Member/GenerateNewPassword";
        $.post(url, data, function (result) {
            if (result.IsSuccess)
            {
                $("#pwd").val(result.Message);
            }
            else
            {
                toastr.error(result.Message, 'Error');
            }
        });
    }


    function submitDocManual() {
        var memId = $('#memId').attr('data-val');
        var imgUrl = $('#idImageBig').attr('src');

        console.log("MemID: [" + memId + "], ImgUrl: [" + imgUrl + "]")

        if (NoochId == '')
        {
            toastr.error('No NoochID was selected...', 'Error');
            return;
        }
        else if (memId == '')
        {
            toastr.error('No MemberID selected', 'Error');
            return;
        }
        else if (imgUrl == '')
        {
            toastr.error('No Img URL found', 'Error');
            return;
        }

        showBlockUI("Submitting Doc Img...");

        var url = "../Member/submitDocToSynapseV3_manual";
        var data = {};
        data.memid = memId;
        data.docUrl = imgUrl;

        console.log("Data.docUrl: [" + data.docUrl + "]");

        $.post(url, data, function (result) {
            $.unblockUI();
            console.log(result.msg);

            if (result.isSuccess == true)
                toastr.success(result.msg, 'Succcess');
            else
                toastr.error(result.msg, 'Error');
        });
    }


    function submitSsn() {
        var memId = $('#memId').attr('data-val');

        if (NoochId == '')
        {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }
        if (memId == '' || memId.length < 20)
        {
            toastr.error('No MemberID was selected...', 'Error');
            return;
        }

        showBlockUI("Submitting SSN");

        var url = "../Member/submitSsnToSynapseV3";
        var data = {};
        data.memid = memId;
        $.post(url, data, function (result) {
            $.unblockUI();
            console.log(result.msg);
            if (result.isSuccess == true)
            {
                toastr.success(result.msg, 'SSN Submitted Successfully');
            }
            else
            {
                toastr.error(result.msg, 'Error');
            }
        });
    }


    function refreshSynapse() {
        var memId = $('#memId').attr('data-val');

        if (memId == '' || memId.length < 20)
        {
            toastr.error('No MemberID was selected...', 'Error');
            return;
        }

        showBlockUI("Refreshing User With Synapse...");

        var url = "../Member/refreshSynapseUserV3";
        var data = {};
        data.memid = memId;
        $.post(url, data, function (result) {
            $.unblockUI();
            console.log(result.msg);
            if (result.isSuccess == true)
            {
                toastr.success(result.msg, 'Synapse refreshed successfully');
                toastr.info('Reloading this page...', 'FYI', { timeOut: '2500' });
                setTimeout(function () { location.reload(true) }, 2500);
            }
            else
                toastr.error(result.msg, 'Error');
        });
    }

    function refreshSynapseBank() {
        var memId = $('#memId').attr('data-val');

        if (memId == '' || memId.length < 20)
        {
            toastr.error('No MemberID was selected...', 'Error');
            return;
        }

        showBlockUI("Refreshing Banks With Synapse...");

        var url = "../Member/refreshSynapseBankV3";
        var data = {};
        data.memid = memId;
        $.post(url, data, function (result) {
            $.unblockUI();
            console.log(result.msg);
            if (result.isSuccess == true)
                toastr.success(result.msg, 'Synapse refreshed successfully');
            else
                toastr.error(result.msg, 'Error');
        });
    }

    return {
        ApplyChoosenOperation: applyOperation,
        EditMember: editdetails,
        ResetPin: restpin,
        OpenPopupForAdminNote: AdminNoteAboutUserModalPopup,
        AdminNoteForUser: SaveAdminNoteForUser,
        VerifyBankAccount: verifyBankAccount,
        UnVerifyBankAccount: unVerifyBankAccount,
        getSynapseInfo: getSynapseInfo,
        sendSmsReminder: sendSmsReminderForVerification,
        ChangePassword: ChangePassword,
        GenerateNewPassword: GenerateNewPassword,
        editCip: editCip,
        submitDocManual: submitDocManual,
        submitSsn: submitSsn,
        refreshSynapse: refreshSynapse,
        refreshSynapseBank: refreshSynapseBank
    };
}();