using UnityEngine;
using System;
using System.Collections;
using ColonyCopilot.OpenAI;
using UnityEngine.Networking;

namespace ColonyCopilot.Rimworld;

[RequireComponent(typeof(AudioSource))]
public class SpeechGenerator : MonoBehaviour
{
    private Client _client = null!;
    public static bool Speaking { get; private set; }
    public static string? CurrentSpeakingText { get; private set; }
    public static SpeechGenerator Instance { get; private set; } = null!;

    public static SpeechGenerator Create(Client client)
    {
        var speechGenerator = new GameObject("SpeechGenerator").AddComponent<SpeechGenerator>();
        speechGenerator._client = client;
        Instance = speechGenerator;
        return speechGenerator;
    }
    
    public async Task Speak(string text, Voice voice)
    {
        AudioClip clip;
        try {
            clip = await GenerateAudioClip(text, voice);
        } catch (Exception e)
        {
            CLog.Error("Error speaking: " + e);
            throw;
        }

        if (clip == null) return;
        try
        {
            await PlayAudio(clip, text);

        } catch (Exception e)
        {
            CLog.Error("Error playing audio: " + e);
            throw;
        }
    }
    
    private AudioSource _audioSource = null!;

    public AudioSource AudioSource
    {
        get
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            return _audioSource;
        }
        set => _audioSource = value;
    }

    public async Task<AudioClip> GenerateAudioClip(string text, Voice voice)
    {
        //Make sure text is valid by converting it from bytes to unicode, and then removing any invalid characters
        text = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, System.Text.Encoding.Unicode.GetBytes(text)));
        text = System.Text.RegularExpressions.Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);
        var data = new Dictionary<string, string>
        {
            {"model", "tts-1"},
            {"input", text},
            {"voice", voice.ToString().ToLower()}
        };
        //TODO: What is the best way to create a json string from a dictionary in C# Without using external libraries?
        string body = "{";
        foreach (var pair in data)
        {
            body += $"\"{pair.Key}\": \"{pair.Value}\",";
        }
        body = body.TrimEnd(',');
        body += "}";
        using (UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/audio/speech", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            var uuid = Guid.NewGuid().ToString();
            request.downloadHandler = new DownloadHandlerAudioClip($"{uuid}.mp3", AudioType.MPEG);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _client.ApiKey);

            request.SendWebRequest();

            while (!request.isDone)
            {
                await Task.Delay(100);
            }
            await Task.Delay(100);

            if (request.isHttpError || request.isNetworkError)
            {
                throw new Exception("Audio download failed: " + request.error);
            }

            try
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                return audioClip;
            }
            catch (Exception)
            {
                CLog.Error("Response from Open AI was not audio: " + request.downloadHandler.text);
                throw;
            }

        }
    }
    
    public async Task PlayAudio(AudioClip audioClip, string associatedText)
    {
        CurrentSpeakingText = associatedText;
        Speaking = true;
        AudioSource.clip = audioClip;
        AudioSource.Play();
        await Task.Delay((int) (audioClip.length * 1000));
        Speaking = false;
        CurrentSpeakingText = null;
    }
}