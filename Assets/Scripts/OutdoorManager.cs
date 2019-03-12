using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorManager : MonoBehaviour {

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

    private void Start()
    {
        windZone = FindObjectOfType(typeof(WindZone)) as WindZone;

        cloth = GetComponentInChildren<Cloth>();

        externalWind = Vector3.Scale(windZone.transform.forward, externalWind);
        randomWind = Vector3.Scale(windZone.transform.forward+windZone.transform.right, randomWind);


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
                windAudio.volume = Mathf.MoveTowards(windAudio.volume, 1, Time.deltaTime);
            else
                windAudio.volume = Mathf.MoveTowards(windAudio.volume, 0.06f, Time.deltaTime);
        }
       

    }

    void EnteredOutside(bool outside)
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
            EnteredOutside(true);
        else if (other.tag == "inside")
            EnteredOutside(false);
    }

    void PlayerDeath()
    {
        stealthMusic1.Stop();
    }
    void PlayerReset()
    {
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
