using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FancyHeaderAttribute))]
public class FancyHeaderDrawer : DecoratorDrawer
{
    private const float UPPER_MARGIN = 17;
    private const float TITLE_HEIGHT = 15;
    private const float SUBTITLE_HEIGHT = 10;
    private const float LINE_OFFSET = 8;
    private const float LINE_OFFSET_REDUCTION = 4;

    private FancyHeaderAttribute FancyHeader { get { return (FancyHeaderAttribute) attribute; } }

    public override float GetHeight()
    {
        float height = 0f;

        if (FancyHeader.IsSubtitleSet())
            height = UPPER_MARGIN + TITLE_HEIGHT + SUBTITLE_HEIGHT;
        else
            height = UPPER_MARGIN + TITLE_HEIGHT;

        return base.GetHeight() + height;
    }

    public override void OnGUI(Rect position)
    {
        Rect lineRect;

        Rect titleRect = new Rect(position.x, position.y + UPPER_MARGIN, position.width, position.height - UPPER_MARGIN);
        EditorGUI.LabelField(titleRect, new GUIContent(FancyHeader.title), GetTitleStyle());

        if (FancyHeader.IsSubtitleSet())
        {
            Rect subTitleRect = new Rect(titleRect.x, titleRect.y + TITLE_HEIGHT, position.width, position.height - (UPPER_MARGIN + TITLE_HEIGHT));
            EditorGUI.LabelField(subTitleRect, new GUIContent(FancyHeader.subtitle), GetSubTitleStyle());
            lineRect = new Rect(subTitleRect.x, subTitleRect.y + SUBTITLE_HEIGHT + LINE_OFFSET, position.width, 1.5f);
        }
        else
            lineRect = new Rect(new Rect(titleRect.x, titleRect.y + UPPER_MARGIN + LINE_OFFSET - LINE_OFFSET_REDUCTION, position.width, 1.5f));

        DrawLine(lineRect);
    }

    private void DrawLine(Rect position)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Color oldGuiColor = GUI.color;
            GUI.color = new Color(0.5f, 0.5f, 0.5f);

            GUIStyle boxStyle = GUI.skin.box;
            boxStyle.border = new RectOffset(0, 0, 0, 0);
            boxStyle.normal.background = Texture2D.whiteTexture;
            boxStyle.Draw(position, GUIContent.none, 0);
            GUI.color = oldGuiColor;
        }
    }

    private GUIStyle GetTitleStyle()
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.fontSize = 14;
        return style;
    }

    private GUIStyle GetSubTitleStyle()
    {
        float offset = 0.18f;
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);

        if(EditorGUIUtility.isProSkin)
            style.normal.textColor = new Color(style.normal.textColor.r - offset, style.normal.textColor.g - offset, style.normal.textColor.b - offset);
        else
        {
            offset = 0.3f;
            style.normal.textColor = new Color(style.normal.textColor.r + offset, style.normal.textColor.g + offset, style.normal.textColor.b + offset);
        }

        style.fontSize = 11;
        return style;
    }
}