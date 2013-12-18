using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FontEffectsLib.SpriteTypes
{
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


        protected Rectangle? _sourceRectangle = null;

        public Rectangle? SourceRectangle
        {
            get { return _sourceRectangle; }
            set { _sourceRectangle = value; }
        }

        public ComplexSprite(Vector2 position, params BaseGameObject[] subsprites)
            : base(position, Color.White)
        {
            Subsprites.AddRange(subsprites);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var drawable in Subsprites)
            {
                drawable.Update(gameTime);
            }
        }

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

                drawable.Effects = drawable.Effects | _effects;
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

        public override void SetCenterAsOrigin()
        {
            //_origin = new Vector2(_target.Width / 2, _target.Height / 2);
        }
    }
}
