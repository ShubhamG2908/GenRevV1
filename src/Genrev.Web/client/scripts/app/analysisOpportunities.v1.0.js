// analysisOpportunities.js

define(function () {

    /*****************************************
    
    ******************************************/

    var Interface = {
        
        Initialize: function () { page.initialize(); }

    }   // end Customers.Interface


    var config = {

        mainGridElementID: "opp-grid-main",

        tabContentElementID: "opportunities-tab-content",

        salesperson: {
            data: {
                url: '/Analysis/Opportunities/Data/Salesperson'
            },
            containers: {
                potential: 'an-opp-sp-potential',
                current: 'an-opp-sp-current',
                future: 'an-opp-sp-future'
            }            
        },

        industry: {
            data: {
                url: '/Analysis/Opportunities/Data/Industry'
            },
            containers: {
                potential: 'an-opp-ind-potential',
                current: 'an-opp-ind-current',
                future: 'an-opp-ind-future'
            }
        },

        customerType: {
            data: {
                url: '/Analysis/Opportunities/Data/CustomerType'
            },
            containers: {
                potential: 'an-opp-ct-potential',
                current: 'an-opp-ct-current',
                future: 'an-opp-ct-future'
            }
        },

        accountType: {
            data: {
                url: '/Analysis/Opportunities/Data/AccountType'
            },
            containers: {
                potential: 'an-opp-at-potential',
                current: 'an-opp-at-current',
                future: 'an-opp-at-future'
            }
        },

        customer: {
            data: {
                url: '/Analysis/Opportunities/Data/Customer'
            },
            containers: {
                potential: 'an-opp-cr-potential',
                current: 'an-opp-cr-current',
                future: 'an-opp-cr-future'
            }
        }

    }   // end config


    var page = {

        viewParams: {

            initialize: function() {

                DevEx.Controls.GetByName("an-opp-salespersonCombo").ValueChanged.AddHandler(function () {
                    page.rows.refresh();
                });                
                DevEx.Controls.GetByName("an-opp-yearsToShowCombobox").ValueChanged.AddHandler(function () {
                    page.rows.refresh();
                });
                
            }

        },

        getParams: function() {

            var salesperson = DevEx.Controls.GetValue("an-opp-salespersonCombo");
            var years = DevEx.Controls.GetValue("an-opp-yearsToShowCombobox");

            return {
                salesperson: salesperson,
                years: years
            }

        },  // end page.getParams

        initialize: function() {

            page.ensureMainGridHeight();
            page.viewParams.initialize();
            
            var salesperson = page.rows.salesperson;
            salesperson.load();

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



        charts: {


            tooltipFormatter: function() {

                var s = '<b>' + this.point.name + '</b><br />';
                s += '<hr>'
                s += 'Dollars: ' + Genrev.FormatMoney(this.point.y) + '<br />';
                s += 'Percent: ' + Math.round(this.point.percentage) + '%';

                return s;
            },

            renderPie: function (title, seriesName, seriesData, containerName) {

                var base = Charts.GetBase();
                
                var options = {

                    chart: {
                        type: 'pie',
                        height: 225
                    },
                    title: { text: title },
                    tooltip: {
                        formatter: page.charts.tooltipFormatter
                    },
                    series: [{
                        name: seriesName,
                        data: seriesData
                    }]                    
                }

                var final = merge(base, options);

                $("#" + containerName).highcharts(final);

            }

        },

        
        rows: {


            refresh: function() {
                page.rows.salesperson.load();   // this will call the rest in succession
            },

            salesperson: {

                load: function () {

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.salesperson.data.url,
                        data: {
                            salespersonID: params.salesperson,
                            year: params.years
                        },
                        success: function (res) {

                            page.rows.salesperson.render(res);
                            page.rows.industry.load();
                            
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },

                render: function (data) {

                    data = JSON.parse(data);

                    page.charts.renderPie(
                            'Potential',
                            'Salespersons',
                            data.potential,
                            config.salesperson.containers.potential);

                    page.charts.renderPie(
                            'Current',
                            'Salespersons',
                            data.current,
                            config.salesperson.containers.current);

                    page.charts.renderPie(
                            'Future',
                            'Salespersons',
                            data.future,
                            config.salesperson.containers.future);

                }

            },

            industry: {

                load: function () {

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.industry.data.url,
                        data: {
                            salespersonID: params.salesperson,
                            year: params.years
                        },
                        success: function (res) {
                            page.rows.industry.render(res);
                            page.rows.customerType.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },

                render: function (data) {

                    data = JSON.parse(data);

                    page.charts.renderPie(
                            'Potential',
                            'Industry',
                            data.potential,
                            config.industry.containers.potential);

                    page.charts.renderPie(
                            'Current',
                            'Industry',
                            data.current,
                            config.industry.containers.current);

                    page.charts.renderPie(
                            'Future',
                            'Industry',
                            data.future,
                            config.industry.containers.future);

                }
            },

            customerType: {


                load: function () {

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.customerType.data.url,
                        data: {
                            salespersonID: params.salesperson,
                            year: params.years
                        },
                        success: function (res) {
                            page.rows.customerType.render(res);
                            page.rows.accountType.load();

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },

                render: function (data) {

                    data = JSON.parse(data);

                    page.charts.renderPie(
                            'Potential',
                            'Customer Type',
                            data.potential,
                            config.customerType.containers.potential);

                    page.charts.renderPie(
                            'Current',
                            'Customer Type',
                            data.current,
                            config.customerType.containers.current);

                    page.charts.renderPie(
                            'Future',
                            'Customer Type',
                            data.future,
                            config.customerType.containers.future);

                }
            },

            accountType: {

                load: function () {

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.accountType.data.url,
                        data: {
                            salespersonID: params.salesperson,
                            year: params.years
                        },
                        success: function (res) {
                            page.rows.accountType.render(res);
                            page.rows.customer.load();
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },

                render: function (data) {

                    data = JSON.parse(data);

                    page.charts.renderPie(
                            'Potential',
                            'Account Type',
                            data.potential,
                            config.accountType.containers.potential);

                    page.charts.renderPie(
                            'Current',
                            'Account Type',
                            data.current,
                            config.accountType.containers.current);

                    page.charts.renderPie(
                            'Future',
                            'Account Type',
                            data.future,
                            config.accountType.containers.future);

                }
            },

            customer: {

                load: function () {

                    var params = page.getParams();

                    $.ajax({
                        type: 'get',
                        url: config.customer.data.url,
                        data: {
                            salespersonID: params.salesperson,
                            year: params.years
                        },
                        success: function (res) {
                            page.rows.customer.render(res);
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },

                render: function (data) {

                    data = JSON.parse(data);

                    page.charts.renderPie(
                        'Potential',
                        'Customer',
                        data.potential,
                        config.customer.containers.potential);

                    page.charts.renderPie(
                        'Current',
                        'Customer',
                        data.current,
                        config.customer.containers.current);

                    page.charts.renderPie(
                        'Future',
                        'Customer',
                        data.future,
                        config.customer.containers.future);

                }
            }

        }

    }   // end page
    

    window.AnalysisOpportunities = Interface;

});