Shader "Custom/CycleMask"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_CenterX ("CenterX", Range(0, 1)) = 0.5//中心点x
		_CenterY ("CenterY", Range(0, 1)) = 0.5//中心点y
		_Ridus("R", Range(0, 1)) = 0.5//半径
		_Ran("Ran", Range(0,0.1)) = 0.05//渐变宽度半径
		
		_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}
	SubShader
	{
		Tags { 
			
		        "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
		}

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
		
		Cull Off
		Pass
		{
			//关闭深度
			ZWrite Off  
            //常规透明
			Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			
			#include "UnityCG.cginc"
            #include "UnityUI.cginc"


 
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
 
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
                float4 worldPosition : TEXCOORD1;
			};
 
			//声明变量
			float _Ran;
			float _Ridus;
			float _CenterX;
			float _CenterY;
			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPosition = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
 
				fixed4 col = tex2D(_MainTex, i.uv);//纹理采样
				float val = (i.uv.x - _CenterX)*(i.uv.x - _CenterX) + (i.uv.y - _CenterY)* (i.uv.y - _CenterY);//计算标准圆形公式左值
				if(val > _Ridus * _Ridus)//对点是否在圆内做判断
				{
					float dis = sqrt(val) - _Ridus;//获取超出半径偏移量
					if(dis < _Ran){
                        if(_Ridus <= 0)
                        {
                            return fixed4(0,0,0,1);//超出则返回黑色;
                        }else{
						// #ifdef UNITY_UI_CLIP_RECT
                        // 	col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                    	// #endif

                    	// #ifdef UNITY_UI_ALPHACLIP
                        // 	clip (col.a - 0.001);
                    	// #endif
						col = fixed4(0,0,0,1);
						col *= saturate(1 - ((_Ran - dis)/_Ran));
						return col;//超出半径偏移量小于限定渐变半径，则将该值归一化，并和采样颜色相乘
                        }
                    }
					else{
					    return fixed4(0,0,0,1);//超出则返回黑色
                    }
				}
				else
				{
					col.a = 0;
					return col;
				}
			}
			ENDCG
		}
	}
}