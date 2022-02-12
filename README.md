# MissingScriptType
Find missing scripts in your components and ScriptableObjects more easily by knowing their type.

[![openupm](https://img.shields.io/npm/v/com.solidalloy.missing-script-type?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.solidalloy.missing-script-type/)

## Installation

### OpenUPM

Once you have the [OpenUPM cli](https://github.com/openupm/openupm-cli#installation), run the following command:

```
openupm install com.solidalloy.missing-script-type
```

Or if you don't have it, add the scoped registry to manifest.json with the desired dependency semantic version:

```json
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.solidalloy.util",
        "com.solidalloy.missing-script-type",
        "com.openupm"
      ]
    }
  ],
  "dependencies": {
    "com.solidalloy.missing-script-type": "1.0.0"
  },
```

### Git URL

To install the package through Package Manager, do the following:

1. In Unity, open **Window** -> **Package Manager**.
2. Press the **+** button, choose "**Add package from git URL...**"
3. Enter "https://github.com/SolidAlloy/SolidUtilities.git" and press **Add**.
4. Do the same with MissingScriptType:
   - https://github.com/SolidAlloy/MissingScriptType.git

## Quick Start

When you stumble upon a MonoBehaviour or ScriptableObject with a missing script, it will now look like this:

<img src=".images/missing-script-ui.png" alt="missing-script-ui" style="zoom:67%;" />

You will see the type name of the script that had been assigned to this object before it went missing. Note that it will only for the objects where the script had been lost before the package was installed. If the package is installed after the object missed its script, the type field will be empty.

The plugin also searches for a script that contains this type. If the script is found, it will appear on the next line, and you will be able to restore the object in one button press:

<img src=".images/missing-script-button.png" alt="missing-script-button" style="zoom:67%;" />

## Customization

You can customize how the type name is shown in **Project Settings/Packages/Missing Script Type**:

![settings](.images/settings.png)

**Short** - TestBehaviour

**Full** (default) - DefaultNamespace.TestBehaviour

**Full And Assembly** - DefaultNamespace.TestBehaviour, Assembly-CSharp

## Integrations With Other Plugins

The plugin works with **Odin Inspector**. When the object script is loaded, you will see the normal Odin interface. When the script is missing, you will be able to see its type.

There is also integration with [GenericUnityObjects](https://github.com/SolidAlloy/GenericUnityObjects):

<img src=".images/generic-unity-objects.png" alt="generic-unity-objects" style="zoom:67%;" />

## Using In Custom Inspectors

The plugin works through the custom editor called `MissingScriptTypeEditor` which is enabled for all MonoBehaviours and ScriptableObjects that don't have their own custom editor.

If you have a custom editor in which you want to implement the "Last Known Type" feature, you can inherit from `MissingScriptTypeEditor`:

```csharp
[CustomEditor(typeof(MyClass))]
public class MyCustomEditor : MissingScriptTypeEditor
{
    protected override OnEnable()
    {
        base.OnEnable();
        // your custom enable code
    }
    
    public override void OnInspectorGUI()
    {
        if (_missingScriptUtility.IsScriptLoaded())
        {
            // your custom inspector UI
        }
        else
        {
            _missingScriptUtility.Draw();
        }
    }
}
```

Or use `MissingScriptTypeUtility`:

```csharp
[CustomEditor(typeof(MyClass))]
public class MyCustomEditor : Editor
{
    private MissingScriptTypeUtility _missingScriptUtility;
    
    protected override OnEnable()
    {
        _missingScriptUtility = new MissingScriptTypeUtility(serializedObject);
    }
    
    public override void OnInspectorGUI()
    {
        if (_missingScriptUtility.IsScriptLoaded())
        {
            // your custom inspector UI
        }
        else
        {
            _missingScriptUtility.Draw();
        }
    }
}
```

If you have another custom editor that you want to use on general MonoBehaviours and ScriptableObjects, you can disable `MissingScriptTypeEditor` by adding `DISABLE_MISSING_SCRIPT_EDITOR` to **Player/Scripting Define Symbols**.
