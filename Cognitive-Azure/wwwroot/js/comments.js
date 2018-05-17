$(function () {
    var timeout = null;

    $("#Comment").keyup(function () {
        clearTimeout(timeout);

        timeout = setTimeout(postComment, 1000, this.value);
    });
});

function postComment() {
    var form = $("form").serializeArray();

    $.post("/Images/AnalyseComment", form, onSuccess);
}

function onSuccess(result) {
    var score = result.score * 100;

    if (score > 80) {
        $("#reaction").attr("src", "/img/emoji/Great.png");
    } else if (score > 60) {
        $("#reaction").attr("src", "/img/emoji/Happy.png");
    } else if (score > 40) {
        $("#reaction").attr("src", "/img/emoji/Neutral.png");
    } else if (score > 20) {
        $("#reaction").attr("src", "/img/emoji/Bad.png");
    } else {
        $("#reaction").attr("src", "/img/emoji/Terrible.png");
    }

    $("#score").html(result.score);
    $("#phrases").html(result.phrases);
}