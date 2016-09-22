#load "image-lib.csx"

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

public static async Task Run(byte[] image, string filename, Stream outputBlob, TraceWriter log)
{
    string result = await CallEmotionAPI(image);
    log.Info(result);

    if (String.IsNullOrEmpty(result)) {
        return;
    }
 
    var personInfo = GetNameAndTitle(filename);

    var imageData = JsonConvert.DeserializeObject<Face[]>(result);
    var faceData = imageData[0]; // assume exactly one face
}

static async Task<string> CallEmotionAPI(byte[] image)
{
    var client = new HttpClient();

    var content = new StreamContent(new MemoryStream(image));
    var url = "https://api.projectoxford.ai/emotion/v1.0/recognize";
    var key = Environment.GetEnvironmentVariable("EmotionAPIKey");

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
    var httpResponse = await client.PostAsync(url, content);

    if (httpResponse.StatusCode == HttpStatusCode.OK)
    {
        return await httpResponse.Content.ReadAsStringAsync();
    }

    return null;
}