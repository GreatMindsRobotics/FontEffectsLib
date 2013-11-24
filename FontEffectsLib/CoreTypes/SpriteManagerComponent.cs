using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace FontEffectsLib.CoreTypes
{
    /// <summary>
    /// A game component that manages <see cref="ISprite"/>s on the specified <see cref="SpriteBatch"/>.
    /// </summary>
    public class SpriteManagerComponent : Microsoft.Xna.Framework.DrawableGameComponent, ICollection<ISprite>
    {
        private List<ISprite> _sprites = new List<ISprite>();

        /// <summary>
        /// Creates a new <see cref="SpriteManagerComponent"/>.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> to manage sprites for.</param>
        /// <param name="batch">The <see cref="SpriteBatch"/> to render sprites to.</param>
        public SpriteManagerComponent(Game game, SpriteBatch batch)
            : base(game)
        {
            if (batch == null)
            {
                throw new ArgumentNullException("batch");
            }
            _batch = batch;
        }

        private SpriteBatch _batch;

        /// <summary>
        /// Gets or sets the <see cref="SpriteBatch"/> to render sprites to.
        /// </summary>
        public SpriteBatch Batch
        {
            get { return _batch; }
            set {
                if (value == null)
                {
                    throw new ArgumentNullException("Batch");
                }
                _batch = value; }
        }

        /// <summary>
        /// Updates all of the <see cref="ISprite"/>s within this <see cref="SpriteManagerComponent"/>.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (ISprite s in this)
            {
                s.Update(gameTime);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws all of the <see cref="ISprite"/>s within this <see cref="SpriteManagerComponent"/>.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (ISprite s in this)
            {
                s.Draw(Batch);
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// Adds the specified sprite to this <see cref="SpriteManagerComponent"/>.
        /// </summary>
        /// <param name="item">The <see cref="ISprite"/> to add.</param>
        public void Add(ISprite item)
        {
            _sprites.Add(item);
        }

        /// <summary>
        /// Clears this <see cref="SpriteManagerComponent"/> of <see cref="ISprite"/>s.
        /// </summary>
        public void Clear()
        {
            _sprites.Clear();
        }

        /// <summary>
        /// Determines whether this <see cref="SpriteManagerComponent"/> contains the specified <see cref="ISprite"/>.
        /// </summary>
        /// <param name="item">The <see cref="ISprite"/> to check for containment.</param>
        /// <returns>Whether this <see cref="SpriteManagerComponent"/> contains the specified <see cref="ISprite"/>.</returns>
        public bool Contains(ISprite item)
        {
            return _sprites.Contains(item);
        }

        public void CopyTo(ISprite[] array, int arrayIndex)
        {
            _sprites.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of sprites in this <see cref="SpriteManagerComponent"/>.
        /// </summary>
        public int Count
        {
            get { return _sprites.Count; }
        }

        /// <summary>
        /// Returns false, indicating that this collection is read-write.
        /// </summary>
        bool System.Collections.Generic.ICollection<ISprite>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ISprite item)
        {
            return _sprites.Remove(item);
        }

        public IEnumerator<ISprite> GetEnumerator()
        {
            return _sprites.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _sprites.GetEnumerator();
        }
    }
}
