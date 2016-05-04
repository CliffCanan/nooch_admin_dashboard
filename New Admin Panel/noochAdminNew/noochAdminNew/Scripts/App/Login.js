$(document).ready(function () {
	$('#Username').focus();
});

var User = function () {

    function doLogin() {
        var url = $('#urlToHit').val();
        var data = {};
        data.UserName = $("#Username").val();
        data.Password = $("#Password").val();

        $.post(url, data, function (result) {
            if (result.IsSuccess == true) {
                //console.log(result.Message);
                window.location.replace("../Admin/Dashboard");
            }
			else {
                toastr.error('Invalid username or password!', 'Error');
                console.log('Login Attempt, Error occured');
                console.log(result.Message);
            } 
        });
    }

    return {
        GoIn: doLogin        
    };
}();