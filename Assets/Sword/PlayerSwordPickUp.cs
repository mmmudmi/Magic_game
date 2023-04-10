using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordPickUp : MonoBehaviour
{
    public Transform swordHolder;
    private GameObject pickedUpSword;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && pickedUpSword == null)
        {
            PickupSword();
        }
    }

    private void PickupSword()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.0f);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Sword"))
            {
                pickedUpSword = hitCollider.gameObject;
                pickedUpSword.transform.SetParent(swordHolder);
                pickedUpSword.transform.localPosition = Vector3.zero;
                pickedUpSword.transform.localRotation = Quaternion.identity;
                pickedUpSword.GetComponent<Collider>().enabled = true;
                pickedUpSword.GetComponent<Rigidbody>().isKinematic = true;
                break;
            }
        }
    }
}