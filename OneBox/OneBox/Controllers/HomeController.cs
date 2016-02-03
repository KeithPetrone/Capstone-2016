﻿using System;
using Google.Apis.Drive.v2;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Drive.v2.Data;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.IO;

namespace OneBox.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FileList()
        {
            ViewBag.Message = "Your file list page.";
            GoogleDriveDownloader g = new GoogleDriveDownloader();
            DropBoxDownloader d = new DropBoxDownloader();

            IEnumerable<Google.Apis.Drive.v2.Data.File> googleresults = g.Download();
            TempData["GoogleResult"] = googleresults;

            IEnumerable<DropboxRestAPI.Models.Core.MetaData> dropboxresults = d.Download();
            TempData["DropBoxResult"] = dropboxresults;

            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            GoogleDriveDownloader g = new GoogleDriveDownloader();

            var fileName = Path.GetFileName(file.FileName);
            var path = "C:/TempFiles/" + fileName;
            file.SaveAs(Path.Combine(@"C:/TempFiles", fileName));

            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
            body.Title = System.IO.Path.GetFileName(path);
            body.Description = "File uploaded OneBox";
            body.MimeType = GetMimeType(fileName);

            byte[] byteArray = System.IO.File.ReadAllBytes(path);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
            g.Upload(body, stream, GetMimeType(fileName));

            return Redirect("FileList");
        }

        [HttpPost]
        public ActionResult FileList(string search)
        {
            GoogleDriveDownloader g = new GoogleDriveDownloader();

            IEnumerable<Google.Apis.Drive.v2.Data.File> results = g.Search(search);
            TempData["Result"] = results;

            return View();
        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}