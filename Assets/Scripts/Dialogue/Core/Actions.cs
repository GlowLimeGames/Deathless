using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/*Design to be able to allow you to have things
  happen in the game with dialogue-->used with unity events
  Unity is pretty limited with their event system-->so we expand on it with
  this script.
     */

  
namespace Dialogue {
    [System.Serializable]
    public class Actions : NodeCache {
        [SerializeField]
        //holds ALL OF THE ACTIONS 
        private UnityEvent actions;
        //queue used to handle if game tries to trigger multiple trigger at the same time
        private static List<Actions> queue = new List<Actions>();
        //the current action
        private static Actions current {
            get {
                if (queue.Count > 0) { return queue[0]; }
                else { return null; }
            }
        }
        private int pendingActions = 0;

        //stuff with nodes is used because there's the dialogue, then we want to stop the dialogue, 
        //let the action happen, then resume dialogue 
        //interacting with the dialogue nodes 
        private Node currentNode;


        //how to trigger all the actions
       //not all actions needs the dialogue to pause, so we have stuff in the code handling that 
        public bool Invoke(Node currentNode) {
            if (actions != null && actions.GetPersistentEventCount() > 0 && !queue.Contains(this)) {
                this.currentNode = currentNode;
                queue.Add(this);
                if (current == this) { actions.Invoke(); }
                else { Debug.Log("Please note that simultaneous Actions are not permitted. Actions will be queued and run in succession."); }
                return true;
            }
            else {
                return DialogueManager.Continue(currentNode);
            }
        }


        void Update() {
            if (current == this && pendingActions == 0) {
                CompleteActions();
            }
        }

        private void OnDestroy() {
            if (current == this) { CompleteActions(); }
        }

        private void CompleteActions() {
            queue.Remove(this);
            if (current != null) { current.actions.Invoke(); }
            else { DialogueManager.Continue(currentNode); }
        }

        public static void CompletePendingAction() {
            current.pendingActions--;
        }



        #region Actions

        [EnumAction(typeof(GlobalBool))]
        public void SetTrue(int i) {
            Globals.SetGlobal((GlobalBool)i, true);
        }

        [EnumAction(typeof(GlobalBool))]
        public void SetFalse(int i) {
            Globals.SetGlobal((GlobalBool)i, false);
        }

        public void SetInt(int i) {
            IntVar = i;
        }

        public void AddToInt(int i) {
            IntVar += i;
        }

        public void SetString(string s) {
            StringVar = s;
        }

        public void AddToInventory(InventoryItem item) {
            Inventory.AddItem(item);
        }

        public void RemoveFromInventory(InventoryItem item) {
            Inventory.RemoveItem(item);
        }

        public void SetSelectedItem (InventoryItem item) {
            Inventory.SelectItem(item);
        }

        public void ClearSelectedItem() {
            Inventory.ClearSelection();
        }

        public void RedirectDialogue (SerializableTree dlg) {
            DialogueManager.RedirectDialogue(dlg);
        }

        public void RemoveFromWorld(WorldItem item) {
            item.RemoveFromWorld();
        }

        public void EnableItem(GameItem item) {
            item.Enable();
        }

        public void InstantiateItemAtPosition(Transform t) {
            WorldItem item = itemVar as WorldItem;
            if (item != null) {
                Instantiate(item, GetPos(t), Quaternion.identity).gameObject.name = item.gameObject.name; ;
            }
        }

        public void ChangeItemName(string name) {
            if (itemVar != null) { itemVar.SetName(name); }
            else {
                Debug.LogWarning("Unable to change item name to \"" + name + "\" " +
                    "- Note that you must cache an item (SetItemVar) before using this command.");
            }
        }

        public void ChangeItemSprite(Sprite sprite) {
            if (itemVar!= null) {
                itemVar.ChangeSprite(sprite);

                if (itemVar.AnimController != null) {
                    Debug.LogWarning("Changing sprites on animated items is not supported. Use ChangeItemIdle instead.");
                }
            }
            else {
                Debug.LogWarning("Unable to set the sprite on this item. " +
                    "Note that you must cache an item (SetItemVar) before using this command.");
            }
        }

        public void ChangeItemIdle(AnimationClip anim) {
            if (itemVar != null && itemVar.AnimController != null) {
                itemVar.AnimController.SetIdle(anim);
            }
        }

        /// <summary>
        /// Replaces the cached WorldItem with the given WorldItem in the game scene.
        /// </summary>
        public void ReplaceItemWith(WorldItem item) {
            if (itemVar != null) {
                if (itemVar.GetType() == typeof(WorldItem)) {
                    Instantiate(item, GetPos(itemVar.transform), Quaternion.identity).gameObject.name = item.gameObject.name;
                    ((WorldItem)itemVar).RemoveFromWorld();
                }
                else { Debug.LogWarning("Cannot place an inventory item in the world."); }
            }
            else { Debug.LogWarning("No WorldItem has been cached. Use SetItemVar to cache an item."); }
        }

        public void PlayAnimationAtItem(AnimationClip clip) {
            if (itemVar != null && itemVar.AnimController != null) {
                pendingActions++;
                itemVar.AnimController.PlayOneShot(clip, true);
            }
        }

        public void PlayAnimationAtPosition(AnimationClip clip) {
            if (transformVar != null) {
                pendingActions++;
                AnimController.PlayAnimAt(clip, GetPos(transformVar), true);
            }
        }

        public void MoveTo(Transform t) {
            WorldItem item = itemVar as WorldItem;
            if (item != null) {
                pendingActions++;
                item.MoveToPoint(GetPos(t), true);
            }
        }

        public void TeleportTo(Transform t) {
            WorldItem item = itemVar as WorldItem;
            if (item != null) {
                item.TeleportToPoint(GetPos(t));
            }
        }

        public void FadeIn() {
            pendingActions++;
            UIManager.FadeIn(true);
        }

        public void FadeOut() {
            pendingActions++;
            UIManager.FadeOut(true);
        }

        public void GhostFadeIn(WorldItem ghost) {
            if (ghost.hasInstance) {
                GhostFadeAnim anim = ghost.Instance.gameObject.GetComponent<GhostFadeAnim>();
                if (anim != null) {
                    pendingActions++;
                    anim.StartFadeIn(true);
                }
            }
        }

        public void GhostFadeOut(WorldItem ghost) {
            if (ghost.hasInstance) {
                GhostFadeAnim anim = ghost.Instance.gameObject.GetComponent<GhostFadeAnim>();
                if (anim != null) {
                    pendingActions++;
                    anim.StartFadeOut(true);
                }
            }
        }

   
        //plays sound of the event 
        public void TriggerSound( string eventName )
        {
       
            AudioController.PlayEvent(eventName);
              
          
            
        }
        //pause sound of event 
    
        public void PauseSound(string eventName) {
          
            AudioController.PauseEvent(eventName);
        }

        //resume sound 
        public void ResumeSound(string eventName) {
         
            AudioController.ResumeEvent(eventName);
        }

        //Completely stops the sound in the game 
        public void StopSound(string eventName) {
           
            AudioController.StopEvent(eventName);

        }
       

        #endregion
    }
}