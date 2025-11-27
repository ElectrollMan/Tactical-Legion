using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    public GameObject preFab;
    public int number = 3;

    private int startNumber = 0;
    private RaycastHit2D hit;

    void Start()
    {
        while (true)
        {
            Vector2 direction = new Vector2(transform.position.x + Random.Range(-15, 15), transform.position.y - 10);
            hit = Physics2D.Raycast(transform.position, direction);

            if (hit.transform != null && hit.transform.tag == "background")
            {
                Instantiate(preFab, new Vector2(hit.point.x, hit.point.y + 2), Quaternion.identity);
                startNumber++;
            }

            if (startNumber == number)
                break;
        }
    }

    void Update()
    {
        Vector2 debugDir = new Vector2(transform.position.x + Random.Range(-15, 15), transform.position.y - 10);
        Debug.DrawRay(transform.position, debugDir);
    }
}
