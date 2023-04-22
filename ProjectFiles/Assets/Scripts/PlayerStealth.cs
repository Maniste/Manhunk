using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    private bool isPlayerHiddenInDarkness = false;
    public bool IsPlayerHidden { get { return isPlayerHiddenInDarkness; } }
    private float brightness = 0f;
    private float pixelBrightness = 0;

    [SerializeField] private float darknessValueToHidePlayer = 0.5f;

    private void CheckGround()
    {
        //Check if ground has shadow over itself
        if(Physics.Raycast(transform.position, -transform.up,out RaycastHit _hit, 1.1f))
        {
            //Get Light data
            Renderer _renderer = _hit.collider.GetComponent<Renderer>();

            //Return if theres no valid light map in Renderer
            if (_renderer.lightmapIndex < 0 || _renderer.lightmapIndex >= LightmapSettings.lightmaps.Length)
                return;

            LightmapData lightData = LightmapSettings.lightmaps[_renderer.lightmapIndex];
            Texture2D lightMapTex = lightData.lightmapColor;

            Vector2 hitPixel = _hit.lightmapCoord;
            Color surfaceColor = lightMapTex.GetPixelBilinear(hitPixel.x, hitPixel.y);

            brightness = (surfaceColor.r + surfaceColor.r + surfaceColor.b + surfaceColor.b + surfaceColor.g + surfaceColor.g) / 6;

            pixelBrightness = surfaceColor.r + surfaceColor.g + surfaceColor.b * 10f;

            if (pixelBrightness >= darknessValueToHidePlayer)
                isPlayerHiddenInDarkness = false;
            else if (pixelBrightness < darknessValueToHidePlayer)
                isPlayerHiddenInDarkness = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
    }

    private void OnGUI()
    {
        GUI.color = Color.magenta;
        GUI.Label(new Rect(10, 10, 250, 50), "Is player hidden in the dark?: " + isPlayerHiddenInDarkness.ToString());

        GUI.color = Color.red;
        GUI.Label(new Rect(10, 60, 100, 20), brightness.ToString());

        GUI.color = Color.green;
        GUI.Label(new Rect(10, 120, 100, 20), pixelBrightness.ToString());
    }
}
