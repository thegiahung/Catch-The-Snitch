using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
//using System.Numerics;

public class PredictionProvider : MonoBehaviour
{
    public int SAMPLE_SIZE = 50;
    public float SAMPLE_INTERVAL = 0.01F;
    public float PREDICION_TIME = 0.05f;

    //private int deltaActualPridicOffset;
    //private Queue<Vector3> deltaPredicQ;

    private PredictionModel predictionModel;
    //public HandPredictionModelSimple predictionModel;

    public GameObject trackingOrigin;

    public Vector3 initPosition;

    private List<String> recordedPositionsString;
    private string positionsLogPath;

    private List<String> predicAnalysis;
    private string predicAnalysisLogPath;

    //public List<String> deltaAnalysis;
    //public string deltaAnalysisLogPath;

    public Vector3 predictedPotision { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Time.fixedDeltaTime = SAMPLE_INTERVAL; // call fixedUpdate every xxx ms

        //predictionModel = new HandPredictionModelSimple(SAMPLE_INTERVAL, PREDICION_TIME); // sampling - prediction
        predictionModel = new PredictionModel(SAMPLE_SIZE, SAMPLE_INTERVAL, PREDICION_TIME);

        initPosition = new Vector3();

        string fname1 = "positionLog_" + SAMPLE_INTERVAL + "_" + PREDICION_TIME + "_" + System.DateTime.Now.ToString("dd-MMM_HH-mm-ss") + ".csv";
        positionsLogPath = Path.Combine(Application.persistentDataPath, fname1);

        string fname2 = "predicLog_" + SAMPLE_INTERVAL + "_" + PREDICION_TIME + "_" + System.DateTime.Now.ToString("dd-MMM HH-mm-ss") + ".csv";
        predicAnalysisLogPath = Path.Combine(Application.persistentDataPath, fname2);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = predictedPotision;
    }

    private void FixedUpdate()
    {
        //Debug.Log("Called Fixed Update");

        //Stopwatch stopwatch = Stopwatch.StartNew();

        //Vector3 initPosition = GetControllerPosition();
        Vector3 initPosition = GetTrackingOriginPosition();

        predictionModel.RecordPosition(initPosition);
        predictedPotision = initPosition + predictionModel.CalculateDisplacement();

        //deltaPredicQ.Enqueue(predictedPotision);

        ////UnityEngine.Debug.Log("predictedPotision:" + predictedPotision.ToString());

        //if (deltaPredicQ.Count >= deltaActualPridicOffset)
        //{
        //    float deltaPredicActual = Vector3.Distance(initPosition, deltaPredicQ.Dequeue());
        //    WriteToCSV(predicAnalysisLogPath, predicAnalysis, deltaPredicActual);
        //}

        /*
          write initPosition to csv for prediction validation
        /*/

        //WriteToCSV(initPosition);
        //WriteToCSV(predicAnalysisLogPath, predicAnalysis, predictedPotision);

        //stopwatch.Stop();
        //UnityEngine.Debug.Log($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        //UnityEngine.Debug.Log($"Elapsed time (high precision): {stopwatch.Elapsed.TotalMilliseconds} ms");


    }

    //public System.Numerics.Vector3 GetControllerVelocity()
    //{
    //    System.Numerics.Vector3 localVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
    //    return new System.Numerics.Vector3(trackingSpace.transform.TransformVector(localVelocity));
    //}

    public Vector3 GetControllerPosition()
    {
        Vector3 localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        //Vector3 filtered = new Vector3((float)Math.Round(localPosition.x, 2),
        //    (float)Math.Round(localPosition.y, 2),
        //    (float)Math.Round(localPosition.z, 2));
        //UnityEngine.Debug.Log(localPosition);
        //UnityEngine.Debug.Log(localPosition.x);
        //UnityEngine.Debug.Log(localPosition.y);
        //UnityEngine.Debug.Log(localPosition.z);
        //System.Numerics.Vector3 temp = new System.Numerics.Vector3(localPosition.x, localPosition.y, localPosition.z);
        //UnityEngine.Debug.Log(temp);
        //UnityEngine.Debug.Log("-----------------");
        return localPosition;
    }

    public Vector3 GetTrackingOriginPosition()
    {
        return trackingOrigin.transform.position;
    }

    //public void WriteToCSV(Vector3 position)
    //{
    //    recordedPositionsString.Add(position.x.ToString() + "," +
    //                                position.y.ToString() + "," +
    //                                position.z.ToString());
    //    if (recordedPositionsString.Count > 40)
    //    {
    //        File.AppendAllLines(positionsLogPath, recordedPositionsString);
    //        recordedPositionsString.Clear();
    //    }
    //}

    //public void WriteToCSV(String filePath, List<String> tempList, Vector3 position)
    //{
    //    tempList.Add(position.x.ToString() + "," +
    //                position.y.ToString() + "," +
    //                position.z.ToString()); // store in memery first to for faster process
    //    if (tempList.Count > 40) // batch write
    //    {
    //        File.AppendAllLines(filePath, tempList);
    //        tempList.Clear();
    //    }
    //}

    //public void WriteToCSV(String filePath, List<String> tempList, float position)
    //{
    //    tempList.Add(position.ToString()); // store in memery first to for faster process
    //    if (tempList.Count > 40) // batch write
    //    {
    //        File.AppendAllLines(filePath, tempList);
    //        tempList.Clear();
    //    }
    //}
}
