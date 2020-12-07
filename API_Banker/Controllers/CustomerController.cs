using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;


namespace API_Banker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        DataAbstractionLayer _dal = new DataAbstractionLayer();

        //Affiche la page Web de la liste des clients
        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            ViewData["Customers"] = _dal.SelectCustomers();
            return View();
        }

        //Retourne la liste des clients au format JSON
        [HttpGet]
        [Route("view")]
        public IEnumerable<Customer> GetAllCustomers()
        {
            return _dal.SelectCustomers();
        }

        //Affiche la Page de recherche d'un client
        [HttpGet]
        [Route("Search")]
        public ActionResult Search()
        {
            return View();
        }

        //Affiche la page Web du client recherché par son numéro
        [HttpGet]
        [Route("Read")]
        public IActionResult ReadSelectCustomerByNumber(String customerNumber)
        {
            ViewData["Customers"] = _dal.SelectCustomerByNumber(customerNumber);

            if (ViewData["Customers"] != null)
            {
                ViewData["TitlePage"] = "Résultat de la recherche";
                return View("Read");
            }
            else
            {
                ViewData["TitlePage"] = "Client inexistant";
                ViewData["ErrorMsg"] = "Désolé, le client que vous recherchez n'existe pas...Veuillez réessayer.";
                return View("Error");
            }
        }

        //Recherche un client selon son numéro et le renvoi au format JSON
        [HttpGet]
        [Route("view/{customerNumber}")]
        public Customer GetSelectCustomerByNumber(String customerNumber)
        {
            return _dal.SelectCustomerByNumber(customerNumber);
        }

        //Affiche la Page de création d'un nouveau client
        [Route("Create")]
        public ActionResult Create()
        {
            return View();
        }

        //Enregistre un nouveau client et le renvoi au format JSON
        [HttpPost]
        [Route("new")]
        public Customer PostCreateCustomer([FromForm] Customer userEntry)
        {
            return _dal.CreateCustomer(userEntry);
        }

        [HttpGet]
        [Route("Edit/{customerNumber}")]
        public IActionResult Update(String customerNumber)
        {
            ViewData["Customers"] = _dal.SelectCustomerByNumber(customerNumber);
            return View();
        }

        //Edite un client selon les données en entrée
        [HttpPost]
        [Route("update")]
        public IActionResult PostUpdateCustomer(Customer userEntry)
        {
            if (_dal.UpdateCustomer(userEntry))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
