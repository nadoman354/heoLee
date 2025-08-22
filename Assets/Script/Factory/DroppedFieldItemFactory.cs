using UnityEngine;

public class DroppedFieldItemFactory : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // DroppedFieldItem ÇÁ¸®ÆÕ
    [SerializeField] private ResourcesItemMetaCatalog metaCatalog;

    private void Awake()
    {
        if (!metaCatalog) metaCatalog = ResourcesItemMetaCatalog.LoadDefault();
    }

    public DroppedItem CreateFromId(string id, Vector3 spawnPos)
    {
        if (!metaCatalog || !metaCatalog.TryGetFieldItem(id, out var meta))
        {
            Debug.LogError($"[DroppedFieldItemFactory] FieldItem meta not found: {id}");
            return null;
        }
        var go = PoolManager.Spawn(prefab, spawnPos, Quaternion.identity);
        var d = go.GetComponent<DroppedFieldItem>();
        d.Setup(meta);
        return d;
    }
}
