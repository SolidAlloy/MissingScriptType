namespace MissingScriptType.Editor
{
    using System;
    using SolidUtilities;
    using UnityEditor;
    using UnityEngine;

    public class MissingScriptTypeUtility
    {
        private readonly SerializedProperty _scriptProperty;
        private readonly SerializedProperty _editorClassIdProperty;

        private GUIStyle _wrappedText;
        private GUIStyle WrappedText => _wrappedText ??= new GUIStyle(EditorStyles.textField) { wordWrap = true };
        
        public MissingScriptTypeUtility(SerializedObject serializedObject)
        {
            _scriptProperty = serializedObject.FindProperty("m_Script");
            _editorClassIdProperty = serializedObject.FindProperty("m_EditorClassIdentifier");

            if (_scriptProperty.objectReferenceValue == null)
                return;

            var type = serializedObject.targetObject.GetType();

            if (type.FullName.StartsWith("GenericUnityObjects.ConcreteClasses"))
                type = type.BaseType;

            _editorClassIdProperty.stringValue = GetTypeName(type);
            serializedObject.ApplyModifiedProperties(); 
        }

        public void OnInspectorGUI()
        {
            if (_scriptProperty.objectReferenceValue != null) 
                return;
            
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.LabelField("Last Known Type");
                EditorGUILayout.TextArea(_editorClassIdProperty.stringValue, WrappedText);
            }
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType || type.IsGenericTypeDefinition)
                return TypeHelper.GetNiceNameOfGenericType(type, true);

            return type.FullName;
        }
    }
}