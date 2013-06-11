using UnityEngine;
using System.Collections.Generic;

public class DungeonShadowUnityPool : MonoBehaviour
{
    public GameObject shadowPrefab;

    private List<DungeonShadowUnity> shadowTilesPool = new List<DungeonShadowUnity>(1024);
    private Transform trans;

    public void Awake()
    {
        trans = transform;
    }

    public DungeonShadowUnity GetShadowTile()
    {
        DungeonShadowUnity shadow;

        if (shadowTilesPool.Count > 0)
        {
            shadow = shadowTilesPool[shadowTilesPool.Count - 1];
            shadowTilesPool.RemoveAt(shadowTilesPool.Count - 1);
            shadow.go.SetActive(true);
        }
        else
        {
            shadow = ((GameObject)GameObject.Instantiate(shadowPrefab)).GetComponent<DungeonShadowUnity>();
        }

        return shadow;
    }

    public void ReturnShadowTile(DungeonShadowUnity shadow)
    {
        shadow.go.SetActive(false);
        shadowTilesPool.Add(shadow);
        shadow.trans.parent = trans;
    }
}


