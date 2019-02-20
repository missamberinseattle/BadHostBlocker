using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BadHostBlocker;
using System.Net;
using System.IO;

namespace IpWhiteLister
{
    public partial class _Default : System.Web.UI.Page
    {
        public string RemoteIp
        {
            get
            {
                return Request.ServerVariables["REMOTE_ADDR"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            IpAddress.InnerText = RemoteIp;
        }

        protected void Go_Click(object sender, EventArgs e)
        {
            if (Passcode.Value != "Fuck!ngSp@mm3rz1")
            {
                ErrorMessage.Visible = true;
                ErrorMessage.InnerText = "Sorry, I don't recognize that code.";
                return;
            }

            var pagePath = Request.MapPath("~/");
            var catalogPath = BadHostPlugin.CatalogPath;
            catalogPath = catalogPath.Replace(Path.GetPathRoot(catalogPath), Path.GetPathRoot(pagePath));

            try
            {
                InputDiv.Visible = false;
                StatusDiv.Visible = true;

                var catalog = IpCatalogItem.LoadCatalog(catalogPath);

                var item = IpCatalogItem.Find(RemoteIp, catalog);

                if (item != null)
                {
                    item.State = ListState.WhiteList;
                }
                else
                {
                    IPAddress address;
                    address = IPAddress.Parse(RemoteIp);

                    item = new IpCatalogItem(address, -1, DateTime.Now, DateTime.Now, null, ListState.WhiteList, DateTime.Now);
                    catalog.Add(item);
                }

                IpCatalogItem.Save(catalog, catalogPath);

                StatusDiv.InnerText = "Success! " + RemoteIp + " has been white listed!";
            }
            catch (Exception ex)
            {
                StatusDiv.InnerText = "Could not add " + RemoteIp + " to catalog. " + ex.Message;
            }
        }
    }
}