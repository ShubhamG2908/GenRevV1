// forecast.js

define(function () {

    var api = {

        Initialize: function () { code.initialize(); }

    }


    var code = {

        selectedYear: function() {
            return $("input[name='ctlYear']").val();
        },

        selectedSalesperson: function() {
            return DevEx.Controls.GetValue("ctlSalesperson");
        },


        initialize: function () {

            code.loadData(code.selectedYear(), code.selectedSalesperson());
            
            DevEx.Controls.GetByName("ctlYear").ValueChanged.AddHandler(function (s, e) {
                code.loadData(code.selectedYear(), code.selectedSalesperson());
            });

            DevEx.Controls.GetByName("ctlSalesperson").ValueChanged.AddHandler(function (s, e) {
                code.loadData(code.selectedYear(), code.selectedSalesperson());
            });

        }, // end code.initialize


        loadData: function (year, salespersonID) {

            // reload yearly plan
            $.ajax({
                type: 'GET',
                url: '/Data/ForecastPlanByYear',
                data: {
                    year: year,
                    salespersonID: salespersonID
                },
                success: function (res) {
                    $('#data-forecast-plan-by-year').empty().append(res);
                    window.setTimeout(function () { code.initPlanByYearGrid(); }, 0);
                }
            });

            // reload forecast list
            $.ajax({
                type: 'GET',
                url: '/Data/ForecastDetailByYear',
                data: {
                    year: year,
                    personnelID: salespersonID
                },
                success: function (res) {
                    $('#data-forecast-detail-by-year').empty().append(res);
                    window.setTimeout(function () { code.initDetailByYearGrid(); }, 0);
                }
            });


            // reload yearly detail

        },

        initPlanByYearGrid: function () {

            var grid = DevEx.Controls.GetByName("ForecastPlanByYearGrid");

            grid.BeginCallback.AddHandler(function (s, e) {
                e.customArgs["year"] = code.selectedYear();
                e.customArgs["salespersonID"] = code.selectedSalesperson();
            });

        },

        initDetailByYearGrid: function () {

            var grid = DevEx.Controls.GetByName("ForecastDetailByYearGrid");

            grid.BeginCallback.AddHandler(function (s, e) {
                e.customArgs["year"] = code.selectedYear();
                e.customArgs["personnelID"] = code.selectedSalesperson();
            });
                       
        }



    }

    window.Forecast = api;

});