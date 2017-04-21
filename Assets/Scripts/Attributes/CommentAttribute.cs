using UnityEngine;

public class CommentAttribute : PropertyAttribute
{
    public readonly string comment;
    public readonly float size;
    public CommentAttribute(string comment, float size = 20)
    {
        this.comment = comment;
        this.size = size;
    }
}