using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class InteractUIManager : MonoBehaviour {

    Camera main;

    [SerializeField]
    private Canvas targetCanvas;

    [SerializeField]
    private float updateInterval;

    [SerializeField]
    private LayerMask blockingLayers;

    [SerializeField]
    private LayerMask interactLayers;


    [SerializeField]
    private float scanRadius = 10;


    [SerializeField]
    private float scanInterval = 2;

    private float lastScanTime;

    int maxInteractAmount = 15;
    List<GameObject> iconPool;

    List<InteractLinker> linkers;

    [SerializeField]
    private GameObject interactIconPrefab;

    List<InteractLinker> toRemove;

    [System.Serializable]
    class InteractLinker
    {
        public GameObject target;
        public GameObject icon;

        public InteractLinker(GameObject target, GameObject icon)
        {
            this.target = target;
            this.icon = icon;
        }
    }

    private void Start()
    {
        linkers = new List<InteractLinker>();
        toRemove = new List<InteractLinker>();
        iconPool = new List<GameObject>();

        for (int i = 0; i < maxInteractAmount; i++)
        {
            GameObject g = (GameObject)Instantiate(interactIconPrefab);
            g.SetActive(false);
            g.transform.SetParent(targetCanvas.transform);
            iconPool.Add(g);
        }

        main = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        if (Time.time > lastScanTime + scanInterval)
        {
            ScanForInteractables();
        }

        for (int i = 0; i < linkers.Count; i++)
        {
            Vector3 screenPos = main.WorldToScreenPoint(linkers[i].target.transform.position);
            screenPos.z = 0;
            linkers[i].icon.transform.position = screenPos;
            if (main.transform.InverseTransformPoint(linkers[i].target.transform.position).z <= 0)
                toRemove.Add(linkers[i]);
        }

        if (toRemove.Count > 0)
            RemoveInteractable(toRemove);
        toRemove.Clear();
    }

    void ScanForInteractables()
    {
        lastScanTime = Time.time;
        Vector3 headPosition = main.transform.position; //transform.position + Vector3.up * 2;
        Collider[] cols = Physics.OverlapSphere(headPosition, scanRadius, interactLayers, QueryTriggerInteraction.Collide);

        for (int i = 0; i < cols.Length; i++)
        {
            Vector3 checkPoint = cols[i].transform.position + (headPosition - cols[i].transform.position).normalized * 0.5f;
            Debug.DrawLine(headPosition, cols[i].transform.position);
           if (!Physics.Linecast(headPosition, checkPoint, blockingLayers))
           {
                if (main.transform.InverseTransformPoint(cols[i].transform.position).z > 0)
                {
                    if (!linkers.Any(x => x.target == cols[i].gameObject) && linkers.Count < maxInteractAmount)
                    {
                        AddNewInteractable(cols[i].gameObject);
                    }
                }
           }
        }

        for (int i = 0; i < linkers.Count; i++)
        {
            if (/*Physics.Linecast(headPosition, linkers[i].target.transform.position, blockingLayers) ||*/ main.transform.InverseTransformPoint(linkers[i].target.transform.position).z <= 0)
            {
                toRemove.Add(linkers[i]);
            }
        }

        if (toRemove.Count > 0)
            RemoveInteractable(toRemove);

      
    }

    void AddNewInteractable(GameObject g)
    {
        GameObject icon = getIcon();
        InteractLinker linker = new InteractLinker(g, icon);
        linkers.Add(linker);
        icon.SetActive(true);
    }
    void RemoveInteractable(List<InteractLinker> l)
    {
        for (int i = 0; i < l.Count; i++)
        {
            l[i].icon.SetActive(false);
            linkers.Remove(l[i]);
        }

    }
    GameObject getIcon()
    {
        for (int i = 0; i < iconPool.Count; i++)
            if (iconPool[i].activeInHierarchy == false)
                return iconPool[i];

        return null;
    }
}
