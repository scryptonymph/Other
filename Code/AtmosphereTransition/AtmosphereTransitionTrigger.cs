using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AtmosphereTransition
{
    [RequireComponent(typeof(BoxCollider))]
    public class AtmosphereTransitionTrigger : MonoBehaviour
    {
        [Header("Common options")]
        [Tooltip("Needs to be a trigger collider.")]
        [SerializeField] private BoxCollider boxCollider;

        [Header("Skybox before transition")]
        #if UNITY_EDITOR
        // The if block wrapper is required or else using the MessageType param will crash any builds.
        [Help("Source Skybox Material is always the same as in Render Settings!", UnityEditor.MessageType.Info)]
        #endif
        [ReadOnly] [SerializeField] private Material sourceSkyboxMaterial;

        [SerializeField] private Color sourceTint;

        [Range(0, 8)]
        [SerializeField] private float sourceExposure;

        [Range(0, 360)]
        [SerializeField] private float sourceRotation;

        [Header("Lighting before transition")]
        [SerializeField] private bool fog;

        [SerializeField] private Color sourceFogColor;

        [Range(0, 1)]
        [SerializeField] private float sourceFogDensity;

        [Header("Skybox after transition")]
        #if UNITY_EDITOR
        // The if block wrapper is required or else using the MessageType param will crash any builds.
        [Help("Change Target Skybox Material if you wish to blend skyboxes during transition. \r\n" +
            "Get Material Values gets the original target skybox material values. \r\n", UnityEditor.MessageType.Info)]
        #endif
        [SerializeField] private Material targetSkyboxMaterial;

        [SerializeField] private bool getMaterialValues = false;

        [SerializeField] private Color targetTint;

        [Range(0, 8)]
        [SerializeField] private float targetExposure;

        [Range(0, 360)]
        [SerializeField] private float targetRotation;

        [Header("Lighting after transition")]
        [SerializeField] private Color targetFogColor;

        [Range(0, 1)]
        [SerializeField] private float targetFogDensity;

        [Header("Time it takes to transition")]
        [SerializeField] private float duration = 5f;

        [HideInInspector]
        [SerializeField] private Material skyboxMaterial;

        [HideInInspector]
        [SerializeField] private string skyboxProperty;
        
        private Color tint;
        private float exposure;
        private float rotation;

        private Color fogColor;
        private float fogDensity;

        private float elapsed;
        private float transitionDirection;
        private bool isTransitioning;

        private void Awake()
        {
            skyboxMaterial = RenderSettings.skybox;
            sourceSkyboxMaterial = RenderSettings.skybox;

            tint = sourceTint;
            exposure = sourceExposure;
            rotation = sourceRotation;

            GetTintProperty();

            if (targetSkyboxMaterial != sourceSkyboxMaterial)
            {
                ChangeShader("Skybox/SkyboxBlended", true);
            }
        }

        private void Reset()
        {
            boxCollider = this.GetComponent<BoxCollider>();

            skyboxMaterial = RenderSettings.skybox;
            sourceSkyboxMaterial = RenderSettings.skybox;
            targetSkyboxMaterial = RenderSettings.skybox;

            GetTintProperty();

            sourceTint = sourceSkyboxMaterial.GetColor(skyboxProperty);
            sourceExposure = sourceSkyboxMaterial.GetFloat("_Exposure");
            sourceRotation = sourceSkyboxMaterial.GetFloat("_Rotation");

            targetTint = targetSkyboxMaterial.GetColor(skyboxProperty);
            targetExposure = targetSkyboxMaterial.GetFloat("_Exposure");
            targetRotation = targetSkyboxMaterial.GetFloat("_Rotation");

            fog = RenderSettings.fog;

            sourceFogColor = RenderSettings.fogColor;
            sourceFogDensity = RenderSettings.fogDensity;
        }

        private void OnValidate()
        {
            if (getMaterialValues) {
                targetTint = targetSkyboxMaterial.GetColor(skyboxProperty);
                targetExposure = targetSkyboxMaterial.GetFloat("_Exposure");
                targetRotation = targetSkyboxMaterial.GetFloat("_Rotation");

                getMaterialValues = false;
            }
        }

        // Because if using the Procedural shader, the property is _SkyTint instead of _Tint.
        private void GetTintProperty()
        {
            if (sourceSkyboxMaterial.HasProperty("_Tint"))
            {
                skyboxProperty = "_Tint";
            } else if (sourceSkyboxMaterial.HasProperty("_SkyTint"))
            {
                skyboxProperty = "_SkyTint";
            }
        }

        public void InitializationSetup()
        {
            boxCollider = this.GetComponent<BoxCollider>();

            boxCollider.isTrigger = true;

            boxCollider.size = new Vector3(10f, 5f, 10f);
            boxCollider.center = new Vector3(0f, 2.5f, 0f);

            Reset();
        }
        
        private void OnTriggerEnter(Collider _other)
        {
            if (_other.CompareTag("Player"))
            {
                isTransitioning = true;

                transitionDirection = 1;
            }
        }
        
        private void OnTriggerExit(Collider _other)
        {
            if (_other.CompareTag("Player"))
            {
                isTransitioning = true;

                transitionDirection = -1;
            }
        }
        private void Update()
        {
            if (!isTransitioning)
            {
                return;
            }

            elapsed += transitionDirection * Time.deltaTime;
            elapsed = Mathf.Clamp(elapsed, 0, duration);

            var lerp = elapsed / duration;
            
            SetValues(lerp);

            if ((elapsed == duration) || (elapsed == 0))
            {
                isTransitioning = false;
            }
        }

        private void SetValues(float _lerpTime)
        {
            tint = Color.Lerp(sourceTint, targetTint, _lerpTime);
            exposure = Mathf.Lerp(sourceExposure, targetExposure, _lerpTime);
            rotation = Mathf.Lerp(sourceRotation, targetRotation, _lerpTime);

            if (fog)
            {
                RenderSettings.fogColor = Color.Lerp(sourceFogColor, targetFogColor, _lerpTime);
                RenderSettings.fogDensity = Mathf.Lerp(sourceFogDensity, targetFogDensity, _lerpTime);
            }

            skyboxMaterial.SetColor(skyboxProperty, tint);
            skyboxMaterial.SetFloat("_Exposure", exposure);
            skyboxMaterial.SetFloat("_Rotation", rotation);

            skyboxMaterial.SetFloat("_Blend", _lerpTime);
        }

        private void ChangeShader(string _shaderName, bool _blendSkyboxes)
        {
            skyboxMaterial.shader = Shader.Find(_shaderName);

            skyboxMaterial.SetTexture("_FrontTex", sourceSkyboxMaterial.GetTexture("_FrontTex"));
            skyboxMaterial.SetTexture("_BackTex", sourceSkyboxMaterial.GetTexture("_BackTex"));
            skyboxMaterial.SetTexture("_LeftTex", sourceSkyboxMaterial.GetTexture("_LeftTex"));
            skyboxMaterial.SetTexture("_RightTex", sourceSkyboxMaterial.GetTexture("_RightTex"));
            skyboxMaterial.SetTexture("_UpTex", sourceSkyboxMaterial.GetTexture("_UpTex"));
            skyboxMaterial.SetTexture("_DownTex", sourceSkyboxMaterial.GetTexture("_DownTex"));

            if (_blendSkyboxes)
            {
                skyboxMaterial.SetTexture("_FrontTex2", targetSkyboxMaterial.GetTexture("_FrontTex"));
                skyboxMaterial.SetTexture("_BackTex2", targetSkyboxMaterial.GetTexture("_BackTex"));
                skyboxMaterial.SetTexture("_LeftTex2", targetSkyboxMaterial.GetTexture("_LeftTex"));
                skyboxMaterial.SetTexture("_RightTex2", targetSkyboxMaterial.GetTexture("_RightTex"));
                skyboxMaterial.SetTexture("_UpTex2", targetSkyboxMaterial.GetTexture("_UpTex"));
                skyboxMaterial.SetTexture("_DownTex2", targetSkyboxMaterial.GetTexture("_DownTex"));
            }
        }

        private void OnDestroy()
        {
            ChangeShader("Skybox/6 Sided", false);

            SetValues(0);
        }
    }
}
