 
var interval = setInterval(function(){
if($('#O365_MainLink_Logo').length)
{   
 $(document).ready(function() {
     
                                var linkString   = "<a title='First Link' \
                                class='' </a>"
                               
           
       var lnk="<div><a class='o365cs-nav-item o365cs-nav-link o365cs-topnavText ms-bgc-td-h o365button'\
           role='menuitem' tabindex='0' href='https://outlook.office365.com/owa/?realm=JCIstage.onmicrosoft.com&amp;exsvurl=1&amp;ll-cc=1033&amp;modurl=0' aria-disabled='false' id='O365_MainLink_ShellMail' aria-label='Go to Outlook Web App for email' aria-selected='false' aria-haspopup='false' style='display: none;'><span>Outlook</span><span style='display: none;'><span role='presentation' class='wf wf-o365-x14 wf-family-o365 header-downcarat'>?</span></span><div class='o365cs-activeLinkIndicator ms-bcl-w' style='display: none;'></div></a></div>";
      

  var lnk2="<a>sample </a>         "
var img="<div class='o365cs-nav-topItem'><div><a class='o365cs-nav-item o365cs-nav-link o365cs-topnavText ms-bgc-td-h o365button' role='menuitem' tabindex='0' href='https://outlook.office365.com/owa/?realm=JCIstage.onmicrosoft.com&amp;exsvurl=1&amp;ll-cc=1033&amp;modurl=0' aria-disabled='false' id='O365_MainLink_ShellMail' aria-label='Go to Outlook Web App for email' aria-selected='false' aria-haspopup='false' style='display: none;'><span>Outlook</span><span style='display: none;'><span role='presentation' class='wf wf-o365-x14 wf-family-o365 header-downcarat'>?</span></span><div class='o365cs-activeLinkIndicator ms-bcl-w' style='display: none;'></div></a></div><img src='https://jcistage.sharepoint.com/_catalogs/masterpage/JCIBrandingPOC/images/logo.png'></div>";

 $("#O365_MainLink_Help").parent().parent().append(img);
  $(".o365cs-nav-notificationTrayContainer o365cs-topnavLinkBackground").remove();
  $(".o365cs-nav-O365LinksContainer o365cs-topnavLinkBackground").remove();
  
$("#O365_TopMenu").removeClass("o365cs-nav-rightAlign");
$(".o365cs-nav-notificationTrayContainer o365cs-topnavLinkBackground").removeClass("o365cs-nav-notificationTrayContainer o365cs-topnavLinkBackground");

  $(".o365cs-nav-notificationTrayContainer o365cs-topnavLinkBackground").remove();
  $('.o365cs-nav-notificationTrayContainer o365cs-topnavLinkBackground').removeAttr('class');
	 
 $(".o365cs-w100-h100").parent().removeClass("o365cs-nav-notificationTrayContainer o365cs-topnavLinkBackground");
 
 $(".o365cs-me-tileview-container").hide();
 
$("<div class='OuterMainContainer BorderBottom SteelGray'></div>" ).insertAfter($("#suiteBarTop"));

 var colorOrig=$("#O365_MainLink_ShellMail").css('background-color');
   
 $("#O365_MainLink_ShellMail").hover(
    function() {
        //mouse over
        $(this).css('background', '#eee');
          $(this).css('text-decoration', 'underline')
    }, function() {
        //mouse out
        $(this).css('background', colorOrig);
        $(this).css('text-decoration', 'none');
    });
    
    
    $("#O365_MainLink_ShellCalendar").hover(
    function() {
        //mouse over
        $(this).css('background', '#eee');
          $(this).css('text-decoration', 'underline')
    }, function() {
        //mouse out
        $(this).css('background', colorOrig);
        $(this).css('text-decoration', 'none');
    });

    
     $("#O365_MainLink_ShellNewsfeed").hover(
    function() {
        //mouse over
        $(this).css('background', '#eee');
          $(this).css('text-decoration', 'underline')
    }, function() {
        //mouse out
        $(this).css('background', colorOrig);
        $(this).css('text-decoration', 'none');
    });

$("#O365_MainLink_ShellPeople").hover(
    function() {
        //mouse over
        $(this).css('background', '#eee');
          $(this).css('text-decoration', 'underline')

    }, function() {
        //mouse out
        $(this).css('background', colorOrig);
        $(this).css('text-decoration', 'none');
    });


$("#O365_MainLink_ShellDocuments").hover(
    function() {
        //mouse over
        $(this).css('background', '#eee');
          $(this).css('text-decoration', 'underline')
    }, function() {
        //mouse out
        $(this).css('background', colorOrig);
        $(this).css('text-decoration', 'none');
    });

$("#O365_MainLink_ShellTasks").hover(
    function() {
        //mouse over
        $(this).css('background', '#eee');
          $(this).css('text-decoration', 'underline')
    }, function() {
        //mouse out
        $(this).css('background', colorOrig);
        $(this).css('text-decoration', 'none');
    });

$("#O365_MainLink_ShellAdmin").hover(
    function() {
        //mouse over
        $(this).css('background', '#eee');
          $(this).css('text-decoration', 'underline')
    }, function() {
        //mouse out
        $(this).css('background', colorOrig);
        $(this).css('text-decoration', 'none');
    });
 });
clearInterval(interval);                
}
}, 1000);

     
       
  