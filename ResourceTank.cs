using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTank : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetTankLevel(string value)
    {
        //Parse String Value as an integer, Divide by max value, set Y scale of cylinder to needed value
        Debug.Log("Set Tank Level " + value);
    }
}
