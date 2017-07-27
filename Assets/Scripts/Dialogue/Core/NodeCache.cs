﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue {
    [System.Serializable]
    public class NodeCache : MonoBehaviour {
        #region Variable Caching

        private int intIndex;
        private int stringIndex;
        private int boolIndex;
        protected GameItem itemVar;

        protected int IntVar {
            get { return Globals.GetGlobal((GlobalInt)intIndex); }
            set { Globals.SetGlobal((GlobalInt)intIndex, value); }
        }

        protected string StringVar {
            get { return Globals.GetGlobal((GlobalString)stringIndex); }
            set { Globals.SetGlobal((GlobalString)stringIndex, value); }
        }

        protected bool BoolVar {
            get { return Globals.GetGlobal((GlobalBool)boolIndex); }
            set { Globals.SetGlobal((GlobalBool)boolIndex, value); }
        }

        protected void ResetVars() {
            intIndex = stringIndex = boolIndex = -1;
        }

        [EnumAction(typeof(GlobalInt))]
        public void SetIntVar(int i) {
            intIndex = i;
        }

        [EnumAction(typeof(GlobalString))]
        public void SetStringVar(int i) {
            stringIndex = i;
        }

        [EnumAction(typeof(GlobalBool))]
        public void SetBoolVar(int i) {
            boolIndex = i;
        }

        public void SetItemVar(GameItem item) {
            itemVar = item;
        }

        #endregion
    }
}