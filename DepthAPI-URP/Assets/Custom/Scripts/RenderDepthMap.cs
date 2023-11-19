using UnityEngine;
using UnityEngine.UI;
using Meta.XR.Depth;
using TMPro;
using UnityEditor;

namespace SolerSoft.Customization
{
    /// <summary>
    /// Renders a Meta depth map into Unity UI
    /// </summary>
    public class RenderDepthMap : MonoBehaviour
    {
        #region Constants
        private static readonly int VirtualDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");
        #endregion // Constants

        #region Unity Inspector Variables
        [SerializeField]
        [Tooltip("The RawImage where the physical depth map will be displayed.")]
        private RawImage m_physicalDepthImage;

        [SerializeField]
        [Tooltip("The RawImage where the virtual depth map will be displayed.")]
        private RawImage m_virtualDepthImage;

        [SerializeField]
        [Tooltip("The Text control to display when depth information is missing.")]
        private TextMeshProUGUI m_missingDepthText;

        [SerializeField]
        [Tooltip("The depth texture provider.")]
        private EnvironmentDepthTextureProvider m_depthTextureProvider;
        #endregion // Unity Inspector Variables

        #region Private Methods
        /// <summary>
        /// Attempts to get any unassigned components.
        /// </summary>
        /// <returns>
        /// <c>true</c> if all components were satisfied; otherwise <c>false</c>.
        /// </returns>
        private bool TryGetComponents()
        {
            // Find if missing
            if (m_missingDepthText == null) { m_missingDepthText = GetComponent<TextMeshProUGUI>(); }
            if (m_depthTextureProvider == null) { m_depthTextureProvider = GetComponent<EnvironmentDepthTextureProvider>(); }

            // All satisfied?
            return m_physicalDepthImage != null && m_virtualDepthImage != null && m_missingDepthText != null && m_depthTextureProvider != null;
        }

        /// <summary>
        /// Attempts to show the depth textures.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the textures were shown; otherwise <c>false</c>.
        /// </returns>
        private bool TryShowTextures()
        {
            // Attempt to get the global depth texture
            var physicalDepthTex = Shader.GetGlobalTexture(EnvironmentDepthTextureProvider.DepthTextureID);
            var virtualDepthTex = Shader.GetGlobalTexture(VirtualDepthTextureID);

            // Show based on success
            if ((physicalDepthTex != null) && (virtualDepthTex != null))
            {
                // Hide text
                m_missingDepthText.enabled = false;

                // Show images
                m_physicalDepthImage.enabled = true;
                m_virtualDepthImage.enabled = true;

                // Link images
                m_physicalDepthImage.texture = physicalDepthTex;
                m_virtualDepthImage.texture = virtualDepthTex;

                // Success
                return true;
            }
            else
            {
                // Hide image
                m_physicalDepthImage.enabled = false;
                m_virtualDepthImage.enabled = false;

                // Show text
                m_missingDepthText.enabled = true;

                // Failed
                return false;
            }
        }
        #endregion // Private Methods

        #region Unity Message Handlers
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        protected void Start()
        {
            if (!TryGetComponents())
            {
                Debug.LogError("Missing components, disabling.");
                enabled = false;
            }
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected void Update()
        {
            // Attempt to show the render textures
            TryShowTextures();
        }
        #endregion // Unity Message Handlers
    }
}