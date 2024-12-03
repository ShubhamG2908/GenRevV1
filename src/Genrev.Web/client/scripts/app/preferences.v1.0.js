// preferences.js

define(function () {

    var Interface = {

        Initialize: function() {preferences.initialize();},

        Save: function () { preferences.save(); },

        ResetToDefault: function () { preferences.resetToDefault();}

    }


    var preferences = {

        initialize: function(){
            console.log('initializing preferences');
            var saveButton = DevEx.Controls.GetByName("PreferencesSave");
            saveButton.Click.AddHandler(function (s, e) { preferences.save(); });

            var resetButton = DevEx.Controls.GetByName("PreferencesReset");
            resetButton.Click.AddHandler(function (s, e) { preferences.resetToDefault(); });

            //var color0 = DevEx.Controls.GetByName("Color0");
            //color0.ValueChanged.AddHandler(function (s, e) { alert("changed");})
        },

        save: function () {
            $.ajax({
                type: "POST",
                url: "/Preferences/Save",
                data: $("#PreferencesEditForm").serialize(),
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
        },

        resetToDefault: function () {
            var color0 = DevEx.Controls.GetByName("Color0");
            var color1 = DevEx.Controls.GetByName("Color1");
            var color2 = DevEx.Controls.GetByName("Color2");
            var color3 = DevEx.Controls.GetByName("Color3");

            color0.SetValue("");
            color1.SetValue("");
            color2.SetValue("");
            color3.SetValue("");


        }

    }

    window.Preferences = Interface;
});