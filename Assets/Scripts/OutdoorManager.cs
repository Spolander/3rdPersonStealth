using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorManager : MonoBehaviour
{

    [SerializeField]
    private WindZone windZone;

    [SerializeField]
    private Cloth cloth;

    [SerializeField]
    private Vector3 externalWind;

    [SerializeField]
    private Vector3 randomWind;


    private Vector3 windDirection;

    private Vector3 randomTarget;
    private Vector3 windTarget;

    bool outside = false;

    [SerializeField]
    private bool startOutside = true;

    [SerializeField]
    private AudioSource windAudio;

    public static bool enteredInside = false;

    [SerializeField]
    private AudioSource stealthMusic1;

    [SerializeField]
    private AudioReverbZone insideReverbZone;

    public static OutdoorManager instance;

    private float areaChangeSpeed = 0.6f;

    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        windZone = FindObjectOfType(typeof(WindZone)) as WindZone;

        cloth = GetComponentInChildren<Cloth>();

        externalWind = Vector3.Scale(windZone.transform.forward, externalWind);
        randomWind = Vector3.Scale(windZone.transform.forward + windZone.transform.right, randomWind);


        startOutside = !Physics.Raycast(transform.TransformPoint(0, 1, 0), Vector3.up, 100, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore);
        EnteredOutside(startOutside);
    }

    private void Update()
    {
        if (cloth)
        {
            cloth.externalAcceleration = Vector3.MoveTowards(cloth.externalAcceleration, windTarget, Time.deltaTime * 25);
            cloth.randomAcceleration = Vector3.MoveTowards(cloth.randomAcceleration, randomTarget, Time.deltaTime * 15);

        }

        if (windAudio)
        {
            if (outside)
                windAudio.volume = Mathf.MoveTowards(windAudio.volume, 1, Time.deltaTime*areaChangeSpeed);
            else
                windAudio.volume = Mathf.MoveTowards(windAudio.volume, 0.06f, Time.deltaTime*areaChangeSpeed);
        }


    }

    public void EnteredOutside(bool outside)
    {
        this.outside = outside;

        if (outside)
        {
            insideReverbZone.enabled = false;
            windTarget = externalWind;
            randomTarget = randomWind;
        }
        else
        {
            if (!enteredInside)
                stealthMusic1.Play();

            enteredInside = true;
            insideReverbZone.enabled = true;
            windTarget = Vector3.zero;
            randomTarget = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "outside")
        {
            EnteredOutside(true);

            DarkAmbientActivator da = other.GetComponent<DarkAmbientActivator>();

            if (da != null)
            {
                DarkAmbient.darkAmbientActivated = da.activate;
            }
        }

        else if (other.tag == "inside")
        {
            EnteredOutside(false);

            DarkAmbientActivator da = other.GetComponent<DarkAmbientActivator>();

            if (da != null)
            {
                DarkAmbient.darkAmbientActivated = da.activate;
            }
        }

    }

    void PlayerDeath()
    {
        DarkAmbient.darkAmbientActivated = false;
        stealthMusic1.Stop();
    }
    void PlayerReset()
    {
        cloth.ClearTransformMotion();
        stealthMusic1.Stop();
        enteredInside = false;
        EnteredOutside(startOutside);
    }
    private void OnEnable()
    {
        Player.OnDeath += PlayerDeath;
        Player.OnRestart += PlayerReset;
    }

    private void OnDisable()
    {
        Player.OnDeath -= PlayerDeath;
        Player.OnRestart -= PlayerReset;
    }

}
