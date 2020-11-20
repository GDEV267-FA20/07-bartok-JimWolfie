using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "cornerobject", menuName = "cornerObjects/newObject", order = 2)]
public class CornerObject : ScriptableObject
{
    public Color color{get{return sprite.color;}set{sprite.color = value; } }
    readonly public Vector3 pos;
    public SpriteRenderer sprite;

}
