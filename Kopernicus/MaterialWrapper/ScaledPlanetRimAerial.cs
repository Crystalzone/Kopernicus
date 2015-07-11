// Material wrapper generated by shader translator tool
using System;
using System.Reflection;
using UnityEngine;

namespace Kopernicus
{
    namespace MaterialWrapper
    {
        public class ScaledPlanetRimAerial : ScaledPlanetSimple
        {
            // Internal property ID tracking object
            protected new class Properties : ScaledPlanetSimple.Properties
            {
                // Return the shader for this wrapper
                private new const string shaderName = "Terrain/Scaled Planet (RimAerial)";
                public static new Shader shader
                {
                    get { return Shader.Find (shaderName); }
                }

                // Rim Power, default = 3
                private const string rimPowerKey = "_rimPower";
                public int rimPowerID { get; private set; }

                // Rim Blend, default = 1
                private const string rimBlendKey = "_rimBlend";
                public int rimBlendID { get; private set; }

                // RimColorRamp, default = "white" {}
                private const string rimColorRampKey = "_rimColorRamp";
                public int rimColorRampID { get; private set; }

                // LightDirection, default = (1,0,0,0)
                private const string localLightDirectionKey = "_localLightDirection";
                public int localLightDirectionID { get; private set; }

                // Singleton instance
                private static Properties singleton = null;
                public static new Properties Instance
                {
                    get
                    {
                        // Construct the singleton if it does not exist
                        if(singleton == null)
                            singleton = new Properties();
            
                        return singleton;
                    }
                }

                protected Properties() : base()
                {
                    rimPowerID = Shader.PropertyToID(rimPowerKey);
                    rimBlendID = Shader.PropertyToID(rimBlendKey);
                    rimColorRampID = Shader.PropertyToID(rimColorRampKey);
                    localLightDirectionID = Shader.PropertyToID(localLightDirectionKey);
                }
            }

            // Rim Power, default = 3
            public float rimPower
            {
                get { return GetFloat (Properties.Instance.rimPowerID); }
                set { SetFloat (Properties.Instance.rimPowerID, value); }
            }

            // Rim Blend, default = 1
            public float rimBlend
            {
                get { return GetFloat (Properties.Instance.rimBlendID); }
                set { SetFloat (Properties.Instance.rimBlendID, value); }
            }

            // RimColorRamp, default = "white" {}
            public Texture2D rimColorRamp
            {
                get { return GetTexture (Properties.Instance.rimColorRampID) as Texture2D; }
                set { SetTexture (Properties.Instance.rimColorRampID, value); }
            }

            // LightDirection, default = (1,0,0,0)
            public Vector4 localLightDirection
            {
                get { return GetVector (Properties.Instance.localLightDirectionID); }
                set { SetVector (Properties.Instance.localLightDirectionID, value); }
            }

            public ScaledPlanetRimAerial() : base()
            {
                base.shader = Properties.shader;
            }

            public ScaledPlanetRimAerial(string contents) : base(contents)
            {
                base.shader = Properties.shader;
            }

            public ScaledPlanetRimAerial(Material material) : base(material)
            {
                // Copy the shader
                base.shader = Properties.shader;
            }

        }
    }
}
