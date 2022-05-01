namespace MissingScriptType.Editor
{
    using System;
    using SolidUtilities;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;

    public class MissingScriptTypeUtility
    {
        private readonly SerializedObject _serializedObject;
        private readonly SerializedProperty _scriptProperty;
        private readonly SerializedProperty _editorClassIdProperty;

        private bool _triedSearchingType;
        private Type _foundType;
        private MonoScript _foundMonoScript;

        private GUIStyle _wrappedText;
        private GUIStyle WrappedText => _wrappedText ??= new GUIStyle(EditorStyles.textField) { wordWrap = true };
        
        public MissingScriptTypeUtility(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;

            if (_serializedObject == null)
                return;
            
            _scriptProperty = _serializedObject.FindProperty("m_Script");
            _editorClassIdProperty = _serializedObject.FindProperty("m_EditorClassIdentifier");

            if (_scriptProperty.objectReferenceValue == null || _serializedObject.targetObject is null)
                return;

            var type = _serializedObject.targetObject.GetType();

            if (type.FullName.StartsWith("GenericUnityObjects.ConcreteClasses"))
                type = type.BaseType;

            _editorClassIdProperty.stringValue = GetTypeName(type);
            serializedObject.ApplyModifiedProperties(); 
        }

        public bool IsScriptLoaded()
        {
            if (_scriptProperty == null)
                return true;

#pragma warning disable RCS1172
            return _scriptProperty.objectReferenceValue as MonoScript != null;
#pragma warning restore RCS1172
        }

        public void Draw()
        {
            if (_serializedObject == null)
                return;
            
            _serializedObject.Update();
            bool previousGUIEnabled = GUI.enabled;
            GUI.enabled = true;

            EditorGUILayout.PropertyField(_scriptProperty);

            ShowScriptNotLoadedWarning();

            if (!_triedSearchingType)
            {
                TrySearchScript(_editorClassIdProperty.stringValue);
                _triedSearchingType = true;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Last Known Type", GUILayout.Width(110f));
            
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.TextArea(GetTypeRepresentation(_editorClassIdProperty.stringValue), WrappedText);
                }                
            }
            
            DrawFoundScript();
            
            if (_serializedObject.ApplyModifiedProperties())
                EditorHelper.ForceRebuildInspectors();

            GUI.enabled = previousGUIEnabled;
        }

        private void DrawFoundScript()
        {
            if (_foundMonoScript == null)
                return;

            using var _ = new EditorGUILayout.HorizontalScope();
            
            EditorGUILayout.LabelField("Found Matching Script", GUILayout.Width(145f));

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField(_foundMonoScript, typeof(MonoScript), false);

            if (_foundType.IsGenericTypeDefinition)
                return;
            
            if (GUILayout.Button("Set", GUILayout.Width(30f)))
                _scriptProperty.objectReferenceValue = _foundMonoScript;
        }
        
        private static void ShowScriptNotLoadedWarning()
        {
            var text = L10n.Tr(
                "The associated script can not be loaded.\nPlease fix any compile errors\nand assign a valid script.");
            EditorGUILayout.HelpBox(text, MessageType.Warning, true);
        }

        private void TrySearchScript(string assemblyQualifiedTypeName)
        {
            _foundType = GetType(assemblyQualifiedTypeName);

            if (_foundType != null)
                _foundMonoScript = AssetHelper.GetMonoScriptFromType(_foundType);
        }

        private static Type GetType(string assemblyQualifiedTypeName)
        {
            // Transform generic type into generic type definition.
            if (assemblyQualifiedTypeName.Contains("<"))
            {
                string genericArgsPart = assemblyQualifiedTypeName.GetSubstringAfter('<').GetSubstringBefore('>');
                int argsCount = genericArgsPart.CountChars(',') + 1;
                assemblyQualifiedTypeName = assemblyQualifiedTypeName.Replace($"<{genericArgsPart}>", $"`{argsCount}");
            }

            return Type.GetType(assemblyQualifiedTypeName);
        }

        private static string GetTypeRepresentation(string assemblyQualifiedName)
        {
            if (Settings.TypeRepresentation == TypeRepresentation.FullAndAssembly)
                return assemblyQualifiedName;

            string fullTypeName = assemblyQualifiedName.GetSubstringBeforeLast(',');

            if (Settings.TypeRepresentation == TypeRepresentation.Full)
                return fullTypeName;

            return fullTypeName.GetSubstringAfterLast('.');
        }

        private static string GetTypeName(Type type)
        {
            string typeName = (type.IsGenericType || type.IsGenericTypeDefinition)
                ? TypeHelper.GetNiceNameOfGenericType(type, true)
                : type.FullName;
            
            return $"{typeName}, {type.Assembly.GetName().Name}";
        }
    }
}