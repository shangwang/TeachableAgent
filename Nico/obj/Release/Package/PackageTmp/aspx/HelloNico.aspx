<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HelloNico.aspx.cs" Inherits="Nico.aspx.HelloNico" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <div class="container" id="overalldiv"> 
        <div class="row">                
           <div id="touchzone" class=".col-xs-12 center-block text-center">
                <br />
                <h3>Say Hello to Emma</h3>    
                <img id="NAOButton" style="width:247px; height:262px;position:relative;" src="../content/img/imagedirectory/nao-sit.png"/>
                <br />
            </div>
        </div>

        <div class="row">
            <div class=".col-xs-12 center-block" >
                <img id="listening" src='../content/img/loading/circles_listening.gif' style="display:none;width:150px; height:150px;position:relative;" />
                <br />
            </div>
        </div>

        <div class="row">
            <div  class=".col-xs-12 center-block">
                <img id="thinking" src='../content/img/loading/coloredcircles_thinking.gif' style="display:none;width:150px; height:150px;position:relative;"/>
                <br />
            </div>
        </div>
        
        <div class="row">
            <div class=".col-xs-12 center-block">
                <h4><asp:LinkButton ID="StartTeaching" Text="Start Teaching"  OnClick="Start_Teaching" runat="server" style="color:hsl(0, 0%, 30%);visibility:hidden;"/></h4>
          </div>
        </div>
   </div>

   <link rel="stylesheet" runat="server" media="screen" href="../content/bootstrap.css" />

   <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js" type="text/javascript"></script>
   <script src="https://code.jquery.com/jquery-1.9.1.min.js"></script>
   <script src="../js/jquery.signalR-2.0.0.min.js"></script>
   <script src="../js/talk.js"></script>
   <script src="../js/audio.js"></script>  
   <script src="../js/recorder.js"></script>


   <script type="text/javascript">
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
       var firstTime = true;

       // Initialize recognition object values
       recognition.continuous = false;
       recognition.interimResults = false;
       recognition.lang = 6;

       // Variable for triggering speech if speaker is not responding
       var timer;

       // space bar down to start recognizing
       document.body.onkeydown = function (e) {

           clearTimeout(timer);
           if (!(recognizing)) {
               if (e.keyCode == 32) {
                   $("#listening").css("display", "block");
                   recognizing = true;
                   e.preventDefault();
                   __log("down - start recognizing");
                   startRecording();
               }

           }
           return false;
       }

       function mouseDown() {
           clearTimeout(timer);
           if (!(recognizing)) {
               $("#listening").css("display", "block");
               recognizing = true;
               __log("down - start recognizing");
               startRecording();
           }
           return false;

       }

       function touchHandlerDown(evt) {
           evt.preventDefault();
           clearTimeout(timer);
           if (!(recognizing)) {
               $("#listening").css("display", "block");
               recognizing = true;
               __log("down - start recognizing");
               startRecording();
           }
           return false;

       }

       // space bar up to stop recognizing
       document.body.onkeyup = function (e) {
           $("#listening").css("display", "none");
           if (e.keyCode == 32) {
               recognizing = false;
               e.preventDefault();
               __log("on up");
               //stopRecording(problemStepAnalyzer);
               stopRecording();
           }
           return false;
       }


       function mouseUp() {
           $("#listening").css("display", "none");
           recognizing = false;
           __log("on up");
           //stopRecording(problemStepAnalyzer);
           stopRecording();

       }

       function touchHandlerUp(evt) {
           evt.preventDefault();
           $("#listening").css("display", "none");
           recognizing = false;
           __log("on up");
           //stopRecording(problemStepAnalyzer);
           stopRecording();
           
       }

       function absorbEvent_(event) {
           var e = event || window.event;
           e.preventDefault && e.preventDefault();
           e.stopPropagation && e.stopPropagation();
           e.cancelBubble = true;
           e.returnValue = false;
           return false;
       }

       function preventLongPressMenu(node) {
           node.ontouchstart = absorbEvent_;
           node.ontouchmove = absorbEvent_;
           node.ontouchend = absorbEvent_;
           node.ontouchcancel = absorbEvent_;
       }


       // check if microphone is connected and if it can be used as a media stream
       function startUserMedia(stream) {
           var input = audio_context.createMediaStreamSource(stream);
           __log('Media stream created.');
           recorder = new Recorder(input);
           __log('Recorder initialised.');
       }

       // start recording the user's voice upon button push
       function startRecording() {
           recorder && recorder.record();
           __log('Calling START record');

           // reset ASR variables
           final_transcript = '';
           recognition.start();
           ignore_onend = false;
           start_timestamp = event.timeStamp;

           // begin ASR
           recognition.onstart = function () {
               recognizing = true;
               __log('Starting ASR');

           };

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
               timer = setTimeout(function () { noResponseCallNico("no response"); }, 25000);
           }

           ignore_onend = false;
           start_timestamp = event.timeStamp
       }

       // function to stop recording users voice when they push the "STOP" button
       function stopRecording() {
           recorder && recorder.stop();
           __log('Calling STOP record.');


           // function to happen when recognition ends
           recognition.onend = function () {
               recognizing = false;
               if (ignore_onend) {
                   return;
               }
               if (!final_transcript) {
                   __log('No final transcript!!!');
                   window.alert("Your speech wasn't detected. Please try again!");
                   timer = setTimeout(function () { noResponseCallNico("no response"); }, 25000);
                   return;
               }
           };

           // function to happen when recognition completes
           recognition.onresult = function (event) {

               for (var i = event.resultIndex; i < event.results.length; ++i) {
                   if (event.results[i].isFinal) {
                       final_transcript += event.results[i][0].transcript;
                   }
               }

               __log("Final Transcript posted");
               if (final_transcript) {
                   //results.innerHTML = results.innerHTML + "<br />" + final_transcript;
                   __log(final_transcript);
                   // create WAV download link using audio captured as a blob
                   createDownloadLink(sendBlob);
               }
               else {
                   window.alert("Your speech wasn't detected. Please try again!");
                   timer = setTimeout(function () { noResponseCallNico("no response"); }, 25000);
               }
           };

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
               data.append('transcript', final_transcript);
               data.append('page_loc', 'HelloNico');
               data.append('lib', blob);


               $(document).ajaxStart(function () {
                   $("#thinking").css("display", "block");
               });

               $(document).ajaxStop(function () {
                   $("#thinking").css("display", "none");
               });

               __log("Calling SAVE");
               $.ajax({
                   url: "../handlers/DialogueEngine.ashx",
                   type: 'POST',
                   data: data,
                   contentType: false,
                   processData: false,
                   success: function () {
                       __log("recognized speech");
                       document.getElementById('<%=StartTeaching.ClientID%>').style.visibility = "visible";
                   },
                   error: function (err) {
                       alert(err.statusText)
                   }
               });

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

       // This function is triggered whenever there has been a 20 second lapse with no response from the user
       function noResponseCallNico(text) {
           var data = new FormData();
           data.append('transcript', text);
           data.append('page_loc', 'HelloNico');

           $(document).ajaxStart(function () {
               $("#thinking").css("display", "block");
           });

           $(document).ajaxStop(function () {
               $("#thinking").css("display", "none");
           });


           $.ajax({
               url: "../handlers/DialogueEngine.ashx",
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


       // Window loading initializations
       window.onload = function init() {
           var touchzone = document.getElementById('touchzone');
           touchzone.addEventListener("touchstart", touchHandlerDown, false);
           touchzone.addEventListener("touchend", touchHandlerUp, false);
           //preventLongPressMenu(document.getElementById('NAOButton'));



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
               navigator.getUserMedia({ audio: true },

                   // successCallback
                   function (stream) {
                       input = audio_context.createMediaStreamSource(stream);
                       __log('Media stream created.');
                       recorder = new Recorder(input);
                       __log('Recorder initialised.');
                       recording = false;
                   },

                   // errorCallback
                   function (err) { console.log("The following error occured: " + err); }
               );
           } else {
               __log('getUserMedia not supported');
           }

           noResponseCallNico("HELLO FIRST TIME");

           timer = setTimeout(function () { noResponseCallNico("no response"); }, 45000);

       }

       // function for writing data to window
       function __log(e, data) {
           //log.innerHTML += "\n" + e + " " + (data || '');
       }

       // response if need to upgrade window
       function upgrade() {
           __log('info_upgrade');
       }

   </script>
</asp:Content>
