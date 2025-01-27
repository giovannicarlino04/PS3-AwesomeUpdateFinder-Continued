/* Awesome Update Finder - NZHawk 2010
 * 
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.

 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.

 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.

 *   I wrote this application at 4am, excuse any errors. Although it does seem to work fine
 *   it might be a little dodgy :P
 *   
 *   oh, icon http://blog.mozilla.com/faaborg/2008/12/16/new-os-x-icons-made-by-sofa/
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.Xml.Linq;
using System.IO;
using static System.Windows.Forms.LinkLabel;

namespace Awesome_Update_Findr
{
    public partial class Form1 : Form
    {
        string webUrl;
        string updateUrl; //L'ho resa pubblica per la funzione che copia i link
        XmlNodeList elemList;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

                //  Set up the url of the xml file containing the game updates
                webUrl = "https://a0.ww.np.dl.playstation.net/tpl/np/" + textBox1.Text + "/" + textBox1.Text + "-ver.xml";
                //  Update status..
                lbl_Url.Text = "fetching... " + webUrl;
                //  Needed to allow the certificate
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //  Make a new webClient to get the xml file and get/parse it on another thread.
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += HttpsCompleted;
                wc.DownloadStringAsync(new Uri(webUrl));
            
        }

        private void HttpsCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //  Check if it downloaded the xml file ok
            if (e.Error == null)
            {   
                //  Make an new XmlDocument
                XmlDocument xdoc = new XmlDocument();
                // Gotta love try :P
                try
                {
                    //  Load the xml file from e.Result into the XmlDoc
                    xdoc.LoadXml(e.Result);
                    //  Make nodeList to hold all the Package Elements
                    elemList = xdoc.GetElementsByTagName("package");
                    //  Loop through the list and get each entry
                    for (int i = 0; i < elemList.Count; i++)
                    {
                        //  Obvious?
                        string updateVersion = elemList[i].Attributes["version"].Value;
                        string updateSize = elemList[i].Attributes["size"].Value;
                        string updateSHA1 = elemList[i].Attributes["sha1sum"].Value;
                        updateUrl = elemList[i].Attributes["url"].Value;
                        string updateFWVer = elemList[i].Attributes["ps3_system_ver"].Value;
                        //  Add it to the dataGridView
                        dataGridView1.Rows.Add(updateVersion, updateSize, updateSHA1, updateUrl, updateFWVer);
                    }
                    //  Update the Status labels
                    lbl_gameTitle.Text = xdoc.SelectSingleNode("/titlepatch/tag/package/paramsfo/TITLE").InnerText;
                    lbl_Url.Text = webUrl;
                }
                //  If error..
                catch (Exception a)
                {
                    //  Show error
                    showError();
                }
            }
            else // If error..
            {
                //  Show error
                showError();
            }
        }

        private void showError()
        {
            //  Show error..
            MessageBox.Show("Error: Title ID not recognised! Please check it and try again.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //  Clear stuff
            lbl_Url.Text = "";
            lbl_gameTitle.Text = "";
            dataGridView1.Rows.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string clipboard = "";

            if (elemList != null)
            {
                //  Loop through the list and get each entry
                for (int i = 0; i < elemList.Count; i++)
                {
                    updateUrl = elemList[i].Attributes["url"].Value;
                    clipboard = $"{clipboard} {updateUrl}";
                }
                Clipboard.SetText(clipboard);
            }


        }
    }
}
