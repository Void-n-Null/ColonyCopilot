using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonyCopilot.Rimworld;



[HarmonyPatch(typeof (UIRoot_Play), "UIRootOnGUI")]
public class UIRoot_Play_UIRootOnGUI_Patch
{
    private static int Width = 200;
    private const int Height = 140;
    private static Texture2D? _copilotImage = null;
    private static bool _showUI = true;
    
    public static Texture2D GetCopilotTexture()
    {
        if (_copilotImage == null)
        {
            _copilotImage = ContentFinder<Texture2D>.Get("Copilot");
        }
        return _copilotImage;
        
    }
    
    private static int ScreenWidth => UI.screenWidth;
    private static int ScreenHeight => UI.screenHeight;
    private static string _textInput = "";
    public static void Prefix()
    {
        if (_showUI)
        {
            RenderCopilot();
            RenderTextInput();
            RenderSendButton();
            RenderSpeechPanel();
        }
        RenderHideUIButton();
    }

    private static void RenderHideUIButton()
    {
        var rect = new Rect(TextInputX - TextInputWidth - 150, TextInputY, 150, TextInputLineHeight);
        var toggleLabel = _showUI ? "Hide" : "Show";
        var button = Widgets.ButtonText(rect, $"{toggleLabel} Copilot UI");
        if (button)
        {
            _showUI = !_showUI;
        }
    }

    private const float TextInputWidth = 700;
    private const float TextInputLineHeight = 45;
    private static float TextInputX => (ScreenWidth - TextInputWidth) / 2f;
    private static float TextInputY => (ScreenHeight - TextInputLineHeight) / 2f + ScreenHeight / 3f;
    public static void RenderTextInput()
    {
        var rect = new Rect(TextInputX, TextInputY, TextInputWidth, TextInputLineHeight);
        _textInput = UIUtil.TextInput(rect, _textInput, GameFont.Medium, TextAnchor.UpperLeft, CcpGameManager.InitializeAgentTalk);
    }
    
    
    private const float SpeechPanelWidth = 600;
    private const float SpeechPanelHeight = 200;
    private static float SpeechPanelX => (ScreenWidth - SpeechPanelWidth) / 2f;
    private static float SpeechPanelY => (ScreenHeight - SpeechPanelHeight) / 2f + ScreenHeight / 5f;
    private static bool Speaking => SpeechGenerator.Speaking;
    public static void RenderSpeechPanel()
    {
        if (Speaking)
        {
            if (SpeechGenerator.CurrentSpeakingText == null) return;
            string text = SpeechGenerator.CurrentSpeakingText;
            //SpeechGenerator.CurrentSpeakingText is what should be in the panel
            var rect = new Rect(SpeechPanelX, SpeechPanelY, SpeechPanelWidth, SpeechPanelHeight);
            UIUtil.Panel(rect, new Color(0.1f, 0.1f, 0.1f, 0.8f), text, GameFont.Medium);
        }
    }

    public static void RenderSendButton()
    {
        //Place the send button on the right side of the text input
        var rect = new Rect(TextInputX + TextInputWidth + 10, TextInputY, TextInputLineHeight, TextInputLineHeight);
        var button = UIUtil.Button(rect, "\u27a4", new Color(0f, 0f, 0f, 0.8f), GameFont.Medium, TextAnchor.MiddleCenter);
        if (button)
        {
            CcpGameManager.InitializeAgentTalk(_textInput);
            _textInput = "";
        }
    }

    public static void RenderCopilot()
    {
        if (!CcpGameManager.Instance.Agent.IsRunning) return;
        var copilotRect = new Rect(0, ScreenHeight - (ScreenHeight / 2.2f), ScreenWidth /4f, ScreenWidth /4f);
        Widgets.DrawTextureFitted(copilotRect, GetCopilotTexture(), 1f);
    }
}