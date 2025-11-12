Shader "Custom/OceanWithNormal"
{
    Properties
    {
        _Color ("Cor Principal", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalScale ("Intensidade do Normal Map", Range(0, 2)) = 1
        _WaveSpeed ("Velocidade das Ondas", Float) = 1.0
        _WavePower ("Força das Ondas", Float) = 0.5
        _Shininess ("Brilho (Especularidade)", Range (0.03, 1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 300

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 TtoW0 : TEXCOORD2;
                float3 TtoW1 : TEXCOORD3;
                float3 TtoW2 : TEXCOORD4;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // Texturas
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            float4 _MainTex_ST;
            float4 _Color;

            // Parâmetros
            float _NormalScale;
            float _WaveSpeed;
            float _WavePower;
            float _Shininess;

            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // --- Animação das ondas ---
                float waveX = cos(_Time.y * _WaveSpeed + input.positionOS.z);
                float waveZ = sin(_Time.y * _WaveSpeed + input.positionOS.x);
                input.positionOS.y += (waveX + waveZ) * _WavePower;

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);

                // UV
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                // Matrizes de Tangente / Normal / Binormal para espaço tangente
                float3 worldNormal = normalize(TransformObjectToWorldNormal(input.normalOS));
                float3 worldTangent = normalize(TransformObjectToWorldDir(input.tangentOS.xyz));
                float3 worldBinormal = cross(worldNormal, worldTangent) * input.tangentOS.w;

                output.TtoW0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
                output.TtoW1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
                output.TtoW2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);

                return output;
            }

            float4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Textura base e cor
                float4 albedoColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;

                // --- Normal Map ---
                float4 normalSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv);
                float3 tangentNormal = UnpackNormal(normalSample);
                tangentNormal.xy *= _NormalScale;

                float3x3 TBN = float3x3(input.TtoW0, input.TtoW1, input.TtoW2);
                float3 worldNormal = normalize(mul(TBN, tangentNormal));

                // --- Iluminação Principal ---
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                float3 lightDir = normalize(mainLight.direction);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - input.positionWS);

                float diffTerm = max(0, dot(worldNormal, lightDir));
                float3 diffuse = diffTerm * mainLight.color * albedoColor.rgb;

                // --- Especular (brilho da água) ---
                float3 halfwayDir = normalize(lightDir + viewDir);
                float specTerm = pow(max(0, dot(worldNormal, halfwayDir)), _Shininess * 128);
                float3 specular = specTerm * mainLight.color;

                // --- Ambiente ---
                float3 ambient = SampleSH(worldNormal) * albedoColor.rgb;

                // --- Cor final ---
                float3 finalColor = ambient + diffuse + specular;

                return float4(finalColor, albedoColor.a);
            }
            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
