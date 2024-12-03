// devex.js

define(function () {

    /*****************************************

        DevEx.Controls.GetByName(controlName)
        DevEx.NavBar.SetSelection(navbar, item)
        DevEx.NavBar.GetItemByUrl(navbar, url, isRelativePath)


    ******************************************/

    var Interface = {

        Controls: {
            GetByName: function(controlName) {
                return ASPxClientControl.GetControlCollection().GetByName(controlName);
            },
            GetValue: function (controlName) {
                return ASPxClientControl.GetControlCollection().GetByName(controlName).GetValue();
            }
        },  // end DevEx.Controls.GetByName

        Grid: {
            GetFocusedKey: function (grid) {
                var vi = grid.GetFocusedRowIndex();
                return grid.GetRowKey(vi);
            }
        },

        NavBar: {

            // sets a selection as well as group expand/collapse
            // does not perform any navigation, only item and group selection state
            SetSelection: function (navbar, item) {

                navbar.SetSelectedItem(item);
                if (item.group.GetExpanded()) {
                    return;
                } else {
                    item.group.SetExpanded(true);
                }
            },  // end DevEx.NavBar.SetSelection


            // Returns the NavBarItem that matches the specified url, or null if not found
            GetItemByUrl: function (navbar, url, isRelativePath) {

                if (isRelativePath) {
                    url = window.location.protocol + "//" + window.location.host + url;
                }

                var groupCount = navbar.GetGroupCount();

                for (i = 0; i < groupCount; i++) {

                    var group = navbar.GetGroup(i);
                    var itemCount = group.GetItemCount();

                    for (j = 0; j < itemCount; j++) {

                        var item = group.GetItem(j);
                        var itemUrl = item.GetNavigateUrl();

                        if (item.GetNavigateUrl() == url) {
                            return item;
                        }
                    }
                }

                return null;

            }   // end DevEx.NavBar.GetItemByUrl

        }   // end DevEx.NavBar

    }   // end DevEx.Interface




    var dx = {

        get: function (controlName) {
            return ASPxClientControl.GetControlCollection().GetByName(controlName);
        },

        grid: {

            options: {

                Default: function (gridName, stretchHeightRefContainerId, devexLayoutSplitterName) {
                    return {
                        name: gridName,
                        stretch: {
                            enabled: false,
                            devexLayoutSplitterName: devexLayoutSplitterName,
                            containerId: stretchHeightRefContainerId
                        },
                        serverCallbackParams: {}
                    }
                }

            }

        },


        Grid: function (config) {

            var defaultConfig = new dx.grid.options.Default();
            config = $.extend(true, defaultConfig, config);

            var grid = {

                initialize: function () {

                    grid.name = config.name;

                    // use config argument to supply values
                    if (config.stretch.enabled) {
                        grid.sys.initStretch();
                    }
                    if (config.serverCallbackParams.length > 0) {
                        grid.events.beginCallback.addParams(config.serverCallbackParams);
                    }

                    grid.events.endCallback.coreHandler();

                },

                name: null,
                isStretched: false,

                events: {

                    rowDoubleClick: {

                        handlers: [],

                        reconcileHandlers: function () {

                            var rdc = grid.events.rowDoubleClick;
                            var count = rdc.handlers.length;

                            $('[data-dym-cell="' + grid.name + '"]').dblclick(function () {

                                var idx = $(this).attr('data-dym-visibleIndex');
                                var key = dx.get(grid.name).GetRowKey(idx);

                                var sender = dx.get(grid.name);
                                var args = new grid.events.rowDoubleClick.RowDoubleClickEventArgs(key);

                                for (var i = 0; i < count; i++) {
                                    var h = rdc.handlers[i];
                                    if (App.IsCallback(h)) {
                                        h(sender, args);
                                    }
                                }

                            });

                        },

                        RowDoubleClickEventArgs: function (clickedIndex) {
                            return {
                                clickedIndex: clickedIndex
                            }
                        },

                        addHandler: function (handler) {

                            $('[data-dym-cell="' + grid.name + '"]').dblclick(function () {

                                var idx = $(this).attr('data-dym-visibleIndex');
                                var key = dx.get(grid.name).GetRowKey(idx);

                                var sender = dx.get(grid.name);
                                var args = new grid.events.rowDoubleClick.RowDoubleClickEventArgs(key);


                                if (App.IsCallback(handler)) {
                                    grid.events.rowDoubleClick.handlers.push(handler);
                                    handler(sender, args);
                                }

                            });

                        }

                    },

                    endCallback: {

                        addHandler: function (handler) {
                            dx.get(grid.name).EndCallback.AddHandler(function (s, e) {
                                if (App.IsCallback(handler)) {
                                    handler(s, e);
                                }
                            });
                        },

                        coreHandler: function () {
                            dx.get(grid.name).EndCallback.AddHandler(function (s, e) {
                                grid.sys.reconcileStretch();
                                grid.events.rowDoubleClick.reconcileHandlers();
                            });
                        },

                    },

                    beginCallback: {

                        params: {},	// key/value pair to send back through as 

                        addParam: function (k, v) {
                            grid.events.beginCallback.params[k] = v;
                        },

                        addParams: function (p) {
                            for (k in p) {
                                grid.events.beginCallback.params[k] = p[k];
                            }
                        },

                        addHandler: function (handler) {
                            dx.get(grid.name).BeginCallback.AddHandler(function (s, e) {
                                if (App.IsCallback(handler)) {
                                    handler(s, e);
                                }
                            });
                        },

                        coreHandler: function () {
                            dx.get(grid.name).BeginCallback.AddHandler(function (s, e) {
                                var p = grid.events.beginCallback.params
                                for (k in p) {
                                    e.customArgs[k] = p[k];
                                }
                            });
                        }

                    }


                },	// end grid.events

                methods: {

                    refresh: function () {
                        dx.get(grid.name).Refresh();
                    },

                    getObject: function () {
                        return dx.get(grid.name);
                    },

                    toggleStretch: function () {

                        if (grid.isStretched) {
                            grid.sys.stretch();
                        } else {
                            grid.sys.unstretch();
                        }

                    },	// end grid.methods.toggleStretch

                },	// end grid.methods

                sys: {

                    initStretch: function () {

                        window.addEventListener('resize', function () {
                            grid.sys.stretch();
                        });

                        grid.toggleStretch();

                    },

                    stretch: function () {

                        var splitter = dx.get(config.stretch.devexLayoutSplitterName);
                        var h = splitter.GetPane(1).GetClientHeight();
                        var outerHeight = $(config.stretch.containerId).outerHeight(true);
                        var buffer = 60;

                        h = h - buffer - outerHeight;

                        dx.get(grid.name).SetHeight(h);

                        grid.isStretched = true;
                    },
                    unstretch: function () {
                        // unstretch here
                        grid.isStretched = false;
                    },
                    reconcileStretch: function () {
                        // call this after a devex callback ends
                        // at this point our isStretched state and our actual stretch
                        // may be out of sync...
                        if (config.stretch.enabled) {
                            grid.sys.stretch();
                            // and then link up the window resize event again as that'll be broken...
                            grid.sys.initStretch();
                        }

                    }
                }

            };


            grid.initialize();

            return {

                name: grid.name,

                refresh: function () { grid.methods.refresh(); },

                getObject: function () { return grid.methods.getObject(); },

                serverCallback: {
                    addParam: function (key, value) { grid.methods.addParam(key, value); },
                    addParams: function (params) { grid.methods.addParams(params); },
                    addBeginHandler: function (handler) { grid.events.beginCallback.addHandler(handler); },
                    addEndHandler: function (handler) { grid.events.endCallback.addHandler(handler); },
                },

                rowDoubleClick: {
                    addHandler: function (handler) { grid.events.rowDoubleClick.addHandler(handler); },
                },

                toggleStretch: function () { return grid.methods.toggleStretch(); },


            }

        }

    }


    window.devex = dx;
    window.DevEx = Interface;


});