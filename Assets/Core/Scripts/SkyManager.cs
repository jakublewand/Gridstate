using UnityEngine;

public class SkyManager : MonoBehaviour
{
    [SerializeField] private Transform Sun;
    [SerializeField] private Transform Moon;
    [SerializeField] private Material skyBlendMaterial;
    private AnimationCurve blendCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(05f, 1f),
        new Keyframe(30f, 1f),
        new Keyframe(60f, 0f),
        new Keyframe(85f, 0f)
    );

    private City city;

    void Start()
    {
        city = City.instance;
    }

    void Update()
    {
        float progress = city.getDayProgress();
        float deg = 360f * (progress / 100f);

        Sun.transform.rotation = Quaternion.Euler(deg, 30f, 0);
        Moon.transform.rotation = Quaternion.Euler(180f + deg, 30f, 0);

        if (skyBlendMaterial != null)
            skyBlendMaterial.SetFloat("_Blend", blendCurve.Evaluate(progress));
    }
}
