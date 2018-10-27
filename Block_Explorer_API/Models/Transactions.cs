using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using NBitcoin;
using QBitNinja.Client.Models;
using QBitNinja.Client;
using System.Runtime.Serialization;

namespace Block_Explorer_API.Models
{
    /// <summary>
    /// Transactions class is used to get all information from a given transaction
    /// Other methods in this class are used to build and propagate transactions
    /// </summary>
    public class Transactions
    {
        /// <summary>
        /// Client for QBitNinja library
        /// </summary>
        protected QBitNinjaClient client = new QBitNinjaClient(Network.TestNet);
        
        /// <summary>
        /// BroadcastResponse object to propagate transactions
        /// </summary>
        protected BroadcastResponse broadcastResponse;
        private GetTransactionResponse _transactionResponse;
        private Transaction _transactionDetails;
        private Transaction _transaction;
        private List<ICoin> _receivedCoins;
        private List<ICoin> _spentCoins;
        private OutPoint _outPoint;

        /// <summary>
        /// List for Transaction Outputs
        /// </summary>
        public List<ICoin> ReceivedCoins { get => _receivedCoins; set => _receivedCoins = value; }

        /// <summary>
        /// Get details for a given transaction in NBitcoin format
        /// </summary>
        /// <param name="transaction">Transaction Id to look for</param>
        /// <returns>Transaction object in NBitcoin format</returns>
        public Transaction GetTransactionDetails(string transaction)
        {
            var tx = uint256.Parse(transaction);
            _transactionResponse = client.GetTransaction(tx).Result;
            _transactionDetails = _transactionResponse.Transaction;
            return _transactionDetails;
        }

        /// <summary>
        /// Get details for a given transaction in QBitNinja format
        /// </summary>
        /// <param name="transaction">Transaction Id to look for</param>
        /// <returns>Transaction object in QBitNinja format</returns>
        public GetTransactionResponse GetTxResponse(string transaction)
        {
            try
            {
                var tx = uint256.Parse(transaction);
                _transactionResponse = client.GetTransaction(tx).Result;
            }catch { }
            return _transactionResponse;
        }

        /// <summary>
        /// Get all Transaction Outputs from a specified Transaction
        /// </summary>
        /// <param name="transactionResponse">Transaction Object in QBitNinja format</param>
        /// <returns>List of all Transaction Outputs</returns>
        public List<TxOuts> GetTransactionOutputs(GetTransactionResponse transactionResponse)
        {
            List<TxOuts> allOutputs = new List<TxOuts>();
            _receivedCoins = transactionResponse.ReceivedCoins;
            foreach (var coin in _receivedCoins)
            {
                TxOuts txOut = new TxOuts();
                var amount = (Money)coin.Amount;
                var paymentScript = coin.TxOut.ScriptPubKey;
                var address = paymentScript.GetDestinationAddress(Network.TestNet);
                txOut.Amount = amount.ToString();
                txOut.DestinationAddress = paymentScript.ToString();
                allOutputs.Add(txOut);
            }
            return allOutputs;
        }

        /// <summary>
        /// Get a list of all Transaction Inputs of a given Transaction Id
        /// </summary>
        /// <param name="transactionResponse">Transaction Object in QBitNinja format</param>
        /// <returns>List of Transaction Inputs</returns>
        public List<TxIns> GetTransactionInputs(GetTransactionResponse transactionResponse)
        {
            List<TxIns> allInputs = new List<TxIns>();
            
            _spentCoins = transactionResponse.SpentCoins;
            foreach (var coin in _spentCoins)
            {
                TxIns txIn = new TxIns();
                Money amount = (Money)coin.Amount;
                txIn.SpentCoins = amount.ToString();
                txIn.SenderAddress = coin.TxOut.ScriptPubKey.GetDestinationAddress(Network.TestNet).ToString();
                allInputs.Add(txIn);
            }
            return allInputs;
        }

        /// <summary>
        /// Creates Transaction Output
        /// </summary>
        /// <param name="transactionResponse">Transaction to get the ScriptPubKey</param>
        /// <param name="amount">Total amount</param>
        /// <returns></returns>
        public TxOut CreateTxOut(Transaction transactionResponse, decimal amount)
        {
            Money sendingAmount = new Money(amount, MoneyUnit.BTC);
            var scriptPubKey = transactionResponse.Outputs.First().ScriptPubKey;
            TxOut txOut = new TxOut(sendingAmount, scriptPubKey);
            Console.WriteLine("Transaction Details");
            Console.WriteLine("Amount Sent: " + txOut.Value.ToString());
            Console.WriteLine("Receiver Pub Key: " + txOut.ScriptPubKey);
            return txOut;
        }

        /// <summary>
        /// Gets Transaction Fee of a given Transaction
        /// </summary>
        /// <param name="transaction">Transaction in NBitcoin format</param>
        /// <returns>Total Fee amount in decimal format</returns>
        public decimal GetTransactionFee(Transaction transaction)
        {
            var fee = transaction.GetFee(_spentCoins.ToArray());
            Console.WriteLine("Transaction fee: " + fee.ToDecimal(MoneyUnit.BTC));
            return fee.ToDecimal(MoneyUnit.BTC);
        }

        /// <summary>
        /// Creates an Outpoint
        /// </summary>
        /// <param name="transactionResponse">Transaction object in QBitNinja format</param>
        /// <param name="privateKey">Private Key of the sender address</param>
        /// <returns></returns>
        public OutPoint CreateOutpoint(GetTransactionResponse transactionResponse, Key privateKey)
        {
            var receivedCoins = transactionResponse.ReceivedCoins;
            _outPoint = null;
            foreach (var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == privateKey.ScriptPubKey)
                    _outPoint = coin.Outpoint;
            }
            if (_outPoint == null)
                throw new Exception("Transaction provided does not contain the ScriptPubKey expected");
            return _outPoint;
        }

        /// <summary>
        /// To get a wallet signature
        /// </summary>
        /// <param name="key">Private key of the wallet</param>
        /// <returns>Returns a signature in string format</returns>
        public string GetSignature(Key key)
        {
            return key.SignMessage("Hola Mundo");
        }

        /// <summary>
        /// Creates a new empty Transaction
        /// </summary>
        /// <returns></returns>
        public Transaction CreateTransaction()
        {
            _transaction = Transaction.Create(Network.TestNet);
            return _transaction;
        }

        /// <summary>
        /// Adds Transaction Inputs for a given transaction
        /// </summary>
        /// <param name="transaction">Transaction in NBitcoin format. 
        /// This is the transaction in which the Input is goint to be added</param>
        /// <param name="txIn">Outpoint to be added to the Input</param>
        /// <returns></returns>
        public Transaction AddTxIn(Transaction transaction, OutPoint txIn)
        {
            transaction.Inputs.Add(new TxIn
            {
                PrevOut = txIn
            });
            _transaction = transaction;
            return _transaction;
        }

        /// <summary>
        /// Constructs a Transaction Output and adds it to a speficied Transaction
        /// </summary>
        /// <param name="transaction">Transaction to add Output to</param>
        /// <param name="receiverAddress">Destination address</param>
        /// <param name="amount">Amount to send</param>
        /// <returns></returns>
        public Transaction AddTxOut(Transaction transaction, Key receiverAddress, Money amount)
        {
            TxOut txOut = new TxOut
            {
                Value = amount,
                ScriptPubKey = receiverAddress.ScriptPubKey
            };
            transaction.Outputs.Add(txOut);
            _transaction = transaction;
            return _transaction;
        }

        /// <summary>
        /// Adds a message into the Transaction Outputs
        /// </summary>
        /// <param name="transaction">Transaction to add the message to</param>
        /// <param name="txMessage">Message in string format</param>
        /// <returns></returns>
        public Transaction AddTxMessage(Transaction transaction, string txMessage)
        {
            var bytes = Encoding.UTF8.GetBytes(txMessage);
            transaction.Outputs.Add(new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
            });
            _transaction = transaction;
            return _transaction;
        }


        public Money CalculateTxChangeAmount(GetTransactionResponse transaction, Money txAmount, Money minerFee, OutPoint outPointToSpend)
        {
            var txInAmount = (Money)transaction.ReceivedCoins[(int)outPointToSpend.N].Amount;
            var changeAmount = txInAmount - txAmount - minerFee;
            return changeAmount;
        }

        public Transaction GetScriptSig(Transaction transaction, uint inputIndex, Key senderAddress)
        {
            transaction.Inputs[inputIndex].ScriptSig = senderAddress.ScriptPubKey;
            _transaction = transaction;
            return _transaction;
        }

        public void PropagateTransaction(Transaction transaction)
        {
            broadcastResponse = client.Broadcast(transaction).Result;
            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success! You can check out the hash of the transaciton in any block explorer:");
                Console.WriteLine(transaction.GetHash());
            }
        }

        public bool VerifySignature(string message, string signature, BitcoinPubKeyAddress address)
        {
            return address.VerifyMessage(message, signature);
        }
    }
}
