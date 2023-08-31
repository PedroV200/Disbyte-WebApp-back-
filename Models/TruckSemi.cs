namespace WebApiSample.Models
{
    public class TruckSemi
    {
        public int id { get; set; }
        public string description { get; set; }
        public double pesomin {get;set;}
        public double pesomax {get;set;}
        public double largo {get;set;}
        public double costindex1 {get;set;}
        public double costindex2 {get;set;}
        public int paisregion_id {get;set;} 
    }

    public class TruckSemiVista
    {
        public int id { get; set; }
        public string description { get; set; }
        public double pesomin {get;set;}
        public double pesomax {get;set;}
        public double largo {get;set;}
        public double costindex1 {get;set;}
        public double costindex2 {get;set;}
        public int paisregion_id {get;set;} 
        public string pais {get;set;}
        public string region {get;set;}
    }
}
