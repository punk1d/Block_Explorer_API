using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Http;
using Block_Explorer_API.Models;
using Block_Explorer_API.BL;
using NBitcoin;
using QBitNinja.Client.Models;

namespace Block_Explorer_API.Controllers
{
    /// <summary>
    /// API to get Transactions out of TestNet (multiple net support in development).
    /// </summary>
    [CollectionDataContract]
    public class TransactionsController : ApiController
    {
        /// <summary>
        /// transactionActions object allows interacting with NBitcoin and QBitNinja libraries to get Transaction information
        /// </summary>
        public Transactions transactionActions = new Transactions();
        
        // GET: api/Transactions
        /// <summary>
        /// Transactions empty Get method returns a message for the user not to leave Transaction Id emtpy.
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "No Transaction Id has been provided."));
        }

        // GET: api/Transactions/5
        /// <summary>
        /// This returns Transaction Details for the given Transaction Id
        /// </summary>
        /// <param name="id">This is the Transaction Id</param>
        /// <returns></returns>
        public IHttpActionResult Get(string id)
        {
            HttpResponseMessage httpResponse = null;
            try
            {
                Tx tx = new Tx();
                var transactionResponse = transactionActions.GetTxResponse(id);
                tx.FetchGeneralTxInfo(transactionResponse);
                tx.TxInputs = transactionActions.GetTransactionInputs(transactionResponse);
                tx.TxOutputs = transactionActions.GetTransactionOutputs(transactionResponse);
                    var response = tx;
                    httpResponse = Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.OK, "Entry provided does not " +
                    "have the proper Transaction Id format. Please try with a valid Transaction Id.");
            }
            return ResponseMessage(httpResponse);
        }
    }
}
