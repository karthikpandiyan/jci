<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JCI.Branding.POCWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/MicrosoftAjax.js"></script>    
    <script type="text/javascript" src="../Scripts/app.js"></script>
    <script type="text/javascript">
        //function callback to render chrome after SP.UI.Controls.js loads
        function renderSPChrome() {
            //Set the chrome options
            var options = {
                'appTitle': "Branding - POC",
                'onCssLoaded': 'chromeLoaded()'
            };

            //Load the Chrome Control in the chrome_ctrl_placeholder element of the page
            var chromeNavigation = new SP.UI.Controls.Navigation('chrome_ctrl_placeholder', options);
            chromeNavigation.setVisible(true);
        }

        function chromeLoaded() {
            $('body').show();
        }


    </script>
<script type="text/javascript">


    $(document).ready(function () {
        var scriptbase = _spPageContextInfo.webServerRelativeUrl + "_layouts/15/";
        $.getScript(scriptbase + "SP.Runtime.js", function () {
            $.getScript(scriptbase + "SP.js", function () {
                $.getScript(scriptbase + "SP.Taxonomy.js", execOperation);
            });
        });
        // wait for the sharepoint javascript libraries to load, then call the function 'Initialize'
        //  ExecuteOrDelayUntilScriptLoaded(ChangeMasterPage, "sp.js");
    });
    function ChangeMasterPage() {
        alert("Applying");
        var context;
        var web;
        var strMasterPageUrl = '/_catalogs/masterpage/JCIBrandingPOC.master';

        context = new SP.ClientContext.get_current();
        web = context.get_web();

        web.set_customMasterUrl(strMasterPageUrl);
        web.set_masterUrl(strMasterPageUrl);
        web.update();

        context.executeQueryAsync(function () {

            alert("Master Page has been set to \n" + strMasterPageUrl);

        }, function (sender, args) {

            alert("Error: " + args.get_message());

        });
    }

    // ChangeMasterPage();
</script> ​​​​ ​​​​
</head>
<body style="display:none">
    <form id="form1" runat="server">
        <div id="chrome_ctrl_placeholder"></div>        
        <div style="padding-left: 20px; padding-right: 20px;">
            <h2>Instructions</h2>
            <br />
            <h3>Deploy all the artifacts</h3>
            <p>
                Click the Deploy button to create folders, upload Master Pages, CSS, image and JavaScript files.<br />
            </p>
            <asp:Button runat="server" ID="btnIniSiteContent" Text="Deploy" OnClick="btnIniSiteContent_Click" />
            <br />
            <br />
            <h3>Apply Master Page</h3>
            <p>
                To apply master page deployed, click the Apply MasterPage button below to set MasterPage by the app.
            </p>
            <asp:Button runat="server" ID="btnApply" Text="Apply MasterPage" OnClick="btnApply_Click" />
            <input id="btnApplyscript" type="button" value="Apply MasterPage"   onclick="ChangeMasterPage();"/>
            <br />
            <br />
            <h3>Reset Master Page</h3>
            <p>
                To reset master page deployed, click the Reset MasterPage button below to set RESET by the app.
            </p>
            <asp:Button runat="server" ID="btnReset" Text="Reset MasterPage" OnClick="btnReset_Click" />
            <br />
            <br />
            <asp:Label ID="lblInfo" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
