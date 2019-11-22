using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCardBackBehaviour : MonoBehaviour {
    private VirtualCardFrontBehaviour vCardFrontBhv;

	void Start () {
        vCardFrontBhv = GameObject.Find("ImageTarget Virtual Card Front").GetComponent<VirtualCardFrontBehaviour>();
	}
	
	void Update () {
        // Updating selected card transform
        if (vCardFrontBhv.SelectedCard == null)
        {
            transform.Find("SetOnField").gameObject.SetActive(false);
            return;
        }
        vCardFrontBhv.SelectedCard.transform.position = this.transform.position;
        vCardFrontBhv.SelectedCard.transform.eulerAngles = this.transform.eulerAngles;
        vCardFrontBhv.SelectedCard.transform.Rotate(new Vector3(0, 0, 180));

        //Vector3 eAngles = this.transform.eulerAngles;
        //Vector3 fwd = this.transform.TransformDirection(Vector3.forward);
        //vCardFrontBhv.SelectedCard.transform.Rotate(fwd[0] * 180f, fwd[1] * 180f, fwd[2] * 180f);
        //vCardFrontBhv.SelectedCard.transform.eulerAngles = this.transform.eulerAngles;
    }
}
