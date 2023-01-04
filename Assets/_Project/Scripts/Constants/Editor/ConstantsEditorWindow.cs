using Sirenix.OdinInspector.Editor;
using Sectile;
using UnityEditor;

public class ConstantsEditorWindow : OdinEditorWindow
{
    [MenuItem("Constants/Edit Constants...")]
    private static void OpenEditor() => GetWindow<ConstantsEditorWindow>("Constants");

    public Constants constants;

    protected override object GetTarget()
    {
        return Constants.instance;
    }
}