using UnityEngine;

public class FancyHeaderAttribute : PropertyAttribute
{
    public readonly string title;
    public readonly string subtitle;

    public FancyHeaderAttribute(string title, string subtitle = "")
    {
        this.title = title;
        this.subtitle = subtitle;
    }

    public bool IsSubtitleSet()
    {
        return subtitle.Length > 0;
    }
}