using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

//This just automatically sets the baked light to read/write
//because im too lazy to do it myself


[InitializeOnLoad]
public static class LightBakingHelperEditor
{
    static LightBakingHelperEditor()
    {
        Lightmapping.bakeCompleted += GetAndSetFileToReadWrite;
        Debug.Log("Lazy light bake thing intialized");
    }

    static void GetAndSetFileToReadWrite()
    {
        Scene targetScene = SceneManager.GetActiveScene();

        string nameOfFile = targetScene.name + ".unity";
        string partToRemoveFromPath = "Assets/";
        string pathToFiles = targetScene.path.Substring(0, targetScene.path.Length - 6);

        int startIndex = 7;
        int length = targetScene.path.Length - nameOfFile.Length - partToRemoveFromPath.Length - 1;

        string pathWithoutAssetFolder = targetScene.path.Substring(startIndex, length);
        string folderPath = Application.dataPath + "/" + pathWithoutAssetFolder + "/" + targetScene.name;
        Debug.Log(folderPath);
        string[] files = Directory.GetFiles(folderPath, "*.exr");
        
        foreach(string file in files)
        {
            string fullPath = file;
            if (File.Exists(fullPath))
            {

                string newPath = fullPath.Substring(31, fullPath.Length - 31);
                var import = AssetImporter.GetAtPath(newPath) as TextureImporter;


                if (import != null)
                {
                    import.isReadable = true;
                    import.SaveAndReimport();
                }
                else
                {
                    Debug.Log("texture importer at:  " + newPath + " is null");
                }
            }
            else
                Debug.Log("File doesnt exist at: " + fullPath);
        }
        Debug.Log("LightBake Done");
    }

}
