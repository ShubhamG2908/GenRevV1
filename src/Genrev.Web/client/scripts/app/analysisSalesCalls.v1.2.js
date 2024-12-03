// dashboard.js

define(function () {

    /*****************************************
    
    ******************************************/

    var Interface = {
        
        Initialize: function () {
            page.initialize();
            grid.initialize();
        }

    }   // end Customers.Interface


    var config = {

        mainGridElementID: "sc-grid-main",

        tabContentElementID: "sc-tab-content",

        salesperson: {
            page: {
                url: '/Analysis/SalesCalls/Page/Salesperson'
            },
            calls: {
                containerName: 'an-sc-sp-calls',
                dataUrl: '/Analysis/SalesCalls/ChartData/Calls'
            }
        },
        industry: {
            page: { url: '/Analysis/SalesCalls/Page/Industry' },
            calls: {
                containerName: 'an-sc-ind-calls',
                dataUrl: '/Analysis/SalesCalls/ChartData/Industry/Calls'
            }
        },
        customerType: {
            page: { url: '/Analysis/SalesCalls/Page/CustomerType' },
            calls: {
                containerName: 'an-sc-customerType-calls',
                dataUrl: '/Analysis/SalesCalls/ChartData/CustomerType/Calls'
            }
        },
        accountType: {
            page: { url: '/Analysis/SalesCalls/Page/AccountType' },
            calls: {
                containerName: 'an-sc-accountType-calls',
                dataUrl: '/Analysis/SalesCalls/ChartData/AccountType/Calls'
            }
        },
        product: {
            page: { url: '/Analysis/SalesCalls/Page/Product' },
            calls: {
                containerName: 'an-sc-product-calls',
                dataUrl: '/Analysis/SalesCalls/ChartData/Product/Calls'
            }
        },
        customer: {
            page: { url: '/Analysis/SalesCalls/Page/Customer' },
            calls: {
                containerName: 'an-sc-customer-calls',
                dataUrl: '/Analysis/SalesCalls/ChartData/Customer/Calls'
            }
        }

    }   // end config


    var page = {

        viewParams: {

            initialize: function () {

                var grid = DevEx.Controls.GetByName("SalesCallPlanYearOverview");

                DevEx.Controls.GetByName("an-sc-salespersonCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                    grid.PerformCallback();
                });
                DevEx.Controls.GetByName("an-sc-industriesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-sc-customerTypesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-sc-accountTypesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-sc-productsCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-sc-customersCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-sc-yearsToShowCombobox").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                    grid.PerformCallback();
                });

            }

        },

        getParams: function() {

            var salesperson = DevEx.Controls.GetValue("an-sc-salespersonCombo");
            var industry = DevEx.Controls.GetValue("an-sc-industriesCombo");
            var customerType = DevEx.Controls.GetValue("an-sc-customerTypesCombo");
            var accountType = DevEx.Controls.GetValue("an-sc-accountTypesCombo");
            var product = DevEx.Controls.GetValue("an-sc-productsCombo");
            var customer = DevEx.Controls.GetValue("an-sc-customersCombo");
            var years = DevEx.Controls.GetValue("an-sc-yearsToShowCombobox");

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

                $("[data-dym-selector='an-sc']").click(function (event) {

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
                
                var oldID = "#an-sc-selector-" + page.tabSelectors.current;
                var newID = "#an-sc-selector-" + selection;

                if (selection !== 'salesperson') {
                    $("#an-sc-selector-salesperson").removeClass("selected");
                }

                $(oldID).removeClass("selected");
                $(newID).addClass("selected");

                page.tabSelectors.current = selection;
                DevEx.Controls.GetByName("SalesCallPlanYearOverview").PerformCallback();

                var tabs = page.tabs;

                switch (selection) {
                    case 'salesperson':
                        $("#an-sc-salespersonComboContainer").show();
                        $("#an-sc-industryComboContainer").hide();
                        $("#an-sc-customerTypeComboContainer").hide();
                        $("#an-sc-accountTypeComboContainer").hide();
                        $("#an-sc-productComboContainer").hide();
                        $("#an-sc-customerComboContainer").hide();
                        tabs.salesperson.loadPage();
                        break;
                    case 'industry':
                        $("#an-sc-salespersonComboContainer").hide();
                        $("#an-sc-industryComboContainer").show();
                        $("#an-sc-customerTypeComboContainer").hide();
                        $("#an-sc-accountTypeComboContainer").hide();
                        $("#an-sc-productComboContainer").hide();
                        $("#an-sc-customerComboContainer").hide();
                        tabs.industry.loadPage();
                        break;
                    case 'customerType':
                        $("#an-sc-salespersonComboContainer").hide();
                        $("#an-sc-industryComboContainer").hide();
                        $("#an-sc-customerTypeComboContainer").show();
                        $("#an-sc-accountTypeComboContainer").hide();
                        $("#an-sc-productComboContainer").hide();
                        $("#an-sc-customerComboContainer").hide();
                        tabs.customerType.loadPage();
                        break;
                    case 'accountType':
                        $("#an-sc-salespersonComboContainer").hide();
                        $("#an-sc-industryComboContainer").hide();
                        $("#an-sc-customerTypeComboContainer").hide();
                        $("#an-sc-accountTypeComboContainer").show();
                        $("#an-sc-productComboContainer").hide();
                        $("#an-sc-customerComboContainer").hide();
                        tabs.accountType.loadPage();
                        break;
                    case 'product':
                        $("#an-sc-salespersonComboContainer").hide();
                        $("#an-sc-industryComboContainer").hide();
                        $("#an-sc-customerTypeComboContainer").hide();
                        $("#an-sc-accountTypeComboContainer").hide();
                        $("#an-sc-productComboContainer").show();
                        $("#an-sc-customerComboContainer").hide();
                        tabs.product.loadPage();
                        break;
                    case 'customer':
                        $("#an-sc-salespersonComboContainer").hide();
                        $("#an-sc-industryComboContainer").hide();
                        $("#an-sc-customerTypeComboContainer").hide();
                        $("#an-sc-accountTypeComboContainer").hide();
                        $("#an-sc-productComboContainer").hide();
                        $("#an-sc-customerComboContainer").show();
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
                            sp.calls.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },  // end page.tabs.salesperson.loadPage

                calls: {

                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.salesperson.calls.dataUrl,
                            data: { salesperson: params.salesperson, years: params.years },
                            success: function (result) {

                                var base = Charts.GetBase();

                                data = JSON.parse(result);

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
                                            return window.Charts.GetTooltip(this, function (str) { return str; });
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Calls' }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.salesperson.calls.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.salesperson.calls.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + data.totals.target + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.salesperson.calls.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                            '</div>');
                                    }


                                });

                            }
                        });

                    }   // end page.tags.salesperson.calls.load

                }  // end page.tabs.salesperson.calls
                
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
                            ind.calls.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.industry.loadPage

                calls: {
                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.industry.calls.dataUrl,
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
                                        enabled: true
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, function (str) { return str; });
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Calls' }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.industry.calls.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.industry.calls.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + data.totals.target + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.industry.calls.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.industry.calls.load

                }  // end page.tabs.industry.calls
                
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
                            ct.calls.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.industry.loadPage

                calls: {
                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.customerType.calls.dataUrl,
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
                                        enabled: true
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, function (str) { return str; });
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Calls' }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customerType.calls.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.customerType.calls.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + data.totals.target + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.customerType.calls.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.industry.calls.load

                }  // end page.tabs.industry.calls

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
                            ct.calls.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.industry.loadPage

                calls: {
                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.accountType.calls.dataUrl,
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
                                        enabled: true
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, function (str) { return str; });
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Calls' }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.accountType.calls.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.accountType.calls.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + data.totals.target + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.accountType.calls.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.industry.calls.load

                },  // end page.tabs.industry.calls

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
                            ct.calls.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.industry.loadPage

                calls: {
                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.product.calls.dataUrl,
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
                                        enabled: true
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, function (str) { return str; });
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Calls' }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.product.calls.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.product.calls.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                                '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + data.totals.target + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.product.calls.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                                '<span class="chart-ytd-overlay">Total Sales: ' + data.totals.calls + '</span><br />' +
                                                '<span class="chart-ytd-overlay">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.industry.calls.load

                },  // end page.tabs.industry.calls

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
                            ct.calls.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },   // end page.tabs.customer.loadPage

                calls: {
                    load: function () {

                        var params = page.getParams();

                        $.ajax({
                            url: config.customer.calls.dataUrl,
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
                                        enabled: true
                                    },
                                    tooltip: {
                                        shared: true,
                                        useHTML: true,
                                        formatter: function () {
                                            return window.Charts.GetTooltip(this, function (str) { return str; });
                                        }
                                    },
                                    title: { text: 'Actual vs. Forecast' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Calls' }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customer.calls.containerName).highcharts(final, function () {

                                    if ("target" in data.totals) {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.customer.calls.containerName, 7, 10,
                                            '<div class="chart-overlay-small">' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Sales: ' + data.totals.calls + '</span><br />' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                            '<span class="chart-ytd-overlay chart-overlay-small">Total Target: ' + data.totals.target + '</span>' +
                                            '</div>');
                                    }

                                    else {
                                        Genrev.Charts.Overlays.GenerateTopRight(
                                            config.customer.calls.containerName, 7, 10,
                                            '<div style="text-align: right;">' +
                                            '<span class="chart-ytd-overlay">Total Sales: ' + data.totals.calls + '</span><br />' +
                                            '<span class="chart-ytd-overlay">Total Forecast: ' + data.totals.forecast + '</span><br />' +
                                            '</div>');
                                    }

                                });

                            }
                        });

                    }   // end page.tabs.customer.calls.load

                },  // end page.tabs.customer.calls

            }   // end page.tabs.customer


        }   // end page.tabs

    }   // end page
    
    var grid = {
        
        events: {
            addBeginCallbackHandler: function (dvxGrid) {

                dvxGrid.BeginCallback.AddHandler(function (s, e) {
                    var params = page.getParams();

                    if (page.tabSelectors.current == "salesperson") {
                        e.customArgs["salespersonID"] = params.salesperson;
                    }
                    e.customArgs["fiscalYear"] = params.years;
                });

            }
        },

        initialize: function () {
            console.log("initializing SalesCallPlanYearOverview grid")
            var dvxGrid = DevEx.Controls.GetByName("SalesCallPlanYearOverview");
            grid.events.addBeginCallbackHandler(dvxGrid);
        }

    }

    window.AnalysisSalesCalls = Interface;

});