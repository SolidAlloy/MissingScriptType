namespace MissingScriptType.Editor
{
    using UnityEditor;
    using UnityEngine;

#if ! DISABLE_MISSING_SCRIPT_EDITOR
    [CustomEditor(typeof(ScriptableObject), true)]
#endif
    public class ScriptableObjectEditor : MissingScriptTypeEditor { }
}