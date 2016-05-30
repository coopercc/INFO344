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
        $("playerStats").empty();
        function onSuccess(data) {
            var nbaResults = data;
            if (nbaResults != "") {
                var well = $("<div><div>").addClass("well");
                well.append($("<h3></h3>").text(nbaResults["name"]));

                var table = $("<table></table>").addClass("table");
                var headerRow = $("<tr></tr>");
                headerRow.append($("<th></th>").text("Team"));
                headerRow.append($("<th></th>").text("Games Played"));
                headerRow.append($("<th></th>").text("Off Rebounds"));
                headerRow.append($("<th></th>").text("Def Rebounds"));
                headerRow.append($("<th></th>").text("Tot Rebounds"));


                var playerRow = $("<tr></tr>");
                playerRow.append($("<td></td>").text(nbaResults["team"]));
                playerRow.append($("<td></td>").text(nbaResults["gp"]));
                playerRow.append($("<td></td>").text(nbaResults["offReb"]));
                playerRow.append($("<td></td>").text(nbaResults["defReb"]));
                playerRow.append($("<td></td>").text(nbaResults["totReb"]));


                table.append(headerRow);
                table.append(playerRow);
                
                
                well.append(table);

                var table = $("<table></table>").addClass("table");
                var headerRow = $("<tr></tr>");
                headerRow.append($("<th></th>").text("3 Pointers"));
                headerRow.append($("<th></th>").text("Assists"));
                headerRow.append($("<th></th>").text("Turnovers"));
                headerRow.append($("<th></th>").text("Steals"));
                headerRow.append($("<th></th>").text("Blocks"));
                headerRow.append($("<th></th>").text("PPG"));

                var playerRow = $("<tr></tr>");
                playerRow.append($("<td></td>").text(nbaResults["threept"]));
                playerRow.append($("<td></td>").text(nbaResults["ast"]));
                playerRow.append($("<td></td>").text(nbaResults["turnovers"]));
                playerRow.append($("<td></td>").text(nbaResults["stl"]));
                playerRow.append($("<td></td>").text(nbaResults["block"]));
                playerRow.append($("<td></td>").text(nbaResults["ppg"]));

                table.append(headerRow);
                table.append(playerRow);

                well.append(table);
                $("#playerStats").append(well);
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