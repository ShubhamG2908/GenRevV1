// customers.js

define(function () {

    /*****************************************
    
    ******************************************/

    var Interface = {
        
        Initialize: function (pageControlName, callback) { customers.initialize(pageControlName, callback); },

        Types: { Init: function () { customers.types.initialize(); } },
        Industries: { Init: function () { customers.industries.initialize(); } },
        Customers: { Init: function () { customers.customers.initialize(); } },
        Mappings: { Init: function () { customers.mappings.initialize(); } },
        Classifications: { Init: function () { customers.classifications.initialize(); } }

    }   // end Customers.Interface
    
    var customers = {

        initialize: function (pageControlName, callback) {

            var p = customers.pageBase;
            p.object = DevEx.Controls.GetByName(pageControlName);
            p.objectName = pageControlName;
            
            // wire up events
            p.object.BeginCallback.AddHandler(function (s, e) { customers.pageBase.beginCallback(s, e); });
            p.object.ActiveTabChanging.AddHandler(function (s, e) { customers.pageBase.activeTabChanging(s, e); });

            // finalize the initialization
            p.initializationComplete();

            if (App.IsCallback(callback)) {
                callback();
            }
            
        },  // end customers.initialize

        pageBase: {

            object: null,
            objectName: null,

            activeTabChanging: function (s, e) {
                e.reloadContentOnCallback = true;
            },

            beginCallback: function (s, e) {
                console.log('Customers.PageBase.BeginCallback');
                var tab = s.GetActiveTab();
                e.customArgs["tabName"] = tab.name;
            },

            initializationComplete: function () {
                customers.pageBase.object.PerformCallback();
            }


        },   // end customers.pageBase

        customers: {

            Model: function() {
                return {
                    id: null,
                    name: null,
                    address1: null,
                    address2: null,
                    city: null,
                    state: null,
                    postal: null,
                    country: null,
                    phone: null,
                    typeID: null,
                    industryID: null,
                    accountTypeID: null
                }
            },

            addNew: function(model, cbSuccess, cbFailure) {
                                
                var valid = true;

                App.Try(function () { model.name = model.name.trim(); });
                
                if (model.name == null || model.name == "" || model.name == undefined) {
                    valid = false;
                }

                if (!valid) {
                    App.Errors.Show("Unable to add customer.  Please be sure all required fields are filled out and that all data is valid.");
                    return;
                }

                // submit
                $.ajax({
                    type: 'POST',
                    url: '/Customers/AddCustomer',
                    data: {
                        customerName: model.name,
                        customerAddress1: model.address1,
                        customerAddress2: model.address2,
                        customerCity: model.city,
                        customerState: model.state,
                        customerPostalCode: model.postal,
                        customerCountry: model.country,
                        customerPhone: model.phone,
                        customerTypeID: model.typeID,
                        customerIndustryID: model.industryID,
                        customerAccountTypeID: model.accountTypeID
                    },
                    success: function (res) {

                        if (res == "ok") {
                            //customers.customers.grid.refresh();
                            App.Callback(cbSuccess);
                        } else {
                            // assume res = "ERR: message"
                            App.Errors.Show(res.substr(4));
                            App.Callback(cbFailure);
                        }
                    },

                    error: function () {
                        App.Errors.ShowGeneral();
                        App.Callback(cbFailure);
                    }
                    
                });



            },  // end customers.customers.addNew

            edit: function(id) {
                customers.customers.popups.edit.show(id);
            },

            editDirect: function(model, cbSuccess) {

                var valid = true;

                App.Try(function () { model.name = model.name.trim(); });

                if (model.name == null || model.name == "" || model.name == undefined) {
                    valid = false;
                }

                if (!valid) {
                    App.Errors.Show("Unable to add customer.  Please be sure all required fields are filled out and that all data is valid.");
                    return;
                }

                // submit
                $.ajax({
                    type: 'POST',
                    url: '/Customers/EditCustomer',
                    data: {
                        id: model.id,
                        customerName: model.name,
                        customerAddress1: model.address1,
                        customerAddress2: model.address2,
                        customerCity: model.city,
                        customerState: model.state,
                        customerPostalCode: model.postal,
                        customerCountry: model.country,
                        customerPhone: model.phone,
                        customerTypeID: model.typeID,
                        customerIndustryID: model.industryID,
                        customerAccountTypeID: model.accountTypeID
                    },
                    success: function (res) {

                        if (res == "ok") {
                            //customers.customers.grid.refresh();
                            App.Callback(cbSuccess);
                        } else {
                            // assume res = "ERR: message"
                            App.Errors.Show(res.substr(4));
                            App.Callback(cbFailure);
                        }
                    },

                    error: function () {
                        App.Errors.ShowGeneral();
                        App.Callback(cbFailure);
                    }

                });
            },

            remove: function(id, cbSuccess, cbError) {

                App.Confirm("Are you sure you want to delete this customer and all of the associated data?", function () {
                    $.ajax({
                        url: '/Customers/DeleteCustomer',
                        type: 'POST',
                        data: { id: id },
                        success: function (res) {
                            App.Callback(cbSuccess);
                        },
                        error: function (err) {
                            App.Errors.Show(err);
                            App.Callback(cbError);
                        }
                    });
                });

            },

            grid: {

                object: null,
                objectName: null,

                init: function(gridName) {

                    var grid = customers.customers.grid;
                    grid.object = DevEx.Controls.GetByName(gridName);
                    grid.objectName = gridName;
                    grid.stretchHeight(grid.object);

                    grid.events.addWindowResizeStretchGridHandler(grid.object);
                    grid.events.addRowDoubleClickHandler(grid.object);
                    grid.events.addEndCallbackHandler(grid.object);
                    
                },

                refresh: function () {
                    var g = customers.customers.grid;
                    g.object.Refresh();                   
                },


                events: {

                    addEndCallbackHandler(grid) {
                        grid.EndCallback.AddHandler(function(s, e) {
                            // relink stuff...
                            var ge = customers.customers.grid.events;
                            ge.addWindowResizeStretchGridHandler(s);
                            ge.addRowDoubleClickHandler(s);
                        });
                    },

                    addWindowResizeStretchGridHandler(grid) {
                        window.addEventListener("resize", function (event) {
                            customers.customers.grid.stretchHeight(grid);
                        });
                    },

                    addRowDoubleClickHandler(grid) {
                        $("[data-dym-cell='CustomersGrid']").dblclick(function() {
                            var cell = $(this);
                            var index = cell.attr('data-dym-visibleIndex');
                            var id = DevEx.Grid.GetFocusedKey(DevEx.Controls.GetByName("CustomersGrid"));
                            customers.customers.edit(id);
                        });
                    },

                },

                stretchHeight: function(grid) {
                    console.log('stretching height');
                    var splitter = DevEx.Controls.GetByName("ContentSplitter");
                    var h = splitter.GetPane(1).GetClientHeight();
                    var outerHeight = $("#customers-header-area-container").outerHeight(true);
                    var buffer = 60;

                    h = h - buffer - outerHeight;

                    grid.SetHeight(h);

                }   // end customers.grid.stretchHeight

            },  // end customers.customers.grid
            
            popups: {

                edit: {

                    show: function(id) {

                        App.Popup.Show({
                            url: "/Customers/EditCustomerPopup",
                            type: 'GET',
                            data: { id: id },
                            options: {
                                width: 300,
                                height: 200,
                                title: "Edit Customer",
                                allowDrag: true,
                                allowResize: false
                            },
                            opened: function () {


                                DevEx.Controls.GetByName("CustomerDeleteButton").Click.AddHandler(function (s, e) {
                                    customers.customers.remove(id, function () {
                                        App.Popup.Hide('ok');
                                    });
                                });

                                DevEx.Controls.GetByName("CustomerEditOkButton").Click.AddHandler(function (s, e) {

                                    var model = new customers.customers.Model();

                                    model.id = id;
                                    model.name = DevEx.Controls.GetValue("EditCustomerName");
                                    model.address1 = DevEx.Controls.GetValue("EditCustomerAddress1");
                                    model.address2 = DevEx.Controls.GetValue("EditCustomerAddress2");
                                    model.city = DevEx.Controls.GetValue("EditCustomerCity");
                                    model.state = DevEx.Controls.GetValue("EditCustomerState");
                                    model.postal = DevEx.Controls.GetValue("EditCustomerPostalCode");
                                    model.country = DevEx.Controls.GetValue("EditCustomerCountry");
                                    model.phone = DevEx.Controls.GetValue("EditCustomerPhone");
                                    model.typeID = DevEx.Controls.GetValue("EditCustomerCustomerType");
                                    model.industryID = DevEx.Controls.GetValue("EditCustomerIndustry");
                                    model.accountTypeID = DevEx.Controls.GetValue("EditCustomerAccountType");

                                    customers.customers.editDirect(model, function () {
                                        App.Popup.Hide('ok');
                                    });
                                });

                                DevEx.Controls.GetByName("CustomerEditCancelButton").Click.AddHandler(function (s, e) {
                                    App.Popup.Hide('cancelled');
                                });

                            },
                            done: function (r) {
                                if (r == "ok") {
                                    customers.customers.grid.refresh();
                                }
                            },
                            error: function (r) {
                                App.Errors.ShowGeneral();
                            }
                        });

                    }

                },  // end customers.customers.popups.edit

                addNew: {

                    show: function () {

                        App.Popup.Show({
                            url: "/Customers/AddNewCustomerPopup",
                            type: 'GET',
                            data: null,
                            options: {
                                width: 300,
                                height: 200,
                                title: "Add New Customer",
                                allowDrag: true,
                                allowResize: false
                            },
                            opened: function () {

                                DevEx.Controls.GetByName("CustomerAddNewOkButton").Click.AddHandler(function (s, e) {
                                    
                                    var model = new customers.customers.Model();

                                    model.name = DevEx.Controls.GetValue("AddCustomerName");
                                    model.address1 = DevEx.Controls.GetValue("AddCustomerAddress1");
                                    model.address2 = DevEx.Controls.GetValue("AddCustomerAddress2");
                                    model.city = DevEx.Controls.GetValue("AddCustomerCity");
                                    model.state = DevEx.Controls.GetValue("AddCustomerState");
                                    model.postal = DevEx.Controls.GetValue("AddCustomerPostalCode");
                                    model.country = DevEx.Controls.GetValue("AddCustomerCountry");
                                    model.phone = DevEx.Controls.GetValue("AddCustomerPhone");
                                    model.typeID = DevEx.Controls.GetValue("AddCustomerCustomerType");
                                    model.industryID = DevEx.Controls.GetValue("AddCustomerIndustry");
                                    model.accountTypeID = DevEx.Controls.GetValue("AddCustomerAccountType");
                                    
                                    customers.customers.addNew(model, function () {
                                        App.Popup.Hide('ok');
                                    });
                                });

                                DevEx.Controls.GetByName("CustomerAddNewCancelButton").Click.AddHandler(function (s, e) {
                                    App.Popup.Hide('cancelled');
                                });

                            },
                            done: function (r) {
                                if (r == "ok") {
                                    customers.customers.grid.refresh();
                                }
                            },
                            error: function (r) {
                                App.Errors.ShowGeneral();
                            }
                        });

                    }

                }   // end customers.customers.popups.addNew

            },  // end customers.customers.popups

            events: {

                addNewCustomerClick: function () {
                    customers.customers.popups.addNew.show();
                }

            },  // end customers.customers.events

            initialize: function () {

                console.log('customers.customers.initialize');

                DevEx.Controls.GetByName("CustomersAddNewButton").Click.AddHandler(function (s, e) {
                    customers.customers.events.addNewCustomerClick();
                });

                customers.customers.grid.init("CustomersGrid");


            }   // end customers.customers.initialize


        },  // end customers.customers

        classifications: {

            initialize: function () {
                console.log('initilializing classifications');

                var cls = customers.classifications;

                cls.types.initialize();
                cls.industries.initialize();
                cls.accountTypes.initialize();

                var msgDismiss = $("#cust-class-header-msg-close").on('click', function () {
                    $("#cust-class-header-msg").hide(250);
                });

            },
            
            types: {

                edit: function (id) {
                    customers.classifications.types.popups.edit.show(id);
                },

                remove: function(id, cbSuccess) {
                    
                    App.Confirm("Are you sure you want to delete this Customer Type?", function () {
                        $.ajax({
                            type: 'POST',
                            url: '/Customers/DeleteCustomerType',
                            data: { id: id },
                            success: function (res) {
                                App.Callback(cbSuccess);
                            },
                            error: function () {
                                App.Errors.ShowGeneral();
                            }
                        });
                    });

                },

                editDirect: function(id, value, cbSuccess) {
                    
                    $.ajax({
                        type: 'POST',
                        url: '/Customers/EditCustomerType',
                        data: {
                            id: id,
                            name: value
                        },
                        success: function (res) {
                            if (res == "ok") {
                                App.Callback(cbSuccess);
                            } else {
                                App.Errors.Show(res);
                            }                            
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },

                popups: {

                    edit: {
                        show: function (id) {

                            App.Popup.Show({
                                url: "/Customers/EditCustomerTypePopup",
                                type: 'GET',
                                data: { id: id },
                                options: {
                                    width: 300,
                                    height: 200,
                                    title: "Edit Customer Type",
                                    allowDrag: true,
                                    allowResize: false
                                },
                                opened: function () {


                                    DevEx.Controls.GetByName("CustomerTypeDeleteButton").Click.AddHandler(function (s, e) {
                                        customers.classifications.types.remove(id, function () {
                                            App.Popup.Hide('ok');
                                        });
                                    });

                                    DevEx.Controls.GetByName("CustomerTypeEditOkButton").Click.AddHandler(function (s, e) {

                                        
                                        var value = DevEx.Controls.GetValue("CustomerTypeEditName");
                                        
                                        customers.classifications.types.editDirect(id, value, function() {
                                            App.Popup.Hide('ok');
                                        });
                                    });

                                    DevEx.Controls.GetByName("CustomerTypeEditCancelButton").Click.AddHandler(function (s, e) {
                                        App.Popup.Hide('cancelled');
                                    });

                                },
                                done: function (r) {
                                    if (r == "ok") {
                                        customers.classifications.types.grid.refresh();
                                    }
                                },
                                error: function (r) {
                                    App.Errors.ShowGeneral();
                                }
                            });

                        }
                    }

                },

                grid: null,
                
                addType: function (value) {

                    console.log('adding type');

                    var valid = true;

                    App.Try(function () { value = value.trim(); });

                    if (value == null || value == "" || value == undefined) {
                        valid = false;
                    }

                    if (!valid) {
                        App.Errors.Show("Please supply a Type to add");
                        return;
                    }

                    // we're good on frontend validation, attempt backend submission
                    $.ajax({
                        type: 'POST',
                        url: '/Customers/AddCustomerType',
                        data: {
                            typeName: value
                        },
                        success: function (res) {

                            if (res == "ok") {
                                
                            } else {
                                // assume response is "ERR: message"
                                App.Errors.Show(res.substr(4));
                            }

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });



                },  // end customers.classifications.types.addType

                events: {

                    addTypeClick: function () {
                        var value = DevEx.Controls.GetByName("AddCustomerType").GetValue();
                        customers.classifications.types.addType(value);
                    }

                },  // end customers.classifications.types.events

                initialize: function () {

                    var ctg = new devex.Grid({ name: 'CustomerTypesGrid' });
                    customers.classifications.types.grid = ctg;

                    ctg.rowDoubleClick.addHandler(function (s, e) {
                        customers.classifications.types.edit(e.clickedIndex);
                    });
                    
                    var btn = DevEx.Controls.GetByName("AddCustomerTypeButton");
                    btn.Click.AddHandler(function (s, e) {
                        customers.classifications.types.events.addTypeClick();
                        ctg.refresh();
                    });

                }   // end customers.classifications.types.initialize

            },  // end customers.classifications.types
            
            industries: {
                
                edit: function(id) {
                    customers.classifications.industries.popups.edit.show(id);
                },

                remove: function(id, cbSuccess) {

                    App.Confirm("Are you sure you want to delete this Industry?", function () {
                        $.ajax({
                            type: 'POST',
                            url: '/Customers/DeleteIndustry',
                            data: { id: id },
                            success: function (res) {
                                if (res == 'ok') {
                                    App.Callback(cbSuccess);
                                } else {
                                    App.Errors.Show(res);
                                }
                            },
                            error: function (err) {
                                App.Errors.ShowGeneral();
                            }
                        });
                    });
                },

                editDirect: function(id, value, cbSuccess) {
                    
                    $.ajax({
                        type: 'POST',
                        url: '/Customers/EditIndustry',
                        data: {
                            id: id,
                            name: value
                        },
                        success: function (res) {
                            if (res == "ok") {
                                App.Callback(cbSuccess);
                            } else {
                                App.Errors.Show(res);
                            }
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });
                },
                
                popups: {

                    edit: {
                        show: function (id) {

                            App.Popup.Show({
                                url: "/Customers/EditIndustryPopup",
                                type: 'GET',
                                data: { id: id },
                                options: {
                                    width: 300,
                                    height: 200,
                                    title: "Edit Industry",
                                    allowDrag: true,
                                    allowResize: false
                                },
                                opened: function () {


                                    DevEx.Controls.GetByName("CustomerIndustryDeleteButton").Click.AddHandler(function (s, e) {
                                        customers.classifications.industries.remove(id, function () {
                                            App.Popup.Hide('ok');
                                        });
                                    });

                                    DevEx.Controls.GetByName("CustomerIndustryEditOkButton").Click.AddHandler(function (s, e) {
                                        
                                        var value = DevEx.Controls.GetValue("CustomerIndustryEditName");

                                        customers.classifications.industries.editDirect(id, value, function () {
                                            App.Popup.Hide('ok');
                                        });
                                    });

                                    DevEx.Controls.GetByName("CustomerIndustryEditCancelButton").Click.AddHandler(function (s, e) {
                                        App.Popup.Hide('cancelled');
                                    });

                                },
                                done: function (r) {
                                    if (r == "ok") {
                                        customers.classifications.industries.grid.refresh();
                                    }
                                },
                                error: function (r) {
                                    App.Errors.ShowGeneral();
                                }
                            });
                        }
                    }

                },
                
                grid: null,

                addIndustry: function (value) {

                    var valid = true;

                    App.Try(function () { value = value.trim(); });

                    if (value == null || value == "" || value == undefined) {
                        valid = false;
                    }

                    if (!valid) {
                        App.Errors.Show("Please supply an Industry to add");
                        return;
                    }

                    // we're good on frontend validation, attempt backend submission
                    $.ajax({
                        type: 'POST',
                        url: '/Customers/AddCustomerIndustry',
                        data: {
                            industryName: value
                        },
                        success: function (res) {

                            if (res == "ok") {
                                // nothing
                            } else {
                                // assume response is "ERR: message"
                                App.Errors.Show(res.substr(4));
                            }

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },  // end customers.classifications.industries.addIndustry
                
                events: {

                    addIndustryClick: function () {
                        var value = DevEx.Controls.GetByName("AddCustomerIndustry").GetValue();
                        customers.classifications.industries.addIndustry(value);
                    }

                },  // end customers.industries.events

                initialize: function () {

                    var indGrid = new devex.Grid({ name: 'CustomerIndustriesGrid' });
                    customers.classifications.industries.grid = indGrid;

                    indGrid.rowDoubleClick.addHandler(function (s, e) {
                        customers.classifications.industries.edit(e.clickedIndex);
                    });

                    var btn = DevEx.Controls.GetByName("AddCustomerIndustryButton");
                    btn.Click.AddHandler(function (s, e) {
                        customers.classifications.industries.events.addIndustryClick();
                        indGrid.refresh();
                    });


                }   // end customers.classifications.industries.initialize


            },  // end customers.classifications.industries

            accountTypes: {

                edit: function (id) {
                    customers.classifications.accountTypes.popups.edit.show(id);
                },

                remove: function (id, cbSuccess) {

                    App.Confirm("Are you sure you want to delete this Account Type?", function () {
                        $.ajax({
                            type: 'POST',
                            url: '/Customers/DeleteAccountType',
                            data: { id: id },
                            success: function (res) {
                                App.Callback(cbSuccess);
                            },
                            error: function () {
                                App.Errors.ShowGeneral();
                            }
                        });
                    });

                },

                editDirect: function (id, value, callsPerMonth, cbSuccess) {

                    console.log(id + '|' + value + '|' + callsPerMonth);

                    $.ajax({
                        type: 'POST',
                        url: '/Customers/EditAccountType',
                        data: {
                            id: id,
                            name: value,
                            callsPerMonth: callsPerMonth
                        },
                        success: function (res) {
                            if (res == "ok") {
                                App.Callback(cbSuccess);
                            } else {
                                App.Errors.Show(res);
                            }
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });

                },
                
                popups: {


                    edit: {


                        show: function(id) {

                            App.Popup.Show({
                                url: "/Customers/EditAccountTypePopup",
                                type: 'GET',
                                data: { id: id },
                                options: {
                                    width: 300,
                                    height: 200,
                                    title: "Edit Account Type",
                                    allowDrag: true,
                                    allowResize: false
                                },
                                opened: function () {


                                    DevEx.Controls.GetByName("AccountTypeDeleteButton").Click.AddHandler(function (s, e) {
                                        customers.classifications.accountTypes.remove(id, function () {
                                            App.Popup.Hide('ok');
                                        });
                                    });

                                    DevEx.Controls.GetByName("AccountTypeEditOkButton").Click.AddHandler(function (s, e) {

                                        var value = DevEx.Controls.GetValue("AccountTypeEditName");
                                        var calls = DevEx.Controls.GetValue("AccountTypeCallsPerMonthGoal");

                                        customers.classifications.accountTypes.editDirect(id, value, calls, function () {
                                            App.Popup.Hide('ok');
                                        });
                                    });

                                    DevEx.Controls.GetByName("AccountTypeEditCancelButton").Click.AddHandler(function (s, e) {
                                        App.Popup.Hide('cancelled');
                                    });

                                },
                                done: function (r) {
                                    if (r == "ok") {
                                        customers.classifications.accountTypes.grid.refresh();
                                    }
                                },
                                error: function (r) {
                                    App.Errors.ShowGeneral();
                                }
                            });


                        }

                    }


                },

                grid: null,
                
                addType: function (value) {

                    var valid = true;

                    App.Try(function () { value = value.trim(); });

                    if (value == null || value == "" || value == undefined) {
                        valid = false;
                    }

                    if (!valid) {
                        App.Errors.Show("Please supply an Account Type to add");
                        return;
                    }

                    // we're good on frontend validation, attempt backend submission
                    $.ajax({
                        type: 'POST',
                        url: '/Customers/AddAccountType',
                        data: {
                            typeName: value
                        },
                        success: function (res) {

                            if (res == "ok") {
                                
                            } else {
                                // assume response is "ERR: message"
                                App.Errors.Show(res.substr(4));
                            }

                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }
                    });



                },  // end customers.classifications.accountTypes.addType

                events: {

                    addTypeClick: function () {
                        var value = DevEx.Controls.GetByName("AddAccountType").GetValue();
                        customers.classifications.accountTypes.addType(value);
                    }

                },  // end customers.classifications.types.events

                initialize: function () {
                    
                    var atg = new devex.Grid({ name: 'AccountTypesGrid' });
                    customers.classifications.accountTypes.grid = atg;

                    atg.rowDoubleClick.addHandler(function (s, e) {
                        customers.classifications.accountTypes.edit(e.clickedIndex);
                    });

                    var btn = DevEx.Controls.GetByName("AddAccountTypeButton");
                    btn.Click.AddHandler(function (s, e) {
                        customers.classifications.accountTypes.events.addTypeClick();
                        atg.refresh();
                    });

                }   // end customers.classifications.accountTypes.initialize

            },  // end customers.classifications.accountTypes
            
        },  // end customers.classifications

        mappings: {

            
            setSelection: function(personnelID, supressHide)  {

                if (personnelID == null) {
                    return;
                }
                    
                $.ajax({
                    type: 'GET',
                    url: "/Customers/GetMappingCustomerList",
                    data: {
                        personnelID: personnelID
                    },
                    success: function (res) {
                        if (res.substr(0, 4) == "ERR:") {
                            App.Errors.Show(res.substr(5));
                        } else {

                            // assume this a good return
                            customers.mappings.customersGrid.loadContent(res, supressHide);
                        }
                    },
                    error: function () {
                        App.Errors.ShowGeneral();
                    }

                });


            },  // end customers.mappings.setSelection

            personnelGrid: {
                // this is loaded by default with page

                currentID: function() {
                    return DevEx.Grid.GetFocusedKey(DevEx.Controls.GetByName(customers.mappings.personnelGrid.objectName));
                },

                object: null,
                objectName: null,

                events: {
                    
                    focusedRowChanged: function (s, e) {
                        var id = customers.mappings.personnelGrid.currentID();
                        customers.mappings.setSelection(id);
                    }

                },   // end customers.mappings.personnelGrid.events

                init: function (gridName) {

                    var control = DevEx.Controls.GetByName(gridName);
                    var grid = customers.mappings.personnelGrid;

                    grid.object = control;
                    grid.objectName = gridName;

                    control.FocusedRowChanged.AddHandler(function (s, e) { grid.events.focusedRowChanged(s, e); });

                }


            },  // end customers.mappings.personnelGrid

            customersGrid: {
                // this is loaded dynamically after selected item from personnel

                containerElementID: null,
                object: null,
                objectName: null,

                toggleSelection: function(customerID) {


                    $.ajax({
                        type: 'POST',
                        url: '/Customers/ToggleMapping',
                        data: {
                            personnelID: customers.mappings.personnelGrid.currentID(),
                            customerID: customerID
                        },
                        success: function (r) {
                            if (r == "ok") {
                                // for some reason a simple refresh breaks the click events
                                // this was the case also when trying to re-apply the events after
                                // however if I clear and replace the entire grid via setSelection we're in decent shape
                                // refactor later... (see also, personnel ReportsTo grid)

                                customers.mappings.setSelection(customers.mappings.personnelGrid.currentID(), true);
                                
                            } else {
                                App.Errors.ShowGeneral();
                            }
                        },
                        error: function () {
                            App.Errors.ShowGeneral();
                        }

                    });



                },  // end customers.mappings.customersGrid.toggleSelection

                loadContent: function(content, supressHide) {
                    // run this each time the grid needs to be rebuilt
                    var el = $("#" + customers.mappings.customersGrid.containerElementID);

                    if (supressHide) {

                        el.empty().append(content);
                        customers.mappings.customersGrid.objectName = "CustomerMappingsCustomerGrid";
                        var control = DevEx.Controls.GetByName("CustomerMappingsCustomerGrid");
                        customers.mappings.customersGrid.object = control;
                        customers.mappings.customersGrid.events.reloadGrid(control);

                    } else {

                        el.hide(250, function () {
                            el.empty().append(content);

                            customers.mappings.customersGrid.objectName = "CustomerMappingsCustomerGrid";
                            var control = DevEx.Controls.GetByName("CustomerMappingsCustomerGrid");
                            customers.mappings.customersGrid.object = control;
                            customers.mappings.customersGrid.events.reloadGrid(control);
                            
                            el.show(250);
                        });

                    }

                    

                },  // end customers.mappings.customersGrid.loadContent

                init: function (containerElementID) {
                    // run this first on page startup
                    customers.mappings.customersGrid.containerElementID = containerElementID;
                },

                events: {

                    reloadGrid: function(grid) {

                        grid.BeginCallback.AddHandler(function (s, e) { customers.mappings.customersGrid.events.beginCallback(s, e); });
                        $('[data-dym-cellClick="CustomerMappingsGrid"]').click(function () { customers.mappings.customersGrid.events.cellClick($(this)); });

                    },

                    cellClick: function(cell) {

                        var index = cell.attr('data-dym-visibleIndex');
                        var field = cell.attr('data-dym-fieldName');

                        var id = customers.mappings.customersGrid.object.GetRowKey(index);

                        if (field == "Selected") {
                            console.log('cell clicked: ' + index + ', ' + field + '(' + id + ')');
                            customers.mappings.customersGrid.toggleSelection(id);
                        }
                    },

                    beginCallback: function (s, e) {
                        e.customArgs["personnelID"] = customers.mappings.personnelGrid.currentID();
                    }

                }   // end customers.mappings.customerGrid.events

            },  // end customers.mappings.customerGrid

            initialize: function () {


                customers.mappings.customersGrid.init("MappingsCustomersGridContainer");
                customers.mappings.personnelGrid.init("CustomerMappingsPersonnelGrid");

                



            }   // end customers.mappings.initialize

        }   // end customers.mappings


    }   // end customers


    window.Customers = Interface;




});