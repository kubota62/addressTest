using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResolutionChanger : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SetCanvasResolution(int width, int height);
    
    private record Resolution
    {
        public int Width;
        public int Height;
    }

    [SerializeField] private Button _144PButton;
    [SerializeField] private Button _240PButton;
    [SerializeField] private Button _360PButton;
    [SerializeField] private Button _480PButton;
    [SerializeField] private Button _720PButton;
    [SerializeField] private Button _1080PButton;
    [SerializeField] private Button _1440PButton;
    [SerializeField] private Button _2160PButton;

    private readonly Resolution _144P = new() { Width = 256, Height = 144 };
    private readonly Resolution _240P = new() { Width = 427, Height = 240 };
    private readonly Resolution _360P = new() { Width = 640, Height = 360 };
    private readonly Resolution _480P = new() { Width = 720, Height = 480 };
    private readonly Resolution _720P = new() { Width = 1280, Height = 720 };
    private readonly Resolution _1080P = new() { Width = 1920, Height = 1080 };
    private readonly Resolution _1440P = new() { Width = 2560, Height = 1440 };
    private readonly Resolution _2160P = new() { Width = 3840, Height = 2160 };

    private void Start()
    {
        _144PButton.onClick.AddListener(() => ChangeResolution(_144P.Width, _144P.Height));
        _240PButton.onClick.AddListener(() => ChangeResolution(_240P.Width, _240P.Height));
        _360PButton.onClick.AddListener(() => ChangeResolution(_360P.Width, _360P.Height));
        _480PButton.onClick.AddListener(() => ChangeResolution(_480P.Width, _480P.Height));
        _720PButton.onClick.AddListener(() => ChangeResolution(_720P.Width, _720P.Height));
        _1080PButton.onClick.AddListener(() => ChangeResolution(_1080P.Width, _1080P.Height));
        _1440PButton.onClick.AddListener(() => ChangeResolution(_1440P.Width, _1440P.Height));
        _2160PButton.onClick.AddListener(() => ChangeResolution(_2160P.Width, _2160P.Height));
    }

    private void ChangeResolution(int width, int height)
    {
        Debug.Log($"int width={width}, int height={height}");
        SetCanvasResolution(width, height);
    }
}