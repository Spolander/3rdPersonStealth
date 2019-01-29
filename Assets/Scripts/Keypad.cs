using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Keypad : MonoBehaviour {

    [SerializeField]
    private int[] combination;

    [SerializeField]
    List<int> inputNumbers;


    [SerializeField]
    private TextMeshPro inputText;

    public MonoBehaviour target;

    [SerializeField]
    private string methodName;
    private void Start()
    {
        inputNumbers = new List<int>();
    }

    public void InputNumber(int number, bool erase, bool enter)
    {
        if (erase)
        {
            if(inputNumbers.Count > 0)
            {
                inputNumbers.RemoveAt(inputNumbers.Count - 1);
                string s = "";

                for (int i = 0; i < inputNumbers.Count; i++)
                    s += inputNumbers[i].ToString();

                inputText.text = s;
            }
            return;
        }
     
           
        else if (enter)
        {
            CheckCombination();
        }
        else if (inputNumbers.Count < combination.Length)
        {
            inputNumbers.Add(number);
            inputText.text = inputText.text + number.ToString();
        }
    }

    public void CheckCombination()
    {
        if (inputNumbers.Count != this.combination.Length)
        {
            //ouput error message
            inputText.text = "";
            inputNumbers.Clear();
        }
        else
        {
            for (int i = 0; i < combination.Length; i++)
                if (inputNumbers[i] != combination[i])
                {

                    inputText.text = "";
                    inputNumbers.Clear();
                    return;
                }
            inputText.text = "";
            inputNumbers.Clear();


            target.Invoke(methodName, 0);
                   
        }
    }
}
