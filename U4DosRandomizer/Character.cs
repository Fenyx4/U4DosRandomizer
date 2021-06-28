namespace U4DosRandomizer
{
    public class Character
    {
        public Character()
        {
        }

        public ushort Hp { get; internal set; }
        public ushort MaxHp { get; internal set; }
        public ushort XP { get; internal set; }
        public ushort Str { get; internal set; }
        public ushort Dex { get; internal set; }
        public ushort Int { get; internal set; }
        public ushort Mp { get; internal set; }
        public ushort Weapon { get; internal set; }
        public ushort Armor { get; internal set; }
        public string Name { get; internal set; }
        public char Sex { get; internal set; }
        public byte Class { get; internal set; }
        public char Status { get; internal set; }
    }
}