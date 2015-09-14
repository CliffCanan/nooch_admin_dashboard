var NoochId = '';
var operationtoperform = 0;

$(document).ready(function () {
    $("#MemberMenuExpander").trigger("click");

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    NoochId = getParameterByName('NoochId');

    // Format the contact number if present
    if ($("#contactNumber").val().length > 1) {
        $("#contactNumber").val(function (i, text) {
            text = text.replace(/(\d{3})(\d{3})(\d{4})/, "($1) $2-$3");
            return text;
        });
    }

    $('.locLink').click(function () {
        var lat = $(this).attr('data-lat');
        var longi = $(this).attr('data-long');
        var locText = $(this).attr('data-locText');

        //console.log('LAT AND LONG: ' + lat + ", " + longi);

        var v = 'https://www.google.com/maps/embed/v1/place?q=' + lat + ',' + longi + '&center=' + lat + ',' + longi + '&key=AIzaSyDrUnX1gGpPL9fWmsWfhOxIDIy3t7YjcEY&zoom=12';
        $('#googleFrame').attr('src', v);

        $("#citystate").text(locText);

        $('#modal-transferLocation').modal();
    });

    $('#idDocLnk').click(function () {
        var src = $('.idDocImg').attr('src');

        $('#idImageBig').attr('src', src);

        $('#idDocModal').modal();
    })
});


$('#DeleteUser').click(function () {
    $('#myModalConfirmDelete').modal('show');
});

var Member = function () {
    function applyOperation(operation) {
        if (NoochId == '') {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        var url = "../Member/ApplyOperation";
        var data = {};
        data.operation = operation;
        data.noochIds = NoochId;
        $.post(url, data, function (result) {

            if (result.IsSuccess == true) {
                console.log(result.Message);
                console.log(result.MemberOperationsOuterClass);

                // iterating through all innerclass objects

                $.each(result.MemberOperationsOuterClass, function (key, value) {
                    if (value.IsSuccess == true) {
                        toastr.success(value.Message, value.NoochId);

                        if (operation == 4) {
                            $("#memberStatus").html('Active');
                        }
                        else if (operation == 5) {
                            window.location.replace("../Member/ListAll");
                            $('#myModalConfirmDelete').modal('hide');
                        }
                    }
                    else {
                        $('#myModalConfirmDelete').modal('hide');
                        toastr.error(value.Message, value.NoochId);
                    }
                });
                if (operation != 5) {
                    location.reload(true);
                }

            }
            else {
                toastr.error('An error occured on the server, please try again!', 'Error');
            }
        });

    }


    function editdetails() {
        if (NoochId == '') {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        var url = "../Member/EditMemberDetails";
        var data = {};
        data.contactno = $("#contactNumber").val();
        data.streetaddress = $("#streetaddress").val();
        data.city = $("#city").val();
        data.secondaryemail = $("#secondaryemail").val();
        data.recoveryemail = $("#recoveryemail").val();
        data.state = $("#stateinput").val();
        data.zip = $("#zipcodeinput").val();
        data.noochid = NoochId;

        console.log(data);

        $.post(url, data, function (result) {
            if (result.IsSuccess == true) {
                toastr.info('Reloading this page...', 'FYI', { timeOut: '3500' })
                toastr.success(result.Message, 'Success');

                $("#contactNumber").val(result.contactnum);
                $("#streetaddress").val(result.Address);
                $("#city").val(result.City);
                $("#secondaryemail").val(result.secondaryemail);
                $("#recoveryemail").val(result.recoveryemail)
                $("#stateinput").val(result.state);
                $("#zipcodeinput").val(result.zip);

                setTimeout(function () { location.reload(true) }, 3500);
            }
            else {
                toastr.error(result.Message, 'Error');
            }
        });
    }


    function restpin() {
        if (NoochId == '') {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        var url = "../Member/ResetPin";
        var data = {};
        data.noochId = NoochId;

        $.post(url, data, function (result) {
            if (result.IsSuccess == true) {
                toastr.success(result.Message, 'Succcess');
                $("#pinNumber").html(result.Pin);
            }
            else {
                toastr.error(result.Message, 'Error');
            }
        });
    }


    // Manually set bank account's status to 'Verified'
    function verifyBankAccount() {
        var accountId = $('#bnkIdHidden').val();

        if (accountId == '') {
            toastr.error('No bank account was selected...', 'Error');
            return;
        }

        var url = "../Member/VerifyAccount";
        var data = {};
        data.accountId = accountId;

        $.post(url, data, function (result) {
            console.log(result);

            if (result.IsSuccess == true) {
                toastr.success(result.Message, 'Succcess');

                $('#bankAccountStatusDiv').html('');
                $('#bankAccountStatusDiv').html("<span class='text-success' style='display: inline-block'>Verified</span>");
            }
            else {
                toastr.error(result.Message, 'Error');
            }
        });
    }


    // Manually set bank account's status to 'Pending Review'
    function unVerifyBankAccount() {
        var accountId = $('#bnkIdHidden').val();
        if (accountId == '') {
            toastr.error('No bank account was selected...', 'Error');
            return;
        }

        var url = "../Member/UnVerifyAccount";
        var data = {};
        data.accountId = accountId;

        $.post(url, data, function (result) {
            if (result.IsSuccess == true) {
                toastr.success(result.Message, 'Succcess');

                $('#bankAccountStatusDiv').html('');
                $('#bankAccountStatusDiv').html("<span class='text-warning' style='display: inline-block'>Pending Review</span>");
            }
            else {
                toastr.error(result.Message, 'Error');
            }
        });
    }


    // Open AdminNotes Modal
    function AdminNoteAboutUserModalPopup() {
        if (NoochId == '') {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        var url = "../Member/AdminNoteAboutUserModalPopup";
        var data = {};
        data.noochId = NoochId;

        $.post(url, data, function (result) {
            if (result.IsSuccess == true) {
                $('#Modal-AdminNotes').modal();
                $("#AmdinNotesAboutUser").val(result.AdminNote);
            }
            else {
                toastr.error(result.Message, result.Message);
            }
        });
    }


    // Provide Info About the User In ModalPopup
    function SaveAdminNoteForUser() {
        if (NoochId == '') {
            toastr.error('No NoochId was selected...', 'Error');
            return;
        }

        if ($("#AmdinNotesAboutUser").val() == " ") {
            $('#Modal-AdminNotes').modal('hide');
        }

        var url = "../Member/SaveAdminNoteForUser";
        var data = {};
        data.noochid = NoochId;
        data.AdminNote = $("#AmdinNotesAboutUser").val();
        $.post(url, data, function (result) {
            if (result == "Success") {
                toastr.success(result.Message, 'Admin Note edited successfully.');
                $('#Modal-AdminNotes').modal('hide');
            }
            else {
                toastr.error(result.Message, 'Error');
                $('#Modal-AdminNotes').modal('hide');
            }
        });
    }


    return {
        ApplyChoosenOperation: applyOperation,
        EditMember: editdetails,
        ResetPin: restpin,
        OpenPopupForAdminNote: AdminNoteAboutUserModalPopup,
        AdminNoteForUser: SaveAdminNoteForUser,
        VerifyBankAccount: verifyBankAccount,
        UnVerifyBankAccount: unVerifyBankAccount
    };
}();