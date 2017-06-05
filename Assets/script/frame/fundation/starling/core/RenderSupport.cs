using System.Collections.Generic;
using UnityEngine;

namespace starling
{
    public class RenderSupport
    {
        private string mBlendMode;
        private int mDrawCount;

        private Camera _camera;
        internal GameObject uiRoot;

        private List<QuadBatch> mQuadBatches = new List<QuadBatch>();

        private int mCurrentQuadBatchID;

        public RenderSupport()
        {
            mQuadBatches.Add(new QuadBatch());
            uiRoot = new GameObject("UI Root");
            uiRoot.transform.position = new Vector3(0, -50);

 
            GameObject uiCamara = new GameObject("UI Camera");
            _camera = uiCamara.AddComponent<Camera>();
            _camera.transform.parent = uiRoot.transform;

            Vector3 position = new Vector3(0, 0, -10);
            _camera.transform.localPosition = position;
            _camera.orthographic = true;
            _camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            _camera.clearFlags = CameraClearFlags.Depth;
            _camera.orthographicSize = 1f;
        }

        public void nextFrame()
        {
            mCurrentQuadBatchID = 0;
            mBlendMode = BlendModeX.NORMAL;
            mDrawCount = 0;   
        }

        public string blendMode
        {
            get { return mBlendMode; }
            set { if (value != BlendModeX.AUTO) mBlendMode = value; }
        }

        public void batchQuad(Quad quad, float parentAlpha,TextureX texture)
        {
            if (mQuadBatches[mCurrentQuadBatchID].isStateChange(quad, parentAlpha,1,texture))
            {
                finishQuadBatch();
            }

            mQuadBatches[mCurrentQuadBatchID].addQuad(quad, parentAlpha,texture);
        }

        public void finishQuadBatch()
        {
            QuadBatch currentBatch = mQuadBatches[mCurrentQuadBatchID];

            if (currentBatch.numQuads != 0)
            {
                currentBatch.render(mDrawCount,this);
            }
            currentBatch.reset();

            ++mCurrentQuadBatchID;
            ++mDrawCount;

            if (mQuadBatches.Count <= mCurrentQuadBatchID)
            {
                mQuadBatches.Add(new QuadBatch());
            }
        }
    }
}