
define(['core', 'devex'], function (app, devex) {

    var test = {

        doStuff: function () {
            alert('doing stuff');

            
            
        }

    };

    return {
        doStuff: test.doStuff
    }

});