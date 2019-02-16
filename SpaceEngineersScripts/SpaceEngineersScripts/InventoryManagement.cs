using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Entity;
using VRage.Game.ModAPI.Ingame;

namespace SpaceEngineersScripts
{
    
    public class InventoryManagement
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        
        
        public void Main()
        {

            Dictionary<string, long> inventoryCounts = GetCountOfInventoryItems();

            foreach(KeyValuePair<string, long> keyValuePair in inventoryCounts)
            {
                //Echo(keyValuePair.Key + " " + keyValuePair.Value.ToString());
            }
        }

        public Dictionary<string,long> GetCountOfInventoryItems()
        {
            Dictionary<string, long> inventoryCounts = new Dictionary<string, long>();
            List<IMyInventoryItem> items = GetGridInventoryItems();
            

            foreach (IMyInventoryItem item in items)
            {
                if (inventoryCounts.TryGetValue(GetInventoryItemByName(item), out long value))
                {
                    inventoryCounts[GetInventoryItemByName(item)] = value + item.Amount.RawValue;
                }
                else
                {
                    inventoryCounts.Add(GetInventoryItemByName(item), item.Amount.RawValue);
                }   
            }
            return inventoryCounts;
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
