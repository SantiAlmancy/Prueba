using Microsoft.AspNetCore.Mvc;
using UPB.CoreLogic.Models;
using UPB.CoreLogic.Managers;

namespace UPB.FinalPractice.Controllers;

[ApiController]
[Route("products")]
public class ProductController : ControllerBase
{
   private readonly ProductManager _productmanager;

   public ProductController(ProductManager productmanager)
   {
      _productmanager = productmanager;
   }

   [HttpPost]
   public Product Post([FromBody]Product patientToCreate)
   {
      return _productmanager.Create(patientToCreate.Name, patientToCreate.Type, patientToCreate.Stock, patientToCreate.Code);
   }

   [HttpGet]
   public List<Product> Get()
   {
      return _productmanager.GetAll();
   }

   [HttpGet]
   [Route("{code}")]
   public Product GetByCode([FromRoute] string code)
   {
      return _productmanager.GetByCode(code);
   }

   [HttpPut]
   [Route("{code}")]
   public Product Update([FromRoute] string code, [FromBody]Product productToUpdate)
   {
      return _productmanager.Update(code, productToUpdate.Name, productToUpdate.Stock);
   }

   [HttpPut]
   [Route("calculate-prices")]
   public async Task<List<Product>> PutAllPrices()
   {
      return (await _productmanager.PutAllPrices());
   }

   
   [HttpPut]
   [Route("calculate-prices/{code}")]
   public async Task<Product> PutPrice([FromRoute] string code)
   {
      return (await _productmanager.PutPrice(code));
   }
   [HttpDelete]
   [Route("{code}")]
   public Product Delete([FromRoute] string code)
   {
       return _productmanager.DeleteByCode(code);
   }
}