using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{

    [SerializeField]
    private int currentFloor = 0;

    private int targetFloor = 0;

    private bool moving = false;

    [SerializeField]
    private ElevatorDoors doors;

    [SerializeField]
    private float elevatorSpeed = 4;

    [SerializeField]
    private float elevatorAcceleration = 1;

    private float currentSpeed = 0;

    private float[] elevatorPoints = { 0.318f, 6.759f, 13.447f, 20.051f, 26.671f, 33.32f, 39.953f };

    public static bool elevatorPowered = false;

    public bool eEnabled = true;

    new AudioSource audio;

    void Start()
    {
        for (int i = 0; i < elevatorPoints.Length; i++)
        {
            if (i != currentFloor)
                doors.CloseDoor(i);
            else doors.OpenDoor(i);
        }
        Animator door = doors.getDoor(currentFloor);
        door.GetComponent<Collider>().enabled = false;

        audio = GetComponent<AudioSource>();

        elevatorPowered = eEnabled;

    }

    public void CallElevator(int floor)
    {
        if (floor == currentFloor || moving || !elevatorPowered)
            return;

        targetFloor = floor;
        StartCoroutine(MoveElevator());

    }
    public void EnableElevators()
    {
        elevatorPowered = true;
    }

    IEnumerator MoveElevator()
    {

        //close current floor doors
        doors.CloseDoor(currentFloor);

        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Misc, "elevatorDoorClose", doors.getDoor(currentFloor).transform.TransformPoint(-1.76f,1,0), null, 1, 0);
        moving = true;
        yield return new WaitForSeconds(2);

        float targetY = elevatorPoints[targetFloor];
        float currentY = transform.localPosition.y;


        //play sound
        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Misc, "elevatorStart", transform.position, transform, 1, 0);
        audio.Play();
        //move to target height
        while (!Mathf.Approximately(targetY, transform.localPosition.y))
        {
            currentY = Mathf.MoveTowards(currentY, targetY, currentSpeed * Time.deltaTime);
            currentSpeed = Mathf.MoveTowards(currentSpeed, elevatorSpeed, elevatorAcceleration * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, currentY, transform.localPosition.z);
            yield return null;
        }

        //stop and open doors
        currentSpeed = 0;
        currentFloor = targetFloor;
        doors.OpenDoor(currentFloor);

        //stop sound
        audio.Stop();
        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Misc, "elevatorStop", transform.position, transform, 1, 0);
        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Misc, "elevatorDoorOpen", doors.getDoor(currentFloor).transform.TransformPoint(-1.76f,1,0), null, 1, 0);

        yield return new WaitForSeconds(2);
        moving = false;

        Animator door = doors.getDoor(currentFloor);
        door.GetComponent<Collider>().enabled = false;


    }

}
