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
            if (result.IsSuccess == true)
            {
                //console.log(result.Message);
                toastr.success('Logging in...', 'Success');
                window.location.replace($('#dashUrl').val());
            }
            else
            {
                toastr.error('Invalid username or password!', 'Error');
                console.log(result.Message);
            }
            $.unblockUI();
        });
    }

    return {
        GoIn: doLogin
    };
}();