// forecastLock.js

define(function () {
    var key = "__data";
    var nodeId = "data-forecast-lock-list";

    function getSelectedYear() {
        return $("input[name='ctlYear']").val();
    }

    function loadData(year) {
        LoadingPanel.Show();
        $.ajax({
            type: "GET",
            url: "/Data/GetForecastLocks",
            data: {
                year: year
            }
        }).done(function (items) {
            if (items) {
                var $node = $("#" + nodeId);
                $node.data(key, items)
                render($node);
            }
        }).always(function () {
            LoadingPanel.Hide();
        });
    }

    function reload() {
        var selectedYear = getSelectedYear();
        loadData(selectedYear);
    }

    function render(node) {
        var items = node.data(key);
        if (items) {
            var $ul = $("<ul></ul>");
            items.forEach(function (i) {
                var $li = $("<li></li>").html(i.PersonnelLastName + ", " + i.PersonnelFirstName);
                var $button = $("<button></button>").html("<div class=\"dxb\">" + (i.IsLocked ? "Unlock" : "Lock") + "</div>").addClass("dxbButton_Glass dxbButtonSys dxbTSys");
                $button.data(key, i);
                $button.bind("click", function () {
                    var data = $(this).data(key);
                    if (data) {
                        if (data.IsLocked) {
                            removeForecastLock(data, node);
                        } else {
                            addForecastLock(data, node);
                        }
                    }
                });
                $li.append($button);
                $ul.append($li);
            });
            node.empty().append($ul);
        }
    }

    function addForecastLock(e, node) {
        App.Confirm("Are you sure you want to lock the forecast?", function () {
            var selectedYear = getSelectedYear();
            var personnelId = e.PersonnelID;
            LoadingPanel.Show();
            $.ajax({
                url: "/Data/AddForecastLock",
                type: "POST",
                data: {
                    year: selectedYear,
                    personnelId: personnelId
                }
            }).done(function (response) {
                if (response) {
                    e.IsLocked = true;
                    render(node);
                }
            }).always(function () {
                LoadingPanel.Hide();
            });
        });
    }

    function removeForecastLock(e, node) {
        App.Confirm("Are you sure you want to unlock the forecast?", function () {
            var selectedYear = getSelectedYear();
            var personnelId = e.PersonnelID;
            LoadingPanel.Show();
            $.ajax({
                url: "/Data/RemoveForecastLock",
                type: "POST",
                data: {
                    year: selectedYear,
                    personnelId: personnelId
                }
            }).done(function (response) {
                if (response) {
                    e.IsLocked = false;
                    render(node);
                }
            }).always(function () {
                LoadingPanel.Hide();
            });
        });
    }

    window.ForecastLock = {
        Initialize: function () {
            DevEx.Controls.GetByName("ctlYear").ValueChanged.AddHandler(reload);
            reload();
        }
    };
});