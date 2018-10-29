using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Block_Explorer_API.Models
{
    /// <summary>
    /// Model for Transaction Inputs inside TX object
    /// </summary>
    public class TxIns
    {
        /// <summary>
        /// Total amount of coins in a Transaction
        /// </summary>
        public string SpentCoins { get; set; }
        /// <summary>
        /// Who owns the coins
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// TXIns constructor class
        /// </summary>
        public TxIns()
        {

        }

    }
}