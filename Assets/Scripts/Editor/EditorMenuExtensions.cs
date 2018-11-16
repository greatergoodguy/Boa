// C# example.
using System;
using System.Diagnostics;
using UnityEditor;

public class EditorMenuExtensions {
    [MenuItem("Serpentes/linux-x64-server-build")]
    public static void BuildGame() {
        string[] levels = new string[] { "Assets/scenes/server.unity" };

        BuildPipeline.BuildPlayer(levels, "Builds/snakeserver", BuildTarget.StandaloneLinux64, BuildOptions.EnableHeadlessMode);
    }

    [MenuItem("Serpentes/windows-build")]
    public static void BuildWindows() {
        string[] levels = new string[] { "Assets/scenes/client.unity" };

        BuildPipeline.BuildPlayer(levels, "Builds/windows/snakeWindowsClient.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }
}
