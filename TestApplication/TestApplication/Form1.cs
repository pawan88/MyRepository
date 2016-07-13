using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HealthCheckLib;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace TestApplication
{
    public partial class frmHome : Form,HealthCheckLib.IApplicationCheckClass
    {
        static string strAppName;
        string strXmlFilePath;
        string str;

        string teststr = "dfdshbfdsf";
        public frmHome()
        {
            InitializeComponent();        
        }

        private void frmHome_Load(object sender, EventArgs e)
        {
            strXmlFilePath = System.Configuration.ConfigurationManager.AppSettings["filePath"].ToString();            
        }        

        private void btnChkUrl_Click(object sender, EventArgs e)
        {
            strAppName = "Test_URL";
            string strUrl = textBox2.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(strUrl))
            { 
                MessageBox.Show("Please enter valid Url");
                return;
            }            

            if (strUrl.Contains("http://") == false && strUrl.Contains("https://")==false)
            {
                strUrl = "http://" + strUrl;
                //MessageBox.Show("Invalid Url", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }

            List<ApplicationResultCheck> listResultCheck = new List<ApplicationResultCheck>();    
   
            ApplicationResultCheck objCheckClass = check(strUrl);
            ApplicationResultCheck objCheckClass1 = check(strUrl);

            listResultCheck.Add(objCheckClass);
            listResultCheck.Add(objCheckClass1);
            
            XmlDocument xmlDoc = generateXML(listResultCheck, strAppName, "Processing", "Dev101");
            xmlDoc.Save(strXmlFilePath);
            strUrl = string.Empty;
        }

        public XmlDocument generateXML(List<ApplicationResultCheck> listParameter, string strApplicationName, string strOverallStatus, string strServerName)
        {

            XmlDocument xmlDoc = new XmlDocument();

            XmlElement root = xmlDoc.CreateElement("application_result");
            root.SetAttribute("name", strApplicationName);
            root.SetAttribute("status", strOverallStatus);
            root.SetAttribute("server-name", strServerName);
            xmlDoc.AppendChild(root);

            foreach (ApplicationResultCheck item in listParameter)
            {
                XmlNode userNode = xmlDoc.CreateElement("test_result");

                XmlAttribute xmlAttName = xmlDoc.CreateAttribute("name");
                xmlAttName.Value = item.strName;
                userNode.Attributes.Append(xmlAttName);

                XmlAttribute xmlAttStatus = xmlDoc.CreateAttribute("status");
                xmlAttStatus.Value = item.strStatus;
                userNode.Attributes.Append(xmlAttStatus);

                XmlAttribute xmlAttMessage = xmlDoc.CreateAttribute("message");
                xmlAttMessage.Value = item.strMessage;
                userNode.Attributes.Append(xmlAttMessage);

                XmlAttribute xmlAttElapsedTime = xmlDoc.CreateAttribute("elapsedtime");
                xmlAttElapsedTime.Value = item.StrElapsedTime;
                userNode.Attributes.Append(xmlAttElapsedTime);

                root.AppendChild(userNode);
            }

            return xmlDoc;

        }


        public ApplicationResultCheck check()
        {            
            ApplicationResultCheck paramList = new ApplicationResultCheck();
            return paramList;
        }

        public ApplicationResultCheck check(string toCheckString)
        {            
            TimeSpan ts;            
            DateTime dtFrom = DateTime.Now;
            HealthCheckLib.ApplicationResultCheck objResultCheck = new ApplicationResultCheck();            

            if (strAppName == "Test_DBCON")
            {
                toCheckString = "";
            }
            else if (strAppName == "Test_URL")
            {
                try
                {
                    HttpWebRequest request = HttpWebRequest.Create(toCheckString) as HttpWebRequest;
                    request.Timeout = 5000;
                    request.Method = "HEAD";

                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    int statuscode = (int)response.StatusCode;

                    objResultCheck.strStatus = response.StatusCode.ToString();
                    objResultCheck.strMessage = "";
                    objResultCheck.strName = "Test Url";

                }
                catch (WebException ex)
                {
                    objResultCheck.strStatus = "Down";
                    objResultCheck.strMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    objResultCheck.strStatus = "Down";
                    objResultCheck.strMessage = ex.Message;
                }

                ts = DateTime.Now - dtFrom;
                objResultCheck.StrElapsedTime = ts.Seconds.ToString();                
            }
            else
            {
                toCheckString = "";
            }
                        
            return objResultCheck;
        }
        
        public ApplicationResultCheck check(string strParam1, string strParam2)
        {
            ApplicationResultCheck paramList = new ApplicationResultCheck();
            return paramList;
        }        

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void testXML()
        {
            XDocument doc;
            string filePath = "file.xml";
            if (!File.Exists(filePath))
            {
                doc = new XDocument(new XElement("Parent"));
                doc.Save(filePath);                
            }

            // Xml exists now
            // Adding a child to the xml 
            doc = XDocument.Load(filePath);
            doc.Element("Parent").Add(new XElement("Child",
                                        new XAttribute("Date", DateTime.Now.ToLongDateString() + " "
                                                        + DateTime.Now.ToShortTimeString()),
                                        new XAttribute("SeqID", "You ID"),
                                        new XElement("Project", new XText("Pra")),
                                        new XElement("Mehtod", new XText("PP")),
                                        new XElement("Msg", new XText("Pandit"))));
            doc.Save(filePath);            
        }

    }
}