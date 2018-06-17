$(() => {

    $(".btn").on("click", function () {
        const id = parseInt($("#user-id").val());
        const url = $("#url").val();
        $("#url").val('');
        $.post("/home/NewShortUrl", { id, url }, function (Url) {
            $(".new-url").empty();
            $(".new-url").append(`
                <h4>Your new url <a href = "${Url.ShortUrl}">${Url.ShortUrl}<a/> copy for yor records<h4/>
            `)
        })
    })

});