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
        var searchQuery = $("#query").val();

        function onSuccess(data) {
            console.log(data);
            var nbaResults = JSON.stringify(data);
            if (nbaResults != "") {
                console.log(nbaResults);
            }
        }

        $.ajax({
            crossDomain: true,
            contentType: "application/json; charset=utf-8",
            url: "http://ec2-52-37-84-201.us-west-2.compute.amazonaws.com/search.php",
            data: { name: searchQuery },
            dataType: "jsonP",
            success: onSuccess
        });



        $.ajax({
            url: "Admin.asmx/searchUrl",
            data: JSON.stringify({ searchPhrase: searchQuery }),
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                $("#results").html("");

                for (var i = 0; i < data.d.length; i++ ) {
                    var div = $("<div class='well'></div>");
                    var link = $("<a href=" + data.d[i] + "></a>")
                    link.html(data.d[i]);
                    div.html(link);
                    $("#results").append(div);
                    console.log(data.d[i]);
                }




            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        })
    });


});