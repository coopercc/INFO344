$(function () {
   indexSize();
   totalCrawled();
   LastTen();
   QueueSize();

    $("#start").click(function () {
        $.ajax({
            url: "Admin.asmx/Start",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                console.log("Started crawling");
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    });

    $("#stop").click(function () {
        $.ajax({
            url: "Admin.asmx/Stop",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {

            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    });

    $("#continue").click(function () {
        $.ajax({
            url: "Admin.asmx/Continue",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {

            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    });

    $("#clear").click(function () {
        $.ajax({
            url: "Admin.asmx/Clear",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    });

    function indexSize() {
        $.ajax({
            url: "Admin.asmx/IndexSize",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                //put somewhere
                $("#indexed").html(data.d);
                console.log(data.d);
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }

    function QueueSize() {
        $.ajax({
            url: "Admin.asmx/QueueSize",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                //put somewhere
                $("#queueSize").html(data.d);
                console.log(data);
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }

    function totalCrawled() {
        $.ajax({
            url: "Admin.asmx/totalCrawled",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                //put somewhere
                $("#total").html(data.d);
                console.log(data.d);
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }

    function LastTen() {
        $.ajax({
            url: "Admin.asmx/LastTen",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                //put somewhere, returns list of strings
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }
});

/*
JQUERY FUNCTIONALITY LEFT TO INCLUDE:
    last 10 urls
    Size of Queue
    Size of Index
    Error URLs
    Worker role CPU%, Ram available

*/


/* How to do timed calls, like displaying recent 10

function executeQuery() {
$.ajax({
    url: 'url/path/here',
    success: function(data) {
        // do something with the return value here if you like
    }
});
setTimeout(executeQuery, 5000); // you could choose not to continue on failure...
}

$(document).ready(function() {
    // run the first time; all subsequent calls will take care of themselves
    setTimeout(executeQuery, 5000);
});
*/