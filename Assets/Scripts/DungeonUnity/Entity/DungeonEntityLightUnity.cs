using UnityEngine;

public class DungeonEntityLightUnity : DungeonEntityUnity
{
    public Light dungeonLight;
    public GameObject fx;

    private float originalIntensity;

    public override void Awake()
    {
        base.Awake();

        originalIntensity = dungeonLight.intensity;
    }

    public void Update()
    {
        if (entity.IsVisible())
            dungeonLight.intensity = Mathf.MoveTowards(dungeonLight.intensity, originalIntensity, Time.deltaTime * 4.0f);
        else
            dungeonLight.intensity = Mathf.MoveTowards(dungeonLight.intensity, 0, Time.deltaTime * 4.0f);

        dungeonLight.enabled = dungeonLight.intensity > 0;
        fx.SetActive(dungeonLight.enabled);
    }
}


