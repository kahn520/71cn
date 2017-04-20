using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace _71cn
{
    public class DownLoader
    {
        private readonly string strBaseUrl = "http://upload.71.cn/pptnotes/";
        private string id;
        private List<string> listFiles;
        private string strDownloadPath;
        public DownLoader(string _id, string _path)
        {
            id = _id;
            listFiles = new List<string>();
            strDownloadPath = _path;
        }

        public List<string> Files
        {
            get { return listFiles; }
        }

        public void StartDownload()
        {
            int page = 0;
            using (WebClient webClient = new WebClient())
            {
                while (true)
                {
                    page++;
                    string strUrl = JoinDownloadUrl(id, page.ToString());
                    string strSave = $"{strDownloadPath.TrimEnd('\\')}\\{id}_{page}.pptx";
                    try
                    {
                        webClient.DownloadFile(strUrl, strSave);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    listFiles.Add(strSave);
                }
            }
        }

        private string JoinDownloadUrl(string id, string page)
        {
            return $"{strBaseUrl}{id}/{id}_{page}.pptx";
        }
    }
}
