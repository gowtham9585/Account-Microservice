using Account_Microservice.Model;
using Account_Microservice.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
     //   List<Customer> customerList = new List<Customer>();
     //   readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AccountController));
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

       

        [HttpPost("createAccount")]
        public IActionResult createAccount([FromBody] dynamic obj)
        {
            ResponseObj responseObj = new ResponseObj();
            if (Convert.ToInt32(obj.CustomerId) == 0)
            {
               
                responseObj.isSuccess = false;
                responseObj.resMessage = "User has sent some invalid CustomerId";
                responseObj.resObject = null;



                return NotFound(responseObj);
            }
            try
            {
              //  customerList.Add(obj);
                
                AccountCreationStatus acs, acs1 = new AccountCreationStatus();

                acs = _service.AddAccount(Convert.ToInt32(obj.CustomerId), "Savings");
               
                acs1 = _service.AddAccount(Convert.ToInt32(obj.CustomerId), "Current");
              
                responseObj.isSuccess = true;
                responseObj.resMessage = "Current account has been successfully created";
                //  responseObj.resMessage = "Saving account has been successfully created";
                responseObj.resObject = acs1;

                return Ok(responseObj);
            }
            catch (Exception e)
            {
                
                responseObj.isSuccess = false;
                responseObj.resMessage = "User already exists";
                responseObj.resObject = null;
                return BadRequest(responseObj);
            }
        }

       
        [HttpGet]
        [Route("getCustomerAccounts/{CustomerId}")]
        public IActionResult getCustomerAccounts(int CustomerId)
        {
            if (CustomerId == 0)
            {
                
                return NotFound();
            }
            try
            {

                var Listaccount = _service.getAllAccounts(CustomerId);
                return Ok(Listaccount);

            }
            catch (Exception e)
            {
             
                throw e;
            }
        }
        
        [HttpGet]
        [Route("getAllCustomerAccounts")]
        public IActionResult getAllCustomerAccounts()
        {
            List<Account> Listaccount = new List<Account>();
            try
            {
                Listaccount = _service.getCustomersAllAccounts().ToList();

            }
            catch (Exception e)
            {
               
                throw e;
            }

           
            return Ok(Listaccount);
        }


       
        [HttpGet]
        [Route("getAccount/{AccountId}")]
        public IActionResult getAccount(int AccountId)
        {
            if (AccountId == 0)
            {
             
                return NotFound();

            }
            try
            {
                Account a = new Account();
                a = _service.getCustomerAccount(AccountId);
             
                return Ok(a);
            }
            catch (Exception e)
            {
              
                return NotFound(new { status = 404, reqMessage = "Customer ID does not exist" });

            }

        }


       
        [HttpPost("getAccountStatement")]
       
        public IActionResult getAccountStatement([FromBody] AccountStatement obj)
        {
            bool flag = _service.isAccountThere(obj.AccountId);
            if (Convert.ToInt32(obj.AccountId) == 0 || !flag)
            {
              
                return NotFound(new { status = 404, isSuccess = false, resMessage = "Couldn't find account, enter valid ID" });
            }
            try
            {
                List<Transactions> statements = new List<Transactions>();





                statements = _service.getStatement(Convert.ToInt32(obj.AccountId), Convert.ToDateTime(obj.from_date), Convert.ToDateTime(obj.to_date));

                if (statements.Count > 0)
                    return Ok(new { isSuccess = true, resObject = statements });
                else
                    return NotFound(new { status = 404, isSuccess = false, resMessage = "Couldn't find transactions during this time period" });
             


            }
            catch (Exception e)
            {
             
                throw e;
            }
        }



        
        [HttpPost("deposit")]
        public IActionResult deposit([FromBody] dynamic obj)
        {
            if (Convert.ToInt32(obj.accountId) == 0 || Convert.ToInt32(obj.amount) == 0)
            {
              

                return NotFound(new { isSuccess = false, resMessage = "Invalid account id" });
            }

            try
            {
                TransactionStatus ts = new TransactionStatus();
                ts = _service.depositAccount(Convert.ToInt32(obj.accountId), Convert.ToInt32(obj.amount));
              
                return Ok(ts);
            }
            catch (Exception e)
            {
              
                return NotFound(new { isSuccess = false, resMessage = "Invalid account ID or Insufficient balance" });
            }

        }

       
        [HttpPost("withdraw")]
        public IActionResult withdraw([FromBody] dynamic obj)
        {

            if (Convert.ToInt32(obj.accountId) == 0 || Convert.ToInt32(obj.amount) == 0)
            {

                return NotFound();
            }
            try
            {
                TransactionStatus ts = new TransactionStatus();
                ts = _service.withdrawAccount(Convert.ToInt32(obj.accountId), Convert.ToInt32(obj.amount));

              
                return Ok(ts);

            }

            catch (Exception e)
            {
                
                throw e;
            }


        }

      /*  [HttpGet]
        [Route("getCustomerDetails/{id}")]
        public Customer getCustomerDetails(int id)
        {
            try
            {
               return customerList.Find(p => p.CustomerId == id); 
            }catch(Exception e)
            {
                throw e;
            }
        } */  
    }
}
