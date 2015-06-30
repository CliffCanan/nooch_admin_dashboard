var allvalues = [];

$(document).ready(function () {

    $('#selecctAllForDisputeStatus').click(function (event) {
        if (this.checked) { // check select status
            $('.checkbox1').each(function () { //loop through each checkbox
                this.checked = true;  //select all checkboxes with class "checkbox1"     
                ArrayOperations.RemoveGivenItemFromGivenArray(allvalues, $(this).val());
                allvalues.push($(this).val());
            });
        }
        else {
            $('.checkbox1').each(function () { //loop through each checkbox
                this.checked = false; //deselect all checkboxes with class "checkbox1"        
                ArrayOperations.RemoveGivenItemFromGivenArray(allvalues, $(this).val());
            });
        }

        console.log(allvalues.length);
    });

});


function chkboxClick(idPassed) {
    ArrayOperations.RemoveGivenItemFromGivenArray(allvalues, $(idPassed).val());
    if (idPassed.checked == 1) {
        allvalues.push($(idPassed).val());
    }
}


var DisputeDetailsList = function () {

    function applyOperation() {

        if ($("#opDisputeMaster").val() == 0) {
            toastr.error('Select an action first!', 'Error');
            return;
        }

        if (allvalues.length == 0) {
            toastr.error('No disputes were selected. Please select at least 1.', 'Error');
            return;
        }

        // making csv
        var csvids = '';
        for (var i = 0; i < allvalues.length; i++) {
            csvids = csvids + allvalues[i] + ',';
        }

        console.log("CSV ID's: " + csvids);

        if (csvids.length > 0 && $("#opDisputeMaster").val() != 0) {
            var url = "../Disputes/ApplyDisputeOperation";
            var data = {};
            data.operation = $("#opDisputeMaster").val();
            AdminNotes: "";
            data.DisputeId = csvids;
            $.post(url, data, function (results) {
                var result = jQuery.parseJSON(results)
                if (result.IsSuccess == true) {

                    console.log(result.Message);
                    console.log(result.MemberDisputeSUbListClass);

                    $.each(result.MemberDisputeSUbListClass, function (key, value) {
                        if (value.IsSuccess == true) {
                            //console.log('Success: ' + value.DisputeId);
                            toastr.success('Dispute updated and users notified.', 'Success');
                        } else {
                            toastr.error('Error occured ;..( ', 'Error');
                            console.log('Error occured ;..(  DisputeID: ' + value.DisputeId + '.  Result.Message is: ' + result.Message);
                        }
                    });

                    $('#selectedDispute').modal('hide');
                    setTimeout(function(){location.reload(true)},2500);
                }
                else {
                    toastr.error(result.Message, 'Error');
                    console.log('Error occured: ' + result.Message);
                }
            });
        }
    }

    return {
        ApplyChoosenDisputeOperation: applyOperation
    };
}();