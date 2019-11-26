using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSelectCard : MonoBehaviour {

    //private GameObject selectedCard = null;
    //private Vector3 oldPosition;
    private Vector3 oldScale;
    private Vector3 oldEuler;
    private Vector3 oldPosition;
    private Collider selectedCardCollider = null;
    private VirtualCardFrontBehaviour vCardFront;
    gameButton button = null;

    void Start()
    {
        vCardFront = GetComponentInParent<VirtualCardFrontBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter");
        if (other.transform.parent == null)
            return;
        Debug.Log(other.transform.parent.name);
        Transform colliderParent = other.transform.parent;
        if (colliderParent.name == "card")
        {
            removeSelectedCard();
            Transform cardTransform = colliderParent.parent;
            vCardFront.SelectedCard = cardTransform.gameObject;
            selectedCardCollider = other;
            selectedCardCollider.enabled = false;
            //oldParent = cardTransform.parent;
            //cardTransform.parent = this.transform;
            //cardTransform.localPosition = new Vector3(0.04f, -0.15f, 0.07f);
            //cardTransform.localEulerAngles = new Vector3(0, 0, 0);
            this.oldScale = cardTransform.localScale;
            this.oldEuler = cardTransform.eulerAngles;
            this.oldPosition = cardTransform.position;

            cardTransform.localScale = new Vector3(0.14f, 0.025f, 0.17f);

            this.transform.parent.Find("text").gameObject.SetActive(false);

            Debug.Log(vCardFront.SelectedCard);
            
            List<gameCard> cards = Program.I().ocgcore.cards;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].gameObject == cardTransform.gameObject)
                {
                    
                    Debug.Log("card id " + i);
                    foreach (gameButton b in cards[i].buttons)
                    {
                        Debug.Log("button " + b.hint);
                        if (b.hint == InterString.Get("通常召唤@ui") || b.hint == InterString.Get("发动效果@ui"))
                        {
                            Debug.Log("Adding Summon or Activate button");
                            vCardFront.summonOrActivateBtn = b;
                        }
                        else if (b.hint == InterString.Get("前场放置@ui") || b.hint == InterString.Get("后场放置@ui"))
                        {
                            Debug.Log("Adding Set button");
                            vCardFront.setBtn = b;
                        }
                    }
                }
            }
        }
    }

    public void removeSelectedCard()
    {
        if (vCardFront.SelectedCard != null)
        {
            vCardFront.SelectedCard.transform.localScale = oldScale;
            vCardFront.SelectedCard.transform.eulerAngles = oldEuler;
            vCardFront.SelectedCard.transform.position = oldPosition;
            selectedCardCollider.enabled = true;
            Program.I().ocgcore.realize();
            vCardFront.SelectedCard = null;
        }
    }
}
