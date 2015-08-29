using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fiddler;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Specialized;


[assembly: Fiddler.RequiredVersion("4.6.0.2")]

namespace FPDDiger
{
    public class Demo : IAutoTamper, IFiddlerExtension
    {
        private UserInterface ui;
        private bool bLoaded;

        private ArrayList geturlList;
        private ArrayList posturlList;
        private ArrayList xGETurlList;
        private ArrayList xPOSTdataList;
        private Regex regex;
        private Match m;
        private string strParams = string.Empty;


        public Demo() { }
        public void OnLoad()
        {
            this.ui = new UserInterface();
            this.bLoaded = true;

            this.geturlList = new ArrayList();
            this.posturlList = new ArrayList();
            this.xGETurlList = new ArrayList();
            this.xPOSTdataList = new ArrayList(); 
        }
        public void OnBeforeUnload(){  /*noop*/ }

        public void AutoTamperRequestBefore(Session oSession)
        {
            if (this.bLoaded && this.ui.bEnabled)
            {
                this.regex = new Regex(@"\/\S*\?");
                this.m = this.regex.Match(oSession.url);
                if (m.Success)
                {
                    try
                    {
                            if(oSession.RequestMethod == "GET")
                            {
                                if (this.geturlList.Contains(oSession.url) || this.xGETurlList.Contains(oSession.url))
                                    return;
                                this.geturlList.Add(oSession.url);
                                this.strParams = oSession.url.Split('?')[1];
                                string[] Params = strParams.Split('&');
                                foreach (string strParamname in Params)
                                {
                                    string strUrl = oSession.url;
                                    strUrl = strUrl.Replace(strParamname.Split('=')[0], strParamname.Split('=')[0] + "[]");
                                    this.xGETurlList.Add(strUrl);     
                                    FiddlerApplication.oProxy.InjectCustomRequest(oSession.ToString().Replace(oSession.url, strUrl));
                                }
                            }
                            else if(oSession.RequestMethod == "POST")
                            {
                                if (this.posturlList.Contains(oSession.url) && this.xPOSTdataList.Contains(oSession.GetRequestBodyEncoding().GetString(oSession.requestBodyBytes)))
                                    return;
                                this.posturlList.Add(oSession.url);
                                this.xPOSTdataList.Add(oSession.GetRequestBodyEncoding().GetString(oSession.requestBodyBytes));
                                this.strParams = oSession.GetRequestBodyEncoding().GetString(oSession.requestBodyBytes);
                                string[] Params = strParams.Split('&');
                                foreach (string strParamname in Params)
                                {
                                    String strData = oSession.ToString();
                                    strData = this.strParams.Replace(strParamname.Split('=')[0], strParamname.Split('=')[0] + "[]");
                                    this.xPOSTdataList.Add(this.strParams.Replace(strParamname.Split('=')[0], strParamname.Split('=')[0] + "[]"));
                                    StringDictionary dictionary = new StringDictionary();
                                    dictionary["Flag"] = "FreeBuf";
                                    FiddlerApplication.oProxy.InjectCustomRequest(oSession.oRequest.headers, oSession.GetRequestBodyEncoding().GetBytes(strData), dictionary);
                                }
                            }
                    }catch { }
                }
            }
        }
        public void AutoTamperRequestAfter(Session oSession) { /* noop */ }
        public void AutoTamperResponseBefore(Session oSession) 
        {
            if (this.bLoaded && this.ui.bEnabled)
            {
                if (oSession.responseCode == 404)
                    return;
                if (this.xGETurlList.Contains(oSession.url) || this.posturlList.Contains(oSession.url))
                {
                    string strResponse = oSession.GetResponseBodyAsString().Replace('\\',  '/');
                    this.regex = new Regex(@"PHP\s*\w*\:.*\/\w*.php");
                    this.m = this.regex.Match(strResponse);
                    if (this.m.Success)
                    {
                        if (oSession.RequestMethod == "GET")
                            this.ui.AddResult("[" + oSession.RequestMethod + "]    " + oSession.url);
                        else if (oSession.RequestMethod == "POST")
                        {
                            this.ui.AddResult("[" + oSession.RequestMethod + "]    " + oSession.url);
                            this.ui.AddResult("[POST Data:]    " + oSession.GetRequestBodyEncoding().GetString(oSession.requestBodyBytes));
                        }
                    }
                    else return ; 
                }
            }
        
        }
        public void AutoTamperResponseAfter(Session oSession) { /* noop */ }
        public void OnBeforeReturningError(Session oSession) { /* noop */ }



    }
}
