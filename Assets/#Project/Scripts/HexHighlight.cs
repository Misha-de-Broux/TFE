using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexHighlight : HighLightManager
{
    Dictionary<Renderer, Material[]> fogMaterialDict = new Dictionary<Renderer, Material[]>();
    [SerializeField][ColorUsage(true, true)] Color pathColor = Color.cyan;
    public Material fogMaterial;
    private bool hidden = false;

    protected override void PrepareMaterialDictionaries() {
        base.PrepareMaterialDictionaries();
        Dictionary<Color, Material> cachedFogMaterials = new Dictionary<Color, Material>();
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            Material[] originalMaterial = renderer.materials;
            Material[] newMaterials = new Material[originalMaterial.Length];
            for (int i = 0; i < originalMaterial.Length; i++) {
                Material mat = null;
                if (!cachedFogMaterials.TryGetValue(originalMaterial[i].color, out mat)) {
                    mat = new Material(fogMaterial);
                    if (originalMaterial[i].GetTexturePropertyNameIDs().Length > 0) {
                        mat.SetTexture("_MainTex", originalMaterial[i].mainTexture);
                    }
                    mat.SetColor("_Color", originalMaterial[i].color);
                }
                newMaterials[i] = mat;
            }
            fogMaterialDict.Add(renderer, newMaterials);
        }
    }
    public override void ToggleGlow() {
        if (hidden)
        {
            isGlowing = !isGlowing;
        } else {
             base.ToggleGlow();
            ResetGlowHightLight();
        }
    }

    internal void ResetGlowHightLight() {
        if (isGlowing) {
            foreach (Renderer renderer in glowMaterialDict.Keys) {
                foreach (Material mat in glowMaterialDict[renderer]) {
                    mat.SetColor("_HighLightColor", glowColor);
                    glowMaterial.SetFloat("_HighlightStrength", glowPower);
                }
            }
        }
    }

    internal void HighLightPath() {
        if (isGlowing) {
            foreach (Renderer renderer in glowMaterialDict.Keys) {
                foreach (Material mat in glowMaterialDict[renderer]) {
                    mat.SetColor("_HighLightColor", pathColor);
                    glowMaterial.SetFloat("_HighlightStrength", glowPower*1.5f);
                }
            }
        }
    }

    internal void HighLightColor(Color color) {
        SetGlow(true);
        foreach (Renderer renderer in glowMaterialDict.Keys) {
            foreach (Material mat in glowMaterialDict[renderer]) {
                mat.SetColor("_HighLightColor", color);
                glowMaterial.SetFloat("_HighlightStrength", glowPower);
            }
        }
    }

    internal void Hide(bool hidden) {
        this.hidden = hidden;
        if (hidden) { 
            SetMaterials(fogMaterialDict);
        } else if (isGlowing) {
            SetMaterials(glowMaterialDict);
        } else {
            SetMaterials(originalMaterialDict);
        }
    }
}
