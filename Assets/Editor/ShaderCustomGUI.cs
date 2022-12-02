using UnityEditor;

public class ShaderCustomGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // Custom code that controls the appearance of the Inspector goes here

        base.OnGUI(materialEditor, properties);
    }
}