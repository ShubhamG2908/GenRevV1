// owner.js

define(function () {

    /*****************************************

        Data.Initialize()
        

    ******************************************/

    var Interface = {

        Initialize: function () { data.initialize(); },
        Matrix: {
            Grid: {
                Initialized: function (value) { console.log('dx initializing'); data.matrix.grid.isInitializedByDevEx = value; },
                ContextMenu: {
                    Invoked: function (s, e) { data.matrix.grid.contextMenu.invoked(s, e); },
                    ItemClicked: function (s, e) { data.matrix.grid.contextMenu.itemClick(s, e); }
                }
            }
        }
        
    }   // end Data.Interface









    var data = {

        initialize: function () {

            data.matrix.init();
            
        },   // end data.initialize


        matrix: {

            init: function () {

                data.matrix.grid.initialize();

            },   // end data.matrix.init

            grid: {

                setEditVisualState: function() {
                    // what do? I had this in here, not sure what for...
                },

                selectedYear: function() {
                    var x = $("input[name='ctlYear']").val();
                    console.log(x);
                    return x;
                },

                events: {

                    addBeginCallbackHandler: function(grid) {

                        grid.BeginCallback.AddHandler(function (s, e) {
                            e.customArgs["selectedYear"] = data.matrix.grid.selectedYear();
                            e.customArgs["showClassification"] = DevEx.Controls.GetValue("ShowClassification");
                            e.customArgs["showSales"] = DevEx.Controls.GetValue("ShowSales");
                            e.customArgs["showGPD"] = DevEx.Controls.GetValue("ShowGPD");
                            e.customArgs["showGPP"] = DevEx.Controls.GetValue("ShowGPP");
                            e.customArgs["showCalls"] = DevEx.Controls.GetValue("ShowCalls");
                        });
                    },   // end data.matrix.grid.events.addBeginCallbackHandler

                    

                    addColumnSelectorsHandlers: function(grid) {
                    
                        DevEx.Controls.GetByName("ShowClassification").ValueChanged.AddHandler(function (s, e) {
                            grid.PerformCallback();
                        });

                        DevEx.Controls.GetByName("ShowSales").ValueChanged.AddHandler(function (s, e) {
                            grid.PerformCallback();
                        });

                        DevEx.Controls.GetByName("ShowGPD").ValueChanged.AddHandler(function (s, e) {
                            grid.PerformCallback();
                        });

                        DevEx.Controls.GetByName("ShowGPP").ValueChanged.AddHandler(function (s, e) {
                            grid.PerformCallback();
                        });

                        DevEx.Controls.GetByName("ShowCalls").ValueChanged.AddHandler(function (s, e) {
                            grid.PerformCallback();
                        });
                        
                    },

                    addEndCallbackHandler: function(grid) {

                        grid.EndCallback.AddHandler(function (s, e) {
                            
                            var events = data.matrix.grid.events;

                            events.addYearSelectorChangedHandler(grid);
                            events.addColumnSelectorsHandlers(grid);
                            
                            //events.addAddRowClickHandler(grid);
                            //events.addViewModeChangedHandler(grid);

                            //var filterControl = grid.GetFilterControl();
                            //var oldParams = filterControl.GetCallbackParams;

                            //filterControl.GetCallbackParams = function () {

                            //    var params = oldParams.apply(filterControl);

                            //    if (!grid.BeginCallback.IsEmpty()) {
                            //        var args = new MVCxClientBeginCallbackEventArgs();
                            //        grid.BeginCallback.FireEvent(grid, args);
                            //        for (var key in args.customArgs) {
                            //            params[key] = args.customArgs[key];
                            //        }
                            //    }

                            //    return params;

                            //}

                            moveFooterSums();

                        });

                    },

                    addWindowResizeStretchGridHandler(grid) {

                        window.addEventListener("resize", function (event) {
                            data.matrix.grid.stretchHeight(grid);
                        });

                    },

                    addYearSelectorChangedHandler: function (grid) {

                        DevEx.Controls.GetByName("ctlYear").ValueChanged.AddHandler(function (s, e) {
                            grid.PerformCallback();
                        });

                    },
                    
                    addViewModeChangedHandler: function (grid) {

                        //DevEx.Controls.GetByName("ctlViewMode").ValueChanged.AddHandler(function (s, e) {
                        //    grid.PerformCallback();
                        //});
                    }


                },  // end data.matrix.grid.events
                
                initialize: function () {
                    
                    console.log('initializing drilldown grid');

                    var grid = DevEx.Controls.GetByName("DrilldownGrid");
                    var events = data.matrix.grid.events;

                    data.matrix.grid.stretchHeight(grid);

                    events.addBeginCallbackHandler(grid);
                    events.addEndCallbackHandler(grid);
                    events.addWindowResizeStretchGridHandler(grid);
                    events.addYearSelectorChangedHandler(grid);
                    events.addColumnSelectorsHandlers(grid);
                    //events.addViewModeChangedHandler(grid);
                },   // end data.matrix.grid.initialize


                contextMenu: {

                    lastClickedIndex: null,

                    invoked: function(s, e) {
                        if (e.objectType != "row") {
                            e.showBrowserMenu = true;
                        } else {
                            data.matrix.grid.contextMenu.lastClickedIndex = e.index;
                        }
                    },  // end data.matrix.grid.contextMenu.invoked

                    itemClick: function (s, e) {
                        //e.item.name = ...

                        var grid = DevEx.Controls.GetByName("ManagementMatrixGrid");
                        var index = data.matrix.grid.contextMenu.lastClickedIndex;
                        var key = grid.GetRowKey(index);
                        data.matrix.grid.deleteRow(index, key);

                    }   // end data.matrix.grid.contextMenu.itemClick

                },  // end data.matrix.grid.contextMenu

                stretchHeight: function (grid) {

                    var splitter = DevEx.Controls.GetByName("ContentSplitter");
                    var h = splitter.GetPane(1).GetClientHeight();
                    var outerHeight = $("#drilldown-grid-headers").outerHeight(true);
                    var buffer = 20;
                    
                    h = h - buffer - outerHeight;
                    
                    grid.SetHeight(h);

                    moveFooterSums();

                },   // end data.matrix.grid.stretchHeight

            },  // end data.matrix.grid


            load: function () {

                //$.ajax({
                //    type: 'get',
                //    url: '/Data/ManagementMatrix',
                //    success: function (res) {
                //        $("#datamanagement-matrix-container").empty().append(res);
                //        data.matrix.grid.initialize();
                //    },
                //    error: function () {
                //        App.Error.ShowGeneral();
                //    }
                //});

            }   // end data.matrix.load

        }   // end data.matrix

    }   // end data

    window.Drilldown = Interface;

});

function moveFooterSums() {
    var headTable = $('#DrilldownGrid_DXHeaderTable:first-child');
    var footerRow = $('#DrilldownGrid_DXFooterRow');
    if (footerRow && headTable) {
        $('#DrilldownGrid_DXFooterRow td').text(
            function (ndx, text) {
                var newText = text
                    .replace("Sum=", "")
                    .replace("Avg=", "");
                return newText;
            }
        );
        headTable.append(footerRow);
    }
}