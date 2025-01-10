Shader "Custom/AlwaysOnTopTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { combine texture * primary }
        }
    }
}