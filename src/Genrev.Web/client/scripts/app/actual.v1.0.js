define(function () {
    var interface = {
        Initialize: function () { code.init(); }
    }

    var code = {
        init: function () {
            $.ajax({
                type: 'get',
                url: '/Data/ActualMatrix',
                success: function (res) {
                    $("#data-actual-matrix-container").empty().append(res);
                    code.grid.initialize();
                },
                error: function () {
                    App.Error.ShowGeneral();
                }
            });
        },

        grid: {
            initialize: function () {

            }
        }
    }

    window.Actual = interface;
});