using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication5.Models
{
    public class arquivo
    {
        public int id { get; set; }
        public string nome { get; set; }
        public byte[] arquivos { get; set; }
    }
}