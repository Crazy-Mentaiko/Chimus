namespace ChimusBot.Utils;

public static class RandomUtil
{
    private static readonly Random _sharedRandom = new(); 
    
    public static int Random(int start, int length)
    {
        lock (_sharedRandom)
        {
            return _sharedRandom.Next(start, length - start);
        }
    }

    public static T PickOne<T>(T[] arr) => arr[Random(0, arr.Length)];
    public static T PickOne<T>(List<T> list) => list[Random(0, list.Count)];
}