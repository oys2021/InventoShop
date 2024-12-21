using System;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;



public class Databasehelper{

private readonly string _connectionstring;
private readonly IConfiguration _configuration;

public Databasehelper(string connectionstring,IConfiguration configuration){
    _connectionstring=connectionstring;
    _configuration=configuration;
}



public void SendPasswordToUser(string userEmail, string password,string username)
{
    try
    {
        // Create the email message
        var fromAddress = new MailAddress("yawsarfo2019@domain.com", "ISMS");
        var toAddress = new MailAddress(userEmail);
        string subject = "Your Login Credentials";
        string body = $@"
            Hello,

            Welcome to [ISMS].

            Here are your login credentials:

            Username: {username}
            Password: {password}

            Please log in and change your password immediately.

            Best regards,
            [Your Company Name]
        ";

        // SMTP client configuration
        using (var smtp = new SmtpClient())
        {
            smtp.Host = "smtp.yourprovider.com";  // Use SMTP server of your provider
            smtp.Port = 587;  // Or your provider's SMTP port
            smtp.Credentials = new NetworkCredential("yawsarfo@gmail.com", "yourpassword");
            smtp.EnableSsl = true;

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            // Send email
            smtp.Send(message);
        }

        Console.WriteLine("Password sent successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending email: {ex.Message}");
    }
}





public List<User> getUser(string username,string ? email,string phone)
{
    var newlist = new List<User>();
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = "SELECT * FROM Users WHERE Username = @Username OR Email=@Email OR phone=@phone";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@phone", phone);


        connection.Open();
        using (var result = command.ExecuteReader())
        {
            Console.WriteLine("Query executed. Inspecting results...");

            while (result.Read()) 
            {
                Console.WriteLine("Row data:");
                var user = new User
                {
                    id = result["UserId"] != DBNull.Value ? Convert.ToInt32(result["UserId"]) : 0,
                    username = result["Username"].ToString(),
                    email = result["Email"].ToString(),
                    password = result["PasswordHash"].ToString(),
                    role = result["RoleId"] != DBNull.Value ? Convert.ToInt32(result["RoleId"]) : 0,
                    isActive = result["IsActive"] != DBNull.Value && Convert.ToBoolean(result["IsActive"]),
                    firstname = result["firstname"].ToString(),
                    lastname = result["lastname"].ToString(),
                    phone = result["phone"].ToString(),
                    CreatedAt = result["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(result["CreatedAt"]) : DateTime.MinValue,
                    RequirePasswordChange = result["RequirePasswordChange"] != DBNull.Value && Convert.ToBoolean(result["RequirePasswordChange"]),

                };

                newlist.Add(user);
            }
        }
    }

    return newlist.Count > 0 ? newlist : null;
}


public List<Product> getProduct(string productname)
{
    var newproduct = new List<Product>();
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = "SELECT * FROM Users WHERE product_name = @product_name";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@product_name", productname);



        connection.Open();
        using (var result = command.ExecuteReader())
        {
            Console.WriteLine("Query executed. Inspecting results...");

            while (result.Read()) 
            {
                Console.WriteLine("Row data:");
                var product = new Product
                {
                    ProductId = result["product_id"] != DBNull.Value ? Convert.ToInt32(result["UserId"]) : 0,
                    ProductName = result["product_name"].ToString(),
                   

                };

                newproduct.Add(product);
            }
        }
    }

    return newproduct.Count > 0 ? newproduct : null;
}
public List<User> getUsers()
{
    var newlist = new List<User>();
    using (var connection = new SqlConnection(_connectionstring))
    {
        // var query = "SELECT * FROM Users";
        var query = @"
        SELECT u.UserId,u.Username,u.Email,u.firstname,u.lastname,u.phone,u.RoleId,r.rolename,u.PasswordHash,u.image_url,u.CreatedAt,u.RequirePasswordChange,u.IsActive
        FROM Users u LEFT JOIN ROLES r on u.RoleId = r.RoleId"; 
        var command = new SqlCommand(query, connection);

        connection.Open();
        using (var result = command.ExecuteReader())
        {
            Console.WriteLine("Query executed. Inspecting results...");

            while (result.Read()) 
            {
                Console.WriteLine("Row data:");
                var user = new User
                {
                    id = result["UserId"] != DBNull.Value ? Convert.ToInt32(result["UserId"]) : 0,
                    username = result["Username"].ToString(),
                    email = result["Email"].ToString(),
                    password = result["PasswordHash"].ToString(),
                    role = result["RoleId"] != DBNull.Value ? Convert.ToInt32(result["RoleId"]) : 0,
                    isActive = result["IsActive"] != DBNull.Value && Convert.ToBoolean(result["IsActive"]),
                    firstname = result["firstname"].ToString(),
                    lastname = result["lastname"].ToString(),
                    phone = result["phone"].ToString(),
                    CreatedAt = result["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(result["CreatedAt"]) : DateTime.MinValue,
                    RequirePasswordChange = result["RequirePasswordChange"] != DBNull.Value && Convert.ToBoolean(result["RequirePasswordChange"]),
                    ImageUrl = result["image_url"] != DBNull.Value ? result["image_url"].ToString() : null,
                    Rolename = result["rolename"].ToString(),
                };

                newlist.Add(user);
            }
        }
    }

    
    return newlist;
}
public List<User> getUserbyUsername(string username)
{
    if (string.IsNullOrEmpty(username))
    {
        return null; 
    }

    var newlist = new List<User>();
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = "SELECT * FROM Users WHERE Username = @Username";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username ?? (object)DBNull.Value);

        connection.Open();
        using (var result = command.ExecuteReader())
        {
            Console.WriteLine("Query executed. Inspecting results...");

            while (result.Read())
            {
                Console.WriteLine("Row data:");
                var user = new User
                {
                    id = result["UserId"] != DBNull.Value ? Convert.ToInt32(result["UserId"]) : 0,
                    username = result["Username"].ToString(),
                    email = result["Email"].ToString(),
                    password = result["PasswordHash"].ToString(),
                    role = result["RoleId"] != DBNull.Value ? Convert.ToInt32(result["RoleId"]) : 0,
                    isActive = result["IsActive"] != DBNull.Value && Convert.ToBoolean(result["IsActive"]),
                    firstname = result["firstname"].ToString(),
                    lastname = result["lastname"].ToString(),
                    phone = result["phone"].ToString(),
                    CreatedAt = result["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(result["CreatedAt"]) : DateTime.MinValue,
                    RequirePasswordChange = result["RequirePasswordChange"] != DBNull.Value && Convert.ToBoolean(result["RequirePasswordChange"]),
                };

                newlist.Add(user);
            }
        }
    }

    return newlist.Count > 0 ? newlist : null;  // Return null if no users are found
}



public bool Userexist(string username, string password){
    using(var connection=new SqlConnection(_connectionstring)){
        var query="SELECT * FROM Users WHERE username=@Username OR password=@PasswordHash";
        var command=new SqlCommand(query,connection);
        command.Parameters.AddWithValue("@Username",username);
        command.Parameters.AddWithValue("@PasswordHash",HashPassword(password));

        connection.Open();
        var result=command.ExecuteReader();

        if (result.Read()){
            return true;
        }
    }
    return false;
}


public bool createuser(string username, string email, int role, string firstname, string lastname, string phone, string password, string imageUrl)
{
    try
    {
        // string password = GenerateRandomPassword(12);  // Generate the password
        Console.WriteLine("Generated Password: " + password);
        
        // Insert user into the database
        using (var connection = new SqlConnection(_connectionstring))
        {
            var query = "INSERT INTO Users (Username, Email, PasswordHash, RoleId, IsActive, firstname, lastname, phone,image_url) " +
                        "VALUES(@Username, @Email, @PasswordHash, @RoleId, @IsActive, @firstname, @lastname, @phone,@image_url)";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@PasswordHash", HashPassword(password)); // Store hashed password
            command.Parameters.AddWithValue("@RoleId", role);
            command.Parameters.AddWithValue("@IsActive", 1);
            command.Parameters.AddWithValue("@firstname", firstname);
            command.Parameters.AddWithValue("@lastname", lastname);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@image_url", (object?)imageUrl ?? DBNull.Value);

            connection.Open();
            var result = command.ExecuteNonQuery();

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;  // Database insertion failed
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating user: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        return false; 
    }
}


public string GenerateCategoryCode()
{
    // Query the database for the last category code
    string lastCode = "";
    using (var connection = new SqlConnection(_connectionstring)) 
    {
        connection.Open();
        var command = new SqlCommand("SELECT TOP 1 category_code FROM Categories ORDER BY category_id DESC", connection);
        var result = command.ExecuteScalar();
        lastCode = result?.ToString();
    }

    if (string.IsNullOrEmpty(lastCode))
    {
        return "CT001"; 
    }

    int numericPart = int.Parse(lastCode.Substring(2)); // Assuming the format is CT001
    numericPart++;

    return "CT" + numericPart.ToString("D3"); // D3 ensures it has 3 digits, e.g., CT002
}
public string GenerateProductCode()
{
    // Query the database for the last category code
    string lastCode = "";
    using (var connection = new SqlConnection(_connectionstring)) 
    {
        connection.Open();
        var command = new SqlCommand("SELECT TOP 1 sku FROM Products ORDER BY product_id DESC", connection);
        var result = command.ExecuteScalar();
        lastCode = result?.ToString();
    }

    if (string.IsNullOrEmpty(lastCode))
    {
        return "PT001"; 
    }

    int numericPart = int.Parse(lastCode.Substring(2)); // Assuming the format is CT001
    numericPart++;

    return "PT" + numericPart.ToString("D3"); // D3 ensures it has 3 digits, e.g., CT002
}

public bool createcategory(string categoryname)
{
    var category_code=GenerateCategoryCode();
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = "INSERT INTO Categories (category_name,category_code) " +
                    "VALUES(@category_name,@category_code)";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@category_name", categoryname);
        command.Parameters.AddWithValue("@category_code", category_code);
        
        connection.Open();
        var result = command.ExecuteNonQuery();

        // 
        return result>0;
    }
}

public bool createproduct(
    string productName,
    int? categoryId,
    decimal price,
    int quantityInStock,
    string sku,
    DateTime? expirationDate,
    string? imageUrl)
{
    // var sku=GenerateProductCode();
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = @"
            INSERT INTO Products (product_name, category_id, price, quantity_in_stock, sku, expiration_date, image_url) 
            VALUES (@product_name, @category_id, @price, @quantity_in_stock, @sku, @expiration_date, @image_url)";
        
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@product_name", productName);
        command.Parameters.AddWithValue("@category_id", (object?)categoryId ?? DBNull.Value);
        command.Parameters.AddWithValue("@price", price);
        command.Parameters.AddWithValue("@quantity_in_stock", quantityInStock);
        command.Parameters.AddWithValue("@sku", sku);
        command.Parameters.AddWithValue("@expiration_date", (object?)expirationDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@image_url", (object?)imageUrl ?? DBNull.Value);

        connection.Open();
        var result = command.ExecuteNonQuery();

        return result > 0;
    }
}

 public bool DoesProductExist(string productName)
    {
        bool doesExist = false;
        var query = "SELECT COUNT(1) FROM Products WHERE product_name = @ProductName";

        using (var connection = new SqlConnection(_connectionstring))
        {
            connection.Open();
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductName", productName);
                doesExist = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        return doesExist;
    }

    

 public bool CreateSale(DateTime saleDate, decimal totalAmount, List<SaleItem> saleItems)
{
    // Start a transaction to ensure all data is inserted correctly
    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var saleQuery = "INSERT INTO Sales (SaleDate, TotalAmount) " +
                                "VALUES (@SaleDate, @TotalAmount); SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(saleQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@SaleDate", saleDate);
                    command.Parameters.AddWithValue("@TotalAmount", totalAmount);

                    var saleId = Convert.ToInt32(command.ExecuteScalar());
                    Console.WriteLine($"saleid-------o)))))){saleId}");
                    Console.WriteLine($"saleitems-------o)))))){saleItems}");
                    Console.WriteLine($"Number of sale items: {saleItems.Count}");

                    foreach (var item in saleItems)
                    {
                        Console.WriteLine($"new days-------{item.Quantity}");
                        var saleItemQuery = "INSERT INTO SalesItems (SaleID, product_id, Quantity) " +
                                            "VALUES (@SaleID, @product_id, @Quantity)";

                        using (var itemCommand = new SqlCommand(saleItemQuery, connection, transaction))
                        {
                            itemCommand.Parameters.AddWithValue("@SaleID", saleId);
                            itemCommand.Parameters.AddWithValue("@product_id", item.ProductId);
                            itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);

                            itemCommand.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                // Log the exception (this should be logged properly in your application)
                Console.WriteLine($"SQL Exception: {ex.Message}");
                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                // General catch for other exceptions
                Console.WriteLine($"General Exception: {ex.Message}");
                transaction.Rollback();
                return false;
            }
        }
    }
}


public bool UpdateSale(int saleId, DateTime saleDate, decimal totalAmount, List<SaleItem> saleItems)
{
    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                // Update the sale information
                var saleQuery = "UPDATE Sales SET SaleDate = @SaleDate, TotalAmount = @TotalAmount WHERE SaleID = @SaleID";
                using (var command = new SqlCommand(saleQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@SaleDate", saleDate);
                    command.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    command.Parameters.AddWithValue("@SaleID", saleId);
                    command.ExecuteNonQuery();
                }

                // Delete old sale items (if necessary) to update them
                var deleteSaleItemsQuery = "DELETE FROM SalesItems WHERE SaleID = @SaleID";
                using (var deleteCommand = new SqlCommand(deleteSaleItemsQuery, connection, transaction))
                {
                    deleteCommand.Parameters.AddWithValue("@SaleID", saleId);
                    deleteCommand.ExecuteNonQuery();
                }

                // Insert new sale items
                foreach (var item in saleItems)
                {
                    var saleItemQuery = "INSERT INTO SalesItems (SaleID, product_id, Quantity) VALUES (@SaleID, @product_id, @Quantity)";
                    using (var itemCommand = new SqlCommand(saleItemQuery, connection, transaction))
                    {
                        itemCommand.Parameters.AddWithValue("@SaleID", saleId);
                        itemCommand.Parameters.AddWithValue("@product_id", item.ProductId);
                        itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                        itemCommand.ExecuteNonQuery();
                    }
                }

                // Commit the transaction
                transaction.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                Console.WriteLine($"SQL Exception: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"General Exception: {ex.Message}");
                return false;
            }
        }
    }
}


    public bool CreateProduct(string productName, int categoryId, decimal price, int quantityInStock, DateTime? expirationDate, string imageUrl)
    {
        var sku=GenerateProductCode();
        var query = "INSERT INTO Products (product_name, category_id, price, quantity_in_stock, sku, expiration_date, image_url) " +
                    "VALUES (@ProductName, @CategoryId, @Price, @QuantityInStock, @Sku, @ExpirationDate, @ImageUrl)";

        using (var connection = new SqlConnection(_connectionstring))
        {
            connection.Open();
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@QuantityInStock", quantityInStock);
                command.Parameters.AddWithValue("@Sku", sku);
                command.Parameters.AddWithValue("@ExpirationDate", expirationDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ImageUrl", imageUrl ?? (object)DBNull.Value);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }

public List<Sale> GetSalesWithItems()
{
    var sales = new List<Sale>();

    var query = @"
        SELECT s.SaleID, s.SaleDate, s.TotalAmount, 
               si.SalesItemID, si.product_id, si.Quantity, p.price, p.product_name
        FROM Sales s
        INNER JOIN SalesItems si ON si.SaleID = s.SaleID
        INNER JOIN Products p ON p.product_id = si.product_id";

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Match SaleID with SaleId
                    var sale = sales.FirstOrDefault(x => x.Id == reader.GetInt32(reader.GetOrdinal("SaleID")));
                    if (sale == null)
                    {
                        sale = new Sale
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("SaleID")),  // Map SaleID to Id
                            Date = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                            SaleItems = new List<SaleItem>()
                        };
                        sales.Add(sale);
                    }

                    // Map SalesItemID to Id, and product_id to ProductId
                    sale.SaleItems.Add(new SaleItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("SalesItemID")), // Map SalesItemID to Id
                        ProductId = reader.GetInt32(reader.GetOrdinal("product_id")), // Map product_id to ProductId
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        Product = new Product
                        {
                            ProductName = reader.GetString(reader.GetOrdinal("product_name")), // Map product_name to ProductName
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),  // Map price to Price
                        }
                    });
                }
            }
        }
    }

    return sales;
}

public Sale GetSaleById(int saleId)
{
    var sale = new Sale();

    var query = @"
        SELECT s.SaleID, s.SaleDate, s.TotalAmount, 
               si.SalesItemID, si.product_id, si.Quantity, p.price, p.product_name
        FROM Sales s
        INNER JOIN SalesItems si ON si.SaleID = s.SaleID
        INNER JOIN Products p ON p.product_id = si.product_id
        WHERE s.SaleID = @SaleID"; 

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@SaleID", saleId); // Adding the saleId as a parameter to the query

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    
                    if (sale.Id == 0) 
                    {
                        sale.Id = reader.GetInt32(reader.GetOrdinal("SaleID"));
                        sale.Date = reader.GetDateTime(reader.GetOrdinal("SaleDate"));
                        sale.TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"));
                        sale.SaleItems = new List<SaleItem>();
                    }

                    // Fetch the sale item details
                    sale.SaleItems.Add(new SaleItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("SalesItemID")),
                        ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        Product = new Product
                        {
                            ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price"))
                        }
                    });
                }
            }
        }
    }

    return sale; // Return the sale with its sale items
}public Product GetProductById(int productId)
{
    
    var product = new Product();

    
    var query = @"
        SELECT p.product_id, p.product_name, p.expiration_date,p.price,p.quantity_in_stock,p.sku,p.category_id,p.image_url,c.category_name 
        FROM Products p LEFT JOIN Categories c on p.category_id = c.category_id
        WHERE p.product_id = @product_id"; // Filter by the provided productId

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {
            // Add productId as a parameter to the query to prevent SQL injection
            command.Parameters.AddWithValue("@product_id", productId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read()) // Ensure we only process the first row (if it exists)
                {
                    // Fetch product details
                    product.ProductId = reader.GetInt32(reader.GetOrdinal("product_id"));
                    product.ProductName = reader.GetString(reader.GetOrdinal("product_name"));
                    product.Sku = reader.GetString(reader.GetOrdinal("sku"));

                    product.Price = reader.GetDecimal(reader.GetOrdinal("price"));
                    product.QuantityInStock = reader.GetInt32(reader.GetOrdinal("quantity_in_stock"));
                    product.CategoryName = reader.IsDBNull(reader.GetOrdinal("category_name"))
                        ? null 
                        : reader.GetString(reader.GetOrdinal("category_name"));
                    
                    product.CategoryId = reader.IsDBNull(reader.GetOrdinal("category_id")) 
                        ? (int?)null 
                        : reader.GetInt32(reader.GetOrdinal("category_id"));
                    product.ExpirationDate = reader.IsDBNull(reader.GetOrdinal("expiration_date")) 
                        ? (DateTime?)null 
                        : reader.GetDateTime(reader.GetOrdinal("expiration_date"));

                    product.ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url")) 
                        ? null 
                        : reader.GetString(reader.GetOrdinal("image_url"));
                }
            }
        }
    }

    return product; 
}

public List<SaleItem> GetSaleItemsBySaleId(int saleId)
{
    var saleItems = new List<SaleItem>();

    var query = @"
        SELECT si.SalesItemID, si.SaleID, si.product_id, si.Quantity, 
               p.product_name, p.Price, p.Sku
        FROM dbo.SalesItems si  -- Update to the correct table name SalesItems
        INNER JOIN dbo.Products p ON si.product_id = p.product_id  -- Join with Products table
        WHERE si.SaleID = @SaleID";  // Filter by SaleID

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {
            // Add saleId as a parameter to the query to prevent SQL injection
            command.Parameters.AddWithValue("@SaleID", saleId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read()) // Iterate through all the rows returned by the query
                {
                    var saleItem = new SaleItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("SalesItemID")), // Update to SalesItemID
                        SaleId = reader.GetInt32(reader.GetOrdinal("SaleID")),
                        ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                    };

                    saleItem.Product = new Product
                    {
                        ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                        ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                        Price = reader.GetDecimal(reader.GetOrdinal("price")),
                        Sku = reader.GetString(reader.GetOrdinal("sku"))
                    };

                    saleItems.Add(saleItem);
                }
            }
        }
    }

    return saleItems;
}



public User GetUserById(int userId)
{
    var user = new User();

    var query = @"
        SELECT u.UserId,u.Username,u.Email,u.firstname,u.lastname,u.phone,u.RoleId,r.rolename,u.PasswordHash,u.image_url,u.CreatedAt
        FROM Users u LEFT JOIN ROLES r on u.RoleId = r.RoleId
        WHERE u.UserId = @UserId"; 

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {
            // Add productId as a parameter to the query to prevent SQL injection
            command.Parameters.AddWithValue("@UserId", userId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read()) 
                {
                    user.id = reader.GetInt32(reader.GetOrdinal("UserId"));
                    user.username = reader.GetString(reader.GetOrdinal("Username"));
                    user.password = reader.GetString(reader.GetOrdinal("PasswordHash"));
                    user.email = reader.GetString(reader.GetOrdinal("Email"));
                    user.firstname = reader.GetString(reader.GetOrdinal("firstname"));
                    user.lastname = reader.GetString(reader.GetOrdinal("lastname"));
                    user.phone = reader.GetString(reader.GetOrdinal("phone"));
                    user.role = reader.IsDBNull(reader.GetOrdinal("RoleId")) 
                        ? (int?)null 
                        : reader.GetInt32(reader.GetOrdinal("RoleId"));

                    user.Rolename = reader.IsDBNull(reader.GetOrdinal("rolename"))
                        ? null 
                        : reader.GetString(reader.GetOrdinal("rolename"));
                    
                    user.ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url")) 
                        ? null 
                        : reader.GetString(reader.GetOrdinal("image_url"));

                     user.CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) 
                        ? (DateTime?)null 
                        : reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
                }
            }
        }
        
    }
    return user; 
}
public Category GetCategoryById(int categoryId)
{
    var category = new Category();

    var query = @"
        SELECT category_id, category_name, category_code
        FROM Categories 
        WHERE category_id = @category_id";  // Ensure no use of @category_name here

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {
            // Add categoryId as a parameter to the query to prevent SQL injection
            command.Parameters.AddWithValue("@category_id", categoryId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read()) 
                {
                    category.id = reader.GetInt32(reader.GetOrdinal("category_id"));
                    category.category_name = reader.GetString(reader.GetOrdinal("category_name"));
                    category.category_code = reader.IsDBNull(reader.GetOrdinal("category_code")) ? null : reader.GetString(reader.GetOrdinal("category_code"));
                }
            }
        }
    }

    return category;  
}


public bool UpdateProduct(Product updatedProduct)
{
    var query = @"
        UPDATE Products
        SET product_name = @product_name,
            price = @price,
            quantity_in_stock = @quantity_in_stock,
            category_id = @category_id,
            image_url = @image_url,
            expiration_date = @expiration_date
        WHERE product_id = @product_id";

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@product_name", updatedProduct.ProductName);
            command.Parameters.AddWithValue("@price", updatedProduct.Price);
            command.Parameters.AddWithValue("@quantity_in_stock", updatedProduct.QuantityInStock);
            command.Parameters.AddWithValue("@category_id", updatedProduct.CategoryId);
            command.Parameters.AddWithValue("@expiration_date", updatedProduct.ExpirationDate.HasValue ? (object)updatedProduct.ExpirationDate.Value : DBNull.Value);
            command.Parameters.AddWithValue("@product_id", updatedProduct.ProductId);
            command.Parameters.AddWithValue("@image_url", updatedProduct.ImageUrl);


            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}


public bool UpdateUser(User updatedUser)
{
    var query = @"
        UPDATE Users
        SET Username = @Username,
            Email = @Email,
            RoleId = @RoleId,
            firstname = @firstname,
            lastname = @lastname,
            phone = @phone,
            image_url=@image_url
        WHERE UserId = @UserId";

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserId", updatedUser.id);
            command.Parameters.AddWithValue("@Username", updatedUser.username);
            command.Parameters.AddWithValue("@Email", updatedUser.email);
            command.Parameters.AddWithValue("@RoleId", updatedUser.role);
            command.Parameters.AddWithValue("@firstname", updatedUser.firstname);
            command.Parameters.AddWithValue("@lastname", updatedUser.lastname);           
            command.Parameters.AddWithValue("@phone", updatedUser.phone);
            command.Parameters.AddWithValue("@image_url", updatedUser.ImageUrl);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}
public bool UpdateCategory(Category category)
{
    var query = @"
        UPDATE Categories
        SET category_name = @category_name
        WHERE category_id = @category_id";

    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();

        using (var command = new SqlCommand(query, connection))
        {

            command.Parameters.AddWithValue("@category_name", category.category_name);
            command.Parameters.AddWithValue("@category_id", category.id);

          
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0; 
        }
    }
}


public List<Product> getproducts()
{
    var products = new List<Product>();

    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = @"
            SELECT p.product_id, p.product_name, p.category_id, p.price, p.quantity_in_stock, 
                   p.sku, p.expiration_date, p.created_at, p.image_url, c.category_name
            FROM Products p
            LEFT JOIN Categories c ON p.category_id = c.category_id";
        
        var command = new SqlCommand(query, connection);
        connection.Open();

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var product = new Product
                {
                    ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                    ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                    CategoryId = reader.IsDBNull(reader.GetOrdinal("category_id")) 
                        ? (int?)null 
                        : reader.GetInt32(reader.GetOrdinal("category_id")),
                    CategoryName = reader.IsDBNull(reader.GetOrdinal("category_name"))
                        ? null 
                        : reader.GetString(reader.GetOrdinal("category_name")), // New field
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    QuantityInStock = reader.GetInt32(reader.GetOrdinal("quantity_in_stock")),
                    Sku = reader.GetString(reader.GetOrdinal("sku")),
                    ExpirationDate = reader.IsDBNull(reader.GetOrdinal("expiration_date")) 
                        ? (DateTime?)null 
                        : reader.GetDateTime(reader.GetOrdinal("expiration_date")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url")) 
                        ? null 
                        : reader.GetString(reader.GetOrdinal("image_url"))
                };

                products.Add(product);
            }
        }
    }

    // var lastFiveProducts = products.OrderByDescending(p => p.CreatedAt).Take(5).ToList();
    
    // // Loop through the last 5 products and print their names and creation date to the console
    // foreach (var product in lastFiveProducts)
    // {
    //     Console.WriteLine($"Product name - {product.ProductName} - Product Created At: {product.CreatedAt}");
    // }

    return products;
}
public bool UpdateProduct(
    int id,
    string productName,
    int categoryId,
    decimal price,
    int quantityInStock,
    DateTime? expirationDate,
    string imageUrl)
{
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = @"
            UPDATE Products 
            SET 
                product_name = @product_name,
                category_id = @category_id,
                price = @price,
                quantity_in_stock = @quantity_in_stock,
                expiration_date = @expiration_date,
                image_url = @image_url
            WHERE product_id = @product_id";
        
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@product_id", id);
        command.Parameters.AddWithValue("@product_name", productName);
        command.Parameters.AddWithValue("@category_id", categoryId);
        command.Parameters.AddWithValue("@price", price);
        command.Parameters.AddWithValue("@quantity_in_stock", quantityInStock);
        command.Parameters.AddWithValue("@expiration_date", (object?)expirationDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@image_url", (object?)imageUrl ?? DBNull.Value);

        connection.Open();
        var result = command.ExecuteNonQuery();

        return result > 0;
    }
}


public List<Category> getcategories()
{
    var categories = new List<Category>();

    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = @"
            SELECT category_id, category_name,category_code
            FROM Categories";
        
        var command = new SqlCommand(query, connection);
        connection.Open();

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var category = new Category
                {
                    id = reader.GetInt32(reader.GetOrdinal("category_id")),
                    category_name = reader.GetString(reader.GetOrdinal("category_name")),
                    category_code = reader.GetString(reader.GetOrdinal("category_code"))
                };

                categories.Add(category);
            }
        }
    }

    return categories;
}


public bool doescategoryexist(string categoryName)
{
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = "SELECT COUNT(*) FROM Categories WHERE category_name = @category_name";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@category_name", categoryName);

        connection.Open();
        int count = (int)command.ExecuteScalar();

        return count > 0;
    }
}


public bool createrole(string rolename,string description){
    using(var connection=new SqlConnection(_connectionstring)){
        var query="INSERT INTO Roles(rolename,role_description) VALUES (@rolename,@role_description)";
        var command=new SqlCommand(query,connection);
        command.Parameters.AddWithValue("@rolename",rolename);
        command.Parameters.AddWithValue("@role_description",description);

        connection.Open();
        var result=command.ExecuteNonQuery();
        return result>0;
        }
}

public bool UpdatePassword(int userId, string hashedPassword)
{
    try
    {
        // Use the 'using' block to manage resources
        using (var connection = new SqlConnection(_connectionstring))
        {
            var sqlQuery = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId";
            var command = new SqlCommand(sqlQuery, connection);
            
            command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
            command.Parameters.AddWithValue("@UserId", userId);

            // Open the connection if it's not already open
            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"SQL Exception: {ex.Message}");
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        return false; 
    }
}

  
public List<Role> getRole(){
    var newrole=new List<Role>();
    using (var connection=new SqlConnection(_connectionstring)){
        var query="SELECT * FROM Roles";
        var command=new SqlCommand(query,connection);
        connection.Open();

        var result=command.ExecuteReader();

        while (result.Read()){
            var role= new Role{
            id=result.GetInt32(0),
            rolename=result.GetString(1),
            description=result.GetString(2),
        };
        newrole.Add(role);
        }
    }
    return newrole;
}


 // Method to generate a random password
    private string GenerateRandomPassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=<>?";
        char[] password = new char[length];

        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            for (int i = 0; i < length; i++)
            {
                int randomIndex = randomBytes[i] % validChars.Length;
                password[i] = validChars[randomIndex];
            }
        }

        return new string(password);
    }

 public string HashPassword(string password){
        using(var sha256 =SHA256.Create()){

            var bytes=Encoding.UTF8.GetBytes(password);
            var hash=sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        
    }



    public bool alterPassword(int userId)
{
    using (var connection = new SqlConnection(_connectionstring))
    {
        connection.Open();
        using (var command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = @"
                UPDATE Users
                SET RequirePasswordChange = 0
                WHERE UserId = @UserId";
            command.Parameters.AddWithValue("@UserId", userId);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }


    
}

 
}


