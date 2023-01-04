using System;
using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    public string ObjectName;
    public SpriteRenderer SpriteRenderer;

    protected virtual void OnMouseDown()
    {
        Debug.Log("object clicked: " + gameObject.name);
    }
}
