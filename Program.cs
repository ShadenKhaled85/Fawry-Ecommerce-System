using System;
using System.Collections.Generic;

namespace FawryECommerceInteractive
{
    interface IShippable
    {
        string GetName();
        double GetWeight();
    }

    class Product : IShippable
    {
        public int Id;
        public string Name;
        public double Price;
        public int Stock;
        public bool IsExpirable;
        public DateTime ExpiryDate;
        public bool IsShippable;
        public double Weight;

        public Product(int id, string name, double price, int stock, bool isExpirable, DateTime expiryDate, bool isShippable, double weight)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
            IsExpirable = isExpirable;
            ExpiryDate = expiryDate;
            IsShippable = isShippable;
            Weight = weight;
        }

        public bool IsExpired() => IsExpirable && DateTime.Now > ExpiryDate;

        public string GetName() => Name;
        public double GetWeight() => Weight;

        public static void Display(List<Product> products)
        {
            Console.WriteLine("\nAvailable Products:");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id}. {p.Name} - ${p.Price} - In Stock: {p.Stock} {(p.IsExpirable ? "(Expirable)" : "")}{(p.IsShippable ? " (Shippable)" : "")}");
            }
        }
    }

    class Customer
    {
        public string Name;
        public double Balance;
        public List<(Product, int)> Cart = new();

        public Customer(string name, double balance)
        {
            Name = name;
            Balance = balance;
        }

        public void ViewCart()
        {
            Console.WriteLine("\nYour Cart:");
            if (Cart.Count == 0) Console.WriteLine("Cart is empty.");
            else
            {
                double total = 0;
                foreach (var (product, qty) in Cart)
                {
                    Console.WriteLine($"{qty}x {product.Name} = ${product.Price * qty}");
                    total += product.Price * qty;
                }
                Console.WriteLine($"Subtotal: ${total}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            List<Product> products = new()
            {
                new(1, "Cheese", 10, 5, true, DateTime.Now.AddDays(3), true, 0.5),
                new(2, "Milk", 8, 10, true, DateTime.Now.AddDays(2), true, 1.0),
                new(3, "TV", 300, 2, false, DateTime.Now, true, 5),
                new(4, "Scratch Card", 20, 5, false, DateTime.Now, false, 0)
            };

            Console.WriteLine("Welcome to Fawry E-Commerce!");
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Customer customer = new(name, 500);

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine($"\nHello {customer.Name}, what would you like to do?");
                Console.WriteLine("1. View Products & Add to Cart");
                Console.WriteLine("2. View Cart");
                Console.WriteLine("3. Checkout");
                Console.WriteLine("4. Exit");
                Console.Write("Your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Product.Display(products);
                        Console.Write("\nEnter product ID to add to cart (or 0 to go back): ");
                        if (int.TryParse(Console.ReadLine(), out int id) && id > 0 && id <= products.Count)
                        {
                            Product selected = products[id - 1];
                            if (selected.IsExpired()) Console.WriteLine("Cannot add expired product.");
                            else
                            {
                                Console.Write("Enter quantity: ");
                                if (int.TryParse(Console.ReadLine(), out int qty) && qty <= selected.Stock)
                                {
                                    customer.Cart.Add((selected, qty));
                                    selected.Stock -= qty;
                                    Console.WriteLine("Added to cart.");
                                }
                                else Console.WriteLine("Invalid quantity.");
                            }
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "2":
                        customer.ViewCart();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "3":
                        Checkout(customer);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "4":
                        exit = true;
                        break;
                }
            }
        }

        static void Checkout(Customer customer)
        {
            if (customer.Cart.Count == 0)
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            double subtotal = 0;
            double shipping = 0;
            foreach (var (product, qty) in customer.Cart)
            {
                subtotal += product.Price * qty;
                if (product.IsShippable)
                    shipping += product.Weight * qty * 30;
            }
            double total = subtotal + shipping;

            if (customer.Balance < total)
            {
                Console.WriteLine($"Insufficient balance. Total: ${total}, Your balance: ${customer.Balance}");
                return;
            }

            customer.Balance -= total;
            Console.WriteLine("\n** Checkout Successful **");
            Console.WriteLine($"Subtotal: ${subtotal}");
            Console.WriteLine($"Shipping: ${shipping}");
            Console.WriteLine($"Total: ${total}");
            Console.WriteLine($"Remaining Balance: ${customer.Balance}");
            customer.Cart.Clear();
        }
    }
}
