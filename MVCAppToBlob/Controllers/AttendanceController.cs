using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCAppToBlob.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVCAppToBlob.Controllers
{
    
    public class AttendanceController : Controller
    {
       // GET: Attendanc


        public ActionResult Index()
        {
            return View("Create");
        }

        // GET: Attendanc/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //[HttpGet]

        // GET: Attendanc/Create
        public ActionResult Create()
        {
            var model = new Attendence();
            // model.Name = "Please enter name";
            return View(model);
        }

        // POST: Attendanc/Create

        [ValidateAntiForgeryToken]
        

        public ActionResult Create(Attendence attendence)
        {
            try
            {
                string attendStr = Newtonsoft.Json.JsonConvert.SerializeObject(attendence);
                string conStr = "DefaultEndpointsProtocol=https;AccountName=studentstorageaccount;AccountKey=LQ4aJxyeX670rQ6DlHdKOG61lTaYMohSCe6RwL3Ug22/AbSJHRMh/t6imZffYobB5cVJh5brTyD++AStGfQKIA==;EndpointSuffix=core.windows.net";

                try
                {
                    UploadBlob(conStr, attendStr, "source", true);
                    ViewBag.MessageToScreent = "Details Updated to Blob :" + attendStr;
                }
                catch (Exception ex)
                {
                    ViewBag.MessageToScreent = "Failed to update blob " + ex.Message;
                }

                return View("Create");
            }
            catch
            {
                return View("Create");
            }
        }

        public static string UploadBlob(string conStr, string fileContent, string containerName, bool isAppend = false)
        {
            //[{w.v.f.newdata,newdata}]

            ///
            string result = "Success";
            try
            {
                //string containerName = "sample1";
                string fileName, existingContent;
                BlobClient blobClient;
                SetVariables(conStr, containerName, out fileName, out existingContent, out blobClient);

                if (isAppend)
                {
                    string fillerStart = "";
                    string fillerEnd = "]";
                    existingContent = GetContentFromBlob(conStr, fileName, containerName);
                    if (string.IsNullOrEmpty(existingContent.Trim()))
                    {
                        fillerStart = "[";
                        fileContent = fillerStart + existingContent + fileContent + fillerEnd;

                    }
                    else
                    {
                        existingContent = existingContent.Substring(0, existingContent.Length - 3);
                        fileContent = fillerStart + existingContent + "," + fileContent + fillerEnd;
                    }
                }

                var ms = new MemoryStream();
                TextWriter tw = new StreamWriter(ms);
                tw.Write(fileContent);
                tw.Flush();
                ms.Position = 0;

                blobClient.UploadAsync(ms, true);

            }
            catch (Exception ex)
            {

                result = "Failed";
                throw ex;
            }
            return result;
        }

        private static void SetVariables(string conStr, string containerName, out string fileName, out string existingContent, out BlobClient blobClient)
        {
            var serviceClient = new BlobServiceClient(conStr);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);

            fileName = "data.txt";
            existingContent = "";
            blobClient = containerClient.GetBlobClient(fileName);
        }

        private static string GetContentFromBlob(string conStr, string fileName, string containerName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(conStr);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            string line = string.Empty;
            if (blobClient.Exists())
            {
                var response = blobClient.Download();
                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    while (!streamReader.EndOfStream)
                    {
                        line += streamReader.ReadLine() + Environment.NewLine;
                    }
                }
            }
            return line;
        }
    
//GET: Attendanc / Edit / 5
        public ActionResult Edit(int id)
{
    return View();
}

// POST: Attendanc/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit(int id, IFormCollection collection)
{
    try
    {
        return RedirectToAction(nameof(Index));
    }
    catch
    {
        return View();
    }
}

        // GET: Attendanc/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Attendanc/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
