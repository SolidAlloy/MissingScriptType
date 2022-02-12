namespace MissingScriptType.Editor
{
    using System.Collections.Generic;
    using System.Reflection;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;

    public static class SettingsDrawer
    {
        private const string TypeRepresentationLabel = "Type Representation";

        private static readonly GUIContent _typeRepresentationContent = new GUIContent(TypeRepresentationLabel, "How to show the type name when the script is missing.");

        private static readonly MethodInfo _repaintAllMethod;

        static SettingsDrawer()
        {
            var inspectorType = typeof(SceneView).Assembly.GetType("UnityEditor.InspectorWindow");
            _repaintAllMethod = inspectorType.GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.NonPublic);
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Packages/Missing Script Type", SettingsScope.Project)
            {
                guiHandler = OnGUI,
                keywords = GetKeywords()
            };
        }

        private static void OnGUI(string searchContext)
        {
            var newValue = (TypeRepresentation) EditorGUILayout.EnumPopup(_typeRepresentationContent, Settings.TypeRepresentation, GUILayout.MaxWidth(300f));

            if (Settings.TypeRepresentation == newValue)
                return;

            Settings.TypeRepresentation = newValue;
            RepaintAllInspectors();
        }

        private static void RepaintAllInspectors() => _repaintAllMethod?.Invoke(null, null);

        private static HashSet<string> GetKeywords()
        {
            var keywords = new HashSet<string>();
            keywords.AddWords(TypeRepresentationLabel);
            return keywords;
        }

        private static readonly char[] _separators = { ' ' };

        private static void AddWords(this HashSet<string> set, string phrase)
        {
            foreach (string word in phrase.Split(_separators))
            {
                set.Add(word);
            }
        }
    }
}