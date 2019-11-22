using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutOnFieldTrigger : MonoBehaviour {
    VirtualCardFrontBehaviour vCardFrontBhv;
    ColliderSelectCard selCardCollider;
    void Start()
    {
        vCardFrontBhv = GetComponentInParent<VirtualCardFrontBehaviour>();
        selCardCollider = transform.parent.GetComponentInChildren<ColliderSelectCard>();
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
