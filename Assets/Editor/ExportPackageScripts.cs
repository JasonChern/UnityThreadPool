using System.IO;
using com.FunJimChee.CommonTool;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ExportPackageScripts : EditorWindow
    {
        private static string _projectScriptPath;

        private static string _packageScriptPath;
        
        [MenuItem("Export/CustomPackageScripts")]
        private static void ShowWindow()
        {
            var window = GetWindow<ExportPackageScripts>();
            window.titleContent = new GUIContent("ExportPackageScripts");
            window.Show();

            _projectScriptPath = Path.Combine(Application.dataPath, "Runtime");

            var packagePath = new DirectoryInfo(Application.dataPath).Parent.FullName;

            _packageScriptPath = Path.Combine(packagePath, "Runtime");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Export"))
            {
                if (Directory.Exists(_packageScriptPath))
                {
                    Directory.Delete(_packageScriptPath, true);
                }

                DirectoryHelper.CopyFolder(_projectScriptPath, _packageScriptPath);
            }
        }
    }
}