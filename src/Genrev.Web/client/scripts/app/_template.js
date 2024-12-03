// app/template.js

// requires
//  app/app     v1.0
//  app/devex   v1.0
//
//
//



define(function () {


    /***********************************************
        General Interface Items

        Kb.Search.Initialize(callback)
        // etc etc

    ***********************************************/


    // CONSTANTS
    var CONSTANT_IDENTIFIER = "value";

    // ====================================
    //      Interface
    // ====================================

    var Interface = {

        Search: {
            Initialize: function (callback) { kb.initialize(callback); }

        }

    }



    // ====================================
    //      Private Objects
    // ====================================

    var kb = {
        initialize: function (callback) {
            // init logic here
        }
    }



    // ====================================
    //      Exports
    // ====================================

    window.Kb = Interface;


});