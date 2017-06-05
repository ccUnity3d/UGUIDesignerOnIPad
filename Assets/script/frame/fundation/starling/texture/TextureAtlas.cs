using foundation;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace starling
{
    public class TextureAtlas
    {
        private Dictionary<string, TextureInfo> mTextureInfos;
        private Texture mAtlasTexture;

        public TextureAtlas(Texture texture, XmlDocument atlasXml = null)
        {
            mTextureInfos = new Dictionary<string, TextureInfo>();
            mAtlasTexture = texture;

            if (atlasXml != null)
            {
                parseAtlasXml(atlasXml);
            }
        }

        public string cleanMasterString(string str)
        {
            return ("_" + str).Substring(1);
        }

        private void parseAtlasXml(XmlDocument atlasXml)
        {
            float scale = Convert.ToSingle(atlasXml.Attributes["scale"]);
          
            XmlNodeList nodeList = atlasXml.SelectNodes("SubTExture");
            foreach (XmlNode subTexture in nodeList)
            {
                string name = cleanMasterString(subTexture.Attributes["name"].Value);
                float x = Convert.ToSingle(subTexture.Attributes["x"].Value)/scale;
                float y = Convert.ToSingle(subTexture.Attributes["y"].Value)/scale;

                float width = Convert.ToSingle(subTexture.Attributes["width"].Value)/scale;
                float height = Convert.ToSingle(subTexture.Attributes["height"].Value)/scale;
                float frameX = Convert.ToSingle(subTexture.Attributes["frameX"].Value)/scale;
                float frameY = Convert.ToSingle(subTexture.Attributes["frameY"].Value)/scale;
                float frameWidth = Convert.ToSingle(subTexture.Attributes["frameWidth"].Value)/scale;
                float frameHeight = Convert.ToSingle(subTexture.Attributes["frameHeight"].Value)/scale;
                bool rotated = Convert.ToBoolean(subTexture.Attributes["rotated"].Value);
           

                RectangleX region = new RectangleX(x, y, width, height);
                RectangleX frame = frameWidth > 0 && frameHeight > 0
                    ? new RectangleX(frameX, frameY, frameWidth, frameHeight)
                    : null;

                addRegion(name, region, frame, rotated);
            }

        }

        public void addRegion(string name, RectangleX region, RectangleX frame = null,bool rotated = false)
        {
            if (mTextureInfos.ContainsKey(name))
            {
                return;
            }
            mTextureInfos.Add(name, new TextureInfo(region, frame, rotated));
            
        }

        /** Removes a region with a certain name. */

        public void removeRegion(string name)
        {
            if (mTextureInfos.ContainsKey(name))
            { 
                mTextureInfos.Remove(name);
            }

        }
    }

    internal class TextureInfo
    {
        public RectangleX region;
        public RectangleX frame;
        public bool rotated;
    
        public TextureInfo(RectangleX region, RectangleX frame, bool rotated)
        {
            this.region = region;
            this.frame = frame;
            this.rotated = rotated;
        }
    }
}
