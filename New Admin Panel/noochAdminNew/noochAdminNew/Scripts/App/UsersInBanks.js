var NoochId = '';


$(document).ready(function () {
    
    Member.getUsersInBanks();




});

function generateBar(data1, ticks) {

    var barData = new Array();


    barData.push({
        data: data1,
        label: "Users",
        bars: {
            show: true,
            barWidth: 0.08,
            order: 1,
            lineWidth: 0,
            fillColor: '#EDC240'
        }
    });



    /* Let's create the chart */
    if ($('#bar-chart')[0]) {
        $.plot($("#bar-chart"), barData, {
            grid: {
                borderWidth: 1,
                borderColor: '#eee',
                show: true,
                hoverable: true,
                clickable: true
            },

            yaxis: {
                tickColor: '#eee',
                tickDecimals: 0,
                font: {
                    lineHeight: 13,
                    style: "normal",
                    color: "#9f9f9f",
                },
                shadowSize: 0
            },

            xaxis: {
                ticks: ticks,
                tickColor: '#fff',
                tickDecimals: 0,
                font: {
                    lineHeight: 13,
                    style: "normal",
                    color: "#9f9f9f"
                },
                

                shadowSize: 0,
            },

            legend: {
                container: '.flc-bar',
                backgroundOpacity: 0.5,
                noColumns: 0,
                backgroundColor: "white",
                lineWidth: 0
            }
        });
    }

    /* Tooltips for Flot Charts */

    if ($(".flot-chart")[0]) {
        $(".flot-chart").bind("plothover", function (event, pos, item) {
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

var Member = function () {
    function getUsersInBanks() {
        

     
        var data1 = [];
        var ticks = [];
        var url = "getUsersInBanks";
        var data = {};
        $.post(url, data, function (result) {
            
            
            if (result.IsSuccess) {
               

                $(result.externalData).each(function (index) {

                    data1.push(this.internalData);
                     
                });
                $(result.Duration).each(function (index) {

                    ticks.push(this.durationData);
                });
                console.log(ticks);
                generateBar(data1, ticks);
                 


                toastr.success(result.Message, 'Sucess!');


            }
            else {

                toastr.error(result.Message, 'Error');

            }
        });


    }





    return {
        getUsersInBanks: getUsersInBanks


    };
}();

 