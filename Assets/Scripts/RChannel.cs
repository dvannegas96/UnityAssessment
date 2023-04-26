using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;

public static class Extensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }
}

public class RChannel : MonoBehaviour
{
    public Texture2D texture;
    [SerializeField] private bool useJobSystem;

    private Color[] colors;

    private Color[] area1;
    private Color[] area2;
    private Color[] area3;
    private Color[] area4;

    private NativeArray<Color> nativeColorAreaOne;
    private NativeArray<Color> nativeColorAreaTwo;
    private NativeArray<Color> nativeColorAreaThree;
    private NativeArray<Color> nativeColorAreaFour;

    private NativeArray<int> resultAreaOne;
    private NativeArray<int> resultAreaTwo;
    private NativeArray<int> resultAreaThree;
    private NativeArray<int> resultAreaFour;



    // Start is called before the first frame update
    void Start()
    {
        colors = texture.GetPixels();
        int division = colors.Length / 4;
        area1 = colors.SubArray(0, division);
        area2 = colors.SubArray(division, division);
        area3 = colors.SubArray(2 * division, division);
        area4 = colors.SubArray(3 * division, division);
    }

    // Update is called once per frame
    void Update()
    {

        if (useJobSystem)
        {
            calculateRChannelJobSystem();
        }
        else
        {
            calculateRChannelSingleThread();
        }


    }

    private void calculateRChannelSingleThread()
    {
        int result = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            result = result + (int)colors[i].r;
        }
        Debug.Log("Total R Channel" + result);
    }

    private void calculateRChannelJobSystem()
    {
        nativeColorAreaOne = new NativeArray<Color>(area1, Allocator.Persistent);
        nativeColorAreaTwo = new NativeArray<Color>(area2, Allocator.Persistent);
        nativeColorAreaThree = new NativeArray<Color>(area3, Allocator.Persistent);
        nativeColorAreaFour = new NativeArray<Color>(area4, Allocator.Persistent);

        resultAreaOne = new NativeArray<int>(1, Allocator.TempJob);
        resultAreaTwo = new NativeArray<int>(1, Allocator.TempJob);
        resultAreaThree = new NativeArray<int>(1, Allocator.TempJob);
        resultAreaFour = new NativeArray<int>(1, Allocator.TempJob);


        float startTime = Time.realtimeSinceStartup;
        NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);

        ParallelJob jobArea1 = CalculateSumParallel(nativeColorAreaOne, resultAreaOne);
        JobHandle jobHandleArea1 = jobArea1.Schedule();

        ParallelJob jobArea2 = CalculateSumParallel(nativeColorAreaTwo, resultAreaTwo);
        JobHandle jobHandleArea2 = jobArea2.Schedule();


        ParallelJob jobArea3 = CalculateSumParallel(nativeColorAreaThree, resultAreaThree);
        JobHandle jobHandleArea3 = jobArea3.Schedule();

        ParallelJob jobArea4 = CalculateSumParallel(nativeColorAreaFour, resultAreaFour);
        JobHandle jobHandleArea4 = jobArea4.Schedule();

        jobHandleList.Add(jobHandleArea1);
        jobHandleList.Add(jobHandleArea2);
        jobHandleList.Add(jobHandleArea3);
        jobHandleList.Add(jobHandleArea4);

        JobHandle.CompleteAll(jobHandleList);
        Debug.Log("Area 1 result" + jobArea1.result[0]);
        Debug.Log("Area 2 result" + jobArea2.result[0]);
        Debug.Log("Area 3 result" + jobArea3.result[0]);
        Debug.Log("Area 4 result" + jobArea4.result[0]);
        Debug.Log("Total R Channel" + (jobArea1.result[0] + jobArea2.result[0] + jobArea3.result[0]) + jobArea4.result[0]);

        jobHandleList.Dispose();
        disposeAll();
    }

    private void disposeAll()
    {
        nativeColorAreaOne.Dispose();
        nativeColorAreaTwo.Dispose();
        nativeColorAreaThree.Dispose();
        nativeColorAreaFour.Dispose();

        resultAreaOne.Dispose();
        resultAreaTwo.Dispose();
        resultAreaThree.Dispose();
        resultAreaFour.Dispose();
    }

    private ParallelJob CalculateSumParallel(NativeArray<Color> currentArea, NativeArray<int> result)
    {
        return new ParallelJob { area = currentArea, result = result };
    }

    public struct ParallelJob : IJob
    {
        public NativeArray<Color> area;
        public NativeArray<int> result;

        public void Execute()
        {
            for (int i = 0; i < area.Length; i++)
            {
                result[0] = result[0] + (int)area[i].r;
            }
        }
    }

}


