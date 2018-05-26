$(function () {
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
        var url = URL.createObjectURL(event.data.blob);
        $("#audioSource").attr("src", url);
        $("#audioControls").load();

        var reader = new FileReader();
        reader.readAsDataURL(event.data.blob);

        var a = reader.result.split(',')[1];
        console.log(reader.result);

        $("#Audio").val(a);
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
            sampleRate: 16000,//audioContext.sampleRate,
            numChannels: 1
        });

        processor.onaudioprocess = function (event) {
            worker.postMessage({ command: 'record', buffers: getBuffers(event) });
        };
    }

    function stopRecordingProcess(finish) {
        input.disconnect();
        processor.disconnect();
        worker.postMessage({ command: finish ? 'finish' : 'cancel' });
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

    $("#startRecording").click(function (e) {
        e.preventDefault();
        console.log("start recording");

        startRecording();
    });

    $("#stopRecording").click(function (e) {
        e.preventDefault();
        console.log("stop recording");
        stopRecordingProcess(true);
    });
});