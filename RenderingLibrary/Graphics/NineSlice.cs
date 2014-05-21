﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ToolsUtilities;
using RenderingLibrary.Content;

namespace RenderingLibrary.Graphics
{
    #region Enums

    public enum NineSliceSections
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    #endregion

    public class NineSlice : IPositionedSizedObject, IRenderable, IVisible
    {
        #region Fields


        List<IPositionedSizedObject> mChildren = new List<IPositionedSizedObject>();

        Vector2 Position;

        IPositionedSizedObject mParent;

        Sprite mTopLeftSprite = new Sprite(null);
        Sprite mTopSprite = new Sprite(null);
        Sprite mTopRightSprite = new Sprite(null);
        Sprite mRightSprite = new Sprite(null);
        Sprite mBottomRightSprite = new Sprite(null);
        Sprite mBottomSprite = new Sprite(null);
        Sprite mBottomLeftSprite = new Sprite(null);
        Sprite mLeftSprite = new Sprite(null);
        Sprite mCenterSprite = new Sprite(null);



        #endregion

        #region Properties

        public int Alpha
        {
            get
            {
                return Color.A;
            }
            set
            {
                if (value != Color.A)
                {
                    Color = new Color(Color.R, Color.G, Color.B, value);
                }
            }
        }

        public int Red
        {
            get
            {
                return Color.R;
            }
            set
            {
                if (value != Color.R)
                {
                    Color = new Color(value, Color.G, Color.B, Color.A);
                }
            }
        }

        public int Green
        {
            get
            {
                return Color.G;
            }
            set
            {
                if (value != Color.G)
                {
                    Color = new Color(Color.R, value, Color.B, Color.A);
                }
            }
        }

        public int Blue
        {
            get
            {
                return Color.B;
            }
            set
            {
                if (value != Color.B)
                {
                    Color = new Color(Color.R, Color.G, value, Color.A);
                }
            }
        }


        public float Rotation { get; set; }


        public string Name
        {
            get;
            set;
        }
        public object Tag { get; set; }

        public float Width
        {
            get;
            set;
        }

        public float Height
        {
            get;
            set;
        }

        public float EffectiveWidth
        {
            get
            {
                // I think we want to treat these individually so a 
                // width could be set but height could be default
                if (Width != 0)
                {
                    return Width;
                }
                else if (LeftTexture != null && CenterTexture != null && RightTexture != null)
                {
                    return LeftTexture.Width + CenterTexture.Width + RightTexture.Width;
                }
                else
                {
                    return 32;
                }
            }
        }

        public float EffectiveHeight
        {
            get
            {
                // See comment in Width
                if (Height != 0)
                {
                    return Height;
                }
                else if (TopTexture != null && CenterTexture != null && BottomTexture != null)
                {
                    return TopTexture.Height + CenterTexture.Height + BottomTexture.Height;
                }
                else
                {
                    return 32;
                }
            }
        }

        float IPositionedSizedObject.Width
        {
            get
            {
                return EffectiveWidth;
            }
            set
            {
                Width = value;
            }
        }

        float IPositionedSizedObject.Height
        {
            get
            {
                return EffectiveHeight;
            }
            set
            {
                Height = value;
            }
        }

        public Texture2D TopLeftTexture 
        {
            get { return mTopLeftSprite.Texture; }
            set { mTopLeftSprite.Texture = value; }
        }
        public Texture2D TopTexture 
        {
            get { return mTopSprite.Texture; }
            set { mTopSprite.Texture = value; }
        }
        public Texture2D TopRightTexture 
        {
            get { return mTopRightSprite.Texture; }
            set { mTopRightSprite.Texture = value; }
        }
        public Texture2D RightTexture 
        {
            get { return mRightSprite.Texture; }
            set { mRightSprite.Texture = value; }
        }
        public Texture2D BottomRightTexture 
        {
            get { return mBottomRightSprite.Texture; }
            set { mBottomRightSprite.Texture = value; }
        }
        public Texture2D BottomTexture 
        {
            get { return mBottomSprite.Texture; }
            set { mBottomSprite.Texture = value; }
        }
        public Texture2D BottomLeftTexture
        {
            get { return mBottomLeftSprite.Texture; }
            set { mBottomLeftSprite.Texture = value; }
        }
        public Texture2D LeftTexture
        {
            get { return mLeftSprite.Texture; }
            set { mLeftSprite.Texture = value; }
        }
        public Texture2D CenterTexture 
        {
            get { return mCenterSprite.Texture; }
            set { mCenterSprite.Texture = value; }
        }

        public bool Wrap
        {
            get { return false; }
        }

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public float Z
        {
            get;
            set;
        }

        public IPositionedSizedObject Parent
        {
            get { return mParent; }
            set
            {
                if (mParent != value)
                {
                    if (mParent != null)
                    {
                        mParent.Children.Remove(this);
                    }
                    mParent = value;
                    if (mParent != null)
                    {
                        mParent.Children.Add(this);
                    }
                }
            }
        }

        public Color Color
        {
            get
            {
                return mCenterSprite.Color;
            }
            set
            {
                mTopLeftSprite.Color = value;
                mTopSprite.Color = value;
                mTopRightSprite.Color = value;
                mRightSprite.Color = value;
                mBottomRightSprite.Color = value;
                mBottomSprite.Color = value;
                mBottomLeftSprite.Color = value;
                mLeftSprite.Color = value;
                mCenterSprite.Color = value;
            }
        }

        public BlendState BlendState
        {
            get
            {
                return mCenterSprite.BlendState;
            }
            set
            {
                mTopLeftSprite.BlendState = value;
                mTopSprite.BlendState = value;
                mTopRightSprite.BlendState = value;
                mRightSprite.BlendState = value;
                mBottomRightSprite.BlendState = value;
                mBottomSprite.BlendState = value;
                mBottomLeftSprite.BlendState = value;
                mLeftSprite.BlendState = value;
                mCenterSprite.BlendState = value;
            }
        }



        public ICollection<IPositionedSizedObject> Children
        {
            get { return mChildren; }
        }

        public static Dictionary<NineSliceSections, string> PossibleNineSliceEndings
        {
            get;
            private set;
        }
        

        #endregion


        #region Methods


        void IRenderable.Render(SpriteBatch spriteBatch, SystemManagers managers)
        {
            if (this.AbsoluteVisible)
            {
                float desiredMiddleWidth = this.Width - mTopLeftSprite.EffectiveWidth - mTopRightSprite.EffectiveWidth;
                float desiredMiddleHeight = this.Height - this.mTopLeftSprite.EffectiveHeight - this.mBottomLeftSprite.EffectiveHeight;

                this.mTopSprite.Width = desiredMiddleWidth;
                this.mCenterSprite.Width = desiredMiddleWidth;
                this.mBottomSprite.Width = desiredMiddleWidth;

                this.mLeftSprite.Height = desiredMiddleHeight;
                this.mCenterSprite.Height = desiredMiddleHeight;
                this.mRightSprite.Height = desiredMiddleHeight;

                float y = this.GetAbsoluteY();

                mTopLeftSprite.X = this.GetAbsoluteX() ;
                mTopLeftSprite.Y = y;

                mTopSprite.X = mTopLeftSprite.X + mTopLeftSprite.EffectiveWidth;
                mTopSprite.Y = y;

                mTopRightSprite.X = mTopSprite.X + mTopSprite.EffectiveWidth;
                mTopRightSprite.Y = y;

                y = mTopLeftSprite.Y + mTopLeftSprite.EffectiveHeight;

                mLeftSprite.X = this.GetAbsoluteX();
                mLeftSprite.Y = y;

                mCenterSprite.X = mLeftSprite.X + mLeftSprite.EffectiveWidth;
                mCenterSprite.Y = y;

                mRightSprite.X = mCenterSprite.X + mCenterSprite.EffectiveWidth;
                mRightSprite.Y = y;

                y = mLeftSprite.Y + mLeftSprite.EffectiveHeight;

                mBottomLeftSprite.X = this.GetAbsoluteX();
                mBottomLeftSprite.Y = y;

                mBottomSprite.X = mBottomLeftSprite.X + mBottomLeftSprite.EffectiveWidth;
                mBottomSprite.Y = y;

                mBottomRightSprite.X = mBottomSprite.X + mBottomSprite.EffectiveWidth;
                mBottomRightSprite.Y = y;


                ((IRenderable)mTopLeftSprite).Render(spriteBatch, managers);
                ((IRenderable)mTopSprite).Render(spriteBatch, managers);
                ((IRenderable)mTopRightSprite).Render(spriteBatch, managers);
                ((IRenderable)mLeftSprite).Render(spriteBatch, managers);
                ((IRenderable)mCenterSprite).Render(spriteBatch, managers);
                ((IRenderable)mRightSprite).Render(spriteBatch, managers);
                ((IRenderable)mBottomLeftSprite).Render(spriteBatch, managers);
                ((IRenderable)mBottomSprite).Render(spriteBatch, managers);
                ((IRenderable)mBottomRightSprite).Render(spriteBatch, managers);

            }
        }

        void IPositionedSizedObject.SetParentDirect(IPositionedSizedObject parent)
        {
            mParent = parent;
        }


        #region IVisible Implementation

        public bool Visible
        {
            get;
            set;
        }

        public bool AbsoluteVisible
        {
            get
            {
                if (((IVisible)this).Parent == null)
                {
                    return Visible;
                }
                else
                {
                    return Visible && ((IVisible)this).Parent.AbsoluteVisible;
                }
            }
        }

        IVisible IVisible.Parent
        {
            get
            {
                return ((IPositionedSizedObject)this).Parent as IVisible;
            }
        }

        #endregion

        static NineSlice()
        {
            PossibleNineSliceEndings = new Dictionary<NineSliceSections, string>()
            {
                {NineSliceSections.Center, "_center"},
                {NineSliceSections.Left, "_left"},
                {NineSliceSections.Right, "_right"},
                {NineSliceSections.TopLeft, "_topLeft"},
                {NineSliceSections.Top, "_topCenter"},
                {NineSliceSections.TopRight, "_topRight"},
                {NineSliceSections.BottomLeft, "_bottomLeft"},
                {NineSliceSections.Bottom, "_bottomCenter"},
                {NineSliceSections.BottomRight, "_bottomRight"}
            };

        }

        public NineSlice()
        {
            Visible = true;
        }

        public void SetTexturesUsingPattern(string anyOf9Textures, SystemManagers managers)
        {

            string absoluteTexture = anyOf9Textures;

            if(FileManager.IsRelative(absoluteTexture))
            {
                absoluteTexture = FileManager.RelativeDirectory + absoluteTexture;

                absoluteTexture = FileManager.RemoveDotDotSlash(absoluteTexture);
            }

            string extension = FileManager.GetExtension(absoluteTexture);

            string bareTexture = GetBareTextureForNineSliceTexture(absoluteTexture);
            string error;
            if (!string.IsNullOrEmpty(bareTexture))
            {
                TopLeftTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.TopLeft] + "." + extension, managers, out error);
                TopTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.Top] + "." + extension, managers, out error);
                TopRightTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.TopRight] + "." + extension, managers, out error);

                LeftTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.Left] + "." + extension, managers, out error);
                CenterTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.Center] + "." + extension, managers, out error);
                RightTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.Right] + "." + extension, managers, out error);

                BottomLeftTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.BottomLeft] + "." + extension, managers, out error);
                BottomTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.Bottom] + "." + extension, managers, out error);
                BottomRightTexture = LoaderManager.Self.LoadOrInvalid(
                    bareTexture + NineSlice.PossibleNineSliceEndings[NineSliceSections.BottomRight] + "." + extension, managers, out error);
            }


        }

        
        public static string GetBareTextureForNineSliceTexture(string absoluteTexture)
        {
            string extension = FileManager.GetExtension(absoluteTexture);

            string withoutExtension = FileManager.RemoveExtension(absoluteTexture);

            string toReturn = withoutExtension;

            foreach (var kvp in NineSlice.PossibleNineSliceEndings)
            {
                if (withoutExtension.ToLower().EndsWith(kvp.Value.ToLower()))
                {
                    toReturn = withoutExtension.Substring(0, withoutExtension.Length - kvp.Value.Length);
                    break;
                }
            }

            // No extensions, because we'll need to append that
            //toReturn += "." + extension;

            return toReturn;
        }

        #endregion
    }

}
