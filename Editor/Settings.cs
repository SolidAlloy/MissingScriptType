namespace MissingScriptType.Editor
{
    using UnityEditor.SettingsManagement;
    using UnitySettings = UnityEditor.SettingsManagement.Settings;

    public enum TypeRepresentation { Short, Full, FullAndAssembly }

    public static class Settings
    {
        private const string PackageName = "com.solidalloy.missing-script-type";

        private static UnitySettings _instance;

        private static UserSetting<TypeRepresentation> _typeRepresentation;

        public static TypeRepresentation TypeRepresentation
        {
            get
            {
                InitializeIfNeeded();
                return _typeRepresentation.value;
            }

            set => _typeRepresentation.value = value;
        }

        private static void InitializeIfNeeded()
        {
            if (_instance != null)
                return;

            _instance = new UnitySettings(PackageName);

            _typeRepresentation = new UserSetting<TypeRepresentation>(_instance, nameof(_typeRepresentation), TypeRepresentation.Full);
        }
    }
}