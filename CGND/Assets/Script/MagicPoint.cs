using System;
using UnityEngine;

public class MagicPoint : MonoBehaviour
{
    public static Action<int,string> OnMPChanged;
    public static MagicPoint Instance;
    [Header("MP")]
    private int currentMP;
    [SerializeField] private int maxMP = 5;


    private void Awake() {
        Instance = this;
    }

    void Start()
    {
        currentMP = maxMP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DecreaseMP() {
        currentMP--;
        if (currentMP <= 0) {
            currentMP = 0;
        }
        UpdateMPUI("Decrease");
    }

    public void IncreaseMP() {
        if (maxMP <= currentMP) return;

        currentMP++;
        UpdateMPUI("Increase");

    }

    private void UpdateMPUI(string status) {
        OnMPChanged?.Invoke(currentMP,status);
    }
}
