namespace U4DosRandomizer
{
    public interface IAvatarOffset
    {
        int BELL_REQUIREMENT_OFFSET { get; }
        int BOOK_REQUIREMENT_OFFSET { get; }
        int CANDLE_REQUIREMENT_OFFSET { get; }
        int MOONGATE_X_OFFSET { get; }
        int MOONGATE_Y_OFFSET { get; }
        int AREA_X_OFFSET { get; }
        int AREA_Y_OFFSET { get; }
        int DEATH_EXIT_X_OFFSET { get; }
        int DEATH_EXIT_Y_OFFSET { get; }
        int PIRATE_COVE_X_OFFSET { get; }
        int PIRATE_COVE_Y_OFFSET { get; }
        int PIRATE_COVE_SHIP_TILES { get; }
        int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1 { get; }
        int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1 { get; }
        int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET2 { get; }
        int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET2 { get; }
        int WORD_OF_PASSAGE { get; }
        int MONSTER_HP_OFFSET { get; }
        int MONSTER_LEADER_TYPES_OFFSET { get; }
        int MONSTER_ENCOUNTER_SIZE_OFFSET { get; }
        int ALTAR_EXIT_DESTINATION { get; }
        int AMBUSH_MONSTER_TYPES { get; }
        int CITY_RUNE_MASK_PAIRS_OFFSET { get; }
        int ITEM_LOCATIONS_OFFSET { get; }
        int MONSTER_DAMAGE_BITSHIFT_OFFSET { get; }
        int WEAPON_DAMAGE_OFFSET { get; }
        int MONSTER_SPAWN_TIER_ONE { get; }
        int MONSTER_SPAWN_TIER_TWO { get; }
        int MONSTER_SPAWN_TIER_THREE { get; }
        int MONSTER_QTY_ONE { get; }
        int MONSTER_QTY_TWO { get; }
        int HERB_PRICES { get; }
        int HERB_PRICE_INPUT { get; }
        int LB_TEXT_OFFSET { get; }
        int LB_HELP_TEXT_OFFSET { get; }
        int TAVERN_TEXT_OFFSET { get; }
        int MANTRA_OFFSET { get; }
        int MANTRA_POINTERS_OFFSET { get; }
        int SHRINE_TEXT_OFFSET { get; }
        int WHITE_STONE_LOCATION_TEXT { get; }
        int BLACK_STONE_LOCATION_TEXT { get; }
        int DEMON_SPAWN_TRIGGER_X1_OFFSET { get; }
        int DEMON_SPAWN_TRIGGER_X2_OFFSET { get; }
        int DEMON_SPAWN_TRIGGER_Y1_OFFSET { get; }
        int DEMON_SPAWN_TRIGGER_Y2_OFFSET { get; }
        int DEMON_SPAWN_LOCATION_X_OFFSET { get; }
        int BALLOON_SPAWN_TRIGGER_X_OFFSET { get; }
        int BALLOON_SPAWN_TRIGGER_Y_OFFSET { get; }
        int BALLOON_SPAWN_LOCATION_X_OFFSET { get; }
        int BALLOON_SPAWN_LOCATION_Y_OFFSET { get; }
        int LBC_DUNGEON_EXIT_X_OFFSET { get; }
        int LBC_DUNGEON_EXIT_Y_OFFSET { get; }
        int ITEM_USE_TRIGGER_BELL_X_OFFSET { get; }
        int ITEM_USE_TRIGGER_BELL_Y_OFFSET { get; }
        int ITEM_USE_TRIGGER_BOOK_X_OFFSET { get; }
        int ITEM_USE_TRIGGER_BOOK_Y_OFFSET { get; }
        int ITEM_USE_TRIGGER_CANDLE_X_OFFSET { get; }
        int ITEM_USE_TRIGGER_CANDLE_Y_OFFSET { get; }
        int ITEM_USE_TRIGGER_SKULL_X_OFFSET { get; }
        int ITEM_USE_TRIGGER_SKULL_Y_OFFSET { get; }
        int WEAPON_REQUIRED_FOR_ABYSS { get; }
        int WHIRLPOOL_EXIT_X_OFFSET { get; }
        int WHIRLPOOL_EXIT_Y_OFFSET { get; }
        int USE_PRINCIPLE_ITEM_TEXT { get; }
        int USE_PRINCIPLE_ITEM_BELL_TEXT_POINTERS_OFFSET { get; }
        int USE_PRINCIPLE_ITEM_BOOK_TEXT_POINTERS_OFFSET { get; }
        int USE_PRINCIPLE_ITEM_CANDLE_TEXT_POINTERS_OFFSET { get; }
        int ABYSS_EJECTION_LOCATIONS_X { get; }
        int ABYSS_EJECTION_LOCATIONS_Y { get; }
        int ABYSS_PARTY_COMPARISON { get; }
        int LB_PARTY_COMPARISON { get; }
        int SPELL_RECIPE_OFFSET { get; }
        int BLINK_CAST_EXCLUSION_X1_OFFSET { get; }
        int BLINK_CAST_EXCLUSION_X2_OFFSET { get; }
        int BLINK_CAST_EXCLUSION_Y1_OFFSET { get; }
        int BLINK_CAST_EXCLUSION_Y2_OFFSET { get; }
        int BLINK_DESTINATION_EXCLUSION_X1_OFFSET { get; }
        int BLINK_DESTINATION_EXCLUSION_X2_OFFSET { get; }
        int BLINK_DESTINATION_EXCLUSION_Y1_OFFSET { get; }
        int BLINK_DESTINATION_EXCLUSION_Y2_OFFSET { get; }
        int ENABLE_TOWN_SAVE1 { get; }
        int ENABLE_TOWN_SAVE2 { get; }
        int ENABLE_TOWN_SAVE3 { get; }
        int ENABLE_TOWN_SAVE4 { get; }
        int ENABLE_MIX_QUANTITY_OFFSET { get; }
        int ENABLE_SLEEP_BACKOFF_OFFSET { get; }
        int ENABLE_DAEMON_TRIGGER_FIX { get; }
        int ENABLE_MAP_EDGE_FIX1 { get; }
        int ENABLE_MAP_EDGE_FIX2 { get; }
        int ENABLE_MAP_EDGE_FIX3 { get; }
        int ENABLE_AWAKEN_ALL { get; }
        int ENABLE_ACTIVE_PLAYER_1_OFFSET { get; }
        int ENABLE_HIT_CHANCE_OFFSET { get; }
        int ENABLE_DIAGONAL_ATTACK_OFFSET { get; }
        int ENABLE_SACRIFICE_FIX_OFFSET { get; }
        int ENABLE_PRINCIPLE_ITEM_REORDER_OFFSET { get; }
        int ENABLE_WEAPON_OVERFLOW_FIX { get; }
        int BLINK_DESTINATION2_EXCLUSION_X1_OFFSET { get; }
        int BLINK_DESTINATION2_EXCLUSION_X2_OFFSET { get; }
        int BLINK_DESTINATION2_EXCLUSION_Y1_OFFSET { get; }
        int BLINK_DESTINATION2_EXCLUSION_Y2_OFFSET { get; }

        int ENCODED_FLAGS_OFFSET { get; }
        int SEED_OFFSET { get; }
        int SHOP_LOCATION_OFFSET { get; }
    }
}