namespace TerrariaParsers.Common;

public static class ArrayExtensions
{
    public static void FillArray<T>(this T[] array)
        where T : new()
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new T();
        }
    }

    public static byte BoolArrayToByte(this bool[] array, int length = 8, int start = 0)
    {
        byte result = 0;

        for (var i = start; i < length; i++)
        {
            if (array[i])
                result |= (byte)(1 << i);
        }
        return result;
    }
}
