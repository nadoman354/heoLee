using UnityEngine;

public class RngService : MonoBehaviour, IRng
{
    public static RngService I { get; private set; }
    public int RunSeed { get; private set; }

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);

    }

    public void SetRunSeed(int seed)
    {
        RunSeed = seed;
        UnityEngine.Random.InitState(seed);
        Debug.Log($"[RngService] RunSeed={RunSeed}");
    }

    public System.Random GetScoped(string domain, params int[] tags)
    {
        unchecked
        {
            int s = Hash(RunSeed, domain);
            if (tags != null) foreach (var t in tags) s = Mix(s, t);
            return new System.Random(s);
        }
    }

    private static int Hash(int seed, string text)
    {
        unchecked
        {
            uint h = 2166136261; h ^= (uint)seed; h *= 16777619;
            if (!string.IsNullOrEmpty(text))
                for (int i = 0; i < text.Length; i++) { h ^= text[i]; h *= 16777619; }
            return (int)h;
        }
    }
    private static int Mix(int a, int b)
    {
        unchecked
        {
            uint x = (uint)(a ^ b);
            x *= 16777619; x += 0x9E3779B9; x ^= x >> 16;
            return (int)x;
        }
    }
}
