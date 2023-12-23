//foreach (var item in Directory.EnumerateFiles("../../../../Dota2Counters/wwwroot/Avatars"))
//{
//	if (item.Contains(' '))
//	{
//		File.Move(item, item.Replace(" ", "_"));
//	}
//}
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using System.Text.Json;

Dictionary<string, string> base64Avatars = new();
Dictionary<string, string> base64AvatarsLarge = new();
IImageEncoder pngEncoder = new PngEncoder();
foreach (string item in Directory.EnumerateFiles("Avatars"))
{
    string hero = item[(item.IndexOf('\\') + 1)..^4];
    using MemoryStream outputStream = new();
    byte[] bytes = File.ReadAllBytes(Path.Combine(item));
    using (Image image = Image.Load(bytes))
    {
        image.Mutate(x => x.Resize(40, 0));
        image.Save(outputStream, pngEncoder);
    }
    string base64 = Convert.ToBase64String(outputStream.ToArray());
    base64Avatars.Add(hero.Replace("_", " "), $"data:image/png;base64,{base64}");
    base64AvatarsLarge.Add(hero.Replace("_", " "), $"data:image/png;base64,{Convert.ToBase64String(bytes)}");
}
File.WriteAllText("Avatars.txt", JsonSerializer.Serialize(base64Avatars));
File.WriteAllText("AvatarsLarge.txt", JsonSerializer.Serialize(base64AvatarsLarge));