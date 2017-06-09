using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [System.Serializable]
    public class Condition : MonoBehaviour {
        private enum Type { AND, OR }

        public bool isValid {
            get { return CheckValidity(); }
            set {
                if (type == Type.AND) {
                    if (!value) { validAnd = value; }
                }
                else if (type == Type.OR) {
                    if (value) { validOr = value; }
                }
            }
        }

        private Type type;
        private bool validAnd;
        private bool validOr;
        
        private int intVar;
        private string stringVar;

        [SerializeField]
        private UnityEvent andConditions, orConditions;

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
            
            return (validAnd && validOr);
        }

        private void ResetVars() {
            intVar = 0;
            stringVar = null;
        }

        public void SetIntVar (int i) {
            intVar = i;
        }

        public void SetStringVar (string s) {
            stringVar = s;
        }

        public void IsGreaterThan (int i) {
            isValid = intVar > i;
        }
    }
}