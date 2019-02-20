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

            List<IMyCargoContainer> primaryStorage = GetPrimaryStorage(cargoContainers);


            OrganizeInventory(primaryStorage);
            //foreach (IMyCargoContainer block in cargoContainers)
            //{

            //    Echo(block.DefinitionDisplayNameText + " " + IsLargeCargo(block));
            //}


            //Dictionary<string, IMyInventoryItem> InventoryObjects = GetInvetoryObjects();

            //IMyTerminalBlock programmableBlock = Blocks.Where(b => b.CustomName == "Programmable block").FirstOrDefault();
           
            //programmableBlock.CustomData = "";

            //foreach (KeyValuePair<string, IMyInventoryItem> nameObjectPair in InventoryObjects)
            //{
                
            //    programmableBlock.CustomData += string.Format("{0}-{1}\n", nameObjectPair.Key, GetItemTypeName(nameObjectPair.Value));
            //    Echo(nameObjectPair.Key + " " + GetItemTypeName(nameObjectPair.Value));
            //}
        }

        public void OrganizeInventory(List<IMyCargoContainer> primaryStorage)
        {
            List<IMyInventory> inventories = GetInventories();
            

            string componentStorageName = "Component Storage";
            string oreIngotStorageName = "Ore and Ingot Storage";
            string garbageStorageName = "Garbage Storage";
            string gearStorageName = "Gear Storage";

            IMyCargoContainer component = (IMyCargoContainer)GridTerminalSystem.GetBlockWithName(componentStorageName);
            IMyCargoContainer ironOre = (IMyCargoContainer)GridTerminalSystem.GetBlockWithName(oreIngotStorageName);
            IMyCargoContainer garbage = (IMyCargoContainer)GridTerminalSystem.GetBlockWithName(garbageStorageName);
            IMyCargoContainer gear = (IMyCargoContainer)GridTerminalSystem.GetBlockWithName(gearStorageName);

            if(component == null & ironOre == null)
            {
                primaryStorage[0].CustomName = componentStorageName;
                primaryStorage[1].CustomName = oreIngotStorageName;

                component = (IMyCargoContainer)GridTerminalSystem.GetBlockWithName(componentStorageName);
                ironOre = (IMyCargoContainer)GridTerminalSystem.GetBlockWithName(oreIngotStorageName);
            }
            else if (component == null)
            {
                primaryStorage[0].CustomName = componentStorageName;
                component = (IMyCargoContainer)GridTerminalSystem.GetBlockWithName(componentStorageName);
            }
            else if (ironOre == null)
            {
                primaryStorage[1].CustomName = oreIngotStorageName;
                ironOre = (IMyCargoContainer)GridTerminalSystem.GetBlockWithName(oreIngotStorageName);
            }

            for(int y = 0; y < inventories.Count; y++)
            {
                if (inventories[y].GetItems().Count > 0)
                {

                    List<IMyInventoryItem> items = inventories[y].GetItems();

                    for (int x = 0; x < items.Count; x++)
                    {
                        Echo(inventories[y].CurrentMass.ToString());

                        IMyInventoryItem item;
                        item = items[x];
                        inventories[y].TransferItemTo(component.GetInventory(), x, null, true, item.Amount);
                    }

                }
            }
          
        }

        public string GetItemTypeName(IMyInventoryItem item)
        {
            string name = item.Content.ToString();
            char[] delimChars = "_".ToCharArray();
            string[] splitString = name.Split(delimChars);
            name = splitString.Last();


            return name;
        }

        public bool IsLargeCargo(IMyCargoContainer cargo)
        {
            
            bool isLarge = false;
             
            string toSplit = cargo.DefinitionDisplayNameText;

            char[] delimChars = " ".ToCharArray();

            string[] splitString = toSplit.Split(delimChars);

            if (splitString[0] == "Large")
                isLarge = true;

            return isLarge;
        }

        public List<IMyCargoContainer> GetPrimaryStorage(List<IMyCargoContainer> cargoContainers)
        {
            List<IMyCargoContainer> primaryStorage = new List<IMyCargoContainer>();
        
            int numLarge = 2;
            int count = 0;

            
            foreach(IMyCargoContainer cargo in cargoContainers)
            {
                if(IsLargeCargo(cargo) & count < numLarge)
                {
                    
                    primaryStorage.Add(cargo);
                    count++;
                }
            }
            return primaryStorage;
        }

        public List<IMyCargoContainer> GetAuxillaryStorage()
        {
            List<IMyCargoContainer> auxStorage = new List<IMyCargoContainer>();





            return auxStorage;

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
