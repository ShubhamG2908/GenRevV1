// subscription.js

define(function () {

    /*****************************************

        Subscription.Initialize(pageControlName, [callback])
        Subscription.PageBase.Init(s, e)
        Subscription.PageBase.BeginCallback(s, e)

    ******************************************/

    var Interface = {

        Initialize: function (pageControlname, callback) { subscription.initialize(pageControlname, callback); },  // end Subscription.Initialize
        
        PageBase: {
            Init: function (s, e) { subscription.pageBase.init(s, e); },
            BeginCallback: function (s, e) { subscription.pageBase.beginCallback(s, e); },
            LoadActiveTab: function () { subscription.pageBase.loadActiveTab(); }

            
        },  // end Subscription.PageBase

        General: {
            Init: function (uiPrefix) { subscription.general.init(uiPrefix); }

        },   // end Subscription.General

        Integrations: {
            Init: function () { subscription.integrations.init(); }
        }


    }   // end Subscription.Interface



    var subscription = {



        integrations: {

            init: function() {

                var regenKeyButton = DevEx.Controls.GetByName("btnRegenKey");
                var saveButton = DevEx.Controls.GetByName("btnSaveChanges");

                regenKeyButton.Click.AddHandler(function (s, e) {
                    subscription.integrations.getNewKey();
                });

                saveButton.Click.AddHandler(function (s, e) {
                    subscription.integrations.save();
                });

            },

            getNewKey: function() {
                
                $.ajax({
                    type: 'POST',
                    url: '/Subscription/GetNewApiKey',
                    success: function (res) {
                        if (res == "error") {
                            App.Error();
                        } else {
                            DevEx.Controls.GetByName("ApiKey").SetText(res);
                        }
                    }
                });

            },

            save: function () {

                var apiEnabled = DevEx.Controls.GetValue("ApiEnabled");
                var apiKey = DevEx.Controls.GetValue("ApiKey");
                var apiPassword = DevEx.Controls.GetValue("ApiPassword");
                
                $.ajax({
                    type: 'POST',
                    url: '/Subscription/SaveApiSettings',
                    data: {
                        apiEnabled: apiEnabled,
                        apiKey: apiKey,
                        apiPassword: apiPassword                
                    },
                    success: function () {
                        App.Alert("Changes saved successfully");
                    }
                })

            }

        },

        general: {

            init: function (uiPrefix) {

                console.log('initializing subscription.general');

                var saveButton = DevEx.Controls.GetByName(uiPrefix + "SaveButton");
                saveButton.Click.AddHandler(function (s, e) { subscription.general.saveForm(); });
                

            },   // end subscription.general.init

            saveForm: function () {
                
                dc = DevEx.Controls;
                
                var companyFullName = dc.GetValue("CompanyFullName");
                var companyName = dc.GetValue("CompanyName");
                var companyCode = dc.GetValue("CompanyCode");
                var contactFirstName = dc.GetValue("ContactFirstName");
                var contactLastName = dc.GetValue("ContactLastName");

                $.ajax({

                    url: "/Subscription/General",
                    type: 'POST',
                    data: {
                        companyFullName: companyFullName,
                        companyName: companyName,
                        companyCode: companyCode,
                        contactFirstName: contactFirstName,
                        contactLastName: contactLastName
                    },
                    success: function (r) {
                        if (r == "ok") {
                            App.Alert("Changes have been saved", DialogIcons.Ok, "Saved");
                        } else {
                            App.Errors.ShowGeneral();
                        }
                    },
                    error: function (r) {
                        App.Errors.ShowGeneral();
                    }

                });

            }

        },  // end subscription.general

        initialize: function (pageControlName, callback) {
            var p = subscription.pageBase;
            p.object = DevEx.Controls.GetByName(pageControlName);
            p.objectName = pageControlName;

            // wire up events
            p.object.BeginCallback.AddHandler(function (s, e) { subscription.pageBase.beginCallback(s, e); });
            p.object.ActiveTabChanging.AddHandler(function (s, e) { subscription.pageBase.activeTabChanging(s, e); });


            // finalize the initialization
            p.initializationCompleting();
            
            if (App.IsCallback(callback)) {
                callback();
            }

        },  // end subscription.initialize

        pageBase: {

            object: null,
            objectName: null,
            
            activeTabChanging: function(s, e) { 
                e.reloadContentOnCallback = true;
            },

            beginCallback: function (s, e) {
                console.log('Subscription.PageBase.BeginCallback');
                var tab = s.GetActiveTab();
                e.customArgs["tabName"] = tab.name;
            },

            initializationCompleting: function () {
                subscription.pageBase.object.PerformCallback();
            }

        }   // end subscription.pageBase

    }




    window.Subscription = Interface;

});