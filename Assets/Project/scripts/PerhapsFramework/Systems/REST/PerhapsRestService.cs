using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using System.Diagnostics;

namespace Perhaps
{
    public enum RESTOperation
    {
        GET, POST, PUT, DELETE
    }

    public struct RestResponse
    {
        public bool valid;
        public long code;
        public string body;
        public long completionTime;
        public ulong downloadedBytes;
    }

    public static class PerhapsRestService
    {
        public static void SendRESTRequest(RESTOperation operation, string uri, string body = "", Action<RestResponse> responseCallback = null)
        {        
            Uri _uri = new Uri(uri);
            UnityWebRequest request = new UnityWebRequest(_uri);
            request.method = operation.ToString();

            if ((operation != RESTOperation.GET || operation != RESTOperation.DELETE) && body != null)
            {
                body = Beautify(body);
                byte[] bytes = Encoding.UTF8.GetBytes(body);
                request.uploadHandler = new UploadHandlerRaw(bytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
            }

            Stopwatch sw = Stopwatch.StartNew();
            request.SendWebRequest().completed += task =>
            {
                sw.Stop();
                long time = sw.ElapsedMilliseconds;

                RestResponse response = new RestResponse();
                response.completionTime = time;
                response.code = request.responseCode;
                response.valid = false;


                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    response.downloadedBytes = request.downloadedBytes;
                    response.body = request.downloadHandler.text;
                    try
                    {
                        response.body = Beautify(response.body);
                        response.valid = true;
                    }
                    catch
                    {
                        
                    }
                }
                else
                {
                    response.body = "";
                }
                
                if(responseCallback != null)
                {
                    responseCallback(response);
                }
                
            };
        }

        public static string GetResponseString(long code)
        {
            string responseInfo = "";
            switch (code)
            {
                case 0:
                    responseInfo = "Failed to establish connection";
                    break;
                case 200:
                    responseInfo = "Ok";
                    break;
                case 204:
                    responseInfo = "No Content";
                    break;
                case 401:
                    responseInfo = "Unauthorized";
                    break;
                case 403:
                    responseInfo = "Forbidden";
                    break;
                case 404:
                    responseInfo = "Not found";
                    break;
                case 405:
                    responseInfo = "Not allowed";
                    break;
                case 429:
                    responseInfo = "Too many requests.";
                    break;
                case 500:
                case 501:
                    responseInfo = "Server error";
                    break;
                default:
                    break;
            }

            return responseInfo;
        }

        public static string Beautify(string uglyJson)
        {
            return null;
            /*
            object parsedJson = JsonConvert.DeserializeObject(uglyJson);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            */
        }
    }

}