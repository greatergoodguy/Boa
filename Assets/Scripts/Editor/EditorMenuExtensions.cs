// C# example.
using System;
using System.Diagnostics;
using UnityEditor;

public class EditorMenuExtensions {
    [MenuItem("Serpentes/linux-x64-server-build")]
    public static void BuildGame() {
        // Get filename.
        // string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { "Assets/scenes/server.unity" };

        BuildPipeline.BuildPlayer(levels, "Builds/snakeserver", BuildTarget.StandaloneLinux64, BuildOptions.EnableHeadlessMode);

        // Copy a file from the project folder to the build folder, alongside the built game.
        // FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");
    }
}
