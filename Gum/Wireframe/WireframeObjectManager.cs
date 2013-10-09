﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RenderingLibrary.Math.Geometry;
using Gum.DataTypes;
using Gum.ToolStates;
using Gum.DataTypes.Variables;
using Gum.Managers;
using RenderingLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using RenderingLibrary.Content;
using RenderingLibrary;
using System.Collections;
using Gum.RenderingLibrary;
using Microsoft.Xna.Framework;
using FlatRedBall.AnimationEditorForms.Controls;
using System.Threading.Tasks;

namespace Gum.Wireframe
{
    #region Enums

    public enum InstanceFetchType
    {
        InstanceInCurrentElement,
        DeepInstance
    }

    #endregion

    public partial class WireframeObjectManager
    {
        #region Fields

        ElementSave mElementShowing;

        static WireframeObjectManager mSelf;

        List<LineRectangle> mLineRectangles = new List<LineRectangle>();
        List<Sprite> mSprites = new List<Sprite>();
        List<Text> mTexts = new List<Text>();
        List<SolidRectangle> mSolidRectangles = new List<SolidRectangle>();
        List<NineSlice> mNineSlices = new List<NineSlice>();

        List<GraphicalUiElement> mGraphicalElements = new List<GraphicalUiElement>();

        WireframeEditControl mEditControl;
        WireframeControl mWireframeControl;

        Sprite mBackgroundSprite;

        const int left = -4096;
        const int width = 8192;


        #endregion

        #region Properties

        public List<GraphicalUiElement> AllIpsos
        {
            get
            {
                return mGraphicalElements;
                //foreach (Sprite sprite in mSprites)
                //{
                //    yield return sprite;
                //}

                //foreach (Text text in mTexts)
                //{
                //    yield return text;
                //}
                //foreach (LineRectangle rectangle in mLineRectangles)
                //{
                //    yield return rectangle;
                //}
                //foreach (SolidRectangle solidRectangle in mSolidRectangles)
                //{
                //    yield return solidRectangle;
                //}
                //foreach (NineSlice nineSlice in mNineSlices)
                //{
                //    yield return nineSlice;
                //}
            }

        }

        public ElementSave ElementShowing
        {
            get;
            private set;
        }

        public static WireframeObjectManager Self
        {
            get
            {
                if (mSelf == null)
                {
                    mSelf = new WireframeObjectManager();
                }
                return mSelf;
            }
        }

        #endregion

        public void Initialize(WireframeEditControl editControl, WireframeControl wireframeControl)
        {
            mWireframeControl = wireframeControl;
            mWireframeControl.AfterXnaInitialize += HandleAfterXnaIntiailize;
            mWireframeControl.XnaUpdate += HandleXnaUpdate;

            mEditControl = editControl;
            mEditControl.ZoomChanged += new EventHandler(HandleControlZoomChange);
        }

        private void HandleAfterXnaIntiailize(object sender, EventArgs e)
        {
            // Create the Texture2D here
            ImageData imageData = new ImageData(2, 2, null);

            int lightColor = 150;
            int darkColor = 170;
            Color darkGray = new Color(lightColor, lightColor, lightColor);
            Color lightGray = new Color(darkColor, darkColor, darkColor);

            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    bool isDark = ((x + y) % 2 == 0);
                    if (isDark)
                    {
                        imageData.SetPixel(x, y, darkGray);

                    }
                    else
                    {
                        imageData.SetPixel(x, y, lightGray);
                    }
                }
            }

            Texture2D texture = imageData.ToTexture2D(false);
            mBackgroundSprite = new Sprite(texture);

            mBackgroundSprite.Wrap = true;
            mBackgroundSprite.X = -4096;
            mBackgroundSprite.Y = -4096;
            mBackgroundSprite.Width = 8192;
            mBackgroundSprite.Height = 8192;

            mBackgroundSprite.Wrap = true;
            int timesToRepeat = 256;
            mBackgroundSprite.SourceRectangle = new Rectangle(0, 0, timesToRepeat * texture.Width, timesToRepeat * texture.Height);

            SpriteManager.Self.Add(mBackgroundSprite);
        }

        private void HandleXnaUpdate()
        {
        }



        void HandleControlZoomChange(object sender, EventArgs e)
        {
            Renderer.Self.Camera.Zoom = mEditControl.PercentageValue / 100.0f;
        }

        private void ClearAll()
        {
            foreach (LineRectangle rectangle in mLineRectangles)
            {
                ShapeManager.Self.Remove(rectangle);
            }
            mLineRectangles.Clear();

            foreach (Sprite sprite in mSprites)
            {
                SpriteManager.Self.Remove(sprite);
            }
            mSprites.Clear();

            foreach (Text text in mTexts)
            {
                TextManager.Self.Remove(text);
            }
            mTexts.Clear();

            foreach (SolidRectangle solidRectangle in mSolidRectangles)
            {
                ShapeManager.Self.Remove(solidRectangle);
            }
            mSolidRectangles.Clear();

            foreach (NineSlice nineSlice in mNineSlices)
            {
                SpriteManager.Self.Remove(nineSlice);
            }
            mNineSlices.Clear();

            mGraphicalElements.Clear();
        }

        public void RefreshAll(bool force)
        {
            ElementSave elementSave = SelectedState.Self.SelectedElement;

            RefreshAll(force, elementSave);

            SelectionManager.Self.Refresh();

            mWireframeControl.UpdateWireframeToProject();
        }

        public void RefreshAll(bool force, ElementSave elementSave)
        {

            if (elementSave == null || elementSave.IsSourceFileMissing)
            {
                ClearAll();
            }

            else if (elementSave != null && (force || elementSave != ElementShowing))
            {

                ClearAll();

                // If it's the same element, let's not refresh the textures
                bool didElementChange = elementSave != ElementShowing;

                if (didElementChange)
                {
                    LoaderManager.Self.CacheTextures = false;
                }
                LoaderManager.Self.CacheTextures = true;

                CreateIpsoForElement(elementSave);

            }
            ElementShowing = elementSave;
        }



        public GraphicalUiElement GetSelectedRepresentation()
        {
            if (!SelectionManager.Self.HasSelection)
            {
                return null;
            }
            else if (SelectedState.Self.SelectedInstance != null)
            {
                return GetRepresentation(SelectedState.Self.SelectedInstance, SelectedState.Self.GetTopLevelElementStack());
            }
            else if (SelectedState.Self.SelectedElement != null)
            {
                return GetRepresentation(SelectedState.Self.SelectedElement);
            }
            else
            {
                throw new Exception("The SelectionManager believes it has a selection, but there is no selected instance or element");
            }
        }

        public GraphicalUiElement GetRepresentation(ElementSave elementSave)
        {
#if DEBUG
            if (elementSave == null)
            {
                throw new NullReferenceException("The argument elementSave is null");
            }
#endif
            foreach (GraphicalUiElement ipso in AllIpsos)
            {
                if (ipso.Tag == elementSave)
                {
                    return ipso;
                }
            }

            return null;
        }

        public GraphicalUiElement GetRepresentation(InstanceSave instanceSave, List<ElementWithState> elementStack)
        {
            if (instanceSave != null)
            {
                if (elementStack == null)
                {
                    return AllIpsos.FirstOrDefault(item => item.Tag == instanceSave);
                }
                else
                {
                    IEnumerable<IPositionedSizedObject> currentChildren = WireframeObjectManager.Self.AllIpsos.Where(item => item.Parent == null);

                    return AllIpsos.FirstOrDefault(item => item.Tag == instanceSave);
                            

                }
            }
            return null;
        }

        public Text GetText(InstanceSave instanceSave)
        {
            foreach (Text text in mTexts)
            {
                if (text.Name == instanceSave.Name)
                {
                    return text;
                }
            }

            return null;

        }

        public Text GetText(ElementSave elementSave)
        {
            foreach (Text text in mTexts)
            {
                if (text.Name == elementSave.Name)
                {
                    return text;
                }
            }
            return null;
        }


        /// <summary>
        /// Returns the InstanceSave that uses this representation or the
        /// instance that has a a contained instance that uses this representation.
        /// </summary>
        /// <param name="representation">The representation in question.</param>
        /// <returns>The InstanceSave or null if one isn't found.</returns>
        public InstanceSave GetInstance(IPositionedSizedObject representation, InstanceFetchType fetchType, List<ElementWithState> elementStack)
        {
            ElementSave selectedElement = SelectedState.Self.SelectedElement;


            string prefix = selectedElement.Name + ".";
            if (selectedElement is ScreenSave)
            {
                prefix = "";
            }

            return GetInstance(representation, selectedElement, prefix, fetchType, elementStack);

        }

        public InstanceSave GetInstance(IPositionedSizedObject representation, ElementSave instanceContainer, string prefix, InstanceFetchType fetchType, List<ElementWithState> elementStack)
        {
            if (instanceContainer == null)
            {
                return null;
            }

            InstanceSave toReturn = null;


            string qualifiedName = representation.GetAttachmentQualifiedName(elementStack);

            // strip off the guide name if it starts with a guide
            qualifiedName = StripGuideOrParentNameIfNecessaryName(qualifiedName, representation);


            foreach (InstanceSave instanceSave in instanceContainer.Instances)
            {
                if (prefix + instanceSave.Name == qualifiedName)
                {
                    toReturn = instanceSave;
                    break;
                }
            }


            if (toReturn == null)
            {
                foreach (InstanceSave instanceSave in instanceContainer.Instances)
                {
                    ElementSave instanceElement = instanceSave.GetBaseElementSave();

                    toReturn = GetInstance(representation, instanceElement, prefix + instanceSave.Name + ".", fetchType, elementStack);

                    if (toReturn != null)
                    {
                        if (fetchType == InstanceFetchType.DeepInstance)
                        {
                            // toReturn will be toReturn, no need to do anything
                        }
                        else // fetchType == InstanceInCurrentElement
                        {
                            toReturn = instanceSave;
                        }
                        break;
                    }
                }
            }

            return toReturn;
        }

        private string StripGuideOrParentNameIfNecessaryName(string qualifiedName, IPositionedSizedObject representation)
        {
            bool wasStripped = false;
            foreach (NamedRectangle rectangle in ObjectFinder.Self.GumProjectSave.Guides)
            {
                if (qualifiedName.StartsWith(rectangle.Name + "."))
                {
                    return qualifiedName.Substring(rectangle.Name.Length + 1);
                }
            }

            if (representation.Parent != null && representation.Parent.Tag is InstanceSave && representation.Tag is InstanceSave)
            {
                // strip this off!
                if ((representation.Parent.Tag as InstanceSave).ParentContainer == (representation.Tag as InstanceSave).ParentContainer)
                {
                    string whatToTakeOff = (representation.Parent.Tag as InstanceSave).Name + ".";

                    int index = qualifiedName.IndexOf(whatToTakeOff);

                    return qualifiedName.Replace(whatToTakeOff, "");

                    //return qualifiedName.Substring((representation.Parent.Tag as InstanceSave).Name.Length + 1);
                }
            }

            return qualifiedName;
        }


        public bool IsRepresentation(IPositionedSizedObject ipso)
        {
            return mLineRectangles.Contains(ipso) || mSprites.Contains(ipso) ||
                mTexts.Contains(ipso) || mSolidRectangles.Contains(ipso) ||
                mNineSlices.Contains(ipso) || mLineRectangles.Contains(ipso) || mGraphicalElements.Contains(ipso);
        }

        public ElementSave GetElement(IPositionedSizedObject representation)
        {
            if (SelectedState.Self.SelectedElement != null &&
                SelectedState.Self.SelectedElement.Name == representation.Name)
            {
                return SelectedState.Self.SelectedElement;
            }

            return null;
        }

        public T GetIpsoAt<T>(float x, float y, IList<T> list) where T : IPositionedSizedObject
        {
            foreach (T ipso in list)
            {
                if (ipso.HasCursorOver(x, y))
                {
                    return ipso;
                }
            }
            return default(T);
        }


        internal void UpdateScalesAndPositionsForSelectedChildren()
        {
            List<ElementWithState> elementStack = new List<ElementWithState>();
            elementStack.Add(new ElementWithState(SelectedState.Self.SelectedElement));
            foreach (IPositionedSizedObject selectedIpso in SelectionManager.Self.SelectedIpsos)
            {
                UpdateScalesAndPositionsForSelectedChildren(selectedIpso as GraphicalUiElement, selectedIpso.Tag as InstanceSave, elementStack);
            }
        }

        internal void UpdateScalesAndPositionsForChildren(List<GraphicalUiElement> children, List<ElementWithState> elementStack)
        {
            foreach (IPositionedSizedObject selectedIpso in children)
            {
                UpdateScalesAndPositionsForSelectedChildren(selectedIpso as GraphicalUiElement, selectedIpso.Tag as InstanceSave, elementStack);
            }
        }

        void UpdateScalesAndPositionsForSelectedChildren(GraphicalUiElement ipso, InstanceSave instanceSave, List<ElementWithState> elementStack)
        {
            if (ipso == null)
            {
                throw new ArgumentException("ipso must not be null");
            }
            if (ipso.Children.Count() != 0 && ipso.Children.Any(item => item is GraphicalUiElement) == false)
            {
                throw new Exception("All GraphicalUiElement children should also be GraphicalUiElements");
            }

            float width = ((IPositionedSizedObject)ipso).Width;
            float height = ((IPositionedSizedObject)ipso).Height;
            if ((width == 0 || height == 0))
            {
                int m = 3;
            }




            ElementSave selectedElement = null;

            bool wasAdded = TryAddToElementStack(instanceSave, elementStack, out selectedElement);

            // Let's do children of the instance first
            Predicate<InstanceSave> predicate = (childInstance) => childInstance != null && !childInstance.IsParentASibling(elementStack);
            SetWidthPositionOnIpsoChildren(ipso, elementStack, selectedElement, predicate);

            
            // pop the stack, then do siblings
            if (wasAdded)
            {
                elementStack.Remove(selectedElement);
            }

            predicate = (childInstance) => childInstance != null && childInstance.IsParentASibling(elementStack);
            SetWidthPositionOnIpsoChildren(ipso, elementStack, selectedElement, predicate);

            if (ipso != null)
            {
                // Now we can calculate the width/height of this thing
                width = ((IPositionedSizedObject)ipso).Width;
                height = ((IPositionedSizedObject)ipso).Height;
                if ((width == 0 || height == 0) && (ipso.Component is Sprite == false && ipso.Component is Text == false))
                {
                    float requiredWidth;
                    float requiredHeight;

                    GetRequiredDimensionsFromContents(ipso, out requiredWidth, out requiredHeight);

                    if (((IPositionedSizedObject)ipso).Width == 0)
                    {
                        ((IPositionedSizedObject)ipso).Width = requiredWidth;
                    }
                    if (((IPositionedSizedObject)ipso).Height == 0)
                    {
                        ((IPositionedSizedObject)ipso).Height = requiredHeight;
                    }

                    wasAdded = TryAddToElementStack(instanceSave, elementStack, out selectedElement);

                    // Let's do children of the instance first
                    predicate = (childInstance) => childInstance != null && !childInstance.IsParentASibling(elementStack);
                    SetWidthPositionOnIpsoChildren(ipso, elementStack, selectedElement, predicate);

                    // pop the stack, then do siblings
                    if (wasAdded)
                    {
                        elementStack.Remove(selectedElement);
                    }

                    predicate = (childInstance) => childInstance != null && childInstance.IsParentASibling(elementStack);
                    SetWidthPositionOnIpsoChildren(ipso, elementStack, selectedElement, predicate);

                }
            }

        }

        private static bool TryAddToElementStack(InstanceSave instanceSave, List<ElementWithState> elementStack, out ElementSave selectedElement)
        {
            bool toReturn = false;
            if (instanceSave == null)
            {
                selectedElement = elementStack.Last().Element;
            }
            else
            {
                selectedElement = ObjectFinder.Self.GetElementSave(instanceSave.BaseType);

                if (elementStack.Count == 0 || elementStack.Last().Element != selectedElement)
                {

                    ElementWithState elementWithState = new ElementWithState(selectedElement);
                    var state = new DataTypes.RecursiveVariableFinder(instanceSave, elementStack).GetValue("State") as string;
                    elementWithState.StateName = state;
                    //elementWithState.StateName 
                    elementStack.Add(elementWithState);
                    toReturn = true;
                }
            }
            return toReturn;
        }

        private void GetRequiredDimensionsFromContents(IPositionedSizedObject parentIpso, out float requiredWidth, out float requiredHeight)
        {
            requiredWidth = 0;
            requiredHeight = 0;
            foreach (var child in parentIpso.Children)
            {
                requiredWidth = System.Math.Max(requiredWidth, child.X + child.Width);
                requiredHeight = System.Math.Max(requiredHeight, child.Y + child.Height);
            }
        }

        private void SetWidthPositionOnIpsoChildren(GraphicalUiElement ipso, List<ElementWithState> elementStack, ElementSave selectedElement, Predicate<InstanceSave> predicate)
        {
            SetWidthPositionOnIpsoChildren(ipso.Children, elementStack, selectedElement, predicate);
        }

        private void SetWidthPositionOnIpsoChildren(IEnumerable<IPositionedSizedObject> children, List<ElementWithState> elementStack, ElementSave selectedElement, Predicate<InstanceSave> predicate)
        {
            if (children.Count() != 0 && children.Any(item => item is GraphicalUiElement) == false)
            {
                throw new Exception("All GraphicalUiElement children should also be GraphicalUiElements");
            }
            // Make sure we only look at IPSOs that actually represent a Gum element/instance
            foreach (GraphicalUiElement child in children.Where(item=>item.Tag != null && item.Tag is InstanceSave))
            {
                InstanceSave childInstance = child.Tag as InstanceSave;

                // ignore siblings:
                if (predicate(childInstance))
                {
                    RecursiveVariableFinder rvf = new RecursiveVariableFinder(childInstance, elementStack);
                    SetIpsoWidthAndPositionAccordingToUnitValueAndTypes(child, selectedElement, rvf);
                    UpdateScalesAndPositionsForSelectedChildren(child, childInstance, elementStack);
                }

            }
        }
    }
}
