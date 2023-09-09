using System;

using DG.Tweening;

using Sirenix.OdinInspector;

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
    /// <summary>
    /// 按钮缩放数据类
    /// </summary>
    [Serializable]
    public class UIScaleHelper
    {
        [SerializeField, HideInInspector]
        private RectTransform _rectTransform;

        public RectTransform rectTransform => target ? target : _rectTransform;

        [SerializeField]
        [ReadOnly, DisplayAsString, VerticalGroup("btnScale"), LabelText("缩放动画ID")]
        private int tweenId;

        [SerializeField]
        [VerticalGroup("btnScale"), LabelText("缩放目标")]
        private RectTransform target;

        [SerializeField]
        [VerticalGroup("btnScale"), LabelText("动画曲线")]
        private Ease scaleAnimation = Ease.Linear;

        [SerializeField]
        [VerticalGroup("btnScale"), LabelText("动画时长"), SuffixLabel("秒", true)]
        private float duration = 0.02f;

        [SerializeField]
        [VerticalGroup("btnScale"), LabelText("缩放比例")]
        private Vector3 scaleRatio = new Vector3(0.9f, 0.9f, 0.9f);

        [SerializeField, HideInInspector] private Vector3 originalScale;

        public UIScaleHelper(RectTransform rectTransform)
        {
            _rectTransform = rectTransform;

            tweenId = _rectTransform.GetHashCode();
        }

        public void Init() { originalScale = rectTransform.localScale; }

        /// <summary>
        /// 执行缩放
        /// </summary>
        public void In()
        {
            DOTween.Kill(tweenId);

            var targetScale = originalScale;
            targetScale.Scale(scaleRatio);
            rectTransform.DOScale(targetScale, duration).SetEase(scaleAnimation).SetId(tweenId).Play();
        }

        /// <summary>
        /// 重置缩放
        /// </summary>
        public void Out(bool force = false)
        {
            DOTween.Kill(tweenId);

            if (!force)
                rectTransform.DOScale(originalScale, duration).SetEase(scaleAnimation).SetId(tweenId).Play();
            else
                rectTransform.localScale = originalScale;
        }

        /// <summary>
        /// 结束
        /// </summary>
        public void Stop() => DOTween.Kill(tweenId);
    }
}