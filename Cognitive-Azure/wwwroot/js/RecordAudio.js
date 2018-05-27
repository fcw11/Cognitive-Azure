navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia;

window.URL = window.URL || window.webkitURL;

var audioContext = new AudioContext;

if (audioContext.createScriptProcessor == null)
    audioContext.createScriptProcessor = audioContext.createJavaScriptNode;

var microphone = undefined,
    microphoneLevel = audioContext.createGain(),
    mixer = audioContext.createGain(),
    input = audioContext.createGain(),
    processor = undefined;

microphoneLevel.gain.value = 0.5;
microphoneLevel.connect(mixer);
mixer.connect(input);

var worker = new Worker('/js/EncoderWorker.js');

worker.onmessage = function (event) {
    var form = new FormData();
    debugger;
    var id = $("#Id").val();
    form.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
    form.append("id", id);
    form.append("Audio", event.data.blob);

    var request = new XMLHttpRequest();
    request.open("POST", "/Audio/EnrollProfile/" + id, true);
        
    request.onreadystatechange = function () {
        if (request.readyState == 4 && request.status == 200) {
            var response = null;
            try {
                response = JSON.parse(request.responseText);
            } catch (e) {
                response = request.responseText;
            }

            console.log(response);
        }
    }
    debugger;
    request.send(form);
};

function getBuffers(event) {
    var buffers = [];
    for (var ch = 0; ch < 2; ++ch)
        buffers[ch] = event.inputBuffer.getChannelData(ch);
    return buffers;
}

function startRecordingProcess() {
    processor = audioContext.createScriptProcessor(2048, 2, 1);
    input.connect(processor);
    processor.connect(audioContext.destination);

    worker.postMessage({
        command: 'start',
        process: 'separate',
        sampleRate: audioContext.sampleRate,
        numChannels: 1
    });

    processor.onaudioprocess = function (event) {
        worker.postMessage({ command: 'record', buffers: getBuffers(event) });
    };
}

function stopRecordingProcess(finish) {
    input.disconnect();
    if (processor != undefined) {
        processor.disconnect();
        worker.postMessage({ command: finish ? 'finish' : 'cancel' });
    }
}

function startRecording() {
    if (microphone == null)
        navigator.getUserMedia({ audio: true },
            function (stream) {
                microphone = audioContext.createMediaStreamSource(stream);
                microphone.connect(microphoneLevel);
            },
            function (error) {
                window.alert("Could not get audio input.");
            });

    startRecordingProcess();
}