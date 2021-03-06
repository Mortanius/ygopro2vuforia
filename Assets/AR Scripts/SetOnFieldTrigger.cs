﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOnFieldTrigger : MonoBehaviour {
    GameObject vCardFront;
    VirtualCardFrontBehaviour vCardFrontBhv;
    ColliderSelectCard selCardCollider;
    void Start()
    {
        vCardFront = GameObject.Find("ImageTarget Virtual Card Front");
        Debug.Assert(vCardFront != null);
        vCardFrontBhv = vCardFront.GetComponent<VirtualCardFrontBehaviour>();
        Debug.Assert(vCardFrontBhv != null);
        //selCardCollider = vCardFront.GetComponentInChildren<ColliderSelectCard>();
        selCardCollider = vCardFront.transform.Find("Selector").GetComponent<ColliderSelectCard>();
        Debug.Assert(selCardCollider != null);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name != "FieldCollider") return;
        selCardCollider.removeSelectedCard();
        gameButton btn = vCardFrontBhv.PutOnFieldBtn;
        if (btn != null)
            Program.I().ocgcore.ES_gameButtonClicked(btn);
    }
}
