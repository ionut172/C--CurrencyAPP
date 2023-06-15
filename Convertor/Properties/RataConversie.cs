using System;
namespace Convertor
{


    public class RataConversie
    {

        public string FromCurrencyCode { get; set; }
        public string ToCurrencyCode { get; set; }
        public double Rate { get; set; }
        public string Date { get; set; }
        List<Moneda> monede = new List<Moneda>();

        
    }

   
   
}

