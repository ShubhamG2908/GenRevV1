// charts.js

define(function () {

    /*****************************************
    
    ******************************************/

    var Interface = {
        
        GetBase: function () { return charts.getBase(); },

        GetTooltip: function (data, formatFunc) { return charts.getTooltip(data, formatFunc); }

    }   // end Charts

    var charts = {

        colors: [],

        getBase: function () {
            if (charts.colors.length == 0) {
            $.ajax({
                    type: "POST",
                    url: "/Preferences/Get",
                    data: { optionName: "chart_colors" },
                    async: false,
                    success: function (result) {
                        //alert(result);
                        console.log('loaded color preferences');
                        var defaultColors = ['#7eacb1', '#7e93b1', '#93b17e', '#acb17e'];

                        if (result != "") {
                            charts.colors = result.split(",");
                            for (var i = 0; i < charts.colors.length; i++) {
                                if (charts.colors[i] == "") {
                                    charts.colors[i] = defaultColors[i];
                                }
                            }


                        }
                        else {
                            charts.colors = defaultColors;
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }


                var options = {

                    chart: {
                        backgroundColor: null,
                        style: {
                            fontFamily: "Tahoma"
                        }
                    },

                    credits: {
                        enabled: false
                    },

                    colors: charts.colors

                };

            return options;


        },

        getTooltip(data, formatFunc) {

            var actual, forecast, target, mtd;
            var hasTarget = false;
            var hasMTD = false;

            for (var i = 0; i < data.points.length; i++) {

                if (data.points[i].series.name == "Actual") {
                    actual = data.points[i].y;
                }

                else if (data.points[i].series.name == "Target") {
                    hasTarget = true;
                    target = data.points[i].y;
                }

                else if (data.points[i].series.name == "MTDForecast") {
                    mtd = data.points[i].y;
                    hasMTD = (mtd != null);
                }

                else if (data.points[i].series.name.includes("Forecast")) {
                    forecast = data.points[i].y;
                }

            }


            var forecastDiff = actual - forecast;
            var s = App.Dates.GetFullMonthFromShortMonth(data.x) + '<br /><hr />';
            s = s + '<b>Actual:</b> ' + formatFunc(actual) + '<br />';
            s = s + '<b>Forecast:</b> ' + formatFunc(forecast) + '<br />';
            s = s + '<b>Difference:</b> ';
            if (forecastDiff < 0) {
                s = s + '<span class="number-bad"> ' + formatFunc(forecastDiff) + '</span>';
            } else {
                s = s + '<span class="number-good"> ' + formatFunc(forecastDiff) + '</span>';
            }

            if (hasTarget) {

                var targetDiff = actual - target;
                s = s + '<br /><hr />';
                s = s + '<b>Actual:</b> ' + formatFunc(actual) + '<br />';
                s = s + '<b>Target:</b> ' + formatFunc(target) + '<br />';
                s = s + '<b>Difference:</b> ';
                if (targetDiff < 0) {
                    s = s + '<span class="number-bad"> ' + formatFunc(targetDiff) + '</span>';
                } else {
                    s = s + '<span class="number-good"> ' + formatFunc(targetDiff) + '</span>';
                }

            }

            if (hasMTD) {
                s = s + '<br /><hr />';
                s = s + '<b>MTD Forecast:</b> ' + formatFunc(mtd);
            }

            return s;


        }


    }


    window.Charts = Interface;

});