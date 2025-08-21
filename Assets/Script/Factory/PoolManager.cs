using UnityEngine;

public static class PoolManager
{
    public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
        => Object.Instantiate(prefab, pos, rot);

    public static void Despawn(GameObject go)
        => Object.Destroy(go);
}
