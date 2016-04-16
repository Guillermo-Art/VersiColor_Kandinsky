using UnityEngine;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using UnityEngine.UI;

public class BodySourceView : MonoBehaviour 
{
    /// <summary>
    /// GameObjects
    /// </summary>
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    public GameObject pincel;
    public GameObject face_result;

    /// <summary>
    /// Kinect variables
    /// </summary>
    private Kinect.Joint handRight;
    private Kinect.Joint thumbRight;
    private Kinect.Joint handLeft;
    private Kinect.Joint thumbLeft;

    /// <summary>
    /// Nodos
    /// </summary>
    public GameObject node1, node2; //objeto a isntanciar

    private Color colLine, colIni, colEnd;

    /// <summary>
    /// Joint Dictionary
    /// </summary>
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    public string flag = "Open";

    private IList<Kinect.Body> data = null;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    void Start()
    {
        //comienza neutral
        colLine = new Color(42 / 255.0f, 44 / 255.0f, 41 / 255.0f);
        colIni = new Color(199 / 255.0f, 30 / 255.0f, 49 / 255.0f);
        colEnd = new Color(132 / 255.0f, 71 / 255.0f, 87 / 255.0f);
        setColors();
    }

    void Update () 
    {
        string fr = face_result.GetComponent<FaceManager>().isHappy;
   
        if (fr.Equals("Happy"))
        {
            colLine = new Color(252 / 255.0f, 211 / 255.0f, 0 / 255.0f);
            colIni = new Color(250 / 255.0f, 100 / 255.0f, 0 / 255.0f);
            colEnd = new Color(157 / 255.0f, 226 / 255.0f, 221 / 255.0f);
        }
        else {
            colLine = new Color(42 / 255.0f, 44 / 255.0f, 41 / 255.0f);
            colIni = new Color(199 / 255.0f, 30 / 255.0f, 49 / 255.0f);
            colEnd = new Color(132 / 255.0f, 71 / 255.0f, 87 / 255.0f); 
        }

        setColors();

        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();

        if (_BodyManager == null)
        {
            return;
        }
        
        data = _BodyManager.GetData();

        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();

        foreach(var body in data)
        {
            if (body == null)
            {
                return;
           }
           
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);

                switch (body.HandRightState) {

                   case Kinect.HandState.Open: {
                       if (flag.Equals("Close")) {
                                flag = "Open";
                                Vector3 pos = pincel.transform.position;
                                Instantiate(node2, pos, Quaternion.identity);
                            }
                        
                        break;
                   }
                   case Windows.Kinect.HandState.Closed:{
                            if (flag.Equals("Open")) {
                                flag = "Close";
                                Vector3 pos = pincel.transform.position;
                                Instantiate(node1, pos, Quaternion.identity);
                            }
                       
                        break;
                   }
                }
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                return;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.2f, 0.2f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            if (sourceJoint.JointType == Kinect.JointType.HandRight) {
                pincel.transform.position = jointObj.position; 

                
            }
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.white;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    void setColors()
    {
        node1.GetComponent<SpriteRenderer>().color = colIni;
        node2.GetComponent<SpriteRenderer>().color = colEnd;
        node1.GetComponent<LineRenderer>().SetColors(colLine, colLine);
    }

    private Kinect.Body getDistance(IList<Kinect.Body> body) {

        foreach (Kinect.Body item in body) {
            if (item.Joints[Windows.Kinect.JointType.SpineBase].Position.Z < 10000)
            {
                return item;
            }
        }

        return null;       
    }
}
