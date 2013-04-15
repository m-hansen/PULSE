using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SampleGame.Attacks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame.Agents
{
    public class Item : MovingAgent
    {
        public int ItemType;

        public Item(int itemType, Vector2 position)
        {
            string assetName = string.Empty;
            Rectangle? rect = null;
            int frames = 1;

            switch (itemType)
            {
                case (int)Enums.ItemType.NukeAttack:

                    assetName = "Images\\nuke_icon";

                    break;

                case (int)Enums.ItemType.HealthPowerUp:

                    assetName = "Images\\health_icon";

                    break;

                case (int)Enums.ItemType.EnergyPowerUp:

                    assetName = "Images\\energy_icon";

                    break;

                case (int)Enums.ItemType.TeleportSpell:

                    assetName = "Images\\teleport_icon";

                    break;
            }

            ItemType = itemType;
            Position = position;
            Type = (int)Enums.AgentType.Item;
            
            LoadContent(Game1.Current.Content, assetName, rect, frames);
        }

        public void GetItem()
        {
            switch (ItemType)
            {
                case (int)Enums.ItemType.NukeAttack:

                    GetNukeAttack();

                    break;

                case (int)Enums.ItemType.HealthPowerUp:

                    GetHealthPowerUp();

                    break;

                case (int)Enums.ItemType.EnergyPowerUp:

                    GetEnergyPowerUp();

                    break;

                case (int)Enums.ItemType.TeleportSpell:

                    GetTeleportSpell();

                    break;
            }
        }

        private void GetEnergyPowerUp()
        {
            Player playerObj = Game1.Current.player;

            Random rand = new Random();

            float minEnergyPercent = 0.25f;
            float maxEnergyPercent = 0.50f;

            int power = rand.Next((int)(playerObj.MaxPower * minEnergyPercent), (int)(playerObj.MaxPower * maxEnergyPercent));

            playerObj.Power = (power + playerObj.Power) > playerObj.MaxPower ? (power + playerObj.Power) : playerObj.MaxPower;
        }

        private void GetHealthPowerUp()
        {
            Player playerObj = Game1.Current.player;

            Random rand = new Random();

            float minHealthPercent = 0.25f;
            float maxHealthPercent = 0.50f;

            int health = rand.Next((int)(playerObj.MaxHealth * minHealthPercent), (int)(playerObj.MaxHealth * maxHealthPercent));

            playerObj.Health = (health + playerObj.Health) > playerObj.MaxHealth ? (health + playerObj.Health) : playerObj.MaxHealth;
        }

        private void GetNukeAttack()
        {
            Game1 game = Game1.Current;

            Attack attackObj = new Attack();
            attackObj.Active = true;
            attackObj.Key = GetNextAvaliableKey(game.player.attackList.Count);
            attackObj.AttackType = Enums.AttackType.Bullet;
            attackObj.AttackSubType = Enums.AttackSubType.Nuke;
            attackObj.CoolDown = 3000;
            attackObj.Texture = game.Content.Load<Texture2D>("Images\\raindrop");
            attackObj.IconTexture = game.Content.Load<Texture2D>("Images\\skill4");
            attackObj.HasIcon = true;
            attackObj.AttackCost = 60;
            attackObj.MinDamage = 50;
            attackObj.MaxDamage = 100;
            game.player.attackList.Add(attackObj);
        }

        private void GetTeleportSpell()
        {
            Game1 game = Game1.Current;

            Attack attackObj = new Attack();
            attackObj.Active = true;
            attackObj.Key = GetNextAvaliableKey(game.player.attackList.Count);
            attackObj.AttackType = Enums.AttackType.MovementEffect;
            attackObj.AttackSubType = Enums.AttackSubType.Teleport;
            attackObj.CoolDown = 10000;
            attackObj.IconTexture = game.Content.Load<Texture2D>("Images\\teleport_image");
            attackObj.HasIcon = true;
            attackObj.AttackCost = 20;
            game.player.attackList.Add(attackObj);
        }

        private Keys GetNextAvaliableKey(int attackListSize)
        {
            switch (attackListSize)
            {
                case 1: return Keys.D1;
                case 2: return Keys.D2;
                case 3: return Keys.D3;
                case 4: return Keys.D4;
                case 5: return Keys.D5;
                case 6: return Keys.D6;
                case 7: return Keys.D7;
                case 8: return Keys.D8;
                case 9: return Keys.D9;
            }

            return Keys.P;
        }
    }
}
