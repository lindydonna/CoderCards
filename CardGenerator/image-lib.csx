#r "Newtonsoft.Json"
#r "System.Drawing"

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

static double RoundScore(double score) => Math.Round(score * 100);

static void NormalizeScores(Scores scores)
{
    scores.Anger = RoundScore(scores.Anger);
    scores.Happiness = RoundScore(scores.Happiness);
    scores.Neutral = RoundScore(scores.Neutral);
    scores.Sadness = RoundScore(scores.Sadness);
    scores.Surprise = RoundScore(scores.Surprise);
}

static Image MergeCardImage(Image card, byte[] imageBytes, Tuple<string, string> personInfo, double leftScore, double rightScore)
{
    using (MemoryStream faceImageStream = new MemoryStream(imageBytes)) 
    {
        const int topLeftFaceX   = 82;
        const int topLeftFaceY   = 60;
        const int faceRect       = 380;
        const int namePosY       = 470;
        const int titlePosY      = 514;
        const int scoreLeftX     = 36;
        const int scoreRightX    = 445;
        const int scorePosY      = 684;
        const int scoreWidth     = 64;

        using (Image faceImage = Image.FromStream(faceImageStream, true)) 
        {
            using (Graphics g = Graphics.FromImage(card)) 
            {
                g.DrawImage(faceImage, topLeftFaceX, topLeftFaceY, faceRect, faceRect);
                RenderText(card, g, namePosY, personInfo.Item1);
                RenderText(card, g, titlePosY, personInfo.Item2);

                RenderScores(g, scoreLeftX, scorePosY, scoreWidth, leftScore.ToString());
                RenderScores(g, scoreRightX, scorePosY, scoreWidth, rightScore.ToString());
            }

            return card;
        }
    }
}

static void RenderScores(Graphics graphics, int xPos, int yPos, int width, string score)
{
    var brush = new SolidBrush(Color.Black);
    var fontSize = 28;
    var font = new Font("Microsoft Sans Serif", fontSize, FontStyle.Bold);
    SizeF size = graphics.MeasureString(score, font);
    graphics.DrawString(score, font, brush, (width - size.Width) / 2 + xPos, yPos);
}

static void RenderText(Image source, Graphics graphics, float yCoordinate, string text)
{
    var brush = new SolidBrush(Color.Black);
    var fontSize = 28;
    var font = new Font("Microsoft Sans Serif", fontSize, FontStyle.Bold);
    SizeF size;

    do {
        font = new Font("Microsoft Sans Serif", fontSize--, FontStyle.Bold);
        size = graphics.MeasureString(text, font);
    }
    while (size.Width > (source.Width - 50));

    graphics.DrawString(text, font, brush, (source.Width - size.Width) / 2, yCoordinate);
}

static string GetFullImagePath(string filename)
{
    var appPath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site", "wwwroot", "CardGenerator", "assets");
    return Path.Combine(appPath, filename);
}

static Tuple<string, string> GetNameAndTitle(string filename)
{
    string[] words = filename.Split('-');
    return Tuple.Create(words[0], words[1]);
}

public class Face
{
    public FaceRectangle FaceRectangle { get; set; }
    public Scores Scores { get; set; }
}

public class FaceRectangle
{
    public int Left { get; set; }
    public int Top { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class Scores
{
    public double Anger { get; set; }
    public double Contempt { get; set; }
    public double Disgust { get; set; }
    public double Fear { get; set; }
    public double Happiness { get; set; }
    public double Neutral { get; set; }
    public double Sadness { get; set; }
    public double Surprise { get; set; }
}

