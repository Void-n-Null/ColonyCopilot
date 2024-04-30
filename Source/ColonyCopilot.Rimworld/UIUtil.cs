using UnityEngine;
using Verse;

namespace ColonyCopilot.Rimworld;

public class UIUtil
{
    public static void Panel(Rect rect, Color color, string label = "", GameFont font = default, TextAnchor anchor = default)
    {
        int oldFont = (int)Text.Font;
        int oldAnchor = (int)Text.Anchor;
        Widgets.DrawBoxSolid(rect, color);
        Text.Font = font == default ? Text.Font : font;
        Text.Anchor = anchor == default ? TextAnchor.UpperLeft : anchor;
        Widgets.Label(rect.ExpandedBy(-10f,0f), label);
        Text.Anchor = (TextAnchor)oldAnchor;
        Text.Font = (GameFont)oldFont;
    }
    
    public static float RelativeWidth(float width)
    {
        //Relative to 1920, so 200 pixels would be 200/1920 = 0.10416666666666667
        //We multiply by the actual screen width to get the actual width
        return (width / 1920) * Screen.width;
    }
    
    public static float RelativeHeight(float height)
    {
        //Relative to 1080, so 200 pixels would be 200/1080 = 0.18518518518518517
        //We multiply by the actual screen height to get the actual height
        return (height / 1080) * Screen.height;
    }
    
    public static bool Button(Rect rect, string label, GameFont font = default, TextAnchor anchor = default)
    {
        int oldFont = (int)Text.Font;
        int oldAnchor = (int)Text.Anchor;
        var button = Widgets.ButtonText(rect, label);
        Text.Font = font == default ? Text.Font : font;
        Text.Anchor = anchor == default ? TextAnchor.MiddleCenter : anchor;
        Text.Anchor = (TextAnchor)oldAnchor;
        Text.Font = (GameFont)oldFont;
        return button;
    }
    
    public static string TextInput(Rect rect, string text, GameFont font = default, TextAnchor anchor = default, Action<string>? onEnter = null)
    {
        int oldFont = (int)Text.Font;
        int oldAnchor = (int)Text.Anchor;
        Text.Font = font == default ? Text.Font : font;
        Text.Anchor = anchor == default ? TextAnchor.UpperLeft : anchor;
        var textFieldInput = Widgets.TextArea(rect, text);
        Text.Anchor = (TextAnchor)oldAnchor;
        var current = Event.current;
        
        //If the user presses enter, it would have added a \n to the text field.
        //This is how we detect an enter press
        bool textChanged = textFieldInput != text;
        bool shifting = current.shift;
        if (textChanged && !shifting)
        {
            bool pressedEnter = textFieldInput.EndsWith("\n");
            if (pressedEnter)
            {
                textFieldInput = textFieldInput.Substring(0, textFieldInput.Length - 1);
                onEnter?.Invoke(textFieldInput);
                textFieldInput = "";
            }
        }
        Text.Font = (GameFont)oldFont;
        return textFieldInput;
    }

}