var NoochId = '';

$(document).ready(function ()
{
    $("#GraphMenuExpander").trigger("click");
    $('#UserInBanksMenu').addClass('active');

    Member.getUsersInBanks();
});

function generateBar(data1, ticks)
{
    var barData = new Array();

    barData.push({
        data: data1,
        label: "Users",
        highlightColor: '#1d83b7',
        bars: {
            align: 'center',
            show: true,
            barWidth: 0.35,
            order: 1,
            lineWidth: 0,
            fillColor: '#3fabe1'
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

                $(".flot-tooltip").html(Math.round(y) + " Users").css({ top: item.pageY + 5, left: item.pageX + 5 }).show();
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
    function getUsersInBanks()
    {
        var data1 = [];
        var ticks = [];
        var url = "getUsersInBanks";
        var data = {};

        $.post(url, data, function (result)
        {
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

                toastr.success(result.Message, 'Sucess!');
            }
            else {
                toastr.error('Unable to generate bank data from server.', 'Error');
                toastr.error(result.Message, 'Error');
            }
        });
    }

    return {
        getUsersInBanks: getUsersInBanks
    };
}();
