var Referrals = function () {

    function create() {

        var levelChoosen = $("input:radio[name=adminLevel]:checked").val();

        var url = "../Referrals/CreateNewReferralCode";
        var data = {};
        data.newCode = $("#newCode").val();
        data.allowedUses = $("#allowedUses").val();
        data.newCodeNotes = $("#newCodeNotes").val();

        if ($("#newCode").val() == "") {
            toastr.error('Code required', 'Empty string');
            return;
        }

        if ($("#allowedUses").val() == "") {
            toastr.error('Number of users allowed is misssing.', 'Empty string');
            return;
        }

        if ($("#newCodeNotes").val() == "") {
            toastr.error('Notes required.', 'Empty string');
            return;
        }


        $.post(url, data, function (result) {
            if (result.IsSuccess == true) {
                console.log(result.Message);
                console.log(result.MemberOperationsOuterClass);

                // iterating through all innerclass objects

                toastr.success(result.Message, 'Success');

                $("#newCode").val('');
                $("#allowedUses").val('');
                $("#newCodeNotes").val('');
                location.reload(true);
            }
            else {
                $("#modalshowbutton").trigger("click");   
                toastr.error(result.Message, 'Error');
            }
        });
    }

    return {
        Create: create
    };
}();