using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TravelState
{
    toDrawpile, drawpile, toHand, hand, toTarget, target, discard, to, idle
}
public class PassedObject : MonoBehaviour
{
    public bool runningAlgo = true;
    public Color currentColor;
    public Material mat;
    public TravelState state = TravelState.to;
    public Vector3 destination;
    //references to the corners. changing the color changes the sprite's rendered values. that code lives
    //on the object for health reasons
    public ScriptableObject tpR;
    public ScriptableObject tpL;
    public ScriptableObject btR;
    public ScriptableObject btL;

    public Color topRightColor{get {return (tpR.color); } set{ tpR.color = value;}} 
    public Color topLeftColor { get { return (tpL.color); } set { tpL.color = value; } } 
    public Color bottomRightColor { get { return (btR.color); } set { btR.color = value; } } 
    public Color bottomLeftColor { get { return (btL.color); } set { btL.color = value; } }
    public List<Vector4> AllColors;
    public Vector3 topRightPos { get { return (tpR.pos); } }//read only
    public Vector3 topLeftPos { get { return (tpL.pos); }}
    public Vector3 bottomRightPos { get { return (btR.pos); } }
    public Vector3 bottomLeftPos { get { return (btL.pos); } }
    public List<Vector3> AllPositions;

    public List<Vector3> bezierPts;
    public List<Quaternion> bezierRots;
    public float timeStart, timeDuration;
    static public float MOVE_DURATION = 0.5f;
    static public string MOVE_EASING = Easing.InOut;
    public Dictionary<Vector3, ScriptableObject> keyValues;

    public GameObject reportFinishTo = null;
   
    private void Awake()
    {
        mat.color = currentColor;
        topLeftColor = Color.yellow;
        topRightColor = Color.red;
        bottomLeftColor = Color.green;
        bottomRightColor = Color.blue;
        //set them to established colors because we might exit play mode early.
        AllPositions.Add(topLeftPos);
        AllPositions.Add(topRightPos);
        AllPositions.Add(bottomLeftPos);
        AllPositions.Add(bottomRightPos);
        Dictionary<Vector3, ScriptableObject> keyValues = new Dictionary<Vector3, ScriptableObject>(){
            {topLeftPos,tpL },{topRightPos, tpR },{bottomRightPos,btR },{bottomLeftPos, btL }
        };
    }
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
    private void Start()
    {
        Invoke("StartMovement", 3f);
    }
    private void Update()
    {
        if(AllcolorsMatch())
        {
            DisplayEndResultText();
        }

        switch(state)
        {
            case TravelState.toHand:
            case TravelState.toTarget:
            case TravelState.toDrawpile:
            case TravelState.to:
            float u = (Time.time - timeStart)/timeDuration;
            float uC = Easing.Ease(u, MOVE_EASING);
            if(u<0)
            {
                transform.localPosition = bezierPts[0];
                transform.rotation = bezierRots[0];
                return;
            } else if(u>=1)
            {
                uC = 1;
                
                if(state == TravelState.to)
                    state=TravelState.idle;
                transform.localPosition = bezierPts[bezierPts.Count-1];
                transform.localRotation = bezierRots[bezierRots.Count-1];
                timeStart =0;
                //use ContainsKey() to check for an unknown key
                if(keyValues.ContainsKey(destination))
                {
                   var obj= keyValues[destination];
                   obj.color=currentColor;
                }
                MoveTo(RandomCorner());

            } else
            {
                Vector3 pos = Utils.Bezier(uC, bezierPts);
                transform.localPosition = pos;
                Quaternion rotQ = Utils.Bezier(uC, bezierRots);
                transform.rotation= rotQ;
                if(u>0.5f)
                {
                    RandomColor();
                }
            }
            break;
        }
    }
    void DisplayEndResultText()
    {

    }
    public Vector3 RandomCorner()
    {
        int goTo = Random.Range(0, 4);
        return(AllPositions[goTo]);
    }
    public void RandomColor()
    {
        int goTo = Random.Range(0, 4);
        var thing = AllColors[goTo];
        currentColor = thing;
        mat.color = thing;

        
    }

    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }

    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition);
        bezierPts.Add(ePos);

        bezierRots = new List<Quaternion>();
        bezierRots.Add(transform.rotation);
        bezierRots.Add(eRot);

        if(timeStart == 0)
        {
            timeStart = Time.time;
        }
        timeDuration = MOVE_DURATION;
        state = TravelState.to;

    }
    public void StartMovement()
    {
        MoveTo(RandomCorner());
    }
}
