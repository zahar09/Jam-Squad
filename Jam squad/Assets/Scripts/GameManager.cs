using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform[] cells;
    [SerializeField] private Transform[] hormonesContainers;
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    [SerializeField] private ArrowPointer arrow;

    private bool isFirstActivation = true;

    public void ActivateCell(int cellIndex)
    {
        if (!ValidateIndex(cellIndex)) return;

        if (isFirstActivation)
        {
            DetachAllObjects();
            isFirstActivation = false;
        }

        Transform cell = cells[cellIndex];
        cell.gameObject.SetActive(true);

        Vector3 targetScale = cell.localScale;
        cell.localScale = Vector3.zero;
        cell.DOScale(targetScale, scaleDuration)
            .SetEase(scaleEase)
            .OnComplete(() => ReleaseHormones(cellIndex));

        arrow.gameObject.SetActive(true);
        arrow.SetTarget(cell);
    }

    private void DetachAllObjects()
    {
        foreach (Transform cell in cells)
        {
            cell.SetParent(null);
        }

        foreach (Transform container in hormonesContainers)
        {
            container.SetParent(null);
        }
    }

    private void ReleaseHormones(int hormoneIndex)
    {
        if (!ValidateHormoneIndex(hormoneIndex)) return;

        Transform container = hormonesContainers[hormoneIndex];
        container.gameObject.SetActive(true);

        foreach (Transform hormone in container)
        {
            Vector3 targetScale = hormone.localScale;
            hormone.localScale = Vector3.zero;
            hormone.gameObject.SetActive(true);
            hormone.DOScale(targetScale, scaleDuration)
                .SetEase(scaleEase);
        }
    }

    private bool ValidateIndex(int index)
    {
        if (index >= 0 && index < cells.Length) return true;
        //Debug.LogError($"Invalid cell index: {index}");
        return false;
    }

    private bool ValidateHormoneIndex(int index)
    {
        if (index >= 0 && index < hormonesContainers.Length) return true;
        Debug.LogError($"Invalid hormone container index: {index}");
        return false;
    }
}