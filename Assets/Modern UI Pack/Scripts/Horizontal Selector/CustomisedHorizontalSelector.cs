using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.MUIP
{
    [RequireComponent(typeof(Animator))]
    public class CustomisedHorizontalSelector : MonoBehaviour
    {
        [System.Serializable] public class SelectorEvent : UnityEvent<int> { }
        [System.Serializable] public class ItemTextChangedEvent : UnityEvent<TMP_Text> { }

        [System.Serializable]
        public class Item
        {
            public string itemTitle = "Item Title";
            public Sprite itemImage;
            public UnityEvent onItemSelect = new UnityEvent();
        }

        public enum GeneratedInputType
        {
            Slider,
            InputField,
            Toggle,
            CustomPrefab
        }

        [Header("Resources")]
        public TextMeshProUGUI label;
        public TextMeshProUGUI labelHelper;

        public Image itemImage;
        public Image itemImageHelper;

        public Transform indicatorParent;
        public GameObject indicatorObject;
        public Animator selectorAnimator;

        public HorizontalLayoutGroup contentLayout;
        public HorizontalLayoutGroup contentLayoutHelper;

        [Header("Generated Inputs")]
        public bool autoGenerateItems = false;
        public int numberOfItems = 3;

        public Transform generatedInputParent;
        public GeneratedInputType inputType = GeneratedInputType.Slider;

        public Slider sliderPrefab;
        public TMP_InputField inputFieldPrefab;
        public Toggle togglePrefab;
        public GameObject customInputPrefab;

        [Header("Saving")]
        public bool enableImage = true;
        public bool saveSelected = false;
        public string saveKey = "My Selector";

        [Header("Settings")]
        public bool enableIndicators = true;
        public bool invokeAtStart;
        public bool invertAnimation;
        public bool loopSelection;

        [Range(0.25f, 2.5f)] public float imageScale = 1;
        [Range(1, 50)] public int contentSpacing = 15;

        public int defaultIndex = 0;
        [HideInInspector] public int index = 0;

        [Header("Items")]
        public List<Item> items = new List<Item>();

        [Header("Events")]
        public SelectorEvent onValueChanged;
        public ItemTextChangedEvent onItemTextChanged;

        void Awake()
        {
            if (selectorAnimator == null)
                selectorAnimator = GetComponent<Animator>();

            if (label == null || labelHelper == null)
            {
                Debug.LogError("<b>[Customised Horizontal Selector]</b> Missing label resources.", this);
                return;
            }

            if (autoGenerateItems)
                GenerateItems();

            SetupSelector();
            UpdateContentLayout();

            if (invokeAtStart && items.Count > 0)
            {
                items[index].onItemSelect.Invoke();
                onValueChanged.Invoke(index);
            }
        }

        void OnEnable()
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DisableAnimator());
        }

        public void SetupSelector()
        {
            if (items.Count == 0)
                return;

            defaultIndex = Mathf.Clamp(defaultIndex, 0, items.Count - 1);

            if (saveSelected)
            {
                string key = "HorizontalSelector_" + saveKey;

                if (PlayerPrefs.HasKey(key))
                    defaultIndex = Mathf.Clamp(PlayerPrefs.GetInt(key), 0, items.Count - 1);
                else
                    PlayerPrefs.SetInt(key, defaultIndex);
            }

            index = defaultIndex;

            UpdateLabelsAndImage(index);

            if (enableIndicators)
                UpdateIndicators();
            else if (indicatorParent != null)
                Destroy(indicatorParent.gameObject);
        }

        public void PreviousItem()
        {
            if (items.Count == 0)
                return;

            if (!loopSelection && index == 0)
                return;

            PrepareAnimation();

            labelHelper.text = label.text;

            if (itemImage != null && itemImageHelper != null && enableImage)
                itemImageHelper.sprite = itemImage.sprite;

            index = index == 0 ? items.Count - 1 : index - 1;

            UpdateLabelsAndImage(index);
            InvokeCurrentItem();

            if (invertAnimation)
                selectorAnimator.Play("Forward");
            else
                selectorAnimator.Play("Previous");

            FinalizeSelection();
        }

        public void NextItem()
        {
            if (items.Count == 0)
                return;

            if (!loopSelection && index == items.Count - 1)
                return;

            PrepareAnimation();

            labelHelper.text = label.text;

            if (itemImage != null && itemImageHelper != null && enableImage)
                itemImageHelper.sprite = itemImage.sprite;

            index = index + 1 >= items.Count ? 0 : index + 1;

            UpdateLabelsAndImage(index);
            InvokeCurrentItem();

            if (invertAnimation)
                selectorAnimator.Play("Previous");
            else
                selectorAnimator.Play("Forward");

            FinalizeSelection();
        }

        public void PreviousClick()
        {
            PreviousItem();
        }

        public void ForwardClick()
        {
            NextItem();
        }

        public void CreateNewItem(string title)
        {
            Item item = new Item();
            item.itemTitle = title;
            items.Add(item);

            UpdateUI();
        }

        public void CreateNewItem(string title, Sprite image)
        {
            Item item = new Item();
            item.itemTitle = title;
            item.itemImage = image;
            items.Add(item);

            UpdateUI();
        }

        public void RemoveItem(string itemTitle)
        {
            Item item = items.Find(x => x.itemTitle == itemTitle);

            if (item == null)
                return;

            items.Remove(item);

            if (index >= items.Count)
                index = Mathf.Max(0, items.Count - 1);

            SetupSelector();
        }

        public void GenerateItems()
        {
            items.Clear();

            for (int i = 0; i < numberOfItems; i++)
            {
                Item item = new Item();
                item.itemTitle = "Item " + (i + 1);
                items.Add(item);
            }

            GenerateInputs();
        }

        public void GenerateInputs()
        {
            if (generatedInputParent == null)
                return;

            foreach (Transform child in generatedInputParent)
                Destroy(child.gameObject);

            for (int i = 0; i < numberOfItems; i++)
            {
                GameObject inputObject = null;

                if (inputType == GeneratedInputType.Slider && sliderPrefab != null)
                    inputObject = Instantiate(sliderPrefab.gameObject, generatedInputParent);

                else if (inputType == GeneratedInputType.InputField && inputFieldPrefab != null)
                    inputObject = Instantiate(inputFieldPrefab.gameObject, generatedInputParent);

                else if (inputType == GeneratedInputType.Toggle && togglePrefab != null)
                    inputObject = Instantiate(togglePrefab.gameObject, generatedInputParent);

                else if (inputType == GeneratedInputType.CustomPrefab && customInputPrefab != null)
                    inputObject = Instantiate(customInputPrefab, generatedInputParent);

                if (inputObject != null)
                    inputObject.name = "Input " + (i + 1);
            }
        }

        public void UpdateUI()
        {
            if (items.Count == 0)
                return;

            index = Mathf.Clamp(index, 0, items.Count - 1);

            selectorAnimator.enabled = true;

            UpdateLabelsAndImage(index);
            UpdateContentLayout();

            if (enableIndicators)
                UpdateIndicators();

            if (gameObject.activeInHierarchy)
                StartCoroutine(DisableAnimator());
        }

        public void UpdateIndicators()
        {
            if (!enableIndicators || indicatorParent == null || indicatorObject == null)
                return;

            foreach (Transform child in indicatorParent)
                Destroy(child.gameObject);

            for (int i = 0; i < items.Count; i++)
            {
                GameObject go = Instantiate(indicatorObject, indicatorParent);
                go.name = items[i].itemTitle;

                Transform onObj = go.transform.Find("On");
                Transform offObj = go.transform.Find("Off");

                if (onObj == null || offObj == null)
                    continue;

                bool isSelected = i == index;

                onObj.gameObject.SetActive(isSelected);
                offObj.gameObject.SetActive(!isSelected);
            }
        }

        public void UpdateContentLayout()
        {
            if (contentLayout != null)
                contentLayout.spacing = contentSpacing;

            if (contentLayoutHelper != null)
                contentLayoutHelper.spacing = contentSpacing;

            if (itemImage != null)
                itemImage.transform.localScale = Vector3.one * imageScale;

            if (itemImageHelper != null)
                itemImageHelper.transform.localScale = Vector3.one * imageScale;

            if (label != null)
            {
                RectTransform labelRect = label.GetComponent<RectTransform>();

                if (labelRect != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(labelRect);

                if (label.transform.parent != null)
                {
                    RectTransform parentRect = label.transform.parent.GetComponent<RectTransform>();

                    if (parentRect != null)
                        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
                }
            }
        }

        void UpdateLabelsAndImage(int targetIndex)
        {
            label.text = items[targetIndex].itemTitle;
            labelHelper.text = label.text;

            onItemTextChanged?.Invoke(label);

            if (itemImage != null && enableImage)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = items[targetIndex].itemImage;
            }

            if (itemImageHelper != null && enableImage)
            {
                itemImageHelper.gameObject.SetActive(true);
                itemImageHelper.sprite = items[targetIndex].itemImage;
            }

            if (!enableImage)
            {
                if (itemImage != null)
                    itemImage.gameObject.SetActive(false);

                if (itemImageHelper != null)
                    itemImageHelper.gameObject.SetActive(false);
            }
        }

        void PrepareAnimation()
        {
            StopCoroutine("DisableAnimator");

            selectorAnimator.enabled = true;
            selectorAnimator.Play(null);
            selectorAnimator.StopPlayback();
        }

        void InvokeCurrentItem()
        {
            items[index].onItemSelect.Invoke();
            onValueChanged.Invoke(index);
        }

        void FinalizeSelection()
        {
            if (saveSelected)
                PlayerPrefs.SetInt("HorizontalSelector_" + saveKey, index);

            if (enableIndicators)
                UpdateIndicatorState();

            if (gameObject.activeInHierarchy)
                StartCoroutine(DisableAnimator());
        }

        void UpdateIndicatorState()
        {
            if (indicatorParent == null)
                return;

            for (int i = 0; i < indicatorParent.childCount; i++)
            {
                GameObject go = indicatorParent.GetChild(i).gameObject;

                Transform onObj = go.transform.Find("On");
                Transform offObj = go.transform.Find("Off");

                if (onObj == null || offObj == null)
                    continue;

                bool isSelected = i == index;

                onObj.gameObject.SetActive(isSelected);
                offObj.gameObject.SetActive(!isSelected);
            }
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            if (selectorAnimator != null)
                selectorAnimator.enabled = false;
        }
    }
}