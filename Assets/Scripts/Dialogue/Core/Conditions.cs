using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [System.Serializable]
    public class Conditions : MonoBehaviour {
        #region Conditional Logic

        private enum Type { AND, OR, NOT }

        public bool isValid {
            get { return CheckValidity(); }
            set {
                if (type == Type.AND) {
                    if (!value) { validAnd = value; }
                }
                else if (type == Type.OR) {
                    if (value) { validOr = value; }
                }
                else if (type == Type.NOT) {
                    if (value) { validNot = !value; }
                }
            }
        }

        private Type type;
        private bool validAnd;
        private bool validOr;
        private bool validNot;

        [SerializeField]
        private UnityEvent andConditions, orConditions, notConditions;

        private bool CheckValidity() {
            ResetVars();

            type = Type.AND;
            validAnd = true;
            andConditions.Invoke();

            type = Type.OR;
            if (orConditions.GetPersistentEventCount() > 0) {
                validOr = false;
                orConditions.Invoke();
            }
            else { validOr = true; }

            type = Type.NOT;
            validNot = true;
            notConditions.Invoke();

            return (validAnd && validOr && validNot);
        }

        private void ResetVars() {
            intVar = 0;
            stringVar = null;
        }

        #endregion

        #region Variable Caching

        private int intVar;
        private string stringVar;
        private GameItem itemVar;

        [EnumAction(typeof(GlobalInt))]
        public void SetIntVar(int i) {
            intVar = Globals.GetGlobal((GlobalInt)i);
        }

        [EnumAction(typeof(GlobalString))]
        public void SetStringVar(int i) {
            stringVar = Globals.GetGlobal((GlobalString)i);
        }

        public void SetItemVar(GameItem item) {
            itemVar = item;
        }

        #endregion

        #region Conditions

        public void IntIsGreaterThan(int i) {
            isValid = intVar > i;
        }

        public void IntIsLessThan(int i) {
            isValid = intVar < i;
        }

        public void StringEquals(string s) {
            isValid = (stringVar == s);
        }

        public void IntEquals(int i) {
            isValid = (intVar == i);
        }

        public void ItemSelected(InventoryItem item) {
            isValid = Inventory.SelectedItem != null && Inventory.SelectedItem.Equals(item);
        }

        public void ItemInInventory(InventoryItem item) {
            isValid = Inventory.HasItem(item);
        }

        public void ItemCombination(GameItem item) {
            isValid =
                ((Inventory.SelectedItem.Equals(itemVar) && GameItem.InteractionTarget.Equals(item))
                || (Inventory.SelectedItem.Equals(item) && GameItem.InteractionTarget.Equals(itemVar)));
        }

        public void InteractionTarget(GameItem item) {
            isValid = GameItem.InteractionTarget != null && GameItem.InteractionTarget.Equals(item);
        }

        #endregion
    }
}