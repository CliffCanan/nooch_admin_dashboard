var DisputeDetails = function () {

    function getISODateTime(d) {
        // padding function
        var s = function (a, b) { return (1e15 + a + "").slice(-b) };

        // default date parameter
        if (typeof d === 'undefined') {
            d = new Date();
        };

        var month = new Array();
        month[0] = "January";
        month[1] = "February";
        month[2] = "March";
        month[3] = "April";
        month[4] = "May";
        month[5] = "June";
        month[6] = "July";
        month[7] = "August";
        month[8] = "September";
        month[9] = "October";
        month[10] = "November";
        month[11] = "December";
        // return ISO datetime
        var amPm = "AM";
        if (d.getHours() > 12) {
            amPm = "PM";
        }
        console.log(amPm);
        return month[d.getMonth()] + ' ' +
            s(d.getDate(), 2) + ', ' +
		    d.getFullYear() + '.  ' +
            Math.abs(s(d.getHours(), 2) - 12) + ':' +
            s(d.getMinutes(), 2) + amPm;
    }

    ///// Show the Popup Of Of disputId Details
    function DisputePopUpDetail(DisputeTracKiD) {
        $("#AmdinNotesProvided").val("");
        var url = "../Disputes/Details";
        var data = {};
        data.id = DisputeTracKiD;
        $.post(url, data, function (result) {
            if (result.IsResultSucess == true) {
                console.log(result);

                $("#DisputeId").val(DisputeTracKiD);
                $("#AmountDispute").text("$" + result.Amount.substring(0, result.Amount.indexOf(".") + 3));
                $("#SenderName").text(result.SenderId);
                $("#RecipName").text(result.ReceiptId);
                $("#SenderImg").attr("src", result.SenderImg);
                $("#RecipImg").attr("src", result.ReceiptImg);
                var date = new Date(parseInt((result.TransactionDate.substr(6))));
                $("#TransactinDate").text("Transaction Date: " + getISODateTime(date));
                var DisputeDat = new Date(parseInt((result.DisputeDate.substr(6))));
                $("#DisputeDate").text("Disputed:  " + getISODateTime(DisputeDat));
                if (result.TransactionLocation != null)
                {
                    $("#locationLi").show();
                    $("#TransactionLocation").text(result.Transactionlocation.trim());
                }
                else {
                    $("#locationLi").hide();
                }
                $("#TransactionFor").text(result.TransactionFor);

                $('#selectedDispute').modal();
            }
            else {
                toastr.error("Failed to get that Dispute's details from the server!", "Error");
            }
        });
    }


    ///// Save Changes For DisputeTransaction
    function SaveDiputedStatusOnTransaction() {

        if ($('#SelectDisputeId').attr('data-dispStatus') == 'Under Review' &&
            $("#DisputeStatus").val() == 1)
        {
            alert("Ain't been no change!!  Make sure to update this Dispute's status before saving!");
            return;
        }

        var url = "../Disputes/ApplyDisputeOperation";
        var data = {
            DisputeId: $("#DisputeId").val(),
            AdminNotes: $("#AmdinNotesProvided").val(),
            operation: $("#DisputeStatus").val()
        };
        $.post(url, data, function (results) {
            var result = jQuery.parseJSON(results)
            if (result.IsSuccess == true) {

                console.log(result.Message);
                console.log(result.MemberDisputeSUbListClass);

                $.each(result.MemberDisputeSUbListClass, function (key, value) {
                    if (value.IsSuccess == true) {
                        //console.log('Success: ' + value.DisputeId);
                        toastr.success('Dispute updated and both users notified.', 'Success');
                    } else {
                        toastr.error('Error occured ;..( ', 'Error');
                        console.log('Error occured ;..(  DisputeID: ' + value.DisputeId + '.  Result.Message is: ' + result.Message);
                    }
                });

                $('#selectedDispute').modal('hide');
                setTimeout(function(){location.reload(true)},2500);
            }
            else {
                location.reload(true);
                toastr.error(result.Message, 'Error');
                console.log('Error occured: ' + result.Message);
            }
        });
    }

    return {
        DisputePopUp: DisputePopUpDetail,
        SaveDisputeStatus: SaveDiputedStatusOnTransaction
    };
}();