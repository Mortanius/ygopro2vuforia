using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class FieldTrackableEventHandler : DefaultTrackableEventHandler {

    private bool firstDetection = true;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        // Update cards placement
        if (firstDetection)
        {
            Program.I().ocgcore.realize();
            firstDetection = false;
        }
    }
}