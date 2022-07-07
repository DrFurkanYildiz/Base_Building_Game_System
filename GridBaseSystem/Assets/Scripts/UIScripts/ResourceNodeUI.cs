using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ResourceNodeUI : MonoBehaviour
{
    public static ResourceNodeUI Instance { get; private set; }

    private ResourceNode resourceNode;
    private TextMeshProUGUI resourceItemText;


    private void Awake()
    {
        Instance = this;

        transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(Hide);
        transform.Find("WorkerSelected").Find("ArttirBtn").GetComponent<Button>().onClick.AddListener(WorkerEkle);
        transform.Find("WorkerSelected").Find("EksiltBtn").GetComponent<Button>().onClick.AddListener(WorkerCikar);
        resourceItemText = transform.Find("ResourceItem").Find("Text").GetComponent<TextMeshProUGUI>();

        Hide();
    }

    /// <summary>
    /// Ekleme çıkarma bölümleri manager üzeirnde halledilecek ve değişecek
    /// </summary>

    private void WorkerEkle()
    {
        if (GathererManager.Instance.EmptyWorkerAmount() > 0)
        {
            resourceNode.workers.Add(GathererManager.Instance.GetEmptyWorker());
            UpdateText();
        }
    }
    private void WorkerCikar()
    {
        if (resourceNode.workers.Count == 0)
            return;

        resourceNode.workers[0].TargetResourceNode = null;
        resourceNode.workers.RemoveAt(0);
        UpdateText();
    }

    private void ResourceNode_OnItemStorageCountChanged(object sender, EventArgs e)
    {
        UpdateText();
    }
    private void UpdateText()
    {
        if (resourceNode != null)
        {
            transform.Find("WorkerSelected").Find("Text").GetComponent<TextMeshProUGUI>().text = resourceNode.workers.Count.ToString();
            resourceItemText.text = resourceNode.GetTotalAvailableReseurce().ToString();
        }
    }


    public void Show(ResourceNode resourceNode)
    {
        gameObject.SetActive(true);

        if (this.resourceNode != null)
        {
            this.resourceNode.OnItemStorageCountChanged -= ResourceNode_OnItemStorageCountChanged;
        }

        this.resourceNode = resourceNode;

        if (resourceNode != null)
        {
            transform.Find("ResourceItem").Find("Item").GetComponent<Image>().sprite = resourceNode.GetResourceItem()?.sprite;
            resourceNode.OnItemStorageCountChanged += ResourceNode_OnItemStorageCountChanged;
        }

        UpdateText();
    }
    private void Hide()
    {
        gameObject.SetActive(false);

        if (this.resourceNode != null)
        {
            this.resourceNode.OnItemStorageCountChanged -= ResourceNode_OnItemStorageCountChanged;
        }

        resourceNode = null;
    }
}
