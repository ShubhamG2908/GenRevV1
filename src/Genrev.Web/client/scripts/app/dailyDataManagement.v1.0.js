// owner.js

define(function () {

    /*****************************************
     * 
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
        },
        Upload: {
            Initialize: function () { data.upload.initialize(); }
        }
        
    }   // end Data.Interface

    var data = {

        initialize: function () {            
            data.matrix.init();
            
        },   // end data.initialize



        upload: {

            initialize: function () {

                data.upload.descriptions.initialize();

                //var uploader = DevEx.Controls.GetByName("uploadCompanies");
                //uploader.FileUploadComplete.AddHandler(function (s, e) { data.upload.uploadEvents.fileUploadComplete(s, e); });
                
                var uploader = DevEx.Controls.GetByName("uploadPersonnel");
                uploader.FileUploadComplete.AddHandler(function (s, e) { data.upload.uploadEvents.fileUploadComplete(s, e); });

                uploader = DevEx.Controls.GetByName("uploadAccountTypes");
                uploader.FileUploadComplete.AddHandler(function (s, e) { data.upload.uploadEvents.fileUploadComplete(s, e); });

                uploader = DevEx.Controls.GetByName("uploadCustomerTypes");
                uploader.FileUploadComplete.AddHandler(function (s, e) { data.upload.uploadEvents.fileUploadComplete(s, e); });

                uploader = DevEx.Controls.GetByName("uploadIndustryTypes");
                uploader.FileUploadComplete.AddHandler(function (s, e) { data.upload.uploadEvents.fileUploadComplete(s, e); });

                uploader = DevEx.Controls.GetByName("uploadCustomers");
                uploader.FileUploadComplete.AddHandler(function (s, e) { data.upload.uploadEvents.fileUploadComplete(s, e); });

                uploader = DevEx.Controls.GetByName("uploadMonthlyData");
                uploader.FileUploadComplete.AddHandler(function (s, e) { data.upload.uploadEvents.fileUploadComplete(s, e); });

                $('#template-dl-personnel').click(function (e) { return getFile(e, 'personnel'); });
                $('#template-dl-accountTypes').click(function (e) { return getFile(e, 'accountTypes'); });
                $('#template-dl-customerTypes').click(function (e) { return getFile(e, 'customerTypes'); });
                $('#template-dl-industryTypes').click(function (e) { return getFile(e, 'industryTypes'); });
                $('#template-dl-customers').click(function (e) { return getFile(e, 'customers'); });
                $('#template-dl-monthlyData').click(function (e) { return getFile(e, 'monthlyData'); });

                function getFile(e, file) {
                    e.preventDefault();
                    var url = '/Data/GetImportTemplate/?templateName=' + file;
                    var win = window.open(url, '_blank');
                    win.focus();
                    return false;
                }
                
            },   // end data.upload.initialize

            descriptions: {
                initialize: function () {

                    $('[data-dym-description-link]').click(function (e) {
                        e.preventDefault();
                        var item = $(this).attr('data-dym-description-link');
                        $('[data-dym-description-container="' + item + '"').toggle("slow");
                        return false;
                    })

                }
            },

            uploadEvents: {

                fileUploadComplete: function (s, e) {
                    if (!e.isValid) {
                        App.Errors.Show("Unable to import file:\n\n" + e.errorText);
                        return;
                    } else {
                        App.Alert("Import Complete", DialogIcons.Ok);
                    }
                }
            },

        },  // end data.upload


        matrix: {

            init: function () {

                data.matrix.load();

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
                            console.log('beginning callback: ' + DevEx.Controls.GetValue("ctlYear"));
                            console.log('SELECTED YEAR: ' + data.matrix.grid.selectedYear());
                            e.customArgs["selectedYear"] = data.matrix.grid.selectedYear();
                            e.customArgs["viewMode"] = DevEx.Controls.GetValue("ctlViewMode");
                        });
                    },   // end data.matrix.grid.events.addBeginCallbackHandler

                    addBatchEditEndingHandler: function(grid) {
                        grid.BatchEditEndEditing.AddHandler(function (s, e) {
                            console.log('edit ending');
                            data.matrix.grid.setEditVisualState();
                        });
                    },

                    addEndCallbackHandler: function(grid) {

                        grid.EndCallback.AddHandler(function (s, e) {
                            
                            var events = data.matrix.grid.events;

                            events.addYearSelectorChangedHandler(grid);
                            events.addCancelChangesClickHandler(grid);
                            events.addSaveChangesClickHandler(grid);
                            events.addAddRowClickHandler(grid);
                            events.addViewModeChangedHandler(grid);

                            var headTable = $('#DailyManagementMatrixGrid_DXHeaderTable:first-child');
                            var footerRow = $('#DailyManagementMatrixGrid_DXFooterRow');
                            if (footerRow && headTable) {
                                $('#DailyManagementMatrixGrid_DXFooterRow td').text(
                                    function (ndx, text) {
                                        return text.replace("Sum=", "");
                                    }
                                );
                                headTable.append(footerRow);
                            }
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

                    addCancelChangesClickHandler: function (grid) {

                        DevEx.Controls.GetByName("btnCancelChanges").Click.AddHandler(function (s, e) {
                            App.Confirm("Are you sure you want to cancel your changes?", function () {
                                grid.CancelEdit();
                                // remove custom visual state for deleted grid items
                                $("tr[id^=DailyManagementMatrixGrid_DXDataRow]").removeClass("grid-row-deleted");
                                data.matrix.grid.setEditVisualState();
                            });
                        });

                    },

                    addSaveChangesClickHandler: function (grid) {
                        DevEx.Controls.GetByName("btnSaveChanges").Click.AddHandler(function (s, e) {
                            grid.UpdateEdit();
                            data.matrix.grid.setEditVisualState();
                        });
                    },

                    addAddRowClickHandler: function (grid) {
                        //DevEx.Controls.GetByName("btnAddNew").Click.AddHandler(function (s, e) {
                        //    grid.AddNewRow();
                        //    data.matrix.grid.setEditVisualState();
                        //});
                    },

                    addViewModeChangedHandler: function (grid) {
                        DevEx.Controls.GetByName("ctlViewMode").ValueChanged.AddHandler(function (s, e) {
                            grid.PerformCallback();
                        });
                    }


                },  // end data.matrix.grid.events
                
                initialize: function () {
                    
                    var grid = DevEx.Controls.GetByName("DailyManagementMatrixGrid");
                    var events = data.matrix.grid.events;

                    data.matrix.grid.stretchHeight(grid);

                    events.addBeginCallbackHandler(grid);
                    events.addBatchEditEndingHandler(grid);
                    events.addEndCallbackHandler(grid);
                    events.addWindowResizeStretchGridHandler(grid);
                    events.addYearSelectorChangedHandler(grid);
                    events.addCancelChangesClickHandler(grid);
                    events.addSaveChangesClickHandler(grid);
                    events.addAddRowClickHandler(grid);
                    events.addViewModeChangedHandler(grid);

                },   // end data.matrix.grid.initialize

                deleteRow: function(rowIndex, rowID) {

                    App.Confirm("Are you sure you want to permenantly delete this row?", function () {
                        var grid = DevEx.Controls.GetByName("DailyManagementMatrixGrid");
                        grid.DeleteRowByKey(rowID);
                        $("#DailyManagementMatrixGrid_DXDataRow" + rowIndex).addClass("grid-row-deleted").show();
                    });

                },  // end data.matrix.grid.deleteRow

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

                        var grid = DevEx.Controls.GetByName("DailyManagementMatrixGrid");
                        var index = data.matrix.grid.contextMenu.lastClickedIndex;
                        var key = grid.GetRowKey(index);
                        data.matrix.grid.deleteRow(index, key);

                    }   // end data.matrix.grid.contextMenu.itemClick

                },  // end data.matrix.grid.contextMenu

                stretchHeight: function (grid) {

                    var splitter = DevEx.Controls.GetByName("ContentSplitter");
                    var h = splitter.GetPane(1).GetClientHeight();
                    var outerHeight = $("#daily-data-management-heading").outerHeight(true);
                    var buffer = 10;
                    
                    h = h - buffer - outerHeight;
                    
                    grid.SetHeight(h);

                }   // end data.matrix.grid.stretchHeight

            },  // end data.matrix.grid


            load: function () {

                console.log('loading grid');

                $.ajax({
                    type: 'get',
                    url: '/Data/DailyManagementMatrix',
                    success: function (res) {
                        $("#daily-datamanagement-matrix-container").empty().append(res);
                        data.matrix.grid.initialize();
                    },
                    error: function () {
                        App.Error.ShowGeneral();
                    }
                });

            }   // end data.matrix.load

        }   // end data.matrix

    }   // end data


    window.DailyData = Interface;

});