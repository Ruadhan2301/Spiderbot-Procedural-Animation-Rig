using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Author - Rowan Goswell
/// Date - 02/02/2021
/// </summary>
public class MultiLegWalkerCode : MonoBehaviour
{
    [Header("Parts")]
    public Transform[] LegRoots;
    public string HipJointName = "HipJoint";
    public string KneeJointName = "KneeJoint";
    public string AnkleJointName = "AnkleJoint";

    [Header("Gait Pattern")]
    public string walkPattern = "0,1,2,3";
    public float GaitOverlap = 1f;


    [Header("Stance")]
    public float stance = 1.65f;
    public float gaitSpread = 0.5f;
    public float gaitLength = 2f;


    [Header("Motion Control")]
    public float WalkSpeed = 1f;
    public float TurnSpeed = 1f;
    public Vector3 MoveDirection = Vector3.zero;
    public Vector3 FaceDirection = Vector3.zero;


    private List<LegComponents> Legs = new List<LegComponents>();
    private float radianMultiplier = 57.2958f;

    private float WalkCycle = 0f;

    private float WalkCycleLength = 1f;

    private List<List<int>> gaitdisposition = new List<List<int>>();

    private Vector3 hipOffset = Vector3.zero;

    void Start()
    {
        Vector3 offsetCumulative = Vector3.zero;
        foreach(Transform leg in LegRoots)
        {
            LegComponents lComp = EvaluateLeg(leg);
            offsetCumulative += lComp.target;
        }
        offsetCumulative /= Legs.Count;
        hipOffset = transform.InverseTransformPoint(offsetCumulative);

        ConstructGait();

        WalkCycleLength = 1 + ((gaitdisposition.Count - 1) * (1f-GaitOverlap));
    }

    private void ConstructGait()
    {
        gaitdisposition = new List<List<int>>();
        List<string> walkParts = walkPattern.Split(',').ToList(); // split into blocks;
        int partCount = walkParts.Count;
        for (int i = 0; i < partCount; i++)
        {
            string gaitSegment = walkParts[i];
            List<char> gaitParts = gaitSegment.ToCharArray().ToList();
            List<int> gaitIDs = new List<int>();
            int gaitPartCount = gaitParts.Count;
            for (int j = 0; j < gaitPartCount; j++)
            {
                gaitIDs.Add(int.Parse("" + gaitParts[j]));
            }
            gaitdisposition.Add(gaitIDs);
        }
    }
    // Update is called once per frame
    void Update()
    {

        Vector3 TargetDirection = Vector3.Slerp(transform.forward, FaceDirection, Time.deltaTime*TurnSpeed);
        transform.rotation = Quaternion.LookRotation(TargetDirection, Vector3.up);

        transform.position = GetAvgLegPosition() + Vector3.up * stance + transform.forward * hipOffset.z *-1f;
        
        Vector3 offset = MoveDirection.normalized * gaitLength;
        
        WalkCycle += Time.deltaTime * WalkSpeed;
        if(WalkCycle > WalkCycleLength)
        {
            WalkCycle = 0f;
        }
        int legCount = Legs.Count;
        for(int i = 0; i < legCount; i++)
        {
            LegComponents leg = Legs[i];
            if (leg.stepPercentage < 1f)
            {
                leg.stepPercentage += Time.deltaTime * WalkSpeed;
                float stepDistance = (leg.origin - leg.destination).magnitude;
                float clampedDistance = Mathf.Min(stepDistance, 1f);
                float stepLift = (float)System.Math.Sin(leg.stepPercentage * 3f) * clampedDistance;
                
                leg.target = Vector3.Lerp(leg.origin, leg.destination, leg.stepPercentage) + Vector3.up * Mathf.Min(stepLift,0.75f) * 0.5f;
            }
        }

        int gaitDispositionCount = gaitdisposition.Count;
        for (int i = 0; i < gaitDispositionCount; i++)
        {
            List<int> gaitItems = new List<int>();

            if(WalkCycle > i - (i * GaitOverlap) && WalkCycle <= (i + 1) - (i * GaitOverlap))
            
            {
                List<int> gaitLegs = gaitdisposition[i];
                int gaitLegsCount = gaitLegs.Count;
                for (int j = 0; j < gaitLegsCount; j++)
                {
                    int id = gaitLegs[j];
                    LegComponents leg = Legs[id];
                    if (leg.stepPercentage >= 1f)
                    {
                        RaycastHit hit = GetFootPositionHit(transform.TransformPoint(leg.Offset + leg.Root.localPosition) + offset + Vector3.up * 10f, Vector3.up);
                        leg.origin = leg.destination;
                        leg.destination = hit.point;

                        leg.stepPercentage = 0f;
                    }
                }
            }

        }


        foreach (LegComponents leg in Legs)
        {
            OperateLeg(leg);
        }
    }



    private LegComponents EvaluateLeg(Transform HipRoot)
    {
        LegComponents newLeg = new LegComponents();
        newLeg.Root = HipRoot;
        
        Transform hipPitch = HipRoot.Find(HipJointName);
        Transform thigh = hipPitch.Find("Thigh");
        Transform knee = hipPitch.Find(KneeJointName);
        Transform shin = knee.Find("Shin");
        Transform ankle = knee.Find(AnkleJointName);
        Transform foot = ankle.Find("Foot");
        newLeg.HipPitch = hipPitch;
        newLeg.Thigh = thigh;
        newLeg.Knee = knee;
        newLeg.Shin = shin;
        newLeg.Ankle = ankle;
        newLeg.Foot = foot;

        float thighLength = thigh.localScale.z;
        float shinLength = shin.localScale.z;
        float footLength = foot.localScale.z;

        newLeg.ThighLength = thighLength;
        newLeg.ShinLength = shinLength;
        newLeg.FootLength = footLength;

        float cumulativeLength = thighLength + shinLength;

        Vector3 HipOffset = HipRoot.TransformPoint(new Vector3(0f, 1f, gaitSpread));
        newLeg.origin = HipOffset;
        newLeg.target = HipOffset;
        newLeg.stepPercentage = 1f;
        newLeg.Offset = transform.InverseTransformPoint(HipOffset);

        RaycastHit hit = GetFootPositionHit(transform.TransformPoint(newLeg.Offset) + newLeg.Root.localPosition + Vector3.up * 10f, Vector3.up);
        newLeg.origin = hit.point;
        newLeg.destination = newLeg.origin;
        newLeg.target = newLeg.origin;

        Legs.Add(newLeg);
        return newLeg;
    }



    private RaycastHit GetFootPositionHit(Vector3 origin, Vector3 localUp)
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;

        if (Physics.Raycast(origin, localUp * -10f, out hit, Mathf.Infinity, layerMask))
        {
            return hit;
        }
        return hit;
    }

    private Vector3 GetAvgLegPosition()
    {
        Vector3 avg = Vector3.zero;
        foreach(LegComponents leg in Legs)
        {
            avg += leg.target;
        }
        avg /= Legs.Count;
        return avg;
    }
    private void OperateLeg(LegComponents leg)
    {
        Transform HipRoot = leg.Root;
        Transform HipPitch = leg.HipPitch;
        Transform Knee = leg.Knee;
        Transform Ankle = leg.Ankle;
        float ThighLength = leg.ThighLength;
        float ShinLength = leg.ShinLength;
        float FootLength = leg.FootLength;
        
        Vector3 FootPos = leg.target + Vector3.up * FootLength;


        var hypeLength = (HipRoot.position - FootPos).magnitude;
        if (hypeLength > ThighLength + ShinLength)
        {
            hypeLength = ThighLength + ShinLength;
        }

        var HAng = ((Mathf.Acos((Mathf.Pow(ThighLength, 2) + Mathf.Pow(hypeLength, 2) - Mathf.Pow(ShinLength, 2)) / (2 * ThighLength * hypeLength)) * radianMultiplier) - 180f) * 1f;
        var KAng = Mathf.Acos((Mathf.Pow(ThighLength, 2) + Mathf.Pow(ShinLength, 2) - Mathf.Pow(hypeLength, 2)) / (2 * ThighLength * ShinLength)) * radianMultiplier * -1f;

        HipRoot.LookAt(FootPos);

        if (!float.IsNaN(HAng) && !float.IsNaN(KAng))
        {


            HipPitch.localEulerAngles = new Vector3(-HAng, 0f, 0f);
            Knee.localEulerAngles = new Vector3(KAng + 180f, 0f, 0f);
        }

        var kneePitch = Knee.rotation.eulerAngles.x;
        Ankle.rotation = Quaternion.Euler(-90f, Knee.rotation.eulerAngles.y, Knee.rotation.eulerAngles.z);
        
    }
}

public class LegComponents
{
    public float stepPercentage = 1f;
    public Vector3 origin = Vector3.zero;
    public Vector3 destination = Vector3.zero;
    public Vector3 target = Vector3.zero;
    public Vector3 Offset = new Vector3(0f,0f,0f);

    public Transform Root;
    public Transform HipPitch;
    public Transform Thigh;
    public Transform Knee;
    public Transform Shin;
    public Transform Ankle;
    public Transform Foot;

    public float ThighLength;
    public float ShinLength;
    public float FootLength;
}