define(function () {

    /*

    App.Dates.GetFullMonthFromShortMonth(shortMonth)
    App.Initialize
    App.IsCallback(cbTest)
    App.Errors
        App.Errors.NotImplemented
    App.Navigation(url)
    App.Alert(msg, icon, title)
    App.Confirm(prompt, cbOk, cbCancel, icon, title)
    App.Popup.Show(url, data, cbCompleted)
    App.Popup.Hide(result)


    




    */

    // ====================================
    //      App
    // ====================================
    var AppInterface = {

        Dates: {

            GetFullMonthFromShortMonth: function (shortMonth) {

                var s = shortMonth.toLowerCase();

                switch (s) {
                    case "jan": return "January";
                    case "feb": return "February";
                    case "mar": return "March";
                    case "apr": return "April";
                    case "may": return "May";
                    case "jun": return "June";
                    case "jul": return "July";
                    case "aug": return "August";
                    case "sep": return "September";
                    case "oct": return "October";
                    case "nov": return "November";
                    case "dec": return "December";
                }

            }

        },

        Initialize: function () {

            //console.log('app initialize');

        },  // end AppInterface.Initialize
        
        IsCallback: function(cbTest) {
            if (typeof (cbTest) == "function") {
                return true;
            } else {
                return false;
            }
        },

        Callback: function(cb) {
            if (App.IsCallback(cb)) {
                cb();
            }
        },

        Try: function(f) {
            try {
                f();
            } catch (err) { }
        },

        Errors: {

            NotImplemented: {
                name: 'NotImplemented',
                message: 'This has not yet been implemented'
            },

            Show: function(message) {
                App.Alert(message, DialogIcons.Error, "Error");
            },

            ShowGeneral: function () {
                App.Alert("We're sorry, we seem to have run into an issue with that request.  Please contact our support desk if this problem continues.", DialogIcons.Error, "Error");
            }

        },  // end App.Errors

        Navgiate: function (url) {
            // This navigates to a different page.
            // It does not load a content area for the shell (as in the main user area)
            // Do load a shell content area in SPA-ish style, use Shell.Load() after login
            window.location = url;
        },  // end App.Navigate

        Popup: {

            Show: function (options) {

                core.popup.show(options);
                
            },   // end App.Popup.Show

            Hide: function (response) {

                core.popup.hide(response);

            }   // end App.Popup.Hide

        },  // end App.Popup

        Alert: function (message, icon, title) {
            if (message == null) {
                core.dialogs.alert.setMessage("");
            } else {
                core.dialogs.alert.setMessage(message);
            }

            if (icon == null) {
                core.dialogs.alert.setIcon(DialogIcons.Info);
            } else {
                core.dialogs.alert.setIcon(icon);
            }

            if (title == null) {
                core.dialogs.alert.setTitle("Alert");
            } else {
                core.dialogs.alert.setTitle(title);
            }

            core.dialogs.alert.dxElement.Show();
        },  // end App.Alert

        Confirm: function (prompt, cbOk, cbCancel, icon, title) {

            if (prompt == null) {
                core.dialogs.confirm.setMessage("");
            } else {
                core.dialogs.confirm.setMessage(prompt);
            }

            if (icon == null) {
                core.dialogs.confirm.setIcon(DialogIcons.Question);
            } else {
                core.dialogs.confirm.setIcon(icon);
            }

            if (title == null) {
                core.dialogs.confirm.setTitle("Confirm");
            } else {
                core.dialogs.confirm.setTitle(title);
            }

            core.dialogs.confirm.okCallback = cbOk;
            core.dialogs.confirm.cancelCallback = cbCancel;
            
            core.dialogs.confirm.dxElement.Show();

        }   // end App.Confirm

    }   // end App





    // -------------------
    //      App Private
    // -------------------
    var core = {
        
        popup: {

            /* 
            
                Popup Naming:
                {ID} is a random ID generated for each popup

                Anchor Element:     PopupContainer_{ID}
                DevEx Control:      PopupControl_{ID}
                Content Div:        PopupContent_{ID}
                
                Currently handles nested popups in stack (first in last out) behavior.
                No current handling for variable order items.
                App.Popup.Hide() always hides the last opened popup.


            */
            
            lastReturn: null,

            lastID: null,

            show: function (options) {

                var defaultOptions = {
                    url: null,
                    data: null,
                    type: 'GET',
                    options: {
                        width: 500,
                        height: 300,
                        title: " ",
                        allowDrag: false,
                        allowResize: false
                    },
                    opened: function() {
                        
                    },
                    done: function (response) {

                    },
                    error: function (response) {
                        App.Errors.ShowGeneral();
                    }
                }

                options.url = options.url || defaultOptions.url;
                options.data = options.data || defaultOptions.data;
                options.type = options.type || defaultOptions.type;

                options.options.width = options.options.width || defaultOptions.options.width;
                options.options.height = options.options.height || defaultOptions.options.height;
                options.options.title = options.options.title || defaultOptions.options.title;
                options.options.allowDrag = options.options.allowDrag || defaultOptions.options.allowDrag;
                options.options.allowResize = options.options.allowResize || defaultOptions.options.allowResize;
                options.options.id = null;

                options.opened = options.opened || defaultOptions.opened;
                options.done = options.done || defaultOptions.done;
                options.error = options.error || defaultOptions.error;

                // create popup div container
                var baseID = Math.random().toString(36).substring(2, 11);
                options.options.id = baseID;
                core.popup.createNewPopupAnchor(baseID);
                
                // first get the view they're requesting
                $.ajax({
                    type: options.type,
                    url: options.url,
                    data: options.data,
                    success: function (contentResponse) {

                        // now get the popup wrapper and inject the response content
                        $.ajax({
                            url: '/Dialogs/Popup',
                            type: 'GET',
                            data: {
                                id: options.options.id,
                                width: options.options.width,
                                height: options.options.height,
                                title: options.options.title,
                                allowDrag: options.options.allowDrag,
                                allowResize: options.options.allowResize
                            },
                            success: function (popupResponse) {

                                $("#PopupContainer_" + baseID).empty().append(popupResponse);
                                $("#PopupContent_" + baseID).empty().append(contentResponse);

                                core.popup.lastID = baseID;

                                var popupObject =
                                    ASPxClientControl.GetControlCollection().GetByName("PopupControl_" + baseID);

                                popupObject.CloseUp.AddHandler(function (s, e) {
                                    core.popup.events.closing(core.popup.lastReturn, options.done);
                                });

                                popupObject.Show();

                                options.opened();

                            },
                            error: function (popupResponse) {
                                options.error(popupResponse);
                            }
                        });

                    },
                    error: function (contentResponse) {
                        options.error(contentResponse);
                    }
                });


            },  // end core.popup.show

            events: {

                closing: function (response, callback) {

                    if (App.IsCallback(callback)) {
                        callback(response);
                    }
                    
                    window.setTimeout(function () {
                        // remove the anchor
                        $("#PopupContainer_" + core.popup.lastID).remove();

                    }, 100);

                }   // end core.popup.events.closing

            },  // end core.popup.events

            hide: function (response) {

                // get last ID
                var id = core.popup.lastID;
                core.popup.lastReturn = response;

                var obj = ASPxClientControl.GetControlCollection().GetByName("PopupControl_" + id);
                obj.Hide();

            },   // end core.popup.hide

            createNewPopupAnchor: function (id) {
                var element = document.createElement('div');
                element.id = "PopupContainer_" + id;
                var body = document.getElementsByTagName('body')[0].appendChild(element);
                return id;
            }   // end core.popup.createnewPopupAnchor

        },  // end core.popup



        dialogs: {

            confirm: {

                dxElement: null,
                msgElement: null,
                iconElement: null,
                okCallback: null,
                cancelCallback: null,

                init: function (dxPopup, okCallback, cancelCallback) {
                    core.dialogs.confirm.dxElement = dxPopup;
                    core.dialogs.confirm.okCallback = okCallback;
                    core.dialogs.confirm.cancelCallback = cancelCallback;
                    core.dialogs.confirm.msgElement = $("#mb-confirm-msg");
                    core.dialogs.confirm.iconElement = $("#mb-confirm-icon");
                },

                ok: function () {
                    core.dialogs.confirm.dxElement.Hide();
                    if (App.IsCallback(core.dialogs.confirm.okCallback)) {
                        core.dialogs.confirm.okCallback();
                    }
                    
                },

                cancel: function () {
                    core.dialogs.confirm.dxElement.Hide();
                    if (App.IsCallback(core.dialogs.confirm.cancelCallback)) {
                        core.dialogs.confirm.cancelCallback();
                    }
                },

                setMessage: function (msg) {
                    msg = msg.replace(/(?:\r\n|\r|\n)/g, '<br />');
                    core.dialogs.confirm.msgElement.empty().append(msg);
                },

                setIcon: function (icon) {
                    el = core.dialogs.confirm.iconElement;

                    el.removeClass(function (index, css) {
                        // remove all sprite-ico* classes
                        return (css.match(/(^|\s)sprite-ico\S+/g) || []).join(' ');
                    });

                    el.addClass('sprite-ico').addClass(DialogIcons.classname(icon));
                },

                setTitle: function (title) {
                    core.dialogs.confirm.dxElement.SetHeaderText(title);
                }

            },  // end core.dialogs.confirm




            alert: {

                dxElement: null,
                msgElement: null,
                iconElement: null,

                init: function (dxPopup) {
                    core.dialogs.alert.dxElement = dxPopup;
                    core.dialogs.alert.msgElement = $("#mb-alert-msg");
                    core.dialogs.alert.iconElement = $("#mb-alert-icon");
                },

                setMessage: function (msg) {
                    msg = msg.replace(/(?:\r\n|\r|\n)/g, '<br />');
                    core.dialogs.alert.msgElement.empty().append(msg);
                },

                setIcon: function (icon) {
                    el = core.dialogs.alert.iconElement;

                    el.removeClass(function (index, css) {
                        // remove all sprite-ico* classes
                        return (css.match(/(^|\s)sprite-ico\S+/g) || []).join(' ');
                    });

                    el.addClass('sprite-ico').addClass(DialogIcons.classname(icon));
                },

                setTitle: function (title) {
                    core.dialogs.alert.dxElement.SetHeaderText(title);
                }

            }   // end core.dialogs.alert

        }   // end core.dialogs




    }   // end app



    // ====================================
    //      Namespace
    // ====================================
    var NamespaceInterface = {
        ns: function (topLevelNs, nsString) {
            console.log('ns called: ' + nsString);
        }
    }


    // ====================================
    //      Dialogs
    // ====================================
    var DialogIconsInterface = {

        None: -1,
        Info: 0,
        Ok: 1,
        Warning: 2,
        Error: 3,
        Question: 4,
        Save: 5,
        Delete: 6,

        classname: function (icon) {

            switch (icon) {
                case 0: return 'sprite-ico-info-48';
                case 1: return 'sprite-ico-ok-48';
                case 2: return 'sprite-ico-warning-48';
                case 3: return 'sprite-ico-delete-48';
                case 4: return 'sprite-ico-question-48';
                case 5: return 'sprite-ico-save-48';
                case 6: return 'sprite-ico-delete-48';
                default: return 'sprite-ico-info-48';
            }
        }   // end window.DialogIcons.classname

    }   // end window.DialogIcons





    // ====================================
    //      Exports
    // ====================================


    window.App = AppInterface;
    window.core = core;   // for system/internal use
    window.DialogIcons = DialogIconsInterface,
    window.ns = NamespaceInterface.ns;
            
});