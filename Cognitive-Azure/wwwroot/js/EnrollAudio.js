
var checkEnrollmentInterval = 10000;

$(function () {
    $("#startRecording").click(function(e) {
        e.preventDefault();
        startRecording();
    });

    $("#stopRecording").click(function(e) {
        e.preventDefault();
        stopRecordingProcess(true);
    });

    setTimeout(checkEnrollment, 1000);
});


function checkEnrollment() {
    var id = $("#Id").val();

    var url = "/Audio/CheckEnrollmentStatus/" + id;

    $.ajax({
        dataType: "json",
        url: url,
        success: updateEnrollmentStatus
    });
}

function updateEnrollmentStatus(data) {

    var prettifiedResponse = syntaxHighlight(data);

    prettifiedResponse = prettifiedResponse.replace(/{/g, "{<br />").replace(/}/g, "<br />}").replace(/,/g, ",<br />");

    $("#status").html(prettifiedResponse);

    if (data.enrollmentStatus !== "Enrolled") {
        setTimeout(checkEnrollment, checkEnrollmentInterval);
    } else {
        stopRecordingProcess(true);
    }
}

function syntaxHighlight(json) {
    if (typeof json != 'string') {
        json = JSON.stringify(json, undefined, 2);
    }
    json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
        var cls = 'number';
        if (/^"/.test(match)) {
            if (/:$/.test(match)) {
                cls = 'key';
            } else {
                cls = 'string';
            }
        } else if (/true|false/.test(match)) {
            cls = 'boolean';
        } else if (/null/.test(match)) {
            cls = 'null';
        } 
        return '<span class="' + cls + '">' + match + '</span>';
    });
}