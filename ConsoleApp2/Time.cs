using System.Diagnostics;
public static class Time
{
    public static double deltaTime { get; private set; }
    static double secondFrame;
    public static double elapsedTime { get; private set; }
    static Stopwatch stopWatch;
    private static float multi = 1.0f;
    public static void Start()
    {
        stopWatch = new Stopwatch();
        stopWatch.Start();

        Events.update += Update;
    }

    private static void Update()
    {
        TimeSpan ts = stopWatch.Elapsed;
        double FirstFrame = ts.TotalMilliseconds;

        deltaTime = (FirstFrame - secondFrame) * multi;
        elapsedTime += deltaTime;

        secondFrame = FirstFrame;
    }
}

