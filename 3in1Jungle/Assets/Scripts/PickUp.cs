using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform Item;
    private Vector3 scaleChange;
    private int spin= 100;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, spin * Time.deltaTime));
    }
    private void OnMouseDown()
    {
        
            //GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = Item.position;
            this.transform.parent = GameObject.Find("ItemOnCamera").transform;
            this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            spin = 0;
            



    }
    private void OnMouseUp()
    {
        this.transform.parent = null;
        transform.position = new Vector3(transform.position.x, -0.4f, transform.position.z);
        this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        spin = 100;

    }
    
}
