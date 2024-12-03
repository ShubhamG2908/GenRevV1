// personnel.js

define(function () {

    /*****************************************

        Personnel.Overview.Initialize(overviewGridName, reportsToGridContainerID, [callback])
        Personnel.Overview.ReportsToGrid.Refresh()

        
    ******************************************/

    var Interface = {

        Overview: {

            AddNew: function() {
                personnel.overview.addNew.showPopup();
            },

            ReportsToGrid: {
                Refresh: function () { personnel.overview.reportsToGrid.refresh(); }
            },

            CurrentPersonnel:{
                Initialize: function (s, e) { return currentPersonnel.initialize(s, e);}
            },

            Availability: {
                Close: function() { personnel.avail.close(); },
                Grid: {
                    ContextMenu: {
                        Invoked: function (s, e) { personnel.avail.menu.invoked(s, e); },
                        ItemClicked: function (s, e) { personnel.avail.menu.itemClick(s, e); }
                    },
                    Initialized: function () { personnel.avail.initialize(); }
                }

            },

            Initialize: function (
                overviewGridControlName,
                reportsToGridContainerID,
                callback) {

                    personnel.overview.initialize(
                        overviewGridControlName,
                        reportsToGridContainerID,
                        callback);

            },  // end Personnel.Overview.Initialize
        }
        
    }   // end Personnel.Interface

    var currentPersonnel = {

        initialize: function(){
            console.log('initializing currentPersonnel Grid');

            var dxGrid = DevEx.Controls.GetByName("personnelmgmtGrid");

            $("[data-dym-cellClick='gvPersonnel']").dblclick(function () {

                var cell = $(this);
                var index = cell.attr("data-dym-visibleIndex");
                var field = cell.attr("data-dym-fieldName");
                var id = dxGrid.GetRowKey(index);

                if (field == "IsAdmin") {
                    $.ajax({
                        type: 'POST',
                        url: '/Personnel/ToggleAdmin',
                        data: {
                            personnelID: id
                        },
                        success: function(res){
                            if(res == "ok"){
                                dxGrid.Refresh();
                            }else{
                                App.Errors.Show(res);
                            }
                        }
                    });
                }
            });

        }
    }

    var personnel = {


        avail: {

            close: function() {

                var grid = DevEx.Controls.GetByName("AvailabilityPopupGrid");

                if (grid.batchEditApi.HasChanges()) {

                    App.Confirm("There are pending changes - close anyway?", function () {
                        App.Popup.Hide();
                    });

                } else {
                    App.Popup.Hide();
                }

            },

            menu: {

                lastClickedIndex: null,

                invoked: function (s, e) {
                    if (e.objectType != "row") {
                        e.showBrowserMenu = true;
                    } else {
                        personnel.avail.menu.lastClickedIndex = e.index;
                    }
                },

                itemClick: function (s, e) {

                    var grid = DevEx.Controls.GetByName("AvailabilityPopupGrid");
                    var index = personnel.avail.menu.lastClickedIndex;
                    var key = grid.GetRowKey(index);
                    personnel.avail.deleteItem(index, key);

                }

            },

            deleteItem: function(rowIndex, rowID) {
                App.Confirm("Are you sure you want to permenantly delete this row?", function () {
                    var grid = DevEx.Controls.GetByName("AvailabilityPopupGrid");
                    grid.DeleteRowByKey(rowID);
                    $("#AvailabilityPopupGrid_DXDataRow" + rowIndex).addClass("grid-row-deleted").show();                    
                });
            },

            initialize: function () {
                var grid = DevEx.Controls.GetByName("AvailabilityPopupGrid");

                grid.BeginCallback.AddHandler(function (s, e) {
                    e.customArgs["personnelID"] = personnel.overview.currentID();
                });

                $('#availabilitygrid-add').click(function (e) {
                    e.preventDefault();
                    grid.AddNewRow();
                    return false;
                });

                $('#availabilitygrid-save').click(function (e) {
                    e.preventDefault();
                    grid.UpdateEdit();
                    return false;
                });

                $('#availabilitygrid-cancel').click(function (e) {
                    e.preventDefault();
                    grid.CancelEdit();
                    $("tr[id^=AvailabilityPopupGrid_DXDataRow]").removeClass("grid-row-deleted");
                    return false;
                });

            }


        },

        overview: {


            editAvailability: function(id) {

                App.Popup.Show({
                    url: '/Personnel/AvailabilityPopup',
                    type: 'GET',
                    data: { personnelID: id },
                    options: {
                        width: 700,
                        height: 380,
                        title: 'Time Management',
                        allowDrag: true,
                        allowResize: true                
                    },
                    opened: function () {
                        // initialize popup
                        

                    },
                    done: function (r) {

                    },
                    error: function (r) {
                        App.Errors.ShowGeneral();
                    }

                });

            },  // end personnel.overview.editAvailability


            setSelection: function(id) {

                var ns = personnel.overview;

                ns.loginInfo.setSelection(id, false);
                ns.reportsToGrid.setSelection(id);

            },

            loginInfo: {

                elementID: null,
                withLoginElementID: null,
                withoutLoginElementID: null,

                update: function() {

                    var email = DevEx.Controls.GetByName("EditLoginInfoEmail").GetValue();
                    var displayName = DevEx.Controls.GetByName("EditLoginInfoDisplayName").GetValue();
                    var password = DevEx.Controls.GetByName("EditLoginInfoPassword").GetValue();

                    // update
                    $.ajax({
                        type: 'POST',
                        url: '/Personnel/UpdateLogin',
                        data: {
                            personnelID: personnel.overview.mainGrid.currentID(),
                            email: email,
                            displayName: displayName,
                            password: password
                        },
                        success: function (res) {

                            if (res == "ok") {
                                personnel.overview.loginInfo.setSelection(personnel.overview.mainGrid.currentID(), true);
                                return;
                            }
                            
                            if (res.substr(0, 3) == "err") {
                                App.Errors.Show(res.substr(4));
                            } else {
                                App.Errors.ShowGeneral();
                            }

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }

                    });

                },

                remove: function() {

                    // remove via personnel id
                    $.ajax({
                        type: 'POST',
                        url: '/Personnel/RemoveLogin',
                        data: {
                            personnelID: personnel.overview.mainGrid.currentID()
                        },
                        success: function (res) {
                            // check for errors, update UI accordingly
                            if (res.substr(0, 4) == "err:") {
                                App.Errors.Show(res.substr(5));
                            } else {
                                personnel.overview.loginInfo.setSelection(personnel.overview.mainGrid.currentID(), true);
                            }
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }

                    });
                },

                create: function() {

                    var email = DevEx.Controls.GetByName("CreateLoginInfoEmail").GetValue();
                    var displayName = DevEx.Controls.GetByName("CreateLoginInfoDisplayName").GetValue();
                    var password = DevEx.Controls.GetByName("CreateLoginInfoPassword").GetValue();

                    // create new
                    $.ajax({
                        type: 'POST',
                        url: '/Personnel/CreateLogin',
                        data: {
                            personnelID: personnel.overview.mainGrid.currentID(),
                            email: email,
                            displayName: displayName,
                            password: password
                        },
                        success: function (res) {
                            if (res == "ok") {
                                personnel.overview.loginInfo.setSelection(personnel.overview.mainGrid.currentID(), true);
                                return;
                            }

                            if (res.substr(0, 3) == "err") {
                                App.Errors.Show(res.substr(4));
                            } else {
                                App.Errors.ShowGeneral();
                            }
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }

                    });

                },

                initWithLogin: function() {
                    DevEx.Controls.GetByName("RemoveLoginInfoSubmitButton").Click.AddHandler(function (s, e) {
                        personnel.overview.loginInfo.remove();
                    });
                    DevEx.Controls.GetByName("UpdateLoginInfoSubmitButton").Click.AddHandler(function (s, e) {
                        personnel.overview.loginInfo.update();
                    });
                },

                initWithoutLogin: function() {
                    DevEx.Controls.GetByName("CreateLoginInfoSubmitButton").Click.AddHandler(function (s, e) {
                        personnel.overview.loginInfo.create();
                    });
                },

                currentDisplayedID: -1,
                setSelection: function (id, force) {
                    if (!force && personnel.overview.loginInfo.currentDisplayedID == id) {
                        return;
                    }

                    personnel.overview.loginInfo.currentDisplayedID = id;

                    var info = personnel.overview.loginInfo;
                    

                    if (id == null) {
                        $("#" + info.elementID).hide(250);
                        $("#" + info.withLoginElementID).hide();
                        $("#" + info.withoutLoginElementID).hide();
                        return;
                    }

                    $.ajax({
                        url: '/Personnel/LoginInfo',
                        type: 'GET',
                        data: { id: id },
                        success: function (r) {

                            $("#" + info.elementID).hide(250, function () {
                                $("#" + info.withLoginElementID).hide();
                                $("#" + info.withoutLoginElementID).hide();

                                if (r == "err") {
                                    App.Errors.ShowGeneral();
                                    return;
                                }

                                if (r.substr(0, 4) == "with") {
                                    $("#" + info.withLoginElementID).empty().append(r.substr(4));
                                    personnel.overview.loginInfo.initWithLogin();
                                    $("#" + info.withLoginElementID).show();
                                    $("#" + info.elementID).show(250);

                                } else {
                                    $("#" + info.withoutLoginElementID).empty().append(r);
                                    personnel.overview.loginInfo.initWithoutLogin();
                                    $("#" + info.withoutLoginElementID).show();
                                    $("#" + info.elementID).show(250);

                                }
                            });
                            


                        },
                        error: function () {
                            $("#" + info.elementID).hide(250);
                            $("#" + info.withLoginElementID).hide();
                            $("#" + info.withoutLoginElementID).hide();
                            App.Errors.ShowGeneral();
                        }
                    });

                }

            },

            addNew: {

                submit: function(firstName, lastName, isAdmin, callback) {

                    $.ajax({
                        type: 'POST',
                        url: "/Personnel/AddNew",
                        data: {
                            firstName: firstName,
                            lastName: lastName,
                            isAdmin: isAdmin
                        },
                        success: function (r) {
                            
                            if (r == "ok") {
                                callback(true);
                            } else {
                                callback(false);
                            }
                            
                        },
                        error: function (r) {
                            callback(false);
                        }

                    });

                },  // end personnel.overview.addnew.submit

                showPopup: function () {
                    
                    App.Popup.Show({
                        url: '/Personnel/AddNewPopup',
                        type: 'GET',
                        data: null,
                        options: {
                            width: 300,
                            height: 200,
                            title: "Add New Person",
                            allowDrag: true,
                            allowResize: false
                        },

                        opened: function () {

                            DevEx.Controls.GetByName("personnelAddNewOkButton").Click.AddHandler(function (s, e) {

                                var firstName = DevEx.Controls.GetByName("AddPersonnelFirstName").GetValue();
                                var lastName = DevEx.Controls.GetByName("AddPersonnelLastName").GetValue();
                                var isAdmin = DevEx.Controls.GetByName("AddPersonnelIsAdmin").GetValue();
                                if (isAdmin == null) { isAdmin = false;}

                                personnel.overview.addNew.submit(firstName, lastName, isAdmin, function (r) {
                                    if (r) {
                                        App.Popup.Hide('ok');
                                    } else {
                                        App.Errors.Show("Unable to add this person.  Please ensure all values are filled out and the name is not already used.");
                                    }
                                });
                                   
                            });

                            DevEx.Controls.GetByName("personnelAddNewCancelButton").Click.AddHandler(function (s, e) {
                                App.Popup.Hide('cancelled');
                            });

                        },  // end personnel.overview.addnew.showPopup.opened

                        done: function (r) {
                            if (r == "ok") {
                                personnel.overview.mainGrid.refresh();
                            }
                        },
                        error: function (r) {
                            App.Errors.ShowGeneral();
                        }
                    });


                }   // end personnel.overview.addNew.showPopup

            },  // end personnel.overview.addNew

            remove: function(id) {

                App.Confirm(
                    "Are you sure you want to remove the selected person from your company?  This cannot be undone.",
                    function () {

                        // confirmed, remove them
                        $.ajax({
                            type: 'POST',
                            url: '/Personnel/Remove',
                            data: {
                                id: id
                            },
                            success: function (r) {
                                if (r == "ok") {
                                    personnel.overview.mainGrid.refresh();
                                    return;
                                }
                                if (r.substr(0, 4) == "err:") {
                                    App.Errors.Show(r.substr(5));
                                } else {
                                    App.Errors.ShowGeneral();
                                }
                            },
                            error: function () {
                                App.Errors.ShowGeneral();
                            }
                        });

                    });

            },  // end personnel.overview.remove

            reportsToGrid: {

                object: null,
                objectName: null,

                currentDisplayedID: -1,
                setSelection: function(personnelID, supressHide) {

                    if (personnel.overview.reportsToGrid.currentDisplayedID == personnelID) {
                        return;
                    }

                    personnel.overview.reportsToGrid.currentDisplayedID = personnelID;

                    if (!supressHide) {
                        $("#personnelOverviewReportsToGridContainer").hide(250);
                    }
                    
                    $.ajax({
                        type: 'GET',
                        url: '/Personnel/ReportsToGrid',
                        data: { personnelID: personnelID },
                        success: function (r) {
                            if (r.substr(0, 4) == "err:") {
                                App.Errors.ShowGeneral();
                            } else {
                                // set source here
                                $("#personnelOverviewReportsToGridContainer").empty().append(r);
                                if (!supressHide) {
                                    $("#personnelOverviewReportsToGridContainer").show(250);
                                }
                                personnel.overview.reportsToGrid.initialize();
                            }
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },


                events: {

                    _fromSource: {

                        cellClick: function (cell) {

                            var index = cell.attr("data-dym-visibleIndex");
                            var field = cell.attr("data-dym-fieldName");

                            var id = personnel.overview.reportsToGrid.object.GetRowKey(index);

                            if (field == "ReportsTo") {
                                console.log('cell clicked: ' + index + ', ' + field + '(' + id + ')');
                                personnel.overview.reportsToGrid.toggle(id);
                            }
                            
                            
                        }

                    }

                },

                toggle: function(targetID) {

                    $.ajax({
                        type: 'POST',
                        url: '/Personnel/ToggleReportsTo',
                        data: {
                            sourceID: personnel.overview.currentID(),
                            targetID: targetID
                        },
                        success: function (r) {
                            if (r == "ok") {
                                // for some reason a simple refresh breaks the click events
                                // this was the case also when trying to re-apply the events after
                                // however if I clear and replace the entire grid via setSelection we're in decent shape
                                // refactor later...
                                personnel.overview.reportsToGrid.setSelection(personnel.overview.currentID(), true);
                            } else {
                                App.Errors.ShowGeneral();
                            }
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }

                    });

                },

                initialize: function (gridName) {

                    var grid = personnel.overview.reportsToGrid;
                    grid.object = DevEx.Controls.GetByName("personnelmgmtReportsToGrid");
                    grid.objectName = gridName;
                    
                    grid.object.BeginCallback.AddHandler(function (s, e) { e.customArgs["personnelID"] = personnel.overview.currentID(); });

                    $('[data-dym-cellClick="ReportsToGrid"]').click(function () { personnel.overview.reportsToGrid.events._fromSource.cellClick($(this)); });
                    
                },  // end personnel.overview.reportsToGrid.initialize

            },  // end personnel.overview.reportsToGrid

            mainGrid: {

                object: null,
                objectName: null,

                refresh: function() {
                    personnel.overview.mainGrid.object.Refresh();
                },

                currentID: function() {
                    var grid = personnel.overview.mainGrid;
                    var fr = grid.object.GetFocusedRowIndex();
                    return grid.object.GetRowKey(fr);
                },

                events: {

                    rowSelected: function(personnelID) {
                        personnel.overview.setSelection(personnelID);
                    },

                    _fromSource: {

                        focusedRowChanged: function (s, e) {
                            personnel.overview.mainGrid.events.rowSelected(personnel.overview.mainGrid.currentID());
                        },

                        beginCallback: function (s, e) {
                            e.customArgs["personnelID"] = personnel.overview.currentID();
                        }

                    },  // end personnel.overview.mainGrid.events._fromSource
                    
                }  // end personnel.overview.mainGrid.events

            },  // end personnel.overview.mainGrid

            

            initialize: function(overviewGridControlName, reportsToGridID, callback) {

                console.log('initializing personnel.overview');

                var maingrid = personnel.overview.mainGrid;
                maingrid.object = DevEx.Controls.GetByName(overviewGridControlName);
                maingrid.objectName = overviewGridControlName;

                maingrid.object.FocusedRowChanged.AddHandler(function (s, e) { maingrid.events._fromSource.focusedRowChanged(s, e); });

                //personnel.overview.reportsToGrid.initialize(reportsToGridID, "asdf");

                DevEx.Controls.GetByName("PersonnelOverviewAddNewButton").Click.AddHandler(function (s, e) {
                    personnel.overview.addNew.showPopup();
                });

                DevEx.Controls.GetByName("PersonnelOverviewRemoveSelectedButton").Click.AddHandler(function (s, e) {
                    personnel.overview.remove(personnel.overview.mainGrid.currentID());
                });

                DevEx.Controls.GetByName("PersonnelOverviewEditAvailability").Click.AddHandler(function (s, e) {
                    personnel.overview.editAvailability(personnel.overview.mainGrid.currentID());
                });

                personnel.overview.loginInfo.elementID = "PersonnelOverviewLoginInfoContainer";
                personnel.overview.loginInfo.withLoginElementID = "PersonnelOverviewLoginInfoWithLoginContainer";
                personnel.overview.loginInfo.withoutLoginElementID = "PersonnelOverviewLoginInfoWithoutLoginContainer";
                

                // done, initialize the initial selection
                personnel.overview.setSelection(personnel.overview.currentID());

                if (App.IsCallback(callback)) {
                    callback();
                }

            },   // end personnel.overview.initialize

            currentID: function () {
                var grid = personnel.overview.mainGrid;
                var fr = grid.object.GetFocusedRowIndex();
                return grid.object.GetRowKey(fr);
            }

        }   // end personnel.overview

        

            
            
            

        
        
    }




    window.Personnel = Interface;

});