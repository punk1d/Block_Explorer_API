using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Block_Explorer_API.Models
{
    /// <summary>
    /// Model for Transaction Outputs inside TX object
    /// </summary>
    public class TxOuts
    {
        /// <summary>
        /// Address who will receive the coins
        /// </summary>
        public string DestinationAddress { get; set; }
        
        /// <summary>
        /// Amount to be sent
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// TXOuts class constructor
        /// </summary>
        public TxOuts()
        {

        }
    }
}