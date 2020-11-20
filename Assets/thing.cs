using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thing : MonoBehaviour
{
    public Color color { get { return sprite.color; } set { sprite.color = value; } }
    public Vector3 pos;
    public SpriteRenderer sprite;

    private void Awake()
    {
        pos = gameObject.transform.position;
    }
}
