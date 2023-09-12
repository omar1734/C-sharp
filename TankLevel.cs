using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TankLevel : PLC
{

    private float fullHeightScale;
    Vector3 scale = new Vector3();

    public bool scaleFromParent = false;

    public Material highFull;

    public Material mediumFull;

    public Material lowFull;

    MeshRenderer mr;

    Vector3 targetScale = new Vector3();

    public UnityEngine.Events.UnityEvent onTankFull;

    public bool IsTankFull;

    [Range(0,1)]
    public float explodeChance = .25f;


    [Range(0,1)]
    public float fractionToTest;

    public int maximumLevel = 250000;

    public void Awake()
    {
        if (scaleFromParent)
        {
            fullHeightScale = transform.parent.localScale.y;
            scale = transform.parent.localScale - transform.parent.localScale.y * transform.parent.up;
        }
        else
        {
            fullHeightScale = transform.localScale.y;
            scale = transform.localScale - transform.localScale.y * transform.up;
        }
        

        mr = GetComponent<MeshRenderer>();

        targetScale = scale;
    }

    bool lerping = false;

    public override void SetState(string state)
    {
        base.SetState(state);

        try
        {
            double amount = System.Convert.ToDouble(state);
            float fraction = Mathf.Clamp((float)System.Convert.ToInt32(state) / (float)maximumLevel, 0, 1);

            SetAmountFull(fraction);

            IsTankFull = fraction >= 1;

            /*
            if (text)
            {
                text.text = amount + " / " + maximumLevel + " L";
            }
            */
        }
        catch
        {
            Debug.LogError("[TankLevel] Couldn't convert " + state + " to int.");
        }
    }



    public void SetAmountFull(float fractionFull)
    {

        // Clamp fraction full because if there is a lid you don't want it to dissappear.
        targetScale.y = Mathf.Clamp(fractionFull, .01f, 1) * fullHeightScale;

        if (!lerping)
        {
            StartCoroutine(LerpTankLevel());
        }

    }

    public IEnumerator LerpTankLevel()
    {
        lerping = true;

        if (scaleFromParent)
        {
            while (Vector3.Magnitude(transform.parent.localScale - targetScale) > .1f)
            {
                transform.parent.localScale = Vector3.Lerp(transform.parent.localScale, targetScale, .05f);

                float fractionFull = transform.parent.localScale.y / fullHeightScale;

                if (fractionFull > .90)
                {
                    if (fractionFull == 1)
                        onTankFull.Invoke();

                    if (mr.material != highFull)
                        mr.material = highFull;
                }
                else if (fractionFull < .50)
                {
                    if (mr.material != lowFull)
                        mr.material = lowFull;
                }
                else
                {
                    if (mr.material != mediumFull)
                        mr.material = mediumFull;
                }
                yield return null;
            }
        }
        else
        {
            while (Vector3.Magnitude(transform.localScale - targetScale) > .1f)
            {

                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, .05f);

                float fractionFull = transform.localScale.y / fullHeightScale;

                if (fractionFull > .90)
                {
                    if (fractionFull == 1)
                        onTankFull.Invoke();

                    if (mr.material != highFull)
                        mr.material = highFull;
                }
                else if (fractionFull < .50)
                {
                    if (mr.material != lowFull)
                        mr.material = lowFull;
                }
                else
                {
                    if (mr.material != mediumFull)
                        mr.material = mediumFull;
                }
                yield return null;
            }
        }

        lerping = false;
    }
}