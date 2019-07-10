using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class GuardVisionEffect : MonoBehaviour
{
    public Shader shader;
    private Material _material;

	public Texture texture;

	public float value;

	public float minValue;
	public float maxValue;

	private float sinTimer;
	private float speed = 5;

	void Update()
	{
		sinTimer += Time.deltaTime*speed;

		value = Mathf.Lerp(minValue,maxValue,(1+Mathf.Sin(sinTimer))/2);
	}
    protected Material material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null) return;
        Material mat = material;
        mat.SetTexture("_EffectTex", texture);
		mat.SetFloat("_Value",value);
        Graphics.Blit(source, destination, mat);
    }

    void OnDisable()
    {
        if (_material)
        {
            DestroyImmediate(_material);
        }
    }
}