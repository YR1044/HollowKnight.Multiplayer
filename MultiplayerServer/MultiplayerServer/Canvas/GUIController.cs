using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerServer.Canvas
{
    public class GUIController : MonoBehaviour
    {
        public GameObject canvas;
        private static GUIController _instance;

        public Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();
        
        public void BuildMenus()
        {
            if (!GameObject.Find("MultiplayerServer Canvas"))
            {
                LoadResources();
                
                canvas = new GameObject("MultiplayerServer Canvas");
                canvas.AddComponent<UnityEngine.Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
                canvas.AddComponent<GraphicRaycaster>();

                Log("Building MultiplayerServer Canvas Menus");
                OptionsPanel.BuildMenu(canvas);

                DontDestroyOnLoad(canvas);
            }
        }

        public static GUIController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GUIController>();
                    if (_instance == null)
                    {
                        Modding.Logger.LogWarn("[Multiplayer Server] Couldn't find GUIController");

                        GameObject GUIController = new GameObject("GUIController Server");
                        _instance = GUIController.AddComponent<GUIController>();
                        DontDestroyOnLoad(GUIController);
                    }

                    Modding.Logger.Log("GUI Controller instance attached to:  " + _instance.gameObject.name);
                }
                return _instance;
            }
        }

        public Font arial;
        public Font perpetua;
        public Font trajanBold;
        public Font trajanNormal;

        private void LoadResources()
        {
            foreach (Font font in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (font != null && font.name == "TrajanPro-Bold")
                {
                    trajanBold = font;
                }

                if (font != null && font.name == "TrajanPro-Regular")
                {
                    trajanNormal = font;
                }

                //Just in case for some reason the computer doesn't have arial
                if (font != null && font.name == "Perpetua")
                {
                    perpetua = font;
                }

                foreach (string fontName in Font.GetOSInstalledFontNames())
                {
                    if (fontName.ToLower().Contains("arial"))
                    {
                        arial = Font.CreateDynamicFontFromOSFont(fontName, 13);
                        break;
                    }
                }
            }
            
            Assembly asm = Assembly.GetExecutingAssembly();
            
            foreach (string res in asm.GetManifestResourceNames())
            {
                if (!res.StartsWith("MultiplayerServer.Images.")) continue;
                
                try
                {
                    using (Stream imageStream = asm.GetManifestResourceStream(res))
                    {
                        byte[] buffer = new byte[imageStream.Length];
                        imageStream.Read(buffer, 0, buffer.Length);

                        Texture2D tex = new Texture2D(2, 2);
                        tex.LoadImage(buffer.ToArray(), true);

                        string[] split = res.Split('.');
                        string internalName = split[split.Length - 2];
                        
                        images.Add(internalName, tex);

                        Log("Loaded image: " + internalName);
                    }
                }
                catch (Exception e)
                {
                    Modding.Logger.LogError("Failed to load image: " + res + "\n" + e);
                }
            }
        }
        
        private void Log(object message) => Modding.Logger.Log("[GUI Controller] " + message);
    }
}