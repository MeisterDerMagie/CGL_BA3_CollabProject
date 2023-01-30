using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJPult : MonoBehaviour
{
    [SerializeField] private List<KeyCode> inputs;
    [SerializeField] private List<Transform> icons;
    [SerializeField] private float moveAmount;
    [SerializeField] private List<GameObject> buttonsUp;
    [SerializeField] private List<GameObject> buttonsDown;

    void Start()
    {
        foreach (GameObject button in buttonsUp)
        {
            button.SetActive(true);
        }
        foreach (GameObject button in buttonsDown)
        {
            button.SetActive(false);
        }
    }

    void Update()
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            if (Input.GetKeyDown(inputs[i]))
            {
                buttonsUp[i].SetActive(false);
                buttonsDown[i].SetActive(true);
                icons[i].localPosition = new Vector3(icons[i].localPosition.x, icons[i].localPosition.y - moveAmount, icons[i].localPosition.z);
            }
            if (Input.GetKeyUp(inputs[i]))
            {
                buttonsUp[i].SetActive(true);
                buttonsDown[i].SetActive(false);
                icons[i].localPosition = new Vector3(icons[i].localPosition.x, icons[i].localPosition.y + moveAmount, icons[i].localPosition.z);
            }
        }
    }
}
