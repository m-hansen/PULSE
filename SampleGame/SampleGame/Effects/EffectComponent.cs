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


namespace SampleGame.Effects
{
    public class EffectComponent : DrawableGameComponent
    {
        SpriteBatch batch;
        List<Effect> effectList = new List<Effect>();

        public EffectComponent(Game game)
            : base(game)
        {
            
        }

        public override void Initialize()
        {
            batch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        public void AddEffect(Effect effect)
        {
            effectList.Add(effect);
        }

        public void SplitBulletsFromPlayer()
        {
            List<Effect> bulletList = effectList.Where(e => e.CastedBy == Enums.AgentType.Player && e.EffectType == Enums.AttackType.Bullet).ToList();

            foreach (Effect effect in bulletList)
            {
                effectList.Add(effect.CloneToDirection((float)(Math.PI / 4)));
                effectList.Add(effect.CloneToDirection((float)(3 * Math.PI / 4)));
                effectList.Add(effect.CloneToDirection((float)(5 * Math.PI / 4)));
                effectList.Add(effect.CloneToDirection((float)(7 * Math.PI / 4)));
            }
        }

        public void NukeSplit()
        {
            List<Effect> bulletList = effectList.Where(e => e.CastedBy == Enums.AgentType.Player && e.EffectSubType == Enums.AttackSubType.Nuke).ToList();

            foreach (Effect effect in bulletList)
            {
                effectList.Add(effect.CloneToDirection((float)(Math.PI / 4)));
                effectList.Add(effect.CloneToDirection((float)(3 * Math.PI / 4)));
                effectList.Add(effect.CloneToDirection((float)(5 * Math.PI / 4)));
                effectList.Add(effect.CloneToDirection((float)(7 * Math.PI / 4)));
            }
        }

        public override void Update(GameTime gameTime)
        {
            LevelInfo levelInfo = Game1.Current.levelInfo;

            for (int i = 0; i < effectList.Count; i++)
            {
                effectList[i].Update(gameTime, levelInfo);
            }

            foreach (Effect effect in effectList.Where(e => !e.Active).ToList())
            {
                effectList.Remove(effect);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //SpriteFont font1 = Game1.Current.font1;
            Rectangle visibleRect = Game1.Current.levelInfo.VisibleRect;

            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            foreach (Effect effect in effectList)
            {
                effect.Draw(batch, visibleRect);
            }

            batch.End();

            base.Draw(gameTime);
        }
    }
}
