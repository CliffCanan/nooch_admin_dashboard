var NoochId = '';
var dateType = 'daily';

$(document).ready(function ()
{


    window.onload = function ()
    {
        new JsDatePick({
            useMode: 2,
            target: "startDate",
            dateFormat: "%d-%M-%Y"
        });
        new JsDatePick({
            useMode: 2,
            target: "endDate",
            dateFormat: "%d-%M-%Y"
        });
    };

    $("#GraphMenuExpander").trigger("click");
    $('#UserOverTimeMenu').addClass('active');

    Member.getUserOverTime("daily");
});


function showDate()
{
    $('.showDate').removeClass('hide');
}

function generateBar(data1, ticks)
{
    var barData = new Array();

    barData.push({
        data: data1,
        label: dateType,
        highlightColor: '#1d83b7',
        bars: {
            align: 'center',
            show: true,
            barWidth: 0.35,
            order: 1,
            lineWidth: 0,
            fillColor: '#3fabe1'
        },
    });

    // Create the chart
    if ($('#bar-chart')[0]) {
        $.plot($("#bar-chart"), barData, {
            grid: {
                borderWidth: 1,
                borderColor: '#ddd',
                show: true,
                hoverable: true,
                clickable: false
            },

            yaxis: {
                tickColor: '#ddd',
                tickDecimals: 0,
                font: {
                    lineHeight: 18,
                    size: 14,
                    style: "normal",
                    color: "#444",
                },
                shadowSize: 1
            },

            xaxis: {
                ticks: ticks,
                tickColor: '#fff',
                tickDecimals: 0,
                font: {
                    lineHeight: 18,
                    size: 14,
                    style: "normal",
                    color: "#444"
                },
                shadowSize: 0,
            },

            legend: {
                show: false,
                container: '.flc-bar',
            }
        });
    }

    /* Tooltips for Flot Charts */
    if ($(".flot-chart")[0]) {
        $(".flot-chart").bind("plothover", function (event, pos, item)
        {
            if (item) {
                var x = item.datapoint[0].toFixed(2),
                    y = item.datapoint[1].toFixed(2);

                $(".flot-tooltip").html(Math.round(y) + " Users").css({ top: item.pageY + 5, left: item.pageX + 5 }).show();
            }
            else {
                $(".flot-tooltip").hide();
            }
        });

        $("<div class='flot-tooltip' class='chart-tooltip'></div>").appendTo("body");
    }
}

// When selecting a Date Type from the Actions Dropdown Menu
function updateDateType(newDateType)
{
  
     
    // Set global var 'dateType'
    dateType = newDateType;

    Member.getUserOverTime(dateType);
}

// On Clicking the 'Apply' Button
$('#userFilter').submit(function (event)
{
  
    event.preventDefault();

    Member.getUserOverTime(dateType);
});

var Member = function ()
{
    function getUserOverTime(dateType)
    {
        console.log(dateType);
        $(document).ajaxStart($.blockUI).ajaxStop($.unblockUI);
        var fromDate = '';
        var toDate = '';

        if (dateType == 'dateRange') {
            fromDate = $('#startDate').val();
            toDate = $('#endDate').val();
        }

        var userType = ($('input[name="userType"]:checked').val());
        var userStatus = ($('input[name="userStatus"]:checked').val());

        $('#headerUserOverTime').html('Users Over Time  | <span style="font-size:90%; opacity:.7;">' + dateType + '</span>');

        var data1 = [];
        var ticks = [];
        var url = "GetUsersOverTimeOverTimeData?dateType=" + dateType + "&userType=" + userType + "&userStatus=" + userStatus + "&fromDate=" + fromDate + "&toDate=" + toDate;

        console.log(url);

        var data = {};

        var typeString = "&nbsp;<em>ALL</em>";
        if (userType == 1)
            typeString = "&nbsp;<em>Registered Only</em>"
        else if (userType == 2)
            typeString = "&nbsp;<em>Non-Registered Only</em>"

        var statusString = "&nbsp;<em>ALL</em>";
        if (userStatus == 1)
            statusString = "&nbsp;<em>Active Only</em>"
        else if (userStatus == 2)
            statusString = "&nbsp;<em>Deleted Only</em>"

        if (dateType == 'dateRange') {
            if ($('#frmTarget').parsley().validate()) {
                $('#customDateModal').modal('hide');
                $.post(url, data, function (result)
                {
                    console.log(result);
                   
                    if (result.IsSuccess) {
                        $('#customDateModal').modal('hide');

                        $(result.externalData).each(function (index)
                        {
                            data1.push(this.internalData);
                        });

                        $(result.Duration).each(function (index)
                        {
                            ticks.push(this.durationData);
                        });
                        console.log(ticks);

                        generateBar(data1, ticks);

                        toastr.success('<strong>' + statusString + '</strong> users loaded.', 'Success!');
                    }
                    else {
                        toastr.error(result.Message, 'Error');
                    }
                });
            }
        }
        else {
            $.post(url, data, function (result)
            {
                console.log(result);

                if (result.IsSuccess) {

                    $(result.externalData).each(function (index)
                    {
                        data1.push(this.internalData);
                    });

                    $(result.Duration).each(function (index)
                    {
                        ticks.push(this.durationData);
                    });

                    console.log(ticks);

                    generateBar(data1, ticks);

                    toastr.success('<strong>Type:</strong> ' + typeString + '<br/><strong>Status:</strong> ' + statusString, 'Success!');
                }
                else {
                    toastr.error('Unable to generate chart data from server.', 'Error');
                }
            });
        }
    }


    return {
        getUserOverTime: getUserOverTime,
        showDate: showDate
    };
}();