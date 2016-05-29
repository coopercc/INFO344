$(function () {
    //When there is a keyup event in the textbox, it calls this
    $('#query').on('keyup', function () {
        //Calls the SearchTrie method with the text in the textbox
        $.ajax({
            url: "QuerySuggestions.asmx/SearchTrie",
            data: "{str: '" + $(this).val() + "'}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                $('#display').html("");
                $.each(data.d, function (i, str) {
                    $('#display').append(str + "<br/>");
                })
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        });
    });

    $("#submitSearch").click(function () {
        $.ajax({
            url: "Admin.asmx/searchUrl",
            data: JSON.stringify({ searchPhrase: $("#query").val() }),
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                $("#results").html("");
                for(var string in data.d) {
                    $("#results").append("<div>" + string + "</div>");
                }
                $("#results").html(data.d);
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    });


});