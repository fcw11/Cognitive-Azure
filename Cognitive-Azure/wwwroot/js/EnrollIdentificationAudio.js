var checkEnrollmentInterval = 3000;
var recorder;
var audio_context;

$(function () {
    $("#startRecording").click(function (e) {
        e.preventDefault();
        startRecording();
    });

    $("#stopRecording").click(function (e) {
        e.preventDefault();
        stopRecording();
    });

    function startRecording() {
        navigator.getUserMedia({ audio: true }, onSuccess, onError);
    }

    function stopRecording() {
        recorder && recorder.stop();
        recorder.exportWAV(function (blob) {
            createIdentificationProfile(blob);
        });
        recorder.clear();
    }

    function onError(e) {
        console.error('Error', e);
    }

    function onSuccess(stream) {
        audio_context = audio_context || new window.AudioContext;
        var input = audio_context.createMediaStreamSource(stream);
        recorder = new Recorder(input);
        recorder.record();
    }

    function createIdentificationProfile(blob) {
        var form = new FormData();
        var id = $("#Id").val();
        form.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        form.append("id", id);
        form.append("Audio", blob);

        var request = new XMLHttpRequest();
        request.open("POST", "/AudioIdentification/EnrollProfile/" + id, true);

        request.onreadystatechange = function () {
            if (request.readyState == 4 && request.status == 200) {
                var response = null;
                try {
                    response = JSON.parse(request.responseText);
                } catch (e) {
                    response = request.responseText;
                }
            }
        }
        request.send(form);
    }

    function checkEnrollment() {
        var id = $("#Id").val();

        var url = "/AudioIdentification/CheckEnrollmentStatus/" + id;

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

    setTimeout(checkEnrollment, 1000);
});