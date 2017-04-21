using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CommentAttribute))]
public class CommentDrawer : DecoratorDrawer
{
    private const int TITLE_SIZE = 17;

    private CommentAttribute Comment { get { return (CommentAttribute) attribute; } }

    public override float GetHeight()
    {
        return base.GetHeight() + Comment.size + TITLE_SIZE;
    }

    public override void OnGUI(Rect position)
    {
        GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
        labelStyle.fontSize = 14;

        Rect helpBoxRect = new Rect(position.x, position.y + TITLE_SIZE, position.width, position.height - TITLE_SIZE);

        EditorGUI.LabelField(position, new GUIContent("Comment"), labelStyle);
        EditorGUI.HelpBox(helpBoxRect, Comment.comment, MessageType.None);
    }
}