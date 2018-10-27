using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Block_Explorer_API.Models
{
    public class TxOuts
    {
        public string DestinationAddress { get; set; }
        public string Amount { get; set; }

        public TxOuts()
        {

        }
    }
}