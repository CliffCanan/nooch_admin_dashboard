// Show the  Live Transaction On Dashboard   
$(document).ready(function () {
    var val = $("input:radio[name=options]").val();
    DashboardDetailsOperation(val);

    //Date Format changes
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

        return month[d.getMonth()] + ' ' +
            s(d.getDate(), 2) + ', ' +
		    d.getFullYear() + '.  ' +
            Math.abs(s(d.getHours(), 2) - 12) + ':' +
            s(d.getMinutes(), 2) + amPm;
    }


    //Click Event For Label
    $("#dash_TransPanel2 .btn-group label").click(function () {
        alert('came in click');
        var c = $(this).attr("For");
        var val = $('#' + c + '').val();

        DashboardDetailsOperation(val);
    });

    
    function DashboardDetailsOperation(operation) {

        alert(operation);

        if (operation == '0' || operation == '1' || operation == '2') {

            var url = "../Admin/ShowLiveTransactionsOnDashBoard";
            var data = {};
            data.operation = operation;
            $.post(url, data, function (result) {
                if (result.IsSuccess == true) {
                    console.log(result);

                    var trHTML = '';
                    $("#TBOdy tr").remove();

                    $.each(result.RecentLiveTransaction, function (i, item) {
                        //DisputeStatus
                        var disputestatus;
                        if (item.DisputeStatus == null || item.DisputeStatus == "") {
                            disputestatus = "No";
                        }
                        else {
                            disputestatus = "Yes";
                        }
                        //TransactionTime
                        var TransactionTime = getISODateTime(new Date(parseInt((item.TransDateTime.substr(6)))));

                        //GeoLocation
                        var GeoLocation;
                        if (item.GeoStateCityLocation == null || $.trim(item.GeoStateCityLocation) == ",") {
                            GeoLocation = "";
                        }
                        else {
                            GeoLocation = $.trim(item.GeoStateCityLocation);
                        }

                        trHTML = '<tr><td><small>' + item.TransID + '</small></td>' +
                                 '<td>' + item.TransactionType + '</td>' +
                                 '<td class="' + item.TransactionStatus + '">' + item.TransactionStatus + '</td>' +
                                 '<td>' + TransactionTime + '</td>';

                        if (item.SenderId != "")
                        {
                            trHTML += '<td><a href="../Member/Detail?NoochId=' + item.SenderId + '">' + item.SenderUserName + '</a></td>';
                        }
                        else
                        {
                            trHTML += '<td>' + item.SenderUserName + '</td>';
                        }
                        if (item.RecepientId != "")
                        {
                            trHTML += '<td><a href="../Member/Detail?NoochId=' + item.RecepientId + '">' + item.RecepientUserName + '</a></td>';
                        }
                        else
                        {
                            trHTML += '<td>' + item.RecepientUserName + '</td>';
                        }

                        trHTML += '<td>$ ' + Number(item.Amount) + '</td>' +
                                  '<td>' + disputestatus + '</td>' +
                                  '<td><a href="#" OnClick="showLocationModal(' + item.Latitude + ',' + item.Longitude + ',' + "'" + GeoLocation + "'" + ')" class="btn btn-link" data-loctext="' + GeoLocation + '">' + GeoLocation + '</a></td></tr>';

                        $("#TBOdy").append(trHTML);
                    });

                    //$("#TBOdy").append(trHTML);
                }

                else {
                    toastr.error('Server was unable to return the request transactions!', 'Error');
                }
            });
        }

        else {
            toastr.error('Invalid selection! Please try again.', 'Error');
        }
    }
});
