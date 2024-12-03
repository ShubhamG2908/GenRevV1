// owner.js

define(function () {

    /*****************************************

        Owner.Initialize(pageControlName, [callback])
        
        Owner.PageBase.Init(s, e)
        Owner.PageBase.BeginCallback(s, e)

        Owner.Create.Init(uiPrefix)

    ******************************************/

    var Interface = {

        Initialize: function (pageControlname, callback) { owner.initialize(pageControlname, callback); },  // end Owner.Initialize
        
        PageBase: {
            Init: function (s, e) { owner.pageBase.init(s, e); },
            BeginCallback: function (s, e) { owner.pageBase.beginCallback(s, e); },
            LoadActiveTab: function () { owner.pageBase.loadActiveTab(); }

            
        },  // end Owner.PageBase

        Create: {
            Init: function (uiPrefix) { owner.create.init(uiPrefix); }

        }   // end Owner.General


    }   // end Owner.Interface



    var owner = {


        create: {

            init: function (uiPrefix) {

                console.log('initializing owner.overview');

                var saveButton = DevEx.Controls.GetByName(uiPrefix + "CreateButton");
                saveButton.Click.AddHandler(function (s, e) { owner.create.saveForm(); });
                

            },   // end owner.create.init

            saveForm: function () {
                
                dc = DevEx.Controls;
                
                var email = dc.GetValue("MasterEmail");
                var companyFullName = dc.GetValue("CompanyFullName");
                var companyName = dc.GetValue("CompanyName");
                var companyCode = dc.GetValue("CompanyCode");
                var contactFirstName = dc.GetValue("ContactFirstName");
                var contactLastName = dc.GetValue("ContactLastName");

                $.ajax({

                    url: "/Owner/CreateSubscription",
                    type: 'POST',
                    data: {
                        email: email,
                        companyFullName: companyFullName,
                        companyName: companyName,
                        companyCode: companyCode,
                        contactFirstName: contactFirstName,
                        contactLastName: contactLastName
                    },
                    success: function (r) {
                        if (r != "err") {
                            App.Alert("Account successfully created.\r\n\r\nThe temporary password is: " + r, DialogIcons.Ok, "Saved");
                        } else {
                            App.Errors.ShowGeneral();
                        }
                    },
                    error: function (r) {
                        App.Errors.ShowGeneral();
                    }

                });

            }

        },  // end owner.create

        initialize: function (pageControlName, callback) {

            var p = owner.pageBase;
            p.object = DevEx.Controls.GetByName(pageControlName);
            p.objectName = pageControlName;

            // wire up events
            p.object.BeginCallback.AddHandler(function (s, e) { owner.pageBase.beginCallback(s, e); });
            p.object.ActiveTabChanging.AddHandler(function (s, e) { owner.pageBase.activeTabChanging(s, e); });


            // finalize the initialization
            p.initializationCompleting();
            
            if (App.IsCallback(callback)) {
                callback();
            }

        },  // end owner.initialize

        pageBase: {

            object: null,
            objectName: null,
            
            activeTabChanging: function(s, e) { 
                e.reloadContentOnCallback = true;
            },

            beginCallback: function (s, e) {
                console.log('Owner.PageBase.BeginCallback');
                var tab = s.GetActiveTab();
                e.customArgs["tabName"] = tab.name;
            },

            initializationCompleting: function () {
                owner.pageBase.object.PerformCallback();
            }

        }   // end owner.pageBase

    }




    window.Owner = Interface;

});