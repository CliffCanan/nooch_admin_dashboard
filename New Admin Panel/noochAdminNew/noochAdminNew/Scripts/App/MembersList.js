var allvalues = [];
$(document).ready(function () {

    $('#selecctall').click(function (event) {
        if (this.checked)
        { // check select status
            $('.checkbox1').each(function () { //loop through each checkbox
                this.checked = true;  //select all checkboxes with class "checkbox1"     
                ArrayOperations.RemoveGivenItemFromGivenArray(allvalues, $(this).val());
                allvalues.push($(this).val());
            });
        }
        else
        {
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
    if (idPassed.checked == 1)
    {
        allvalues.push($(idPassed).val());
    }
}


var Member = function () {

    function applyOperation() {

        if ($("#opMaster").val() == 0)
        {
            toastr.error('Select an action first!', 'Error');
            return;
        }

        if (allvalues.length == 0)
        {
            toastr.error('No Members were selected. Please select at least 1.', 'Error');
            return;
        }

        // making csv
        var csvids = '';
        for (var i = 0; i < allvalues.length; i++)
        {
            csvids = csvids + allvalues[i] + ',';
        }

        console.log(csvids);

        if (csvids.length > 0 && $("#opMaster").val() != 0)
        {
            var url = "../Member/ApplyOperation";
            var data = {};
            data.operation = $("#opMaster").val();
            data.noochIds = csvids;
            $.post(url, data, function (result) {
                console.log(result);
                if (result.IsSuccess == true)
                {
                    console.log(result.Message);
                    console.log(result.MemberOperationsOuterClass);

                    // iterating through all innerclass objects

                    $.each(result.MemberOperationsOuterClass, function (key, value) {
                        if (value.IsSuccess == true)
                        {
                            toastr.success(value.Message, 'Success');
                        } else
                        {
                            toastr.error(value.Message, 'Error');
                        }
                    });

                    location.reload(true);
                }
                else
                {
                    toastr.error('Server failed to make the requested update!', 'Error');
                    console.log('Error occured: ' + result.Message);
                }
            });
        }
    }

    return {
        ApplyChoosenOperation: applyOperation
    };
}();