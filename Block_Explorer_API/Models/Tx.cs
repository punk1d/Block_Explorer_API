using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NBitcoin;
using QBitNinja.Client.Models;

namespace Block_Explorer_API.Models
{
    /// <summary>
    /// Transaction Class to be passed to controller to construct the http response in the Get call
    /// </summary>
    public class Tx
    {
        /// <summary>
        /// Transaction Id
        /// </summary>
        public string TxId { get; set; }
        
        /// <summary>
        /// Total amount of the transaction 
        /// </summary>
        public string TotalTxAmount { get; set; }
        
        /// <summary>
        /// Transaction Fee. Amount sent to miners to process the given transaction
        /// </summary>
        public string Fee { get; set; }
        
        /// <summary>
        /// Date in which this Transaction was published in the blockchain.
        /// </summary>
        public string DateReceived { get; set; }
        
        /// <summary>
        /// Total cofirmations on the given transaction. More than 6 indicates a successful transaction 
        /// </summary>
        public string TxConfirmations { get; set; }
        
        /// <summary>
        /// Collection of Transaction Inputs. In Transactions, this indicates where the money comes from.
        /// </summary>
        public List<TxIns> TxInputs { get; set; }

        /// <summary>
        /// Collection of Transaction Outputs. In Transactions, this indicates where the money goes.
        /// </summary>
        public List<TxOuts> TxOutputs { get; set; }

        /// <summary>
        /// Hash of the block in which this transaction was recorded
        /// </summary>
        public string BlockHash { get; set; }

        /// <summary>
        /// Block height in the blockchain
        /// </summary>
        public string BlockHeight { get; set; }
        
        /// <summary>
        /// Index of the Transaction inside the Block
        /// </summary>
        public string TxIndex { get; set; }

        /// <summary>
        /// Size of the Transaction message
        /// </summary>
        public string Txsize { get; set; }

        /// <summary>
        /// Message Version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Who added it into the Blockchain
        /// </summary>
        public string RelayedBy { get; set; }
        
        /// <summary>
        /// Constructor for Tx class
        /// </summary>
        public Tx()
        {

        }

        /// <summary>
        /// Gets general information for a given transaction
        /// </summary>
        /// <param name="transactionResponse">Object containing the transaction in QBitNinja format</param>
        public void FetchGeneralTxInfo(GetTransactionResponse transactionResponse)
        {
            try
            {
                TxId = transactionResponse.Transaction.GetHash().ToString();
                TotalTxAmount = transactionResponse.Transaction.TotalOut.ToString();
                Fee = transactionResponse.Fees.ToString();
                DateReceived = transactionResponse.FirstSeen.ToString();
                TxConfirmations = transactionResponse.Block.Confirmations.ToString();
                BlockHash = transactionResponse.Block.GetHashCode().ToString();
                BlockHeight = transactionResponse.Block.Height.ToString();
                //Transaction Index pending
                Txsize = transactionResponse.Transaction.GetSerializedSize().ToString();
                Version = transactionResponse.Transaction.Version.ToString();
                //relayedBy pending
            }
            catch { }
        }

    }
}