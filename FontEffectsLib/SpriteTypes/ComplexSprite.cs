using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FontEffectsLib.SpriteTypes
{
    /// <summary>
    /// A sprite containing multiple relatively positioned sprites.
    /// </summary>
    /// <remarks>
    /// This type of sprite has no definite size.
    /// </remarks>
    public class ComplexSprite : BaseGameObject
    {
        private List<BaseGameObject> _subSprites = new List<BaseGameObject>();

        /// <summary>
        /// Gets a list of sprites within this <see cref="ComplexSprite"/>, of which the positions are relative to the parent sprite.
        /// </summary>
        public List<BaseGameObject> Subsprites
        {
            get { return _subSprites; }
        }

        /// <summary>
        /// Creates a <see cref="ComplexSprite"/> at the specified position with the specified subsprites, tinted white.
        /// </summary>
        /// <param name="position">The root position of the <see cref="ComplexSprite"/>.</param>
        /// <param name="subsprites">The array of sprites to include within this <see cref="ComplexSprite"/>.</param>
        public ComplexSprite(Vector2 position, params BaseGameObject[] subsprites)
            : base(position, Color.White)
        {
            _subSprites = subsprites == null ? new List<BaseGameObject>() : subsprites.ToList();
        }

        /// <summary>
        /// Updates all of the components of this <see cref="ComplexSprite"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (var drawable in Subsprites)
            {
                drawable.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws all of the subsprites of this <see cref="ComplexSprite"/> to the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to render objects to.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible)
            {
                return;
            }
            
            foreach (var drawable in Subsprites)
            {
                if (!drawable.IsVisible)
                {
                    continue;
                }

                Vector2 pos = drawable.Position;
                Vector2 origin = drawable.Origin;
                SpriteEffects effect = drawable.Effects;
                float rotate = drawable.Rotation;
                Vector2 scale = drawable.Scale;
                Color tint = drawable.TintColor;

                drawable.Effects |= _effects;
                drawable.Origin += Origin;
                drawable.Position += Position;
                drawable.Rotation += Rotation;
                drawable.Scale *= Scale;
                drawable.TintColor = new Color(TintColor.ToVector4() * drawable.TintColor.ToVector4());
                
                drawable.Draw(spriteBatch);

                drawable.Effects = effect;
                drawable.Origin = origin;
                drawable.Position = pos;
                drawable.Rotation = rotate;
                drawable.Scale = scale;
                drawable.TintColor = tint;
            }
        }

        /// <summary>
        /// Sets the center of this <see cref="ComplexSprite"/> as the origin.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when you invoke this method, because a <see cref="ComplexSprite"/> has no definite size.</exception>
        public override void SetCenterAsOrigin()
        {
            throw new InvalidOperationException("A ComplexSprite has no definite size, so the center cannot be determined.");
        }
    }
}
