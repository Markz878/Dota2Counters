using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Dota2CountersLibrary;

public static class ImageMethods
{
    private static readonly int[] partitions = new[] { 100, 275, 445, 595, 750, 1285, 1433, 1610, 1770, 1930 };
    public static Bitmap TakeScreenshotBitmap()
    {
        Bitmap bitmap = new(2100, 175);
        using Graphics g = Graphics.FromImage(bitmap);
        g.CopyFromScreen(250, 0, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
        return bitmap;
    }

    public static string[] NamePickedHeroes(Bitmap bitmap, Image<Bgr, byte> source, string avatarsLocation)
    {
        string[] heroes = new string[10];
        foreach (string avatar in Directory.EnumerateFiles(avatarsLocation).Where(x => !x.EndsWith("Name.png")))
        {
            string hero = Path.GetFileNameWithoutExtension(avatar);
            int? position = GetTemplatePosition(source, hero + ".png", avatarsLocation);
            if (position.HasValue)
            {
                DrawHeroName(bitmap, position.Value, hero);
                heroes[position.Value] = hero.Replace("_", " ");
                if (heroes.All(x => x != null))
                {
                    break;
                }
            }
        }
        return heroes;
    }

    public static void DrawHeroName(Bitmap bitmap, int position, string hero)
    {
        int x = 50;
        if (position < 5)
        {
            x += 169 * position;
        }
        else
        {
            x += 300 + 170 * position;
        }
        RectangleF rectf = new(x, 140, x + 150, 50);
        Graphics g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.DrawString(hero.Replace("_", " "), new Font("Tahoma", 10), Brushes.White, rectf);
        g.Flush();
    }

    public static int? GetPlayerPosition(Image<Bgr, byte> source, string avatarsLocation)
    {
        return GetTemplatePosition(source, "Name.png", avatarsLocation);
    }

    public static int? GetTemplatePosition(Image<Bgr, byte> source, string template, string avatarsLocation)
    {
        int? x = GetTemplateXValue(source, template, avatarsLocation);
        if (x.HasValue)
        {
            for (int i = 0; i < partitions.Length; i++)
            {
                if (x < partitions[i])
                {
                    return i;
                }
            }
        }
        return null;
    }

    public static int? GetTemplateXValue(Image<Bgr, byte> source, string template, string avatarsLocation)
    {
        using Image<Bgr, byte> templateImage = new(Path.Combine(avatarsLocation, template));
        using Image<Gray, float> result = source.MatchTemplate(templateImage, TemplateMatchingType.CcoeffNormed);
        result.MinMax(out double[] minValues, out double[] maxValues, out Point[] minLocations, out Point[] maxLocations);
        return maxValues[0] > 0.7 ? maxLocations[0].X : null;
    }
}

