// genrev.js

define(function () {

    /*****************************************
    
    ******************************************/

    var Interface = {
        
        FormatMoney: function(value, places) { return genrev.formatMoney(value, places); },
        FormatPercent: function(value) { return genrev.formatPercent(value); },
        ViewAs: {
            Init: function () { genrev.viewas.init(); },
            Current: function () { return genrev.viewas.current(); }
        },
        Charts: {
            Overlays: {
                GenerateTopRight: function (parentID, top, right, htmlContent) { genrev.charts.overlays.generateTopRight(parentID, top, right, htmlContent); }
            }
        }

    }   // end GenRev

    var genrev = {

        charts: {

            overlays: {

                generateTopRight: function (parentID, top, right, htmlContent) {

                    // generate a div to place in the top right of the specified parent
                    top = top || 0;
                    right = right || 0;

                    var html = '<div style="position: absolute; z-index: 100; top: ' + top + 'px; right: ' + right + 'px;">';
                    html = html + htmlContent + '</div>';

                    $('#' + parentID).append(html);

                }

            }   // end genrev.charts.overlays

        },  // end genrev.charts
                
        viewas: {

            init: function() {

                $.ajax({
                    url: "/MemberBar/ViewAsPopup",
                    type: 'GET',
                    success: function (res) {
                        $("#memberbar-viewas-popup-container").empty().append(res);
                        
                        $(".viewasClickSelectListItem").click(function() {
                            var id = $(this).attr('id').replace("viewasQSL-", '');
                            var name = $(this).text();
                            genrev.viewas.selectionChanged(id, name);
                            DevEx.Controls.GetByName("memberBarViewAsPopup").Hide();
                        });
                    }
                });


            },  // end genrev.viewas.init

            selectionChanged: function(id, name) {

                var returnUrl = window.location.pathname;

                $.ajax({
                    type: 'post',
                    url: '/Internal/UpdateViewContext',
                    data: {
                        contextID: id,
                        returnUrl: returnUrl
                    },
                    success: function (res) {
                        Shell.SetContent(res);
                        $("#va-content-current-name").text(name)
                    },
                    error: function (res) {
                        App.Errors.ShowGeneral();
                    }
                });

            },  // end genrev.viewas.selectionChanged

            current: function () {

            }   // end genrev.viewas.current

        },  // end genrev.viewas

        formatMoney: function (value, places) {

            // assume places is 0 for now
            var n = Number(value);
            return "$" + numberWithCommas(roundAwayFromZero(n));

            function numberWithCommas(x) {
                return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }

            function roundAwayFromZero(x) {
                if (x < 0) {
                    return Math.round(x * -1.0) * -1;
                }
                else {
                    return Math.round(x);
                }
            }

            
        },   // end genrev.formatMoney

        formatPercent: function(value) {

            return parseFloat(value).toFixed(2) + "%";

        }   // end genrev.formatPercent

    }   // end genrev


    window.Genrev = Interface;

});