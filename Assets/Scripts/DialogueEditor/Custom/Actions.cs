using UnityEngine;
 
namespace Dialogue {
    [System.Serializable]
    public class Actions : ActionBase {
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

        public void DisableItem(GameItem item) {
            item.Disable();
        }

        public void InstantiateItemAtPosition(Transform t) {
            WorldItem item = itemVar as WorldItem;
            if (item != null) {
                Instantiate(item, GetPos(t), item.transform.rotation).gameObject.name = item.gameObject.name;
                World.UpdateNavGraph();
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
                    Debug.LogWarning("This item has an AnimContoller. Did you mean to change its idle?");
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
                    Instantiate(item, GetPos(itemVar.transform), item.transform.rotation).gameObject.name = item.gameObject.name;
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

        public void Wait(float seconds) {
            pendingActions++;
            Util.Wait(this, seconds, CompletePendingAction);
        }

        public void FadeIn() { FadeIn(Fadable.DEFAULT_FADE_RATE); }
        public void FadeOut() { FadeOut(Fadable.DEFAULT_FADE_RATE); }

        public void FadeIn(float fadeDuration) {
            pendingActions++;
            UIManager.FadeIn(fadeDuration, CompletePendingAction);
        }

        public void FadeOut(float fadeDuration) {
            pendingActions++;
            UIManager.FadeOut(fadeDuration, CompletePendingAction);
        }

        public void GhostFadeIn(WorldItem ghost) {
            if (ghost.hasInstance) {
                GhostFadeAnim anim = ghost.Instance.gameObject.GetComponent<GhostFadeAnim>();
                if (anim != null) {
                    pendingActions++;
                    anim.StartGhostFade(true, true);
                }
            }
        }

        public void GhostFadeOut(WorldItem ghost) {
            if (ghost.hasInstance) {
                GhostFadeAnim anim = ghost.Instance.gameObject.GetComponent<GhostFadeAnim>();
                if (anim != null) {
                    pendingActions++;
                    anim.StartGhostFade(false, true);
                }
            }
        }
        
        public void TriggerSound(string eventName) {
            AudioController.PlayEvent(eventName); 
        }
        
        public void PauseSound(string eventName) {
            AudioController.PauseEvent(eventName);
        }
        
        public void ResumeSound(string eventName) {
            AudioController.ResumeEvent(eventName);
        }
        
        public void StopSound(string eventName) {
            AudioController.StopEvent(eventName);
        }

        [EnumAction(typeof(GameScene))]
        public void LoadScene(int scene) {
            Scenes.BeginSceneTransition((GameScene)scene);
        }

        #endregion
    }
}