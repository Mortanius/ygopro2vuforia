using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCardFrontBehaviour : MonoBehaviour {
    private GameObject selectedCard;
    private List<gameButton> cardButtons;
    public GameObject SelectedCard
    {
        get { return selectedCard; }
        set { selectedCard = value; }
    }

    private gameButton putOnFieldBtn;
    public gameButton PutOnFieldBtn { get { return putOnFieldBtn; } }
    public gameButton summonOrActivateBtn;
    public gameButton setBtn;
    // TODO Use enumerators instead of strings
    public string CardOrientation
    {
        get
        {
            if (putOnFieldBtn == summonOrActivateBtn)
                return "UP";
            else if (putOnFieldBtn == setBtn)
                return "DOWN";
            else
                return null;
        }
        set
        {
            if (value == "UP")
                putOnFieldBtn = summonOrActivateBtn;
            else if (value == "DOWN")
                putOnFieldBtn = setBtn;
            else
                putOnFieldBtn = null;

        }
    }
    GameObject text;
    GameObject Selector;
    GameObject PutOnField;

    void Start()
    {
        text = transform.Find("text").gameObject;
        Selector = transform.Find("Selector").gameObject;
        PutOnField = transform.Find("PutOnField").gameObject;
    }

    void Update()
    {
        // Enable / disable card selection
        float rot = Mathf.Abs(180f - transform.eulerAngles[1]);
        if (rot >= 90f - 30f && rot <= 90f + 30f)
        {
            text.SetActive(true);
            //GetComponent<Collider>().enabled = true;
            Selector.gameObject.SetActive(true);
            PutOnField.gameObject.SetActive(false);
        }
        else
        {
            text.gameObject.SetActive(false);
            //GetComponent<Collider>().enabled = false;
            Selector.gameObject.SetActive(false);
            if (putOnFieldBtn != null)
            {
                Debug.Log("Activating PutOnField");
                transform.Find("PutOnField").gameObject.SetActive(true);
            }
        }

        // Updating selected card transform
        if (selectedCard == null)
        {
            transform.Find("PutOnField").gameObject.SetActive(false);
            return;
        }
        selectedCard.transform.position = this.transform.position;
        selectedCard.transform.eulerAngles = this.transform.eulerAngles;
    }
}
