using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using MVCAppToBlob.Models;
using System.Net.Http;
using System.Net;
//using System.Web.Http;
using System.IO;

namespace MVCAppToBlob.Controllers
{
    /// <summary>
    /// Azure storage controller
    /// </summary>
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        /// <summary>
        /// Update Blob
        /// </summary>
        /// <param name="attendence"></param>
        /// <returns></returns>
        // POST: Attendanc/Create
        [System.Web.Http.HttpPost]
        //[ValidateAntiForgeryToken]
        // [Microsoft.AspNetCore.Mvc.Route("UpdateAzureBlob")]
        [Route("UpdateAzureBlob")]
        public ActionResult UpdateAzureBlob([FromBody]Attendence attendence)
        {
            try
            {
                string attendStr = Newtonsoft.Json.JsonConvert.SerializeObject(attendence);
                string conStr = "DefaultEndpointsProtocol=https;AccountName=studentstorageaccount;AccountKey=LQ4aJxyeX670rQ6DlHdKOG61lTaYMohSCe6RwL3Ug22/AbSJHRMh/t6imZffYobB5cVJh5brTyD++AStGfQKIA==;EndpointSuffix=core.windows.net";

                try
                {
                    UploadBlob(conStr, attendStr, "source", true);
                    
                }
                catch (Exception ex)
                {
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                }

                return Ok("Updated");
            }
            catch
            {
                return Ok("Updated");
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public string Get()
        {
            return "API Working";
        }
        [Route("GetAll/{id}")]
        public string Get(int id)
        {
            return "API Working with id: "+id;
        }


        /// <summary>
        /// Update the blok
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="fileContent"></param>
        /// <param name="containerName"></param>
        /// <param name="isAppend"></param>
        /// <returns></returns>
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
    }
}
