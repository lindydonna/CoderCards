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
    string result = await CallVisionAPI(image);
    log.Info(result);

    if (String.IsNullOrEmpty(result)) {
        return;
    }

    var personInfo = GetNameAndTitle(filename);

    var imageData = JsonConvert.DeserializeObject<Face[]>(result);
    var faceData = imageData[0]; // assume exactly one face

    double leftScore = 0, rightScore = 0;
    var card = GetCardImageAndScores(faceData.Scores, out leftScore, out rightScore);

    MergeCardImage(card, image, personInfo, leftScore, rightScore);

    SaveAsJpeg(card, outputBlob);
}

static Image GetCardImageAndScores(Scores scores, out double leftScore, out double rightScore)
{
    NormalizeScores(scores);

    var cardBack = "neutral.png";
    leftScore = scores.Neutral;

    const int angerBoost = 2, happyBoost = 4;

    if (scores.Surprise > 10) {
        cardBack = "surprised.png";
        leftScore = scores.Surprise;
    }
    else if (scores.Anger > 10) {
        cardBack = "angry.png";
        leftScore = scores.Anger * angerBoost;
    }
    else if (scores.Happiness > 50) {
        cardBack = "happy.png";
        leftScore = scores.Happiness * happyBoost;
    }

    rightScore = GetTotalScore(scores);

    return Image.FromFile(GetFullImagePath(cardBack));
}

static double GetTotalScore(Scores scores) =>
    scores.Anger + scores.Happiness + scores.Neutral + scores.Sadness + scores.Surprise;

static async Task<string> CallVisionAPI(byte[] image)
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