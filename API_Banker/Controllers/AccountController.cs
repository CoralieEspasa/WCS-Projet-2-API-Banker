using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace API_Banker.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AccountController : Controller
	{
		DataAbstractionLayer _dal = new DataAbstractionLayer();

		[HttpGet]
		[Route("view/amount/{accountNumber}")]
		//Obtenir le solde d'un compte client - Requ�te 1.1)
		public Object GetAccountAmount_(string accountNumber)
		{
			return _dal.SelectAccountAmount(accountNumber);
		}

		[HttpGet]
		[Route("check/{accountNumber}")]
		//V�rifier si un compte existe - Requ�te 2.1)
		public Object CheckAccountExist(string accountNumber)
		{
			return _dal.CheckAccount(accountNumber);
		}

		[HttpGet]
		[Route("view/info/all")]
		//Obtenir tous les comptes clients
		public List<Account> GetAccountAll()
		{
			return _dal.SelectAccountAll();

		}

		[HttpGet]
		[Route("view/info/{accountNumber}")]
		//Obtenir un client � partir d'un num�ro de compte - Requ�te 3)
		public Account GetAccountAll(string accountNumber)
		{
			return _dal.SelectAccount(accountNumber);

		}

		[HttpGet]
		[Route("view/all")]
		//Obtenir la liste de tous les num�ros de compte - Requ�te 6.2)
		public List<Object> GetAllAccountNumber_()
		{
			return _dal.SelectAllAccountNumber();
		}

		[HttpPost]
		[Route("new")]
		//Cr�ation d'un nouveau compte
		public Account CreateAccount(Account userEntry)
        {
			return _dal.InsertAccount(userEntry);
        }

		[HttpPost]
		[Route("update")]
		//Cr�ation d'un nouveau compte
		public Account ModifyAccount(Account userEntry)
		{
			return _dal.UpdateAccount(userEntry);
		}

		[HttpGet]
		[Route("allaccount")]
		public ActionResult AllAccount()
        {
			ViewData["ListAllAccount"] = _dal.SelectAccountAll();
			return View();
        }

		[HttpGet]
		[Route("searchaccount")]
		public ActionResult AllAccountSearch()
		{
			
			return View();
		}

		//Affiche la page Web du compte recherch� par son num�ro
		[HttpGet]
		[Route("Read")]
		public IActionResult ReadSelectAccountByNumber(String accountNumber)
		{
			ViewData["Account"] = _dal.SelectAccount(accountNumber);

			if (ViewData["Account"] != null)
			{
				ViewData["TitlePage"] = "R�sultat de la recherche";
				return View("AllAccountRead");
			}
			else
			{
				ViewData["TitlePage"] = "Compte inexistant";
				ViewData["ErrorMsg"] = "D�sol�, le compte que vous recherchez n'existe pas...Veuillez r�essayer.";
				return View("Error");
			}
		}
	}

}