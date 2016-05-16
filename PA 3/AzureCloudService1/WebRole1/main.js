$(function () {
    $("#start").click(function () {
        $.ajax({
            url: "Admin.asmx/Start",
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

    $("#stop").click(function () {
        $.ajax({
            url: "Admin.asmx/Start",
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
            url: "Admin.asmx/Start",
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
});

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