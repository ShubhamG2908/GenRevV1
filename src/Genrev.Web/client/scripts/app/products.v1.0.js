// products.js

define(function () {

    /*****************************************

        
        
    ******************************************/

    var api = {

        Overview: {

            Initialize: function (callback) {

                code.initialize(callback);

            },  // end api.overview.initialize
        }
        
    }   // end api



    var code = {


        products: {

            grid: {
                refresh: function () {
                    DevEx.Controls.GetByName("ProductsGrid").Refresh();
                },
                currentID: function () {
                    var grid = DevEx.Controls.GetByName("ProductsGrid");
                    var fr = grid.GetFocusedRowIndex();
                    return grid.GetRowKey(fr);
                }
            },

            addProduct: function (sku, description, groupID, callback) {


                $.ajax({
                    type: 'POST',
                    url: '/Products/AddProduct',
                    data: {
                        sku: sku,
                        description: description,
                        groupID: groupID
                    },
                    success: function (res) {
                        if (res == 'ok') {
                            callback(true);
                        } else {
                            callback(false);
                        }
                    },
                    error: function () {
                        callback(false);
                    }

                });


            },

            showAddPopup: function () {


                App.Popup.Show({
                    url: '/Products/AddNewPopup',
                    type: 'GET',
                    data: null,
                    options: {
                        width: 300,
                        height: 200,
                        title: 'Add New Product',
                        allowDrag: true,
                        allowResize: false
                    },
                    opened: function () {

                        DevEx.Controls.GetByName('productAddNewOkButton').Click.AddHandler(function (s, e) {

                            var sku = DevEx.Controls.GetValue('AddProductSKU');
                            var desc = DevEx.Controls.GetValue('AddProductDescription');
                            var groupID = DevEx.Controls.GetValue('AddProductGroupSelector');

                            code.products.addProduct(sku, desc, groupID, function (res) {
                                if (res) {
                                    App.Popup.Hide('ok');
                                    code.products.grid.refresh();
                                } else {
                                    App.Popup.Errors.Show("Unable to add this product.  Please ensure the SKU is not already in use.");
                                }
                            })

                        });

                        DevEx.Controls.GetByName('productAddNewCancelButton').Click.AddHandler(function (s, e) {
                            App.Popup.Hide('cancelled');
                        });


                    },
                    done: function (res) {

                    },
                    error: function () {
                        App.Errors.ShowGeneral();
                    }

                });

            },

            remove: function (id) {

                App.Confirm(
                    "Are you sure you want to remove the selected Product?",
                    function () {

                        $.ajax({
                            type: 'POST',
                            url: '/Products/RemoveProduct',
                            data: {
                                id: id
                            },
                            success: function (res) {
                                code.products.grid.refresh();
                            },
                            error: function () {
                                App.Errors.ShowGeneral();
                            }
                        });

                    }
                );

            }

        },   // end code.products

        groups: {

            grid: {
                refresh: function () {
                    DevEx.Controls.GetByName("ProductGroupGrid").Refresh();
                },

                currentID: function () {
                    var grid = DevEx.Controls.GetByName("ProductGroupGrid");
                    var fr = grid.GetFocusedRowIndex();
                    return grid.GetRowKey(fr);
                }
            },

            addGroup: function(groupName, parentID, callback) {

                if (groupName == null) {
                    return;
                }

                $.ajax({
                    type: 'POST',
                    url: '/Products/AddGroup',
                    data: {
                        groupName: groupName,
                        parentID: parentID
                    },
                    success: function (res) {
                        if (res == 'ok') {
                            callback(true);
                        } else {
                            callback(false);
                        }
                    },
                    error: function () {
                        callback(false);
                    }

                });

            },

            showAddPopup: function () {
                
                App.Popup.Show({
                    url: '/Products/AddNewGroupPopup',
                    type: 'GET',
                    data: null,
                    options: {
                        width: 300,
                        height: 200,
                        title: 'Add New Product Group',
                        allowDrag: true,
                        allowResize: false
                    },
                    opened: function () {

                        DevEx.Controls.GetByName('productGroupAddNewOkButton').Click.AddHandler(function (s, e) {

                            var groupName = DevEx.Controls.GetValue('AddProductGroupName');
                            var parentID = DevEx.Controls.GetValue('AddProductGroupParentSelector');
                            
                            code.groups.addGroup(groupName, parentID, function (res) {
                                if (res) {
                                    App.Popup.Hide('ok');
                                    code.groups.grid.refresh();
                                } else {
                                    App.Errors.Show('Unable to add this product group.  Please ensure the name is not already used.');
                                }
                            });

                        });

                        DevEx.Controls.GetByName('productGroupAddNewCancelButton').Click.AddHandler(function (s, e) {
                            App.Popup.Hide('cancelled');
                        });
                        

                    },
                    done: function (res) {

                    },
                    error: function () {
                        App.Errors.ShowGeneral();
                    }

                });

            },

            remove: function(id) {
                
                App.Confirm(
                    "Are you sure you want to remove the selected Product Group?  Associated Products will not be removed.",
                    function () {

                        $.ajax({
                            type: 'POST',
                            url: '/Products/RemoveGroup',
                            data: {
                                groupID: id
                            },
                            success: function (res) {
                                code.groups.grid.refresh();
                            },
                            error: function () {
                                App.Errors.ShowGeneral();
                            }
                        });

                    }
                );

            }

        },  // end product.groups

        initialize: function (callback) {


            DevEx.Controls.GetByName('ProductGroupAddNewButton').Click.AddHandler(function (s, e) {
                code.groups.showAddPopup();
            });

            DevEx.Controls.GetByName('ProductGroupRemoveSelectedButton').Click.AddHandler(function (s, e) {
                code.groups.remove(code.groups.grid.currentID());
            });


            DevEx.Controls.GetByName('ProductAddNewButton').Click.AddHandler(function (s, e) {
                code.products.showAddPopup();
            });

            DevEx.Controls.GetByName('ProductRemoveSelectedButton').Click.AddHandler(function (s, e) {
                code.products.remove(code.products.grid.currentID());
            });



            if (App.IsCallback(callback)) {
                callback();
            }

        }   // end code.initialize
        
        
    }   // end code




    window.Products = api;

});