﻿using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class ItemOption
    {
        public Item Item { get; internal set; }
        public string Text { get; internal set; }
        public List<Person> People { get; internal set; }
    }

    public static class ItemOptions
    {
		public static Dictionary<int, List<ItemOption>> ItemToItemOptions = new Dictionary<int, List<ItemOption>>()
		{
			{
				UltimaData.ITEM_RUNE_HONESTY, new List<ItemOption>
				{
					new ItemOption { Item = new Item(0x05, 0x08, 0x06), People = new List<Person>() { new Person { KeywordResponse2 = "Search for the rune of honesty by Mariah's gold!", Name = "William" } } },
                   	new ItemOption { Item = new Item(0x05, 0x1D, 0x01), People = new List<Person>() { new Person { KeywordResponse2 = "Search for the rune of honesty by Tracie's gold!", Name = "William" } } },
					new ItemOption { Item = new Item(0x05, 0x1E, 0x06), People = new List<Person>() { new Person { KeywordResponse2 = "Search in the empty inn room for the rune of honesty!", Name = "William" } } },
					new ItemOption { Item = new Item(0x05, 0x1C, 0x18), People = new List<Person>() { new Person { KeywordResponse2 = "Search in the empty bed at the healers for the rune of honesty!", Name = "William" } } },
					//new ItemOption { Item = new Item(0x05, 0x17, 0x24), People = new List<Person>() { new Person { KeywordResponse2 = "I left the rune of honesty as a tip at the Sage Deli.", Person = "William" } },
					new ItemOption { Item = new Item(0x02, 0x1B, 0x07), People = new List<Person>() { new Person { KeywordResponse2 = "The Lord of the Lycaeum has the rune of honesty in his treasury for safe keeping.", Name = "William" } } },
					new ItemOption { Item = new Item(0x05, 0x1F, 0x0F), People = new List<Person>() { new Person { KeywordResponse2 = "Search behind the Sage Deli.", Name = "William" } } },
                }
			},
			{
				UltimaData.ITEM_RUNE_COMPASSION, new List<ItemOption>
				{
					new ItemOption { Item = new Item(0x06, 0x19 , 0x01), People = new List<Person>() { new Person { KeywordResponse2 = "The rune of compassion lies at the end of a hall somewhere in this towne.", Name = "Pepper" } } }, // Pepper
         			new ItemOption { Item = new Item(0x06, 0x01, 0x16), People = new List<Person>() { new Person { KeywordResponse2 = "The rune of compassion is hidden with the bard Julio.", Name = "Pepper" } } },
					new ItemOption { Item = new Item(0x06, 0x1B, 0x1B), People = new List<Person>() { new Person { KeywordResponse2 = "The rune of compassion lies near the children's campfire.", Name = "Pepper" } } },
					new ItemOption { Item = new Item(0x06, 0x1E, 0x07), People = new List<Person>() { new Person { KeywordResponse2 = "I left the rune of compassion in my room at the inn. Feel free to take it.", Name = "Pepper" } } },
					new ItemOption { Item = new Item(0x03, 0x09, 0x17), People = new List<Person>() { new Person { KeywordResponse2 = "The rune of compassion lies in the shrine at Empath Abbey.", Name = "Pepper" } } },
					new ItemOption { Item = new Item(0x03, 0x05, 0x0C), People = new List<Person>() { new Person { KeywordResponse2 = "The Lord of Empath Abbey hath the rune of compassion in his treasury.", Name = "Pepper" } } },
				}
			},
			{
				UltimaData.ITEM_RUNE_VALOR, new List<ItemOption>
				{
					new ItemOption { Item = new Item(0x07, 0x1E, 0x1E), People = new List<Person>() { new Person { KeywordResponse2 = "The rune is buried in a tower!", Name = "Nostro" } } }, //Nostro
					new ItemOption { Item = new Item(0x07, 0x1E, 0x01), People = new List<Person>() { new Person { KeywordResponse2 = "The rune is buried in the northeast tower!", Name = "Nostro" } } },
					new ItemOption { Item = new Item(0x07, 0x05, 0x14), People = new List<Person>() { new Person { KeywordResponse2 = "The rune is in the southern guard tower!", Name = "Nostro" } } },
					new ItemOption { Item = new Item(0x07, 0x10, 0x1B), People = new List<Person>() { new Person { KeywordResponse2 = "The rune is on Geoffrey's nightstand!", Name = "Nostro" } } },
					new ItemOption { Item = new Item(0x07, 0x05, 0x0A), People = new List<Person>() { new Person { KeywordResponse2 = "The rune is in the northern guard tower!", Name = "Nostro" } } },
					new ItemOption { Item = new Item(0x04, 0x12, 0x19), People = new List<Person>() { new Person { KeywordResponse2 = "Look in the eastern gatehouse of Serpent's Hold for the rune.", Name = "Nostro" } } },
				}
			},
			{
				UltimaData.ITEM_RUNE_JUSTICE, new List<ItemOption>
				{
					new ItemOption { Item = new Item(0x08, 0x0D, 0x06), People = new List<Person>() { new Person { No = "Then do thy penance in a cell, and with the felon search ye well.", Name = "Talfourd" } } }, //Talfourd - No@Pub - Rune
					new ItemOption { Item = new Item(0x08, 0x0E, 0x08), People = new List<Person>() { new Person { No = "Then do thy penance in a cell, and with the beggars search ye well.", Name = "Talfourd" } } },
					new ItemOption { Item = new Item(0x08, 0x1E, 0x12), People = new List<Person>() { new Person { No = "Search ye by the mage in the woods.", Name = "Talfourd" } } },
					//new ItemOption { Item = new Item(0x0B, 0x0A, 0x06), People = new List<Person>() { new Person { No = "Learn where your actions may lead. Ask at the tavern in the Den of thieves.@Rune of Justice, eh? The Balron guards it well." },
					//new ItemOption { Item = new Item(0x0B, 0x05, 0x1C), People = new List<Person>() { new Person { No = "Learn where your actions may lead. Ask at the tavern in the Den of thieves.@Rune of Justice, eh? Search ye by the sleepy sorceress." },
					//new ItemOption { Item = new Item(0x0B, 0x1B, 0x1B), People = new List<Person>() { new Person { No = "Learn where your actions may lead. Ask at the tavern in the Den of thieves.@Rune of Justice, eh? Search ye on the isle with Starlight." },
				}
			},
			{
				UltimaData.ITEM_RUNE_SACRIFICE, new List<ItemOption>
				{
					new ItemOption { Item = new Item(0x09, 0x1C, 0x1E), People = new List<Person>() { new Person { KeywordResponse2 = "The rune of sacrifice is hard to get. It lies within the fires of the forge!", Name = "Mischief" } } }, //Mischief
					new ItemOption { Item = new Item(0x09, 0x17, 0x0B), People = new List<Person>() { new Person { KeywordResponse2 = "The rune of sacrifice lies in damp ground on the west side of the poorhouse.", Name = "Mischief" } } }, //Mischief
					new ItemOption { Item = new Item(0x09, 0x0C, 0x04), People = new List<Person>() { new Person { KeywordResponse2 = "Ha! I placed the rune of sacrifice in the plant near my brother.", Name = "Mischief" } } }, //Mischief
					new ItemOption { Item = new Item(0x09, 0x0A, 0x02), People = new List<Person>() { new Person { KeywordResponse2 = "Search well in the room in the Inn with three beds.", Name = "Mischief" } } }, //Mischief
					new ItemOption { Item = new Item(0x04, 0x18, 0x0B), People = new List<Person>() { new Person { KeywordResponse2 = "The rune of sacrice is far from here! Search ye in Courage Healers of Serpent's Hold.", Name = "Mischief" } } }, //Mischief
					new ItemOption { Item = new Item(0x0F, 0x16, 0x1B), People = new List<Person>() { new Person { KeywordResponse2 = "Search for the rune of sacrifice in the treasure chamber of Vesper!", Name = "Mischief" } } } //Mischief
				}
			},
			{
				UltimaData.ITEM_RUNE_HONOR, new List<ItemOption>
				{
					new ItemOption { Item = new Item(0x0A, 0x02, 0x1D), People = new List<Person>() { new Person { KeywordResponse2 = "It is buried in the southwest corner of towne.", Name = "Terrin" } } }, // Terrin
					new ItemOption { Item = new Item(0x0A, 0x1E, 0x01), People = new List<Person>() { new Person { KeywordResponse2 = "Search ye by the back door of the inn.", Name = "Terrin" } } }, // Terrin
					new ItemOption { Item = new Item(0x0A, 0x0C, 0x01), People = new List<Person>() { new Person { KeywordResponse2 = "The innkeeper has it with his treasure.", Name = "Terrin" } } }, // Terrin
					new ItemOption { Item = new Item(0x0A, 0x1E, 0x0A), People = new List<Person>() { new Person { KeywordResponse2 = "The skeleton keeps it nearby. Says it helps keep its thoughts clear.", Name = "Terrin" } } }
				}
			},
			{
				UltimaData.ITEM_RUNE_SPIRITUALITY, new List<ItemOption>
				{
					new ItemOption { Item = new Item(0x01, 0x11, 0x08), People = new List<Person>() { new Person { KeywordResponse2 = "Search for the rune of spirituality in the treasure chamber of Britannia!", Name = "the Ankh of\nSpirituality" } } }, // Ankh of Spirituality
					new ItemOption { Item = new Item(0x0B, 0x05, 0x18), People = new List<Person>() { new Person { KeywordResponse2 = "Search in the empty bed at the healers.", Name = "the Ankh of\nSpirituality" } } }, // Ankh of Spirituality
					new ItemOption { Item = new Item(0x01, 0x0A, 0x18), People = new List<Person>() { new Person { KeywordResponse2 = "The seer hath the rune of Spirituality.", Name = "the Ankh of\nSpirituality" } } }, // Ankh of Spirituality
					new ItemOption { Item = new Item(0x01, 0x0D, 0x16), People = new List<Person>() { new Person { KeywordResponse2 = "Search for the rune of spirituality with the captive reaper!", Name = "the Ankh of\nSpirituality" } } },
					new ItemOption { Item = new Item(0x10, 0x02, 0x18), People = new List<Person>() { new Person { KeywordResponse2 = "Search for the rune of spirituality by those who study the Axiom in Cove!", Name = "the Ankh of\nSpirituality" } } }
				}
			},
			{
				UltimaData.ITEM_RUNE_HUMILITY, new List<ItemOption>
				{
					new ItemOption { Item = new Item(0x0D, 0x1D, 0x1D), People = new List<Person>() { 
						new Person { KeywordResponse1 = "Around there in the nook of the mountains!", Name = "Wheatpin" },
						new Person { KeywordResponse2 = "Search the hills in the south-east corner of town!", Name = "Barren", Town = "PAWS" },
					} }, // Wheatpin@Barren
					new ItemOption { Item = new Item(0x0D, 0x1E, 0x01), People = new List<Person>() { 
						new Person { KeywordResponse1 = "It was just in the nook there but someone moved it.", Name = "Wheatpin" },
						new Person { KeywordResponse2 = "Search the brush in the north-east corner of town!", Name = "Barren", Town = "PAWS" },
					} }, // Wheatpin@Barren
					new ItemOption { Item = new Item(0x0D, 0x0D, 0x04), People = new List<Person>() { 
						new Person { KeywordResponse1 = "It was just in the nook there but someone moved it.", Name = "Wheatpin" },
						new Person { KeywordResponse2 = "Search in the magic shop!", Name = "Barren", Town = "PAWS" },
					} }, // Wheatpin@Barren
					new ItemOption { Item = new Item(0x0D, 0x16, 0x19), People = new List<Person>() { 
						new Person { KeywordResponse1 = "It was just in the nook there but someone moved it.", Name = "Wheatpin" },
						new Person { KeywordResponse2 = "Search in the bull pen!", Name = "Barren", Town = "PAWS" },
					} }, // Wheatpin@Barren
					new ItemOption { Item = new Item(0x0D, 0x1A, 0x08), People = new List<Person>() { 
						new Person { KeywordResponse1 = "It was just in the nook there but someone moved it.", Name = "Wheatpin" },
						new Person { KeywordResponse2 = "Oh, I gave it to the lumberjack. Seemed like he needed the help.", Name = "Barren", Town = "PAWS" },
					} }
					//Island with the vineyard
					}
			}
		};
	}
}



