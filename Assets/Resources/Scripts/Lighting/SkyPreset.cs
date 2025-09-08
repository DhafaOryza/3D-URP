using UnityEngine;

[CreateAssetMenu(fileName = "SkyPreset", menuName = "Time Of Day/Sky Preset", order = 0)]
public class SkyPreset : ScriptableObject
{
    [Header("Skybox")]
    public Texture2D skyboxTexture;

    [Header("Lighting")]
    public Color lightColor = Color.white;
}
