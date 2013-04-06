using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SampleGame.Attacks;
using Microsoft.Xna.Framework;
using SampleGame.Helpers;
using SampleGame.Effects;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame.Agents
{
    public class Enemy : MovingAgent
    {
        public List<Attack> attackList = new List<Attack>();

        public override void Update(GameTime gametime, Player playerObj, List<GameAgent> agentList, int nodeSize)
        {
            Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);

            bool usedAttack = false;

            foreach (Attack attack in attackList.OrderByDescending(a => a.MaxDamage).ToList())
            {
                if (!usedAttack && attack.ActiveCoolDown <= 0)
                {
                    attack.UseAttack(Enums.AgentType.NPC, this);

                    attack.ActiveCoolDown = attack.CoolDown + (new Random()).Next((int)(attack.CoolDown * 0.2));

                    usedAttack = true;
                }
                else
                {
                    attack.Update(gametime);
                }
            }

 	        base.Update(gametime, playerObj, agentList, nodeSize);
        }

        public override void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Explosion explosion = new Explosion();
                explosion.LoadExplosion(Game1.Current.Content.Load<Texture2D>("Images\\explosion1"), new Rectangle(0, 0, 139, 107), 6);
                explosion.Position = Position;
                explosion.AnimationInterval = new TimeSpan(1100000);

                Game1.Current.EffectComponent.AddEffect(explosion);

                Game1.Current.levelInfo.AgentList.Remove(this);
            }
        }
    }
}
