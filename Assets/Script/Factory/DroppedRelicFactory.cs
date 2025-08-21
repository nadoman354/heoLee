using UnityEngine;

public class DroppedRelicFactory : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // DroppedRelic ÇÁ¸®ÆÕ
    [SerializeField] private ResourcesItemMetaCatalog metaCatalog;

    private void Awake()
    {
        if (!metaCatalog) metaCatalog = ResourcesItemMetaCatalog.LoadDefault();
    }

    public DroppedItem CreateFromId(string id, Vector3 spawnPos)
    {
        if (!metaCatalog || !metaCatalog.TryGetRelic(id, out var meta))
        {
            Debug.LogError($"[DroppedRelicFactory] BaseRelic meta not found: {id}");
            return null;
        }
        var go = PoolManager.Spawn(prefab, spawnPos, Quaternion.identity);
        var d = go.GetComponent<DroppedRelic>();
        d.Setup(meta);
        return d;
    }

    public DroppedItem CreateFromRelic(BaseRelic r, Vector3 spawnPos)
    {
        var go = PoolManager.Spawn(prefab, spawnPos, Quaternion.identity);
        var d = go.GetComponent<DroppedRelic>();
        d.SetupOriginal(r);
        return d;
    }
}
