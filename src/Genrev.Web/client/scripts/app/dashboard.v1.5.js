// dashboard.js

define(['devex', 'core'], function () {

    
    var config = {

        mainGridElementID: 'home-dash-grid-main',

        currentYear: {

            salesVsForecast: {
                containerName:  'cc-currentyear-salesvsforecast',
                dataUrl:        '/Home/Dashboard/ChartData/CurrentYear/SalesVsForecast'
            },

            grossProfitDollars: {
                containerName: 'cc-currentyear-grossprofitdollars',
                dataUrl: '/Home/Dashboard/ChartData/CurrentYear/GrossProfitDollars',
            },

            grossProfitPercent: {
                containerName:  'cc-currentyear-grossprofitpercent',
                dataUrl: '/Home/Dashboard/ChartData/CurrentYear/GrossProfitPercent',
            },

            projections: {
                dataUrl: '/Home/Dashboard/Data/CurrentYear/Projections',
                eomProjectionSpanID: 'cy-pfc-projection-eom',
                eoyProjectionSpanID: 'cy-pfc-projection-eoy',
                eomForecastSpanID: 'cy-pfc-forecast-eom',
                eoyForecastSpanID: 'cy-pfc-forecast-eoy',
                eomDiffSpanID: 'cy-pfc-diff-eom',
                eoyDiffSpanID: 'cy-pfc-diff-eoy'
            },

            topBottom: {
                dataUrl: '/Home/Dashboard/Data/CurrentYear/TopBottom',
                topCustomerHeaderID: 'dash-top-customer-header',
                topCustomerListID: 'dash-top-customer-list',
                bottomCustomerHeaderID: 'dash-bottom-customer-header',
                bottomCustomerListID: 'dash-bottom-customer-list',
                topSPHeaderID: 'dash-top-sp-header',
                topSPListID: 'dash-top-sp-list',
                bottomSPHeaderID: 'dash-bottom-sp-header',
                bottomSPListID: 'dash-bottom-sp-list'
            }
        }

    }   // end config


    var dashboard = {

        ensureMainGridHeight: function (isResizeEventCallback) {

            var minHeight = devex.get('ContentSplitter').GetPane(1).GetClientHeight();
            var actHeight = $('#' + config.mainGridElementID).outerHeight(true);

            if (minHeight > actHeight) {
                $('#' + config.mainGridElementID).height(minHeight);
            }

            if (!isResizeEventCallback) {
                window.addEventListener('resize', function () {
                    dashboard.ensureMainGridHeight(true);
                });
            }

        },  // end dashboard.ensureMainGridHeight

        currentYear: {

            projections: {

                currentSelection: "sales",

                selectors: {

                    current: "sales",

                    mode: "forecast",

                    setSelection: function(selection) {

                        dashboard.currentYear.projections.selectors.current = selection;
                        dashboard.currentYear.projections.load();
                        
                    },  // end dashboard.currentYear.projects.selectors.setSelection()

                    setMode: function(mode){

                        dashboard.currentYear.projections.selectors.mode = mode;
                        dashboard.currentYear.projections.load();
                    },

                    init: function () {

                        $("[data-dym-selector='pvf']").click(function (event) {

                            var selection = $(this).attr('data-dym-value');

                            if (selection == dashboard.currentYear.projections.selectors.current) {
                                return;
                            } else {
                                dashboard.currentYear.projections.selectors.setSelection(selection);
                            }

                        });

                        $("[data-dym-selector='pvf-mode']").click(function (event) {

                            var selection = $(this).attr('data-dym-value');

                            if (selection == dashboard.currentYear.projections.selectors.mode) {
                                return;
                            } else {
                                dashboard.currentYear.projections.selectors.setMode(selection);
                            }

                        });


                    }   // end dashboard.currentYear.projects.selectors.init

                },  // end dashboard.currentYear.projects.selectors


                numberFormatting: {

                    removeAll: function() {

                        $("#" + config.currentYear.projections.eomDiffSpanID).removeClass('number-good-light number-bad-light');
                        $("#" + config.currentYear.projections.eoyDiffSpanID).removeClass('number-good-light number-bad-light');

                    },

                    setAll: function (eomDiff, eoyDiff) {

                        if (eomDiff > 0) {
                            $("#" + config.currentYear.projections.eomDiffSpanID).addClass("number-good-light");
                        }
                        if (eomDiff < 0) {
                            $("#" + config.currentYear.projections.eomDiffSpanID).addClass("number-bad-light");
                        }

                        if (eoyDiff > 0) {
                            $("#" + config.currentYear.projections.eoyDiffSpanID).addClass("number-good-light");
                        }
                        if (eoyDiff < 0) {
                            $("#" + config.currentYear.projections.eoyDiffSpanID).addClass("number-bad-light");
                        }

                    }

                },

                load: function () {

                    var selection = dashboard.currentYear.projections.selectors.current;
                    var mode = dashboard.currentYear.projections.selectors.mode;

                    $("[data-dym-selector='pvf']").removeClass("selected");
                    $("[data-dym-selector='pvf-mode']").removeClass("selected");
                    $("#pvf-selector-" + selection).addClass("selected");
                    $("#pvf-mode-selector-" + mode).addClass("selected");

                    if (mode == "forecast") {
                        $("#projection-forecast-heading").text("Forecast");
                        $("#projection-title").text("Projections vs. Forecast (Month End/Year End)");
                    }
                    else if (mode == "target") {
                        $("#projection-forecast-heading").text("Target");
                        $("#projection-title").text("Projections vs. Target (Month End/Year End)");
                    }


                    if (!selection) {
                        selection = "sales";
                    }

                    $.ajax({

                        type: 'GET',
                        url: config.currentYear.projections.dataUrl,
                        data: { currentDate: new Date().toISOString() },
                        success: function(res) {

                            if (res.substr(0, 4) == "ERR:") {
                                dashboard.currentYear.projections.showError(res.substr(5));
                            } else {

                                var data = JSON.parse(res);

                                if (("saleseomTarget" in data) && $("#pvf-mode-selector-group").css("visibility") == "hidden") {
                                    $("#pvf-mode-selector-group").css('visibility', 'visible').hide().fadeIn();
                                }

                                if (!("saleseomTarget" in data)) {
                                    $("#pvf-mode-selector-group").css('visibility', 'hidden');
                                    $("#projection-forecast-heading").text("Forecast");
                                    $("#projection-title").text("Projections vs. Forecast (Month End/Year End)");
                                    dashboard.currentYear.projections.selectors.mode = "forecast";
                                    mode = "forecast";
                                }

                                if (mode == "forecast") {

                                    if (selection == "sales") {

                                        dashboard.currentYear.projections.updateValuesMoney(
                                            data.saleseomProjection,
                                            data.saleseoyProjection,
                                            data.saleseomForecast,
                                            data.saleseoyForecast,
                                            data.saleseomDiff,
                                            data.saleseoyDiff);

                                    }

                                    if (selection == "gpp") {

                                        dashboard.currentYear.projections.updateValuesPercent(
                                            data.gppeomProjection,
                                            data.gppeoyProjection,
                                            data.gppeomForecast,
                                            data.gppeoyForecast,
                                            data.gppeomDiff,
                                            data.gppeoyDiff);

                                    }

                                    if (selection == "gpd") {

                                        dashboard.currentYear.projections.updateValuesMoney(
                                            data.gpdeomProjection,
                                            data.gpdeoyProjection,
                                            data.gpdeomForecast,
                                            data.gpdeoyForecast,
                                            data.gpdeomDiff,
                                            data.gpdeoyDiff);

                                    }
                                }
                                else if (mode == "target") {

                                    if (selection == "sales") {
                                        dashboard.currentYear.projections.updateValuesMoney(
                                            data.saleseomProjection,
                                            data.saleseoyProjection,
                                            data.saleseomTarget,
                                            data.saleseoyTarget,
                                            data.saleseomTargetDiff,
                                            data.saleseoyTargetDiff);
                                    }

                                    if (selection == "gpp") {

                                        dashboard.currentYear.projections.updateValuesPercent(
                                            data.gppeomProjection,
                                            data.gppeoyProjection,
                                            data.gppeomTarget,
                                            data.gppeoyTarget,
                                            data.gppeomTargetDiff,
                                            data.gppeoyTargetDiff);

                                    }

                                    if (selection == "gpd") {
                                        dashboard.currentYear.projections.updateValuesMoney(
                                            data.gpdeomProjection,
                                            data.gpdeoyProjection,
                                            data.gpdeomTarget,
                                            data.gpdeoyTarget,
                                            data.gpdeomTargetDiff,
                                            data.gpdeoyTargetDiff);
                                    }

                                }

                            }

                            dashboard.currentYear.projections.selectors.init();

                            dashboard.currentYear.topBottom.load();

                        },
                        error: function (res) {
                            dashboard.currentYear.projections.showError(res);
                        }
                    });

                },   // end dashboard.currentYear.projects.load

                showError: function (msg) {

                    // TODO: implement
                    $("#" + config.currentYear.projections.containerName).empty().append("<p class='widget-error-msg'>" + msg + "</p>");

                },   // end dashboard.currentYear.projects.showError

                updateValuesMoney: function(eomProj, eoyProj, eomFore, eoyFore, eomDiff, eoyDiff){

                    dashboard.currentYear.projections.numberFormatting.removeAll();

                    dashboard.currentYear.projections.updateValuesText(
                        Genrev.FormatMoney(eomProj),
                        Genrev.FormatMoney(eoyProj),
                        Genrev.FormatMoney(eomFore),
                        Genrev.FormatMoney(eoyFore),
                        Genrev.FormatMoney(eomDiff),
                        Genrev.FormatMoney(eoyDiff));

                    dashboard.currentYear.projections.numberFormatting.setAll(eomDiff, eoyDiff);

                },

                updateValuesPercent: function(eomProj, eoyProj, eomFore, eoyFore, eomDiff, eoyDiff){

                    dashboard.currentYear.projections.numberFormatting.removeAll();

                    dashboard.currentYear.projections.updateValuesText(
                        Genrev.FormatPercent(eomProj),
                        Genrev.FormatPercent(eoyProj),
                        Genrev.FormatPercent(eomFore),
                        Genrev.FormatPercent(eoyFore),
                        Genrev.FormatPercent(eomDiff),
                        Genrev.FormatPercent(eoyDiff));

                    dashboard.currentYear.projections.numberFormatting.setAll(eomDiff, eoyDiff);

                },
                
                updateValuesText: function (eomProj, eoyProj, eomFore, eoyFore, eomDiff, eoyDiff) {

                    $("#" + config.currentYear.projections.eomProjectionSpanID).fadeOut(function () {
                        $(this).text(eomProj).fadeIn();
                    });
                    $("#" + config.currentYear.projections.eoyProjectionSpanID).fadeOut(function () {
                        $(this).text(eoyProj).fadeIn();
                    });
                    $("#" + config.currentYear.projections.eomForecastSpanID).fadeOut(function () {
                        $(this).text(eomFore).fadeIn();
                    });
                    $("#" + config.currentYear.projections.eoyForecastSpanID).fadeOut(function () {
                        $(this).text(eoyFore).fadeIn();
                    });
                    $("#" + config.currentYear.projections.eomDiffSpanID).fadeOut(function () {
                        $(this).text(eomDiff).fadeIn();
                    });
                    $("#" + config.currentYear.projections.eoyDiffSpanID).fadeOut(function () {
                        $(this).text(eoyDiff).fadeIn();
                    });

                }

            },  // end dashboard.currentYear.projections

            salesVsForecast: {

                load: function() {

                    console.log(config.currentYear.salesVsForecast.dataUrl);

                    $.ajax({
                        url: config.currentYear.salesVsForecast.dataUrl,
                        data: { currentDate: new Date().toISOString() },
                        success: function (result) {

                            var base = Charts.GetBase();

                            data = JSON.parse(result);
                            
                            var series = data.series;

                            Highcharts.SVGRenderer.prototype.symbols.line = function (x, y, w, h) {
                                return ['M', x, y, 'L', x + w, y];
                            };

                            var options = {
                                chart: {
                                    type: 'column'
                                },
                                title: {
                                    text: "Actual vs. Forecast"
                                },
                                tooltip: {
                                    shared: true,
                                    useHTML: true,
                                    formatter: function () {
                                        return window.Charts.GetTooltip(this, Genrev.FormatMoney);
                                    }
                                },
                                xAxis: {
                                    categories: data.categories
                                },
                                yAxis: {
                                    title: {
                                        text: "Gross Dollars"
                                    },
                                    labels: {
                                        formatter: function () {
                                            return "$" + this.axis.defaultLabelFormatter.call(this);
                                        }
                                    }
                                },
                                series: series
                            }

                            var final = merge(base, options);

                            $("#" + config.currentYear.salesVsForecast.containerName).highcharts(final, function () {

                                if ("target" in data.totals) {
                                    Genrev.Charts.Overlays.GenerateTopRight(
                                        config.currentYear.salesVsForecast.containerName, 7, 10,
                                        '<div class="chart-overlay-small">' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + Genrev.FormatMoney(data.totals.ytd.sales) + '</span><br />' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + Genrev.FormatMoney(data.totals.target) + '</span>' +
                                        '</div>');
                                }

                                else {
                                    Genrev.Charts.Overlays.GenerateTopRight(
                                        config.currentYear.salesVsForecast.containerName, 7, 10,
                                        '<div style="text-align: right;">' +
                                            '<span class="chart-ytd-overlay">Total Sales: ' + Genrev.FormatMoney(data.totals.ytd.sales) + '</span><br />' +
                                            '<span class="chart-ytd-overlay">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                        '</div>');
                                }


                            });

                            dashboard.currentYear.grossProfitDollars.load();
                        }
                    });

                }   // end dashboard.currentYear.salesVsForecast.load

            },  // end dashboard.currentYear.salesVsForecast

            grossProfitDollars: {

                load: function () {

                    $.ajax({
                        url: config.currentYear.grossProfitDollars.dataUrl,
                        data: { currentDate: new Date().toISOString() },
                        success: function (result) {

                            var base = Charts.GetBase();

                            var data = JSON.parse(result);

                            var options = {
                                legend: {
                                    enabled: false
                                },
                                tooltip: {
                                    shared: true,
                                    useHTML: true,
                                    formatter: function () {

                                        var a = this.points[0].y;
                                        var f = this.points[1].y;
                                        var d = a - f;

                                        var s = App.Dates.GetFullMonthFromShortMonth(this.x) + '<br /><hr />';
                                        s = s + '<b>Actual:</b> ' + Genrev.FormatMoney(a) + '<br />';
                                        s = s + '<b>Forecast:</b> ' + Genrev.FormatMoney(f) + '<br />';
                                        s = s + '<b>Difference:</b> ';
                                        if (d < 0) {
                                            s = s + '<span class="number-bad"> ' + Genrev.FormatMoney(d) + '</span>';
                                        } else {
                                            s = s + '<span class="number-good"> ' + Genrev.FormatMoney(d) + '</span>';
                                        }

                                        return s;
                                    }
                                },
                                chart: {
                                    type: 'column'
                                },
                                title: {
                                    text: 'Gross Profit Dollars'
                                },
                                rangeSelector: {
                                    enabled: true
                                },
                                xAxis: {
                                    categories: data.categories
                                },
                                yAxis: {
                                    title: {
                                        text: 'Gross Profit'
                                    },
                                    labels: {
                                        formatter: function () {
                                            return "$" + this.axis.defaultLabelFormatter.call(this);
                                        }
                                    }
                                },
                                series: data.series
                            }

                            $("#" + config.currentYear.grossProfitDollars.containerName).highcharts(
                                merge(base, options)
                            );

                            dashboard.currentYear.grossProfitPrecent.load();

                        }
                    });


                }   // end dashboard.currentYear.grossProfitDollars.load

            },   // end dashboard.currentYear.grossProfitDollars

            grossProfitPrecent: {

                load: function () {

                    $.ajax({
                        url: config.currentYear.grossProfitPercent.dataUrl,
                        data: { currentDate: new Date().toISOString() },
                        success: function (result) {

                            var base = Charts.GetBase();

                            var data = JSON.parse(result);

                            var options = {
                                legend: {
                                    enabled: false
                                },
                                tooltip: {
                                    shared: true,
                                    useHTML: true,
                                    formatter: function () {

                                        var a = this.points[0].y;
                                        var f = this.points[1].y;
                                        var d = a - f;

                                        var s = App.Dates.GetFullMonthFromShortMonth(this.x) + '<br /><hr />';
                                        s = s + '<b>Actual:</b> ' + Genrev.FormatPercent(a) + '<br />';
                                        s = s + '<b>Forecast:</b> ' + Genrev.FormatPercent(f) + '<br />';
                                        s = s + '<b>Difference:</b> ';
                                        if (d < 0) {
                                            s = s + '<span class="number-bad"> ' + Genrev.FormatPercent(d) + '</span>';
                                        } else {
                                            s = s + '<span class="number-good"> ' + Genrev.FormatPercent(d) + '</span>';
                                        }

                                        return s;
                                    }
                                },
                                chart: {
                                    type: 'column'
                                },
                                title: {
                                    text: 'Gross Profit Percent'
                                },
                                rangeSelector: {
                                    enabled: true
                                },
                                xAxis: {
                                    categories: data.categories
                                },
                                yAxis: {
                                    title: {
                                        text: 'Gross Profit Percent'
                                    },
                                    labels: {
                                        
                                        format: "{value}%"
                                    }
                                },
                                series: data.series
                            }

                            $("#" + config.currentYear.grossProfitPercent.containerName).highcharts(
                                merge(base, options)
                            );

                            dashboard.currentYear.projections.load();

                        }
                    });


                }   // end dashboard.currentYear.grossProfitPercent.load

            },   // end dashboard.currentYear.grossProfitPercent

            topBottom: {

                load: function () {

                    $.ajax({
                        type: 'GET',
                        url: config.currentYear.topBottom.dataUrl,
                        data: { currentDate: new Date().toISOString() },
                        success: function (res) {

                            var tb = dashboard.currentYear.topBottom;
                            var data = JSON.parse(res);

                            tb.loadTopCustomers(data.topCustomers);
                            tb.loadTopSalespersons(data.topSalespersons);
                            tb.loadBottomCustomers(data.bottomCustomers);
                            tb.loadBottomSalespersons(data.bottomSalespersons);
                        }
                    });

                },   // end dashboard.currentYear.topBottom.load

                loadTopCustomers: function (data) {

                    $('#' + config.currentYear.topBottom.topCustomerHeaderID).show();
                    var parent = $('#' + config.currentYear.topBottom.topCustomerListID);
                    parent.html("");

                    for (var i = 0; i < data.length; i++) {
                        var d = data[i];
                        var e = '<li class="tb-list-item" data-dym-id="' + d.EntityID + '"><span class="tb-list-name">' + d.EntityName + '</span><span class="tb-list-value number-good">' + Genrev.FormatMoney(d.EntityValue) + '</span></li>';
                        parent.append(e);
                    }

                },

                loadBottomCustomers: function (data) {

                    $('#' + config.currentYear.topBottom.bottomCustomerHeaderID).show();
                    var parent = $('#' + config.currentYear.topBottom.bottomCustomerListID);
                    parent.html("");

                    for (var i = 0; i < data.length; i++) {
                        var d = data[i];
                        var e = '<li class="tb-list-item" data-dym-id="' + d.EntityID + '"><span class="tb-list-name">' + d.EntityName + '</span><span class="tb-list-value number-bad">' + Genrev.FormatMoney(d.EntityValue) + '</span></li>';
                        parent.append(e);
                    }
                    
                },

                loadTopSalespersons: function (data) {

                    $('#' + config.currentYear.topBottom.topSPHeaderID).show();
                    var parent = $('#' + config.currentYear.topBottom.topSPListID);
                    parent.html("");

                    for (var i = 0; i < data.length; i++) {
                        var d = data[i];
                        var e = '<li class="tb-list-item" data-dym-id="' + d.EntityID + '"><span class="tb-list-name">' + d.EntityName + '</span><span class="tb-list-value number-good">' + Genrev.FormatMoney(d.EntityValue) + '</span></li>';
                        parent.append(e);
                    }

                },

                loadBottomSalespersons: function (data) {

                    $('#' + config.currentYear.topBottom.bottomSPHeaderID).show();
                    var parent = $('#' + config.currentYear.topBottom.bottomSPListID);
                    parent.html("");

                    for (var i = 0; i < data.length; i++) {
                        var d = data[i];
                        var e = '<li class="tb-list-item" data-dym-id="' + d.EntityID + '"><span class="tb-list-name">' + d.EntityName + '</span><span class="tb-list-value number-bad">' + Genrev.FormatMoney(d.EntityValue) + '</span></li>';
                        parent.append(e);
                    }

                }

            }   // end dashboard.currentYear.topBottom

        },  // end dashboard.currentYear

        initialize: function () {

            dashboard.ensureMainGridHeight();
            dashboard.currentYear.salesVsForecast.load();
                        
        }   // end dashboard.initialize
        
    }   // end dashboard



    return {
        initialize: function() { dashboard.initialize(); }
    }


});