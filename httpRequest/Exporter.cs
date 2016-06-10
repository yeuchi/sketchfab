using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace httpRequest
{
    public class Exporter
    {
        protected string url = "https://api.sketchfab.com/model";
        public string response;

        public Exporter(){}

        public bool upload( string url,
                            string fileSrc, 
                            string txtTitle, 
                            string txtDesc, 
                            string txtToken)
        {
            try
            {
                this.url = url;
                byte[] byteArray = readFile(fileSrc);
                if (null == byteArray)
                {
                    response = "Invalid file read.";
                    return false;
                }

                // http://arcanecode.com/2007/03/21/encoding-strings-to-base64-in-c/
                string strBase64 = System.Convert.ToBase64String(byteArray);
                string txtFilename = getFilename(fileSrc);
                string strJSON = createJSON(txtFilename, txtTitle, txtDesc, txtToken, strBase64);
                byte[] bytesJSON = getBytes(strJSON);
                return sendHTTPRequest(bytesJSON);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return false;
        }

        protected string getFilename(string fileSrc) {
            string name = fileSrc;
            int pos = fileSrc.LastIndexOf("\\");
            if (pos > 0)
                name = fileSrc.Substring(pos+1, fileSrc.Length - (pos+1));
            return name;
        }

        protected bool sendHTTPRequest(byte[] bytesJSON)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                // http://stackoverflow.com/questions/6079282/emulate-xmlhttprequest-with-a-net-webclient

                //Set HttpWebRequest properties
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = bytesJSON.Length;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    //Writes a sequence of bytes to the current stream 
                    requestStream.Write(bytesJSON, 0, bytesJSON.Length);
                    requestStream.Close();//Close stream
                }

                //Sends the HttpWebRequest, and waits for a response.
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                //Get response stream into StreamReader
                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                        response = reader.ReadToEnd();
                }

                httpWebResponse.Close();//Close HttpWebResponse

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                response = "Something bad happened." + e.ToString();
            }
            return false;
        }

        protected byte[] readFile(string fileSrc)
        {
            // http://msdn.microsoft.com/en-us/library/system.io.filestream.aspx
            using (FileStream fs = File.OpenRead(fileSrc))
            {
                int len = (int)fs.Length;
                byte[] byteArray = new byte[len];
                fs.Read(byteArray, 0, len);
                return byteArray;
            }
        }

        protected string createJSON(string txtFilename, 
                                    string txtTitle, 
                                    string txtDesc, 
                                    string txtToken, 
                                    string strBase64)
        {
            string str = "{\"description\": \""+txtDesc+"\", ";
            str += " \"tags\": \"test upload\", ";
            str += " \"title\": \""+txtTitle+"\", ";
            str += " \"filename\": \""+txtFilename+"\", ";
            str += " \"title\": \"" + txtTitle + "\", ";
            str += " \"token\": \""+txtToken+"\", ";
            str += " \"contents\": \" ";
            str += strBase64 + " \"}";

            return str;
        }

        //http://stackoverflow.com/questions/472906/net-string-to-byte-array-c-sharp
        protected byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length];
            char[] c = str.ToCharArray();
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)c[i];
            //System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
