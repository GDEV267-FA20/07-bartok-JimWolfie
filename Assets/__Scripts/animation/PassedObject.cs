using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PassedObject : MonoBehaviour
{
    public bool runningAlgo = true;
    public Color currentColor;
    public ScriptableObject tpR;
    public ScriptableObject tpL;
    public ScriptableObject btR;
    public ScriptableObject btL;

    public Color topRightColor{get {return (tpR.color); } set{ tpR.color = value;}} 
    public Color topLeftColor { get { return (tpL.color); } set { tpL.color = value; } } 
    public Color bottomRightColor { get { return (btR.color); } set { btR.color = value; } } 
    public Color bottomLeftColor { get { return (btL.color); } set { btL.color = value; } } 
    public List<Vector4> AllColors;
    public bool AllcolorsMatch()
    {
        if(isSameColor(topLeftColor, topRightColor) || isSameColor(topLeftColor, bottomLeftColor)||isSameColor(topLeftColor, bottomRightColor) == false)
            {
            return(false);
        }
        if(isSameColor(topRightColor, bottomRightColor)||isSameColor(topRightColor, bottomLeftColor) == false)
        {
            return(false);
        }
        if(isSameColor(bottomRightColor, bottomLeftColor)== false)
        {
            return(false);
        }
        return (false); 
    }
    public bool isSameColor(Vector4 colorA, Vector4 colorB)
    {
        
        return (colorA.Equals(colorB));
    }

    private void Update()
    {
        if(AllcolorsMatch())
        {
            //end the thing and stop updating
        } 
    }
}
