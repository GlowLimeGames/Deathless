using UnityEngine;

namespace Dialogue {
    [System.Serializable]
    public class Conditions : ConditionBase {
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

        public void InteractionTargetIsAnimate(bool animate) {
            isValid = GameItem.InteractionTarget != null && GameItem.InteractionTarget.IsAnimate == animate;
        }

        public void ItemExistsInWorld(WorldItem item) {
            isValid = item.hasInstance;
        }

        #endregion
    }
}