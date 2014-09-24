using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PulseGame.Attacks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace PulseGame.Agents
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
            
            LoadContent(PulseGame.Current.Content, assetName, rect, frames);
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
            Player playerObj = PulseGame.Current.player;

            Random rand = new Random();

            float minEnergyPercent = 0.25f;
            float maxEnergyPercent = 0.50f;

            int power = rand.Next((int)(playerObj.MaxPower * minEnergyPercent), (int)(playerObj.MaxPower * maxEnergyPercent));

            playerObj.Power = (power + playerObj.Power) > playerObj.MaxPower ? (power + playerObj.Power) : playerObj.MaxPower;
        }

        private void GetHealthPowerUp()
        {
            Player playerObj = PulseGame.Current.player;

            Random rand = new Random();

            float minHealthPercent = 0.25f;
            float maxHealthPercent = 0.50f;

            int health = rand.Next((int)(playerObj.MaxHealth * minHealthPercent), (int)(playerObj.MaxHealth * maxHealthPercent));

            playerObj.Health = (health + playerObj.Health) > playerObj.MaxHealth ? (health + playerObj.Health) : playerObj.MaxHealth;
        }

        private void GetNukeAttack()
        {
            PulseGame.Current.player.skillList[(int)Enums.PlayerSkills.Nuke].IsUnlocked = true;
        }

        private void GetTeleportSpell()
        {
            PulseGame.Current.player.skillList[(int)Enums.PlayerSkills.Teleport].IsUnlocked = true;
        }
    }
}
