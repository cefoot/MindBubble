using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Text3D))]
public class Text3dEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Text3D text3D = (Text3D)target;

        if (GUILayout.Button("Auto-Assign Characters"))
        {
            AutoAssignCharacters(text3D);
        }
    }

    private void AutoAssignCharacters(Text3D text3D)
    {
        string assetsFolderPath = "Assets/Models/Text/"; // Replace with your folder path

        // Get all .GLB files in the folder
        string[] glbFiles = Directory.GetFiles(assetsFolderPath, "*.GLB", SearchOption.AllDirectories);

        foreach (string glbFile in glbFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(glbFile);

            // Parse the character from the file name
            char character = fileName[0]; // First character in the file name

            // Load the prefab from the asset database
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(glbFile);

            if (prefab != null)
            {
                // Check if the character is already in the Characters array
                var existingCharacter = System.Array.Find(text3D.Characters, c => c.Character == character);

                if (existingCharacter != null)
                {
                    existingCharacter.Prefab = prefab;
                }
                else
                {
                    // Add a new entry
                    var newCharacter = new Text3D.Character3D
                    {
                        Character = character,
                        Prefab = prefab
                    };

                    var characterList = new System.Collections.Generic.List<Text3D.Character3D>(text3D.Characters);
                    characterList.Add(newCharacter);
                    text3D.Characters = characterList.ToArray();
                }
            }
        }

        EditorUtility.SetDirty(text3D); // Mark the object as dirty to save changes
        Debug.Log("Characters auto-assigned!");
    }
}
