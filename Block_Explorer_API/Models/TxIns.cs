using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Block_Explorer_API.Models
{
    public class TxIns
    {
        //public int TotalInputs { get; set; }
        public string SpentCoins { get; set; }
        public string SenderAddress { get; set; }

        public TxIns()
        {

        }

    }
}