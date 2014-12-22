using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using System.Web.Hosting;
using Microsoft.SharePoint.Client.WebParts;
using System.Text;
using System.Xml.Linq;


namespace JCI.Branding.POCWeb
{
    public partial class Default : System.Web.UI.Page
    {

        #region properties
        private static string ContentTypeID = "0x010048017A06020440BE8498BB193B944C84";
        #endregion
            
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
        }

        protected void btnIniSiteContent_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);               
                clientContext.ExecuteQuery();
                string serverRelativeURL = web.ServerRelativeUrl;

                Web parentWeb = clientContext.Site.RootWeb;
                clientContext.Load(parentWeb, w => w.Url);
                clientContext.ExecuteQuery();
                string parentWebServerRelativeURL = parentWeb.Url;

                serverRelativeURL = parentWebServerRelativeURL + serverRelativeURL;


                //Upload master pages

                List masterPageList = web.Lists.GetByTitle("Master Page Gallery");               
                string masterpageContentTypeId = GetContentType(clientContext, masterPageList, "Master Page");

                UploadMasterPages(clientContext, web, masterPageList, masterpageContentTypeId, serverRelativeURL);

                //Upload CSS, Image and JS files

                UploadFiles(clientContext, web, masterPageList, serverRelativeURL);

                lblInfo.Text = "The deployment operations have successfully completed. Go to the <a href='" + parentWebServerRelativeURL + "/_catalog/MasterPage'>Master Page Gallery </a> to view the master pages.";
            }
        }


        private void UploadMasterPages(ClientContext clientContext, Web web, List masterPageList, string masterpageContentTypeId, string serverRelatedURL)
        {

            var branding = XDocument.Load(HostingEnvironment.MapPath(string.Format("~/{0}", "settings.xml"))).Element("branding");

            foreach (var masterpage in branding.Element("masterpages").Descendants("masterpage"))
            {
                var masterPageURL = masterpage.Attribute("name").Value;
                string fileURL = string.Format("JCIBrandingPOC/Masterpages/{0}", masterPageURL);
                string remoteFileURL = masterPageURL;
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(string.Format("~/{0}", fileURL)));
                newFile.Url = remoteFileURL;
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File uploadFile = masterPageList.RootFolder.Files.Add(newFile);
                web.Context.Load(uploadFile);
                web.Context.ExecuteQuery();

                var listItem = uploadFile.ListItemAllFields;
                if (uploadFile.CheckOutType == CheckOutType.None)
                {
                    uploadFile.CheckOut();
                }

                listItem["ContentTypeId"] = masterpageContentTypeId;
                listItem["UIVersion"] = Convert.ToString(15);
                listItem.Update();
                uploadFile.CheckIn("", CheckinType.MajorCheckIn);
                listItem.File.Publish("");
                clientContext.Load(listItem);
                clientContext.ExecuteQuery();

            }
        }


        private void UploadFiles(ClientContext clientContext, Web web, List masterPageList, string serverRelatedURL)
        {

            var branding = XDocument.Load(HostingEnvironment.MapPath(string.Format("~/{0}", "settings.xml"))).Element("branding");
            var brandingName = branding.Attribute("name").Value;

            var folder = masterPageList.RootFolder;
            clientContext.Load(folder);
            clientContext.ExecuteQuery();
            folder = folder.Folders.Add(brandingName);
            folder.Folders.Add("css");
            folder.Folders.Add("scripts");
            folder.Folders.Add("images");

            clientContext.ExecuteQuery();

            foreach (var file in branding.Element("cssfiles").Descendants("file"))
            {
                var name = file.Attribute("name").Value;
                string fileURL = string.Format("JCIBrandingPOC/CSS/{0}", name);
                string remoteFileURL = string.Format("{0}_catalogs/masterpage/{1}/css/{2}", serverRelatedURL, brandingName, name);

                //Upload files 
                UploadFiles(clientContext, web, masterPageList, fileURL, remoteFileURL);
            }

            foreach (var file in branding.Element("imagefiles").Descendants("file"))
            {
                var name = file.Attribute("name").Value;
                string fileURL = string.Format("JCIBrandingPOC/images/{0}", name);
                string remoteFileURL = string.Format("{0}_catalogs/masterpage/{1}/images/{2}", serverRelatedURL, brandingName, name);

                //Upload files 
                UploadFiles(clientContext, web, masterPageList, fileURL, remoteFileURL);
            }

            foreach (var file in branding.Element("jsfiles").Descendants("file"))
            {
                var name = file.Attribute("name").Value;
                string fileURL = string.Format("JCIBrandingPOC/scripts/{0}", name);
                string remoteFileURL = string.Format("{0}_catalogs/masterpage/{1}/scripts/{2}", serverRelatedURL, brandingName, name);

                //Upload files 
                UploadFiles(clientContext, web, masterPageList, fileURL, remoteFileURL);
            }

        }

        private void UploadFiles(ClientContext clientContext, Web web, List list, string fileURL, string romoteFileURL)
        {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(string.Format("~/{0}", fileURL)));
                newFile.Url = romoteFileURL;
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File uploadFile = list.RootFolder.Files.Add(newFile);
                web.Context.Load(uploadFile);
                web.Context.ExecuteQuery();

                var listItem = uploadFile.ListItemAllFields;
                if (uploadFile.CheckOutType == CheckOutType.None)
                {
                    uploadFile.CheckOut();
                }
                listItem.Update();
                uploadFile.CheckIn("", CheckinType.MajorCheckIn);
                listItem.File.Publish("");
                clientContext.Load(listItem);
                clientContext.ExecuteQuery();
        }

        private string GetContentType(ClientContext clientContext, List list, string contentType)
        {
            ContentTypeCollection collection = list.ContentTypes;
            clientContext.Load(collection);
            clientContext.ExecuteQuery();
            var ct = collection.Where(c => c.Name == contentType).FirstOrDefault();
            string contentTypeID = "";
            if (ct != null)
            {
                contentTypeID = ct.StringId;
            }

            return contentTypeID;
        }


        protected void btnApply_Click(object sender, EventArgs e)
        {
            //var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            //using (var clientContext = spContext.CreateUserClientContextForSPHost())
            //{
            //    // Assign master page to the host web
            //    clientContext.Web.CustomMasterUrl = "/_catalogs/masterpage/JCIBrandingPOC.master";
            //    clientContext.Web.MasterUrl = "/_catalogs/masterpage/JCIBrandingPOC.master";
            //    clientContext.Web.Update();
            //    lblInfo.Text = string.Format("Custom master page called 'JCIBrandingPOC.master' has been uploaded and applied to the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            //}

            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                var web = clientContext.Web;
                clientContext.Load(
                 web,
                 website => website.ServerRelativeUrl,
                 website => website.Created);
                clientContext.Load(web);
                clientContext.ExecuteQuery();
                var masterPageUrl = URLCombine(clientContext.Web.ServerRelativeUrl, "/_catalogs/masterpage/JCIDemoTheme.master");
                web.MasterUrl = masterPageUrl;
                web.CustomMasterUrl = masterPageUrl;
                web.Update();
                clientContext.ExecuteQuery();
                 lblInfo.Text = "Master Page applied Successfully!";
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Assign master page to the host web
                clientContext.Web.CustomMasterUrl = "/_catalogs/masterpage/seattle.master";
                clientContext.Web.MasterUrl = "/_catalogs/masterpage/seattle.master";
                clientContext.Web.Update();
                lblInfo.Text = string.Format("master page reset to 'seattle.master'.", spContext.SPHostUrl.ToString());
            }
        }


        #region Custom List

        private void AddItemsToHeroList(Web web, string title, string imageUrl, string leftCaptionBackgroundColor, string leftCaptionBackgroundOpacity,
     string linkUrl, string rightCaptionDescrption, string rightCaptionTitle, string sortOrder, string tagLine)
        {
            List list = web.Lists.GetByTitle("POC");
            Microsoft.SharePoint.Client.ListItem newListItem = list.AddItem(new ListItemCreationInformation());
            newListItem["Title"] = title;
            string image = string.Format("<a href='{0}'><img src='{1}' alt='this is my image'></a>", linkUrl, imageUrl);
            newListItem["branding_Image"] = image;
            newListItem["branding_LeftCaptionBGColor"] = leftCaptionBackgroundColor;
            newListItem["branding_LeftCaptionBGOpacity"] = leftCaptionBackgroundOpacity;
            newListItem["branding_RightCaptionDescription"] = rightCaptionDescrption;
            newListItem["branding_RightCaptionTitle"] = rightCaptionTitle;
            newListItem["branding_SortOrder"] = sortOrder;
            newListItem["branding_TagLine"] = tagLine;
            newListItem["branding_LeftCaptionBGColor"] = leftCaptionBackgroundColor;

            newListItem.Update();

            web.Context.ExecuteQuery();
        }

        private void UploadImagesToDocumentLibrary(Web web, string fileAddress)
        {
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(fileAddress);
            newFile.Url = System.IO.Path.GetFileName(fileAddress);
            newFile.Overwrite = true;

            List documentLibrary = web.Lists.GetByTitle("Documents");
            Microsoft.SharePoint.Client.File uploadFile = documentLibrary.RootFolder.Files.Add(newFile);
            web.Context.Load(uploadFile);
            web.Context.ExecuteQuery();
        }

        private static void AddContentTypeToList(ClientContext clientContext, Web web, string contentTypeId, string listId)
        {
            List list = web.Lists.GetById(new Guid(listId));
            list.ContentTypesEnabled = true;
            var ct = web.ContentTypes.GetById(contentTypeId);
            clientContext.Load(ct);
            clientContext.ExecuteQuery();

            list.ContentTypes.AddExistingContentType(ct);

            clientContext.ExecuteQuery();

            List<string> fields = GetDemoFieldIds();
            foreach (string str in fields)
            {
                Field f = list.Fields.GetById(new Guid(str));
                f.SetShowInDisplayForm(true);
                f.SetShowInEditForm(true);
                f.SetShowInNewForm(true);

                f.Hidden = false;
                f.UpdateAndPushChanges(true);

            }
            clientContext.ExecuteQuery();

            DeleteDefaultContentTypeFromList(clientContext, web, list);
        }

        private static void DeleteDefaultContentTypeFromList(ClientContext clientContext, Web web, List list)
        {
            //Delete default content type            
            ContentTypeCollection collection = list.ContentTypes;
            clientContext.Load(collection);
            clientContext.ExecuteQuery();
            string contentTypeID = collection.Where(c => c.Name == "Item").FirstOrDefault().StringId;
            ContentType ct = list.ContentTypes.GetById(contentTypeID);
            ct.DeleteObject();
            clientContext.ExecuteQuery();
        }

        private void BindFieldsToContentType(ClientContext clientContext, Web web, string contentTypeId)
        {
            ContentType ct = web.ContentTypes.GetById(contentTypeId);
            clientContext.Load(ct);

            List<string> fields = GetDemoFieldIds();
            foreach (string str in fields)
            {
                FieldLinkCreationInformation fieldLink = new FieldLinkCreationInformation();
                var field = web.Fields.GetById(new Guid(str));
                fieldLink.Field = field;
                ct.FieldLinks.Add(fieldLink);
            }
            ct.Update(true);
            clientContext.ExecuteQuery();
        }

        private void CreateFields(ClientContext clientContext, Web web)
        {
            List<string> fieldsList = BuildDemoFields();
            foreach (string str in fieldsList)
            {
                Field field = web.Fields.AddFieldAsXml(str, false, AddFieldOptions.AddFieldToDefaultView);

            }

            clientContext.ExecuteQuery();
        }

        private string CreateContentType(ClientContext clientContext, Web web)
        {
            ContentTypeCollection contentTypeColl = clientContext.Web.ContentTypes;
            ContentTypeCreationInformation contentTypeCreation = new ContentTypeCreationInformation();
            contentTypeCreation.Name = "POC";
            contentTypeCreation.Description = "Custom Content Type created for POC";
            contentTypeCreation.Group = "Branding";
            contentTypeCreation.Id = ContentTypeID;

            //Add the new content type to the collection
            ContentType ct = contentTypeColl.Add(contentTypeCreation);
            clientContext.Load(ct);
            clientContext.ExecuteQuery();
            return ct.Id.ToString();
        }

        private static List<string> BuildDemoFields()
        {
            List<string> fieldsSchemaList = new List<string>();
            fieldsSchemaList.Add("<Field Type='Text' DisplayName='Tag Line ' ID='{a2589f26-1642-41f2-a6c2-565a0d4e3a88}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_TagLine' Name='branding_TagLine' MaxLength='255' Group='Branding' Required='FALSE' Version='1' Customization=''/>");
            fieldsSchemaList.Add("<Field Type='Text' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Left Caption Background Color' ID='{bcc81121-d55d-4973-baec-aaf221cfd4dc}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_LeftCaptionBGColor' Name='branding_LeftCaptionBGColor' MaxLength='255' Group='Branding' Required='TRUE' Version='1' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Text' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Left Caption Background Opacity' ID='{d4cadb85-7c72-4aa7-9a19-fa42ebd96889}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_LeftCaptionBGOpacity' Name='branding_LeftCaptionBGOpacity' MaxLength='255' Group='Branding' Required='TRUE' Version='1' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Image' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Branding Image' RichText='TRUE' RichTextMode='FullHtml' ID='{6ead18fe-1c31-4d83-8edc-e421db18c560}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_Image' Name='branding_Image' Group='Branding' Required='TRUE' Version='1' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Text' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='LinkURL' ID='{3f18835a-ef65-4221-9f17-51ebe05a958d}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_LinkURL' Name='branding_LinkURL' MaxLength='255' Group='Branding' Required='FALSE' Version='1' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Note' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Right Caption Description' ID='{5d50f254-0980-48a3-b5d4-cef46368226e}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_RightCaptionDescription' Name='branding_RightCaptionDescription' NumLines='3' UnlimitedLengthInDocumentLibrary='FALSE' AllowHyperlink='FALSE' RichText='FALSE' RichTextMode='Compatible' Group='Branding' Required='TRUE' Version='1' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Text' ShowInDisplayForm='1' ShowInEditForm='1' ShowInNewForm='True'   DisplayName='Right Caption Title' ID='{e50b4c7e-f3e2-4c00-9ea0-685e514f7a92}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_RightCaptionTitle' Name='branding_RightCaptionTitle' MaxLength='255' Group='Branding' Required='TRUE' Version='1' Customization='' />");
            fieldsSchemaList.Add("<Field Type='Number' ShowInDisplayForm='1' ShowInEditForm='1'  ShowInNewForm='True'  DisplayName='Sort Order' ID='{6b0c3485-17af-407a-90d0-63a9a4fb10e6}' SourceID='{9a21af5d-5028-47a1-bf9c-7a97128d754e}' StaticName='branding_SortOrder' Name='branding_SortOrder' Min='0' Percentage='FALSE' Decimals='0' Group='Branding' Required='TRUE' Version='1' Customization='' />");
            return fieldsSchemaList;
        }

        private static List<string> GetDemoFieldIds()
        {
            List<string> fieldIds = new List<string>();
            fieldIds.Add("a2589f26-1642-41f2-a6c2-565a0d4e3a88");
            fieldIds.Add("bcc81121-d55d-4973-baec-aaf221cfd4dc");
            fieldIds.Add("d4cadb85-7c72-4aa7-9a19-fa42ebd96889");
            fieldIds.Add("6ead18fe-1c31-4d83-8edc-e421db18c560");
            fieldIds.Add("3f18835a-ef65-4221-9f17-51ebe05a958d");
            fieldIds.Add("5d50f254-0980-48a3-b5d4-cef46368226e");
            fieldIds.Add("e50b4c7e-f3e2-4c00-9ea0-685e514f7a92");
            fieldIds.Add("6b0c3485-17af-407a-90d0-63a9a4fb10e6");
            return fieldIds;
        }

        protected string CreateList(ClientContext clientContext, Web web)
        {
            string listName = "Configuration List";
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = listName;
            creationInfo.TemplateType = (int)ListTemplateType.GenericList;
            List list = web.Lists.Add(creationInfo);
            list.Description = "Configuration List";
            list.Update();
            clientContext.Load(list);
            clientContext.ExecuteQuery();
            return list.Id.ToString();
        }


        private string URLCombine(string baseUrl, string relativeUrl)
        {
            if (baseUrl.Length == 0)
                return relativeUrl;
            if (relativeUrl.Length == 0)
                return baseUrl;
            return string.Format("{0}/{1}", baseUrl.TrimEnd(new char[] { '/', '\\' }), relativeUrl.TrimStart(new char[] { '/', '\\' }));
        }

        #endregion
    }
}