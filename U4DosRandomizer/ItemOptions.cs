using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class ItemOptions
    {

        public Dictionary<int, Dictionary<Item, string>> ItemOptionsToStrings = new Dictionary<int, Dictionary<Item, string>>()
        {
            { 
                UltimaData.ITEM_RUNE_HONESTY, new Dictionary<Item, string>
                {
                    { new Item(0x05, 0x08, 0x06), "Search for the rune of honesty by Mariah's gold!" }, // William
                    { new Item(0x05, 0x29, 0x01), "Search for the rune of honesty by Tracie's gold!" },
                    { new Item(0x05, 0x30, 0x06), "Search in the empty inn room for the rune of honesty!" },
                    { new Item(0x05, 0x28, 0x24), "Search in the empty bed at the healers for the rune of honesty!" },
                    { new Item(0x05, 0x17, 0x24), "I left rune of honesty as a tip at the Sage Deli." },
                    { new Item(0x02, 0x27, 0x07), "The Lord of the Lycaeum has the rune of honesty in his treasury for safe keeping." },
                } 
            },
            {
                UltimaData.ITEM_RUNE_COMPASSION, new Dictionary<Item, string>
                {
                    { new Item(0x06, 0x19, 0x01), "The rune of compassion lies at the end of a hall somewhere in this towne." }, // Pepper
                    //{ new Item(0x06, 0x27, 0x07), "The Lord of the Lycaeum has the rune of honesty in his treasury for safe keeping." },
                }
            },
            {
                UltimaData.ITEM_RUNE_VALOR, new Dictionary<Item, string>
                {
                    { new Item(0x07, 0x1E, 0x1E), "The rune is buried in a tower!" }, //Nostro
                    { new Item(0x07, 0x30, 0x01), "The rune is buried in the northeast tower!" },
                    { new Item(0x07, 0x05, 0x20), "The rune is in the southern guard tower!" },
                    { new Item(0x07, 0x17, 0x26), "The rune is on Geoffrey's nightstand!" },
                    { new Item(0x07, 0x05, 0x10), "The rune is in the northern guard tower!" },
                    { new Item(0x04, 0x18, 0x25), "Look in the eastern gatehouse of Serpent's Hold for the rune." },
                }
            },
            {
                UltimaData.ITEM_RUNE_JUSTICE, new Dictionary<Item, string>
                {
                    { new Item(0x08, 0x0D, 0x06), "It is hidden well!@Can thou honestly claim to be guilty of no crime ever?@Doubtful.@Then do thy penance in a cell, and with the felon search ye well." } //Talfourd - Keyword2@Query@Yes@No
                }
            },
            {
                UltimaData.ITEM_RUNE_SACRIFICE, new Dictionary<Item, string>
                {
                    { new Item(0x09, 0x1C, 0x1E), "The rune of sacrifice is hard to get. It lies within the fires of the forge!" } //Mischief
                }
            },
            {
                UltimaData.ITEM_RUNE_HONOR, new Dictionary<Item, string>
                {
                    { new Item(0x0A, 0x02, 0x1D), "It is buried in the southwest corner of towne." } // Terrin
                }
            },
            {
                UltimaData.ITEM_RUNE_SPIRITUALITY, new Dictionary<Item, string>
                {
                    { new Item(0x01, 0x11, 0x08), "Search for the rune of spirituality in the treasure chamber of Britannia!" } // Ankh of Spirituality
                }
            },
            {
                UltimaData.ITEM_RUNE_HUMILITY, new Dictionary<Item, string>
                {
                    { new Item(0x0D, 0x1D, 0x1D), "Around there in the nook of the mountains!@Search the hills in the south-east corner of town!" } // Wheatpin@Barren
                }
            }
        };



    }
}

