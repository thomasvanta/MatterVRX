// Upgrade NOTE: upgraded instancing buffer 'InstanceProperties' to new syntax.

Shader "Unlit/GPUInstancingShader"
{
    Properties
    {
        _Color ("Color", Color) = (.25, .5, .5, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
            };

            struct v2f
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            UNITY_INSTANCING_BUFFER_START(InstanceProperties)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(InstanceProperties)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                fixed4 col = UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _Color);
                return col;
            }
            ENDCG
        }
    }
}
