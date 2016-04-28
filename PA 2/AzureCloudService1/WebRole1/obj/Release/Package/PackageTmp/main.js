$(function () {
    $('#query').on('keyup', function () {
        
        $.ajax({
            url: "GetQuerySuggestions.asmx/SearchTrie",
            data: "{str: '" + $(this).val() + "'}",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                $('#display').html("");
                $.each(data.d, function (i, str) {

                    $('#display').append(str + "<br/>");
                })
               // $('#display').append(JSON.stringify(data.d, null, 2));
            },
            error: function (x, y, z) {
                console.log(x);

                console.log(x.responseText + "  " + x.status);
            }
        });
    });


});