using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class DungeonTileInfo
    {
        public const byte Nothing = 0x00;	
        public const byte LadderUp                                       = 0x10;	
        public const byte LadderDown                                     = 0x20;	
        public const byte LadderBoth                                      = 0x30;	
        public const byte Chest                                           = 0x40;	
        public const byte HoleUp_unused                         = 0x50;	
        public const byte HoleDown_unused                           = 0x60;	
        public const byte Orb                                  = 0x70;	
        public const byte Trap_Wind                 = 0x80;	
        public const byte TrapFallingRocks               = 0x81;	
        public const byte TrapPit                                        = 0x8E;	
        public const byte Fountain_Ordinary                    = 0x90;	
        public const byte Fountain_Healing               = 0x91;	
        public const byte Fountain_Acid                    = 0x92;	
        public const byte Fountain_Curing                   = 0x93;	
        public const byte Fountain_Poisoned     = 0x94;	
        public const byte Field_Poison                                    = 0xA0;	
        public const byte Field_Energy                                    = 0xA1;	
        public const byte Field_Fire                                      = 0xA2;	
        public const byte Field_Sleep                                     = 0xA3;	
        public const byte AltarOrStone           = 0xB0;	
        public const byte Door                                            = 0xC0;	
        public const byte DungeonRoomStart                              = 0xD0; // To DF
        public const byte HiddenDoor                                     = 0xE0;	
        public const byte Wall                                            = 0xF0;
    }
}
