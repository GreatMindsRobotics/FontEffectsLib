using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FontEffectsLib.SpriteTypes
{
    /// <summary>
    /// A <see cref="Panel"/> which contains a texture.
    /// </summary>
    public class TexturePanel : Panel
    {
        /// <summary>
        /// If <see cref="SetTextureWithBackground"/> method is called, then <see cref="Draw"/> method should not use the specified <see cref="TintColor"/>, to avoid "double-tinting"
        /// </summary>
        protected bool _useTintColorInDraw = true;

        /// <summary>
        /// Sets the image of this panel to be the specified image rendered on top of a solid background of the specified color.
        /// </summary>
        /// <param name="image">The new image.</param>
        /// <param name="background">The background color behind the image.</param>
        public void SetTextureWithBackground(Texture2D image, Color background)
        {
            SetTextureWithBackground(image, background, Vector2.Zero, 0f, Vector2.Zero, Vector2.One, Color.White);
        }

        /// <summary>
        /// Sets the image of this panel to be the specified image rendered on top of a solid background of the specified color.
        /// </summary>
        /// <param name="image">The new image.</param>
        /// <param name="centerImage">true to center image on the <see cref="TexturePanel"/>; false to set it at Vector2.Zero</param>
        public void SetTextureWithBackground(Texture2D image, bool centerImage)
        {
            SetTextureWithBackground(image, _tintColor, centerImage);
        }

        /// <summary>
        /// Sets the image of this panel to be the specified image rendered on top of a solid background of the specified color.
        /// </summary>
        /// <param name="image">The new image.</param>
        /// <param name="backgroundColor">The background color behind the image.</param>
        /// <param name="centerImage">true to center image on the <see cref="TexturePanel"/>; false to set it at Vector2.Zero</param>
        public void SetTextureWithBackground(Texture2D image, Color backgroundColor, bool centerImage)
        {
            Vector2 imagePosition = new Vector2(_texture.Width- image.Width, _texture.Height - image.Height) / 2f;
            SetTextureWithBackground(image, backgroundColor, imagePosition, 0f, Vector2.Zero, Vector2.One, Color.White);
        }

        /// <summary>
        /// Sets the image of this panel to be the specified image rendered on top of a solid background of the specified color.
        /// </summary>
        /// <param name="image">The new image.</param>
        /// <param name="backgroundColor">The background color behind the image.</param>
        /// <param name="imagePos">The position of the image relative to the background.</param>
        /// <param name="imageRotation">The rotation of the image.</param>
        /// <param name="imageOrigin">The origin point of the image.</param>
        /// <param name="imageScale">The scale of the image.</param>
        /// <param name="imageTintColor">The tint color to apply to the image.</param>
        /// <remarks>
        /// This method is expensive and should be called only when neccesary, such as during load time.
        /// </remarks>
        public void SetTextureWithBackground(Texture2D image, Color backgroundColor, Vector2 imagePos, float imageRotation, Vector2 imageOrigin, Vector2 imageScale, Color imageTintColor)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            if ((float)image.Width * imageScale.X > (float)_texture.Width || (float)image.Height * imageScale.Y > (float)_texture.Height)
            {
                throw new ArgumentOutOfRangeException("image", "Image cannot be scaled larger than the current panel");
            }

            RenderTarget2D renderImg = new RenderTarget2D(image.GraphicsDevice, _texture.Width, _texture.Height);
            image.GraphicsDevice.SetRenderTarget(renderImg);
            image.GraphicsDevice.Clear(backgroundColor);

            SpriteBatch temporary = new SpriteBatch(image.GraphicsDevice);
            temporary.Begin();
            temporary.Draw(image, imagePos, null, imageTintColor, imageRotation, imageOrigin, imageScale, SpriteEffects.None, 0f);
            temporary.End();

            image.GraphicsDevice.SetRenderTarget(null);

            _texture = renderImg;
            _useTintColorInDraw = false;
        }

        /// <summary>
        /// Sets the image of this panel to be the specified image rendered on top of a solid background of the tint color.
        /// </summary>
        /// <param name="image">The new image.</param>
        public void SetTextureWithBackground(Texture2D image)
        {
            SetTextureWithBackground(image, _tintColor);
        }


        /// <summary>
        /// Creates a new panel housing a texture. Panel is the same size as the texture.
        /// </summary>
        /// <param name="image">The image to display within the panel.</param>
        /// <param name="speed">The speed of expansion and collapsing.</param>
        /// <param name="position">The position of the center of the panel. The panel expands/collapses from/to this point.</param>
        /// <param name="color">The tint color of the panel.</param>
        public TexturePanel(Texture2D image, Vector2 speed, Vector2 position, Color color)
            : this(image, new Vector2(image.Width, image.Height), speed, position, color)
        {
            //Pass-through constructor
        }

        /// <summary>
        /// Creates a new panel housing a texture. Panel can be larger than the texture. Texture will be centered on the panel.
        /// </summary>
        /// <param name="image">The image to display within the panel.</param>
        /// <param name="size">Panel size. Must be same size or larger than the texture it is housing.</param>
        /// <param name="speed">The speed of expansion and collapsing.</param>
        /// <param name="position">The position of the center of the panel. The panel expands/collapses from/to this point.</param>
        /// <param name="color">The tint color of the panel.</param>
        public TexturePanel(Texture2D image, Vector2 size, Vector2 speed, Vector2 position, Color color)
            : base(image.GraphicsDevice, size, speed, position, color)
        {
            if (image.Width > size.X || image.Height > size.Y)
            {
                throw new ArgumentOutOfRangeException("image", "Image cannot be larger than the current panel.");
            }
            else if (image.Width == size.X && image.Height == size.Y)
            {
                _texture = image;
            }
            else
            {
                SetTextureWithBackground(image, true);
            }
        }


        /// <summary>
        /// Draws this <see cref="TexturePanel"/> to the specified spriteBatch.
        /// </summary>
        /// <param name="spriteBatch"><see cref="SpriteBatch"/> object to draw to.</param> 
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible)
            {
                return;
            }

            spriteBatch.Draw(_texture, _position, null, _useTintColorInDraw ? _tintColor : Color.White, _rotation, _origin, _scale, _effects, _layerDepth);
        }
    }
}
