using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [System.Serializable]
    public class ConditionBase : NodeCache {
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
    }
}