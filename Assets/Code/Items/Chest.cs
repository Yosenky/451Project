using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    // Outlets
    Animator animator;

    // Methods
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        animator.SetTrigger("Open");
        //ThirdPersonController.Instance.AddItem(1);
        GameController.Instance.InteractChest(this);
    }
    public void CancelAnimation()
    {
        animator.SetTrigger("Cancel");
    }
}
