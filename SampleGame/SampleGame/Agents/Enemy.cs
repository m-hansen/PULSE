using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SampleGame.Attacks;
using Microsoft.Xna.Framework;
using SampleGame.Helpers;

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
    }
}
