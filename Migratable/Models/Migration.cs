namespace Migratable.Models
{
    public class Migration
    {
        public int Version;
        public string Name;
        public string Up;
        public string Down;

        public override string ToString()
        {
            return $"V{Version} - {Name}";
        }
    }
}
