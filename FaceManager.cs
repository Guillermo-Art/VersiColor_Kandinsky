using UnityEngine;
using System.Collections;
using Windows.Kinect;
using Microsoft.Kinect.Face;
using UnityEngine.UI;

public class FaceManager : MonoBehaviour
{

    public string face_result = "";

    private KinectSensor kinectSensor = null;
    private BodyFrameReader bodyFrameReader = null;
    private Body[] bodies = null;

    private int bodyCount;
    private FaceFrameSource[] faceFrameSources = null;

    private FaceFrameReader[] faceFrameReaders = null;

    private FaceFrameResult[] faceFrameResults = null;
    public string isHappy;

    void Start()
    {
        this.kinectSensor = KinectSensor.GetDefault();

        this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

        this.bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;

        this.bodyCount = this.kinectSensor.BodyFrameSource.BodyCount;

        this.bodies = new Body[this.bodyCount];

        FaceFrameFeatures faceFrameFeatures = FaceFrameFeatures.FaceEngagement| FaceFrameFeatures.Happy;

        this.faceFrameSources = new FaceFrameSource[this.bodyCount];
        this.faceFrameReaders = new FaceFrameReader[this.bodyCount];
        for (int i = 0; i < this.bodyCount; i++)
        {
            this.faceFrameSources[i] = FaceFrameSource.Create(this.kinectSensor, 0, faceFrameFeatures);

            this.faceFrameReaders[i] = this.faceFrameSources[i].OpenReader();
        }

        this.faceFrameResults = new FaceFrameResult[this.bodyCount];

        this.kinectSensor.Open();

        for (int i = 0; i < this.bodyCount; i++)
        {
            if (this.faceFrameReaders[i] != null)
            {
                this.faceFrameReaders[i].FrameArrived += FaceManager_FrameArrived;
            }
        }

        if (this.bodyFrameReader != null)
        {
            this.bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;
        }
    }

    private void FaceManager_FrameArrived(object sender, FaceFrameArrivedEventArgs e)
    {
        using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
        {
            if (faceFrame != null)
            {
                int index = this.GetFaceSourceIndex(faceFrame.FaceFrameSource);

                if (faceFrame.IsTrackingIdValid)
                {
                    this.faceFrameResults[index] = faceFrame.FaceFrameResult;
                }
               
            }
        }
    }

    private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        using (var bodyFrame = e.FrameReference.AcquireFrame())
        {
            if (bodyFrame != null)
            {
               
                bodyFrame.GetAndRefreshBodyData(this.bodies);

                   
                    for (int i = 0; i < this.bodyCount; i++)
                    {
                        
                        if (this.faceFrameSources[i].IsTrackingIdValid)
                        {
                          
                            if (this.faceFrameResults[i] != null)
                            {
                                if (this.faceFrameResults[i].FaceProperties != null) {
                                   
                                    foreach (var item in this.faceFrameResults[i].FaceProperties)
                                    {

                                        if (item.Key == FaceProperty.Happy || item.Key == FaceProperty.Engaged)
                                        {

                                            if (item.Value == DetectionResult.Maybe)
                                            {
                                                isHappy = "Engaged";
                                            }
                                            else
                                            {
                                                if (item.Value == DetectionResult.Yes)
                                                {
                                                    isHappy = item.Key.ToString();
                                                }
                                            }
                                        }
                                    }
                                }      
                            }
                        }
                        else
                        {
                        
                            if (this.bodies[i].IsTracked)
                            {
                               
                                this.faceFrameSources[i].TrackingId = this.bodies[i].TrackingId;
                            }
                        }
                    }  
            }
        }
    }

    private int GetFaceSourceIndex(FaceFrameSource faceFrameSource)
    {
        int index = -1;

        for (int i = 0; i < this.bodyCount; i++)
        {
            if (this.faceFrameSources[i] == faceFrameSource)
            {
                index = i;
                break;
            }
        }

        return index;
    }
}
    
