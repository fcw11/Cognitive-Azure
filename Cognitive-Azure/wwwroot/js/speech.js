$(function() {
    var recorder;

    $("#startRecording").click(function() {
        console.log("start recording");

        // request permission to access audio stream
        navigator.mediaDevices.getUserMedia({ audio: true }).then(stream => {
            recorder = new MediaRecorder(stream);

            recorder.start();

            recorder.ondataavailable = e => {
                var chunks = [];
                chunks.push(e.data);
                if (recorder.state == 'inactive') {
                    var blob = new Blob(chunks, { type: 'audio/webm' });

                    var url = URL.createObjectURL(blob);

                    $("#audioSource").attr("src", url);
                    $("#audioControls").load();
                };
            }
        }).catch(console.error);
    });

    $("#stopRecording").click(function () {
        console.log("stop recording");
        recorder.stop();
    });
});