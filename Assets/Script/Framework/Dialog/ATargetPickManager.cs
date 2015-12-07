using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine.UI;

namespace Framework
{
    public class ATargetPickManager : MonoBehaviour
    {//llllll 叫我zynga的搬运工

        public static ATargetPickManager instance;

        public GameObject uiTargetPickPrefab;

        private List<TargetPick> picks = new List<TargetPick>();

        private bool isPlayState = true;

        private bool isUiFrozen  = false;

        public static bool isShow = true;

        public bool isEnableClick = true;

        public float iconWidth = 54f;

        public float iconHeight = 54f;

        private void Start()
        {
            instance = this;
        }

        private void CheckHideShow()
        {
            bool flag = this.isPlayState && !this.isUiFrozen;
            if (isShow != flag)
            {
                isShow = flag;
                this.SetPicksEnabled(isShow);
            }
        }

        public void SetPicksEnabled(bool enabled)
        {
            foreach (TargetPick pick in this.picks)
            {
                pick.gameObject.SetActive(enabled);
            }
        }

        public void EnablePicksClicking()
        {
            this.isEnableClick = true;
            UpdateCurrentPicks();
        }

        public void DisablePicksClicking()
        {
            this.isEnableClick = false;
            UpdateCurrentPicks();
        }

        private void UpdateCurrentPicks()
        {
            foreach (TargetPick pick in this.picks)
            {
                pick.GetComponent<Button>().enabled = this.isEnableClick;
            }
        }

        public TargetPick SpawnPick(int duration , long endTime , Transform target, float yOffset, string iconName)
        {
            Debug.Assert(uiTargetPickPrefab != null);

            GameObject go               = Instantiate(uiTargetPickPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.parent         = AUIManager.instance.uiCamera.transform;
            FollowTarget followTarget   = go.GetComponent<FollowTarget>();
            followTarget.target         = target;
            followTarget.UIOffset       = yOffset;
            TargetPick pick             = go.GetComponent<TargetPick>();
            pick.duration               = duration;
            pick.endTime                = endTime;
            pick.SetIcon(iconName);
            Button button               = go.GetComponent<Button>();
            ADebug.Assert(button        != null);
            button.enabled              = this.isEnableClick;
            this.picks.Add(pick);
            return pick;
        }

        public void Remove(TargetPick pick)
        {
            Debug.Assert(pick != null);
            if (this.picks.Contains(pick))
            {
                this.picks.Remove(pick);
            }
        }

        
    }
}

