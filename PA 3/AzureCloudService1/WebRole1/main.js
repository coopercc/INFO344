$(function () {
    indexSize();
    totalCrawled();
    LastTen();
    QueueSize();
    getStatus();
    getCPU();
    getMem();

    setInterval(indexSize, 3000);
    setInterval(totalCrawled, 3000);
    setInterval(LastTen, 3000);
    setInterval(QueueSize, 3000);
    setInterval(getStatus, 3000);
    setInterval(getCPU, 3000);
    setInterval(getMem, 3000);

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
                $("#indexed").html(data.d);
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
                $("#queueSize").html(data.d);
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
                $("#total").html(data.d);
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
                var list = data.d;
                $("#lastTen").empty();
                for (var i = 0; i < list.length; i++) {
                    $("#lastTen").append("<tr><td>" + list[i] + "<td></tr>");
                }
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }

    function getStatus() {
        $.ajax({
            url: "Admin.asmx/CrawlState",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                $("#state").html(data.d);
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }

    function getCPU() {
        $.ajax({
            url: "Admin.asmx/CpuUsage",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                $("#cpu").html(data.d);
                console.log("CPU: " + data.d);
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }

    function getMem() {
        $.ajax({
            url: "Admin.asmx/MemUsage",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                $("#mem").html(data.d);
                console.log("MEM: " + data.d);
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
    Error URLs

*/