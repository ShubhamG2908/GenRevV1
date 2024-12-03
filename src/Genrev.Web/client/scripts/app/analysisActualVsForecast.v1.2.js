// dashboard.js

define(function () {

    /*****************************************
    
    ******************************************/

    var Interface = {
        
        Initialize: function () { page.initialize(); }

    }   // end Customers.Interface


    var config = {

        mainGridElementID: "avf-grid-main",

        tabContentElementID: "avf-tab-content",

        salesperson: {
            page: {
                url: '/Analysis/ActualVsForecast/Page/Salesperson'
            },
            dollars: {
                containerName: 'an-avf-sp-dollars',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/Sales'
            },
            gp: {
                containerName: 'an-avf-sp-gp',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/GP'
            }
        },
        industry: {
            page: { url: '/Analysis/ActualVsForecast/Page/Industry' },
            dollars: {
                containerName: 'an-avf-ind-dollars',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/Industry/Sales'
            },
            gp: {
                containerName: 'an-avf-ind-gp',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/Industry/GP'
            }
        },
        customerType: {
            page: { url: '/Analysis/ActualVsForecast/Page/CustomerType' },
            dollars: {
                containerName: 'an-avf-customerType-dollars',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/CustomerType/Sales'
            },
            gp: {
                containerName: 'an-avf-customerType-gp',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/CustomerType/GP'
            }
        },
        accountType: {
            page: { url: '/Analysis/ActualVsForecast/Page/AccountType' },
            dollars: {
                containerName: 'an-avf-accountType-dollars',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/AccountType/Sales'
            },
            gp: {
                containerName: 'an-avf-accountType-gp',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/AccountType/GP'
            }
        },
        product: {
            page: { url: '/Analysis/ActualVsForecast/Page/Product' },
            dollars: {
                containerName: 'an-avf-product-dollars',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/Product/Sales'
            },
            gp: {
                containerName: 'an-avf-product-gp',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/Product/GP'
            }
        },
        customer: {
            page: { url: '/Analysis/ActualVsForecast/Page/Customer' },
            dollars: {
                containerName: 'an-avf-customer-dollars',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/Customer/Sales'
            },
            gp: {
                containerName: 'an-avf-customer-gp',
                dataUrl: '/Analysis/ActualVsForecast/ChartData/Customer/GP'
            }
        }

    }   // end config


    var page = {

        viewParams: {

            initialize: function() {

                DevEx.Controls.GetByName("an-avf-salespersonCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-avf-industriesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-avf-customerTypesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-avf-accountTypesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-avf-productsCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-avf-customersCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-avf-yearsToShowCombobox").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                
            }

        },

        getParams: function() {

            var salesperson = DevEx.Controls.GetValue("an-avf-salespersonCombo");
            var industry = DevEx.Controls.GetValue("an-avf-industriesCombo");
            var customerType = DevEx.Controls.GetValue("an-avf-customerTypesCombo");
            var accountType = DevEx.Controls.GetValue("an-avf-accountTypesCombo");
            var product = DevEx.Controls.GetValue("an-avf-productsCombo");
            var customer = DevEx.Controls.GetValue("an-avf-customersCombo");
            var years = DevEx.Controls.GetValue("an-avf-yearsToShowCombobox");

            return {
                salesperson: salesperson,
                industry: industry,
                customerType: customerType,
                accountType: accountType,
                product: product,
                customer: customer,
                years: years
            }

        },  // end page.getParams

        initialize: function() {

            page.ensureMainGridHeight();
            page.viewParams.initialize();
            page.tabSelectors.initialize();

            var salesperson = page.tabs.salesperson;
            salesperson.loadPage();

        },  // end page.initialize

        ensureMainGridHeight: function (isResizeEventCallback) {

            var minHeight = devex.get('ContentSplitter').GetPane(1).GetClientHeight();
            var actHeight = $('#' + config.mainGridElementID).outerHeight(true);

            if (minHeight > actHeight) {
                $('#' + config.mainGridElementID).height(minHeight);
            }

            if (!isResizeEventCallback) {
                window.addEventListener('resize', function () {
                    page.ensureMainGridHeight(true);
                });
            }

        },  // end page.ensureMainGridHeight

        tabSelectors: {

            current: 'salesperson',

            initialize: function () {

                console.log('loading tabSelectors');

                var ts = page.tabSelectors;

                $("[data-dym-selector='an-avf']").click(function (event) {

                    console.log('tab clicked');

                    var selection = $(this).attr('data-dym-value');
                    var selectionClass = $(this).attr('class');

                    if (selectionClass === 'selected') {
                        return;
                    } else {
                        ts.setSelection(selection);
                    }

                });

            },   // end page.tabSelectors.initialize

            setSelection: function (selection) {

                var oldID = "#an-avf-selector-" + page.tabSelectors.current;
                var newID = "#an-avf-selector-" + selection;

                if (selection !== 'salesperson') {
                    $("#an-avf-selector-salesperson").removeClass("selected");
                }

                $(oldID).removeClass("selected");
                $(newID).addClass("selected");

                page.tabSelectors.current = selection;

                var tabs = page.tabs;

                switch (selection) {
                    case 'salesperson':
                        $("#an-avf-salespersonComboContainer").show();
                        $("#an-avf-industryComboContainer").hide();
                        $("#an-avf-customerTypeComboContainer").hide();
                        $("#an-avf-accountTypeComboContainer").hide();
                        $("#an-avf-productComboContainer").hide();
                        $("#an-avf-customerComboContainer").hide();
                        tabs.salesperson.loadPage();
                        break;
                    case 'industry':
                        $("#an-avf-salespersonComboContainer").hide();
                        $("#an-avf-industryComboContainer").show();
                        $("#an-avf-customerTypeComboContainer").hide();
                        $("#an-avf-accountTypeComboContainer").hide();
                        $("#an-avf-productComboContainer").hide();
                        $("#an-avf-customerComboContainer").hide();
                        tabs.industry.loadPage();
                        break;
                    case 'customerType':
                        $("#an-avf-salespersonComboContainer").hide();
                        $("#an-avf-industryComboContainer").hide();
                        $("#an-avf-customerTypeComboContainer").show();
                        $("#an-avf-accountTypeComboContainer").hide();
                        $("#an-avf-productComboContainer").hide();
                        $("#an-avf-customerComboContainer").hide();
                        tabs.customerType.loadPage();
                        break;
                    case 'accountType':
                        $("#an-avf-salespersonComboContainer").hide();
                        $("#an-avf-industryComboContainer").hide();
                        $("#an-avf-customerTypeComboContainer").hide();
                        $("#an-avf-accountTypeComboContainer").show();
                        $("#an-avf-productComboContainer").hide();
                        $("#an-avf-customerComboContainer").hide();
                        tabs.accountType.loadPage();
                        break;
                    case 'product':
                        $("#an-avf-salespersonComboContainer").hide();
                        $("#an-avf-industryComboContainer").hide();
                        $("#an-avf-customerTypeComboContainer").hide();
                        $("#an-avf-accountTypeComboContainer").hide();
                        $("#an-avf-productComboContainer").show();
                        $("#an-avf-customerComboContainer").hide();
                        tabs.product.loadPage();
                        break;
                    case 'customer':
                        $("#an-avf-salespersonComboContainer").hide();
                        $("#an-avf-industryComboContainer").hide();
                        $("#an-avf-customerTypeComboContainer").hide();
                        $("#an-avf-accountTypeComboContainer").hide();
                        $("#an-avf-productComboContainer").hide();
                        $("#an-avf-customerComboContainer").show();
                        tabs.customer.loadPage();
                        break;
                }

            }   // end page.tabSelectors.setSelection

        },  // end page.tabSelectors

        tabs: {

            refreshCurrent: function () {
                // force a reload
                page.tabSelectors.setSelection(page.tabSelectors.current);
            },

            salesperson: {

                loadPage: function () {

                    console.log('loading salesperson page');
                    
                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.salesperson.page.url,
                        data: {
                            salespersonID: params.salesperson,
                            yearsToShow: params.years
                        },
                        success: function (res) {
                            $("#" + config.tabContentElementID).empty().append(res);
                            
                            var sp = page.tabs.salesperson;
                            sp.dollars.load();
                            sp.gp.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },  // end page.tabs.salesperson.loadPage

                dollars: {

                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.salesperson.dollars.dataUrl,
                            data: { salesperson: params.salesperson, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                var series = data.series;

                                Highcharts.SVGRenderer.prototype.symbols.line = function (x, y, w, h) {
                                    return ['M', x, y, 'L', x + w, y];
                                };

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },
                                    legend: {
                                        enabled: true
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, Genrev.FormatMoney);
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: series
                                }

                                var final = merge(base, options);

                                $('#' + config.salesperson.dollars.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.salesperson.dollars.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + Genrev.FormatMoney(data.totals.target) + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.salesperson.dollars.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tags.salesperson.dollars.load

                },  // end page.tabs.salesperson.dollars

                gp: {

                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.salesperson.gp.dataUrl,
                            data: { salesperson: params.salesperson, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                console.log(result);

                                console.log(data);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },

                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {

                                            var gpda = this.points[0].y;
                                            var gpdf = this.points[1].y;
                                            var gpdd = gpda - gpdf;

                                            var gppa = this.points[2].y;
                                            var gppf = this.points[3].y;
                                            var gppd = gppa - gppf;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD Actual:</b> ' + Genrev.FormatMoney(gpda) + '<br />';
                                            s = s + '<b>GPD Forecast:</b> ' + Genrev.FormatMoney(gpdf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gpdd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            }
                                            
                                            s = s + "<hr />";

                                            s = s + '<b>GPP Actual:</b> ' + Genrev.FormatPercent(gppa) + '<br />';
                                            s = s + '<b>GPP Forecast:</b> ' + Genrev.FormatPercent(gppf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gppd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            }

                                            
                                            
                                            return s;
                                        }
                                    },

                                    title: { text: 'Actual vs. Forecast (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars (GPD)' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent (GPP)' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.salesperson.gp.containerName).highcharts(final, function () {

                                    Genrev.Charts.Overlays.GenerateTopRight(
                                        config.salesperson.gp.containerName, 7, 10,
                                        '<div style="text-align: right;">' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD: ' + Genrev.FormatMoney(data.totals.gpd) + '</span><br />' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD Forecast: ' + Genrev.FormatMoney(data.totals.gpdForecast) + '</span>' +
                                        '</div>');

                                });

                            }
                        });

                    }   // end page.tags.salesperson.gp.load

                }   // end page.tabs.salesperson.gp


            },  // end page.tabs.salesperson

            industry: {

                loadPage: function() {

                    console.log('loading industry page');

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.industry.page.url,
                        data: {
                            industryID: params.industry,
                            yearsToShow: params.years
                        },
                        success: function (res) {
                            $("#" + config.tabContentElementID).empty().append(res);

                            var ind = page.tabs.industry;
                            ind.dollars.load();
                            ind.gp.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.industry.loadPage

                dollars: {
                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.industry.dollars.dataUrl,
                            data: { industry: params.industry, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },
                                    legend: {
                                        enabled: false
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, Genrev.FormatMoney);
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.industry.dollars.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.industry.dollars.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + Genrev.FormatMoney(data.totals.target) + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.industry.dollars.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.industry.dollars.load

                },  // end page.tabs.industry.dollars

                gp: {
                    load: function () {


                        var params = page.getParams();

                        $.ajax({
                            url: config.industry.gp.dataUrl,
                            data: { industry: params.industry, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                console.log(result);

                                console.log(data);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },

                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {

                                            var gpda = this.points[0].y;
                                            var gpdf = this.points[1].y;
                                            var gpdd = gpda - gpdf;

                                            var gppa = this.points[2].y;
                                            var gppf = this.points[3].y;
                                            var gppd = gppa - gppf;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD Actual:</b> ' + Genrev.FormatMoney(gpda) + '<br />';
                                            s = s + '<b>GPD Forecast:</b> ' + Genrev.FormatMoney(gpdf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gpdd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            }

                                            s = s + "<hr />";

                                            s = s + '<b>GPP Actual:</b> ' + Genrev.FormatPercent(gppa) + '<br />';
                                            s = s + '<b>GPP Forecast:</b> ' + Genrev.FormatPercent(gppf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gppd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            }



                                            return s;
                                        }
                                    },

                                    title: { text: 'Actual vs. Forecast (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars (GPD)' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent (GPP)' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.industry.gp.containerName).highcharts(final, function () {

                                    Genrev.Charts.Overlays.GenerateTopRight(
                                        config.industry.gp.containerName, 7, 10,
                                        '<div style="text-align: right;">' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD: ' + Genrev.FormatMoney(data.totals.gpd) + '</span><br />' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD Forecast: ' + Genrev.FormatMoney(data.totals.gpdForecast) + '</span>' +
                                        '</div>');

                                });

                            }
                        });

                    }   // end page.tabs.industry.gp.load

                }   // end page.tabs.industry.gp
                

            },  // end page.tabs.industry

            customerType: {


                loadPage: function () {

                    console.log('loading customerType page');

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.customerType.page.url,
                        data: {
                            customerTypeID: params.customerType,
                            yearsToShow: params.years
                        },
                        success: function (res) {
                            $("#" + config.tabContentElementID).empty().append(res);

                            var ct = page.tabs.customerType;
                            ct.dollars.load();
                            ct.gp.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.industry.loadPage

                dollars: {
                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.customerType.dollars.dataUrl,
                            data: { customerType: params.customerType, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },
                                    legend: {
                                        enabled: false
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, Genrev.FormatMoney);
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customerType.dollars.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.customerType.dollars.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + Genrev.FormatMoney(data.totals.target) + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.customerType.dollars.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.industry.dollars.load

                },  // end page.tabs.industry.dollars

                gp: {

                    load: function () {


                        var params = page.getParams();

                        $.ajax({
                            url: config.customerType.gp.dataUrl,
                            data: { customerType: params.customerType, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                console.log(result);

                                console.log(data);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },

                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {

                                            var gpda = this.points[0].y;
                                            var gpdf = this.points[1].y;
                                            var gpdd = gpda - gpdf;

                                            var gppa = this.points[2].y;
                                            var gppf = this.points[3].y;
                                            var gppd = gppa - gppf;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD Actual:</b> ' + Genrev.FormatMoney(gpda) + '<br />';
                                            s = s + '<b>GPD Forecast:</b> ' + Genrev.FormatMoney(gpdf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gpdd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            }

                                            s = s + "<hr />";

                                            s = s + '<b>GPP Actual:</b> ' + Genrev.FormatPercent(gppa) + '<br />';
                                            s = s + '<b>GPP Forecast:</b> ' + Genrev.FormatPercent(gppf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gppd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            }



                                            return s;
                                        }
                                    },

                                    title: { text: 'Actual vs. Forecast (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars (GPD)' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent (GPP)' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customerType.gp.containerName).highcharts(final, function () {

                                    Genrev.Charts.Overlays.GenerateTopRight(
                                        config.customerType.gp.containerName, 7, 10,
                                        '<div style="text-align: right;">' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD: ' + Genrev.FormatMoney(data.totals.gpd) + '</span><br />' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD Forecast: ' + Genrev.FormatMoney(data.totals.gpdForecast) + '</span>' +
                                        '</div>');

                                });

                            }
                        });

                    }   // end page.tabs.customerType.gp.load

                }   // end page.tabs.customerType.gp

            },   // end page.tabs.customerType

            accountType: {


                loadPage: function () {

                    console.log('loading accountType page');

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.accountType.page.url,
                        data: {
                            accountTypeID: params.accountType,
                            yearsToShow: params.years
                        },
                        success: function (res) {
                            $("#" + config.tabContentElementID).empty().append(res);

                            var ct = page.tabs.accountType;
                            ct.dollars.load();
                            ct.gp.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.industry.loadPage

                dollars: {
                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.accountType.dollars.dataUrl,
                            data: { accountType: params.accountType, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },
                                    legend: {
                                        enabled: false
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, Genrev.FormatMoney);
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.accountType.dollars.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.accountType.dollars.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + Genrev.FormatMoney(data.totals.target) + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.accountType.dollars.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.industry.dollars.load

                },  // end page.tabs.industry.dollars

                gp: {

                    load: function () {


                        var params = page.getParams();

                        $.ajax({
                            url: config.accountType.gp.dataUrl,
                            data: { accountType: params.accountType, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                console.log(result);

                                console.log(data);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },

                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {

                                            var gpda = this.points[0].y;
                                            var gpdf = this.points[1].y;
                                            var gpdd = gpda - gpdf;

                                            var gppa = this.points[2].y;
                                            var gppf = this.points[3].y;
                                            var gppd = gppa - gppf;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD Actual:</b> ' + Genrev.FormatMoney(gpda) + '<br />';
                                            s = s + '<b>GPD Forecast:</b> ' + Genrev.FormatMoney(gpdf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gpdd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            }

                                            s = s + "<hr />";

                                            s = s + '<b>GPP Actual:</b> ' + Genrev.FormatPercent(gppa) + '<br />';
                                            s = s + '<b>GPP Forecast:</b> ' + Genrev.FormatPercent(gppf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gppd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            }



                                            return s;
                                        }
                                    },

                                    title: { text: 'Actual vs. Forecast (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars (GPD)' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent (GPP)' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.accountType.gp.containerName).highcharts(final, function () {

                                    Genrev.Charts.Overlays.GenerateTopRight(
                                        config.accountType.gp.containerName, 7, 10,
                                        '<div style="text-align: right;">' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD: ' + Genrev.FormatMoney(data.totals.gpd) + '</span><br />' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD Forecast: ' + Genrev.FormatMoney(data.totals.gpdForecast) + '</span>' +
                                        '</div>');

                                });

                            }
                        });

                    }   // end page.tabs.accountType.gp.load

                }   // end page.tabs.accountType.gp

            },   // end page.tabs.accountType
            
            product: {
                
                loadPage: function () {

                    console.log('loading product page');

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.product.page.url,
                        data: {
                            productID: params.product,
                            yearsToShow: params.years
                        },
                        success: function (res) {

                            $("#" + config.tabContentElementID).empty().append(res);

                            var ct = page.tabs.product;
                            ct.dollars.load();
                            ct.gp.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.industry.loadPage

                dollars: {

                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.product.dollars.dataUrl,
                            data: { product: params.product, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },
                                    legend: {
                                        enabled: false
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, Genrev.FormatMoney);
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.product.dollars.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.product.dollars.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + Genrev.FormatMoney(data.totals.target) + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.product.dollars.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.industry.dollars.load

                },  // end page.tabs.industry.dollars

                gp: {

                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.product.gp.dataUrl,
                            data: { product: params.product, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },

                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {

                                            var gpda = this.points[0].y;
                                            var gpdf = this.points[1].y;
                                            var gpdd = gpda - gpdf;

                                            var gppa = this.points[2].y;
                                            var gppf = this.points[3].y;
                                            var gppd = gppa - gppf;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD Actual:</b> ' + Genrev.FormatMoney(gpda) + '<br />';
                                            s = s + '<b>GPD Forecast:</b> ' + Genrev.FormatMoney(gpdf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gpdd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            }

                                            s = s + "<hr />";

                                            s = s + '<b>GPP Actual:</b> ' + Genrev.FormatPercent(gppa) + '<br />';
                                            s = s + '<b>GPP Forecast:</b> ' + Genrev.FormatPercent(gppf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gppd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            }



                                            return s;
                                        }
                                    },

                                    title: { text: 'Actual vs. Forecast (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars (GPD)' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent (GPP)' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.product.gp.containerName).highcharts(final, function () {

                                    Genrev.Charts.Overlays.GenerateTopRight(
                                        config.product.gp.containerName, 7, 10,
                                        '<div style="text-align: right;">' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD: ' + Genrev.FormatMoney(data.totals.gpd) + '</span><br />' +
                                            '<span class="chart-ytd-overlay-med-sm">Total GPD Forecast: ' + Genrev.FormatMoney(data.totals.gpdForecast) + '</span>' +
                                        '</div>');

                                });

                            }
                        });

                    }   // end page.tabs.product.gp.load

                }   // end page.tabs.product.gp

            },   // end page.tabs.product

            customer: {

                loadPage: function () {

                    console.log('loading customer page');

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.customer.page.url,
                        data: {
                            customerID: params.customer,
                            yearsToShow: params.years
                        },
                        success: function (res) {

                            $("#" + config.tabContentElementID).empty().append(res);

                            var ct = page.tabs.customer;
                            ct.dollars.load();
                            ct.gp.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.customer.loadPage

                dollars: {

                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.customer.dollars.dataUrl,
                            data: { customer: params.customer, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },
                                    legend: {
                                        enabled: false
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, Genrev.FormatMoney);
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customer.dollars.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.customer.dollars.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + Genrev.FormatMoney(data.totals.target) + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.customer.dollars.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                            '<span class="chart-ytd-overlay">Total Sales: ' + Genrev.FormatMoney(data.totals.sales) + '</span><br />' +
                                            '<span class="chart-ytd-overlay">Total Forecast: ' + Genrev.FormatMoney(data.totals.forecast) + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.customer.dollars.load

                },  // end page.tabs.customer.dollars

                gp: {

                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.customer.gp.dataUrl,
                            data: { customer: params.customer, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

                                var options = {

                                    chart: {
                                        type: 'column',
                                        height: 340
                                    },

                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {

                                            var gpda = this.points[0].y;
                                            var gpdf = this.points[1].y;
                                            var gpdd = gpda - gpdf;

                                            var gppa = this.points[2].y;
                                            var gppf = this.points[3].y;
                                            var gppd = gppa - gppf;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD Actual:</b> ' + Genrev.FormatMoney(gpda) + '<br />';
                                            s = s + '<b>GPD Forecast:</b> ' + Genrev.FormatMoney(gpdf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gpdd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatMoney(gpdd) + '</span>';
                                            }

                                            s = s + "<hr />";

                                            s = s + '<b>GPP Actual:</b> ' + Genrev.FormatPercent(gppa) + '<br />';
                                            s = s + '<b>GPP Forecast:</b> ' + Genrev.FormatPercent(gppf) + '<br />';
                                            s = s + '<b>Difference:</b> ';

                                            if (gppd < 0) {
                                                s = s + '<span class="number-bad"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            } else {
                                                s = s + '<span class="number-good"> ' + Genrev.FormatPercent(gppd) + '</span>';
                                            }



                                            return s;
                                        }
                                    },

                                    title: { text: 'Actual vs. Forecast (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars (GPD)' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent (GPP)' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customer.gp.containerName).highcharts(final, function () {

                                    Genrev.Charts.Overlays.GenerateTopRight(
                                        config.customer.gp.containerName, 7, 10,
                                        '<div style="text-align: right;">' +
                                        '<span class="chart-ytd-overlay-med-sm">Total GPD: ' + Genrev.FormatMoney(data.totals.gpd) + '</span><br />' +
                                        '<span class="chart-ytd-overlay-med-sm">Total GPD Forecast: ' + Genrev.FormatMoney(data.totals.gpdForecast) + '</span>' +
                                        '</div>');

                                });

                            }
                        });

                    }   // end page.tabs.customer.gp.load

                }   // end page.tabs.customer.gp

            }   // end page.tabs.customer



















        }   // end page.tabs

    }   // end page
    



    window.AnalysisActualVsForecast = Interface;

});