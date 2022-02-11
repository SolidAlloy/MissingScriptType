namespace MissingScriptType.Editor
{
    using System.Reflection;
    using UnityEditor.Callbacks;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector.Editor;
#endif
    
    public class MissingScriptTypeEditor :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        private MissingScriptTypeUtility _missingScriptUtility;
        
#if ODIN_INSPECTOR && ! DISABLE_MISSING_SCRIPT_EDITOR
        [DidReloadScripts(0)]
        private static void OnScriptsReload()
        {
            // a workaround for bug https://bitbucket.org/sirenix/odin-inspector/issues/833/customeditor-with-editorforchildclasses
            var odinEditorTypeField = typeof(InspectorTypeDrawingConfig).GetField("odinEditorType", BindingFlags.Static | BindingFlags.NonPublic);
            odinEditorTypeField.SetValue(null, typeof(MissingScriptTypeEditor));
        }
#endif

        protected
#if ODIN_INSPECTOR
            override
#else
            virtual
#endif
            void OnEnable()
        {
#if ODIN_INSPECTOR
            base.OnEnable();
#endif
            _missingScriptUtility = new MissingScriptTypeUtility(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            Debug.Log("missing script type");
            base.OnInspectorGUI();
            _missingScriptUtility.OnInspectorGUI();
        }
    }
}