// app/userShell.js

define(function () {


    /***********************************************

        Shell.Initialize(callback)
        Shell.Load(url, data, cbSuccess, cbError)
        Shell.SetContent(content)

    ***********************************************/




    var NAVBAR_CONTROL_NAME = "LeftNavBar";
    var MAIN_CONTENT_ELEMENT_ID = "ContentSplitter_1_CC";

    // history.js
    var location = window.history.location || window.location;

    

    // ====================================
    //      Shell (Interface)
    // ====================================
    var Interface = {

        // Initializes the base user shell including navbar, member area, search, dialogs etc
        Initialize: function (callback) { shell.initialize(callback); },

        // Loads the specified url via ajax get into the main content area
        // optionally passes the specified data and calls the success or error callbacks appropriately
        Load: function (url, data, cbSuccess, cbError) { shell.load(url, data, cbSuccess, cbError); },

        SetContent: function (content) { shell.setContent(content) },

        SubmitFeedback: function () { memberbar.submitFeedback(); },
        CancelFeedback: function () { memberbar.cancelFeedback(); }

    }   // end Shell








    // ====================================
    //      Private Objects
    // ====================================
    

    // --------------------------
    //      shell
    // --------------------------

    var shell = {


        initialize: function(callback) {
            
            $.when(
                logolink.initialize(),
                navbar.initialize(),
                memberbar.initialize(),
                //search.initialize(),
                dialogs.initialize()
                ).done(function () {

                if (App.IsCallback(callback)) {
                    callback();
                }

            });

        },  // end initialize


        load: function load(url, data, cbSuccess, cbError) {

            if (data) {
                $.get(url, data, function (content) {
                    shell.setContent(content, url);
                    if (App.IsCallback(cbSuccess)) {
                        cbSuccess();
                    }
                }).fail(function () {
                    if (App.IsCallback(cbError)) {
                        cbError();
                    }
                });

            } else {
                $.get(url, function (content) {
                    shell.setContent(content, url);
                    if (App.IsCallback(cbSuccess)) {
                        cbSuccess();
                    }
                }).fail(function () {
                    if (App.IsCallback(cbError)) {
                        cbError();
                    }
                });
            }

        },   // end shell.load

        
        setContent: function (content, url, title, state) {

            url = url || null;
            title = title || null;
            state = state || null;

            if (url != null) {
                history.pushState(state, title, url);
            }

            $("#" + MAIN_CONTENT_ELEMENT_ID).empty().append(content);

            // this may be called from a post with a self-url return, thus url
            // may not change or be present
            if (url != null) {
                navbar.object.SetSelectedItem(DevEx.NavBar.GetItemByUrl(navbar.object, url));
            }
            
        }   // end shell.setContent

    }   // end shell

    



    var logolink = {

        initialize: function () {

            var def = $.Deferred();

            $("#main-logo-link").click(function () {
                shell.load("/Home");
                return false;
            });

            def.resolve();

        }

    }

    
    
    // --------------------------
    //      memberbar
    // --------------------------

    var memberbar = {

        initialize: function () {

            var
                mbUser = $.Deferred(),
                mbFeedback = $.Deferred();

            $.ajax({
                url: "/MemberBar/UserPopup",
                type: 'GET',
                success: function (res) {
                    $("#memberbar-user-popup-container").empty().append(res);
                    mbUser.resolve();
                }
            });

            $.ajax({
                url: "/MemberBar/FeedbackPopup",
                type: 'GET',
                success: function (res) {
                    $("#memberbar-feedback-popup-container").empty().append(res);
                    mbFeedback.resolve();
                }
            });

            return $.Deferred(function (def) {
                $.when(mbUser, mbFeedback).done(function () {
                    def.resolve();
                });
            });

        },   // end memberbar.initialize

        submitFeedback: function () {
            $("#feedbackSpinner").show();
            DevEx.Controls.GetByName("feedbackSubmit").SetEnabled(false);
            var feedbackText = DevEx.Controls.GetByName("FeedbackComments").GetValue();
            $.ajax({
                url: "/MemberBar/FeedbackSubmit",
                type: "POST",
                data: {
                    feedback: feedbackText
                },
                success: function(res){
                    DevEx.Controls.GetByName("memberBarFeedbackPopup").Hide();
                    DevEx.Controls.GetByName("FeedbackComments").SetText("");
                    $("#feedbackSpinner").hide();
                    DevEx.Controls.GetByName("feedbackSubmit").SetEnabled(true);
                }
            });
        },

        cancelFeedback: function(){
            DevEx.Controls.GetByName("memberBarFeedbackPopup").Hide();
            DevEx.Controls.GetByName("FeedbackComments").SetText("");
        }

    }   // end memberbar



    // --------------------------
    //      dialogs
    // --------------------------

    var dialogs = {

        initialize: function () {

            var alertDone = $.Deferred();
            var confirmDone = $.Deferred();

            $.ajax({
                url: "/Dialogs/AlertPopup",
                type: 'GET',
                success: function (res) {
                    $("#app-dialog-alert-popup-container").empty().append(res);
                    core.dialogs.alert.init(popupDialogAlert);
                    alertDone.resolve();
                }
            });

            $.ajax({
                url: "/Dialogs/ConfirmPopup",
                type: 'GET',
                success: function (res) {
                    $("#app-dialog-confirm-popup-container").empty().append(res);
                    core.dialogs.confirm.init(popupDialogConfirm);
                    confirmDone.resolve();
                }
            });

            return $.Deferred(function (def) {
                $.when(alertDone, confirmDone).done(function () {
                    def.resolve();
                });
            });

        }   // end dialogs.initialize

    }   // end dialogs



    // --------------------------
    //      search
    // --------------------------

    var search = {

        initialize: function () {

            var r = $.Deferred();

            console.log('todo: init search');
            r.resolve();

            return $.Deferred(function (def) {
                $.when(r).done(function () {
                    def.resolve();
                });
            });

        }   // end search.initialize

    }   // end search







    // --------------------------
    //      navbar
    // --------------------------

    var navbar = {
        // https://documentation.devexpress.com/#AspNet/DevExpressWebMVCScriptsMVCxClientNavBarMembersTopicAll

        object: null,

        clickHandler: function (s, e) {
            // the devex object in the razor view needs at least one dummy event defined
            // otherwise the client side events never fire...
            Interface.Load(e.item.GetNavigateUrl());
            e.htmlEvent.preventDefault();
            return false;
        },  // end navbar.ClickHandler

        initialize: function () {

            var done = $.Deferred();

            // reference the object directly for the time being
            // https://www.devexpress.com/Support/Center/Question/Details/T390285
            navbar.object = DevEx.Controls.GetByName(NAVBAR_CONTROL_NAME);
            navbar.object.ItemClick.AddHandler(function (s, e) { navbar.clickHandler(s, e); });

            window.setTimeout(function () {

                window.onpopstate = function (event) {

                    $.ajax({
                        url: window.location.pathname,
                        success: function (res) {

                            $("#" + MAIN_CONTENT_ELEMENT_ID).empty().append(res);

                            DevEx.NavBar.SetSelection(
                                navbar.object,
                                DevEx.NavBar.GetItemByUrl(
                                    navbar.object,
                                    window.location.pathname, true)
                                );
                        }
                    });
                }
            }, 1000);


            done.resolve();
            return $.Deferred(function (def) {
                $.when(done).done(function () {
                    def.resolve();
                });
            });

        }   // end navbar.Initialize


    }   // end navbar









    // ====================================
    //      Shell Exports
    // ====================================
    window.Shell = Interface;
    
});


