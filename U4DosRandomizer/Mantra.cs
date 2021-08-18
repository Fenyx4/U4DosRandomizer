namespace U4DosRandomizer
{
    public class Mantra
    {
        public string Text { get; private set; }
        public string Limerick { get; private set; }

        public Mantra(string text, string limerick)
        {
            this.Text = text;
            this.Limerick = limerick;
        }
    }
}