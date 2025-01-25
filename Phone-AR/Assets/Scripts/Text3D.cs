using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Text3D : MonoBehaviour
{
    [Serializable]
    public class Character3D
    {
        public char Character;
        public GameObject Prefab;
    }

    public float SingleLetterWidth = 0.08f;
    public float LineHeight = 0.1f;

    public Character3D[] Characters;

    public string Text;
    private string _curText;
    private List<GameObject> _letters = new List<GameObject>();

    private void UpdateText(string text)
    {
        // Clear existing text
        Text = _curText = text;
        _letters.ForEach(Destroy);
        _letters.Clear();

        // Get bounds of the collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("No collider attached to the GameObject for text fitting.");
            return;
        }

        var bounds = collider.bounds;
        float maxWidth = bounds.size.x / transform.lossyScale.x;
        float maxHeight = bounds.size.y / transform.lossyScale.y;

        // Prepare character dictionary
        var chars = Characters.ToDictionary(e => e.Character, e => e.Prefab);

        // Wrap text to fit the width
        var wrappedLines = WrapText(Text, maxWidth / SingleLetterWidth);

        // Calculate starting Y position
        float totalHeight = wrappedLines.Count * LineHeight;
        if (totalHeight > maxHeight)
        {
            Debug.LogError("Text is too tall to fit within the collider.");
            return;
        }

        float startY = 0 + totalHeight / 2f - LineHeight / 2f;

        // Generate letters for each line
        for (int lineIndex = 0; lineIndex < wrappedLines.Count; lineIndex++)
        {
            string line = wrappedLines[lineIndex];
            float startX = 0 - (line.Length * SingleLetterWidth) / 2f;

            for (int charIndex = 0; charIndex < line.Length; charIndex++)
            {
                char character = line[charIndex];
                if (chars.ContainsKey(character))
                {
                    var isUnderLine = "pqjgqy".Contains(character);
                    var gameObj = Instantiate(chars[character], transform);
                    gameObj.transform.localPosition = new Vector3(
                        startX + charIndex * SingleLetterWidth,
                        startY - lineIndex * LineHeight - (isUnderLine ? LineHeight / 3f : 0f),
                        0f
                    );
                    gameObj.transform.localRotation = Quaternion.LookRotation(-Vector3.forward, Vector3.up);
                    gameObj.name = character.ToString();
                    _letters.Add(gameObj);
                }
                else if (character != ' ')
                {
                    Debug.LogError($"Character {character} not supported");
                }
            }
        }
    }

    private List<string> WrapText(string text, float maxCharsPerLine)
    {
        var lines = new List<string>();
        var words = text.Split(' ');

        string currentLine = string.Empty;

        foreach (var word in words)
        {
            if ((currentLine.Length + word.Length) <= maxCharsPerLine)
            {
                currentLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
            }
            else
            {
                lines.Add(currentLine);
                currentLine = word;
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine);
        }

        return lines;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!String.Equals(Text, _curText))
        {
            UpdateText(Text);
        }
    }
}
