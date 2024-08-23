namespace LudoGameApi.Models
{
    public class Contest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int entryfee { get; set; }
        public int firstprice { get; set;}
        public int secondprice { get; set; }
        public int thirdprice { get; set; }

    }
}
