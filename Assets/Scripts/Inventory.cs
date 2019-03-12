using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
[RequireComponent(typeof(Camera))]
public class Inventory : MonoBehaviour {

    [SerializeField]
    private List<Item> items;

    public static Inventory instance;

    private Camera inventoryCamera;

    private Item currentItem;

    private bool activated = false;

    private float inventoryDeltaTime;


    [SerializeField]
    private float inspectRotationSpeed = 150;


    private PostProcessVolume ppv;

    private DepthOfField dof;


    public int inventoryIndex = 0;
    private void Awake()
    {
        instance = this;
        items = new List<Item>();
        inventoryCamera = GetComponent<Camera>();
        ppv = FindObjectOfType(typeof(PostProcessVolume)) as PostProcessVolume;

        if (ppv)
            ppv.profile.TryGetSettings(out dof);

    }


    IEnumerator inspectItem()
    {
        float deltaTime = 0;
        float lastFrame = System.DateTime.Now.Millisecond;
        while (activated)
        {
            deltaTime = (System.DateTime.Now.Millisecond - lastFrame)/1000;
            deltaTime = Mathf.Clamp(deltaTime, 0, 1);
            if (currentItem)
            {
                currentItem.transform.Rotate(0, -Input.GetAxisRaw("KeyboardHorizontal") * deltaTime * inspectRotationSpeed, -Input.GetAxisRaw("KeyboardVertical")*deltaTime*inspectRotationSpeed);


                if (Input.GetKeyDown(KeyCode.Q))
                {
                    inventoryIndex++;
                    PositionItem(ref inventoryIndex);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    inventoryIndex--;
                    PositionItem(ref inventoryIndex);
                }
            }
            lastFrame = System.DateTime.Now.Millisecond;
            yield return null;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Player.instance.CloseUpEnabled == false)
        {
            Activate();
        }
    }
    public void AddItem(Item item)
    {
        items.Insert(0, item);
        item.gameObject.layer = LayerMask.NameToLayer("Inventory");
        item.transform.SetParent(transform);
        item.gameObject.SetActive(false);
    }

    public void Activate()
    {
        activated = !activated;

        if (items.Count == 0)
        {
            activated = false;
        }

        inventoryIndex = 0;
        if (activated == false)
        {
            Time.timeScale = 1;
            inventoryCamera.enabled = false;
            if (dof)
                dof.enabled.value= false;
        }
        else
        {
            if (dof)
                dof.enabled.value = true;
            Time.timeScale = 0;
            inventoryCamera.enabled = true;

            if (items.Count > 0)
                PositionItem(ref inventoryIndex);

            StartCoroutine(inspectItem());
        }

    }

    private void PositionItem(ref int index)
    {
        if (currentItem)
            currentItem.gameObject.SetActive(false);

        if (index > items.Count-1)
            index = 0;
        else if (index < 0)
            index = items.Count - 1;

        currentItem = items[index];
        currentItem.gameObject.SetActive(true);
        currentItem.transform.localPosition = currentItem.InventoryPosition;
        currentItem.transform.localEulerAngles = currentItem.InventoryRotation;

    }

    public bool HasItem(string tag)
    {
        for (int i = 0; i < items.Count; i++)
            if (items[i].tag == tag)
                return true;

        return false;
    }
}
