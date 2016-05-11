$(function () {
    $("#start").onClick(function () {
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
    })
});