using AutoMapper;
using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEnpoint;
        IConfigHelper _configHelper;
        ISaleEndpoint _saleEndpoint;
        IMapper _mapper;
        StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        public SalesViewModel(IProductEndpoint productEnpoint,
            IConfigHelper configHelper,
            ISaleEndpoint saleEndpoint,
            IMapper mapper,
            StatusInfoViewModel status,
            IWindowManager window)
        {
            _productEnpoint = productEnpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _status = status;
            _window = window;
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if(ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unathorized Access", "You do not have permession");
                    _window.ShowDialogAsync(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal Error", ex.Message);
                    _window.ShowDialogAsync(_status, null, settings);
                }

                TryCloseAsync();
            }
        }
        private async Task LoadProducts()
        {
            var productList = await _productEnpoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set { 
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelecetedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value;
                NotifyOfPropertyChange(() => SelecetedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            await LoadProducts();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        private CartItemDisplayModel _selecetedCartItem;

        public CartItemDisplayModel SelecetedCartItem
        {
            get { return _selecetedCartItem; }
            set
            {
                _selecetedCartItem = value;
                NotifyOfPropertyChange(() => SelecetedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set { _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }


        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public string SubTotal
        {
            get {
                decimal subTotal = CalculateSubTotal();
                return subTotal.ToString("C");
                //return "$0.00"; 
            
            }
        }
        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }
            return subTotal;
        }
        public string Tax
        {
            get {
                decimal taxAmount = CalculateTax();
                return taxAmount.ToString("C");
                //return "$0.00";
            }
        }
        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;
            taxAmount = Cart
                .Where(x=>x.Product.IsTaxable)
                .Sum(x=>x.Product.RetailPrice * x.QuantityInCart * taxRate);
            
            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
            //    }
            //}
            return taxAmount;
        }
        public string Total
        {
            get 
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                return total.ToString("C");
            }
        }


        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                //If something is selected
                //If Item has stock
                if (ItemQuantity > 0 && SelecetedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }
       
        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //If something is selected
                if(Cart.Count > 0)
                {
                    output = true;
                }
                return output;
            }
        }
        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                if (SelecetedCartItem != null && SelecetedCartItem?.QuantityInCart > 0)
                {
                    output = true;
                }

                return output;
            }
        }
        public void RemoveFromCart()
        {
            SelecetedCartItem.Product.QuantityInStock += 1;
            if (SelecetedCartItem.QuantityInCart > 1)
            {
                SelecetedCartItem.QuantityInCart -= 1;
                //SelecetedCartItem.Product.QuantityInStock += 1;
            }
            else
            {
                
                Cart.Remove(SelecetedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        public void AddToCart()
        {
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelecetedProduct);
            if(existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                
                
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelecetedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            
            SelecetedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            //NotifyOfPropertyChange(() => existingItem.DisplayText);

        }

        public async Task CheckOut()
        {
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(sale);

            await ResetSalesViewModel();
        }
    }
}
