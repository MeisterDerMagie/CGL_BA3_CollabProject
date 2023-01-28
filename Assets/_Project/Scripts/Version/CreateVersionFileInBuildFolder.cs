//(c) copyright by Martin M. Klöckener
#if UNITY_EDITOR
using System.IO;
using PlanetaGameLabo.UnityGitVersion;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class CreateVersionFileInBuildFolder : IPostprocessBuildWithReport
{
    public int callbackOrder { get; }
    public void OnPostprocessBuild(BuildReport report)
    {
        //create version file next to the playable .exe
        if (report.summary.platform is BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64)
        {
            string buildPath = report.summary.outputPath;
            string versionFilePath = buildPath.Replace(PlayerSettings.productName + ".exe", "version.txt");
            string version = GitVersion.version.versionString;

            Debug.Log($"Write version file. version: {version}; path: {versionFilePath}");
        
            File.WriteAllText(versionFilePath, version);
            
            return;
        }

        Debug.Log($"Could not auto create version file. Reason: unsupported build target ({report.summary.platform})");
    }
}
#endif