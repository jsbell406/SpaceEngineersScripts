using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game.Entity;
using VRage.Game.ModAPI.Ingame;


namespace SpaceEngineersScripts
{
   
    public class InventoryManagement
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        
        
        public void Main()
        {
            
            Dictionary<string, IMyInventoryItem> inventory = GetInvetoryObjects();

            foreach(KeyValuePair<string, IMyInventoryItem> keyValuePair in inventory)
            {
                Echo(keyValuePair.Key + " " + keyValuePair.Value.Amount.ToString());
            }
        }

        public Dictionary<string, IMyInventoryItem> GetInvetoryObjects()
        {
            Dictionary<string, IMyInventoryItem> inventory = new Dictionary<string, IMyInventoryItem>();
            List<IMyInventoryItem> items = GetGridInventoryItems();
            
            foreach (IMyInventoryItem item in items)
            {
                IMyInventoryItem invItem;

                if (inventory.TryGetValue(GetInventoryItemByName(item), out invItem))
                {
                    invItem.Amount += item.Amount;
                }
                else
                {
                    inventory.Add(GetInventoryItemByName(item), item);
                }
            }
            return inventory;
        }


        public string GetInventoryItemByName(IMyInventoryItem item)
        {
            string itemName = item.GetDefinitionId().ToString();

            char[] delimChar = "/".ToCharArray();

            string[] newString = itemName.Split(delimChar);

            return newString[1];
        }

        public List<IMyCargoContainer> GetAllCargoContainer()
        {
            List<IMyCargoContainer> myCargoContainers = new List<IMyCargoContainer>();

            GridTerminalSystem.GetBlocksOfType(myCargoContainers);

            return myCargoContainers;
        }

        public List<IMyInventory> GetMyInventory()
        {
            List<IMyCargoContainer> myCargoContainers = GetAllCargoContainer();

            List<IMyInventory> inventory = new List<IMyInventory>();

            inventory.AddRange(myCargoContainers.Select(c => c.GetInventory()));
          
            return inventory;

        }

        public List<IMyInventoryItem> GetGridInventoryItems()
        {
            List<IMyInventoryItem> items = new List<IMyInventoryItem>();
            List<IMyInventory> inventory = GetMyInventory();

            foreach(IMyInventory inv in inventory)
            {
                items.AddRange(inv.GetItems());
            }


            return items;
        }
    }
}
