using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;


namespace API_Banker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController : Controller
    {
        DataAbstractionLayer _dal = new DataAbstractionLayer();

        [HttpGet]
        [Route("view/{accountNumber}")]
        public IEnumerable<Transfer> GetTransferListPerAccountTransmitter(String accountNumber)
        {
            return _dal.SelectTransferListByAccountNumber(accountNumber);
        }

        [HttpGet]
        [Route("search")]
        public ActionResult Search()
        {
            return View();
        }

        [HttpGet]
        [Route("getlist")]
        public ActionResult GetTransferList(String accountNumber)
        {
            ViewData["TransferDebit"] = _dal.SelectTransferDebitListByAccountNumber(accountNumber);
            ViewData["TransferCredit"] = _dal.SelectTransferCreditListByAccountNumber(accountNumber);

            if (ViewData["TransferDebit"] != null && ViewData["TransferCredit"] != null)
            {
                return View("View");
            }
            else
            {
                ViewData["TitlePage"] = "Aucun Transfert";
                ViewData["ErrorMsg"] = "Nous ne trouvons pas de transferts pour ce compte";
                return View("Error");
            }
        }

        [HttpGet]
        [Route("view/date")]
        public IEnumerable<Transfer> GetTransferListByDate(Transfer date)
        {
            return _dal.SelectTransferListByDate(date);
        }

        [HttpPost]
        [Route("new/{accountTransmitter}/{accountReceiver}")]
        public IActionResult PostNewTransfer(Transfer userEntry, String accountTransmitter, String accountReceiver)
        {
            if (_dal.NewTransfer(userEntry, accountTransmitter, accountReceiver) == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }
        }
    }
}