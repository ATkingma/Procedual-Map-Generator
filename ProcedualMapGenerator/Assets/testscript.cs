using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class testscript : MonoBehaviour
{
    public TextMeshProUGUI text;

    int i;
  public  void printofz()
    {
        i++;
        text.text = i.ToString();
    }
}
