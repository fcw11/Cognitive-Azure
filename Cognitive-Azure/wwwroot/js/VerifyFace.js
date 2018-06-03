$(function () {
    $("input[type='submit']").click(function (e) {
        e.preventDefault();
        verifyFace();
    });

    function verifyFace() {
        var form = new FormData();
        form.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        var id = $("#SelectedProfile:checked").val();
        if (id != undefined) {
            form.append("Id", id);
        }
        var image = document.getElementById('Image').files[0];
        form.append("Image", image);

        var request = new XMLHttpRequest();
        request.open("POST", "/FaceVerification/VerifyFace", true);

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

        $("tr").removeClass('notverified');
        $("tr").removeClass('verified');
        $("tr td:nth-child(4)").html('');

        if (data.isIdentical == true) {
            var id = $("#SelectedProfile:checked").val();
            $("tr[id='" + id + "']").addClass('verified');
            $("tr[id='" + id + "'] td:last").html(data.confidence);
        }
        if (data.isIdentical == false) {
            var id = $("#SelectedProfile:checked").val();
            $("tr[id='" + id + "']").addClass('notverified');
            $("tr[id='" + id + "'] td:last").html(data.confidence);
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
});