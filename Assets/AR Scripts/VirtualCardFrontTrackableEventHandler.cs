using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VirtualCardFrontTrackableEventHandler : DefaultTrackableEventHandler
{
    private GameObject vCardBack;
    private VirtualCardFrontBehaviour vCardFrontBhv;
    private bool started = false;

    protected new void Start () {
        base.Start();
        vCardBack = GameObject.Find("ImageTarget Virtual Card Back");
        //vCardBack.SetActive(false);
        vCardFrontBhv = gameObject.GetComponent<VirtualCardFrontBehaviour>();
        started = true;
    }
	
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        if (!started) return;
        //vCardBack.SetActive(false);
        if (vCardFrontBhv.SelectedCard != null)
        {
            vCardFrontBhv.SelectedCard.SetActive(true);
        }
        vCardFrontBhv.CardOrientation = "UP";
        vCardFrontBhv.enabled = true;
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        if (!started) return;
        if (vCardFrontBhv.SelectedCard != null)
        {
            vCardFrontBhv.SelectedCard.SetActive(false);
            //vCardBack.SetActive(true);
        }
        vCardFrontBhv.enabled = false;
    }
}
