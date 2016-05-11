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

$(document).ready(function ()
{

    $("#TransactionMaster").trigger("click");
    $('#DashboardMenu').addClass('active');

    $('#AllTrans').dataTable({
        responsive: true,
        "order": [3, "desc"],
        "columnDefs": [
               { "orderable": false, "targets": [0] }
        ]

    });

    // Pie Chart
    var doughnutData = [
    /*{
        value: 300,
        color: "#a3e1d4",
        highlight: "#1ab394",
        label: "Downloads"
    },*/
    {
        value: 874, //Number(@dd.TotalVerifiedEmailUsers) / Number(@dd.TotalActiveUsers),  // No. of Users w/ Status of "Registered" + "Active" + "Suspended" + "Temporary_blocked"
        color: "#dedede",
        highlight: "#1ab394",
        label: "Signups"
    },
    {
        value: 512, //Number(@dd.TotalVerifiedPhoneUsers) / Number(@dd.TotalActiveUsers),  // No. of Users w/ BOTH Verified Phone AND Status of "Active"
        color: "rgba(63,171,225,1)",
        highlight: "rgba(63,171,225,.7)",
        label: "Verified"
    },
    {
        value: 298, //Number(@dd.TotalActiveBankAccountUsers) / Number(@dd.TotalActiveUsers),
        color: "rgba(114,191,68,1)",
        highlight: "rgba(114,191,68,.7)",
        label: "Bank Added"
    }
    ];

    var doughnutOptions = {
        segmentShowStroke: true,
        segmentStrokeColor: "#fff",
        segmentStrokeWidth: 1,
        percentageInnerCutout: 25, // This is 0 for Pie charts
        animationSteps: 150,
        animationEasing: "easeOutBounce",
        animateRotate: true,
        animateScale: true,
        responsive: true,
        scaleShowLabels: true,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>"
    };

    var ctx = document.getElementById("doughnutChart").getContext("2d");
    var myNewChart = new Chart(ctx).Doughnut(doughnutData, doughnutOptions);


    var val = $("input:radio[name=options]").val();
    DashboardDetailsOperation(val);

    //Date Format changes
    function getISODateTime(d, includeTime)
    {
        // padding function
        var s = function (a, b) { return (1e15 + a + "").slice(-b) };

        // default date parameter
        if (typeof d === 'undefined') {
            d = new Date();
        };


        var amPm = "AM";
        if (d.getHours() > 12) {
            amPm = "PM";
        }

        // return ISO datetime
        if (includeTime) {
            return month[d.getMonth()] + ' ' +
                 s(d.getDate(), 2) + ', ' +
                 d.getFullYear() + '.  ' +
                 Math.abs(s(d.getHours(), 2) - 12) + ':' +
                 s(d.getMinutes(), 2) + amPm;
        }
        else {
            return month[d.getMonth()] + ' ' +
           s(d.getDate(), 2) + ', ' +
           d.getFullYear() + '';
        }
    }


    //Click Event For Label
    $("#dash_TransPanel2 .btn-group label").click(function ()
    {
        var c = $(this).attr("For");
        var val = $('#' + c + '').val();

        DashboardDetailsOperation(val);
    });


    dateVar = new Date();

    // Tooltip for showing dates
    $('.dateHoverToday').tooltip({
        title: getISODateTime(dateVar, false)
    })
    $('.dateHoverThisMonth').tooltip({
        title: month[dateVar.getMonth()] + " " + dateVar.getFullYear()
    })

    // Click the 'Total Users' box - send to Members List page
    $('#totalUsersWidg').click(function ()
    {
        var url = window.location.protocol + "//" + window.location.host + "/noochnewadmin/Member/ListAll";
        window.location.href = url;
    });

    // Show the  Live Transaction On Dashboard  
    function DashboardDetailsOperation(operation)
    {
        if (operation == '0' || operation == '1' || operation == '2') {
            var url = "../Admin/ShowLiveTransactionsOnDashBoard";
            var data = {};
            data.operation = operation;

            $.post(url, data, function (result)
            {
                if (result.IsSuccess == true) {
                    console.log(result);

                    var trHTML = '';
                    $("#TBOdy tr").remove();

                    $.each(result.RecentLiveTransaction, function (i, item)
                    {
                        //DisputeStatus
                        /*var disputestatus;
                        if (item.DisputeStatus == null || item.DisputeStatus == "") {
                            disputestatus = "No";
                        }
                        else {
                            disputestatus = "Yes";
                        }*/
                        //TransactionTime
                        var TransactionTime = getISODateTime(new Date(parseInt((item.TransDateTime.substr(6)))), false);

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

                        if (item.SenderId != "") {
                            trHTML += '<td><a href="../Member/Detail?NoochId=' + item.SenderId + '">' + item.SenderUserName + '</a></td>';
                        }
                        else {
                            trHTML += '<td>' + item.SenderUserName + '</td>';
                        }
                        if (item.RecepientId != "") {
                            trHTML += '<td><a href="../Member/Detail?NoochId=' + item.RecepientId + '">' + item.RecepientUserName + '</a></td>';
                        }
                        else {
                            trHTML += '<td>' + item.RecepientUserName + '</td>';
                        }

                        trHTML += '<td>$ ' + Number(item.Amount) + '</td>' +
                                  //'<td>' + disputestatus + '</td>' +
                                  '<td><a href="#" OnClick="showLocationModal(' + item.Latitude + ',' + item.Longitude + ',' + "'" + GeoLocation + "'" + ')" class="btn btn-link" data-loctext="' + GeoLocation + '">' + GeoLocation + '</a></td> <td>'+item.SynapseStatus+' </td></tr>';

                        $("#TBOdy").append(trHTML);
                    });
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
