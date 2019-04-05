<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Nico.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>About</h3>
    <p></p>

    <p>Set the gender and condition</p>


    <h4>Select Agent or Robot</h4>
    <select id="agent" name="selectAgent">
        <option value="true">Agent</option>
    </select>

    <h4>Select Gender</h4>
    <select id="gender" name="selectGender">
        <option value="female">Female</option>
        <option value="male">Male</option>
    </select>

    <h4>Select Condition</h4>
    <select id="condition" name="selectCondition">
        <option value="nonsocial">Nonsocial</option>
        <option value="social">Social</option>
    </select>

    <h4>Select Entrainment Type</h4>
    <select id="enttype" name="selectEntrainment">
        <option value="none">None</option>
    </select>

    <h4>Select Problem Set</h4>
    <select id="problemSet" name="selectProblemSet">
        <option value="A">Emma - A</option>
        <option value="B">Emma - B</option>
        <option value="C">Emma - C</option>
        <option value="D">Emma - D</option>
        <option value="E">Emma - E</option>
        <option value="F">Emma - F</option>
        <option value="G">Nico - G</option>
    </select>

    <h4>Text or Voice</h4>
    <select id="voiceText" name="selectVoiceText">
        <option value="voice">Voice</option>
    </select>

    
    <h4>Input IP Address</h4>
    <input type="text" name="txtRobotIP" placeholder="192.168.1.8" id="robotIP" runat="server"/>
    <br />
    <h5>NOTE: Port number is 9559. This should not need to change. If you need to change it, see administrator.</h5>
    <br />
    <br />
    <asp:Button id="submitbutton" Text="Submit" runat="server" OnClick="Submit" />
    
</asp:Content>