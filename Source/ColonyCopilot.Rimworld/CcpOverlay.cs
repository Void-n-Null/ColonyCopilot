using UnityEngine;
using Verse;

namespace ColonyCopilot.Rimworld;

/// <summary>
/// The Colony Copilot Overlay.
/// This class is responsible for rendering the UI elements of the mod.
/// </summary>

[StaticConstructorOnStartup]
public class CcpOverlay
{
    static CcpOverlay()
    {
        SpinnerImage = ContentFinder<Texture2D>.Get("Spinner");
    }
    
    //Universal Screen Width and Height
    private static int ScreenWidth => UI.screenWidth;
    private static int ScreenHeight => UI.screenHeight;
    
    //Spinner
    private static readonly Texture2D SpinnerImage;
    private static float _spinnerRotation;
    public const float SpinnerSpeed = 180f;
    public static float SpinnerSize => UIUtil.RelativeWidth(66f);
    
    //Show UI
    private static bool _showUI = true;
    
    //Text Input
    private static float TextInputWidth => UIUtil.RelativeWidth(600);
    private static float TextInputHeight => UIUtil.RelativeHeight(45);
    private static float TextInputX => (ScreenWidth - TextInputWidth) / 2f;
    private static float TextInputY => (ScreenHeight - TextInputHeight) / 2f + ScreenHeight / 3f;
    private static string _textInputValue = "";
    
    //UI Toggle Button
    public static float UIToggleButtonWidth => UIUtil.RelativeWidth(200);
    
    //Speech Panel
    private static float SpeechPanelWidth => UIUtil.RelativeWidth(600);
    private static float SpeechPanelHeight => UIUtil.RelativeHeight(200);
    private static float SpeechPanelX => (ScreenWidth - SpeechPanelWidth) / 2f;
    private static float SpeechPanelY => (ScreenHeight - SpeechPanelHeight) / 2f + ScreenHeight / 5f;
    private static bool Speaking => SpeechGenerator.Speaking;
    
    /// <summary>
    /// Renders the Colony Copilot UI Overlay.
    /// This includes the text input, send button, speech panel, and spinner.
    /// Optionally, the UI can be hidden via a toggle button.
    /// </summary>
    public static void Render()
    {
        if (_showUI)
        {
            RenderTextInput();
            RenderSendButton();
            RenderSpeechPanel();
            RenderSpinner();
        }
        RenderToggleButton();
    }
    
    
    /// <summary>
    /// The button to toggle the UI on and off.
    /// </summary>
    private static void RenderToggleButton()
    {
        //Has 2 states, show or hide.
        //Position, Size, and Label change based on this state.
        string translatedLabel = "HideColonyCopilotUI".Translate();
        var toggleLabel = _showUI ?  translatedLabel : "\u27a5";
        var width = _showUI ? UIToggleButtonWidth : 50;
        var xPosition = _showUI ? TextInputX - UIToggleButtonWidth - 10 : 0;
        var yPosition = _showUI ? TextInputY : ScreenHeight - ScreenHeight / 4f;
        var rect = new Rect(xPosition, yPosition, width, TextInputHeight);
        var button = Widgets.ButtonText(rect, $"{toggleLabel}");
        if (button)
        {
            _showUI = !_showUI;
        }
    }

    
    /// <summary>
    /// Renders the text input field.
    /// Most of the complexity is handled in UIUtil.TextInput()
    /// </summary>
    public static void RenderTextInput()
    {
        var rect = new Rect(TextInputX, TextInputY, TextInputWidth, TextInputHeight);
        _textInputValue = UIUtil.TextInput(rect, _textInputValue, GameFont.Medium, TextAnchor.UpperLeft, CcpGameManager.CallAgent);
    }
    
    
    /// <summary>
    /// Renders the speech panel.
    /// This is where the AI generated text is displayed.
    /// </summary>
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

    /// <summary>
    /// Renders the send button.
    /// This button sends the text in the text input field to the AI agent.
    /// The text input field is then cleared.
    /// </summary>
    public static void RenderSendButton()
    {
        var rect = new Rect(TextInputX + TextInputWidth + 10, TextInputY, TextInputHeight, TextInputHeight);
        var button = UIUtil.Button(rect, "\u27a4", GameFont.Medium, TextAnchor.MiddleCenter);
        if (button)
        {
            CcpGameManager.CallAgent(_textInputValue);
            _textInputValue = "";
        }
    }

    /// <summary>
    /// Renders the spinner.
    /// This spinner is displayed when the AI agent is processing a request.
    /// The spinner rotates at a constant speed;
    /// </summary>
    private static void RenderSpinner()
    {
        var running = false;

        try
        {
            running = CcpGameManager.Instance.Agent.IsRunning;
        } catch (Exception e)
        {
            Log.Error("Error getting agent running status: " + e);
            return;
        }
        //Only render the spinner if the agent is running
        if (running)
        {
            _spinnerRotation += SpinnerSpeed * Time.deltaTime;
            _spinnerRotation %= 360;
            var rect = new Rect(SpeechPanelX + SpeechPanelWidth / 2 - (SpinnerSize / 2), SpeechPanelY + SpeechPanelHeight / 2 - (SpinnerSize / 2), SpinnerSize, SpinnerSize);
            Widgets.DrawTextureFitted(rect, SpinnerImage, 1f, new Vector2(1, 1), new Rect(0, 0, 1, 1), _spinnerRotation);
        } 
    }
}