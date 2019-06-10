using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretAreaTrigger : MonoBehaviour
{
    [SerializeField]
    private SphereMaskController maskController;


    [SerializeField]
    private bool enter = false;

    [SerializeField]
    private Collider[] colliders;

    [SerializeField]
    private GameObject[] hiddenObjects;

    [SerializeField]
    private GameObject secretWall;

    public static SecretAreaTrigger instance;

	[SerializeField]
	private ReflectionProbe secretAreaProbe;


	public Texture tex;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
			StartCoroutine(startRoutine());
        }

    }

	IEnumerator startRoutine()
	{
		int id = secretAreaProbe.RenderProbe();
		

		while(secretAreaProbe.IsFinishedRendering(id) == false)
		{
			yield return null;
		}

		tex = secretAreaProbe.texture;

    	ResetSecretArea();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            maskController.radius = enter ? 300 : 0;

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = enter;
            }

            for (int i = 0; i < hiddenObjects.Length; i++)
            {
                hiddenObjects[i].gameObject.SetActive(enter);
            }

            secretWall.GetComponent<BoxCollider>().enabled = !enter;
            if (enter)
            {
				secretWall.GetComponent<Renderer>().material.shader = Shader.Find("Custom/InverseSphericalMaskDissolve");
            }
			else
			{
				secretWall.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
			}

        }
    }

    public void ResetSecretArea()
    {
        maskController.radius = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        for (int i = 0; i < hiddenObjects.Length; i++)
        {
            hiddenObjects[i].gameObject.SetActive(false);
        }

        secretWall.GetComponent<BoxCollider>().enabled = true;
        secretWall.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
    }
}
