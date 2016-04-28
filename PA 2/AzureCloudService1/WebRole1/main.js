$(function () {
    //When there is a keyup event in the textbox, it calls this
    $('#query').on('keyup', function () {
        //Calls the SearchTrie method with the text in the textbox
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
            },
            error: function (x, y, z) {
                console.log(x);
                console.log(x.responseText + "  " + x.status);
            }
        });
    });


});