using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetToPNGConverter : EditorWindow
{
    [MenuItem("Custom/Convert Assets to PNG")]
    static void ConvertAssetsToPNG()
    {
        // Get selected objects
        Object[] selectedObjects = Selection.objects;

        // Check if any objects are selected
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogError("Please select one or more Texture2D assets.");
            return;
        }

        foreach (Object selectedObject in selectedObjects)
        {
            // Check if the selected object is a Texture2D asset
            if (selectedObject is Texture2D)
            {
                Texture2D texture = (Texture2D)selectedObject;

                // Get the destination path and filename for the PNG file
                //string filePath = EditorUtility.SaveFilePanel("Save PNG", "", texture.name, "png");

                //if (string.IsNullOrEmpty(filePath))
                //{
                //    // User canceled the save operation
                //    return;
                //}

                string filePath = AssetDatabase.GetAssetPath(selectedObject).Replace(".asset", ".png");

                // Encode the Texture2D to PNG and save the file
                byte[] pngBytes = texture.EncodeToPNG();
                System.IO.File.WriteAllBytes(filePath, pngBytes);

                Debug.Log("Conversion complete. PNG file saved at: " + filePath);
            }
            else
            {
                Debug.LogWarning("Selected object is not a Texture2D asset: " + selectedObject.name);
            }
        }
    }
}
