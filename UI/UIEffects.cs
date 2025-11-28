using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEffects : MonoBehaviour
{
    public static UIEffects Instance;
    void Awake()
    {
        Instance = this;
    }
    public void Bump(string ComponentToBump, float bumpMultiplier = 1.25f)
    {
        Image component = SearchComponent(ComponentToBump.ToLower());
        Bumping(component, bumpMultiplier);
    }

    private IEnumerator Bumping(Image comp, float multiplier)
    {
        Vector3 baseScale = comp.rectTransform.localScale;
        comp.rectTransform.localScale *= multiplier;
        while (comp.rectTransform.localScale != baseScale)
        {
            Vector3.Lerp(comp.rectTransform.localScale, baseScale, 0.5f);
            yield return null;
        }
    }

    public Image SearchComponent(string name)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.ToLower() == name)
            {
                return transform.GetChild(i).GetComponent<Image>();
            }
        }
        return null;
    }
}
