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
        List<IMyTerminalBlock> Blocks;


        public void Main()
        {

            Blocks = new List<IMyTerminalBlock>();
            List<IMyAssembler> assemblers = new List<IMyAssembler>();
            List<IMyRefinery> refineries = new List<IMyRefinery>();
            List<IMyRadioAntenna> antennas = new List<IMyRadioAntenna>();
            List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
            List<IMyCargoContainer> cargoContainers = new List<IMyCargoContainer>();

           
            GridTerminalSystem.GetBlocks(Blocks);
            GridTerminalSystem.GetBlocksOfType(assemblers);
            GridTerminalSystem.GetBlocksOfType(refineries);
            GridTerminalSystem.GetBlocksOfType(antennas);
            GridTerminalSystem.GetBlocksOfType(textPanels);
            GridTerminalSystem.GetBlocksOfType(cargoContainers);

            foreach (IMyAssembler block in assemblers)
            {
                
                Echo(block.CustomName);
            }

            foreach (IMyRefinery block in refineries)
            {

                Echo(block.CustomName);
            }
            foreach (IMyRadioAntenna block in antennas)
            {

                Echo(block.CustomName);
            }
            foreach (IMyTextPanel block in textPanels)
            {

                Echo(block.CustomName);
            }
            foreach (IMyCargoContainer block in cargoContainers)
            {

                Echo(block.CustomName);
            }
            //Dictionary<string, IMyInventoryItem> InventoryObjects = GetInvetoryObjects();

            //IMyTerminalBlock programmableBlock = Blocks.Where(b => b.CustomName == "Programmable block").FirstOrDefault();
            //programmableBlock.CustomData = "";

            //foreach (KeyValuePair<string, IMyInventoryItem> nameObjectPair in InventoryObjects)
            //{
            //    programmableBlock.CustomData += string.Format("{0}-{1}\n", nameObjectPair.Key, nameObjectPair.Value.Amount.ToString());
            //    Echo(nameObjectPair.Key + " " + nameObjectPair.Value.Amount.ToString());
            //}
        }

        public List<IMyInventory> GetInventories()
        {
            List<IMyInventory> inventory = new List<IMyInventory>();
            List<IMyTerminalBlock> blocksWIthInventory = GetBlocksWithInventory();

            foreach(IMyTerminalBlock block in blocksWIthInventory)
            {
                for(int x = 0; x < block.InventoryCount; x++)
                {
                    inventory.Add(block.GetInventory(x));
                }
            }
        
            return inventory;

        }

        public Dictionary<string, IMyInventoryItem> GetInvetoryObjects()
        {
            Dictionary<string, IMyInventoryItem> inventory = new Dictionary<string, IMyInventoryItem>();
            List<IMyInventoryItem> items = GetInventoryItems();

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

        public List<IMyInventoryItem> GetInventoryItems()
        {
            List<IMyInventoryItem> items = new List<IMyInventoryItem>();
            List<IMyInventory> inventory = GetInventories();

            foreach (IMyInventory inv in inventory)
            {
                items.AddRange(inv.GetItems());
            }


            return items;
        }

        public List<IMyTerminalBlock> GetBlocksWithInventory()
        {
            List<IMyTerminalBlock> BlocksWithInventory = new List<IMyTerminalBlock>();

            BlocksWithInventory.AddRange(Blocks.Where(b => b.HasInventory));

            return BlocksWithInventory;
        }

        

        public string GetInventoryItemByName(IMyInventoryItem item)
        {
            string itemName = item.GetDefinitionId().ToString();

            char[] delimChar = "/".ToCharArray();

            string[] newString = itemName.Split(delimChar);

            return newString[1];
        }  
    }
}
