using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class SocialShare : Singleton<SocialShare>
{
    [SerializeField] string _title;
    [SerializeField][Multiline] string _text;
    [SerializeField] Texture2D[] _images;

    public void Share()
    {
        Share(null);
    }

    private IEnumerator ShareEntireScreenshot()
    {
        yield return new WaitForEndOfFrame();

        Share(ScreenCapture.CaptureScreenshotAsTexture());
    }

    public IEnumerator ShareRegionOfScreenshot(Rect region)
    {
        yield return new WaitForEndOfFrame();

        Texture2D ScreenShot = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D RegionScreenshot = CutTexture(ScreenShot, region);
        Share(RegionScreenshot);
    }

    public Texture2D CutTexture(Texture2D source, Rect rect)
    {
        Texture2D newTexture = new((int)rect.width, (int)rect.height);
        Color[] pixels = source.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return newTexture;
    }

    public IEnumerator ShareRenderTextureScreenshot(RenderTexture renderTexture, Rect region)
    {
        yield return new WaitForEndOfFrame();

        Texture2D ScreenShot = ConvertRenderTextureToTexture2D(renderTexture);
        Texture2D RegionScreenshot = CutTexture(ScreenShot, region);
        Share(RegionScreenshot);
    }

    private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture = new(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;

        texture.ReadPixels(new(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        return texture;
    }

    void Share(Texture2D screenshot)
    {
        var ns = new NativeShare();

        if (string.IsNullOrEmpty(_title))
        {
            ns.SetTitle(_text);
            ns.SetSubject(_text);
        }
        else
        {
            ns.SetTitle(_title);
            ns.SetSubject(_title);
        }

        ns.SetText(_text);

        if (screenshot != null)
            ns.AddFile(GetFilePath(screenshot));

        foreach (var img in _images)
        {
            ns.AddFile(GetFilePath(img));
        }

        ns.Share();

        //SaveScreenshot(screenshot);
    }

    private string GetFilePath(Texture2D texture)
    {
        var filePath = Path.Combine(Application.temporaryCachePath, $"{Guid.NewGuid()}.png");
        File.WriteAllBytes(filePath, texture.EncodeToPNG());

        return filePath;
    }

    private void SaveScreenshot(Texture2D screenshot)
    {
        byte[] bytes = screenshot.EncodeToPNG();
        var dirPath = Application.persistentDataPath;
        if (Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "/Photo_" + UnityEngine.Random.Range(0, 100000) + ".png", bytes);
    }
}