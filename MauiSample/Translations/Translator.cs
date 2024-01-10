using System.Resources;

namespace MauiSample.Translations;
public static class Translator
{
    public static readonly ResourceManager Translations = new ResourceManager(typeof(Text));

    public static string Translate(string key) =>
        Translations.GetString(key, Thread.CurrentThread.CurrentCulture) ?? $"{{{key}}}";
}
