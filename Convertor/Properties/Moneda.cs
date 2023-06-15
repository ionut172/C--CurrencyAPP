using System;
namespace Convertor
{
    public class Moneda
    {
        public string Code { get; set; }
        public string AlphaCode { get; set; }
        public string NumericCode { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
        public decimal InverseRate { get; set; }
    }

}

