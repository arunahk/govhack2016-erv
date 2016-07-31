using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Use this for initialization
    void Start()
    {
        keywords.Add("Soup Spoon", () =>
        {
            this.BroadcastMessage("ShowScene", 0);
        });

        keywords.Add("Roar", () =>
        {
            this.BroadcastMessage("ShowScene", 1);
        });

        keywords.Add("Terror Lizard", () =>
        {
            this.BroadcastMessage("ShowScene", 1);
        });
        keywords.Add("Play Dead", () =>
        {
            this.BroadcastMessage("PlayDead", 1);
        });

        keywords.Add("Align Surface", () =>
        {
            this.BroadcastMessage("ToggleAlignment", true);

        });

        keywords.Add("F P S", () =>
        {
            this.BroadcastMessage("ToggleFPS", 1);
        });


        keywords.Add("Zoom In", () =>
        {
            // Call the OnReset method on every descendant object.
            this.BroadcastMessage("ZoomIn");
        });

        keywords.Add("Zoom Out", () =>
        {
            this.BroadcastMessage("ZoomOut");
            /*
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the OnDrop method on just the focused object.
                focusObject.SendMessage("OnDrop");
            }
            */
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}