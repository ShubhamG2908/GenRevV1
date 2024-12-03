// dashboard.js

define(function () {

    /*****************************************
    
    ******************************************/

    var Interface = {
        
        Initialize: function () { page.initialize(); }

    }   // end Customers.Interface


    var config = {

        mainGridElementID: "hs-grid-main",

        tabContentElementID: "historic-tab-content",

        salesperson: {
            page: {
                url: '/Analysis/Historic/Page/Salesperson'
            },
            dollars: {
                containerName: 'an-his-sp-dollars',
                dataUrl: '/Analysis/Historic/ChartData/Sales'
            },
            gp: {
                containerName: 'an-his-sp-gp',
                dataUrl: '/Analysis/Historic/ChartData/GP'
            }
        },
        industry: {
            page: { url: '/Analysis/Historic/Page/Industry' },
            dollars: {
                containerName: 'an-his-ind-dollars',
                dataUrl: '/Analysis/Historic/ChartData/Industry/Sales'
            },
            gp: {
                containerName: 'an-his-ind-gp',
                dataUrl: '/Analysis/Historic/ChartData/Industry/GP'
            }
        },
        customerType: {
            page: { url: '/Analysis/Historic/Page/CustomerType' },
            dollars: {
                containerName: 'an-his-customerType-dollars',
                dataUrl: '/Analysis/Historic/ChartData/CustomerType/Sales'
            },
            gp: {
                containerName: 'an-his-customerType-gp',
                dataUrl: '/Analysis/Historic/ChartData/CustomerType/GP'
            }
        },
        accountType: {
            page: { url: '/Analysis/Historic/Page/AccountType' },
            dollars: {
                containerName: 'an-his-accountType-dollars',
                dataUrl: '/Analysis/Historic/ChartData/AccountType/Sales'
            },
            gp: {
                containerName: 'an-his-accountType-gp',
                dataUrl: '/Analysis/Historic/ChartData/AccountType/GP'
            }
        },
        product: {
            page: { url: '/Analysis/Historic/Page/Product' },
            dollars: {
                containerName: 'an-his-product-dollars',
                dataUrl: '/Analysis/Historic/ChartData/Product/Sales'
            },
            gp: {
                containerName: 'an-his-product-gp',
                dataUrl: '/Analysis/Historic/ChartData/Product/GP'
            }
        },
        customer: {
            page: { url: '/Analysis/Historic/Page/Customer' },
            dollars: {
                containerName: 'an-his-customer-dollars',
                dataUrl: '/Analysis/Historic/ChartData/Customer/Sales'
            },
            gp: {
                containerName: 'an-his-customer-gp',
                dataUrl: '/Analysis/Historic/ChartData/Customer/GP'
            }
        }

    }   // end config


    var page = {

        viewParams: {

            initialize: function() {

                DevEx.Controls.GetByName("an-hs-salespersonCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-hs-industriesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-hs-customerTypesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-hs-accountTypesCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-hs-productsCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-hs-customersCombo").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                DevEx.Controls.GetByName("an-hs-yearsToShowCombobox").ValueChanged.AddHandler(function () {
                    page.tabs.refreshCurrent();
                });
                
            }

        },

        getParams: function() {

            var salesperson = DevEx.Controls.GetValue("an-hs-salespersonCombo");
            var industry = DevEx.Controls.GetValue("an-hs-industriesCombo");
            var customerType = DevEx.Controls.GetValue("an-hs-customerTypesCombo");
            var accountType = DevEx.Controls.GetValue("an-hs-accountTypesCombo");
            var product = DevEx.Controls.GetValue("an-hs-productsCombo");
            var customer = DevEx.Controls.GetValue("an-hs-customersCombo"); 
            var years = DevEx.Controls.GetValue("an-hs-yearsToShowCombobox");

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

                $("[data-dym-selector='an-hs']").click(function (event) {

                    console.log('tab clicked');

                    var selection = $(this).attr('data-dym-value');
                    var selectionClass = $(this).attr('class');

                    if (selectionClass === 'selected') {
                        return;
                    }
                    else {
                        ts.setSelection(selection);
                    }

                });

            },   // end page.tabSelectors.initialize

            setSelection: function (selection) {

                var oldID = "#an-hs-selector-" + page.tabSelectors.current;
                var newID = "#an-hs-selector-" + selection;

                if (selection !== 'salesperson') {
                    $("#an-hs-selector-salesperson").removeClass("selected");
                }

                $(oldID).removeClass("selected");
                $(newID).addClass("selected");

                page.tabSelectors.current = selection;

                var tabs = page.tabs;

                switch (selection) {
                    case 'salesperson':
                        $("#an-hs-salespersonComboContainer").show();
                        $("#an-hs-industryComboContainer").hide();
                        $("#an-hs-customerTypeComboContainer").hide();
                        $('#an-hs-accountTypeComboContainer').hide();
                        $('#an-hs-productComboContainer').hide();
                        $('#an-hs-customerComboContainer').hide();
                        tabs.salesperson.loadPage();
                        break;
                    case 'industry':
                        $("#an-hs-salespersonComboContainer").hide();
                        $("#an-hs-industryComboContainer").show();
                        $("#an-hs-customerTypeComboContainer").hide();
                        $('#an-hs-accountTypeComboContainer').hide();
                        $('#an-hs-productComboContainer').hide();
                        $('#an-hs-customerComboContainer').hide();
                        tabs.industry.loadPage();
                        break;
                    case 'customerType':
                        $("#an-hs-salespersonComboContainer").hide();
                        $("#an-hs-industryComboContainer").hide();
                        $("#an-hs-customerTypeComboContainer").show();
                        $('#an-hs-accountTypeComboContainer').hide();
                        $('#an-hs-productComboContainer').hide();
                        $('#an-hs-customerComboContainer').hide();
                        tabs.customerType.loadPage();
                        break;
                    case 'accountType':
                        $("#an-hs-salespersonComboContainer").hide();
                        $("#an-hs-industryComboContainer").hide();
                        $("#an-hs-customerTypeComboContainer").hide();
                        $('#an-hs-accountTypeComboContainer').show();
                        $('#an-hs-productComboContainer').hide();
                        $('#an-hs-customerComboContainer').hide();
                        tabs.accountType.loadPage();
                        break;
                    case 'product':
                        $("#an-hs-salespersonComboContainer").hide();
                        $("#an-hs-industryComboContainer").hide();
                        $("#an-hs-customerTypeComboContainer").hide();
                        $('#an-hs-accountTypeComboContainer').hide();
                        $('#an-hs-productComboContainer').show();
                        $('#an-hs-customerComboContainer').hide();
                        tabs.product.loadPage();
                        break;
                    case 'customer':
                        $("#an-hs-salespersonComboContainer").hide();
                        $("#an-hs-industryComboContainer").hide();
                        $("#an-hs-customerTypeComboContainer").hide();
                        $('#an-hs-accountTypeComboContainer').hide();
                        $('#an-hs-productComboContainer').hide();
                        $('#an-hs-customerComboContainer').show();
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

                                            var d = this.points[0].y;
                                            
                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>Gross Sales:</b> ' + Genrev.FormatMoney(d) + '<br />';
                                            
                                            return s;
                                        }
                                    },
                                    title: { text: 'Sales Historic (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Gross Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.salesperson.dollars.containerName).highcharts(final);

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

                                            var gpd = this.points[0].y;
                                            var gpp = this.points[1].y;
                                            
                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD:</b> ' + Genrev.FormatMoney(gpd) + '<br />';
                                            s = s + '<b>GPP:</b> ' + Genrev.FormatPercent(gpp);
                                            
                                            return s;
                                        }
                                    },

                                    title: { text: 'Sales Historic (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.salesperson.gp.containerName).highcharts(final);

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

                                            var d = this.points[0].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>Gross Sales:</b> ' + Genrev.FormatMoney(d) + '<br />';

                                            return s;
                                        }
                                    },
                                    title: { text: 'Sales Historic (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Gross Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.industry.dollars.containerName).highcharts(final);

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

                                            var gpd = this.points[0].y;
                                            var gpp = this.points[1].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD:</b> ' + Genrev.FormatMoney(gpd) + '<br />';
                                            s = s + '<b>GPP:</b> ' + Genrev.FormatPercent(gpp);

                                            return s;
                                        }
                                    },

                                    title: { text: 'Sales Historic (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.industry.gp.containerName).highcharts(final);

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

                                            var d = this.points[0].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>Gross Sales:</b> ' + Genrev.FormatMoney(d) + '<br />';

                                            return s;
                                        }
                                    },
                                    title: { text: 'Sales Historic (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Gross Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customerType.dollars.containerName).highcharts(final);

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

                                            var gpd = this.points[0].y;
                                            var gpp = this.points[1].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD:</b> ' + Genrev.FormatMoney(gpd) + '<br />';
                                            s = s + '<b>GPP:</b> ' + Genrev.FormatPercent(gpp);

                                            return s;
                                        }
                                    },

                                    title: { text: 'Sales Historic (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customerType.gp.containerName).highcharts(final);

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

                                            var d = this.points[0].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>Gross Sales:</b> ' + Genrev.FormatMoney(d) + '<br />';

                                            return s;
                                        }
                                    },
                                    title: { text: 'Sales Historic (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Gross Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.accountType.dollars.containerName).highcharts(final);

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

                                            var gpd = this.points[0].y;
                                            var gpp = this.points[1].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD:</b> ' + Genrev.FormatMoney(gpd) + '<br />';
                                            s = s + '<b>GPP:</b> ' + Genrev.FormatPercent(gpp);

                                            return s;
                                        }
                                    },

                                    title: { text: 'Sales Historic (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.accountType.gp.containerName).highcharts(final);

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

                                            var d = this.points[0].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>Gross Sales:</b> ' + Genrev.FormatMoney(d) + '<br />';

                                            return s;
                                        }
                                    },
                                    title: { text: 'Sales Historic (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Gross Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.product.dollars.containerName).highcharts(final);

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

                                            var gpd = this.points[0].y;
                                            var gpp = this.points[1].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD:</b> ' + Genrev.FormatMoney(gpd) + '<br />';
                                            s = s + '<b>GPP:</b> ' + Genrev.FormatPercent(gpp);

                                            return s;
                                        }
                                    },

                                    title: { text: 'Sales Historic (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.product.gp.containerName).highcharts(final);

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

                                            var d = this.points[0].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>Gross Sales:</b> ' + Genrev.FormatMoney(d) + '<br />';

                                            return s;
                                        }
                                    },
                                    title: { text: 'Sales Historic (Dollars)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: {
                                        title: { text: 'Gross Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    },
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customer.dollars.containerName).highcharts(final);

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

                                            var gpd = this.points[0].y;
                                            var gpp = this.points[1].y;

                                            var s = this.x + '<br /><hr />';
                                            s = s + '<b>GPD:</b> ' + Genrev.FormatMoney(gpd) + '<br />';
                                            s = s + '<b>GPP:</b> ' + Genrev.FormatPercent(gpp);

                                            return s;
                                        }
                                    },

                                    title: { text: 'Sales Historic (GPP & GPD)' },
                                    xAxis: {
                                        categories: data.categories
                                    },
                                    yAxis: [{
                                        title: { text: 'Gross Profit Dollars' },
                                        labels: {
                                            formatter: function () {
                                                return "$" + this.axis.defaultLabelFormatter.call(this);
                                            }
                                        }
                                    }, {
                                        title: { text: 'Gross Profit Percent' },
                                        labels: {
                                            format: "{value}%"
                                        },
                                        opposite: true
                                    }],
                                    series: data.series
                                }

                                var final = merge(base, options);

                                $('#' + config.customer.gp.containerName).highcharts(final);

                            }
                        });

                    }   // end page.tabs.customer.gp.load

                }   // end page.tabs.customer.gp

            }   // end page.tabs.customer


        }   // end page.tabs

    }   // end page
    

    window.AnalysisHistoric = Interface;

});