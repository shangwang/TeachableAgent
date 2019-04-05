<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Agent.aspx.cs" Inherits="Nico.aspx.Agent_Billy" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<head>
        <title>Robot Study</title>
       	<link rel="stylesheet" type="text/css" href="../content/robot_ui.css"/>
</head>

<body id="body" class ="neutral">
    <div id="eyes"></div>
	<div id="mouth"></div>
    <div id="overlay"></div>

   <div><button class="talkbutton" onclick= "buttonVisibility();" type="button" id="butt" >Click to hear me!</button></div>
              
  
</body>

    <script src='https://code.responsivevoice.org/responsivevoice.js'></script>
    <script src="../js/jquery.signalR-2.0.0.min.js"></script>
    <script src="../signalr/hubs"></script>
    <script type="text/javascript">

        function talk(message) {
            var msg = new SpeechSynthesisUtterance(message)
            var voices = window.speechSynthesis.getVoices()
            msg.voice = voices[1]
            window.speechSynthesis.speak(msg)

            msg.onend = function(event) {
                $('body').removeClass("talking");
                $('body').addClass("neutral");
         }


         }

       


        function playAudioFile(audiopath) {
            var audio = new Audio(audiopath)

            audio.addEventListener("ended", function () {
                $('body').removeClass("talking");
                $('body').addClass("neutral");
            });

            audio.play()
        }


        function buttonVisibility() {
            var element = document.getElementById("butt");
            element.style.visibility = "hidden";
        }

        $(function () {
            var update1 = $.connection.myHub;
            var trigger = "signalR";
            update1.client.playSpeech = function (textString) {
                //log.innerHTML = "Got trigger to play speech";
                $('body').addClass("talking");
                $("#wait").css("display", "none");

                talk(textString)
               // playAudioFile(audiopath);

                //here is what plays the sound
            }
            $.connection.hub.start();
        })

        window.onload = function init() {
            //log.innerHTML = "window loaded";
        }
    </script>


</asp:Content>
