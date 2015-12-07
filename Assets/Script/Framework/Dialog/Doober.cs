using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
//create by yjx
namespace Framework
{
    public class Doober : MonoBehaviour
    {
        public string resourceType = "coin";

        public int index;

        [SerializeField]
        private Image image;

        public float dispersalTime = 0.15f;

        public float collectionTime = 0.7f;

        public float dooberAutoCollectTime = 0.3f;      //Dooberc出现到开始Collect的时间

        public float pathOffset = 2.0f;     //Doober的Collect时曲线的偏移

        public CollectPath collectPath = CollectPath.arc_down;

        public float dooberSize = 80f;      //设置Doober的大小

        public Transform destination;

        public event TweenCallback dooberCollected;

        public event TweenCallback dooberDropped;

        public bool isDisperseOnMap;

        private bool isCollectible = true;

        private float autoCollectTimeElapsed = 0.0f;



        public float minDistance = 1.2f;

        public float maxDistance = 1.5f;

        public Vector2 dispersalAmount = new Vector2(128f, 64f);

        public float pauseTime;

        public Color textColor = new Color(1f, 1f, 1f);

        public bool enableDooberSize = true;

        public Vector3 customSize = Vector3.zero;

        public void Collect()
        {
            if (this.isCollectible)
            {
                this.dooberCollected += DooberAutoDestroy;
                this.isCollectible = false;
                float num = 0.2f;
                Vector3 pathDestinationVec = Vector3.zero;
                if (destination.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    pathDestinationVec = destination.position;
                }
                else
                {
                    pathDestinationVec = ACamera.WorldToUI(destination.transform.position, 0.0f);
                }
                Vector3 scaleVec = Vector3.one * num;
                Vector3 pathMidVec = (transform.position + pathDestinationVec) / 2.0f;
                Vector3 pathHorizontal = (pathDestinationVec - transform.position).normalized;
                Vector3 pathVertical = Vector3.Cross(pathHorizontal, new Vector3(0.0f, 0.0f, 1.0f).normalized);

                switch (collectPath)
                { 
                    case CollectPath.arc_down:
                        pathMidVec = pathMidVec + pathVertical * this.pathOffset;
                        break;
                    case CollectPath.arc_up:
                        pathMidVec = pathMidVec - pathVertical * this.pathOffset;
                        break;
                }

                Vector3[] path = new Vector3[] { pathMidVec, pathDestinationVec };
                Sequence sequence = DOTween.Sequence();

                sequence.Append(this.transform.DOPath(path, this.collectionTime, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuart));
                sequence.Insert(0.0f, this.image.rectTransform.DOScale(scaleVec, this.collectionTime));
                sequence.AppendCallback(dooberCollected);
                sequence.Play();
            }
        }

        public void Disperse()
        {
            float num = 0.2f;
            Vector3 scaleVec = (Vector3)(this.image.rectTransform.localScale * UnityEngine.Random.Range(1.1f, 1.5f));
            Transform cachedTransform = this.image.rectTransform;
            cachedTransform.localScale = ((Vector3)(cachedTransform.localScale * num));
            Vector3 moveVec = transform.localPosition + new Vector3(((this.index % 2) != 0) ? ((float)(-25 * (this.index % 6))) : ((float)(0x19 * ((this.index + 1) % 6))), UnityEngine.Random.Range(70f, 150f), 0f);
            Sequence sequence = DOTween.Sequence();

            sequence.Append(this.transform.DOLocalMove(moveVec, this.dispersalTime + this.index * 0.15f).SetEase(Ease.OutElastic));
            sequence.Insert(0.0f, this.image.rectTransform.DOScale(scaleVec, this.dispersalTime + this.index * 0.15f));
            if (this.dooberDropped != null)
            {
                sequence.AppendCallback(dooberDropped);
            }
            sequence.Play();
        }

        private void DooberAutoDestroy()
        {
            UnityEngine.Object.Destroy(gameObject);
        }

        private void Start()
        {
            this.image = AUIManager.AddImage(this.gameObject,AUIManager.LoadSprite(this.resourceType)).GetComponent<Image>();

            if (this.enableDooberSize && (this.customSize == Vector3.zero))       //adjust sizeDelta
            {
                Vector2 vector = this.image.rectTransform.sizeDelta;
                if (vector.x > vector.y)
                {
                    vector.y *= this.dooberSize / vector.x;
                    vector.x = this.dooberSize;
                }
                else
                {
                    vector.x *= this.dooberSize / vector.y;
                    vector.y = this.dooberSize;
                }
                this.image.rectTransform.sizeDelta = vector;
            }
            else if (this.customSize != Vector3.zero)
            {
                this.image.rectTransform.localScale = this.customSize;
            }

            //SVParticleEffectManager instance = SVParticleEffectManager.Instance;              //add ParticleEffect to doober
            //GameObject prefab = instance.GetPrefab("Doober_Sparkle_03");
            //if (prefab != null)
            //{
            //    Transform transform = (UnityEngine.Object.Instantiate(prefab) as GameObject).transform;
            //    transform.parent = base.transform;
            //    transform.localPosition = this.image.rectTransform.localPosition;
            //    transform.localRotation = Quaternion.identity;
            //}
            //prefab = instance.GetPrefab("Doober_Godrays");
            //if (prefab != null)
            //{
            //    Transform transform2 = (UnityEngine.Object.Instantiate(prefab) as GameObject).transform;
            //    transform2.parent = this.image.rectTransform;
            //    transform2.localPosition = Vector3.zero;
            //    transform2.localRotation = Quaternion.identity;
            //    transform2.localScale = Vector3.one;
            //}

            if (this.isDisperseOnMap)
            {
                this.Disperse();
            }
            else
            {
                this.dispersalTime = 0.0f;
            }
            
        }

        private void Update()
        {
            this.autoCollectTimeElapsed += Time.deltaTime;
            if (this.isCollectible && (this.autoCollectTimeElapsed > ( this.dispersalTime + this.dooberAutoCollectTime + (this.index * 0.15f))))
            {
                this.Collect();
            }
        }
    }

    public enum CollectPath
    { 
        arc_up,
        arc_down
    }
}