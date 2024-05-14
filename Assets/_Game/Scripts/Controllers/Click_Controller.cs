using System;
using UnityEngine;

public class Click_Controller : MonoBehaviour
{
    public static Action<Vector3> OnClick;


    void Update()
    {
        CheckForUserClick();
    }

    private void CheckForUserClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            OnClick?.Invoke(Input.mousePosition);
        }
    }
}
