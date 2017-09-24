using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;
using System.Reflection;

namespace CM3D2.FaceShapeProp.Plugin
{
    [PluginFilter("CM3D2x64"),
    PluginFilter("CM3D2x86"),
    PluginFilter("CM3D2VRx64"),
    PluginName("FaceShapeProp"),
    PluginVersion("0.0.0.1")]
    public class FaceShapeProp : PluginBase
    {
        private int level;
        private XmlManager xmlManager;

        private enum TargetLevel
        {
            //ダンス1:ドキドキ☆Fallin' Love
            SceneDance_DDFL = 4,

            // エディット
            SceneEdit = 5,

            // 夜伽
            SceneYotogi = 14,

            // ADVパート
            SceneADV = 15,

            // ダンス2:entrance to you
            SceneDance_ETYL = 20,

            // ダンス3:scarlet leap
            SceneDance_SCLP = 22,

            // ダンス4:stellar my tears
            SceneDance_STMT = 26,

            // 撮影モード
            ScenePhot = 27,

            // ダンス5:rhythmix to you
            SceneDance_RYFU = 28,

            // ダンス6:happy!happy!スキャンダル
            SceneDance_HAPY = 30,

            // ダンス6:happy!happy!スキャンダル 豪華版
            SceneDance_HAPYDX = 31,

            // ダンス7:Can Know Two Close
            SceneDance_CKTC = 32,

            SceneDance_SSE = 34,
            SceneDance_SSEDX = 35
        }

        private void Awake()
        {
            xmlManager = new XmlManager();
        }

        private void OnLevelWasLoaded(int level)
        {
            this.level = level;
        }

        private void Update()
        {
            if (!Enum.IsDefined(typeof(TargetLevel), level))
            {
                return;
            }

            Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            if (maid != null && maid.body0.Face.morph != null){
                foreach(ShapeKeyNode shapeKeyNode in xmlManager.shapeKeyNodeList)
                {
                    int h;
                    bool c;
                    float f = maid.body0.Face.morph.BlendValues[(int)maid.body0.Face.morph.hash[shapeKeyNode.baseTags]];

                    for (int j = 0; j < maid.body0.goSlot.Count; j++)
                    {
                        TMorph morph = maid.body0.goSlot[j].morph;
                        if (morph != null)
                        {
                            c = false;
                            if (morph.Contains(shapeKeyNode.propTag))
                            {
                                h = (int)maid.body0.goSlot[j].morph.hash[shapeKeyNode.propTag];
                                maid.body0.goSlot[j].morph.BlendValues[h] = f * shapeKeyNode.propRatio;
                                c = true;
                            }
                            if (c) maid.body0.goSlot[j].morph.FixBlendValues();
                        }
                    }
                    
                }
            }
        }

        private class ShapeKeyNode{
            public string baseTags;
            public string propTag;
            public float propRatio;
            
            public ShapeKeyNode(string baseTags,string propTag,float propRatio)
            {
                this.baseTags =baseTags;
                this.propTag = propTag;
                this.propRatio = propRatio;
            }
        }
        
        //------------------------------------------------------xml--------------------------------------------------------------------
        private class XmlManager
        {
            private string xmlFileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Config\FaceShapeProp.xml";
            private XmlDocument xmldoc = new XmlDocument();
            public List<ShapeKeyNode> shapeKeyNodeList = new List<ShapeKeyNode>();
            
            public XmlManager()
            {
                try{
                    InitXml();
                }
                catch(Exception e){
                    Debug.LogError("ShapeProp.Plugin:" + e.Source + e.Message + e.StackTrace);
                }
            }

            private void InitXml()
            {
                xmldoc.Load(xmlFileName);
                // ShapeKeyList
                XmlNodeList shapeKeyList = xmldoc.GetElementsByTagName("ShapeKey");
                foreach (XmlNode shapeKeyNode in shapeKeyList)
                {
                    shapeKeyNodeList.Add(new ShapeKeyNode(
                             ((XmlElement)shapeKeyNode).GetAttribute("From")
                        ,    ((XmlElement)shapeKeyNode).GetAttribute("To")
                        ,    float.Parse(((XmlElement)shapeKeyNode).GetAttribute("Ratio"))
                    ));
                }
            }
        }

    }
}


