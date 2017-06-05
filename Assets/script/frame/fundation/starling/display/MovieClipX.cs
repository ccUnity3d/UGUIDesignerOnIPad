using foundation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace starling
{
    public class MovieClipX : Image
    {
        private List<TextureX> mTextures;
        //private List<AudioSource> mSounds;
        private List<float> mDurations;
        private List<float> mStartTimes;
        private float mDefaultFrameDuration;
        private float mCurrentTime;
        private int mCurrentFrame;
        private bool mLoop;
        private bool mPlaying;
        private bool mMuted=false;

        public MovieClipX(List<TextureX> textures, int fps = 12) : base(textures[0])
        {
            init(textures, fps);
        }

        protected void init(List<TextureX> textures, int fps = 12)
        {
            if (fps <= 0) throw new ArgumentException("Invalid fps: " + fps);
            int numFrames = textures.Count;

            mDefaultFrameDuration = 1.0f/fps;
            mLoop = true;
            mPlaying = true;
            mCurrentTime = 0.0f;
            mCurrentFrame = 0;
            mTextures = textures;
            //mSounds = new List<AudioSource>(numFrames);
 
            mDurations = new List<float>(numFrames);
            mStartTimes = new List<float>(numFrames);

            for (int i = 0; i < numFrames; ++i)
            {
                mDurations.Add(mDefaultFrameDuration);
                mStartTimes.Add(i*mDefaultFrameDuration);
                //mSounds.Add(null);
            }
        }

        public int numFrames
        {
            get { return mTextures.Count; }
        }

        public int currentFrame
        {
            get { return mCurrentFrame; }

            set
            {
                mCurrentFrame = value;
                mCurrentTime = 0.0f;

                for (int i = 0; i < value; ++i)
                {
                    mCurrentTime += getFrameDuration(i);
                }
                texture = mTextures[mCurrentFrame];
                /*if (false == mMuted && mSounds[mCurrentFrame])
                {
                    mSounds[mCurrentFrame].Play();
                }*/
            }
        }

        public float getFrameDuration(int frameID)
        {
            if (frameID < 0 || frameID >= numFrames) throw new ArgumentException("Invalid frame id");
            return mDurations[frameID];
        }

        public float totalTime
        {
            get
            {
                int numFrames = mTextures.Count;
                return mStartTimes[(numFrames - 1)] + mDurations[(numFrames - 1)];
            }
        }


        public void advanceTime()
        {
            if (!mPlaying) return;

            int finalFrame;
            int previousFrame = mCurrentFrame;
            float restTime = 0.0f;
            bool breakAfterFrame = false;
            bool dispatchCompleteEvent = false;
            float totalTime = this.totalTime;

            if (mLoop && mCurrentTime >= totalTime)
            {
                mCurrentTime = 0.0f;
                mCurrentFrame = 0;
            }

            if (mCurrentTime < totalTime)
            {
                mCurrentTime += Time.deltaTime;
                finalFrame = mTextures.Count - 1;

                while (mCurrentTime > mStartTimes[mCurrentFrame] + mDurations[mCurrentFrame])
                {
                    if (mCurrentFrame == finalFrame)
                    {
                        if (mLoop && !hasEventListener(EventX.COMPLETE))
                        {
                            mCurrentTime -= totalTime;
                            mCurrentFrame = 0;
                        }
                        else
                        {
                            breakAfterFrame = true;
                            restTime = mCurrentTime - totalTime;
                            dispatchCompleteEvent = true;
                            mCurrentFrame = finalFrame;
                            mCurrentTime = totalTime;
                        }
                    }
                    else
                    {
                        mCurrentFrame++;
                    }

                    /*AudioSource sound = mSounds[mCurrentFrame];
                    if (sound != null && !mMuted) sound.Play();*/
                    if (breakAfterFrame)
                    {
                        break;
                    }
                }

                // special case when we reach *exactly* the total time.
                if (mCurrentFrame == finalFrame && mCurrentTime == totalTime)
                {
                    dispatchCompleteEvent = true;
                }
            }

            if (mCurrentFrame != previousFrame)
                texture = mTextures[mCurrentFrame];

            if (dispatchCompleteEvent)
            {
                simpleDispatch(EventX.COMPLETE);
            }
            if (mLoop && restTime > 0.0)
            {
                advanceTime();
            }
        }


        public void play()
        {
            mPlaying = true;

            TickManager.add(advanceTime);
        }

        public void pause()
        {
            mPlaying = false;
        }

        public void stop()
        {
            mPlaying = false;
            currentFrame = 0;

            TickManager.remove(advanceTime);
        }
    }
}
