<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="ProblemPageAgent.aspx.cs" Inherits="Nico.aspx.ProblemPageAgent" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="ProblemDescription" runat="server" CssClass ="h3"></asp:Label>

    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <div class="container" id="overalldiv">

       
        <div class="row">
            
            
            <div id="NicoStuff" class=".col-xs-3 center-block">
                <div class="row">
                    <div id="touchzone" class=".col-xs-3 center-block">
                        <br />
                       
                        <img id="mic" style="width:240px;position:fixed;height:279px;top:100px; left:75px;" src="../content/img/imagedirectory/mic.png"/>                        
                        <br />
                    </div>
                </div>

                <div class="row">
                    <div class=".col-xs-3 center-block" >
                        <img id="listening" src='../content/img/loading/circles.gif' style="display:none;width:150px; height:150px;position:fixed;top:400px;left:50px;" />
                        <br />
                        <br />
                        <br />

                    </div>
                
                    <div  class=".col-xs-3 center-block">
                        <img id="thinking" src='../content/img/loading/coloredcircles.gif' style="display:none;width:150px; height:150px;position:fixed;top:400px;left:200px;"/>
                        <br />
                        <br />
                        <br />
                    </div>
                </div>




            </div>

            <div id="ProblemUIStuff" class="col-sm-9">
                <div class="row">
                    <div id="problemText" class="col-sm-12 text-center">
                            <br />
                            <br />
                            <h4 id="probdescr">Help by explaining what to do.</h4> 
                            <br />
                    </div>
                </div>
                <div class="row">
                            <div id="priorsteptouch" class="col-sm-3 text-center" >
                                 <img ID="priorStepButton" style="visibility:hidden;width:75px;height:75px;cursor:pointer;" src="../content/img/imagedirectory/double-up-arrow.jpg"/>
                                 <h4 ID="priorStepText" style="visibility:hidden;">Prior Step</h4>
                            </div>
            
                            <div class="col-sm-6 text-center">
                                    <br />
                                    <table id="table3" class="table table-new"></table>
                            </div>
            
                            <div id="nextsteptouch" class="col-sm-3 text-center">
                                <img ID="nextStepButton" style="visibility:visible;width:75px;height:75px;cursor:pointer;" src="../content/img/imagedirectory/double-down-arrow.jpg" />
                                <h4 ID="nextStepText" style="visibility:visible;">Next Step</h4>
                            </div>
        
               </div>
               
                <div class="row">
                    <div id="NextProbDiv" class=".col-xs-12 center-block">
                        <h4><button type="button" ID="NextProblem" OnClick="Next_Problem()" style="color:hsl(0, 0%, 30%);visibility:hidden;cursor:pointer;">Next Problem</button></h4>
                    </div>
                </div>
            </div>
        </div>
        

        <br />

    </div>
    <link rel="stylesheet" runat="server" media="screen" href="../content/bootstrap.css" />

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js" type="text/javascript"></script>
    <script src="https://code.jquery.com/jquery-1.9.1.min.js"></script>
    <script src="../js/jquery.signalR-2.0.0.min.js"></script>
    <script src="../js/talk.js"></script>
    <script src="../js/audio.js"></script>  
    <script src="../js/download.js"></script>  
    <script src="../js/recorder.js"></script>


    <script type="text/javascript">
       // <input type="image" ID="nextStepButton" onclick="NextStep_Click()" style="visibility:visible;width:50px;height:50px;" src="../content/img/imagedirectory/double-down-arrow.jpg" />
       //<input type="image" ID="priorStepButton" onclick="PriorStep_Click()" style="visibility:hidden;width:50px;height:50px;" src="../content/img/imagedirectory/double-up-arrow.jpg" />
        //                <h4 ID="priorStepText" style="visibility:hidden;">Prior Step</h4>
        //         <h4 ID="nextStepText" style="visibility:visible;">Next Step</h4>
        //<button class="button button-new" type="button" ID="nextStepButton" onclick="NextStep_Click()" style="visibility:visible;" > Next Step </button>
        //          <button class="button button-new" type="button" ID="priorStepButton" onclick="PriorStep_Click()" style="visibility:hidden"> Prior Step </button>

        //<div class="progress progress-striped">
            //<div class="progress-bar progress-bar-success" style="width: 20%">20% Complete</div>
        //</div>
       // <img id="agentimg" style="width:160px;position:fixed;height:240px;top:100px; left:110px;" src="../content/img/imagedirectory/agent-img.png"/>
        //<div>
         //   <h2>Log</h2>
        //    <pre id="log"></pre>
        // </div>
        /*
        <div>
<h2>Log</h2>
<pre id="log"></pre>
</div>
*/
        // Variables for recording
        var recorder;
        var input;
        var audio_context;

        // Variables for ASR
        var final_transcript = '';
        var ignore_onend;
        var start_timestamp;
        var recognition = new webkitSpeechRecognition();
        var recognizing = false;
        var resultevent = false;
        var stoprecord = false;
        
        // Initialize recognition object values
        recognition.continuous = true;
        recognition.interimResults = false;
        recognition.lang = 6;

        // Variable for triggering speech if speaker is not responding
        var timer;

        function touchHandlerDown(evt) {
            $("#thinking").css("display", "none");
            writeLog("touch down");
            //evt.preventDefault();
            clearTimeout(timer);
            $("#listening").css("display", "block");
            recognizing = true;
            __log("click down - start recognizing");

            startRecording();

            //return false;
        }

        function touchHandlerUp(evt) {
            writeLog("touch went UP");
            //evt.preventDefault();
            $("#listening").css("display", "none");
            recognizing = false;
            __log("click up");
            //stopRecording(problemStepAnalyzer);
            stopRecording();
        }

        // check if microphone is connected and if it can be used as a media stream
        function startUserMedia(stream) {
            var input = audio_context.createMediaStreamSource(stream);
            __log('Media stream created.');
            recorder = new Recorder(input);
            __log('Recorder initialised.');
        }

        // event listener for start of recognition
        recognition.onstart = function () {
            __log('Start event fired');
            recognizing = true;
        };

        // event listener for error in recognition
        recognition.onerror = function (event) {
            if (event.error == 'no-speech') {
                __log('No speech was detectable');
                window.alert("Your speech wasn't detected. Please try again!");
                ignore_onend = true;
            }
            if (event.error == 'audio-capture') {
                __log('No microphone available');
                ignore_onend = true;
            }
            if (event.error == 'not-allowed') {
                if (event.timeStamp - start_timestamp < 100) {
                    __log('Time stamp issue');
                } else {
                    __log('Other Not Allowed Issue');
                }
                ignore_onend = true;
            }
            //timer = setTimeout(function () { noResponseCallNico("no response"); }, 25000);
        };

        // function to happen when recognition completes
        recognition.onresult = function (event) {
            for (var i = event.resultIndex; i < event.results.length; ++i) {
                if (event.results[i].isFinal) {
                    final_transcript += event.results[i][0].transcript;
                }
            }
            resultevent = true;
            __log("in on result");
            
            if (stoprecord) {
                responseCallNico(final_transcript);
            }
            
        };    

        // event listener for when recognition ends
        recognition.onend = function () {
            writeLog("recognition onend called");
            recognizing = false;
            if (ignore_onend) {
                return;
            }

            if (!final_transcript) {
                __log('No final transcript!!!');
                window.alert("Your speech wasn't detected. Please try again!");
                //timer = setTimeout(function () { noResponseCallNico("no response"); }, 25000);
                //createDownloadLink(sendBlob);
                return;
            }
        };

        // start recording the user's voice upon button push
        function startRecording() {
            recorder && recorder.record();
            __log('Calling START record');

            writeLog("Calling start recording");

            // reset ASR variables
            resultevent = false;
            stoprecord = false;
            final_transcript = '';
            recognition.start();
            ignore_onend = false;
            start_timestamp = event.timeStamp;
        }



        // function to stop recording users voice when they push the "STOP" button
        function stopRecording() {
            recorder && recorder.stop();
            __log('Calling STOP record.');
            stoprecord = true;

            recognition.stop();

            if (resultevent) {
                responseCallNico(final_transcript);
                stoprecord = false;
            }
            
            writeLog("called stop recording");
            createDownloadLink(sendBlob);
            recorder.clear();
        }
        
        // function to save audio file to server
        function sendBlob(blob) {
            if (object.sendToServer) {
                
                var url1 = (window.url || window.webkitURL);
                //__log(url1);
                var url = url1.createObjectURL(blob);

                var li = document.createElement('li');
                var au = document.createElement('audio');
                var hf = document.createElement('a');

                au.controls = true;
                au.src = url;
                hf.href = url;
                hf.download = new Date().toISOString() + '.wav';
                hf.innerHTML = hf.download;
                
                var data = new FormData();
                //data.append('transcript', final_transcript);
                //data.append('page_loc', 'ProblemPage');
                data.append('lib', blob);
                
                /*
                $(document).ajaxStart(function () {
                    $("#thinking").css("display", "block");

                });

                $(document).ajaxStop(function () {
                    $("#thinking").css("display", "none");
                });
                */

                writeLog("calling dialogue engine");

                __log("Calling SAVE");
                $("#thinking").css("display", "block");
                var audioname = new Date().toISOString() + '.wav';
                download(new Blob(  [blob]), audioname, "audio/wav");
                /*
                //$.ajax({
                    url: "../handlers/saveWav.ashx",
                    type: 'POST',
                    data: data,
                    contentType: false,
                    processData: false,
                    success: function () {
                        //$("#thinking").css("display", "none");
                        //updateTable();
                    },
                    error: function (err) {
                        alert(err.statusText)
                    }
                });*/

            }

        }

        // still a part of saving response to server
        function createDownloadLink(callback) {
            recorder && recorder.exportWAV(function (blob) {
                object = new Object();
                object.sendToServer = true;
                callback(blob);
            });
        }
        
        
        function responseCallNico(text) {
            clearTimeout(timer);
            var data = new FormData();
            data.append('transcript', text);
            data.append('page_loc', 'ProblemPage');

            $("#thinking").css("display", "block");
            $.ajax({
                url: "../handlers/DialogueEngine.ashx",
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: function () {
                    window.clearTimeout(timer);
                    timer = window.setTimeout(function () { responseCallNico("no response"); }, 65000);
                },
                complete: function () {
                    updateTable();
                    $("#thinking").css("display", "none");
                },
                error: function (err) {
                    alert(err.statusText)
                }
            });
        }


     



        // This function updates the table display
        function updateTable(endSession) {
            if (endSession == "end of session") {
                window.location.replace("../aspx/Default.aspx");
            }
            else {
                
                $.getJSON("../handlers/UpdateTable.ashx", function (data) {

                    // Extract variables from first row
                    var numCols = data[0]["Col1"];
                    var step = data[0]["Col2"];
                    var maxSteps = data[0]["Col3"];
                    var answered = data[0]["Col4"];
                    var cell = data[0]["Col5"];
                    var probtext = data[0]["Col6"];
                    var numRows = data[0]["Col7"];
                    var numStep = parseInt(step, 10);
                    var numCell = parseInt(cell, 10);

                    // Update problem description
                    document.getElementById("problemText").getElementsByTagName("h4")[0].innerHTML = String(probtext);
                                        
                    // Create table
                    var oldTable = document.getElementById('table3');
                    var newTable = oldTable.cloneNode();
                    var counter = 0;
                    var stepString = "Step ";
                    var stepCounter = 1;
                    var rowCounter = 0;
                    var thisRow = 0; 

                    // Create Header
                    var trheader = document.createElement('tr');
                    var thfirst = document.createElement('th');
                    thfirst.appendChild(document.createTextNode("Step"));
                    trheader.appendChild(thfirst);

                    for (var key in data[1]) {
                        if (parseInt(numCols, 10) == counter) {
                            counter = 0;
                            break;
                        }
                        else {
                            var thheader = document.createElement('th');
                            thheader.appendChild(document.createTextNode(data[1][key]));
                            trheader.appendChild(thheader);
                            counter++;
                        }
                        newTable.appendChild(trheader);
                    }
                    
                    // Start at i = 2 because in Json, first row is data, second row is header info so 3rd row (i = 2) actual data
                    for (var i = 2; i < data.length; i++) {
                        var tr = document.createElement('tr');

                        // Check if we're on this step; zoom step to make it obvious what step we're on
                        // *********** VERIFY IF THIS IS WHERE I CAN CHANGE IT IF IT IS maxsteps == numsteps
                        // meaning we're on the first step
                        if (numStep == 1 && maxSteps == numRows && i == 2) {
                            tr.style.transform = 'scale(1.2)';
                            tr.style.fontWeight = '700';
                            tr.style.background = 'rgb(140,245,241)';
                            tr.style.border = '1px solid';
                            tr.style.borderColor = 'rgb(140,245,241)';
                            thisRow = 1;
                            rowCounter++;
                        }
                        else if (numStep == 0 && i == 3) {            
                            tr.style.transform = 'scale(1.2)';
                            tr.style.fontWeight = '700';
                            tr.style.background = 'rgb(140,245,241)';
                            tr.style.border = '1px solid';
                            tr.style.borderColor = 'rgb(140,245,241)';
                            thisRow = 1;
                        }
                        else if (i == numStep + 2 && numStep != 0) {
                            tr.style.transform = 'scale(1.2)';
                            tr.style.fontWeight = '700';
                            tr.style.background = 'rgb(140,245,241)';
                            tr.style.border = '1px solid';
                            tr.style.borderColor = 'rgb(140,245,241)';
                            thisRow = 1;
                        }
                        else {
                            // do nothing
                            thisRow = 0;
                        }

                        // Add step number cell
                        var tdstep = document.createElement('td');
                        var stepText = stepString.concat(rowCounter.toString());
                        tdstep.appendChild(document.createTextNode(stepText));
                        // Check if this row is the step that we're currently on, and modify the format of the cell
                        if (thisRow == 1) {
                            tdstep.style.border = '1px solid';
                            tdstep.style.background = 'rgb(140,245,241)';
                            tdstep.style.borderColor = 'rgb(140,245,241)';
                        }
                        tr.appendChild(tdstep);

                        // Add other cell info
                        for (var key in data[i]) {
                            if (counter == parseInt(numCols, 10)) {
                                counter = 0;
                                break;
                            }
                            else {
                                var td = document.createElement('td');
                                td.appendChild(document.createTextNode(data[i][key]));

                                // Check if this cell contains a 'question mark'; if yes then change formatting of text
                                if (String(data[i][key]).includes("?")) {
                                    if (thisRow != 1) {
                                        td.style.color = 'rgb(0,255,255)';
                                        td.style.fontWeight = 'bold';
                                        td.style.fontSize = 'large';
                                    }
                                    else {
                                        td.style.color = '#FFFFFF';
                                    }
                                }
                                

                                // If this row is the step that we're are currently 'on', then modify formatting of the cell, see if this cell contains the 'answer'
                                // and then modify the 'answer'
                                if (thisRow == 1) {
                                    td.style.border = '1px solid';
                                    td.style.borderColor = 'rgb(140,245,241)';
                                    td.style.background = 'rgb(140,245,241)';
                                    if (numCell-1 == counter && parseInt(answered,10) == 1) {
                                        td.style.background = 'rgb(204,255,255)';
                                        td.style.transition = 'background 2s';
                                    }
                                }


                                tr.appendChild(td);
                                counter++;
                            }
                        }
                        rowCounter++; 
                        newTable.appendChild(tr);
                    }
                    oldTable.parentNode.replaceChild(newTable, oldTable);

                    // Update buttons
                    if (parseInt(step, 10) == maxSteps) {
                        document.getElementById("priorStepButton").style.visibility = "visible";  // visible
                        document.getElementById("priorStepText").style.visibility = "visible";
                        document.getElementById("nextStepButton").style.visibility = "hidden";   // hidden
                        document.getElementById("nextStepText").style.visibility = "hidden";
                        document.getElementById("NextProblem").style.visibility = "visible";      // visible

                    }
                    else if (parseInt(step, 10) > 1) {
                        document.getElementById("priorStepButton").style.visibility = "visible";  // visible
                        document.getElementById("priorStepText").style.visibility = "visible";
                        document.getElementById("nextStepButton").style.visibility = "visible";   // visible
                        document.getElementById("nextStepText").style.visibility = "visible";
                        document.getElementById("NextProblem").style.visibility = "hidden";      // hidden
                    }
                    else {
                        document.getElementById("priorStepButton").style.visibility = "hidden";  // hidden
                        document.getElementById("priorStepText").style.visibility = "hidden";
                        document.getElementById("nextStepButton").style.visibility = "visible";   // visible
                        document.getElementById("nextStepText").style.visibility = "visible";
                        document.getElementById("NextProblem").style.visibility = "hidden";      // hidden
                    }

                    if (parseInt(step, 10) == 0) {
                        responseCallNico("problem start");
                    }
                   
                })
                
            }            
        }

        function PriorStep_Click() {
            clearTimeout(timer);
            __log("prior step button clicked");
            var data = new FormData();
            var clicked = "prior";
            data.append('button_info', clicked);

            $(document).ajaxStart(function () {
                $("#thinking").css("display", "block");
            });

            $(document).ajaxStop(function () {
                $("#thinking").css("display", "none");
            });

            $.ajax({
                url: "../handlers/UpdateStep.ashx",
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: function (endSession) {
                    updateTable(endSession);
                    window.clearTimeout(timer)
                    timer = window.setTimeout(function () { responseCallNico("no response"); }, 65000);
                },
                error: function (err) {
                    alert(err.statusText)
                }
            });
            return false;
        }

        function NextStep_Click() {
            clearTimeout(timer);
            __log("next step button clicked");
            var data = new FormData();
            var clicked = "next";
            data.append('button_info', clicked);

            $(document).ajaxStart(function () {
                $("#thinking").css("display", "block");
            });

            $(document).ajaxStop(function () {
                $("#thinking").css("display", "none");
            });

            $.ajax({
                url: "../handlers/UpdateStep.ashx",
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: function (endSession) {
                    updateTable(endSession);
                    window.clearTimeout(timer);
                    timer = window.setTimeout(function () { responseCallNico("no response"); }, 65000);
                },
                error: function (err) {
                    alert(err.statusText)
                }
            });
            return false;
        }

        function PriorStep_Touch(evt) {
            clearTimeout(timer);
            evt.preventDefault();
            __log("prior step button clicked");
            var data = new FormData();
            var clicked = "prior";
            data.append('button_info', clicked);

            $(document).ajaxStart(function () {
                $("#thinking").css("display", "block");
            });

            $(document).ajaxStop(function () {
                $("#thinking").css("display", "none");
            });

            $.ajax({
                url: "../handlers/UpdateStep.ashx",
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: function (endSession) {
                    updateTable(endSession);
                    window.clearTimeout(timer);
                    timer = window.setTimeout(function () { responseCallNico("no response"); }, 65000);
                },
                error: function (err) {
                    alert(err.statusText)
                }
            });
            return false;
        }

        function NextStep_Touch(evt) {
            clearTimeout(timer);
            evt.preventDefault();
            __log("next step button clicked");
            var data = new FormData();
            var clicked = "next";
            data.append('button_info', clicked);

            $(document).ajaxStart(function () {
                $("#thinking").css("display", "block");
            });

            $(document).ajaxStop(function () {
                $("#thinking").css("display", "none");
            });

            $.ajax({
                url: "../handlers/UpdateStep.ashx",
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: function (endSession) {
                    updateTable(endSession);
                    window.clearTimeout(timer);
                    timer = window.setTimeout(function () { responseCallNico("no response"); }, 65000);
                },
                error: function (err) {
                    alert(err.statusText)
                }
            });
            return false;
        }

        function Next_Problem() {
            clearTimeout(timer);
            __log("new problem button clicked");
            var data = new FormData();
            var clicked = "problem";
            data.append('button_info', clicked);

            $(document).ajaxStart(function () {
                $("#thinking").css("display", "block");
            });

            $(document).ajaxStop(function () {
                $("#thinking").css("display", "none");
            });

            $.ajax({
                url: "../handlers/UpdateStep.ashx",
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: function (endSession) {
                    updateTable(endSession);
                    window.clearTimeout(timer);
                    timer = window.setTimeout(function () { responseCallNico("no response"); }, 65000);
                },
                error: function (err) {
                    alert(err.statusText)
                }
            });

            return false;
            
        }


        // Window loading initializations
        window.onload = function init() {
            var touchzone = document.getElementById('touchzone');
            //touchzone.addEventListener("touchstart", touchHandlerDown, false);
            //touchzone.addEventListener("touchend", touchHandlerUp, false);
            //preventLongPressMenu(document.getElementById('NAOButton'));

            touchzone.addEventListener("mousedown", touchHandlerDown, false);
            touchzone.addEventListener("mouseup", touchHandlerUp, false);

            var nextsteptouch = document.getElementById('nextsteptouch');
            var priorsteptouch = document.getElementById('priorsteptouch');
            //nextsteptouch.addEventListener("touchend", NextStep_Touch, false);
            //priorsteptouch.addEventListener("touchend", PriorStep_Touch, false);

            nextsteptouch.addEventListener("click", NextStep_Touch, false);
            priorsteptouch.addEventListener("click", PriorStep_Touch, false);


            updateTable();

            try {
                window.AudioContext = window.AudioContext || window.webkitAudioContext;
                navigator.getUserMedia = (navigator.getUserMedia || navigator.webkitGetUserMedia);
                window.URL = (window.URL || window.webkitURL);

                audio_context = new AudioContext;
                __log('Audio context set up.');
                __log('navigator.getUserMedia ' + (navigator.getUserMedia ? 'available.' : 'not present!'));

                if (!('webkitSpeechRecognition' in window)) {
                    upgrade();
                }
            }
            catch (e) { alert('No web audio support in this browser!'); }

            if (navigator.getUserMedia) {
                navigator.getUserMedia({audio: true},

                    // successCallback
                    function (stream) {
                        input = audio_context.createMediaStreamSource(stream);
                        __log('Media stream created.');
                        recorder = new Recorder(input);
                        __log('Recorder initialised.');
                        recording = false;
                    },

                    // errorCallback
                    function (err) {console.log("The following error occured: " + err); }
                );
            } else {
                __log('getUserMedia not supported');
            }
            
            timer = window.setTimeout(function () { responseCallNico("no response"); }, 65000);


        }

        // function for writing data to window
        function __log(e, data) {
           // log.innerHTML += "\n" + e + " " + (data || '');
        }

        // response if need to upgrade window
        function upgrade() {
            __log('info_upgrade');
        }

        function writeLog(message) {
            var data = new FormData();
            data.append('log', message);

            $.ajax({
                url: "../handlers/ErrorLogger.ashx",
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: function () {
                },
                error: function (err) {
                    alert(err.statusText)
                }
            });
        }

    </script>

</asp:Content>