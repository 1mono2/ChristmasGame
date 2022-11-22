using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace MoNo.Christmas
{
    [RequireComponent(typeof(OrganizeQuantity))]
    [ExecuteInEditMode]
    public class DisplayOrganizeQuantity : MonoBehaviour
    {
        OrganizeQuantity quantity;
        TextMeshPro text;

        private void Start()
        {
            DisplayOnPanel();
        }

        void Reset()
        {
            DisplayOnPanel();
        }

        [ContextMenu("DisplayOnPanel")]
        void DisplayOnPanel()
        {
            quantity = GetComponent<OrganizeQuantity>();
            text = GetComponentInChildren<TextMeshPro>();

            switch (quantity.operations)
            {
                case OrganizeQuantity.ArithmeticOperations.addition:
                    text.text = $"+" + quantity.Num;
                    break;
                case OrganizeQuantity.ArithmeticOperations.subtraction:
                    text.text = $"-" + quantity.Num;
                    break;
                case OrganizeQuantity.ArithmeticOperations.multiplication:
                    text.text = $"x" + quantity.Num;
                    break;
                case OrganizeQuantity.ArithmeticOperations.division:
                    text.text = $"÷" + quantity.Num;
                    break;
                default:
                    text.text = $"Nan";
                    break;
            }
        }

    }
}