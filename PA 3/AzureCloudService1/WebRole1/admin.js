$(function () {
    indexSize();
    totalCrawled();
    LastTen();
    QueueSize();
    getStatus();
    getCPU();
    getMem();
    getErrors();
    getTrie();

    $('#refresh').click(function () {
        indexSize();
        totalCrawled();
        LastTen();
        QueueSize();
        getStatus();
        getCPU();
        getMem();
        getErrors();
        getTrie();
    });

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
                console.log("Clear running");

            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    });

    $("#startSearch").click(function () {
        $.ajax({
            url: "Admin.asmx/searchUrl",
            data: JSON.stringify({ searchPhrase: $("#UrlSearch").val() }),
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                $("#results").html(data.d);
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

    function getErrors() {
        $.ajax({
            url: "Admin.asmx/getErrors",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                var list = data.d;
                $("#Errors").empty();
                for (var i = 0; i < list.length; i++) {
                    $("#Errors").append("<tr><td>" + list[i] + "<td></tr>");
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
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }

    function getTrie() {
        $.ajax({
            url: "Admin.asmx/TrieStats",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                var list = data.d;
                $("#lastWord").empty();
                $("#trieCount").empty();

                $("#trieCount").html(list[0]);
                $("#lastWord").html(list[1]);
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    }
    
}); 