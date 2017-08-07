using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [System.Serializable]
    public class Conditions : NodeCache {
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

        #endregion

        #region Conditions

        public void RandomChance(float percent) {
            isValid = percent <= Random.Range(0f, 1f);
        }

        public void IntIsGreaterThan(int i) {
            isValid = IntVar > i;
        }

        public void IntIsLessThan(int i) {
            isValid = IntVar < i;
        }

        public void IntEquals(int i) {
            isValid = (IntVar == i);
        }

        public void StringEquals(string s) {
            isValid = (StringVar == s);
        }

        [EnumAction(typeof(GlobalBool))]
        public void IsTrue(int i) {
            isValid = Globals.GetGlobal((GlobalBool)i);
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

        public void ItemExistsInWorld(WorldItem item) {
            isValid = item.hasInstance;
        }

        #endregion
    }
}