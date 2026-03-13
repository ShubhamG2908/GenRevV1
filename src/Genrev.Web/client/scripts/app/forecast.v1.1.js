// forecast.js

define(function () {

    var api = {

        Initialize: function () { code.initialize(); }

    }


    var code = {

        selectedYear: function () {
            return $("input[name='ctlYear']").val();
        },

        selectedSalesperson: function () {
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

            DevEx.Controls.GetByName('btnImportForecastData').Click.AddHandler(function (s, e) {
                code.showImportPopup();
            });
        }, // end code.initialize
        getFile: function (e, file) {            
            e.preventDefault();
            var url = '/Data/GetImportTemplate/?templateName=' + file;
            var win = window.open(url, '_blank');
            win.focus();
            return false;
        },
        uploadEvents: {
            fileUploadComplete: function (s, e) {
                if(e.callbackData)
                {
                    App.Warning.Show("File imported with warnings:\n\n" + e.callbackData)
                }
                else if (!e.isValid) {
                    App.Errors.Show("Unable to import file:\n\n" + e.errorText);
                    return;
                } else {
                    App.Alert("Import Complete", DialogIcons.Ok);
                }
            }
        },
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

        },
        showImportPopup: function () {

            App.Popup.Show({
                url: '/Data/OpenImportForecastPopup',
                type: 'GET',
                data: null,
                options: {
                    width: 300,
                    height: 200,
                    title: 'Import Forecast Data',
                    allowDrag: true,
                    allowResize: false
                },
                opened: function () {
                    $('#template-dl-forecastData').click(function (e) { return code.getFile(e, 'forecastData'); });
                    var uploader = DevEx.Controls.GetByName("uploadForecastData");
                    uploader.FileUploadComplete.AddHandler(function (s, e) {
                        App.Popup.Hide('ok');
                        code.uploadEvents.fileUploadComplete(s, e);
                    });
                },
                done: function (r) {
                },
                error: function () {
                    App.Errors.ShowGeneral();
                }

            });

        },
    }
    window.Forecast = api;

});