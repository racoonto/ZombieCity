using UnityEngine;

public class Cursor : MonoBehaviour
{
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")))
        {
            transform.position = hit.point;
        }

        transform.rotation = Quaternion.identity;
    }
}