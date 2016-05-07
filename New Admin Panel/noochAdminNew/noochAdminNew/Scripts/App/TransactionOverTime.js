var NoochId = '';

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

    Member.GetTransactionVolumeOverTimeData("daily");
});


function showDate()
{
    $('.showDate').removeClass('hide');
}

function generateBar(data1, label, ticks)
{
    var barData = new Array();

    barData.push({
        data: data1,
        label: label,
        highlightColor: '#50882f',
        bars: {
            align: 'center',
            show: true,
            barWidth: 0.35,
            order: 1,
            lineWidth: 0,
            fillColor: '#72bf44'
        }
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
                show: true,
                container: '.flc-bar',
                backgroundOpacity: 0.5,
                backgroundColor: "white",
                noColumns: 3,
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

                $(".flot-tooltip").html("$" + Math.round(y)).css({ top: item.pageY + 5, left: item.pageX + 5 }).show();
            }
            else {
                $(".flot-tooltip").hide();
            }
        });

        $("<div class='flot-tooltip' class='chart-tooltip'></div>").appendTo("body");
    }
}


var Member = function ()
{
    function GetTransactionVolumeOverTimeData(type)
    {
        var fromDate = '';
        var toDate = '';
        if (type == 'r')
            type = $('.legendLabel').html();
        if (type == 'dateRange') {
            fromDate = $('#startDate').val();
            toDate = $('#endDate').val();
        }


        $('#headerTransactionOverTime').html('Transaction Volume Over Time  | <span style="font-size:90%; opacity:.6;">' + type + '</span>');

        var status = ($('input[name="status"]:checked').val());
        var data1 = [];
        var ticks = [];
        var url = "GetTransactionVolumeOverTimeData?recordType=" + type + "&status=" + status + "&fromDate=" + fromDate + "&toDate=" + toDate;
        var data = {};

        var statusString = "Completed";
        if (status == 1)
            statusString = "Cancelled/Rejected"
        else if (status == 2)
            statusString = "Pending"

        if (type == 'dateRange') {
            if ($('#frmTarget').parsley().validate()) {
                $.post(url, data, function (result)
                {
                    console.log(result.Duration.durationdata);

                    if (result.IsSuccess) {
                        $('#myModal').modal('hide');

                        $(result.externalData).each(function (index)
                        {
                            data1.push(this.internalData);
                        });

                        $(result.Duration).each(function (index)
                        {
                            ticks.push(this.durationData);
                        });

                        console.log(ticks);
                        generateBar(data1, type, ticks);

                        toastr.success('<strong>' + statusString + '</strong> transactions loaded.', 'Success!');
                    }
                    else {
                        toastr.error(result.Message, 'Error');
                    }
                });
            }
        }
        else if (type != null && type != "") {
            $.post(url, data, function (result)
            {
                console.log(result.Duration.durationdata);

                if (result.IsSuccess) {
                    $('#myModal').modal('hide');

                    $(result.externalData).each(function (index)
                    {
                        data1.push(this.internalData);
                    });

                    $(result.Duration).each(function (index)
                    {
                        ticks.push(this.durationData);
                    });

                    console.log(ticks);

                    generateBar(data1, type, ticks);

                    toastr.success('<strong>' + statusString + '</strong> transactions loaded - <span style="text-transform:uppercase; font-weight:bold;">' + type + '</span>', 'Success!');
                }
                else {
                    toastr.error(result.Message, 'Error');
                }
            });
        }
    }


    return {
        GetTransactionVolumeOverTimeData: GetTransactionVolumeOverTimeData,
        showDate: showDate
    };
}();