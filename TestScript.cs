using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject parentObject;
    private void Update()
    {
        if (gameObject.transform.parent == null)
        {
            gameObject.transform.SetParent(parentObject.transform);
        }
    }
}
