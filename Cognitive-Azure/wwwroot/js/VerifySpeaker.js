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
            verifySpeaker(blob);
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

    function verifySpeaker(blob) {
        var form = new FormData();
        form.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        var id = $("#SelectedProfile:checked").val();
        if (id != undefined) {
            form.append("Id", id);
        }
        form.append("Audio", blob);

        var request = new XMLHttpRequest();
        request.open("POST", "/AudioVerification/VerifySpeaker", true);

        request.onreadystatechange = function () {
            if (request.readyState == 4 && request.status == 200) {
                var response = null;
                try {
                    response = JSON.parse(request.responseText);
                } catch (e) {
                    response = request.responseText;
                }

                updateResult(response);
            }
        }
        request.send(form);
    };
    
    function updateResult(data) {
        var prettifiedResponse = syntaxHighlight(data);

        prettifiedResponse = prettifiedResponse.replace(/{/g, "{<br />").replace(/}/g, "<br />}").replace(/,/g, ",<br />");

        $("#status").html(prettifiedResponse);
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
});