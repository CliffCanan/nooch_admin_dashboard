var Admin = function () {

    function save() {

        var levelChoosen = $("input:radio[name=adminLevel]:checked").val();

        var url = "../Admin/CreateAndSaveNewAdminUser";
        var data = {};
        data.userName = $("#userNameText").val();
        data.emailAddress = $("#emailText").val();
        data.firstName = $("#firstNameText").val();
        data.lastName = $("#lastNameText").val();
        data.level = levelChoosen;

        if ($("#userNameText").val() == ""){
            toastr.warning('Please enter a username.', 'Username Required');
            return;
        }

        if ($("#emailText").val()== ""){
            toastr.warning('Please enter an Email Address.', 'Email Address Required');
            return;
        }

        if ($("#firstNameText").val() == "") {
            toastr.warning('Please enter a First Name.', 'First Name Required');
            return;
        }

        if ($("#lastNameText").val()== "") {
            toastr.warning('Please enter a Last Name.', 'Last Name Required');
            return;
        }

        if (levelChoosen == "" || levelChoosen==null) {
            toastr.warning('Please choose an Admin Level.', 'Admin Level Not Selected');
            return;
        }

        $.post(url, data, function (result) {
            if (result.IsSuccess == true) {
                console.log(result.Message);
                console.log(result.MemberOperationsOuterClass);

                // iterating through all innerclass objects

                toastr.success(result.Message, 'Success');

                $("#userNameText").val('');
				$("#emailText").val('');
                $("#firstNameText").val('');
                $("#lastNameText").val('');
                $("input:radio[name=adminLevel]").removeAttr("checked");
				
				setTimeout(function () {
					window.location.href = "SearchAdmin";
					}, 2000); // will redirect after 2 sec
            } 
			else {
                toastr.error(result.Message, 'Oh No! Error');
            }
        });
    }

    // to add fund to members account
    function creditfund() {

        var userNameText = $("#userNameText").val();
        var amoutinput = $("#amoutinput").val();
        var commentsinput = $("#commentsinput").val();
        var pinInput = $("#pinInput").val();

        if (userNameText == "") {
            toastr.warning('Please enter a username or NoochId.', 'Recipient Not Identified');
			document.getElementById("userNameText").focus();
            return;
        }

        if (amoutinput == "") {
            toastr.warning('Please enter an amount!', 'Amount Needed');
			document.getElementById("amoutinput").focus();
            return;
        }

        if (commentsinput == "") {
            toastr.warning('Please enter a memo.', 'Memo Required');
			document.getElementById("commentsinput").focus();
            return;
        }

        if (pinInput == "") {
            toastr.warning('Please enter the Admin PIN.','PIN Number Required');
			document.getElementById("pinInput").focus();
            return;
        }

        var url = "../Admin/CreditFundToMemberPost";
        var data = {};
        data.transferfundto = userNameText;
        data.transferAmount = amoutinput;
        data.transferNotes = commentsinput;
        data.adminPin = pinInput;


        $.post(url, data, function (result) {
            if (result.IsSuccess == true) {
                console.log(result.Message);
                console.log(result.MemberOperationsOuterClass);

                // iterating through all innerclass objects

                toastr.success(result.Message, 'Success');

                $("#userNameText").val('');
                $("#amoutinput").val('');
                $("#commentsinput").val('');
                $("#pinInput").val('');
            }
            else {
                toastr.error(result.Message, 'Oh No! Error');
            }
        });
    }

    return {
        Create: save,
        AddFund : creditfund
    };
}();