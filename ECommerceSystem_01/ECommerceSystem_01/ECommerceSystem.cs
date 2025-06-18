using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSystem_01
{
    class Program
    {
        private static User[] users;
        private static int maxUsers;
        private static int maxProductsPerSeller;
        private static int maxProductsInCart;
        private static int maxOrdersPerBuyer;
        private static int userCount = 0; // To track the total number of users
        private static int sellerCount = 0;
        private static int buyerCount = 0;
        private static string[] categories = { "kids", "electric", "office", "clothing" };


        static void Main(string[] args)
        {
            Console.Write("Enter the maximum number of users: ");
            maxUsers = int.Parse(Console.ReadLine());
            users = new User[maxUsers];

            Console.Write("Enter the maximum number of products per seller: ");
            maxProductsPerSeller = int.Parse(Console.ReadLine());

            Console.Write("Enter the maximum number of products in a buyer's shopping cart: ");
            maxProductsInCart = int.Parse(Console.ReadLine());

            Console.Write("Enter the maximum number of orders per buyer: ");
            maxOrdersPerBuyer = int.Parse(Console.ReadLine());

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("===== E-Commerce System Menu =====");
                Console.WriteLine("1. Add a buyer");
                Console.WriteLine("2. Add a seller");
                Console.WriteLine("3. Add a product to a seller");
                Console.WriteLine("4. Add a product to a buyer's shopping cart");
                Console.WriteLine("5. Payment of an order for a buyer");
                Console.WriteLine("6. Display details of all buyers");
                Console.WriteLine("7. Display details of all sellers");
                Console.WriteLine("8. Exit");
                Console.Write("Enter your choice: ");

                int choice;
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AddUser(new Buyer(maxProductsInCart, maxOrdersPerBuyer));
                            break;
                        case 2:
                            AddUser(new Seller(maxProductsPerSeller));
                            break;
                        case 3:
                            AddProductToSeller();
                            break;
                        case 4:
                            AddProductToBuyerCart();
                            break;
                        case 5:
                            PaymentForOrder();
                            break;
                        case 6:
                            DisplayUsersDetails<Buyer>();
                            break;
                        case 7:
                            DisplayUsersDetails<Seller>();
                            break;
                        case 8:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please enter a number between 1 and 8.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                Console.WriteLine();
            }
        }

        private static void AddUser(User user)
        {
            if (userCount >= maxUsers)
            {
                Console.WriteLine("Maximum number of users reached.");
                return;
            }

            if (user is Buyer)
            {
                if (buyerCount >= maxUsers / 2)
                {
                    Console.WriteLine("Maximum number of buyers reached.");
                    return;
                }
                buyerCount++;
            }
            else if (user is Seller)
            {
                if (sellerCount >= maxUsers / 2)
                {
                    Console.WriteLine("Maximum number of sellers reached.");
                    return;
                }
                sellerCount++;
            }

            Console.Write("Enter user's username: ");
            string username = Console.ReadLine();
            Console.Write("Enter user's password: ");
            string password = Console.ReadLine();
            Console.Write("Enter user's street name: ");
            string streetName = Console.ReadLine();
            Console.Write("Enter user's building number: ");

            int buildingNumber = int.Parse(Console.ReadLine());
            Console.Write("Enter user's city: ");
            string city = Console.ReadLine();
            Console.Write("Enter user's state: ");
            string state = Console.ReadLine();

            user.Initialize(username, password, new Address(streetName, buildingNumber, city, state));
            users[userCount++] = user; // Store the user and increment the user count

            Console.WriteLine(user.GetType().Name + " added successfully.");
        }

        private static void AddProductToSeller()
        {
            Console.WriteLine("===== Add Product to Seller =====");
            Console.Write("Enter seller's username: ");
            string username = Console.ReadLine();
            Seller seller = FindUser<Seller>(username);
            if (seller != null)
            {
                if (seller.ProductCount >= maxProductsPerSeller)
                {
                    Console.WriteLine("Maximum number of products reached for this seller.");
                    return;
                }

                Console.Write("Enter product name: ");
                string productName = Console.ReadLine();
                Console.Write("Enter product price: ");
                double productPrice = double.Parse(Console.ReadLine());

                Console.WriteLine("Choose category:");
                for (int i = 0; i < categories.Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {categories[i]}");
                }
                Console.Write("Enter the number corresponding to the category: ");
                int categoryChoice = int.Parse(Console.ReadLine());
                if (categoryChoice < 1 || categoryChoice > categories.Length)
                {
                    Console.WriteLine("Invalid category choice. Product not added.");
                    return;
                }
                string category = categories[categoryChoice - 1];

                Console.Write("Contains Special Wrapping? (true/false): ");
                bool specialWrap = bool.Parse(Console.ReadLine());

                if (specialWrap)
                {
                    productPrice += 10.00;
                    seller.AddProductWithWrap(productName, productPrice, category);
                }
                else
                {
                    seller.AddProduct(productName, productPrice, category);
                }

                Console.WriteLine("Product added to seller successfully.");
            }
            else
            {
                Console.WriteLine("Seller not found.");
            }
        }

        private static void AddProductToBuyerCart()
        {
            Console.WriteLine("===== Add Product to Buyer's Shopping Cart =====");
            Console.Write("Enter buyer's username: ");
            string buyerUsername = Console.ReadLine();
            Buyer buyer = FindUser<Buyer>(buyerUsername);
            if (buyer != null)
            {
                if (buyer.CartItemCount >= maxProductsInCart)
                {
                    Console.WriteLine("Maximum number of products reached in the cart for this buyer.");
                    return;
                }

                Console.Write("Enter product name: ");
                string productName = Console.ReadLine();
                Console.Write("Enter seller's username: ");
                string sellerUsername = Console.ReadLine();
                Seller seller = FindUser<Seller>(sellerUsername);
                if (seller != null)
                {
                    if (seller.HasProduct(productName))
                    {
                        Product product = seller.GetProduct(productName);
                        buyer.AddProductToCart(product);
                        Console.WriteLine("Product added to buyer's shopping cart successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Product not found for this seller.");
                    }
                }
                else
                {
                    Console.WriteLine("Seller not found.");
                }
            }
            else
            {
                Console.WriteLine("Buyer not found.");
            }
        }

        private static void PaymentForOrder()
        {
            Console.WriteLine("===== Payment for Order =====");
            Console.Write("Enter buyer's username: ");
            string username = Console.ReadLine();
            Buyer buyer = FindUser<Buyer>(username);
            if (buyer != null)
            {
                if (buyer.OrderCount >= maxOrdersPerBuyer)
                {
                    Console.WriteLine("Maximum number of orders reached for this buyer.");
                    return;
                }

                var order = new Order
                {
                    Products = buyer.ShoppingCart,
                    ProductCount = buyer.CartItemCount,
                    OrderPrice = CalculateOrderPrice(buyer.ShoppingCart),
                    Buyer = buyer
                };

                buyer.AddOrder(order);

                Console.WriteLine("Order payment successful.");
                buyer.ClearShoppingCart();
            }
            else
            {
                Console.WriteLine("Buyer not found.");
            }
        }

        private static void DisplayUsersDetails<T>() where T : User
        {
            Console.WriteLine($"===== {typeof(T).Name}s Details =====");
            for (int i = 0; i < userCount; i++)
            {
                if (users[i] is T user)
                {
                    Console.WriteLine($"{typeof(T).Name} {i + 1}:");
                    Console.WriteLine($"Username: {user.GetUsername()}");
                    Console.WriteLine($"Address: {user.GetResidentialAddress()}");
                    Console.WriteLine();
                }
            }
        }

        private static T FindUser<T>(string username) where T : User
        {
            for (int i = 0; i < userCount; i++)
            {
                if (users[i] is T user && user.GetUsername() == username)
                {
                    return user;
                }
            }
            return null;
        }

        private static double CalculateOrderPrice(Product[] products)
        {
            double totalPrice = 0;
            foreach (var product in products)
            {
                if (product != null)
                    totalPrice += product.GetPrice();
            }
            return totalPrice;
        }
    }

    public class User
    {
        protected string username;
        protected string password;
        protected Address residentialAddress;

        public void Initialize(string username, string password, Address residentialAddress)
        {
            this.username = username;
            this.password = password;
            this.residentialAddress = residentialAddress;
        }

        public string GetUsername()
        {
            return username;
        }

        public string GetResidentialAddress()
        {
            return $"{residentialAddress.streetName}, {residentialAddress.buildingNumber}, {residentialAddress.city}, {residentialAddress.state}";
        }
    }

    public class Buyer : User
    {
        public Product[] ShoppingCart { get; private set; }
        public int CartItemCount { get; private set; }
        private Order[] orderHistory;
        public int OrderCount { get; private set; }
        
        public Buyer(int maxProductsInCart, int maxOrdersPerBuyer)
        {
            ShoppingCart = new Product[maxProductsInCart];
            orderHistory = new Order[maxOrdersPerBuyer];
        }

        public void AddProductToCart(Product product)
        {
            if (CartItemCount < ShoppingCart.Length)
            {
                ShoppingCart[CartItemCount++] = product;
            }
        }
        public void AddOrder(Order order)
        {
            if (OrderCount < orderHistory.Length)
            {
                orderHistory[OrderCount++] = order;
            }
        }

        public void ClearShoppingCart()
        {
            ShoppingCart = new Product[ShoppingCart.Length];
            CartItemCount = 0;
        }
    }    

    public class Seller : User
    {
        public Product[] products;
        public int ProductCount { get; private set; } = 0;

        public Seller(int maxProductsPerSeller)
        {
            products = new Product[maxProductsPerSeller];
        }

        public bool HasProduct(string productName)
        {
            return products.Any(p => p.name == productName);
        }

        public Product GetProduct(string productName)
        {
            return products.FirstOrDefault(p => p.name == productName);
        }

        public void AddProductWithWrap(string productName, double productPrice, string category)
        {
            if (ProductCount < products.Length)
            {
                products[ProductCount++] = new Product(productName, productPrice, category, true);
            }
        }

        public void AddProduct(string productName, double productPrice, string category)
        {
            if (ProductCount < products.Length)
            {
                products[ProductCount++] = new Product(productName, productPrice, category, false);
            }
        }
    }

    public class Product
    {
        public string name;
        private double price;
        private static int counter;
        private int id_product;
        private bool isWrapped;
        private string category;

        public Product(string name)
        {
            this.name = name;
            id_product = ++counter;
        }


        public Product(string name, double price)
        {
            this.name = name;
            this.price = price;
            id_product = ++counter;
        }

        public Product(string name, double price, string category, bool isWrapped)
        {
            this.name = name;
            this.price = price;
            this.category = category;
            this.isWrapped = isWrapped;
            id_product = ++counter;
        }

        public double GetPrice()
        {
            return price;
        }
    }

    public class Address
    {
        public string streetName;
        public int buildingNumber;
        public string city;
        public string state;

        public Address(string streetName, int buildingNumber, string city, string state)
        {
            this.streetName = streetName;
            this.buildingNumber = buildingNumber;
            this.city = city;
            this.state = state;
        }
    }

    public class Order
    {
        public Product[] Products { get; set; }
        public int ProductCount { get; set; }
        public double OrderPrice { get; set; }
        public Buyer Buyer { get; set; }
    }
}
