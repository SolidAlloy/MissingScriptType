namespace MissingScriptType.Editor
{
    using UnityEditor;
    using UnityEngine;

#if ! DISABLE_MISSING_SCRIPT_EDITOR
    [CustomEditor(typeof(MonoBehaviour), true)]
#endif
    public class MonoBehaviourEditor : MissingScriptTypeEditor { }
}