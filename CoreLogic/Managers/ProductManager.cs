using System.Collections.Generic;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using UPB.CoreLogic.Models;
using UPB.CoreLogic.Services;

namespace UPB.CoreLogic.Managers;

public class ProductManager
{
    private string _path;
    private ProductService _service;
    
    public ProductManager(string path, ProductService service)
    {
        _path = path;
        _service = service;
    }

    public Product Create(string name, string tipo, int stock, string code)
    {
        if(!tipo.Equals("SOCCER") && !tipo.Equals("BASKET"))
        {
            throw new Exception("Error, tipo de producto no valido");
        }

        string jsonString = File.ReadAllText(_path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<Product> products = JsonSerializer.Deserialize<List<Product>>(jsonString,options);
        // Asignar nuevo código
        // Encontrar el código más alto en la categoría
        int maxCode =0;
        if(products.Any() && products.Find(p => p.Type == tipo) != null)
        {
            maxCode = products.Where(p => p.Type == tipo)
                .Select(p => int.Parse(p.Code.Split('-')[1]))
                .Max();
        }

        // Generar un nuevo código único para el nuevo producto
        string newCode = $"{tipo}-{(maxCode + 1).ToString("000")}";

        Product newProduct = new Product()
        {
            Code = newCode,
            Name = name,
            Type = tipo,
            Stock = stock,
            Price = 0
        };

        products.Add(newProduct);
        string jsonStringUpdated = JsonSerializer.Serialize(products,options);
        File.WriteAllText(_path, jsonStringUpdated);

        return newProduct;
    }

    public List<Product> GetAll()
    {
        if (!File.Exists(_path))
        {
            return new List<Product>();
        }

        string json = File.ReadAllText(_path);
        List<Product> products = JsonSerializer.Deserialize<List<Product>>(json);

        return products;
    }

    public Product GetByCode(string code)
    {
        if (code.Length != 10)
        {
            throw new Exception("Error, el código no es válido");
        }

        string json = File.ReadAllText(_path);
        List<Product> products = JsonSerializer.Deserialize<List<Product>>(json);

        foreach (Product product in products)
        {
            if (product.Code == code)
            {
                return product;
            }
        }

        throw new Exception("Error, no se encontró un producto con el código: " + code);
    }
    
    public Product Update(string code, string name, int stock){
        //encontrar producto con código
        //obtener lista de productos
        string jsonString = File.ReadAllText(_path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<Product> products = JsonSerializer.Deserialize<List<Product>>(jsonString,options);
        if(products.Any() && products.Find(p => p.Code == code) != null)
        {
            Product producttoUpdate = products.Find(p => p.Code == code);
            producttoUpdate.Name = name;
            producttoUpdate.Stock = stock;

            //guardar en json
            string jsonStringUpdated = JsonSerializer.Serialize(products,options);
            File.WriteAllText(_path, jsonStringUpdated);
            return producttoUpdate;
            
        }else{
            throw new Exception("Error, no se encontró el producto");
        }
    }

    public async Task<List<Product>> PutAllPrices()
    {
        string json = File.ReadAllText(_path);
        List<Product> products = JsonSerializer.Deserialize<List<Product>>(json);

        foreach (Product product in products)
        {
            if (product.Price == 0)
            {
                product.Price = await _service.getRandom();
            }
        }

        // Serialize the updated products back to JSON
        string updatedJson = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });

        // Write the updated JSON back to the file
        File.WriteAllText(_path, updatedJson);

        return products;
    }

    public async Task<Product> PutPrice(string code)
    {
        string json = File.ReadAllText(_path);
        List<Product> products = JsonSerializer.Deserialize<List<Product>>(json);

        Product foundProduct = products.FirstOrDefault(p => p.Code == code);
        if (foundProduct != null)
        {
            foundProduct.Price = await _service.getRandom();

            // Serialize the updated products back to JSON
            string updatedJson = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });

            // Write the updated JSON back to the file
            File.WriteAllText(_path, updatedJson);

            return foundProduct;
        }

        throw new Exception("Error, no se encontró un producto con el code: " + code);
    }
    public Product DeleteByCode(string code)
    {
        if (code.Length != 10)
        {
            throw new Exception("Error, el código no es válido");
        }

        string json = File.ReadAllText(_path);
        List<Product> products = JsonSerializer.Deserialize<List<Product>>(json);

        if (products != null)
        {
            // Find the product with the specified code and remove it from the list
            Product productToRemove = products.FirstOrDefault(p => p.Code == code);
            if (productToRemove != null)
            {
                products.Remove(productToRemove);

                // Serialize the updated list of products back to JSON
                string updatedJson = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });

                // Write the updated JSON back to the file
                File.WriteAllText(_path, updatedJson);

                return productToRemove;
            }
        }

        throw new Exception("Error, no se encontró un producto con el código: " + code);
    }
}