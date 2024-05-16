using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HighLightManager : MonoBehaviour
{
    protected Dictionary<Renderer, Material[]> glowMaterialDict = new Dictionary<Renderer, Material[]>();
    protected Dictionary<Renderer, Material[]> originalMaterialDict = new Dictionary<Renderer, Material[]>();
    protected bool isGlowing = false;
    [SerializeField] protected Material glowMaterial;
    [SerializeField] [ColorUsage(true, true)] protected Color glowColor = Color.green;
    [SerializeField] protected float glowPower;

    void Awake() {
        PrepareMaterialDictionaries();
        glowMaterial.SetColor("_HighLightColor", glowColor);
        glowMaterial.SetFloat("_HighlightStrength", glowPower);
    }

    protected virtual void PrepareMaterialDictionaries() {
        Dictionary<Color, Material> cachedGlowMaterials = new Dictionary<Color, Material>();
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            Material[] originalMaterial = renderer.materials;
            originalMaterialDict.Add(renderer, originalMaterial);
            Material[] newMaterials = new Material[originalMaterial.Length];
            for (int i = 0; i < originalMaterial.Length; i++) {
                Material mat = null;
                if (!cachedGlowMaterials.TryGetValue(originalMaterial[i].color, out mat)) {
                    mat = new Material(glowMaterial);
                    mat.SetTexture("_Texture", originalMaterial[i].mainTexture);
                    mat.SetColor("_Color", originalMaterial[i].color);
                }
                newMaterials[i] = mat;
            }
            glowMaterialDict.Add(renderer, newMaterials);
        }
    }

    public virtual void ToggleGlow() {
        isGlowing = !isGlowing;
        if (isGlowing) {
            SetMaterials(glowMaterialDict);
        } else {
            foreach (Renderer renderer in originalMaterialDict.Keys) {
                SetMaterials(originalMaterialDict);
            }
        }
    }

    public void SetGlow(bool state) {
        if (isGlowing == state) return;
        ToggleGlow();
    }

    protected void SetMaterials(Dictionary<Renderer, Material[]> materialDict) {
        foreach (Renderer renderer in originalMaterialDict.Keys) {
            renderer.materials = materialDict[renderer];
        }
    }
}
