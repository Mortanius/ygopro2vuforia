using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VirtualCardBackTrackableEventHandler : DefaultTrackableEventHandler
{

    private GameObject vCardFront;
    private VirtualCardFrontBehaviour vCardFrontBhv;
    VirtualCardBackBehaviour vCardBackBhv;
    private bool started = false;

    protected new void Start()
    {
        base.Start();
        vCardFront = GameObject.Find("ImageTarget Virtual Card Front");
        vCardFrontBhv = vCardFront.GetComponent<VirtualCardFrontBehaviour>();
        vCardBackBhv = gameObject.GetComponent<VirtualCardBackBehaviour>();
        started = true;
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        if (!started) return;
        //vCardFront.SetActive(false);
        if (vCardFrontBhv.SelectedCard != null)
        {
            vCardFrontBhv.SelectedCard.SetActive(true);
        }
        vCardFrontBhv.CardOrientation = "DOWN";
        vCardBackBhv.enabled = true;
    }
    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        Debug.Log("LOST BACK");
        if (!started) return;
        vCardBackBhv.enabled = false;
        //vCardFront.SetActive(true);
    }

}
