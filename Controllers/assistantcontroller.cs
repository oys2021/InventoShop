using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace ISMS.Controllers{
[Route("assistant")]
public class assistantcontroller:Controller{
private readonly Databasehelper _databaseHelper;
public assistantcontroller(Databasehelper databasehelper){
_databaseHelper=databasehelper;
}
[HttpGet("home")]
public IActionResult home(){
    var username = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(username))
    {
        // If no session found, logout the user
        return RedirectToAction("userlogin", "user");
    }

    // Fetch all products and order them by CreatedAt in descending order
    var products = _databaseHelper.getproducts();
    var lastFiveProducts = products.OrderByDescending(p => p.CreatedAt).Take(5).ToList();
    
    // Pass the last 5 products to the ViewData
    ViewData["lastFiveProducts"] = lastFiveProducts;
    
    // Loop through the last 5 products and print their names and creation date to the console
    foreach (var product in lastFiveProducts)
    {
        Console.WriteLine($"Product name - {product.ProductName} - Product Created At: {product.CreatedAt}");
    }
    
    // Fetch sales data and order them by date in descending order
    var sales = _databaseHelper.GetSalesWithItems();
    var lastFiveSales = sales.OrderByDescending(p => p.Date).Take(5).ToList();
    ViewData["lastFiveSales"] = lastFiveSales;
    
    // Pass all products, sales, and users to the view
    ViewData["lastFiveProducts"] = lastFiveProducts;
    ViewData["sales"] = sales;
    ViewData["users"] = _databaseHelper.getUsers();

    // Calculate total quantity in stock
    var totalQuantityInStock = products.Sum(p => p.QuantityInStock);
    ViewData["totalQuantityInStock"] = totalQuantityInStock;

    return View();
}


[HttpGet("productdetails/{id}")]
public IActionResult ProductDetails(int id)
{
    
    var product = _databaseHelper.GetProductById(id);
    
    if (product == null)
    {
        
        TempData["ErrorMessage"] = "Product not found!";
        return RedirectToAction("Index", "Home");
    }
    ViewData["product"]=product;
    return View();
}

[HttpGet("userdetails/{id}")]
public IActionResult userdetails(int id)
{
    
    var user = _databaseHelper.GetUserById(id);
    
    if (user == null)
    {
        
        TempData["ErrorMessage"] = "User not found!";
        return RedirectToAction("Index", "Home");
    }
    ViewData["user"]=user;
    return View();
}



[HttpGet("adduser")]
public IActionResult adduser(){
    var username = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(username))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    var result=_databaseHelper.getRole();
    ViewData["roles"]=result;
    return View();
} 
[HttpPost("adduser")]
public IActionResult adduser(string firstname, string lastname, string username, string phone, string email, int role, string password,IFormFile image_url)
{   

    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    
    var existingUser = _databaseHelper.getUser(username, email,phone);
    if (existingUser != null)
    {
        var result = _databaseHelper.getRole();
        // ViewData["UserExistMessage"] = result;
        TempData["UserExistMessage"] =  $"Username- {existingUser[0].username} or  Email-{existingUser[0].email} or Phone - {existingUser[0].phone} Already Exist!";
        return RedirectToAction("adduser","assistant"); 
    }
    else
    {
        Console.WriteLine($"Creating new user - Role: {role}");
        var result = _databaseHelper.getRole();
        ViewData["roles"] = result;


         string imageFilePath = null;
    if (image_url != null && image_url.Length > 0)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image_url.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image_url.CopyTo(fileStream);
        }
        
        imageFilePath = $"/uploads/{fileName}";
    }

      
        var newresult = _databaseHelper.createuser(username, email, role, firstname, lastname, phone,password,imageFilePath);
        if (newresult)
        {
            TempData["SuccessMessage"] = "User added successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to add User. Please try again.";
        }
        // var newUser = _databaseHelper.getUser(username, email,phone);

        return RedirectToAction("adduser","assistant"); 
        // newlogin

    }
}


[HttpGet("addrole")]
public IActionResult addrole(){
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    var result=_databaseHelper.getRole();
    foreach(var role in result){
        Console.WriteLine(role.id);
        Console.WriteLine(role.rolename);
        Console.WriteLine(role.description);
    }
    return View();
} 

[HttpPost("addrole")]
public IActionResult addrole(string rolename,string description){
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
var result=_databaseHelper.createrole(rolename,description);
if (result){
    ViewData["Message"]="Role Added Succesfully";
    Console.WriteLine($"{ViewData["Message"]}");
    return View();
}
else{
    ViewData["Message"]="Role Addition  failed";
    Console.WriteLine($"{ViewData["Message"]}");
    return View();
}
}

[HttpGet("addcategory")]
public IActionResult addcategory(){
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
return View();
}

[HttpPost("addcategory")]
public IActionResult addcategory(string categoryname){
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }

    bool doesCategoryExist = _databaseHelper.doescategoryexist(categoryname);

    if (doesCategoryExist)
    {
        TempData["ErrorMessage"] = "Category already exists!";
        return RedirectToAction("addcategory");
    }

    // Create the category if it doesn't exist
    bool isSuccess = _databaseHelper.createcategory(categoryname);

    if (isSuccess)
    {
        TempData["SuccessMessage"] = "Category added successfully!";
    }
    else
    {
        TempData["ErrorMessage"] = "Failed to add category. Please try again.";
    }

    return RedirectToAction("addcategory");
}


[HttpGet("categorylist")]
public IActionResult categorylist()
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
        var categories = _databaseHelper.getcategories();
        
         ViewData["categories"]=categories as List<Category>;
        return View(); 
}

[HttpGet("productlist")]
public IActionResult productlist()
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
        var products = _databaseHelper.getproducts();
        
        ViewData["products"]=products as List<Product>;
        return View(); 
}

[HttpGet("userlist")]
public IActionResult userlist()
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
        var users = _databaseHelper.getUsers();

        ViewData["users"]=users as List<User>;
        return View(); 
}


[HttpGet("addproduct")]
        public IActionResult addproduct()
        
        {
        var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
        var categories = _databaseHelper.getcategories();
         ViewData["categories"]=categories as List<Category>;
            return View();
        }

        

        [HttpPost("addproduct")]
        public IActionResult addproduct(string product_name, int category_id, decimal price, int quantity_in_stock, DateTime? expiration_date, IFormFile image_url)
        {
            var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {

            return RedirectToAction("userlogin", "user");
        }
            bool doesProductExist = _databaseHelper.DoesProductExist(product_name);

            if (doesProductExist)
            {
                TempData["ErrorMessage"] = $"Product {product_name} already exists!";
                return RedirectToAction("AddProduct");
            }

              // Check if a file was uploaded
    string imageFilePath = null;
    if (image_url != null && image_url.Length > 0)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image_url.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image_url.CopyTo(fileStream);
        }
        
        imageFilePath = $"/uploads/{fileName}";
    }

            
            bool isSuccess = _databaseHelper.CreateProduct(product_name, category_id, price, quantity_in_stock, expiration_date, imageFilePath);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = $"Product {product_name} added successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add product. Please try again.";
            }

            return RedirectToAction("AddProduct");
        }


[HttpGet("salesList")]
public IActionResult salesList()
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    var sales = _databaseHelper.GetSalesWithItems(); 

    ViewData["sales"] = sales as List<Sale>;
    return View();
}

[HttpGet("sales/{saleId}")]
public IActionResult ViewSaleDetails(int saleId)
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }

    var sale = _databaseHelper.GetSaleById(saleId);

    ViewData["sale"] = sale;

    return View();
}

[HttpGet("addsale")]
        public IActionResult addsale()
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    var products = _databaseHelper.getproducts();  
    ViewData["products"] = products;

    return View();
}

[HttpGet("EditProducts/{productId}")]
 public IActionResult EditProducts(int productId)
    {
        var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
        Console.WriteLine($"my number is {productId}");
        if (productId == 0) 
        {

            return View();
        }
        var product = _databaseHelper.GetProductById(productId);
        var categories = _databaseHelper.getcategories();

        if (product == null)
        {
            return View();
        }

        ViewData["product"] = product;
        ViewData["categories"] = categories as List<Category>;
        return View();
    }


[HttpGet("editcategory/{categoryId}")]
 public IActionResult editcategory(int categoryId)
    {
        var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
        Console.WriteLine($"my number is {categoryId}");
        if (categoryId == 0) 
        {

            return View();
        }
        var category = _databaseHelper.GetCategoryById(categoryId);

        if (category == null)
        {
            return View();
        }

        ViewData["category"] = category;
        return View();
    }

[HttpPost("editcategory/{categoryId}")]
public IActionResult editcategory(int categoryId, IFormCollection form)
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    // Retrieve the existing product based on the productId
    var category = _databaseHelper.GetCategoryById(categoryId);

    if (category == null)
    {
        return NotFound();
    }

    // Manually update the fields with the form values
    category.category_name = form["category_name"];
   

    // Perform the update in the database
    var success = _databaseHelper.UpdateCategory(category);

    if (success)
    {

        return RedirectToAction("categorylist", "assistant");
    }
    else
    {
        ViewData["category"] = category;
        return View(); 
    }
}
[HttpPost("EditProducts/{productId}")]
public IActionResult EditProducts(int productId, IFormFile image_url, IFormCollection form)
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
    {
        // If no session found, logout the user
        return RedirectToAction("userlogin", "user");
    }

    // Get the product by its ID
    var product = _databaseHelper.GetProductById(productId);
    if (product == null)
    {
        return NotFound();
    }

    // Store the original product name for the uniqueness check
    var originalProductName = product.ProductName;

    // Manually update the fields with the form values
    product.ProductName = form["product_name"];
    product.Price = Convert.ToDecimal(form["price"]);
    product.QuantityInStock = Convert.ToInt32(form["quantity_in_stock"]);
    product.CategoryId = Convert.ToInt32(form["category_id"]);

    // Only check for uniqueness if the product name has changed
    if (product.ProductName != originalProductName)
    {
        bool doesProductExist = _databaseHelper.DoesProductExist(product.ProductName);
        if (doesProductExist)
        {
            TempData["ErrorMessage"] = $"Product {product.ProductName} already exists!";
            return RedirectToAction("EditProducts", new { id = productId });
        }
    }

    string imageFilePath = null;
    if (image_url != null && image_url.Length > 0)
    {
        // var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image_url.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", image_url.FileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image_url.CopyTo(fileStream);
        }
        
         product.ImageUrl = "/uploads/" + image_url.FileName;
         Console.WriteLine($"{ product.ImageUrl }");
    }


    // Handle ExpirationDate (assuming it's in "yyyy-MM-dd" format)
    if (!string.IsNullOrEmpty(form["expiration_date"]))
    {
        product.ExpirationDate = DateTime.Parse(form["expiration_date"]);
    }
    else
    {
        product.ExpirationDate = null; // In case it's empty, set it to null
    }
   
    var success = _databaseHelper.UpdateProduct(product);

    if (success)
    {
        // Redirect to the product list or another relevant page after success
        return RedirectToAction("ProductList", "assistant");
    }
    else
    {
        ModelState.AddModelError("", "Error updating product.");
        ViewData["product"] = product;
        ViewData["categories"] = _databaseHelper.getcategories() as List<Category>;
        return View();
    }
}




[HttpGet("editusers/{userId}")]
 public IActionResult editusers(int userId)
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    var user = _databaseHelper.GetUserById(userId);
    var roles = _databaseHelper.getRole() ?? new List<Role>(); // Ensure roles is not null

    if (roles == null || !roles.Any())
{
    Console.WriteLine("No roles found");
}

    if (user == null)
    {
        return View(); // Handle case when user is not found
    }

    ViewData["user"] = user;
    ViewData["role"] = roles;
    return View();
}



[HttpPost("editusers/{userId}")]
public IActionResult editusers(int userId,IFormFile image_url, IFormCollection form)
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    var user = _databaseHelper.GetUserById(userId);
    // Only check for uniqueness if the product name has changed
   

    if (user == null)
    {
        return NotFound();
    }
    user.username = form["user_name"];
    user.email = form["email"];
    user.firstname = form["firstname"];
    user.lastname = form["lastname"];
    user.phone = form["phone"];

    user.role = Convert.ToInt32(form["role"]);

     var originalUsername=user.username;
    var originalEmail=user.email;

    var originalPhone=user.phone;

   if (user.username != originalUsername || user.email != originalEmail || user.phone != originalPhone)
{
    var doesUserExist = _databaseHelper.getUser(user.username, user.email, user.phone);
    if (doesUserExist != null)
    {
        TempData["ErrorMessage"] = $"User with username '{user.username}', email '{user.email}', or phone '{user.phone}' already exists!";
        return RedirectToAction("editusers", new { id = userId });
    }
}
     string imageFilePath = null;
    if (image_url != null && image_url.Length > 0)
    {
        // var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image_url.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", image_url.FileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image_url.CopyTo(fileStream);
        }
        
         user.ImageUrl = "/uploads/" + image_url.FileName;
         Console.WriteLine($"{ user.ImageUrl }");
    }

    var success = _databaseHelper.UpdateUser(user);

    if (success)
    {
        return RedirectToAction("userlist", "assistant");
    }
    else
    {
        ModelState.AddModelError("", "Error updating product.");
        ViewData["user"] = user;
        ViewData["role"] = _databaseHelper.getRole() as List<Role>;
        return View();
    }
}


[HttpPost("AddSale")]
public IActionResult AddSale(Dictionary<int, int> products)
{
    var newusername = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(newusername))
        {
            // If no session found, logout the user
            return RedirectToAction("userlogin", "user");
        }
    decimal totalAmount = 0;
    Console.WriteLine($"All Products: {products}");
    List<SaleItem> saleItems = new List<SaleItem>();
     foreach (var product in products)
    {
        Console.WriteLine($"All Products: {product.Key}");
    }

    // Log the products dictionary to the console for debugging
    foreach (var product in products)
    {
        Console.WriteLine($"Product ID: {product.Key}, Quantity: {product.Value}");

        // Now, you can retrieve the product from the database
        var productFromDb = _databaseHelper.GetProductById(product.Key);
        if (productFromDb != null)
        {
            totalAmount += productFromDb.Price * product.Value;

            saleItems.Add(new SaleItem
            {
                ProductId = product.Key,
                Quantity = product.Value,
            });
        }
        else
        {
            TempData["ErrorMessage"] = $"Product with ID {product.Key} not found.";
            return RedirectToAction("AddSale");
        }
    }
     foreach (var saleItem in saleItems)
    {
        Console.WriteLine($"All SalesItems: {saleItems.Count}");
    }

    bool isSaleCreated = _databaseHelper.CreateSale(DateTime.Now, totalAmount, saleItems);
    if (isSaleCreated)
    {
        TempData["SuccessMessage"] = "Sale added successfully!";
    }
    else
    {
        TempData["ErrorMessage"] = "Failed to add sale. Please try again.";
    }

    return RedirectToAction("AddSale");
}


[HttpGet("UpdateSale/{saleId}")]
public IActionResult UpdateSale(int saleId)
{
    var sale = _databaseHelper.GetSaleById(saleId); // Retrieve the sale to update
    if (sale == null)
    {
        TempData["ErrorMessage"] = "Sale not found!";
        return RedirectToAction("SalesList");
    }

    var products = _databaseHelper.getproducts(); 
    var saleItems = _databaseHelper.GetSaleItemsBySaleId(saleId); 

    ViewData["products"] = products; 
    ViewData["sale"] = sale;  
    ViewData["saleItems"] = saleItems; 

    return View();
}

[HttpPost("UpdateSale/{saleId}")]
public IActionResult UpdateSale(int saleId, Dictionary<int, int> products)
{
    var sale = _databaseHelper.GetSaleById(saleId); 
    if (sale == null)
    {
        TempData["ErrorMessage"] = "Sale not found!";
        return RedirectToAction("SalesList");
    }

    decimal totalAmount = 0;
    List<SaleItem> saleItems = new List<SaleItem>();

    foreach (var product in products)
    {
        var productFromDb = _databaseHelper.GetProductById(product.Key);
        if (productFromDb != null)
        {
            totalAmount += productFromDb.Price * product.Value;
            saleItems.Add(new SaleItem
            {
                ProductId = product.Key,
                Quantity = product.Value,
            });
        }
        else
        {
            TempData["ErrorMessage"] = $"Product with ID {product.Key} not found.";
            return RedirectToAction("UpdateSale", new { saleId });
        }
    }

    bool isSaleUpdated = _databaseHelper.UpdateSale(saleId, DateTime.Now, totalAmount, saleItems);
    if (isSaleUpdated)
    {
        TempData["SuccessMessage"] = "Sale updated successfully!";
    }
    else
    {
        TempData["ErrorMessage"] = "Failed to update sale. Please try again.";
    }

    return RedirectToAction("SalesList");
}



}





}